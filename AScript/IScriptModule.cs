using System;

namespace AScript
{
	public interface IScriptModule
	{
		object Install(BaseContext context);
		void Uninstall(BaseContext context);
	}
}
