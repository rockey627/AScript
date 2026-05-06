using System.Collections;
using System.Linq;
using System.Reflection;

namespace AScript.Functions
{
	public class AsQueryableFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly AsQueryableFunction Instance = new AsQueryableFunction();

		private static readonly MethodInfo MethodInfo_AsQueryable = typeof(Queryable).GetMethods().First(m => m.Name == "AsQueryable" && m.GetParameters().Length == 1);

		public void Build(FunctionBuildArgs e)
		{
		}

		public void Eval(FunctionEvalArgs e)
		{
			var list = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type);
			if (list is IEnumerable)
			{
				var elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
				var asQueryableMethod = MethodInfo_AsQueryable.MakeGenericMethod(elementType);
				var result = asQueryableMethod.Invoke(null, new[] { list });
				e.SetResult(result, typeof(IQueryable<>).MakeGenericType(elementType));
			}
		}
	}
}
