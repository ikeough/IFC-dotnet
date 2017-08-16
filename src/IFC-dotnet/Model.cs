using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;

namespace IFC4
{
	public class Model
	{
		Dictionary<Guid,BaseIfc> Instances{get;set;}

		private Model()
		{
			Instances = new Dictionary<Guid,BaseIfc>();
		}

		public static Model FromSTEP(string filePath)
		{
			if(!File.Exists(filePath))
			{
				throw new FileNotFoundException($"The specified IFC STEP file does not exist: {filePath}.");
			}
			
			Model model = new Model();

			using (FileStream fs = new FileStream(filePath, FileMode.Open))
			{
				var input = new AntlrInputStream(fs);
				var lexer = new STEP.STEPLexer(input);
				var tokens = new CommonTokenStream(lexer);

				var parser = new STEP.STEPParser(tokens);
				parser.BuildParseTree = true;

				var tree = parser.file();
				var walker = new ParseTreeWalker();
				
				var listener = new STEP.STEPListener();
				walker.Walk(listener, tree);

				foreach(var data in listener.InstanceData.Values)
				{
					if(data.ConstructedGuid != null && model.Instances.ContainsKey(data.ConstructedGuid))
					{
						// Instance may have been previously constructed as the result
						// of another construction.
						continue;
					}
					ConstructRecursive(data, listener.InstanceData, model, 0);
				}
			}

			return model;
		}

		private static BaseIfc ConstructRecursive(STEP.InstanceData data, Dictionary<int,STEP.InstanceData> instanceData, Model model, int level)
		{
			var indent = string.Join("",Enumerable.Repeat("\t", level));

			Console.WriteLine($"{indent}Constructing type {data.Type.Name} with parameters [{string.Join(",",data.Parameters)}]");
			
			for(var i=data.Parameters.Count()-1; i>=0; i--)
			{
				var instData = data.Parameters[i] as STEP.InstanceData;
				if(instData != null)
				{
					var subInstance = ConstructRecursive(instData, instanceData, model, ++level);
					data.Parameters[i] = subInstance;
					continue;
				}

				var stepId = data.Parameters[i] as STEP.STEPId;
				if(stepId != null)
				{
					var id = stepId.Value;
					var guid = instanceData[id].ConstructedGuid;
					if(guid != null)
					{
						if(model.Instances.ContainsKey(guid))
						{
							Console.WriteLine($"{indent}Using existing instance with id, {id}, in {data.Id}");
							data.Parameters[i] = model.Instances[guid];
							continue;
						}
					}

					var subInstance = ConstructRecursive(instanceData[id], instanceData, model, ++level);
					data.Parameters[i] = subInstance;
					continue;
				}

				var list = data.Parameters[i] as List<object>;
				if(list != null)
				{
					var subInstances = new List<BaseIfc>();
					for(var j=list.Count-1; j>=0;j--)
					{
						var id = list[j] as STEP.STEPId;
						if(id == null)
						{
							throw new Exception($"Encountered a list containing a {list[j].GetType().Name}. Was expecting a STEPId.");
						}
						var subInstance = ConstructRecursive(instanceData[id.Value], instanceData, model, level);
						subInstances.Add(subInstance);
					}
					// Replace the list of STEPId with a list of instance references.
					data.Parameters[i] = subInstances;
				}
			}

			// Construct the instance, assuming that all required sub-instances
			// have already been constructed.
			var instance = (BaseIfc)data.Constructor.Invoke(data.Parameters.ToArray());

			if(instanceData.ContainsKey(data.Id))
			{
				// We'll only get here if the instance is not being constructed
				// as a sub-instance.
				instanceData[data.Id].ConstructedGuid = instance.Id;
			}
			
			model.Instances.Add(instance.Id, instance);

			return instance;
		}
	}
}