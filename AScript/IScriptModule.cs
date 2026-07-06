using System;

namespace AScript
{
	public interface IScriptModule
	{
		object Install(BaseContext context);
		void Uninstall(BaseContext context);
	}

	public interface IScriptModuleType
	{
		/// <summary>
		/// 模块类型，Install返回的模块实例类型，主要用于编译模式下获取模块类型
		/// </summary>
		Type ModuleType { get; }
	}
}
