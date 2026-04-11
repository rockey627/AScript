using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class 中文Test
	{
		[TestInitialize]
		public void Init()
		{
			Script.Langs["中文"] = 中文语言.实例;
		}

		[TestCleanup]
		public void Cleanup()
		{
			Script.Langs.TryRemove("中文", out _);
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
整型 n=10;
文本 s='';
如果 n<5 则 {
	s='小于5';
} 否则 如果 n<20 则 {
	s='大于等于5且小于20';
} 否则 {
	s='大于等于20';
}
返回 $'{n},{s}';
";
			var script = new Script();
			Assert.AreEqual("10,大于等于5且小于20", script.Eval(s));
			Assert.AreEqual("10,大于等于5且小于20", script.Eval(s, ECompileMode.All));
		}

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

		public class 如果语法处理器 : ITokenHandler
		{
			private static readonly HashSet<string> _StatementEndTokens = new HashSet<string> { "则", "否则" };

			public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
			{
				e.IsHandled = true;
				e.End = true;

				// 如果前面有语句，则返回
				if (e.TreeBuilder.Root != null)
				{
					e.TokenReader.Push(e.CurrentToken);
					return;
				}

				var condition = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore, endTokens: _StatementEndTokens);
				analyzer.ValidateNextToken(e.TokenReader, "则");
				var createAllOptions = new BuildOptions(e.Options) { CreateFullTreeNode = true };
				var body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createAllOptions, e.TokenReader, e.Control, e.Ignore, endTokens: _StatementEndTokens);
				var node = new IfNode { Condition = condition, Body = body };
				var nextToken = e.TokenReader.Read();
				if (nextToken.HasValue && nextToken.Value.Type != ETokenType.String && nextToken.Value.Value == ";")
				{
					nextToken = e.TokenReader.Read();
				}
				if (nextToken.HasValue)
				{
					if (nextToken.Value.Value == "否则")
					{
						node.Else = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createAllOptions, e.TokenReader, e.Control, e.Ignore);
					}
					else
					{
						e.TokenReader.Push(nextToken.Value);
					}
				}
				if (!e.Ignore)
				{
					e.TreeBuilder.Add(e.BuildContext, e.ScriptContext, e.Options, e.Control, node);
				}
			}
		}
	}
}
