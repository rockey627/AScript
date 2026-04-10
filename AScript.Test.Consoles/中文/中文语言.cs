using AScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles.中文
{
	public class 中文语言 : ScriptLang
	{
		public static readonly 中文语言 实例 = new 中文语言();

		public 中文语言()
		{
			AddType<int>("整型");

			AddTokenHandler("如果", new 如果语法处理器());
		}
	}
}
