using System;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptPromiseExtensions
	{
		public static Task<object> new_Promise(Action<Action<object>, Action<object>> callback)
		{
			var tcs = new TaskCompletionSource<object>();
			callback(
				result => tcs.TrySetResult(result),
				error => tcs.TrySetException(new[] { new JavaScriptException(error) })
			);
			return tcs.Task;
		}

		public static Task<T> then<T>(Task<T> task, Action<T> onSuccess)
		{
			return task.ContinueWith(t =>
			{
				if (t.Exception == null)
				{
					onSuccess?.Invoke(t.Result);
				}
				return t.Result;
			});
		}

		public static Task<T> then<T>(Task<T> task, Action<T> onSuccess, Action<object> onFailed)
		{
			return task.ContinueWith(t =>
			{
				if (t.Exception == null)
				{
					onSuccess?.Invoke(t.Result);
				}
				else if (t.Exception.InnerException is JavaScriptException pe)
				{
					onFailed?.Invoke(pe.Data);
				}
				else
				{
					onFailed?.Invoke(t.Exception.InnerException.Message);
				}
				return t.Result;
			});
		}

		public static Task<T> @catch<T>(Task<T> task, Action<object> onFailed)
		{
			return then(task, null, onFailed);
		}
	}
}
