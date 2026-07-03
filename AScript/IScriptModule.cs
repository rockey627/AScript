using System;

namespace AScript
{
	public interface IScriptModule
	{
		void Install(BaseContext context);
		void Uninstall(BaseContext context);
	}
}
