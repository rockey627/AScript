using System;
using System.Threading.Tasks;

namespace AScript.Readers
{
    public class StringCharStream : ICharStream
    {
        private readonly string _expression;

        private int _currentIndex;

        public StringCharStream(string expression)
        {
            _expression = expression;
        }

        public char? Next()
		{
            if (_currentIndex >= _expression.Length)
            {
                return null;
            }
            return _expression[_currentIndex++];
        }

		public Task<char?> NextAsync()
		{
            return Task.FromResult(Next());
		}
	}
}
