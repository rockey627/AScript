using System;

namespace AScript.Values
{
	public class ULongValue : AValue
	{
		private ulong _value;

		public ulong Value
		{
			get => _value;
			set => _value = value;
		}

		public ULongValue() : base(typeof(ulong))
		{
		}

		public ULongValue(ulong value) : this()
		{
			_value = value;
		}

		public override object Get()
		{
			return _value;
		}

		public override bool GetBool()
		{
			return _value != 0;
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
			throw new InvalidCastException();
		}

		public override decimal GetDecimal()
		{
			return _value;
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
			return _value.ToString();
		}

		public override uint GetUInt()
		{
			return (uint)_value;
		}

		public override ulong GetULong()
		{
			return _value;
		}

		public override ushort GetUShort()
		{
			return (ushort)_value;
		}
	}
}
