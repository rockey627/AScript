using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace AScript.Nodes
{
	/// <summary>
	/// LINQ查询节点
	/// </summary>
	public class QueryNode : TreeNode
	{
		// 变量所属上级
		private readonly Dictionary<string, string> _VarParentDict = new Dictionary<string, string>();

		private int _ParentCounter = 0;
		private string _CurrentVarName;
		private ITreeNode _Source;

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return _Source.Build(buildContext, scriptContext, options);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			return _Source.Eval(context, options, control, out returnType);
		}

		public override bool IsFull()
		{
			return _VarParentDict.Count == 0;
		}

		public void AddFrom(string varName, ITreeNode source)
		{
			if (_Source == null)
			{
				_Source = source;
				_CurrentVarName = varName;
				return;
			}
			// _Source.SelectMany(a => source, (a, b) => new { a, b })
			var selectMany = new CallFuncNode
			{
				Name = "SelectMany",
				Args = new ITreeNode[]
				{
					// a => source
					_Source,
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = _CurrentVarName } },
						Body = source
					},
					// (a, b) => new { a, b })
					new DefineFuncNode
					{
						Args = new DefineVarNode[]
						{
							new DefineVarNode { Name = _CurrentVarName },
							new DefineVarNode { Name = varName }
						},
						Body = new NewNode
						{
							InitProperties = new ITreeNode[]
							{
								new VariableNode{Name = _CurrentVarName },
								new VariableNode{Name = varName }
							}
						}
					}
				}
			};
			_Source = selectMany;
			var oldCurrentName = _CurrentVarName;
			_CurrentVarName = $"<>h__TransparentIdentifier{_ParentCounter++}";
			_VarParentDict[oldCurrentName] = _CurrentVarName;
			_VarParentDict[varName] = _CurrentVarName;
		}

		public void AddWhere(ITreeNode condition)
		{
			// 检索condition中的变量
			if (_VarParentDict.Count > 0)
			{
				condition = VisitAndReplace(condition);
			}
			// _Source.Where(<>h__TransparentIdentifier0 => (<>h__TransparentIdentifier0.a.Age == <>h__TransparentIdentifier0.b.Age))
			var whereNode = new CallFuncNode
			{
				Name = "Where",
				Args = new ITreeNode[]
				{
					_Source,
					// <>h__TransparentIdentifier0 => (<>h__TransparentIdentifier0.a.Age == <>h__TransparentIdentifier0.b.Age)
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode { Name = _CurrentVarName } },
						Body = condition
					}
				}
			};
			_Source = whereNode;
		}

		public void AddSelect(ITreeNode selector)
		{
			// 检索selector中的变量
			if (_VarParentDict.Count > 0)
			{
				selector = VisitAndReplace(selector);
			}
			// _Source.Select(<>h__TransparentIdentifier1 => new <> f__AnonymousType0`2(Name = <> h__TransparentIdentifier1.<> h__TransparentIdentifier0.a.Name, Age = <> h__TransparentIdentifier1.<> h__TransparentIdentifier0.b.Age))
			var selectNode = new CallFuncNode
			{
				Name = "Select",
				Args = new ITreeNode[]
				{
					_Source,
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode { Name = _CurrentVarName } },
						Body = selector
					}
				}
			};
			_Source = selectNode;
			// 重置
			_VarParentDict.Clear();
			_ParentCounter = 0;
			_CurrentVarName = null;
		}

		private ITreeNode VisitAndReplace(ITreeNode node)
		{
			if (node == null) return null;
			if (node is VariableNode varNode)
			{
				string varName = varNode.Name;
				OperatorNode root = null;
				while (_VarParentDict.TryGetValue(varName, out var parentName))
				{
					var left = new OperatorNode(".", DefaultSyntaxAnalyzer.OperatorPriorities["."], 2);
					left.Left = new VariableNode(parentName);
					left.Right = root?.Left ?? new VariableNode(varName);
					if (root == null) root = left;
					else root.Left = left;
					varName = parentName;
				}
				return root ?? node;
			}
			if (node is OperatorNode opNode)
			{
				if (opNode.Name == ".")
				{
					opNode.Left = VisitAndReplace(opNode.Left);
					if (!(opNode.Right is VariableNode))
					{
						VisitAndReplace(opNode.Right);
					}
				}
				else if (opNode.Name == "=")
				{
					if (!(opNode.Left is VariableNode))
					{
						VisitAndReplace(opNode.Left);
					}
					opNode.Right = VisitAndReplace(opNode.Right);
				}
				else
				{
					VisitAndReplace(opNode.Left);
					VisitAndReplace(opNode.Right);
				}
			}
			else if (node is NewNode newNode)
			{
				if (newNode.InitProperties != null)
				{
					for (int i = 0; i < newNode.InitProperties.Count; i++)
					{
						var item = newNode.InitProperties[i];
						newNode.InitProperties[i] = VisitAndReplace(item);
					}
				}
			}
			else if (node is CallFuncNode callNode)
			{
				if (callNode.Args != null)
				{
					for (int i = 0; i < callNode.Args.Length; i++)
					{
						var item = callNode.Args[i];
						callNode.Args[i] = VisitAndReplace(item);
					}
				}
			}
			return node;
		}

		public override void Clear()
		{
			base.Clear();

			_VarParentDict.Clear();
			_ParentCounter = 0;
			_CurrentVarName = null;
			PoolManage.Return(_Source);
			_Source = null;
		}
	}
}
