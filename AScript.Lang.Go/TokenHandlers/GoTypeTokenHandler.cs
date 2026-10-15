using AScript.Lang.Go.Nodes;
using AScript.Lang.Go.Types;
using AScript.Nodes;
using AScript.Syntaxs;
using System;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// type Person struct {
	///		name string
	///		age int
	/// }
	/// </summary>
	public class GoTypeTokenHandler : ITokenHandler
	{
		public static readonly GoTypeTokenHandler Instance = new GoTypeTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			// 解析类型名称
			var typeNameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			string typeName = typeNameToken.Value.Value;

			// 验证 struct 关键字
			analyzer.ValidateNextToken(e.TokenReader, "struct");

			// 验证左大括号
			analyzer.ValidateNextToken(e.TokenReader, "{");

			// 解析字段列表
			var classDefine = e.Ignore ? null : new ClassDefine(typeName);
			while (true)
			{
				var fieldToken = analyzer.ValidateNextToken(e.TokenReader);
				if (fieldToken.Value.IsSymbol("}"))
				{
					break;
				}

				if (fieldToken.Value.Type != ETokenType.Word)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid token '{fieldToken.Value.Value}' at ({fieldToken.Value.Line},{fieldToken.Value.Column}), expect field name");
				}

				string fieldName = fieldToken.Value.Value;

				// 解析字段类型
				var fieldType = GoLang.ParseType(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
				classDefine?.AddProperty(fieldName, fieldType.RealType);
			}

			if (!e.Ignore)
			{
				e.ScriptContext.AddType(typeName, classDefine);
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectNode(classDefine));
			}
		}
	}
}
