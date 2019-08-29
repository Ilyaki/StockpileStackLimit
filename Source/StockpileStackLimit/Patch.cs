using Harmony;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace StockpileStackLimit
{
	abstract class Patch
	{
		protected abstract Desc GetDesc();

		//Remember Prefix/Postfix should be public and static! Do not use lambdas

		protected class Desc
		{
			public Type targetType;
			public string targetMethodName;
			public Type[] targetMethodArguments;

			/// <param name="targetType">Use typeof()</param>
			/// <param name="targetMethodName">Null if constructor is desired</param>
			/// <param name="targetMethodArguments">Null if no method abiguity</param>
			public Desc(Type targetType, string targetMethodName, Type[] targetMethodArguments = null)
			{
				this.targetType = targetType;
				this.targetMethodName = targetMethodName;
				this.targetMethodArguments = targetMethodArguments;
			}
		}


		private void ApplyPatch(HarmonyInstance harmonyInstance)
		{
			try
			{
				var patchDescriptor = GetDesc();

				if (patchDescriptor == null) return;

				MethodBase targetMethod = String.IsNullOrEmpty(patchDescriptor.targetMethodName) ?
				
				(MethodBase)patchDescriptor.targetType.GetConstructor(patchDescriptor.targetMethodArguments ?? new Type[0]) :

				targetMethod = patchDescriptor.targetMethodArguments != null ?
					patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, patchDescriptor.targetMethodArguments)
					: patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, ((BindingFlags)62));

			
				harmonyInstance.Patch(targetMethod,
					new HarmonyMethod(GetType().GetMethod("Prefix")),
					new HarmonyMethod(GetType().GetMethod("Postfix")),
					new HarmonyMethod(GetType().GetMethod("Transpiler") ?? GetType().GetMethod("Transpile")));

				#if DEBUG
				Log.Message($"[{Assembly.GetExecutingAssembly().GetName().Name}] Patched {targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
				#endif
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public static void PatchAll(HarmonyInstance harmonyInstance)
		{
			foreach (Type type in (from type in Assembly.GetExecutingAssembly().GetTypes()
								   where type.IsClass && type.BaseType == typeof(Patch)
								   select type))
				((Patch)Activator.CreateInstance(type)).ApplyPatch(harmonyInstance);

		}
	}
}
