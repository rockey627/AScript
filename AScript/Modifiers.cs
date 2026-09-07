using System;

namespace AScript
{
	public class Modifiers
	{
		/// <summary>
		/// 只读
		/// </summary>
		public const int READONLY = 1;
		/// <summary>
		/// 常量
		/// </summary>
		public const int CONST = 2;

		public static bool IsReadOnly(int modifier)
		{
			return (modifier & READONLY) != 0 || (modifier & CONST) != 0;
		}

		public static bool IsConst(int modifier)
		{
			return (modifier & CONST) != 0;
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
