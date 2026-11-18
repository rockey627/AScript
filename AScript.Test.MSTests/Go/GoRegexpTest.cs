using AScript.Lang.Go;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AScript.Test.MSTests.Go
{
	[TestClass]
	public class GoRegexpTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["go"] = GoLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("go");
		}

		private static Script NewScript(bool compileAll = false)
		{
			var script = new Script();
			if (compileAll) script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "go" };
			return script;
		}

		[TestMethod]
		public void Test01_MatchString()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\d+')
re.MatchString('abc123')
";
			var script = NewScript();
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test01_MatchString_CompileAll()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\d+')
re.MatchString('abc123')
";
			var script = NewScript(true);
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test02_FindString()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\d+')
re.FindString('abc123def456')
";
			var script = NewScript();
			Assert.AreEqual("123", script.Eval(s));
		}

		[TestMethod]
		public void Test03_FindAllString()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\d+')
var arr = re.FindAllString('a1b22c333', -1)
arr[0] + ',' + arr[1] + ',' + arr[2]
";
			var script = NewScript();
			Assert.AreEqual("1,22,333", script.Eval(s));
		}

		[TestMethod]
		public void Test04_ReplaceAllString()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('(\\w+)@(\\w+)')
re.ReplaceAllString('hello@world', '$2@$1')
";
			var script = NewScript();
			Assert.AreEqual("world@hello", script.Eval(s));
		}

		[TestMethod]
		public void Test05_Split()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile(',')
var arr = re.Split('a,b,c', -1)
arr[0] + '|' + arr[1] + '|' + arr[2]
";
			var script = NewScript();
			Assert.AreEqual("a|b|c", script.Eval(s));
		}

		[TestMethod]
		public void Test06_QuoteMeta()
		{
			var s = @"
import 'regexp'
regexp.QuoteMeta('a.b*c')
";
			var script = NewScript();
			Assert.AreEqual("a\\.b\\*c", script.Eval(s));
		}

		[TestMethod]
		public void Test07_MatchStringStatic()
		{
			var s = @"
import 'regexp'
regexp.MatchString('^a.c$', 'abc')
";
			var script = NewScript();
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test08_CompileError()
		{
			var s = @"
import 'regexp'
re, err := regexp.Compile('a(')
len(err) > 0
";
			var script = NewScript();
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test09_NamedGroup()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('(?P<key>\\w+)=(?P<value>\\w+)')
m := re.FindStringSubmatch('name=ascript')
m[1] + ':' + m[2]
";
			var script = NewScript();
			Assert.AreEqual("name:ascript", script.Eval(s));
		}

		[TestMethod]
		public void Test10_SubexpNames()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('(?P<key>\\w+)=(?P<value>\\w+)')
var names = re.SubexpNames()
names[1]
";
			var script = NewScript();
			Assert.AreEqual("key", script.Eval(s));
		}

		[TestMethod]
		public void Test11_NumSubexp()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('(\\w+)=(\\w+)')
re.NumSubexp()
";
			var script = NewScript();
			Assert.AreEqual(2, script.Eval(s));
		}

		[TestMethod]
		public void Test12_FindStringIndex()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\d+')
var idx = re.FindStringIndex('abc123')
idx[0]
";
			var script = NewScript();
			Assert.AreEqual(3, script.Eval(s));
		}

		[TestMethod]
		public void Test13_Match()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('(\\w+)@(\\w+)')
m := re.Match('hello@world')
m.String()
";
			var script = NewScript();
			Assert.AreEqual("hello@world", script.Eval(s));
		}

		[TestMethod]
		public void Test14_ReplaceAllStringFunc()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile('\\w+')
re.ReplaceAllStringFunc('a b c', func(s string) string {
	return 'x'
})
";
			var script = NewScript();
			Assert.AreEqual("x x x", script.Eval(s));
		}

		[TestMethod]
		public void Test15_RawString()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile(`\d+`)
re.FindString('abc123')
";
			var script = NewScript();
			Assert.AreEqual("123", script.Eval(s));
		}

		[TestMethod]
		public void Test16_RawString_NamedGroup()
		{
			var s = @"
import 'regexp'
re := regexp.MustCompile(`(?P<key>\w+)=(?P<value>\w+)`)
var m = re.FindStringSubmatch('name=ascript')
m[1] + ':' + m[2]
";
			var script = NewScript();
			Assert.AreEqual("name:ascript", script.Eval(s));
		}
	}
}
