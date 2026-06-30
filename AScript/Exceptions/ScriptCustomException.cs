using System;

namespace AScript.Exceptions
{
	public class ScriptCustomException : ScriptRuntimeException
	{
		public object Data { get; private set; }

		public ScriptCustomException(object data) : base(data?.ToString())
		{
			this.Data = data;
		}
	}
}
