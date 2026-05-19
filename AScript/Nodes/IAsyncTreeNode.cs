using System;
using System.Threading.Tasks;
using System.Threading;

namespace AScript.Nodes
{
	public interface IAsyncTreeNode
	{
		/// <summary>
		/// 异步执行
		/// </summary>
		/// <param name="context"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<object> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default);

		/// <summary>
		/// 异步执行
		/// </summary>
		/// <param name="context"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<EvalResult> Eval2Async(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default);

	}
}
