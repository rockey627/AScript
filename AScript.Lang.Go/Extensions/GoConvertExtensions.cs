using System;

namespace AScript.Lang.Go.Extensions
{
	/// <summary>
	/// 类型转换
	/// </summary>
	public static class GoConvertExtensions
	{
		public static double float64(object v)
		{
			return Convert.ToDouble(v);
		}
	}
}
