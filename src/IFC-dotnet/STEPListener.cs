using System;
using System.Collections.Generic;
using System.Linq;

namespace STEP
{
	public class STEPListener : STEPBaseListener
	{
		private string currId;

		Dictionary<string,object> instances = new Dictionary<string, object>();

		public override void EnterData(STEPParser.DataContext context)
		{

		}

		public override void EnterInstance(STEPParser.InstanceContext context)
		{
			currId = context.Id().GetText();
		}

		public override void EnterConstructor(STEPParser.ConstructorContext context)
		{
			//TypeRef '(' parameter? (',' parameter)* ')' ';'
			var typeRef = context.TypeRef().GetText();
			var constructorParams = new List<string>();

			foreach(var p in context.parameter())
			{
				if(p.constructor() != null)
				{
					constructorParams.Add(p.constructor().GetText());
				}
				else if(p.collection() != null)
				{
					constructorParams.Add(p.collection().GetText());
				}
				else if(p.Undefined() != null)
				{
					constructorParams.Add(p.Undefined().GetText());
				}
				else if(p.StringLiteral() != null)
				{
					constructorParams.Add(p.StringLiteral().GetText());
				}
				else if(p.Derived() != null)
				{
					constructorParams.Add(p.Derived().GetText());
				}
				else if(p.Enum() != null)
				{
					constructorParams.Add(p.Enum().GetText());
				}
				else if(p.BoolLogical() != null)
				{
					constructorParams.Add(p.BoolLogical().GetText());
				}
				else if(p.RealLiteral() != null)
				{
					constructorParams.Add(p.RealLiteral().GetText());
				}
				else if(p.AnyString() != null)
				{
					constructorParams.Add(p.AnyString().GetText());
				}
				else if(p.Id() != null)
				{
					constructorParams.Add(p.Id().GetText());
				}
				else if(p.IntegerLiteral() != null)
				{
					constructorParams.Add(p.IntegerLiteral().GetText());
				}
			}
			Console.WriteLine($"{typeRef}({string.Join(",",constructorParams)}");
		}
	}
}