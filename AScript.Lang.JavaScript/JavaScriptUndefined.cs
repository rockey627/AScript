using System;

namespace AScript.Lang.JavaScript
{
	public sealed class JavaScriptUndefined
	{
		public static readonly JavaScriptUndefined Instance = new JavaScriptUndefined();

		private JavaScriptUndefined() { }

		public override string ToString()
		{
			return "undefined";
		}
	}
}
