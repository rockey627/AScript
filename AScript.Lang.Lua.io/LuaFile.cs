using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AScript.Lang.Lua.io
{
	/// <summary>
	/// Lua 文件对象，对应 io.open 返回的文件句柄
	/// </summary>
	public class LuaFile
	{
		private readonly FileStream _stream;
		private StreamReader _reader;
		private StreamWriter _writer;
		private bool _closed;

		protected StreamReader Reader
		{
			get
			{
				if (_reader == null)
				{
					_reader = new StreamReader(_stream, Encoding.UTF8, true, 1024, true);
				}
				return _reader;
			}
		}

		protected StreamWriter Writer
		{
			get
			{
				if (_writer == null)
				{
					_writer = new StreamWriter(_stream, Encoding.UTF8, bufferSize: 8192, true) { AutoFlush = false };
				}
				return _writer;
			}
		}

		public LuaFile(FileStream stream)
		{
			_stream = stream;
		}

		public bool Closed => _closed;

		/// <summary>
		/// 关闭文件
		/// </summary>
		public void close()
		{
			if (_closed) return;
			_closed = true;
			_reader?.Dispose();
			_writer?.Dispose();
			_stream.Dispose();
		}

		/// <summary>
		/// 刷新缓冲区
		/// </summary>
		public void flush()
		{
			if (_closed) throw new IOException("file is closed");
			_writer?.Flush();
		}

		public string read()
		{
			//Reader.Peek(); // 确保流位置正确
			return Reader.ReadToEnd();
		}

		public object read(object format)
		{
			if (format == null)
				return read();

			int n = -1;
			if (format is double d)
			{
				n = (int)d;
			}
			else if (format is int i)
			{
				n = i;
			}
			else if (format is long l)
			{
				n = (int)l;
			}
			if (n == 0) return string.Empty;
			if (n > 0)
			{
				// 读取n个字符
				char[] buffer = new char[n];
				int count = Reader.Read(buffer, 0, n);
				return new string(buffer, 0, count);
			}

			string fmt = format as string;
			if (fmt != null)
			{
				switch (fmt)
				{
					case "*a":
					case "a":
						return read();
					case "*l":
					case "l":
						return readLine();
					case "*L":
					case "L":
						return readLineWithTerminator();
					default:
						// 尝试解析为数字
						if (double.TryParse(fmt, out double lineNum) && lineNum >= 0)
						{
							char[] buffer = new char[(int)lineNum];
							int count = Reader.Read(buffer, 0, (int)lineNum);
							return new string(buffer, 0, count);
						}
						break;
				}
			}

			throw new ArgumentException($"invalid format: {format}");
		}

		/// <summary>
		/// 读取文件内容
		/// </summary>
		/// <param name="formats">读取格式：nil/*all, 数字n/*line, "l"/*line, "L"/*line+1, "a"/*all</param>
		/// <returns>读取的内容</returns>
		public List<object> read(object format1, object format2, params object[] formats)
		{
			if (_closed) throw new IOException("file is closed");
			//if (_reader == null) throw new IOException("file not opened for reading");

			var results = new List<object>(2 + (formats?.Length ?? 0));
			results.Add(read(format1));
			results.Add(read(format2));
			if (formats != null && formats.Length > 0)
			{
				foreach (var format in formats)
				{
					results.Add(read(format));
				}
			}
			return results;
		}

		private string readLine()
		{
			return Reader.ReadLine();
		}

		private string readLineWithTerminator()
		{
			int ch = Reader.Read();
			if (ch == -1) return null;
			StringBuilder sb = new StringBuilder();
			sb.Append((char)ch);
			while (true)
			{
				ch = Reader.Read();
				if (ch == -1) break;
				char c = (char)ch;
				sb.Append(c);
				if (c == '\n') break;
			}
			return sb.ToString();
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="values">要写入的值</param>
		/// <returns>当前文件对象（用于链式调用）</returns>
		public LuaFile write(params object[] values)
		{
			if (_closed) throw new IOException("file is closed");
			//if (_writer == null) throw new IOException("file not opened for writing");

			foreach (var value in values)
			{
				Writer.Write(value?.ToString());
			}
			return this;
		}

		/// <summary>
		/// 设置文件位置
		/// </summary>
		/// <param name="whence">起始位置："set", "cur", "end"</param>
		/// <param name="offset">偏移量</param>
		/// <returns>新位置</returns>
		public long seek(string whence = "cur", long offset = 0)
		{
			if (_closed) throw new IOException("file is closed");

			SeekOrigin origin;
			switch (whence)
			{
				case "set":
					origin = SeekOrigin.Begin;
					break;
				case "end":
					origin = SeekOrigin.End;
					break;
				case "cur":
				default:
					origin = SeekOrigin.Current;
					break;
			}
			return _stream.Seek(offset, origin);
		}

		/// <summary>
		/// 设置缓冲模式
		/// </summary>
		/// <param name="mode">缓冲模式："no", "full", "line"</param>
		/// <param name="size">缓冲区大小</param>
		public void setvbuf(string mode, int size = 8192)
		{
			if (_closed) throw new IOException("file is closed");
			// 在 .NET 中，我们主要使用默认缓冲，这里只是占位实现
		}

		/// <summary>
		/// 返回文件行迭代器
		/// </summary>
		public IEnumerable<string> lines()
		{
			if (_closed) yield break;
			string line;
			while ((line = Reader.ReadLine()) != null)
			{
				yield return line;
			}
		}

		/// <summary>
		/// 回到文件开头
		/// </summary>
		public void rewind()
		{
			if (_closed) throw new IOException("file is closed");
			_stream.Seek(0, SeekOrigin.Begin);
		}
	}
}
