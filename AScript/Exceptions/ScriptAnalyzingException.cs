using System;

namespace AScript.Exceptions
{
	/// <summary>
	/// 语法分析异常
	/// </summary>
	public class ScriptAnalyzingException : ScriptException
	{
		public ScriptAnalyzingException() : base() { }
		public ScriptAnalyzingException(string message) : base(message) { }
	}
}
