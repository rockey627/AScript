using System;

namespace AScript
{
	public class SingleTypeScriptModule : IScriptModule
	{
		public Type Type { get; private set; }
		public string TypeName { get; private set; }

		/// <summary>
		/// <para>Install方法返回值：</para>
		/// <para>0-返回null</para>
		/// <para>1-返回Type</para>
		/// <para>2-返回TypeWrapper</para>
		/// <para>3-返回实例</para>
		/// </summary>
		public int ReturnMode { get; set; }

		public SingleTypeScriptModule(Type type) : this(type.Name, type)
		{
		}
		public SingleTypeScriptModule(string typeName, Type type)
		{
			this.Type = type;
			this.TypeName = typeName;
		}

		public object Install(BaseContext context)
		{
			context.AddType(this.TypeName, this.Type);
			switch (this.ReturnMode)
			{
				case 1: return this.Type;
				case 2: return new TypeWrapper(this.TypeName, this.Type);
				case 3: return Activator.CreateInstance(this.Type);
				default:
					return null;
			}
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType(this.TypeName);
		}
	}

	public class SingleTypeScriptModule<T> : SingleTypeScriptModule
	{
		public SingleTypeScriptModule() : base(typeof(T)) { }
		public SingleTypeScriptModule(string typeName) : base(typeName, typeof(T)) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnMode">
		/// <para>Install方法返回值：</para>
		/// <para>0-返回null</para>
		/// <para>1-返回Type</para>
		/// <para>2-返回TypeWrapper</para>
		/// <para>3-返回实例</para>
		/// </param>
		public SingleTypeScriptModule(int returnMode) : base(typeof(T))
		{
			this.ReturnMode = returnMode;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="returnMode">
		/// <para>Install方法返回值：</para>
		/// <para>0-返回null</para>
		/// <para>1-返回Type</para>
		/// <para>2-返回TypeWrapper</para>
		/// <para>3-返回实例</para>
		/// </param>
		public SingleTypeScriptModule(string typeName, int returnMode) : base(typeName, typeof(T))
		{
			this.ReturnMode = returnMode;
		}
	}
}
