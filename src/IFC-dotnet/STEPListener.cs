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
			var constructorParams = context.parameter().Select(p=>p.value().GetText());

			Console.WriteLine($"{typeRef}({string.Join(",",constructorParams)}");
		}
	}
}