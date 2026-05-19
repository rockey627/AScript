using System;

namespace AScript.Exceptions
{
	/// <summary>
	/// 脚本运行时异常
	/// </summary>
	public class ScriptRuntimeException : ScriptException
	{
		public ScriptRuntimeException() : base() { }
		public ScriptRuntimeException(string message) : base(message) { }
		public ScriptRuntimeException(string message, Exception innerException) : base(message, innerException) { }
	}
}
