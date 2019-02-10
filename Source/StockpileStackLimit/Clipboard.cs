using RimWorld;

namespace StockpileStackLimit
{
	class Clipboard1 : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(StorageSettingsClipboard), "Copy");

		public static int clipboardLimit = -1;

		public static void Postfix(StorageSettings s)
		{
			clipboardLimit = Limits.GetLimit(s);
		}
	}

	class Clipboard2 : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(StorageSettingsClipboard), "PasteInto");
		
		public static void Postfix(StorageSettings s)
		{
			Limits.SetLimit(s, Clipboard1.clipboardLimit);
		}
	}
}
