using AScript.Exceptions;
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
				error => tcs.TrySetException(new[] { new ScriptCustomException(error) })
			);
			return tcs.Task;
		}

		public static Task<T2> then<T, T2>(Task<T> task, Func<T, T2> onSuccess)
		{
			return task.ContinueWith(t =>
			{
				if (t.Exception == null)
				{
					if (onSuccess != null)
					{
						return onSuccess.Invoke(t.Result);
					}
					//onSuccess?.Invoke(t.Result);
				}
				//return t.Result;
				return default(T2);
			});
		}

		public static Task<T2> then<T, T2>(Task<T> task, Func<T, T2> onSuccess, Func<object, object> onFailed)
		{
			return task.ContinueWith(t =>
			{
				if (t.Exception == null)
				{
					if (onSuccess != null)
					{
						return onSuccess.Invoke(t.Result);
					}
					if (typeof(T2) == typeof(object)) return (T2)(object)t.Result;
					if (typeof(T2) == typeof(T)) return (T2)(object)t.Result;
					return default(T2);
				}
				if (onFailed != null)
				{
					object er = null;
					t.Exception.Handle(e =>
					{
						if (e is ScriptCustomException je)
						{
							var r = onFailed.Invoke(je.Data);
							if (r != null) er = r;
							return r == null;
						}
						else
						{
							var r = onFailed.Invoke(e);
							if (r != null) er = r;
							return r == null;
						}
					});
					if (er != null)
					{
						throw new ScriptCustomException(er);
					}
				}
				return default(T2);
			});
		}

		public static Task<T> @catch<T>(Task<T> task, Func<object, object> onFailed)
		{
			return task.ContinueWith(t =>
			{
				if (t.Exception != null && onFailed != null)
				{
					object er = null;
					t.Exception.Handle(e =>
					{
						if (e is ScriptCustomException je)
						{
							var r = onFailed.Invoke(je.Data);
							if (r != null) er = r;
							return r == null;
						}
						else
						{
							var r = onFailed.Invoke(e);
							if (r != null) er = r;
							return r == null;
						}
					});
					if (er != null)
					{
						throw new ScriptCustomException(er);
					}
				}
				return t.Exception == null ? t.Result : default(T);
			});
		}
	}
}
