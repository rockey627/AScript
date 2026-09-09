using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.TokenHandlers
{
	/// <summary>
	/// var a int = 5
	/// var a = 5
	/// var a int
	/// var a, b = 5, 'hello'
	/// var a, b int = 5, 10
	/// var a int, b string = 5, 'hello'
	/// </summary>
	public class GoVarTokenHandler : ITokenHandler
	{
		public static readonly GoVarTokenHandler Instance = new GoVarTokenHandler();

		public int Modifier { get; private set; }

		public GoVarTokenHandler() { }
		public GoVarTokenHandler(int modifier)
		{
			this.Modifier = modifier;
		}

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;

			if (e.TreeBuilder.IsFullStatement())
			{
				e.End = true;
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			//var defines = e.Ignore ? null : new List<DefineVarNode>();
			//Token? nextToken;
			//while (true)
			//{
			//	var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			//	string varName = nameToken.Value.Value;

			//	nextToken = e.TokenReader.Read();
			//	if (!nextToken.HasValue)
			//	{
			//		e.End = true;
			//		defines?.Add(PoolManage.CreateDefineVarNode(varName, null, modifier: this.Modifier));
			//		break;
			//	}

			//	string typeName = null;
			//	Type type = null;
			//	if (nextToken.Value.Type == ETokenType.Word)
			//	{
			//		// 类型判断
			//		typeName = nextToken.Value.Value;
			//		type = e.ScriptContext.EvalType(typeName);
			//		if (type == null)
			//		{
			//			e.End = true;
			//			e.TokenReader.Push(nextToken.Value);
			//			typeName = null;
			//		}
			//		else if (defines != null)
			//		{
			//			for (int i = 0; i < defines.Count; i++)
			//			{
			//				var defineVar = defines[i];
			//				if (defineVar.SystemType == null)
			//				{
			//					defineVar.Type = typeName;
			//					defineVar.SystemType = type;
			//				}
			//			}
			//		}
			//		// 
			//		nextToken = e.TokenReader.Read();
			//	}
			//	defines?.Add(PoolManage.CreateDefineVarNode(varName, typeName, type, modifier: this.Modifier));

			//	if (!nextToken.HasValue)
			//	{
			//		e.End = true;
			//		break;
			//	}
			//	if (nextToken.Value.IsSymbol(",")) continue;
			//	break;
			//}
			var defines = GoLang.ParseDefineVars(analyzer, e.ScriptContext, e.TokenReader, e.Ignore);
			if (defines != null)
			{
				for (int i = 0; i < defines.Count; i++)
				{
					var define = defines[i];
					define.Modifier = this.Modifier;
					if (define.SystemType == null && string.IsNullOrEmpty(define.Type))
					{
						define.SystemType = typeof(object);
					}
				}
			}
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.IsSymbol("="))
			{
				e.End = true;
				var values = e.Ignore ? null : new List<ITreeNode>();
				while (true)
				{
					var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
					values?.Add(value);
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue) break;
					if (nextToken.Value.IsSymbol(",")) continue;
					break;
				}
				if (nextToken.HasValue)
				{
					e.TokenReader.Push(nextToken.Value);
				}
				if (defines != null && defines.Count > 0)
				{
					if (defines.Count == 1)
					{
						var assign = PoolManage.CreateOperatorNode("=", 2, 0);
						assign.Left = defines[0];
						assign.Right = values[0];
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
					}
					else
					{
						var assign = PoolManage.CreateOperatorNode("=", 2, 0);
						assign.Left = new TupleNode { Items = new List<ITreeNode>(defines) };
						assign.Right = values.Count == 1 ? values[0] : new TupleNode { Items = values };
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
						//var multNode = new MultiNode
						//{
						//	Nodes = new List<ITreeNode>(Math.Max(defines.Count, values.Count))
						//};
						//int min = Math.Min(defines.Count, values.Count);
						//for (int i = 0; i < min; i++)
						//{
						//	var define = defines[i];
						//	define.Modifier = this.Modifier;
						//	var assign = PoolManage.CreateOperatorNode("=", 2, 0);
						//	assign.Left = define;
						//	assign.Right = values[i];
						//	multNode.Nodes.Add(assign);
						//}
						//for (int i = min; i < defines.Count; i++)
						//{
						//	var define = defines[i];
						//	define.Modifier = this.Modifier;
						//	multNode.Nodes.Add(define);
						//}
						//for (int i = min; i < values.Count; i++)
						//{
						//	multNode.Nodes.Add(values[i]);
						//}
						//e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, multNode);
					}
				}
			}
			else
			{
				if (nextToken.HasValue)
				{
					e.TokenReader.Push(nextToken.Value);
				}

				if (defines != null && defines.Count > 0)
				{
					if (defines.Count == 1)
					{
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, defines[0]);
					}
					else
					{
						var multNode = new MultiNode
						{
							Nodes = new List<ITreeNode>(defines)
						};
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, multNode);
					}
				}
			}
		}
	}
}
