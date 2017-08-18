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
	public class SerializationTests
	{
		private readonly ITestOutputHelper output;

		public SerializationTests(ITestOutputHelper output)
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
		public void ExampleModel_Serialize_JSON()
		{
			var stepPath = "../../../example.ifc";
			var model = Model.FromSTEP(stepPath);
			var json = model.ToJSON();
		}

		[Fact]
		public void ExampleModel_Serialize_DOT()
		{
			var stepPath = "../../../example.ifc";
			var model = Model.FromSTEP(stepPath);
			var dot = model.ToDOT();
			Console.WriteLine(dot);
		}

		[Fact]
		public void ExampleModel_Deserialize_STEP()
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			var stepPath = "../../../example.ifc";
			var model = Model.FromSTEP(stepPath);
			sw.Stop();
			Console.WriteLine($"{sw.Elapsed.ToString()} elapsed for reading the model.");

			var walls = model.AllInstancesOfType<IfcWall>();
			Console.WriteLine($"There are {walls.Count()} walls in the model.");

			var windows = model.AllInstancesOfType<IfcWindow>();
			Console.WriteLine($"There are {windows.Count()} windows in the model.");

			var doors = model.AllInstancesOfType<IfcDoor>();
			Console.WriteLine($"There are {doors.Count()} doors in the model.");

			var boundaries = model.AllInstancesOfType<IfcRelSpaceBoundary>();
			foreach(var b in boundaries)
			{
				Console.WriteLine($"The related building element is {b.RelatedBuildingElement.Name}:{b.RelatedBuildingElement.GlobalId.Value.ToString()}");
			}
		}
	}
}
