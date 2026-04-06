using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AScript.Readers
{
    public class ListTokenStream : ITokenStream
    {
        private readonly IList<Token> _tokens;

        private int _currentIndex;

        public ListTokenStream(IList<Token> tokens)
        {
            _tokens = tokens;
        }

        public Token? Next()
        {
            if (_tokens == null || _currentIndex >= _tokens.Count)
            {
                return null;
            }
            return _tokens[_currentIndex++];
        }

		public Task<Token?> NextAsync()
		{
            return Task.FromResult(Next());
		}

		public void Reset()
        {
            _currentIndex = 0;
        }
    }
}
