using System;

namespace AScript
{
	public readonly struct Token
	{
		public readonly ETokenType Type;
		public readonly string Value;// => (string)this.Data;
		public readonly int Line;
		public readonly int Column;
		public readonly object Data;

		public Token(string value, ETokenType type) : this(value, type, 0, 0)
		{
		}
		public Token(string value, ETokenType type, int line, int column)
		{
			this.Type = type;
			this.Value = value;
			this.Data = null;
			this.Line = line;
			this.Column = column;
		}
		public Token(object value) : this(value, 0, 0) { }
		public Token(object value, int line, int column)
		{
			this.Type = ETokenType.Object;
			this.Value = null;
			this.Data = value;
			this.Line = line;
			this.Column = column;
		}

		public bool IsSymbol(string w)
		{
			return this.Type != ETokenType.String && this.Type != ETokenType.Number && this.Type != ETokenType.Object && this.Value == w;
		}

		public bool IsSymbol(string w, StringComparison comparisonType)
		{
			return this.Type != ETokenType.String && this.Type != ETokenType.Number && this.Type != ETokenType.Object && w.Equals(this.Value, comparisonType);
		}
	}
}
