using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	/// <summary>
	/// 非泛型方法
	/// </summary>
	public class NonGenericFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public object Target { get; private set; }
		public MethodInfo Method { get; private set; }

		public NonGenericFunction(MethodInfo method)
		{
			this.Method = method;
		}
		public NonGenericFunction(MethodInfo method, object target) : this(method)
		{
			this.Target = target;
		}

		public void Build(FunctionBuildArgs e)
		{
			var exprs = e.BuildArgs();
			var argTypes = exprs?.Select(a => a == null ? typeof(Delegate) : a.Type).ToList();
			if (!ScriptUtils.IsMatchArgTypes(argTypes, this.Method, out var useScriptContext, out var hasClosure, out _))
			{
				return;
			}
			if (exprs != null)
			{
				var argExprs = exprs.ToArray();
				if (useScriptContext)
				{
					if (argExprs == null || argExprs.Length == 0)
					{
						argExprs = new Expression[] { Expression.Constant(e.ScriptContext) };
					}
					else
					{
						var argExprs2 = new Expression[argExprs.Length + 1];
						argExprs2[0] = Expression.Constant(e.ScriptContext);
						Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
						argExprs = argExprs2;
					}
				}
				var parameters = this.Method.GetParameters();
				for (int i = 0; i < exprs.Count; i++)
				{
					var p = parameters[hasClosure ? i + 1 : i];
					var arg = argExprs[i];
					if (arg.Type != p.ParameterType)
					{
						argExprs[i] = Expression.Convert(arg, p.ParameterType);
					}
				}
				exprs = argExprs;
			}
			e.Result = this.Target == null ? Expression.Call(this.Method, exprs) : Expression.Call(Expression.Constant(this.Target), this.Method, exprs);
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs(false);
			if (!ScriptUtils.IsMatchArgTypes(e.ArgTypes, this.Method, out var useScriptContext, out var hasClosure, out _))
			{
				return;
			}
			var result = ScriptUtils.DynamicInvoke(e.Context, this.Method, this.Target, e.ArgValues, e.ArgTypes, useScriptContext, hasClosure);
			e.SetResult(result, this.Method.ReturnType);
		}
	}
}
