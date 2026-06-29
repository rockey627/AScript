using System;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptException : Exception
	{
		public object Data { get; set; }

		public JavaScriptException(object data) : base(data?.ToString())
		{
			this.Data = data;
		}
	}
}
