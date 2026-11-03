using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class TranslateTokenStream : ITokenStream
	{
		private readonly ITokenStream _source;

		private int _count;
		private bool _isEnd;

		public TranslateTokenStream(ITokenStream source)
		{
			_source = source;
		}

		public Token? Next()
		{
			if (_isEnd) return null;
			var token = _source.Next();
			if (!token.HasValue)
			{
				_isEnd = true;
				return null;
			}
			if (token.Value.IsSymbol("{"))
			{
				_count++;
			}
			else if (token.Value.IsSymbol("}"))
			{
				_count--;
				if (_count < 0)
				{
					_isEnd = true;
					return null;
				}
			}
			return token;
		}

		public async Task<Token?> NextAsync(CancellationToken cancellationToken = default)
		{
			if (_isEnd) return null;
			var token = await _source.NextAsync(cancellationToken).ConfigureAwait(false);
			if (!token.HasValue)
			{
				_isEnd = true;
				return null;
			}
			if (token.Value.IsSymbol("{"))
			{
				_count++;
			}
			else if (token.Value.IsSymbol("}"))
			{
				_count--;
				if (_count < 0)
				{
					_isEnd = true;
					return null;
				}
			}
			return token;
		}
	}
}
