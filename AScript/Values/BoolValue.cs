using System;

namespace AScript.Values
{
	public class BoolValue : AValue
	{
		private bool _value;

		public bool Value
		{
			get => _value;
			set => _value = value;
		}

		public BoolValue() : base(typeof(bool))
		{
		}

		public BoolValue(bool value) : this()
		{
			_value = value;
		}

		public override object Get()
		{
			return _value;
		}

		public override bool GetBool()
		{
			return _value;
		}

		public override byte GetByte()
		{
			return (byte)(_value ? 1 : 0);
		}

		public override sbyte GetSByte()
		{
			return (sbyte)(_value ? 1 : 0);
		}

		public override char GetChar()
		{
			throw new InvalidCastException();
		}

		public override DateTime GetDateTime()
		{
			throw new InvalidCastException();
		}

		public override decimal GetDecimal()
		{
			return _value ? 1M : 0M;
		}

		public override double GetDouble()
		{
			return _value ? 1D : 0D;
		}

		public override float GetFloat()
		{
			return _value ? 1F : 0F;
		}

		public override int GetInt()
		{
			return _value ? 1 : 0;
		}

		public override long GetLong()
		{
			return _value ? 1L : 0L;
		}

		public override short GetShort()
		{
			return (short)(_value ? 1 : 0);
		}

		public override string GetString()
		{
			return _value.ToString();
		}

		public override uint GetUInt()
		{
			return _value ? 1u : 0u;
		}

		public override ulong GetULong()
		{
			return _value ? 1ul : 0ul;
		}

		public override ushort GetUShort()
		{
			return (ushort)(_value ? 1 : 0);
		}
	}
}
