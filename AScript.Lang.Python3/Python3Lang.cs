using AScript.Lang.Python3.Operators;
using AScript.Lang.Python3.Readers;
using AScript.Lang.Python3.TokenHandlers;
using AScript.Nodes;
using AScript.Operators;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Python3
{
	public class Python3Lang : ScriptLang
	{
		public static readonly Python3Lang Instance = new Python3Lang();

		public static readonly HashSet<string> EndTokens = new HashSet<string>() { ":", "\n" };

		public Python3Lang()
		{
			AddFunc("=", Python3AssignOperator.Instance);
			// 海象运算符：同时进行赋值和返回赋值的值
			AddFunc(":=", Python3AssignOperator.Instance);
			AddFunc("+=", PlusAssignOperator.Instance);
			AddFunc("-=", SubtractAssignOperator.Instance);
			AddFunc("*=", MultiplyAssignOperator.Instance);
			AddFunc("**=", PowerAssignOperator.Instance);
			AddFunc("/=", Python3DivideAssignOperator.Instance);
			AddFunc("//=", Python3Divide2AssignOperator.Instance);
			AddFunc("%=", ModuloAssignOperator.Instance);
			AddFunc("^=", XOrAssignOperator.Instance);
			AddFunc("&=", AndAssignOperator.Instance);
			AddFunc("|=", OrAssignOperator.Instance);
			AddFunc("?=", QuestionAssignOperator.Instance);
			AddFunc(">>=", RightShiftAssignOperator.Instance);
			AddFunc("<<=", LeftShiftAssignOperator.Instance);
			AddFunc("+", PlusOperator.Instance);
			AddFunc("-", SubtractOperator.Instance);
			AddFunc("*", MultiplyOperator.Instance);
			AddFunc("**", PowerOperator.Instance);
			AddFunc("/", Python3DivideOperator.Instance);
			AddFunc("//", Python3Divide2Operator.Instance);
			AddFunc("%", ModuloOperator.Instance);
			AddFunc("&", AndOperator.Instance);
			AddFunc("|", OrOperator.Instance);
			AddFunc("^", XOrOperator.Instance);
			AddFunc("~", NotOperator.Instance);
			AddFunc("<<", LeftShiftOperator.Instance);
			AddFunc(">>", RightShiftOperator.Instance);
			AddFunc("++", IncrementAssignOperator.Instance);
			AddFunc("--", DecrementAssignOperator.Instance);
			AddFunc("<", LessThanOperator.Instance);
			AddFunc(">", GreaterThanOperator.Instance);
			AddFunc(">=", GreaterThanOrEqualOperator.Instance);
			AddFunc("<=", LessThanOrEqualOperator.Instance);
			AddFunc("==", EqualOperator.Instance);
			AddFunc("!=", NotEqualOperator.Instance);
			AddFunc("and", AndAlsoOperator.Instance);
			AddFunc("or", OrElseOperator.Instance);

			AddFunc<ScriptContext, string, object>("exec", Exec);
			AddAction<object>("print", s => Console.WriteLine(s));

			AddTokenHandler("and", AndAlsoTokenHandler.Instance);
			AddTokenHandler("or", OrElseTokenHandler.Instance);
			AddTokenHandler("if", Python3IfTokenHandler.Instance);
			AddTokenHandler("def", Python3DefTokenHandler.Instance);
			AddTokenHandler("return", ReturnTokenHandler.Instance);

			// python中不能使用#lang，用@lang代替
			AddTokenHandler("@lang", new LangTokenHandler("@end"));
		}

		public override ITokenStream GetTokenStream(CharReader charReader)
		{
			return new Python3TokenStream(charReader);
		}

		public override int? GetOperatorPriority(string op)
		{
			switch (op)
			{
				case ":=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["="];
				case "//":
					return DefaultSyntaxAnalyzer.OperatorPriorities["/"];
				case "//=":
					return DefaultSyntaxAnalyzer.OperatorPriorities["/="];
				default:
					break;
			}
			return base.GetOperatorPriority(op);
		}

		private static object Exec(ScriptContext context, string expression)
		{
			var engine = ScriptEngine.GetCurrent(context);
			if (engine == null) throw new Exception("unkown inner ScriptEngine");
			return engine.Eval(context, expression);
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
