using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AScript.Lang.JavaScript.Functions
{
	/// <summary>
	/// String.fromCharCode(65,66,67...)
	/// </summary>
	public class JavaScriptStringFromCharCodeFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly JavaScriptStringFromCharCodeFunction Instance = new JavaScriptStringFromCharCodeFunction();

		private static readonly MethodInfo Method_StringBuilder_Append_char = typeof(StringBuilder).GetMethod("Append", new[] { typeof(char) });

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args == null || e.Args.Count < 2) return;
			var args = e.BuildArgs();
			if (args[0].Type != typeof(JavaScriptString)) return;

			// 创建 StringBuilder 变量
			var sbVar = Expression.Variable(typeof(StringBuilder), "sb");

			// 构建表达式列表
			var expressions = new List<Expression>(args.Count + 1);
			expressions.Add(Expression.Assign(sbVar, Expression.New(typeof(StringBuilder))));

			// 对每个参数追加 (char)Convert.ToInt32(arg)
			for (int i = 1; i < args.Count; i++)
			{
				var arg = args[i];
				var intArg = Expression.Convert(arg, typeof(int));
				var charArg = Expression.Convert(intArg, typeof(char));
				expressions.Add(Expression.Call(sbVar, Method_StringBuilder_Append_char, charArg));
			}

			// 调用 ToString() 并赋值给 Result
			expressions.Add(Expression.Call(sbVar, ExpressionUtils.Method_Object_ToString));

			// 创建代码块
			var body = Expression.Block(new[] { sbVar }, expressions);
			e.Result = body;
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count < 2) return;
			e.EvalArgs();
			if (!(e.ArgValues[0] is JavaScriptString)) return;

			var sb = new StringBuilder(e.ArgValues.Length);
			for (int i = 1; i < e.ArgValues.Length; i++)
			{
				sb.Append((char)Convert.ToInt32(e.ArgValues[i]));
			}
			e.SetResult(sb.ToString());
		}
	}
}
