using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	/// <summary>
	/// 执行函数
	/// </summary>
	public interface IFunctionEvaluator
	{
		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="e"></param>
		void Eval(FunctionEvalArgs e);
	}

	/// <summary>
	/// 异步执行函数
	/// </summary>
	public interface IAsyncFunctionEvaluator
	{
		/// <summary>
		/// 异步执行
		/// </summary>
		/// <param name="e"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task EvalAsync(FunctionEvalArgs e, CancellationToken cancellationToken = default);
	}
}
