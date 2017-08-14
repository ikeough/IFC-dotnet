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
		Dictionary<Guid,IfcBase> Instances{get;set;}

		private Model()
		{
			Instances = new Dictionary<Guid,IfcBase>();
		}

		public static Model FromSTEP(string filePath)
		{
			if(!File.Exists(filePath))
			{
				throw new FileNotFoundException($"The specified IFC STEP file does not exist: {filePath}.");
			}
			
			Model model = new Model();
			
			var tmp = new Dictionary<string,IfcBase>();

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

				var independentInstances = listener.InstanceData.Where(id=>!id.HasDependencies);
				foreach(var id in independentInstances)
				{
					// Find the matching function in the IFC-dotnet assembly.
					Console.WriteLine($"Constructing type {id.Type.Name} with parameters [{string.Join(",",id.Parameters)}]");
					var instance = (IfcBase)Activator.CreateInstance(id.Type,id.Parameters);
					tmp.Add(id.Id.Value, instance);

					// First construct all instances which have no dependencies.
					
					// Then construct all types with only references to resolved types.

					// Then construct remaining types.

					
				}

				// Store instances in the Instances dictionary.
			}

			return model;
		}
	}
}