using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptAxiosModule : IScriptModule
	{
		public void Install(BaseContext context)
		{
			context.SetVar("axios", new System.Net.Http.HttpClient());
			if (context is ScriptLang lang)
			{
				lang.ObjectMemberEnabledDict[typeof(JavaScriptHttpResponse)] = true;
			}
			context.AddFunc(typeof(JavaScriptAxiosExtensions));
		}

		public void Uninstall(BaseContext context)
		{
			context.RemoveVar("axios");
			if (context is ScriptLang lang)
			{
				lang.ObjectMemberEnabledDict.TryRemove(typeof(JavaScriptHttpResponse), out _);
			}
		}
	}
}