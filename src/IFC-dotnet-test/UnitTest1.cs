using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;
using IFC4;
using Newtonsoft.Json;

namespace test
{
	public class UnitTest1
	{
		[Fact]
		public void SerializeProject()
		{
			var project = new IfcProject();
			project.Name = new IfcLabel("Test Project");
			project.Description = new IfcText("A test of IFC-dotnet.");
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.All
			};
			var str = JsonConvert.SerializeObject(project, settings);
			Console.WriteLine(str);
		}
	}
}
