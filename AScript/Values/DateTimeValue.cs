using System;

namespace AScript.Values
{
	public class DateTimeValue : AValue
	{
		private DateTime _value;

		public DateTime Value
		{
			get => _value;
			set => _value = value;
		}

		public DateTimeValue() : base(typeof(DateTime))
		{
		}

		public DateTimeValue(DateTime value) : this()
		{
			_value = value;
		}

		public override object Get()
		{
			return _value;
		}

		public override bool GetBool()
		{
			throw new InvalidCastException();
		}

		public override byte GetByte()
		{
			throw new InvalidCastException();
		}

		public override sbyte GetSByte()
		{
			throw new InvalidCastException();
		}

		public override char GetChar()
		{
			throw new InvalidCastException();
		}

		public override DateTime GetDateTime()
		{
			return _value;
		}

		public override decimal GetDecimal()
		{
			throw new InvalidCastException();
		}

		public override double GetDouble()
		{
			throw new InvalidCastException();
		}

		public override float GetFloat()
		{
			throw new InvalidCastException();
		}

		public override int GetInt()
		{
			throw new InvalidCastException();
		}

		public override long GetLong()
		{
			throw new InvalidCastException();
		}

		public override short GetShort()
		{
			throw new InvalidCastException();
		}

		public override string GetString()
		{
			return _value.ToString();
		}

		public override uint GetUInt()
		{
			throw new InvalidCastException();
		}

		public override ulong GetULong()
		{
			throw new InvalidCastException();
		}

		public override ushort GetUShort()
		{
			throw new InvalidCastException();
		}
	}
}
