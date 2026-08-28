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

		public static IntValue Create(int value)
		{
			return new IntValue(value);
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
