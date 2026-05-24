using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	/// <summary>
	/// LINQ查询节点
	/// </summary>
	public class QueryNode : TreeNode
	{
		// 变量所属上级（变量聚合）
		private readonly Dictionary<string, string> _VarParentDict = new Dictionary<string, string>();

		// 变量所属上级计数（变量聚合计数）
		private int _ParentCounter = 0;
		// 当前变量名
		private string _CurrentVarName;
		// 当前数据源
		private ITreeNode _Source;

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return _Source.Build(buildContext, scriptContext, options);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			return _Source.Eval(context, options, control, out returnType);
		}

		/// <summary>
		/// from varName in source
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		public void AddFrom(string varName, ITreeNode source)
		{
			if (_Source == null)
			{
				// 第1个from语句
				_Source = source;
				_CurrentVarName = varName;
				return;
			}
			// _Source.SelectMany(_CurrentVarName => source, (_CurrentVarName, varName) => new { _CurrentVarName, varName })
			var selectMany = new CallFuncNode
			{
				Name = "SelectMany",
				Args = new ITreeNode[]
				{
					_Source,
					// _CurrentVarName => source
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(source)
					},
					// (_CurrentVarName, varName) => new { _CurrentVarName, varName }
					new DefineFuncNode
					{
						Args = new[]
						{
							new DefineVarNode(_CurrentVarName),
							new DefineVarNode(varName)
						},
						Body = new NewNode
						{
							InitProperties = new ITreeNode[]
							{
								new VariableNode(_CurrentVarName),
								new VariableNode(varName)
							}
						}
					}
				}
			};
			// 更新当前数据源
			_Source = selectMany;
			// 变量聚合
			var oldCurrentName = _CurrentVarName;
			_CurrentVarName = $"<>h__TransparentIdentifier{_ParentCounter++}";
			_VarParentDict[oldCurrentName] = _CurrentVarName;
			_VarParentDict[varName] = _CurrentVarName;
		}

		/// <summary>
		/// from a in query1
		/// from b in query2
		/// where a.Age == b.Age
		/// </summary>
		/// <param name="condition"></param>
		public void AddWhere(ITreeNode condition)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression where");
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
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(condition)
					}
				}
			};
			// 更新当前数据源
			_Source = whereNode;
		}

		/// <summary>
		/// select new { a.Name, b.Age }
		/// </summary>
		/// <param name="selector"></param>
		public void AddSelect(ITreeNode selector)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression select");
			}
			// _Source.Select(<>h__TransparentIdentifier0 => new <> f__AnonymousType0`2(Name = <> h__TransparentIdentifier0.a.Name, Age = <> h__TransparentIdentifier0.b.Age))
			var selectNode = new CallFuncNode
			{
				Name = "Select",
				Args = new ITreeNode[]
				{
					_Source,
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(selector)
					}
				}
			};
			// 更新当前数据源
			_Source = selectNode;
			// 重置变量
			_VarParentDict.Clear();
			_ParentCounter = 0;
			_CurrentVarName = null;
		}

		/// <summary>
		/// join varName in source on a.Age equals varName.Age into intoName
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <param name="intoName"></param>
		public void AddJoin(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2, string intoName = null)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression join");
			}
			if (string.IsNullOrEmpty(intoName))
			{
				AddJoin1(varName, source, key1, key2);
			}
			else
			{
				AddJoin2(varName, source, key1, key2, intoName);
			}
		}

		public void AddLeftJoin(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2)
		{
			string name1 = $"___{varName}___";
			AddJoin(varName, source, key1, key2, name1);
			AddFrom(varName, new CallFuncNode { Name = "DefaultIfEmpty", Args = new ITreeNode[] { new VariableNode(name1) } });
		}

		public void AddRightJoin(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2)
		{
			var right = _Source;
			var rightName = _CurrentVarName;
			_Source = source;
			_CurrentVarName = varName;
			AddLeftJoin(rightName, right, key2, key1);
		}

		/// <summary>
		/// group a.Name by a.Age into intoName
		/// </summary>
		/// <param name="key"></param>
		/// <param name="element"></param>
		/// <param name="intoName"></param>
		public void AddGroup(ITreeNode key, ITreeNode element, string intoName = null)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression join");
			}
			// _Source.GroupBy(a => a.Age, a => a.Name)
			bool hasElement;
			if (element == null) hasElement = false;
			else if (element is VariableNode elementVarNode && elementVarNode.Name == _CurrentVarName)
			{
				hasElement = false;
			}
			else
			{
				hasElement = true;
			}

			ITreeNode group;
			if (hasElement)
			{
				group = new CallFuncNode
				{
					Name = "GroupBy",
					Args = new ITreeNode[]
					{
						_Source,
						// key: a => a.Age
						new DefineFuncNode
						{
							Args = new[] { new DefineVarNode(_CurrentVarName) },
							Body = TryVisitAndReplace(key)
						},
						// element: a => a.Name
						new DefineFuncNode
						{
							Args = new[] { new DefineVarNode(_CurrentVarName) },
							Body = TryVisitAndReplace(element)
						},
					}
				};
			}
			else
			{
				group = new CallFuncNode
				{
					Name = "GroupBy",
					Args = new ITreeNode[]
					{
						_Source,
						// key: a => a.Age
						new DefineFuncNode
						{
							Args = new[] { new DefineVarNode(_CurrentVarName) },
							Body = TryVisitAndReplace(key)
						}
					}
				};
			}
			// 更新当前数据源
			_Source = group;
			// 重置变量
			_CurrentVarName = intoName;
			_VarParentDict.Clear();
			_ParentCounter = 0;
		}

		/// <summary>
		/// orderby a.Age descending
		/// </summary>
		/// <param name="key"></param>
		/// <param name="mode">ascending（默认）/descending</param>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		public void AddOrderby(ITreeNode key, string mode)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression orderby");
			}
			// _Source.OrderByDescending(a => a.Age)
			var orderby = new CallFuncNode
			{
				Name = mode == "desc" || mode == "descending" ? "OrderByDescending" : "OrderBy",
				Args = new ITreeNode[]
				{
					_Source,
					// key: a => a.Age
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(key)
					}
				}
			};
			// 更新当前数据源
			_Source = orderby;
		}

		public void AddThenby(ITreeNode key, string mode)
		{
			if (_Source == null)
			{
				throw new Exceptions.ScriptAnalyzingException("invalid expression orderby");
			}
			// _Source.OrderByDescending(a => a.Age)
			var thenby = new CallFuncNode
			{
				Name = mode == "desc" || mode == "descending" ? "ThenByDescending" : "ThenBy",
				Args = new ITreeNode[]
				{
					_Source,
					// key: a => a.Age
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(key)
					}
				}
			};
			// 更新当前数据源
			_Source = thenby;
		}

		/// <summary>
		/// join varName in source on a.Age equals varName.Age
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <exception cref="Exceptions.ScriptAnalyzingException"></exception>
		private void AddJoin1(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2)
		{
			// _Source.Join(source, a => a.Age, varName => varName.Age, (a, varName) => new <> f__AnonymousType2`2(a = a, varName = varName))
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
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(key1)
					},
					// key2: varName => varName.Age
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(varName) },
						Body = TryVisitAndReplace(key2)
					},
					// (a, varName) => new { a, varName })
					new DefineFuncNode
					{
						Args = new[]
						{
							new DefineVarNode(_CurrentVarName),
							new DefineVarNode(varName)
						},
						Body = new NewNode
						{
							InitProperties = new ITreeNode[]
							{
								new VariableNode(_CurrentVarName),
								new VariableNode(varName)
							}
						}
					}
				}
			};
			// 更新当前数据源
			_Source = joinNode;
			// 变量聚合
			var oldCurrentName = _CurrentVarName;
			_CurrentVarName = $"<>h__TransparentIdentifier{_ParentCounter++}";
			_VarParentDict[oldCurrentName] = _CurrentVarName;
			_VarParentDict[varName] = _CurrentVarName;
		}

		/// <summary>
		/// join varName in source on a.Age equals varName.Age into intoName
		/// </summary>
		/// <param name="varName"></param>
		/// <param name="source"></param>
		/// <param name="key1"></param>
		/// <param name="key2"></param>
		/// <param name="intoName"></param>
		private void AddJoin2(string varName, ITreeNode source, ITreeNode key1, ITreeNode key2, string intoName)
		{
			// _Source.GroupJoin(source, a => a.Age, varName => varName.Age, (a, intoName) => new <> f__AnonymousType2`2(a = a, intoName = intoName))
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
						Args = new[] { new DefineVarNode(_CurrentVarName) },
						Body = TryVisitAndReplace(key1)
					},
					// key2: varName => varName.Age
					new DefineFuncNode
					{
						Args = new[] { new DefineVarNode(varName) },
						Body = TryVisitAndReplace(key2)
					},
					// (a, intoName) => new { a, intoName })
					new DefineFuncNode
					{
						Args = new[]
						{
							new DefineVarNode(_CurrentVarName),
							new DefineVarNode(intoName)
						},
						Body = new NewNode
						{
							InitProperties = new ITreeNode[]
							{
								new VariableNode(_CurrentVarName),
								new VariableNode(intoName)
							}
						}
					}
				}
			};
			// 更新当前数据源
			_Source = joinNode;
			// 变量聚合
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
