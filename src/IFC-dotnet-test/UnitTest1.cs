using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;
using IFC4;
using Newtonsoft.Json;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace test
{
	public class UnitTest1
	{
		private readonly ITestOutputHelper output;

		public UnitTest1(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void SerializeProject()
		{
			var id = new IfcGloballyUniqueId("12345");

			var p1 = new IfcProject(id);
			p1.Name = "Test Project";
			p1.Description = "A test of IFC-dotnet.";
			
			var p2 = JsonConvert.DeserializeObject<IfcProject>(p1.ToJSON());
			Assert.Equal(p1.Name.Value, p2.Name.Value);
			Assert.Equal(p1.Description.Value, p2.Description.Value);

			var wall = new IfcWall(new IfcGloballyUniqueId("wall1"));
		}

		[Fact]
		public void CanOpenSTEPFile()
		{
			var stepPath = "../../../example.ifc";
			using (FileStream fs = new FileStream(stepPath, FileMode.Open))
			{
				var input = new AntlrInputStream(fs);
				var lexer = new STEP.STEPLexer(input);
				var tokens = new CommonTokenStream(lexer);

				var parser = new STEP.STEPParser(tokens);
				parser.BuildParseTree = true;

				var tree = parser.file();
				var walker = new ParseTreeWalker();
				var sb = new StringBuilder();
				var listener = new STEP.STEPListener();
				walker.Walk(listener, tree);

				output.WriteLine("ARGH!!!!!");
				//var outPath = Path.Combine(outputDir, "IFC.cs");
				//File.WriteAllText(outPath,sb.ToString());
			}
		}
	}
}
