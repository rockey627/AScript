using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Python3
{
	public class Python3SyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly Python3SyntaxAnalyzer Instance = new Python3SyntaxAnalyzer();

		private static readonly HashSet<string> _EndTokens = new HashSet<string> { "\n" };

		public override ITreeNode BuildMultiStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			if (endTokens == null) endTokens = _EndTokens;
			return base.BuildMultiStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens);
		}

		public override ITreeNode BuildOneStatement(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, bool noblock = false, IEnumerable<string> endTokens = null)
		{
			if (endTokens == null) endTokens = _EndTokens;
			return base.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, noblock, endTokens);
		}
	}
}
