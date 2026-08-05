using AScript.Nodes;
using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Operators
{
	/// <summary>
	/// 冒号:操作符
	/// </summary>
	public class LuaColonOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly LuaColonOperator Instance = new LuaColonOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is CallFuncNode callFuncNode))
			{
				throw new Exceptions.ScriptRuntimeException($"invalid expression near :");
			}
			var table = e.BuildArgs(0);
			var tableVar = table is ParameterExpression tableParam ? tableParam : Expression.Variable(table.Type);
			Expression[] args;
			if (callFuncNode.Args == null || callFuncNode.Args.Length == 0)
			{
				args = new Expression[] { tableVar };
			}
			else
			{
				args = new Expression[callFuncNode.Args.Length + 1];
				args[0] = tableVar;
				for (int i = 0; i < callFuncNode.Args.Length; i++)
				{
					args[i + 1] = callFuncNode.Args[i].Build(e.BuildContext, e.ScriptContext, e.Options);
				}
			}
			if (table is ParameterExpression)
			{
				e.Result = ScriptUtils.BuildDynamicObject(e.BuildContext, e.ScriptContext, tableVar, callFuncNode.Name, args);
			}
			else
			{
				var assign = Expression.Assign(tableVar, table);
				var invoke = ScriptUtils.BuildDynamicObject(e.BuildContext, e.ScriptContext, tableVar, callFuncNode.Name, args);
				e.Result = Expression.Block(new[] { tableVar }, assign, invoke);
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is CallFuncNode callFuncNode))
			{
				throw new Exceptions.ScriptRuntimeException($"invalid expression near :");
			}
			var table = e.Args[0].Eval(e.Context, e.Options, e.Control, out _);
			object[] args;
			if (callFuncNode.Args == null || callFuncNode.Args.Length == 0)
			{
				args = new object[] { table };
			}
			else
			{
				args = new object[callFuncNode.Args.Length + 1];
				args[0] = table;
				for (int i = 0; i < callFuncNode.Args.Length; i++)
				{
					args[i + 1] = callFuncNode.Args[i].Eval(e.Context, e.Options, e.Control, out _);
				}
			}
			var result = ScriptUtils.InvokeDynamicObject((DynamicObject)table, callFuncNode.Name, args);
			e.SetResult(result);
		}
	}
}
