using System;
using System.Linq.Expressions;
using AScript;

namespace AScript.Lang.Go.Operators
{
	/// <summary>
	/// Go语言的通道发送运算符 -&gt;
	/// </summary>
	public class ChannelSendOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly ChannelSendOperator Instance = new ChannelSendOperator();

		public void Build(FunctionBuildArgs e)
		{
			throw new NotImplementedException("channel send requires runtime support");
		}

		public void Eval(FunctionEvalArgs e)
		{
			throw new NotImplementedException("channel send requires runtime support");
		}
	}
}
