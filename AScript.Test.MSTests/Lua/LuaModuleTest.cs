using AScript.Lang.Lua;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Lua
{
	[TestClass]
	public class LuaModuleTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["lua"] = LuaLang.Instance;
			//LuaLang.Instance.AddModule("module", new FileScriptModule("./Lua/modules/module.lua"));
			LuaLang.Instance.Modules.AddDir("./lua/modules");
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("lua");
		}

		[TestMethod]
		public void Test00_01()
		{
			string code = @"
-- test_module.lua 文件
-- module 模块为上文提到到 module.lua
require('module')
 
print(module.constant)
module.func1()
module.func3()
";
			var script = new Script();
			script.Context.Langs = new[] { "lua" };
			script.Eval(code);
		}
	}
}
