using System;

namespace AScript.Lang.Lua.io
{
	public class LuaIOModule : IScriptModule
	{
		public object Install(BaseContext context)
		{
			var io = new LuaIO();
			context.SetVar("io", io);
			return io;
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveVar("io");
		}
	}
}
