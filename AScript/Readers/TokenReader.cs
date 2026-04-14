using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class TokenReader : IDisposable
	{
		private readonly Stack<Token> _peekStack = new Stack<Token>();

		private readonly ITokenStream _stream;
		private readonly bool _autoDisposeStream;

		public ITokenStream TokenStream => _stream;
		public CharReader CharReader => (_stream as DefaultTokenStream).CharReader;

		public TokenReader(ITokenStream stream, bool autoDisposeStream)
		{
			_stream = stream;
			_autoDisposeStream = autoDisposeStream;
		}

		/// <summary>
		/// 记号入栈，后进先出，先进后出
		/// </summary>
		/// <param name="c"></param>
		public void Push(Token c)
		{
			_peekStack.Push(c);
		}

		/// <summary>
		/// 读取下一个记号，如果栈中有记号则优先从栈中读取，否则从流中读取
		/// </summary>
		/// <returns></returns>
		public Token? Read()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Pop();
			}
			return _stream.Next();
		}

		/// <summary>
		/// 异步读取下一个记号，如果栈中有记号则优先从栈中读取，否则从流中读取
		/// </summary>
		/// <returns></returns>
		public async Task<Token?> ReadAsync()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Pop();
			}
			return await _stream.NextAsync().ConfigureAwait(false);
		}

		public Token? Peek()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Peek();
			}
			var c1 = _stream.Next();
			if (c1.HasValue) _peekStack.Push(c1.Value);
			return c1;
		}

		public async Task<Token?> PeekAsync()
		{
			if (_peekStack.Count > 0)
			{
				return _peekStack.Peek();
			}
			var c1 = await _stream.NextAsync().ConfigureAwait(false);
			if (c1.HasValue) _peekStack.Push(c1.Value);
			return c1;
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
