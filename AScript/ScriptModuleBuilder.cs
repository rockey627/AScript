using System;

namespace AScript
{
	public class ScriptModuleBuilder
	{
		private Func<BaseContext, object> _OnInstall;
		private Action<BaseContext> _OnUninstall;

		public ScriptModuleBuilder OnInstall(Func<BaseContext, object> onInstall)
		{
			_OnInstall = onInstall;
			return this;
		}

		public ScriptModuleBuilder OnUninstall(Action<BaseContext> onUninstall)
		{
			_OnUninstall = onUninstall;
			return this;
		}

		public IScriptModule Build()
		{
			return new CustomScriptModule(_OnInstall, _OnUninstall);
		}

		private class CustomScriptModule : IScriptModule
		{
			private readonly Func<BaseContext, object> _OnInstall;
			private readonly Action<BaseContext> _OnUninstall;

			public CustomScriptModule(Func<BaseContext, object> onInstall, Action<BaseContext> onUninstall)
			{
				_OnInstall = onInstall;
				_OnUninstall = onUninstall;
			}

			public object Install(BaseContext context)
			{
				return _OnInstall?.Invoke(context);
			}

			public void Uninstall(BaseContext context)
			{
				_OnUninstall?.Invoke(context);
			}
		}
	}
}
