using AScript.Exceptions;
using AScript.Nodes;
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// 冒号:操作符
	/// </summary>
	public class LuaColonOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaColonOperator Instance = new LuaColonOperator();

		private static readonly MethodInfo Method_DynamicInvoke = typeof(LuaColonOperator).GetMethod("DynamicInvoke", BindingFlags.Static | BindingFlags.NonPublic);

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is CallFuncNode callFuncNode))
			{
				throw new Exceptions.ScriptRuntimeException($"invalid expression near :");
			}

			var target = e.BuildArgs(0);

			Expression[] args;
			Type[] argTypes;
			if (callFuncNode.Args == null || callFuncNode.Args.Length == 0)
			{
#if NET45
				args = new Expression[0];
#else
				args = Array.Empty<Expression>();
#endif
				argTypes = Type.EmptyTypes;
			}
			else
			{
				args = new Expression[callFuncNode.Args.Length];
				argTypes = new Type[callFuncNode.Args.Length];
				for (int i = 0; i < callFuncNode.Args.Length; i++)
				{
					var arg = callFuncNode.Args[i].Build(e.BuildContext, e.ScriptContext, e.Options);
					args[i] = arg;
					argTypes[i] = arg.Type;
				}
			}

			if (target.Type == typeof(object) || typeof(LuaTable).IsAssignableFrom(target.Type))
			{
				for (int i = 0; i < args.Length; i++)
				{
					var arg = args[i];
					if (arg.Type.IsValueType)
					{
						args[i] = Expression.Convert(arg, typeof(object));
					}
				}
				var argExpr = Expression.NewArrayInit(typeof(object), args);
				e.Result = Expression.Call(Method_DynamicInvoke, new Expression[]
				{
					e.BuildContext.GetScriptContextParameter(),
					target,
					Expression.Constant(callFuncNode.Name),
					argExpr
				});
				return;
			}

			if (e.ScriptContext.IsObjectMemberEnabled(target.Type) ?? true)
			{
				bool useScriptContext = false, hasClosure = false;
				int paramsIndex = 0;
				var method = target.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
					.Where(a => a.Name == callFuncNode.Name && !a.IsGenericMethod)
					.FirstOrDefault(a => ScriptUtils.IsMatchArgTypes(argTypes, a, out useScriptContext, out hasClosure, out paramsIndex));
				if (method != null)
				{
					e.Result = ScriptUtils.BuildInvoke(e.BuildContext, e.ScriptContext, e.Options, method, target, callFuncNode.Args, args, useScriptContext, hasClosure, paramsIndex);
					return;
				}
			}

			//if (typeof(DynamicObject).IsAssignableFrom(target.Type))
			//{
			//	// 动态调用 DynamicObject 的方法
			//	e.Result = ScriptUtils.BuildDynamicObject(e.BuildContext, e.ScriptContext, target, callFuncNode.Name, args);
			//	return;
			//}

			throw new ScriptAnalyzingException($"unknown function: {target.Type}.{callFuncNode.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is CallFuncNode callFuncNode))
			{
				throw new Exceptions.ScriptRuntimeException($"invalid expression near :");
			}
			var target = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
			object[] args;
			if (callFuncNode.Args == null || callFuncNode.Args.Length == 0)
			{
				args = null;
			}
			else
			{
				args = new object[callFuncNode.Args.Length];
				for (int i = 0; i < callFuncNode.Args.Length; i++)
				{
					args[i] = callFuncNode.Args[i].Eval(e.Context, e.Options, e.Control, out _);
				}
			}
			var result = DynamicInvoke(e.Context, target, callFuncNode.Name, args);
			e.SetResult(result);
		}

		private static object DynamicInvoke(ScriptContext context, object target, string name, object[] args)
		{
			if (target is LuaTable table)
			{
				if (args == null || args.Length == 0)
				{
					args = new object[] { target };
				}
				else
				{
					var newArgs = new object[args.Length + 1];
					newArgs[0] = target;
					Array.Copy(args, 0, newArgs, 1, args.Length);
					args = newArgs;
				}
				return ScriptUtils.InvokeDynamicObject(table, name, args);
			}
			Type[] argTypes;
			if (args == null || args.Length == 0)
			{
				argTypes = null;
			}
			else
			{
				argTypes = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					argTypes[i] = args[i]?.GetType();
				}
			}
			var t0 = target.GetType();
			if (context.IsObjectMemberEnabled(t0) ?? true)
			{
				bool useScriptContext = false, hasClosure = false;
				int paramsIndex = 0;
				var method = t0.GetMethods(BindingFlags.Instance | BindingFlags.Public)
					.Where(a => a.Name == name && !a.IsGenericMethod)
					.FirstOrDefault(a => ScriptUtils.IsMatchArgTypes(argTypes, a, out useScriptContext, out hasClosure, out paramsIndex));
				if (method != null)
				{
					return ScriptUtils.DynamicInvoke(context, method, target, args, argTypes, useScriptContext, hasClosure, paramsIndex);
				}
			}
			return context.EvalFunc(name, args, argTypes);
		}
	}
}
