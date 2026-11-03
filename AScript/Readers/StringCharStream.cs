using System;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
    public class StringCharStream : ICharStream
    {
        private readonly string _expression;

		private int _currentIndex;

        public int CurrentIndex => _currentIndex;

        public StringCharStream(string expression)
        {
            _expression = expression;
		}
		public StringCharStream(string expression, int currentIndex) : this(expression)
		{
			_currentIndex = currentIndex;
		}

		public char? Next()
		{
            if (_currentIndex >= _expression.Length)
            {
                return null;
            }
            return _expression[_currentIndex++];
        }

		public Task<char?> NextAsync(CancellationToken cancellationToken = default)
		{
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Next());
		}
	}
}
