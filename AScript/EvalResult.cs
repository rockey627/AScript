using System;

namespace AScript
{
	public struct EvalResult : IConvertible
	{
		public ECompletionType CompletionType;
		public object Value;
		public Type Type;

		public EvalResult(object value)
		{
			this.Value = value;
			this.Type = value?.GetType() ?? typeof(object);
			this.CompletionType = ECompletionType.Normal;
		}
		public EvalResult(object value, Type type)
		{
			this.Value = value;
			this.Type = type;
			this.CompletionType = ECompletionType.Normal;
		}
		public EvalResult(object value, ECompletionType completionType)
		{
			this.Value = value;
			this.Type = value?.GetType() ?? typeof(object);
			this.CompletionType = completionType;
		}
		public EvalResult(object value, Type type, ECompletionType completionType)
		{
			this.Value = value;
			this.Type = type;
			this.CompletionType = completionType;
		}

		public static implicit operator bool(EvalResult result)
		{
			return (bool)result.Value;
		}
		public static implicit operator byte(EvalResult result)
		{
			return (byte)result.Value;
		}
		public static implicit operator int(EvalResult result)
		{
			return (int)result.Value;
		}
		public static implicit operator uint(EvalResult result)
		{
			return (uint)result.Value;
		}
		public static implicit operator long(EvalResult result)
		{
			return (long)result.Value;
		}
		public static implicit operator ulong(EvalResult result)
		{
			return (ulong)result.Value;
		}
		public static implicit operator float(EvalResult result)
		{
			return (float)result.Value;
		}
		public static implicit operator decimal(EvalResult result)
		{
			return (decimal)result.Value;
		}
		public static implicit operator double(EvalResult result)
		{
			return (double)result.Value;
		}
		public static implicit operator string(EvalResult result)
		{
			return (string)result.Value;
		}
		public static implicit operator DateTime(EvalResult result)
		{
			return (DateTime)result.Value;
		}

		public override string ToString()
		{
			return this.Value?.ToString();
		}

		TypeCode IConvertible.GetTypeCode()
		{
			if (this.Type == null) return TypeCode.Empty;
			return Type.GetTypeCode(this.Type);
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
