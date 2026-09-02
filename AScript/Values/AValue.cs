using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AScript.Values
{
	public abstract class AValue : IValue, IConvertible
	{
		public static readonly MethodInfo Method_Get_object = typeof(AValue).GetMethods().FirstOrDefault(a => a.Name == "Get" && !a.IsGenericMethod);
		public static readonly MethodInfo Method_GetInt = typeof(AValue).GetMethod("GetInt");
		public static readonly MethodInfo Method_GetBool = typeof(AValue).GetMethod("GetBool");
		public static readonly MethodInfo Method_GetByte = typeof(AValue).GetMethod("GetByte");
		public static readonly MethodInfo Method_GetSByte = typeof(AValue).GetMethod("GetSByte");
		public static readonly MethodInfo Method_GetChar = typeof(AValue).GetMethod("GetChar");
		public static readonly MethodInfo Method_GetShort = typeof(AValue).GetMethod("GetShort");
		public static readonly MethodInfo Method_GetLong = typeof(AValue).GetMethod("GetLong");
		public static readonly MethodInfo Method_GetUInt = typeof(AValue).GetMethod("GetUInt");
		public static readonly MethodInfo Method_GetULong = typeof(AValue).GetMethod("GetULong");
		public static readonly MethodInfo Method_GetUShort = typeof(AValue).GetMethod("GetUShort");
		public static readonly MethodInfo Method_GetFloat = typeof(AValue).GetMethod("GetFloat");
		public static readonly MethodInfo Method_GetDouble = typeof(AValue).GetMethod("GetDouble");
		public static readonly MethodInfo Method_GetDecimal = typeof(AValue).GetMethod("GetDecimal");
		public static readonly MethodInfo Method_GetDateTime = typeof(AValue).GetMethod("GetDateTime");
		public static readonly MethodInfo Method_GetString = typeof(AValue).GetMethod("GetString");

		public abstract Type Type { get; }

		public static AValue Create(object value, Type type)
		{
			if (value == null)
			{
				return new ObjectValue(value, type);
			}
			if (value is AValue)
			{
				return new ObjectValue(value);
			}
			if (type == null)
			{
				type = value.GetType();
			}
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return Create((bool)value);
				case TypeCode.Byte:
					return Create((byte)value);
				case TypeCode.Char:
					return Create((char)value);
				case TypeCode.DateTime:
					return Create((DateTime)value);
				case TypeCode.Decimal:
					return Create((decimal)value);
				case TypeCode.Double:
					return Create((double)value);
				case TypeCode.Int16:
					return Create((short)value);
				case TypeCode.Int32:
					return Create((int)value);
				case TypeCode.Int64:
					return Create((long)value);
				case TypeCode.SByte:
					return Create((sbyte)value);
				case TypeCode.Single:
					return Create((float)value);
				case TypeCode.String:
					return Create((string)value);
				case TypeCode.UInt16:
					return Create((ushort)value);
				case TypeCode.UInt32:
					return Create((uint)value);
				case TypeCode.UInt64:
					return Create((ulong)value);
				default:
					return new ObjectValue(value, type);
			}
		}
		public static AValue Create<T>(T value)
		{
			var type = typeof(T);
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return Create(Unsafe.As<T, bool>(ref value));
				case TypeCode.Byte:
					return Create(Unsafe.As<T, byte>(ref value));
				case TypeCode.SByte:
					return Create(Unsafe.As<T, sbyte>(ref value));
				case TypeCode.Char:
					return Create(Unsafe.As<T, char>(ref value));
				case TypeCode.Int16:
					return Create(Unsafe.As<T, short>(ref value));
				case TypeCode.Int32:
					return Create(Unsafe.As<T, int>(ref value));
				case TypeCode.Int64:
					return Create(Unsafe.As<T, long>(ref value));
				case TypeCode.UInt16:
					return Create(Unsafe.As<T, ushort>(ref value));
				case TypeCode.UInt32:
					return Create(Unsafe.As<T, uint>(ref value));
				case TypeCode.UInt64:
					return Create(Unsafe.As<T, ulong>(ref value));
				case TypeCode.Single:
					return Create(Unsafe.As<T, float>(ref value));
				case TypeCode.Double:
					return Create(Unsafe.As<T, double>(ref value));
				case TypeCode.Decimal:
					return Create(Unsafe.As<T, decimal>(ref value));
				case TypeCode.DateTime:
					return Create(Unsafe.As<T, DateTime>(ref value));
				case TypeCode.String:
					return Create(Unsafe.As<T, string>(ref value));
				default:
					return new ObjectValue(value, type);
			}
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

		public static object GetValue(object obj)
		{
			if (obj is AValue aValue) return aValue.Get();
			return obj;
		}

		public static T GetValue<T>(object obj)
		{
			if (obj is AValue aValue) return aValue.Get<T>();
			return (T)obj;
		}

		public static Expression GetExpression(Expression obj)
		{
			if (obj.Type == typeof(IntValue))
			{
				return Expression.Call(obj, Method_GetInt);
			}
			if (obj.Type == typeof(BoolValue))
			{
				return Expression.Call(obj, Method_GetBool);
			}
			if (obj.Type == typeof(ByteValue))
			{
				return Expression.Call(obj, Method_GetByte);
			}
			if (obj.Type == typeof(SByteValue))
			{
				return Expression.Call(obj, Method_GetSByte);
			}
			if (obj.Type == typeof(CharValue))
			{
				return Expression.Call(obj, Method_GetChar);
			}
			if (obj.Type == typeof(ShortValue))
			{
				return Expression.Call(obj, Method_GetShort);
			}
			if (obj.Type == typeof(LongValue))
			{
				return Expression.Call(obj, Method_GetLong);
			}
			if (obj.Type == typeof(UIntValue))
			{
				return Expression.Call(obj, Method_GetUInt);
			}
			if (obj.Type == typeof(ULongValue))
			{
				return Expression.Call(obj, Method_GetULong);
			}
			if (obj.Type == typeof(UShortValue))
			{
				return Expression.Call(obj, Method_GetUShort);
			}
			if (obj.Type == typeof(FloatValue))
			{
				return Expression.Call(obj, Method_GetFloat);
			}
			if (obj.Type == typeof(DoubleValue))
			{
				return Expression.Call(obj, Method_GetDouble);
			}
			if (obj.Type == typeof(DecimalValue))
			{
				return Expression.Call(obj, Method_GetDecimal);
			}
			if (obj.Type == typeof(DateTimeValue))
			{
				return Expression.Call(obj, Method_GetDateTime);
			}
			if (obj.Type == typeof(StringValue))
			{
				return Expression.Call(obj, Method_GetString);
			}
			if (obj.Type == typeof(ObjectValue))
			{
				return Expression.Call(obj, Method_Get_object);
			}
			return obj;
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

		public static bool operator !=(AValue v1, int v2)
		{
			return !(v1 == v2);
		}
		public static bool operator ==(AValue v1, int v2)
		{
			switch (Type.GetTypeCode(v1.Type))
			{
				case TypeCode.Boolean:
					return false;
				case TypeCode.Byte:
					return v1.GetByte() == v2;
				case TypeCode.Char:
					return v1.GetChar() == v2;
				case TypeCode.DateTime:
					return false;
				case TypeCode.DBNull:
					return false;
				case TypeCode.Decimal:
					return v1.GetDecimal() == v2;
				case TypeCode.Double:
					return v1.GetDouble() == v2;
				case TypeCode.Empty:
					return false;
				case TypeCode.Int16:
					return v1.GetShort() == v2;
				case TypeCode.Int32:
					return v1.GetInt() == v2;
				case TypeCode.Int64:
					return v1.GetLong() == v2;
				case TypeCode.Object:
					var obj = v1.Get();
					if (obj is AValue ov) return ov == v2;
					return false;
				case TypeCode.SByte:
					return v1.GetSByte() == v2;
				case TypeCode.Single:
					return v1.GetFloat() == v2;
				case TypeCode.String:
					return false;
				case TypeCode.UInt16:
					return v1.GetUShort() == v2;
				case TypeCode.UInt32:
					return v1.GetUInt() == v2;
				case TypeCode.UInt64:
					return v1.GetULong() == (ulong)v2;
				default:
					return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is int v2) return this == v2;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			switch (Type.GetTypeCode(this.Type))
			{
				case TypeCode.Boolean:
					return GetBool().GetHashCode();
				case TypeCode.Byte:
					return GetByte().GetHashCode();
				case TypeCode.Char:
					return GetChar().GetHashCode();
				case TypeCode.DateTime:
					return GetDateTime().GetHashCode();
				case TypeCode.DBNull:
					return 0;
				case TypeCode.Decimal:
					return GetDecimal().GetHashCode();
				case TypeCode.Double:
					return GetDouble().GetHashCode();
				case TypeCode.Empty:
					return 0;
				case TypeCode.Int16:
					return GetShort().GetHashCode();
				case TypeCode.Int32:
					return GetInt().GetHashCode();
				case TypeCode.Int64:
					return GetLong().GetHashCode();
				case TypeCode.Object:
					return Get().GetHashCode();
				case TypeCode.SByte:
					return GetSByte().GetHashCode();
				case TypeCode.Single:
					return GetFloat().GetHashCode();
				case TypeCode.String:
					return GetString().GetHashCode();
				case TypeCode.UInt16:
					return GetUShort().GetHashCode();
				case TypeCode.UInt32:
					return GetUInt().GetHashCode();
				case TypeCode.UInt64:
					return GetULong().GetHashCode();
				default:
					return 0;
			}
		}

		public override string ToString()
		{
			return GetString();
		}

		public T Get<T>()
		{
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Boolean:
					var valueBool = GetBool();
					return Unsafe.As<bool, T>(ref valueBool);
				case TypeCode.Byte:
					var valueByte = GetByte();
					return Unsafe.As<byte, T>(ref valueByte);
				case TypeCode.SByte:
					var valueSByte = GetSByte();
					return Unsafe.As<sbyte, T>(ref valueSByte);
				case TypeCode.Char:
					var valueChar = GetChar();
					return Unsafe.As<char, T>(ref valueChar);
				case TypeCode.Int16:
					var valueInt16 = GetShort();
					return Unsafe.As<short, T>(ref valueInt16);
				case TypeCode.Int32:
					var valueInt32 = GetInt();
					return Unsafe.As<int, T>(ref valueInt32);
				case TypeCode.Int64:
					var valueInt64 = GetLong();
					return Unsafe.As<long, T>(ref valueInt64);
				case TypeCode.UInt16:
					var valueUInt16 = GetUShort();
					return Unsafe.As<ushort, T>(ref valueUInt16);
				case TypeCode.UInt32:
					var valueUInt32 = GetUInt();
					return Unsafe.As<uint, T>(ref valueUInt32);
				case TypeCode.UInt64:
					var valueUInt64 = GetULong();
					return Unsafe.As<ulong, T>(ref valueUInt64);
				case TypeCode.Single:
					var valueSingle = GetFloat();
					return Unsafe.As<float, T>(ref valueSingle);
				case TypeCode.Double:
					var valueDouble = GetDouble();
					return Unsafe.As<double, T>(ref valueDouble);
				case TypeCode.Decimal:
					var valueDecimal = GetDecimal();
					return Unsafe.As<decimal, T>(ref valueDecimal);
				case TypeCode.DateTime:
					var valueDateTime = GetDateTime();
					return Unsafe.As<DateTime, T>(ref valueDateTime);
				case TypeCode.String:
					var valueString = GetString();
					return Unsafe.As<string, T>(ref valueString);
				default:
					return (T)Get();
			}
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
			return Type.GetTypeCode(this.Type);
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
