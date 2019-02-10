using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockpileStackLimit
{
	struct Tuple<T1, T2>
	{
		public T1 Var1 { get; set; }
		public T2 Var2 { get; set; }

		public Tuple(T1 var1, T2 var2)
		{
			this.Var1 = var1;
			this.Var2 = var2;
		}
	}

	static class Tuple
	{
		public static Tuple<T1, T2> Create<T1, T2>(T1 var1, T2 var2) => new Tuple<T1, T2>(var1, var2);
	}
}
