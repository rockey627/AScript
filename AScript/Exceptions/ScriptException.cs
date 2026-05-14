using System;

namespace AScript.Exceptions
{
	/// <summary>
	/// 脚本异常
	/// </summary>
	public class ScriptException : Exception
	{
		public ScriptException() : base() { }
		public ScriptException(string message) : base(message) { }
		public ScriptException(string message, Exception innerException):base(message, innerException) { }
	}
}
