using System;

namespace AScript
{
	public class TypeWrapper
	{
		public Type Type { get; private set; }

		public TypeWrapper(Type type)
		{
			this.Type = type;
		}
	}
}
