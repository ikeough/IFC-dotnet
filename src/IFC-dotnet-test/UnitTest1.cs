using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;
using IFC4;
using Newtonsoft.Json;

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
			var model = Model.FromSTEP(stepPath);

			var walls = model.Instances.Values.Where(v=>v is IfcWall);
			Console.WriteLine($"There are {walls.Count()} walls in the model.");
		}
	}
}
