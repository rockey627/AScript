using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class SqlTest
	{
		[TestInitialize]
		public void Init()
		{
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[TestCleanup]
		public void Cleanup()
		{
			Script.Langs.TryRemove("sql", out _);
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
bool isMatch(Person p) => p.Age>20 and p.Age<50 or p.Name like 'to%';
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var matchedList = new List<Person>();
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			script.Context.SetVar("matchedList", matchedList);
			script.Eval(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
bool isMatch(Person p) => p.Age>20 and p.Age<50 or p.Name like 'to%';
foreach(var item in list) {
	if (isMatch(item)) matchedList.Add(item);
}
matchedList;
";
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var matchedList = new List<Person>();
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			script.Context.SetVar("matchedList", matchedList);
			script.Eval(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test02()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%'";
			var script = new Script();
			var matchFunc = script.Compile<Person, bool>(s, "p");
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25)
			};
			var matchedList = list.Where(matchFunc).ToList();
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test01_2()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}

		public class SqlLang : ScriptLang
		{
			public static readonly SqlLang Instance = new SqlLang();

			public SqlLang()
			{
				AddTokenHandler("and", AndTokenHandler.Instance);
				AddTokenHandler("AND", AndTokenHandler.Instance);
				AddTokenHandler("or", OrTokenHandler.Instance);
				AddTokenHandler("OR", OrTokenHandler.Instance);
				AddTokenHandler("=", EqualTokenHandler.Instance);
				AddTokenHandler("like", LikeTokenHandler.Instance);
				AddTokenHandler("LIKE", LikeTokenHandler.Instance);
			}
		}

		/// <summary>
		/// <![CDATA[age>20 and age<50]]>
		/// </summary>
		public class AndTokenHandler : ITokenHandler
		{
			public static readonly AndTokenHandler Instance = new AndTokenHandler();

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				if (!e.Ignore)
				{
					var op = new OperatorNode("&&", DefaultSyntaxAnalyzer.OperatorPriorities["&&"], 2);
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
				}
			}
		}

		/// <summary>
		/// name='tom' or id=8
		/// </summary>
		public class OrTokenHandler : ITokenHandler
		{
			public static readonly OrTokenHandler Instance = new OrTokenHandler();

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				if (!e.Ignore)
				{
					var op = new OperatorNode("||", DefaultSyntaxAnalyzer.OperatorPriorities["||"], 2);
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
				}
			}
		}

		/// <summary>
		/// id=6
		/// </summary>
		public class EqualTokenHandler : ITokenHandler
		{
			public static readonly EqualTokenHandler Instance = new EqualTokenHandler();

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				if (!e.Ignore)
				{
					var op = new OperatorNode("==", DefaultSyntaxAnalyzer.OperatorPriorities["=="], 2);
					e.TreeBuilder.AddOperator(e.BuildContext, e.ScriptContext, e.Options, e.Control, op);
				}
			}
		}

		/// <summary>
		/// title like '%happy%'
		/// </summary>
		public class LikeTokenHandler : ITokenHandler
		{
			public static readonly LikeTokenHandler Instance = new LikeTokenHandler();

			private static MethodInfo _LikeMethod = typeof(LikeTokenHandler).GetMethod("Like");

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				var arg1 = e.TreeBuilder.Pop();
				var arg2String = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
				if (!e.Ignore)
				{
					ITreeNode result;
					if ((e.Options.CreateFullTreeNode ?? false) 
						|| e.Options.CompileMode.HasValue && e.Options.CompileMode == ECompileMode.All)
					{
						result = new CallFuncNode
						{
							Method = _LikeMethod,
							Args = new ITreeNode[] { arg1, new ObjectNode(arg2String, typeof(string)) }
						};
					}
					else
					{
						// 立即执行
						var s1 = arg1.Eval(e.ScriptContext, e.Options, e.Control, out _)?.ToString();
						var b = Like(s1, arg2String);
						result = new ObjectNode(b);
					}
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, result);
				}
			}

			// 这里只考虑首尾%的情况
			public static bool Like(string s1, string s2)
			{
				if (string.IsNullOrEmpty(s1)) return false;
				if (string.IsNullOrEmpty(s2)) return true;
				if (s2[0] == '%')
				{
					if (s2[s2.Length - 1] == '%')
					{
						return s1.Contains(s2.Substring(1, s2.Length - 2), StringComparison.OrdinalIgnoreCase);
					}
					return s1.EndsWith(s2.Substring(1), StringComparison.OrdinalIgnoreCase);
				}
				if (s2[s2.Length - 1] == '%')
				{
					return s1.StartsWith(s2.Substring(0, s2.Length - 1), StringComparison.OrdinalIgnoreCase);
				}
				return s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
