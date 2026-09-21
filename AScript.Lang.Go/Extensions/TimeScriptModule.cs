using System;

namespace AScript.Lang.Go.Extensions
{
	public class TimeScriptModule : IScriptModule
	{
		public object Install(BaseContext context)
		{
			context.AddType("time", typeof(DateTime));
			context.AddFunc(typeof(TimeExtensions));
			return new TypeWrapper("time", typeof(DateTime));
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType("time");
		}
	}
}
