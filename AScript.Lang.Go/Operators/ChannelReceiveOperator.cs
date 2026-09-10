using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Operators
{
	/// <summary>
	/// Go语言的通道接收运算符 &lt;-
	/// </summary>
	public class ChannelReceiveOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ChannelReceiveOperator Instance = new ChannelReceiveOperator();

		public void Build(FunctionBuildArgs e)
		{
			throw new NotImplementedException("channel receive requires runtime support");
		}

		public void Eval(FunctionEvalArgs e)
		{
			throw new NotImplementedException("channel receive requires runtime support");
		}
	}
}
