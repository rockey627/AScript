using System;
using System.Collections.Generic;
using System.Text;

namespace AScript
{
	/// <summary>
	/// 编译模式
	/// </summary>
	[Flags]
	public enum ECompileMode
	{
		/// <summary>
		/// 不启用编译
		/// </summary>
		None = 0,
		/// <summary>
		/// 编译函数
		/// </summary>
		Function = 1,
		/// <summary>
		/// 编译循环语句
		/// </summary>
		Loop = 2,
		/// <summary>
		/// 编译所有表达式
		/// </summary>
		All = 0xFF
	}
}
