using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptAxiosExtensionObject : IScriptExtensionObject
	{
		public void Init(BaseContext context)
		{
			context.SetVar("axios", new System.Net.Http.HttpClient());
			if (context is ScriptLang lang)
			{
				lang.ObjectMemberEnabledDict[typeof(JavaScriptHttpResponse)] = true;
			}
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
		}
	}
}