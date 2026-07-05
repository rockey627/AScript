using System;

namespace AScript.Test.MSTests
{
	public class PersonModule : IScriptModule
	{
		public object Install(BaseContext context)
		{
			if (context.EvalType("Person") != null) return null;
			context.AddType<Person>();
			context.AddFunc(typeof(PersonExtensions));
			return null;
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveType("Person");
		}
	}
}
