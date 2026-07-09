using System;
using System.Collections.Generic;
using System.IO;
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
			}
			list.Add(@event);
		}

		protected virtual void TriggerEvent(string name)
		{

		}

		protected virtual void TriggerEvent(string name, object data)
		{

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
