using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System.Collections.Generic;

namespace AScript.Lang.Go
{
	public class GoSyntaxAnalyzer : DefaultSyntaxAnalyzer
	{
		public static readonly GoSyntaxAnalyzer Instance = new GoSyntaxAnalyzer();

		//protected override ITreeNode BuildBlock(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore = false)
		//{
		//	// 解析map：make(map[string]int) 或 map[string]int{}
		//	var token = tokenReader.Read();
		//	if (!token.HasValue) return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	tokenReader.Push(token.Value);

		//	// 检查是否是map字面量
		//	if (token.Value.IsSymbol("map"))
		//	{
		//		return BuildMapLiteral(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}

		//	return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//}

		//private ITreeNode BuildMapLiteral(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader, EvalControl control, bool ignore)
		//{
		//	var token = tokenReader.Read(); // 跳过map
		//	token = tokenReader.Read(); // 应该是[
		//	if (!token.HasValue || !token.Value.IsSymbol("["))
		//	{
		//		tokenReader.Push(token.Value);
		//		return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}
		//	// 跳过key类型
		//	token = tokenReader.Read();
		//	if (!token.HasValue)
		//	{
		//		tokenReader.Push(token.Value);
		//		return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}
		//	token = tokenReader.Read(); // 应该是]
		//	if (!token.HasValue || !token.Value.IsSymbol("]"))
		//	{
		//		tokenReader.Push(token.Value);
		//		return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}
		//	token = tokenReader.Read(); // value type
		//	if (!token.HasValue)
		//	{
		//		tokenReader.Push(token.Value);
		//		return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}
		//	token = tokenReader.Read(); // 检查是否是{或make
		//	if (!token.HasValue)
		//	{
		//		tokenReader.Push(token.Value);
		//		return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//	}

		//	tokenReader.Push(token.Value);
		//	return base.BuildBlock(buildContext, scriptContext, options, tokenReader, control, ignore);
		//}

		//protected override object EvalNumber(string num)
		//{
		//	return ScriptUtils.EvalNumber(num, true);
		//}
	}
}
