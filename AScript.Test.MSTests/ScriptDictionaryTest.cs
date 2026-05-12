using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptDictionaryTest
	{
		[TestMethod]
		public void Test02_2()
		{
			var s = @"
var d = new Dictionary<string, int>();
d['age'] = 20;
d['age']++;
d['age']--;
d['age'] += 10;
d['age'] -= 10;
d['age'] *= 10;
d['age'] /= 10;
d['age']
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			var d = new Dictionary<string, int>();
			d["age"] = 20;
			d["age"] += 10;
			Assert.AreEqual(30, d["age"]);

			var s = @"
var d = new Dictionary<string, int>();
d['age'] = 20;
d['age']++;
d['age']--;
d['age'] += 10;
d['age'] -= 10;
d['age'] *= 10;
d['age'] /= 10;
d['age']
";
			var script = new Script();
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void Test01_2()
		{
			var s = @"
var d = new Dictionary<string, object>();
d['name']='tom';
d['age'] = 20;
d['age'];
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual(20, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			var s = @"
var d = new Dictionary<string, object>();
d['name']='tom';
d['age'] = 20;
d['age'];
";
			var script = new Script();
			Assert.AreEqual(20, script.Eval(s));
		}
	}
}
