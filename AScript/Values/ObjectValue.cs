using System;

namespace AScript.Values
{
	public class ObjectValue : AValue
	{
		private object _value;
		private Type _type;

		public object Value
		{
			get => _value;
			set
			{
				_value = value;
				_type = null;
			}
		}

		public override Type Type
		{
			get
			{
				if (_value == null) return typeof(object);
				if (_value is AValue aValue) return aValue.Type;
				if (_type == null) _type = _value.GetType();
				return _type;
			}
		}

		public ObjectValue() { }
		public ObjectValue(object value)
		{
			_value = value;
		}
		public ObjectValue(object value, Type type)
		{
			_value = value;
			_type = type;
		}

		public override object Get()
		{
			if (_value is AValue aValue)
			{
				return aValue.Get();
			}
			return _value;
		}

		public override bool GetBool()
		{
			return Convert.ToBoolean(_value);
		}

		public override byte GetByte()
		{
			return Convert.ToByte(_value);
		}

		public override sbyte GetSByte()
		{
			return Convert.ToSByte(_value);
		}

		public override char GetChar()
		{
			return Convert.ToChar(_value);
		}

		public override DateTime GetDateTime()
		{
			return Convert.ToDateTime(_value);
		}

		public override decimal GetDecimal()
		{
			return Convert.ToDecimal(_value);
		}

		public override double GetDouble()
		{
			return Convert.ToDouble(_value);
		}

		public override float GetFloat()
		{
			return Convert.ToSingle(_value);
		}

		public override int GetInt()
		{
			return Convert.ToInt32(_value);
		}

		public override long GetLong()
		{
			return Convert.ToInt64(_value);
		}

		public override short GetShort()
		{
			return Convert.ToInt16(_value);
		}

		public override string GetString()
		{
			return Convert.ToString(_value);
		}

		public override uint GetUInt()
		{
			return Convert.ToUInt32(_value);
		}

		public override ulong GetULong()
		{
			return Convert.ToUInt64(_value);
		}

		public override ushort GetUShort()
		{
			return Convert.ToUInt16(_value);
		}
	}
}
