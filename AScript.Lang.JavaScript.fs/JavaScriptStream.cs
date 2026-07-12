using System;
using System.Collections.Generic;
using System.Text;

namespace AScript.Lang.JavaScript.fs
{
	/// <summary>
	/// js IO流
	/// </summary>
	public abstract class JavaScriptStream : IDisposable
	{
		protected bool _disposed;
		protected Encoding _encoding;
		protected readonly IDictionary<string, object> _options;
		protected readonly Dictionary<string, List<Delegate>> _events = new Dictionary<string, List<Delegate>>(StringComparer.OrdinalIgnoreCase);

		protected JavaScriptStream()
		{
			_encoding = Encoding.UTF8;
		}
		protected JavaScriptStream(IDictionary<string, object> options)
		{
			_options = options;
			if (options != null && options.TryGetValue("encoding", out var encodingOption))
			{
				if (encodingOption is Encoding enc)
				{
					_encoding = enc;
				}
				else if (encodingOption is string ens)
				{
					_encoding = Encoding.GetEncoding(ens);
				}
				else
				{
					_encoding = Encoding.UTF8;
				}
			}
			else
			{
				_encoding = Encoding.UTF8;
			}
		}

		~JavaScriptStream()
		{
			Dispose(false);
		}

		public void setEncoding(string encoding)
		{
			_encoding = Encoding.GetEncoding(encoding);
		}

		public virtual void on(string name, Delegate @event)
		{
			if (!_events.TryGetValue(name, out var list))
			{
				list = new List<Delegate>();
				_events[name] = list;
			}
			list.Add(@event);
		}

		protected virtual void TriggerEvent(string name)
		{
			if (_events.TryGetValue(name, out var list) && list.Count > 0)
			{
				foreach (var item in list)
				{
					DynamicInvoke(item, null);
				}
			}
		}

		protected virtual void TriggerEvent(string name, object data)
		{
			if (_events.TryGetValue(name, out var list) && list.Count > 0)
			{
				var datas = new[] { data };
				foreach (var item in list)
				{
					DynamicInvoke(item, datas);
				}
			}
		}

		private void DynamicInvoke(Delegate del, params object[] datas)
		{
			if (del == null) return;
			int datasLength = datas == null ? 0 : datas.Length;
			var parameters = del.Method.GetParameters();
			int parametersLength = parameters.Length;
			bool hasClosure = false;
			if (parameters[0].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
			{
				hasClosure = true;
				parametersLength -= 1;
			}
			if (parametersLength == 0)
			{
				del.DynamicInvoke();
			}
			else if (parametersLength == datasLength)
			{
				del.DynamicInvoke(datas);
			}
			else
			{
				var args = new object[parametersLength];
				int count;
				if (datasLength > 0)
				{
					count = Math.Min(parametersLength, datasLength);
					Array.Copy(datas, args, count);
				}
				else
				{
					count = 0;
				}
				for (int i = count; i < parametersLength; i++)
				{
					args[i] = ScriptUtils.GetDefaultValue(parameters[i + (hasClosure ? 1 : 0)].ParameterType);
				}
				del.DynamicInvoke(args);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed) return;

			if (disposing)
			{
				// 释放托管资源

			}
			// 释放非托管资源

			this._disposed = true;
		}
	}
}
