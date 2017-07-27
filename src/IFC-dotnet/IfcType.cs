using Newtonsoft.Json;

namespace IFC4
{
	/// <summary>
	/// A type wrapper for IFC.
	/// </summary>
	public class IfcType<T>
	{
		[JsonProperty("value")]
		public T Value{get;set;}
		public IfcType(T value)
		{
			Value = value;
		}

		public static implicit operator IfcType<T>(T value)
		{
			return new IfcType<T>(value);
		}

		public string ToJSON()
		{
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.Objects
			};
			return JsonConvert.SerializeObject(this,settings);
		}
	}
}