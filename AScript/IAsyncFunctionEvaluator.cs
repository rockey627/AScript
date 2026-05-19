using System;
using System.Threading.Tasks;
using System.Threading;

namespace AScript
{
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
