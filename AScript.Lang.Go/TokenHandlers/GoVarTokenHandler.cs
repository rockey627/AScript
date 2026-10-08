using AScript.Lang.Go.Nodes;
using AScript.Lang.Go.Types;
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

			var defines = GoLang.ParseDefineVars(analyzer, e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Ignore);
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
				var fullOptions = new BuildOptions(e.Options) { CreateFullStatement = true };
				while (true)
				{
					var value = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, fullOptions, e.TokenReader, e.Control, e.Ignore);
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
					else if (defines.Count == 2 && values.Count == 1)
					{
						var indexValue = values[0];
						if (indexValue is OperatorNode op && op.Name == "[]")
						{
							// v, ok = map[key]
							var assign = PoolManage.CreateOperatorNode("=", 2, 0);
							assign.Left = new TupleNode { Items = new List<ITreeNode>(defines) };
							assign.Right = new CallFuncNode { Name = "__GetMapValue__", Args = new[] { op.Left, op.Right } };
							e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
						}
						else
						{
							var assign = PoolManage.CreateOperatorNode("=", 2, 0);
							assign.Left = new TupleNode { Items = new List<ITreeNode>(defines) };
							assign.Right = values.Count == 1 ? values[0] : new TupleNode { Items = values };
							e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
						}
					}
					else
					{
						var assign = PoolManage.CreateOperatorNode("=", 2, 0);
						assign.Left = new TupleNode { Items = new List<ITreeNode>(defines) };
						assign.Right = values.Count == 1 ? values[0] : new TupleNode { Items = values };
						e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, assign);
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
					var multNode = new MultiNode { Nodes = new List<ITreeNode>() };
					foreach (var item in defines)
					{
						multNode.Nodes.Add(item);
						if (item.GoType is GoRuntimeType goRuntimeType && goRuntimeType.RealType.IsClass)
						{
							var assign = new OperatorNode("=", 0, 2)
							{
								Left = new VariableNode(item.Name),
								Right = new NewNode { SystemType = item.SystemType }
							};
							multNode.Nodes.Add(assign);
							continue;
						}
					}
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, multNode);
				}
			}
		}
	}
}
