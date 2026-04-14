using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.Python
{
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
}
