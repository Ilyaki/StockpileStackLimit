using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace StockpileStackLimit
{
	class ThingFilterUIWindowPatcher : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(ThingFilterUI), "DoThingFilterConfigWindow");

		static string buffer = "";
		static StorageSettings oldSettings = null;

		public static void Prefix(ref Rect rect)
		{
			ITab_Storage tab = ITab_StorageFillTabsPatcher.currentTab;
			if (tab == null)
				return;
			
			rect.yMin += 32f;
		}

		public static void Postfix(ref Rect rect)
		{
			ITab_Storage tab = ITab_StorageFillTabsPatcher.currentTab;
			if (tab == null)
				return;

			IStoreSettingsParent storeSettingsParent = (IStoreSettingsParent)typeof(ITab_Storage).GetProperty("SelStoreSettingsParent", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(tab, new object[0]);
			StorageSettings settings = storeSettingsParent.GetStoreSettings();

			int limit = Limits.GetLimit(settings);
			bool hasLimit = limit != -1;
			
			Widgets.CheckboxLabeled(new Rect(rect.xMin, rect.yMin - 24f - 3f - 32f, rect.width /2, 24f), "Total stack limit", ref hasLimit);

			if (hasLimit)
			{
				if (oldSettings != settings)
					buffer = limit.ToString();

				Widgets.TextFieldNumeric<int>(new Rect(rect.xMin + (rect.width / 2) + 60f, rect.yMin - 24f - 3f - 32f, rect.width / 2 - 60f, 24f), ref limit, ref buffer, 0, 9999);
			}

			Limits.SetLimit(settings, hasLimit ? limit : -1);

			oldSettings = settings;
		}
	}

	class ITab_StorageFillTabsPatcher : Patch
	{
		public static ITab_Storage currentTab = null;

		protected override Desc GetDesc() => new Desc(typeof(ITab_Storage), "FillTab");

		public static void Prefix(ITab_Storage __instance)
		{
			currentTab = __instance;
		}

		public static void Postfix()
		{
			currentTab = null;
		}
	}
}
