using System;
using System.Collections.Generic;
using System.Linq;
using IFC4;

namespace STEP
{
	public class STEPId
	{
		public string Value {get;set;}

		public STEPId(string value)
		{
			Value = value;
		}
	}

	public class InstanceData
	{
		public STEPId Id{get;set;}
		public Type Type{get;set;}
		public List<object> Parameters{get;set;}

		public bool HasDependencies
		{
			get
			{
				return Parameters.Any(p=>p is STEPId);
			}
		}

		public InstanceData(string id, Type type, List<object> parameters)
		{
			Id = new STEPId(id);
			Type = type;
			Parameters = parameters;
		}
	}

	public class STEPListener : STEPBaseListener
	{
		private string currId;
		private IEnumerable<Type> enums;
		private IEnumerable<Type> types;

		private List<InstanceData> instanceData;
		public IEnumerable<InstanceData> InstanceData
		{
			get{return instanceData;}
		}

		public STEPListener()
		{
			instanceData = new List<InstanceData>();

			// Parsing will involve finding many enum values
			// Cache the enum types for lookup during parsing. 
			enums = typeof(IFC4.Model).Assembly.GetTypes().Where(t=>t.IsEnum).ToList();

			types = typeof(IFC4.Model).Assembly.GetTypes().Where(t=>!t.IsEnum).ToList();
		}

		public override void EnterInstance(STEPParser.InstanceContext context)
		{
			currId = context.Id().GetText();
		}

		public override void EnterConstructor(STEPParser.ConstructorContext context)
		{
			instanceData.Add(ParseConstructor(currId, context));
		}

		private InstanceData ParseConstructor(string id, STEPParser.ConstructorContext context)
		{
			var typeName = context.TypeRef().GetText();
			var ifcType = types.FirstOrDefault(t=>t.Name.ToUpper() == typeName);

			if(ifcType == null)
			{
				throw new STEPUnknownTypeException(typeName);
			}

			var constructorParams = new List<object>();

			foreach(var p in context.parameter())
			{
				if(p.constructor() != null)
				{
					constructorParams.Add(ParseConstructor(null, p.constructor()));
				}
				else if(p.collection() != null)
				{
					constructorParams.Add(ParseCollection(p.collection()));
				}
				else if(p.Undefined() != null)
				{
					constructorParams.Add(null);
				}
				else if(p.StringLiteral() != null)
				{
					constructorParams.Add(ParseString(p.StringLiteral().GetText()));
				}
				else if(p.Derived() != null)
				{
					constructorParams.Add(null);
				}
				else if(p.Enum() != null)
				{
					constructorParams.Add(ParseEnum(p.Enum().GetText()));
				}
				else if(p.BoolLogical() != null)
				{
					constructorParams.Add(ParseBoolLogical(p.BoolLogical().GetText()));
				}
				else if(p.RealLiteral() != null)
				{
					constructorParams.Add(ParseReal(p.RealLiteral().GetText()));
				}
				else if(p.AnyString() != null)
				{
					constructorParams.Add(ParseString(p.AnyString().GetText()));
				}
				else if(p.Id() != null)
				{
					constructorParams.Add(ParseId(p.Id().GetText()));
				}
				else if(p.IntegerLiteral() != null)
				{
					constructorParams.Add(int.Parse(p.IntegerLiteral().GetText()));
				}
			}

			// Use the constructor which includes all non-optional parameters.
			var ctor = ifcType.GetConstructors().OrderBy(c=>c.GetParameters().Count()).Last();
			var ctorParams = ctor.GetParameters();

			if(ctorParams.Count() != constructorParams.Count())
			{
				throw new STEPParameterMismatchException(ifcType, ctorParams.Count(), constructorParams.Count());
			}

			return new InstanceData(id, ifcType, constructorParams);
		}

		private bool? ParseBoolLogical(string value)
		{
			var v = TrimDots(value);
			if(v == "T")
			{
				return true;
			}
			if(v == "F")
			{
				return false;
			}
			if(v == "U")
			{
				return null;
			}

			throw new STEPParserException(typeof(bool?), value);
		}

		private STEPId ParseId(string value)
		{
			return new STEPId(value);
		}

		private int ParseInt(string value)
		{
			int result;
			if(!int.TryParse(value, out result))
			{
				throw new STEPParserException(typeof(int), value);
			}
			return result;
		}

		private double ParseReal(string value)
		{
			double result;
			if(!double.TryParse(value, out result))
			{
				throw new STEPParserException(typeof(double), value);
			}
			return result;
		}

		private string TrimQuotes(string value)
		{
			return value.TrimStart('\'').TrimEnd('\'');
		}

		private string ParseString(string value)
		{
			try
			{
				return TrimQuotes(value);
			}
			catch
			{
				throw new STEPParserException(typeof(string), value);
			}
			
		}

		private string TrimDots(string value)
		{
			return value.TrimStart('.').TrimEnd('.');
		}

		private Enum ParseEnum(string value)
		{
			Enum eType = null;
			foreach(var e in enums)
			{
				foreach(var ev in e.GetEnumValues())
				{
					if(ev.ToString() == TrimDots(value))
					{
						return eType;
					}
				}
			}

			throw new STEPParserException(typeof(Enum), value);
		}

		private List<object> ParseCollection(STEPParser.CollectionContext value)
		{
			var result = new List<object>();

			foreach(var cv in value.collectionValue())
			{
				if(cv.Id() != null)
				{
					result.Add(ParseId(cv.Id().GetText()));
				}
				else if(cv.AnyString() != null)
				{
					result.Add(ParseString(cv.AnyString().GetText()));
				}
				else if(cv.StringLiteral() != null)
				{
					result.Add(ParseString(cv.StringLiteral().GetText()));
				}
				else if(cv.IntegerLiteral() != null)
				{
					result.Add(ParseInt(cv.IntegerLiteral().GetText()));
				}
				else if(cv.RealLiteral() != null)
				{
					result.Add(ParseReal(cv.RealLiteral().GetText()));
				}
			}

			return result;
		}
	}
}