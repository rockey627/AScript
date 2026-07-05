using System;

namespace AScript
{
	public class TypeWrapper
	{
		public string Name { get; private set; }
		public Type Type { get; private set; }

		public TypeWrapper(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}
	}
}
