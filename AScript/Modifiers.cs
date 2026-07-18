using System;

namespace AScript
{
	public class Modifiers
	{
		/// <summary>
		/// 只读
		/// </summary>
		public const int READONLY = 1;

		public static bool IsReadOnly(int modifier)
		{
			return (modifier & READONLY) != 0;
		}

		public static void ThrowIfReadOnly(string name, int modifier)
		{
			if (IsReadOnly(modifier))
			{
				throw new Exceptions.ScriptRuntimeException($"'{name}' is readonly, can not modify");
			}
		}
	}
}
