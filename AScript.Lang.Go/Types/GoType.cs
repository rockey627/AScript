using System;

namespace AScript.Lang.Go.Types
{
	public abstract class GoType
	{
		public abstract string Name { get; }
		public abstract Type RealType { get; }

		public static implicit operator GoType(Type type)
		{
			return new GoRuntimeType(type);
		}
		public static implicit operator Type(GoType type)
		{
			return type.RealType;
		}
	}
}
