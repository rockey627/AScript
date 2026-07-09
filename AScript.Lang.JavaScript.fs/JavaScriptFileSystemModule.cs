using System;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptFileSystemModule : IScriptModule, IScriptModuleType
	{
		public Type ModuleType => typeof(JavaScriptFileSystem);

		public object Install(BaseContext context)
		{
			context.SetObjectMemberEnabled(this.ModuleType, true);
			context.SetObjectMemberEnabled(typeof(JavaScriptReadStream), true);
			context.SetObjectMemberEnabled(typeof(JavaScriptWriteStream), true);
			return new JavaScriptFileSystem();
		}

		public void Uninstall(BaseContext context)
		{
			context.SetObjectMemberEnabled(this.ModuleType, null);
			context.SetObjectMemberEnabled(typeof(JavaScriptReadStream), null);
			context.SetObjectMemberEnabled(typeof(JavaScriptWriteStream), null);
		}
	}
}
