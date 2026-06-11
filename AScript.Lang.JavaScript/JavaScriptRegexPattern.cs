using System;

namespace AScript.Lang.JavaScript
{
	public struct JavaScriptRegexPattern
	{
		public string Value;

		public JavaScriptRegexPattern(string value)
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return this.Value;
		}
	}
}
