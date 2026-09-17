using System;

namespace AScript.Lang.Go.Types
{
	public class GoRuntimeType : GoType
	{
		private readonly string _Name;
		private readonly Type _Type;

		public override string Name => _Name;
		public override Type RealType => _Type;

		public GoRuntimeType(Type type)
		{
			_Type = type;
		}
		public GoRuntimeType(string name, Type type) : this(type)
		{
			_Name = name;
		}
	}
}
