using Newtonsoft.Json;

namespace IFC4
{
	/// <summary>
	/// A type wrapper for IFC.
	/// </summary>
	public class IfcType<T> : IfcBase
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
	}
}