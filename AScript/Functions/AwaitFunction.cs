using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Functions
{
	public class AwaitFunction : IFunctionEvaluator, IAsyncFunctionEvaluator, IFunctionBuilder
	{
		private static readonly MethodInfo Method_Task_Wait = typeof(Task).GetMethod("Wait", Type.EmptyTypes);

		public void Build(FunctionBuildArgs e)
		{
			var value = e.BuildArgs(0);
			if (value.Type.IsAssignableFrom(typeof(Task)))
			{
				if (value.Type.IsGenericType)
				{
					e.Result = Expression.Property(value, "Result");
				}
				else
				{
					e.Result = Expression.Call(value, Method_Task_Wait);
				}
			}
			else
			{
				e.Result = value;
			}
		}

		public void Eval(FunctionEvalArgs e)
		{
			var value = e.Args[0].Eval(e.Context, e.Options, e.Control, out var returnType);
			if (value == null || !(value is Task task))
			{
				e.SetResult(value, returnType);
				return;
			}
			if (returnType.IsGenericType)
			{
				// Task<TResult>
				dynamic t = task;
				var v = t.Result;
				e.SetResult(v, returnType.GetGenericArguments()[0]);
			}
			else
			{
				task.Wait();
				e.SetResult(null, null);
			}
		}

		public async Task EvalAsync(FunctionEvalArgs e, CancellationToken cancellationToken = default)
		{
			var value = e.Args[0].Eval(e.Context, e.Options, e.Control, out var returnType);
			if (value == null || !(value is Task task))
			{
				e.SetResult(value, returnType);
				return;
			}
			await task.ConfigureAwait(false);
			if (returnType.IsGenericType)
			{
				// Task<TResult>
				dynamic t = task;
				var v = t.Result;
				e.SetResult(v, returnType.GetGenericArguments()[0]);
			}
			else
			{
				e.SetResult(null, null);
			}
		}
	}
}
