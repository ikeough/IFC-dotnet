using System;
using System.ComponentModel;
using System.Globalization;

namespace IFC4
{
	public class SelectConverter <TSelect>: TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			var t = FindChoiceWhichCanConvertType(typeof(TSelect), sourceType);
			return t != null;
		}

		private Type FindChoiceWhichCanConvertType(Type select, Type target)
		{
			var args = select.BaseType.GetGenericArguments();
			foreach(var t in args)
			{
				if(t.IsAssignableFrom(target))
				{
					return t;
				}

				if(typeof(Select).IsAssignableFrom(t))
				{
					var test = FindChoiceWhichCanConvertType(t, target);
					if(test != null)
					{
						return test;
					}
				}
			}
			return null;
		}

		private object ReconstructSelectFromLeaf(Type select, object value)
		{
			// If the select has a constructor which takes 
			// an instance of value, then use it and return
			try
			{
				return Activator.CreateInstance(select, new object[]{value});
			}
			catch
			{	
				//Console.WriteLine($"Could not construct instance of {select.Name} using a parameter of type {value.GetType()}");
			}

			foreach(var t in select.BaseType.GetGenericArguments())
			{
				if(t.IsAssignableFrom(typeof(Select)))
				{
					var result = ReconstructSelectFromLeaf(t, value);
					if(result != null)
					{
						return Activator.CreateInstance(select, new object[]{result});
					}
				}
			}

			return null;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return (TSelect)ReconstructSelectFromLeaf(typeof(TSelect), value);
		}
	}
}