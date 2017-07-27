using System;
using System.Transactions;
using System.Collections.Generic;

namespace IFC4
{
	public interface IModel
	{
		IEnumerable<object> Instances{get;set;}
		void Open(string path);
	}
}