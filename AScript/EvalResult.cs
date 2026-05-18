using System;

namespace AScript
{
	public struct EvalResult
	{
		public object Value;
		public Type Type;

		public EvalResult(object value, Type type)
		{
			this.Value = value;
			this.Type = type;
		}
	}
}
