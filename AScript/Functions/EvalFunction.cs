using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	public class EvalFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly EvalFunction Instance = new EvalFunction();

		private static readonly MethodInfo Method_Eval = typeof(ScriptEngine).GetMethod("Eval", new[] { typeof(BuildContext), typeof(ScriptContext), typeof(BuildOptions), typeof(string), typeof(int), typeof(string), typeof(string) });

		public void Build(FunctionBuildArgs e)
		{
			int argsCount = e.GetArgsCount();
			if (argsCount == 0 || argsCount > 4) return;

			var engine = ScriptEngine.GetCurrent(e.ScriptContext);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			Expression expressionExpr, cacheTimeExpr, cacheKeyExpr, cacheVersionExpr;
			// 
			expressionExpr = e.BuildArgs(0);
			// 
			if (argsCount >= 2) cacheTimeExpr = e.BuildArgs(1);
			else cacheTimeExpr = ExpressionUtils.Constant_zero;
			// 
			if (argsCount >= 3) cacheKeyExpr = e.BuildArgs(2);
			else cacheKeyExpr = ExpressionUtils.Constant_string_empty;
			// 
			if (argsCount >= 4) cacheVersionExpr = e.BuildArgs(3);
			else cacheVersionExpr = ExpressionUtils.Constant_string_empty;
			// 
			if (expressionExpr is ConstantExpression expressionConstantExpression
				&& cacheTimeExpr is ConstantExpression cacheTimeConstantExpression
				&& (int)cacheTimeConstantExpression.Value == 0)
			{
				string expr = (string)expressionConstantExpression.Value;
				var node = ((Script)engine).BuildNode(e.BuildContext, e.ScriptContext, expr);
				e.Result = node.Build(e.BuildContext, e.ScriptContext, e.Options);
				return;
			}
			e.Result = Expression.Call(Expression.Constant(engine), Method_Eval, Expression.Constant(e.BuildContext), Expression.Constant(e.ScriptContext), Expression.Constant(e.Options), expressionExpr, cacheTimeExpr, cacheKeyExpr, cacheVersionExpr);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args == null || e.Args.Count == 0 || e.Args.Count > 4) return;

			var expressionObj = e.Args[0].Eval(e.Context, e.Options, e.Control, out var expressionType);
			if (expressionType != typeof(string)) return;

			string expression = (string)expressionObj;
			if (string.IsNullOrEmpty(expression))
			{
				e.SetResult(null);
				return;
			}
			if (e.Args.Count == 1)
			{
				e.SetResult(Eval(e.Context, e.Options, expression));
				return;
			}

			var cacheTimeObj = e.Args[1].Eval(e.Context, e.Options, e.Control, out var cacheTimeType);
			if (!ScriptUtils.IsIntegerType(cacheTimeType)) return;
			int cacheTime = Convert.ToInt32(cacheTimeObj);
			if (e.Args.Count == 2)
			{
				e.SetResult(Eval(e.Context, e.Options, expression, cacheTime));
				return;
			}

			var cacheKeyObj = e.Args[2].Eval(e.Context, e.Options, e.Control, out var cacheKeyType);
			if (cacheKeyType != typeof(string)) return;
			string cacheKey = (string)cacheKeyObj;
			if (e.Args.Count == 3)
			{
				e.SetResult(Eval(e.Context, e.Options, expression, cacheTime, cacheKey));
				return;
			}

			var cacheVersionObj = e.Args[3].Eval(e.Context, e.Options, e.Control, out var cacheVersionType);
			if (cacheVersionType != typeof(string)) return;
			string cacheVersion = (string)cacheVersionObj;
			e.SetResult(Eval(e.Context, e.Options, expression, cacheTime, cacheKey, cacheVersion));
		}

		private static object Eval(ScriptContext context, BuildOptions options, string expression)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(null, context, options, expression);
		}

		private static object Eval(ScriptContext context, BuildOptions options, string expression, int cacheTime)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(null, context, options, expression, cacheTime);
		}

		private static object Eval(ScriptContext context, BuildOptions options, string expression, int cacheTime, string cacheKey)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression, cacheTime, cacheKey);
		}

		private static object Eval(ScriptContext context, BuildOptions options, string expression, int cacheTime, string cacheKey, string cacheVersion)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(null, context, options, expression, cacheTime, cacheKey, cacheVersion);
		}
	}
}
