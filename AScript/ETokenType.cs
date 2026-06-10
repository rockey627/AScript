using System;

namespace AScript
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum ETokenType
	{
		/// <summary>
		/// 其他，如：换行符、分号、小括号、中括号、大括号等
		/// </summary>
		None,
		/// <summary>
		/// 操作符，如：+ - * / 等
		/// </summary>
		Operator = 1,
		/// <summary>
		/// 数字，如：12.5 12D 0x0A
		/// </summary>
		Number = 2,
		/// <summary>
		/// 字符串，如：'hello' "hello"
		/// </summary>
		String = 4,
		/// <summary>
		/// 单词，如：int name a
		/// </summary>
		Word = 8
	}
}
