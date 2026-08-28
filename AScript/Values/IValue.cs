using System;

namespace AScript.Values
{
	public interface IValue
	{
		Type Type { get; }

		object Get();
		bool GetBool();
		byte GetByte();
		sbyte GetSByte();
		char GetChar();
		short GetShort();
		ushort GetUShort();
		int GetInt();
		uint GetUInt();
		long GetLong();
		ulong GetULong();
		decimal GetDecimal();
		float GetFloat();
		double GetDouble();
		DateTime GetDateTime();
		string GetString();
	}
}
