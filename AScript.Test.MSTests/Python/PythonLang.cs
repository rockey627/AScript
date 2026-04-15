using AScript.Nodes;
using AScript.Operators;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using Newtonsoft.Json.Linq;
using System;

namespace AScript.Test.MSTests.Python
{
	public class PythonLang : ScriptLang
	{
		public static readonly PythonLang Instance = new PythonLang();

		public static readonly HashSet<string> EndTokens = new HashSet<string>() { ":", "\n" };

		public PythonLang()
		{
			this.Compatible = false;

			AddFunc("=", AssignOperator.Instance);
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("/", DivideOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);

			AddTokenHandler("and", AndTokenHandler.Instance);
			AddTokenHandler("or", OrTokenHandler.Instance);
			AddTokenHandler("if", IfTokenHandler.Instance);
			// python中不能使用#lang，用@lang代替
			AddTokenHandler("@lang", new LangTokenHandler("@end"));
		}

		public override ITokenStream GetTokenStream(CharReader charReader)
		{
			return new PythonTokenStream(charReader);
		}

		public static TreeBuilder BuildSubBlock(int parentColumn, DefaultSyntaxAnalyzer analyzer, BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false, IEnumerable<string> endTokens = null)
		{
			var token = tokenReader.Read();
			if (!token.HasValue) return null;
			if (token.Value.Column <= parentColumn)
			{
				tokenReader.Push(token.Value);
				return null;
			}

			var builder = ignore ? null : new TreeBuilder();
			int column = token.Value.Column;
			while (token.HasValue && token.Value.Column == column)
			{
				tokenReader.Push(token.Value);
				var statement = analyzer.BuildOneStatement(buildContext, scriptContext, options, tokenReader, control, ignore, endTokens: endTokens);
				if (!ignore)
				{
					builder.Add(buildContext, scriptContext, options, control, statement);
				}
				token = tokenReader.Read();
			}
			if (token.HasValue)
			{
				tokenReader.Push(token.Value);
			}

			return builder;
		}
	}
}
