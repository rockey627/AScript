using System;

namespace AScript.Values
{
	public abstract class AValue : IValue, IConvertible
	{
		protected Type _type;

		public Type Type => _type;

		protected AValue(Type type)
		{
			_type = type;
		}

		public static NumberValue CreateNumber(double value, Type type)
		{
			return new NumberValue(value, type);
		}
		public static IntValue Create(int value)
		{
			return new IntValue(value);
		}
		public static ByteValue Create(byte value)
		{
			return new ByteValue(value);
		}
		public static SByteValue Create(sbyte value)
		{
			return new SByteValue(value);
		}
		public static BoolValue Create(bool value)
		{
			return new BoolValue(value);
		}
		public static DoubleValue Create(double value)
		{
			return new DoubleValue(value);
		}
		public static DecimalValue Create(decimal value)
		{
			return new DecimalValue(value);
		}
		public static DateTimeValue Create(DateTime value)
		{
			return new DateTimeValue(value);
		}
		public static LongValue Create(long value)
		{
			return new LongValue(value);
		}
		public static ShortValue Create(short value)
		{
			return new ShortValue(value);
		}
		public static UShortValue Create(ushort value)
		{
			return new UShortValue(value);
		}
		public static UIntValue Create(uint value)
		{
			return new UIntValue(value);
		}
		public static ULongValue Create(ulong value)
		{
			return new ULongValue(value);
		}
		public static FloatValue Create(float value)
		{
			return new FloatValue(value);
		}
		public static StringValue Create(string value)
		{
			return new StringValue(value);
		}
		public static CharValue Create(char value)
		{
			return new CharValue(value);
		}

		public static implicit operator AValue(int value)
		{
			return Create(value);
		}
		public static implicit operator AValue(byte value)
		{
			return Create(value);
		}
		public static implicit operator AValue(sbyte value)
		{
			return Create(value);
		}
		public static implicit operator AValue(bool value)
		{
			return Create(value);
		}
		public static implicit operator AValue(double value)
		{
			return Create(value);
		}
		public static implicit operator AValue(decimal value)
		{
			return Create(value);
		}
		public static implicit operator AValue(float value)
		{
			return Create(value);
		}
		public static implicit operator AValue(long value)
		{
			return Create(value);
		}
		public static implicit operator AValue(uint value)
		{
			return Create(value);
		}
		public static implicit operator AValue(ulong value)
		{
			return Create(value);
		}
		public static implicit operator AValue(short value)
		{
			return Create(value);
		}
		public static implicit operator AValue(char value)
		{
			return Create(value);
		}
		public static implicit operator AValue(ushort value)
		{
			return Create(value);
		}
		public static implicit operator AValue(DateTime value)
		{
			return Create(value);
		}
		public static implicit operator AValue(string value)
		{
			return Create(value);
		}

		public static implicit operator int(AValue value)
		{
			return value.GetInt();
		}
		public static implicit operator byte(AValue value)
		{
			return value.GetByte();
		}
		public static implicit operator sbyte(AValue value)
		{
			return value.GetSByte();
		}
		public static implicit operator bool(AValue value)
		{
			return value.GetBool();
		}
		public static implicit operator double(AValue value)
		{
			return value.GetDouble();
		}
		public static implicit operator decimal(AValue value)
		{
			return value.GetDecimal();
		}
		public static implicit operator float(AValue value)
		{
			return value.GetFloat();
		}
		public static implicit operator long(AValue value)
		{
			return value.GetLong();
		}
		public static implicit operator uint(AValue value)
		{
			return value.GetUInt();
		}
		public static implicit operator ulong(AValue value)
		{
			return value.GetULong();
		}
		public static implicit operator short(AValue value)
		{
			return value.GetShort();
		}
		public static implicit operator char(AValue value)
		{
			return value.GetChar();
		}
		public static implicit operator ushort(AValue value)
		{
			return value.GetUShort();
		}
		public static implicit operator DateTime(AValue value)
		{
			return value.GetDateTime();
		}
		public static implicit operator string(AValue value)
		{
			return value.GetString();
		}

		public abstract object Get();

		public abstract bool GetBool();

		public abstract byte GetByte();

		public abstract sbyte GetSByte();

		public abstract char GetChar();

		public abstract DateTime GetDateTime();

		public abstract decimal GetDecimal();

		public abstract double GetDouble();

		public abstract float GetFloat();

		public abstract int GetInt();

		public abstract long GetLong();

		public abstract short GetShort();

		public abstract string GetString();

		public abstract uint GetUInt();

		public abstract ulong GetULong();

		public abstract ushort GetUShort();

		TypeCode IConvertible.GetTypeCode()
		{
			return Type.GetTypeCode(_type);
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return GetBool();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return GetByte();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return GetChar();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return GetDateTime();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return GetDecimal();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return GetDouble();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return GetShort();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return GetInt();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return GetLong();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return GetSByte();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return GetFloat();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return GetString();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convert.ChangeType(Get(), conversionType);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return GetUShort();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return GetUInt();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return GetULong();
		}
	}
}
