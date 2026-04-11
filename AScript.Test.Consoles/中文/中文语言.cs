using AScript;
using System;

namespace AScript.Test.Consoles.中文
{
	public class 中文语言 : ScriptLang
	{
		public static readonly 中文语言 实例 = new 中文语言();

		public 中文语言()
		{
			AddType<int>("整型");
			AddType<string>("文本");

			AddTokenHandler("如果", new 如果语法处理器());
			AddTokenHandler("返回", AScript.TokenHandlers.ReturnTokenHandler.Instance);
		}
	}
}
