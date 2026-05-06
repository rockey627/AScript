using AScript.Nodes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Functions
{
	/// <summary>
	///
	/// </summary>
	public class WhereFunction : IFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo MethodInfo_IQueryable_Where = typeof(Queryable).GetMethods().First(m => m.Name == "Where" && m.GetParameters().Length == 2);

		public void Build(FunctionBuildArgs e)
		{
		}

		public void Eval(FunctionEvalArgs e)
		{
			var query = e.Args[0].Eval(e.Context, e.Options, e.Control, out var queryType);
			var funcDefine = (DefineFuncNode)e.Args[1];

			// 获取 IQueryable 的元素类型 T
			var elementType = queryType.GetGenericArguments()[0];

			// 获取 funcDefine 的参数信息
			var arg = funcDefine.Args[0];

			// 创建参数表达式
			var paramExpr = Expression.Parameter(elementType, arg.Name);

			// 构建临时上下文
			var tempBuildContext = new BuildContext
			{
				RewriteLocalVariables = false,
				ReturnType = typeof(bool),
				IsMain = true
			};
			tempBuildContext.Parameters[arg.Name] = paramExpr;

			// 构建函数体
			var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
			var body = funcDefine.Body.Build(tempBuildContext, e.Context, funcOptions);

			// 构建 Expression<Func<T, bool>>
			var lambdaExpr = Expression.Lambda(body, paramExpr);

			// 调用 Queryable.Where<T>(query, lambdaExpr)
			var whereMethod = MethodInfo_IQueryable_Where.MakeGenericMethod(elementType);

			var result = whereMethod.Invoke(null, new object[] { query, lambdaExpr });
			e.SetResult(result, queryType);
		}
	}
}
