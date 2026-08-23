using System;

namespace AScript
{
	public class CompletionResult : IConvertible
	{
		public ECompletionType CompletionType { get; private set; }
		public object Value { get; private set; }
		public Type ValueType { get; private set; }

		public CompletionResult(ECompletionType completionType)
		{
			this.CompletionType = completionType;
		}
		public CompletionResult(ECompletionType completionType, object value)
		{
			this.CompletionType = completionType;
			this.Value = value;
			this.ValueType = value?.GetType();
		}
		public CompletionResult(ECompletionType completionType, object value, Type valueType)
		{
			this.CompletionType = completionType;
			this.Value = value;
			this.ValueType = valueType ?? value?.GetType();
		}

		public override string ToString()
		{
			return this.Value?.ToString();
		}

		TypeCode IConvertible.GetTypeCode()
		{
			if (this.ValueType == null) return TypeCode.Empty;
			return Type.GetTypeCode(this.ValueType);
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this.Value, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this.Value, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this.Value, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this.Value, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this.Value, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this.Value, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this.Value, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this.Value, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this.Value, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this.Value, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this.Value, provider);
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return Convert.ToString(this.Value, provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(this.Value, conversionType, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this.Value, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this.Value, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this.Value, provider);
		}
	}
}
