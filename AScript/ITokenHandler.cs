using AScript.Syntaxs;
using System;

namespace AScript
{
	/// <summary>
	/// 语法分析过程中的token处理器
	/// </summary>
	public interface ITokenHandler
	{
		/// <summary>
		/// token处理
		/// </summary>
		/// <param name="analyzer">语法分析器</param>
		/// <param name="e">当前token、语法树及上下文信息</param>
		void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e);
	}
}
