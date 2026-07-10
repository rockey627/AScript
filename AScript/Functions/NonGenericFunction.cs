using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AScript.Nodes;

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
			if (!ScriptUtils.IsMatchArgTypes(exprs, this.Method, out var useScriptContext, out var hasClosure, out var paramsIndex))
			{
				return;
			}
			var argExprs = exprs is Expression[] arr ? arr : exprs?.ToArray();
			e.Result = ScriptUtils.BuildInvoke(e.BuildContext, e.ScriptContext, e.Options, this.Method, this.Target, e.Args, argExprs, useScriptContext, hasClosure, paramsIndex);
			//if (exprs != null)
			//{
			//	var argExprs = exprs.ToArray();
			//	if (useScriptContext)
			//	{
			//		if (argExprs == null || argExprs.Length == 0)
			//		{
			//			argExprs = new Expression[] { Expression.Constant(e.ScriptContext) };
			//		}
			//		else
			//		{
			//			var argExprs2 = new Expression[argExprs.Length + 1];
			//			argExprs2[0] = Expression.Constant(e.ScriptContext);
			//			Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
			//			argExprs = argExprs2;
			//		}
			//	}
			//	var parameters = this.Method.GetParameters();
			//	if (paramsIndex >= 0)
			//	{
			//		var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
			//		var paramsExprs = new Expression[argExprs.Length - paramsIndex];
			//		Array.Copy(argExprs, paramsIndex, paramsExprs, 0, paramsExprs.Length);
			//		for (int i = 0; i < paramsExprs.Length; i++)
			//		{
			//			var p = paramsExprs[i];
			//			if (p.Type != itemType)
			//			{
			//				paramsExprs[i] = Expression.Convert(p, itemType);
			//			}
			//		}
			//		var paramsArr = Expression.NewArrayInit(itemType, paramsExprs);
			//		var newExprs = new Expression[paramsIndex + 1];
			//		Array.Copy(argExprs, 0, newExprs, 0, paramsIndex);
			//		newExprs[paramsIndex] = paramsArr;
			//		argExprs = newExprs;
			//	}
			//	for (int i = 0; i < argExprs.Length; i++)
			//	{
			//		var p = parameters[hasClosure ? i + 1 : i];
			//		var arg = argExprs[i];
			//		if (arg == null && typeof(Delegate).IsAssignableFrom(p.ParameterType))
			//		{
			//			var invokeMethod = p.ParameterType.GetMethod("Invoke");
			//			var ps = invokeMethod.GetParameters();
			//			var defineFuncNode = (DefineFuncNode)e.Args[i];
			//			for (int j = 0; j < ps.Length; j++)
			//			{
			//				defineFuncNode.Args[j].SystemType = ps[j].ParameterType;
			//			}
			//			defineFuncNode.ReturnSystemType = invokeMethod.ReturnType;
			//			argExprs[i] = arg = e.Args[i].Build(e.BuildContext, e.ScriptContext, e.Options);
			//		}
			//		if (arg.Type != p.ParameterType)
			//		{
			//			argExprs[i] = Expression.Convert(arg, p.ParameterType);
			//		}
			//	}
			//	exprs = argExprs;
			//}
			//e.Result = this.Target == null ? Expression.Call(this.Method, exprs) : Expression.Call(Expression.Constant(this.Target), this.Method, exprs);
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs(false);
			if (!ScriptUtils.IsMatchArgTypes(e.ArgTypes, this.Method, out var useScriptContext, out var hasClosure, out var paramsIndex))
			{
				return;
			}
			//var argValues = e.ArgValues;
			//var argTypes = e.ArgTypes;
			//if (argValues != null && argValues.Length > 0)
			//{
			//	ParameterInfo[] parameters = null;
			//	for (int i = 0; i < argValues.Length; i++)
			//	{
			//		var arg = argValues[i];
			//		if (ScriptUtils.IsDefineFuncNode(arg))
			//		{
			//			if (parameters == null) parameters = this.Method.GetParameters();
			//			argValues[i] = ScriptUtils.TryParseDelegateArg(e.Context, e.Options, e.Control, arg, parameters[i].ParameterType);
			//		}
			//	}
			//}
			//if (paramsIndex >= 0)
			//{
			//	var parameters = this.Method.GetParameters();
			//	var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
			//	var paramsValues = new object[argValues.Length - paramsIndex];
			//	Array.Copy(argValues, paramsIndex, paramsValues, 0, paramsValues.Length);
			//	var paramsArr = Array.CreateInstance(itemType, paramsValues.Length);
			//	for (int i = 0; i < paramsValues.Length; i++)
			//	{
			//		paramsArr.SetValue(System.Convert.ChangeType(paramsValues[i], itemType), i);
			//	}
			//	var newValues = new object[paramsIndex + 1];
			//	var newTypes = new Type[newValues.Length];
			//	Array.Copy(argValues, 0, newValues, 0, paramsIndex);
			//	Array.Copy(argTypes, 0, newTypes, 0, paramsIndex);
			//	newValues[paramsIndex] = paramsArr;
			//	newTypes[paramsIndex] = paramsArr.GetType();
			//	argValues = newValues;
			//	argTypes = newTypes;
			//}
			//var result = ScriptUtils.DynamicInvoke(e.Context, this.Method, this.Target, argValues, argTypes, useScriptContext, hasClosure);
			var result = ScriptUtils.DynamicInvoke(e.Context, e.Options, e.Control, this.Method, this.Target, e.ArgValues, e.ArgTypes, useScriptContext, hasClosure, paramsIndex);
			e.SetResult(result, this.Method.ReturnType);
		}
	}
}
