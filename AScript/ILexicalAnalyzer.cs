using System;
using System.IO;

namespace AScript
{
	/// <summary>
	/// <para>词法分析器</para>
	/// <para>输入：脚本代码</para>
	/// <para>输出：记号流</para>
	/// </summary>
	public interface ILexicalAnalyzer
	{
		ITokenStream Create(string expression);
		ITokenStream Create(Stream expression, bool autoDisposeStream);
	}
}
