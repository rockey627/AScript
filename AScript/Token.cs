using System;

namespace AScript
{
	public struct Token
	{
		public ETokenType Type;
		public string Value => (string)this.Data;
		public int Line;
		public int Column;
		public object Data;

		public Token(string value, ETokenType type) : this(value, type, 0, 0)
		{
		}
		public Token(string value, ETokenType type, int line, int column)
		{
			this.Type = type;
			this.Data = value;
			this.Line = line;
			this.Column = column;
		}
		public Token(object value) : this(value, 0, 0) { }
		public Token(object value, int line, int column)
		{
			this.Type = ETokenType.Object;
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
