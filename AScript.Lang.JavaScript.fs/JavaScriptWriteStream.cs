using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptWriteStream : JavaScriptStream
	{
		private string _path;
		private Stream _stream;

		public JavaScriptWriteStream(string path) : base()
		{
			_path = path;
		}
		public JavaScriptWriteStream(string path, IDictionary<string, object> options) : base(options)
		{
			_path = path;
		}

		//public bool write(long value)
		//{
		//	return write(value.ToString());
		//}

		//public bool write(double value)
		//{
		//	return write(value.ToString());
		//}

		//public bool write(bool value)
		//{
		//	return write(value.ToString());
		//}

		//public bool write(DateTime value)
		//{
		//	return write(value.ToString());
		//}

		public bool write(string value)
		{
			if (string.IsNullOrEmpty(value)) return true;
			return write(_encoding.GetBytes(value));
		}

		public bool write(IList<byte> value)
		{
			if (value == null || value.Count == 0) return true;
			if (value is byte[] arr)
			{
				return write(arr, 0, arr.Length);
			}
			else
			{
				return write(value.ToArray(), 0, value.Count);
			}
		}

		public bool write(object value)
		{
			if (value == null) return true;
			if (value is IList<byte> bytes)
			{
				return write(bytes);
			}
			var typeCode = Type.GetTypeCode(value.GetType());
			if (typeCode == TypeCode.Object)
			{
				return write(JsonConvert.SerializeObject(value));
			}
			return write(value.ToString());
		}

		public bool write(byte[] value, int start, int count)
		{
			WriteSync(value, start, count);
			return true;
		}

		private void OpenSync()
		{
			_stream = File.Open(_path, FileMode.Create, FileAccess.Write);
			TriggerEvent("open");
		}

		private void WriteSync(byte[] value, int start, int count)
		{
			if (_stream == null) OpenSync();
			try
			{
				_stream.Write(value, start, count);
			}
			catch (Exception ex)
			{
				TriggerEvent("error", ex.Message);
			}
		}

		private Task OpenAsync()
		{
			return Task.Run(OpenSync);
		}

		private async Task WriteAsync(byte[] value, int start, int count)
		{
			if (_stream == null)
			{
				await OpenAsync().ConfigureAwait(false);
			}
			try
			{
				await _stream.WriteAsync(value, start, count).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				TriggerEvent("error", ex.Message);
			}
		}

		public void end()
		{
			TriggerEvent("finish");
			Dispose();
		}

		//public void end(long value)
		//{
		//	write(value);
		//	end();
		//}

		//public void end(double value)
		//{
		//	write(value);
		//	end();
		//}

		//public void end(bool value)
		//{
		//	write(value);
		//	end();
		//}

		//public void end(DateTime value)
		//{
		//	write(value);
		//	end();
		//}

		public void end(string value)
		{
			write(value);
			end();
		}

		public void end(IList<byte> value)
		{
			write(value);
			end();
		}

		public void end(object value)
		{
			write(value);
			end();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_stream != null)
			{
				try { _stream.Dispose(); } catch { }
				_stream = null;
			}
			if (disposing)
			{
				TriggerEvent("close");
			}
		}
	}
}
