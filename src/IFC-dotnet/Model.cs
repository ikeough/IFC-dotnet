using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections;
using System.ComponentModel;
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
					ConstructRecursive(data, listener.InstanceData, model, 0, data.Id);
				}
			}

			return model;
		}

		private static BaseIfc ConstructRecursive(STEP.InstanceData data, Dictionary<int,STEP.InstanceData> instanceData, Model model, int level, int sid)
		{
			Console.WriteLine($"{sid} : Constructing type {data.Type.Name} with parameters [{string.Join(",",data.Parameters)}]");
			
			for(var i=data.Parameters.Count()-1; i>=0; i--)
			{
				var instData = data.Parameters[i] as STEP.InstanceData;
				if(instData != null)
				{
					var subInstance = ConstructRecursive(instData, instanceData, model, ++level, sid);
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
							Console.WriteLine($"Using existing instance with id, {id}, in {data.Id}");
							data.Parameters[i] = model.Instances[guid];
							continue;
						}
					}

					var subInstance = ConstructRecursive(instanceData[id], instanceData, model, ++level, id);
					data.Parameters[i] = subInstance;
					continue;
				}

				var list = data.Parameters[i] as List<object>;
				if(list != null)
				{
					if(!list.Any())
					{
						break;
					}

					// The parameters will have been stored in a List<object> during parsing.
					// We need to create a List<T> where T is the type expected by the constructor
					// in the STEP file.
					var listType = typeof(List<>);
					var instanceType = data.Constructor.GetParameters()[i].ParameterType.GetGenericArguments()[0];
					var constructedListType = listType.MakeGenericType(instanceType);
					var subInstances = (IList)Activator.CreateInstance(constructedListType);

					foreach(var item in list)
					{
						var id = item as STEP.STEPId;
						if(id != null)
						{
							var subInstance = ConstructRecursive(instanceData[id.Value], instanceData, model, level, sid);

							// The object must be converted to the type expected in the list
							// for Select types, this will be a recursive build of the base select type.
							var convert = Convert(instanceType, subInstance);
							subInstances.Add(convert);
						}
						else
						{
							var subInstance = item;
							var convert = Convert(instanceType, subInstance);
							subInstances.Add(convert);
						}
					}
					// Replace the list of STEPId with a list of instance references.
					data.Parameters[i] = subInstances;
				}
			}

			// Construct the instance, assuming that all required sub-instances
			// have already been constructed.
			for(var i=data.Parameters.Count-1; i>=0; i--)
			{
				if(data.Parameters[i] == null)
				{
					continue;
				}
				
				var pType = data.Parameters[i].GetType();
				var expectedType = data.Constructor.GetParameters()[i].ParameterType;
				
				data.Parameters[i] = Convert(expectedType, data.Parameters[i]);
			}
			var instance = (BaseIfc)data.Constructor.Invoke(data.Parameters.ToArray());
			
			//var instance = (BaseIfc)Activator.CreateInstance(data.Type, BindingFlags. data.Parameters.ToArray());
			if(instanceData.ContainsKey(data.Id))
			{
				// We'll only get here if the instance is not being constructed
				// as a sub-instance.
				instanceData[data.Id].ConstructedGuid = instance.Id;
			}
			
			model.Instances.Add(instance.Id, instance);

			return instance;
		}

		private static object Convert(Type expectedType, object value)
		{
			if(expectedType.IsAssignableFrom(value.GetType()))
			{
				return value;
			}

			var converter = TypeDescriptor.GetConverter(expectedType);
			if(converter != null && converter.CanConvertFrom(value.GetType()))
			{
				//Console.WriteLine($"Converting parameter of type, {value.GetType()}, to type, {expectedType}");
				return converter.ConvertFrom(value);
			}
			else
			{
				throw new Exception($"There was no type converter available to convert from {value.GetType()} to {expectedType}.");
			}
		}
	}
}