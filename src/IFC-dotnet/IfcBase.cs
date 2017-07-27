using Newtonsoft.Json;
using System;

namespace IFC4
{
	public abstract class IfcBase
	{
		[JsonProperty("id")]
		public Guid Id{get;}

		public IfcBase()
		{
			Id = Guid.NewGuid();
		}

		public virtual string ToJSON()
		{
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.Objects
			};
			return JsonConvert.SerializeObject(this);
		}

		public virtual string ToSTEP()
		{
			throw new NotImplementedException();
		}
	}
}