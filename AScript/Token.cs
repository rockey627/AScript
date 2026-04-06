using System;

namespace AScript
{
	public struct Token
	{
		public ETokenType Type;
		public string Value;
		public int Line;
		public int Column;

		public Token(string value, ETokenType type) : this(value, type, 0, 0)
		{
		}
		public Token(string value, ETokenType type, int line, int column)
		{
			this.Type = type;
			this.Value = value;
			this.Line = line;
			this.Column = column;
		}
	}
}
