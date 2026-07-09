using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptWriteStream : JavaScriptStream
	{
		public JavaScriptWriteStream(Stream stream) : base(stream)
		{
		}
		public JavaScriptWriteStream(Stream stream, IDictionary<string, object> options) : base(stream, options)
		{
		}

		public bool write(long value)
		{
			return write(value.ToString());
		}

		public bool write(double value)
		{
			return write(value.ToString());
		}

		public bool write(bool value)
		{
			return write(value.ToString());
		}

		public bool write(DateTime value)
		{
			return write(value.ToString());
		}

		public bool write(string value)
		{
			if (string.IsNullOrEmpty(value)) return true;
			return write(_encoding.GetBytes(value));
		}

		public bool write(IList<byte> value)
		{
			if (value == null || value.Count == 0) return true;
			//_events.TryGetValue()
			if (value is byte[] arr)
			{
				_stream.WriteAsync(arr, 0, arr.Length);
			}
			else
			{
				_stream.WriteAsync(value.ToArray(), 0, value.Count);
			}
			return true;
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

		public void end(long value)
		{
			write(value);
			Dispose();
		}

		public void end(double value)
		{
			write(value);
			Dispose();
		}

		public void end(bool value)
		{
			write(value);
			Dispose();
		}

		public void end(DateTime value)
		{
			write(value);
			Dispose();
		}

		public void end(string value)
		{
			write(value);
			Dispose();
		}

		public void end(IList<byte> value)
		{
			write(value);
			Dispose();
		}

		public void end(object value)
		{
			write(value);
			Dispose();
		}
	}
}
