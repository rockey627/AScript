using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptReadStream : JavaScriptStream
	{
		private string _path;
		private StreamReader _reader;

		public JavaScriptReadStream(string path)
		{
			_path = path;
		}
		public JavaScriptReadStream(string path, IDictionary<string, object> options) : base(options)
		{
			_path = path;
		}

		public override void on(string name, Delegate @event)
		{
			base.on(name, @event);

			// 异步读取文件
			if ("data".Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				StartReadAsync();
			}
		}

		public void pipe(JavaScriptWriteStream stream)
		{
			var buffer = new byte[1024];
			using(var reader = File.OpenRead(_path))
			{
				TriggerEvent("open");
				while (true)
				{
					int count = reader.Read(buffer, 0, buffer.Length);
					if (count == 0) break;
					stream.write(buffer, 0, count);
				}
			}
			TriggerEvent("close");
			stream.end();
		}

		private Task OpenAsync()
		{
			return Task.Run(() =>
			{
				_reader = new StreamReader(_path, _encoding);
				TriggerEvent("open");
			});
		}

		private async Task StartReadAsync()
		{
			if (_reader != null) return;
			// 异步打开文件
			await OpenAsync().ConfigureAwait(false);
			// 异步读取文件
			int size = 1024;
			if (_options.TryGetValue("highWaterMark", out var op))
			{
				if (op is long opl) size = (int)opl;
				else if (op is int opi) size = opi;
				else if (op != null && int.TryParse(op.ToString(), out var opx))
				{
					size = opx;
				}
			}
			char[] buffer = new char[size];
			bool isError = false;
			while (true)
			{
				int count;
				try
				{
					count = await _reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					isError = true;
					TriggerEvent("error", ex.Message);
					break;
				}
				if (count == 0) break;
				TriggerEvent("data", new string(buffer, 0, count));
			}
			if (!isError)
			{
				TriggerEvent("end");
			}
			// 关闭流
			try { _reader.Dispose(); } catch { }
			_reader = null;
			TriggerEvent("close");
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (_reader != null)
			{
				try { _reader.Dispose(); } catch { }
				_reader = null;
				if (disposing)
				{
					TriggerEvent("close");
				}
			}
		}
	}
}
