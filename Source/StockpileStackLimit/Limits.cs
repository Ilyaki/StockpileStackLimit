using RimWorld;
using System.Collections.Generic;

namespace StockpileStackLimit
{
	class Limits
	{
		private static Dictionary<StorageSettings, IntHolder> limits = new Dictionary<StorageSettings, IntHolder>();

		public static ref int GetLimit(StorageSettings settings)
		{
			if (!limits.ContainsKey(settings))
				limits.Add(settings, new IntHolder(-1));

			return ref limits[settings].Int;
		}

		public static void SetLimit(StorageSettings settings, int limit)
		{
			if (!limits.ContainsKey(settings))
				limits.Add(settings, new IntHolder(limit));
			else
				limits[settings].Int = limit;
		}

		public static bool HasLimit(StorageSettings settings) => limits.ContainsKey(settings);
	}
}
