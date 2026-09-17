using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.Types
{
	public class GoMapType : GoType
	{
		private readonly string _Name;
		private readonly Type _RealType;

		public override string Name => _Name;

		public override Type RealType => _RealType;

		public Type KeyType { get; private set; }
		public GoType ValueType { get; private set; }

		public GoMapType(string keyTypeName, Type keyType, GoType valueType)
		{
			KeyType = keyType;
			ValueType = valueType;
			_RealType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType.RealType);
			_Name = $"map[{keyTypeName}]{valueType.Name}";
		}
	}
}
