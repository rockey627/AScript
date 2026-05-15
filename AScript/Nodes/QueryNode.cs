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

		//public override bool IsFull()
		//{
		//	return _VarParentDict.Count == 0;
		//}

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
					_Source,
					// a => source
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = _CurrentVarName } },
						Body = TryVisitAndReplace(source)
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
						Body = TryVisitAndReplace(condition)
					}
				}
			};
			_Source = whereNode;
		}

		public void AddSelect(ITreeNode selector)
		{
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
						Body = TryVisitAndReplace(selector)
					}
				}
			};
			_Source = selectNode;
			// 重置
			_VarParentDict.Clear();
			_ParentCounter = 0;
			_CurrentVarName = null;
		}

		public void AddJoin(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2, string intoName = null)
		{
			if (string.IsNullOrEmpty(intoName))
			{
				AddJoin1(varName, source, key1, key2);
			}
			else
			{
				AddJoin2(varName, source, key1, key2, intoName);
			}
		}

		/// <summary>
		/// join b in q2 on a.Age equals b.Age
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <param name="intoName"></param>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		private void AddJoin1(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2)
		{
			// _Source.Join(source, a => a.Age, b => b.Age, (a, b) => new <> f__AnonymousType2`2(a = a, b = b))
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression join");
			}
			var joinNode = new CallFuncNode
			{
				Name = "Join",
				Args = new ITreeNode[]
				{
					_Source,
					TryVisitAndReplace(source),
					// key1: a => a.Age
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = _CurrentVarName } },
						Body = TryVisitAndReplace(key1)
					},
					// key2: b => b.Age
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = varName } },
						Body = TryVisitAndReplace(key2)
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
			_Source = joinNode;
			var oldCurrentName = _CurrentVarName;
			_CurrentVarName = $"<>h__TransparentIdentifier{_ParentCounter++}";
			_VarParentDict[oldCurrentName] = _CurrentVarName;
			_VarParentDict[varName] = _CurrentVarName;
		}

		/// <summary>
		/// join b in q2 on a.Age equals b.Age into bb
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <param name="intoName"></param>
		private void AddJoin2(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2, string intoName)
		{
			// _Source.GroupJoin(source, a => a.Age, b => b.Age, (a, bb) => new <> f__AnonymousType2`2(a = a, bb = bb))
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression join");
			}
			var joinNode = new CallFuncNode
			{
				Name = "GroupJoin",
				Args = new ITreeNode[]
				{
					_Source,
					TryVisitAndReplace(source),
					// key1: a => a.Age
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = _CurrentVarName } },
						Body = TryVisitAndReplace(key1)
					},
					// key2: b => b.Age
					new DefineFuncNode
					{
						Args = new DefineVarNode[] { new DefineVarNode{ Name = varName } },
						Body = TryVisitAndReplace(key2)
					},
					// (a, bb) => new { a, bb })
					new DefineFuncNode
					{
						Args = new DefineVarNode[]
						{
							new DefineVarNode { Name = _CurrentVarName },
							new DefineVarNode { Name = intoName }
						},
						Body = new NewNode
						{
							InitProperties = new ITreeNode[]
							{
								new VariableNode{Name = _CurrentVarName },
								new VariableNode{Name = intoName }
							}
						}
					}
				}
			};
			_Source = joinNode;
			var oldCurrentName = _CurrentVarName;
			_CurrentVarName = $"<>h__TransparentIdentifier{_ParentCounter++}";
			_VarParentDict[oldCurrentName] = _CurrentVarName;
			_VarParentDict[intoName] = _CurrentVarName;
		}

		private ITreeNode TryVisitAndReplace(ITreeNode node)
		{
			if (_VarParentDict.Count == 0) return node;
			return VisitAndReplace(node);
		}

		private ITreeNode VisitAndReplace(ITreeNode node)
		{
			if (node == null) return null;
			if (node is DefineVarNode)
			{

			}
			else if (node is VariableNode varNode)
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
			else if (node is OperatorNode opNode)
			{
				if (opNode.Name == "." || opNode.Name == "?.")
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
				if (callNode.Target is ITreeNode targetNode)
				{
					callNode.Target = VisitAndReplace(targetNode);
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
