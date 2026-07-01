using System;

namespace AScript.Extensions.NewtonsoftJson
{
	public class NewtonsoftJsonExtensionObject : IScriptExtensionObject
	{
		public void Init(BaseContext context)
		{
			context.AddFunc(typeof(NewtonsoftJsonExtensions));
		}
	}
}
