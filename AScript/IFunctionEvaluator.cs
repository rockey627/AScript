using System;

namespace AScript
{
	/// <summary>
	/// 函数运算
	/// </summary>
	public interface IFunctionEvaluator
	{
		/// <summary>
		/// 运算
		/// </summary>
		/// <param name="e"></param>
		void Eval(FunctionEvalArgs e);
	}
}
