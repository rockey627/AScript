using System;

namespace AScript
{
	public class SingleTypeScriptModule : IScriptModule
	{
		public Type Type { get; private set; }
		public string TypeName { get; private set; }

		public SingleTypeScriptModule(Type type) : this(type, type.Name)
		{
		}
		public SingleTypeScriptModule(Type type, string typeName)
		{
			this.Type = type;
			this.TypeName = typeName;
		}

		public object Install(BaseContext context)
		{
			context.AddType(this.TypeName, this.Type);
			return null;
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType(this.TypeName);
		}
	}

	public class SingleTypeScriptModule<T> : SingleTypeScriptModule
	{
		public SingleTypeScriptModule() : base(typeof(T)) { }
		public SingleTypeScriptModule(string typeName) : base(typeof(T), typeName) { }
	}
}
