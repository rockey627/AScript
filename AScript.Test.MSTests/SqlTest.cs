using AScript.Nodes;
using AScript.Operators;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class SqlTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["sql"] = SqlLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("sql", out _);
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
bool isMatch(Person p) => 
#lang sql
p.Age>20 and p.Age<50 or (p.Name like 'to%' or p.Name='pen');
#end
var matchedList = new List<Person>();
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
			var script = new Script();
			script.Context.AddType<Person>();
			script.Context.SetVar("list", list);
			var matchedList = script.Eval<List<Person>>(s);
			Assert.AreEqual(3, matchedList.Count);
			Assert.AreEqual("tony", matchedList[0].Name);
			Assert.AreEqual("tom", matchedList[1].Name);
			Assert.AreEqual("san", matchedList[2].Name);
		}

		[TestMethod]
		public void Test03_2()
		{
			string s = @"
bool isMatch(Person p) => #lang sql p.Age>20 and p.Age<50 or p.Name like 'to%'; #end
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
				new Person("san", 25),
				new Person("lin", 70)
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
bool isMatch(Person p) => #lang sql p.Age>20 and p.Age<50 or p.Name like 'to%'; #end
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
				new Person("san", 25),
				new Person("lin", 70)
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
		public void Test02_2()
		{
			string s = "p.Name like 'to%' or p.Age>20 and p.Age<50";
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			var matchFunc = script.Compile<Person, bool>(s, "p");
			var list = new List<Person>
			{
				new Person("jim", 18),
				new Person("tony", 60),
				new Person("tom", 19),
				new Person("san", 25),
				new Person("lin", 70)
			};
			var matchedList = list.Where(matchFunc).ToList();
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
			script.Context.Langs = new[] { "sql" };
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
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='san'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Name = "san";
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = "p.Age>20 and p.Age<50 or p.Name like 'to%' or p.Name='san'";
			var p = new Person("tom", 60);
			var script = new Script();
			script.Context.Langs = new[] { "sql" };
			script.Context.SetVar("p", p);
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			Assert.AreEqual(false, script.Eval(s));
			p.Name = "san";
			Assert.AreEqual(true, script.Eval(s));
			p.Name = "jim";
			p.Age = 30;
			Assert.AreEqual(true, script.Eval(s));
		}

		public class SqlLang : ScriptLang
		{
			public static readonly SqlLang Instance = new SqlLang();

			public SqlLang()
			{
				this.Compatible = false;

				AddFunc(".", DotOperator.Instance);
				AddFunc("!", BoolNotOperator.Instance);
				AddFunc("<", LessThanOperator.Instance);
				AddFunc(">", GreaterThanOperator.Instance);
				AddFunc("=", EqualOperator.Instance);
				AddFunc(">=", GreaterThanOrEqualOperator.Instance);
				AddFunc("<=", LessThanOrEqualOperator.Instance);
				AddFunc("!=", NotEqualOperator.Instance);
				AddFunc("and", AndAlsoOperator.Instance);
				AddFunc("or", OrElseOperator.Instance);

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
					var op = new OperatorNode("and", DefaultSyntaxAnalyzer.OperatorPriorities["&&"], 2);
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
					var op = new OperatorNode("or", DefaultSyntaxAnalyzer.OperatorPriorities["||"], 2);
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
					var op = new OperatorNode("=", DefaultSyntaxAnalyzer.OperatorPriorities["=="], 2);
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

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				var arg1 = e.TreeBuilder.Pop();
				var arg2String = analyzer.ValidateNextToken(e.TokenReader, ETokenType.String).Value.Value;
				if (!e.Ignore)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new LikeNode { Arg1 = arg1, Arg2 = arg2String });
				}
			}
		}

		public class LikeNode : TreeNode
		{
			private static readonly Expression Constant_false = Expression.Constant(false);
			private static readonly Expression Constant_null = Expression.Constant(null);
			private static readonly Expression Constant_StringComparison_OrdinalIgnoreCase = Expression.Constant(StringComparison.OrdinalIgnoreCase);
			private static readonly MethodInfo Method_String_EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });
			private static readonly MethodInfo Method_String_StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });
			private static readonly MethodInfo Method_String_Contains = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });
			private static readonly MethodInfo Method_String_Equals = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(StringComparison) });

			public ITreeNode Arg1 { get; set; }
			public string Arg2 { get; set; }

			public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
			{
				var s1 = this.Arg1.Build(buildContext, scriptContext, options);
				return LikeBuild(s1, this.Arg2);
			}

			public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
			{
				var s1 = this.Arg1.Eval(context, options, control, out _)?.ToString();
				returnType = typeof(bool);
				return LikeEval(s1, this.Arg2);
			}

			// 这里只考虑首尾%的情况
			public static bool LikeEval(string s1, string s2)
			{
				if (string.IsNullOrEmpty(s1)) return false;
				if (string.IsNullOrEmpty(s2)) return true;
				Parse(s2, out int mode, out string pattern);
				if (mode == 1)
				{
					return s1.EndsWith(pattern, StringComparison.OrdinalIgnoreCase);
				}
				if (mode == 2)
				{
					return s1.StartsWith(pattern, StringComparison.OrdinalIgnoreCase);
				}
				if (mode == 3)
				{
					return s1.Contains(pattern, StringComparison.OrdinalIgnoreCase);
				}
				return s1.Equals(pattern, StringComparison.OrdinalIgnoreCase);
			}

			public static Expression LikeBuild(Expression s1, string s2)
			{
				if (s2 == null) return Constant_false;
				var s1NotNull = Expression.NotEqual(s1, Constant_null);
				if (s2 == "") return s1NotNull;
				Parse(s2, out int mode, out string pattern);
				if (mode == 1)
				{
					var endsWith = Expression.Call(s1, Method_String_EndsWith, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
					return Expression.AndAlso(s1NotNull, endsWith);
				}
				if (mode == 2)
				{
					var startsWith = Expression.Call(s1, Method_String_StartsWith, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
					return Expression.AndAlso(s1NotNull, startsWith);
				}
				if (mode == 3)
				{
					var contains = Expression.Call(s1, Method_String_Contains, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
					return Expression.AndAlso(s1NotNull, contains);
				}
				var equals = Expression.Call(s1, Method_String_Equals, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
				return Expression.AndAlso(s1NotNull, equals);
			}

			private static void Parse(string s, out int mode, out string pattern)
			{
				if (s[0] == '%')
				{
					if (s[s.Length - 1] == '%')
					{
						mode = 3;
						pattern = s.Substring(1, s.Length - 2);
						return;
					}
					mode = 1;
					pattern = s.Substring(1);
				}
				else if (s[s.Length - 1] == '%')
				{
					mode = 2;
					pattern = s.Substring(0, s.Length - 1);
				}
				else
				{
					mode = 0;
					pattern = s;
				}
			}
		}
	}
}
