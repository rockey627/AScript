using System;
using AScript.Nodes;
using AScript.Readers;

namespace AScript
{
	/// <summary>
	/// <para>语法分析器</para>
	/// <para>输入：记号流</para>
	/// <para>输出：语法树</para>
	/// </summary>
	public interface ISyntaxAnalyzer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="tokenReader"></param>
		/// <returns></returns>
		ITreeNode Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, TokenReader tokenReader);
	}
}
