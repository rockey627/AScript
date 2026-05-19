using System;

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
}
