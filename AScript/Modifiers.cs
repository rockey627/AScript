using System;

namespace AScript
{
	public class Modifiers
	{
		/// <summary>
		/// 常量
		/// </summary>
		public const int CONST = 1;

		public static bool IsConst(int modifier)
		{
			return (modifier & CONST) != 0;
		}

		public static void ThrowIfConst(string name, int modifier)
		{
			if (IsConst(modifier))
			{
				throw new Exceptions.ScriptRuntimeException($"'{name}' is const, can not modify");
			}
		}
	}
}
