using System;

namespace AScript.Lang.Python3
{
	public class Python3Type
	{
		private readonly Type _Type;

		public string __name__ { get; set; }

		public Python3Type(Type type)
		{
			_Type = type;
			if (type == null)
			{
				__name__ = "NoneType";
			}
			else if (type == typeof(string))
			{
				__name__ = "str";
			}
			else if (ScriptUtils.IsIntegerType(type))
			{
				__name__ = "int";
			}
			else if (ScriptUtils.IsFloatType(type))
			{
				__name__ = "float";
			}
			else if (type == typeof(bool))
			{
				__name__ = "bool";
			}
			else
			{
				__name__ = type.Name;
			}
		}
	}
}
