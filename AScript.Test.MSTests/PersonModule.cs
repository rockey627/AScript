using System;

namespace AScript.Test.MSTests
{
	public class PersonModule : IScriptModule
	{
		public void Install(BaseContext context)
		{
			context.AddType<Person>();
			context.AddFunc(typeof(PersonExtensions));
		}

		public void Uninstall(BaseContext context)
		{
		}
	}
}
