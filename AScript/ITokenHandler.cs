using AScript.Syntaxs;
using System;

namespace AScript
{
	/// <summary>
	/// 语法分析过程中的记号处理器
	/// </summary>
	public interface ITokenHandler
	{
		/// <summary>
		/// 记号处理
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e);
	}
}
