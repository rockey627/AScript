using System;

namespace AScript
{
	public class TypeWrapper
	{
		public string Name { get; private set; }
		public Type Type { get; private set; }

		public TypeWrapper(Type type) : this(type.Name, type) { }
		public TypeWrapper(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}
	}

	public class TypeWrapper<T> : TypeWrapper
	{
		public TypeWrapper() : base(typeof(T)) { }
		public TypeWrapper(string name) : base(name, typeof(T)) { }
	}
}
