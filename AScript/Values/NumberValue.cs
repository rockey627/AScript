using System;

namespace AScript.Values
{
	public class NumberValue : DoubleValue
	{
		public new Type Type
		{
			get => _type;
			set => _type = value;
		}

		public NumberValue() { }
		public NumberValue(double value) : base(value) { }
		public NumberValue(double value, Type type): base(value)
		{
			_type = type;
		}
	}
}
