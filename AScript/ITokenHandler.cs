using AScript.Syntaxs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	/// <summary>
	/// token处理器
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

	/// <summary>
	/// 异步token处理器
	/// </summary>
	public interface IAsyncTokenHandler
	{
		/// <summary>
		/// 异步token处理
		/// </summary>
		/// <param name="analyzer">语法分析器</param>
		/// <param name="e">当前token、语法树及上下文信息</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task BuildAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken);
	}
}
