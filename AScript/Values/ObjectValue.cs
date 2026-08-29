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
			return (bool)_value;
		}

		public override byte GetByte()
		{
			return (byte)_value;
		}

		public override sbyte GetSByte()
		{
			return (sbyte)_value;
		}

		public override char GetChar()
		{
			return (char)_value;
		}

		public override DateTime GetDateTime()
		{
			return (DateTime)_value;
		}

		public override decimal GetDecimal()
		{
			return (decimal)_value;
		}

		public override double GetDouble()
		{
			return (double)_value;
		}

		public override float GetFloat()
		{
			return (float)_value;
		}

		public override int GetInt()
		{
			return (int)_value;
		}

		public override long GetLong()
		{
			return (long)_value;
		}

		public override short GetShort()
		{
			return (short)_value;
		}

		public override string GetString()
		{
			return _value?.ToString();
		}

		public override uint GetUInt()
		{
			return (uint)_value;
		}

		public override ulong GetULong()
		{
			return (ulong)_value;
		}

		public override ushort GetUShort()
		{
			return (ushort)_value;
		}
	}
}
