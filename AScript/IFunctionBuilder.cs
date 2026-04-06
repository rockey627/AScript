using System;

namespace AScript
{
	/// <summary>
	/// 函数构建
	/// </summary>
	public interface IFunctionBuilder
	{
		/// <summary>
		/// 构建
		/// </summary>
		/// <param name="e"></param>
		void Build(FunctionBuildArgs e);
	}
}
