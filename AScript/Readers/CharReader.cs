using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class CharReader : IDisposable
	{
		private readonly Stack<char> _peekStack = new Stack<char>();

		private readonly ICharStream _stream;
		private readonly bool _autoDisposeStream;

		public int CurrentLine { get; private set; } = 1;
		public int CurrentColumn { get; private set; }

		public CharReader(ICharStream stream, bool autoDisposeStream)
		{
			_stream = stream;
			_autoDisposeStream = autoDisposeStream;
		}

		/// <summary>
		/// 字符入栈
		/// </summary>
		/// <param name="c"></param>
		public void Push(char c)
		{
			_peekStack.Push(c);
			HandleLineAndColumn(c, true);
		}

		public void Push(string s)
		{
			Push(s, 0, s.Length);
		}

		public void Push(string s, int from)
		{
			Push(s, from, -1);
		}

		public void Push(string s, int from, int count)
		{
			if (string.IsNullOrEmpty(s)) return;
			int to = count > 0 ? from + count - 1 : s.Length - 1;
			for (int i = to; i >= from ; i--)
			{
				Push(s[i]);
			}
		}

		/// <summary>
		/// 读取下一个字符，如果栈中有字符则优先从栈中读取，否则从流中读取
		/// </summary>
		/// <returns></returns>
		public char? Read()
		{
			char? c;
			if (_peekStack.Count > 0)
			{
				c = _peekStack.Pop();
			}
			else c = _stream.Next();
			if (c.HasValue)
			{
				HandleLineAndColumn(c.Value, false);
			}
			return c;
		}

		/// <summary>
		/// 异步读取下一个字符，如果栈中有字符则优先从栈中读取，否则从流中读取
		/// </summary>
		/// <returns></returns>
		public async Task<char?> ReadAsync()
		{
			char? c;
			if (_peekStack.Count > 0)
			{
				c = _peekStack.Pop();
			}
			else
			{
				c = await _stream.NextAsync().ConfigureAwait(false);
			}
			if (c.HasValue)
			{
				HandleLineAndColumn(c.Value, false);
			}
			return c;
		}

		public char? Peek()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Peek();
			}
			var c1 = _stream.Next();
			if (c1.HasValue) _peekStack.Push(c1.Value);
			return c1;
		}

		public async Task<char?> PeekAsync()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Peek();
			}
			var c1 = await _stream.NextAsync().ConfigureAwait(false);
			if (c1.HasValue) _peekStack.Push(c1.Value);
			return c1;
		}

		private void HandleLineAndColumn(char c, bool back)
		{
			if (c == '\n')
			{
				if (back) CurrentLine--;
				else
				{
					CurrentLine++;
					CurrentColumn = 0;
				}
			}
			else
			{
				if (back) CurrentColumn--;
				else CurrentColumn++;
			}
		}

		public void Dispose()
		{
			_peekStack.Clear();
			if (_autoDisposeStream)
			{
				(_stream as IDisposable)?.Dispose();
			}
		}
	}
}
