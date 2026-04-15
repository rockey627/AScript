using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class TreeBuilder : TreeNode
	{
		private ITreeNode _Root;
		private ITreeNode _Current;
		private ITreeNode _LastResult;
		private List<Expression> _Expressions;

		public ITreeNode Root => _Root;
		public ITreeNode Current => _Current;

		public ITreeNode Pop()
		{
			var c = _Current;
			if (_Current != null)
			{
				_Current = _Current.Parent;
				if (_Current == null)
				{
					_Root = null;
				}
				else
				{
					var opNode = _Current as OperatorNode;
					while (opNode != null && opNode.Name == ".")
					{
						c = opNode;
						opNode = opNode.Parent;
					}
					_Current = opNode;
					if (opNode == null)
					{
						_Root = null;
					}
					else if (opNode.Right != null)
					{
						opNode.Right = null;
					}
					else
					{
						opNode.Left = null;
					}
				}
			}
			return c;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (_Root != null)
			{
				return _Root.Eval(context, options, control, out returnType);
			}
			if (_LastResult != null)
			{
				return _LastResult.Eval(context, options, control, out returnType);
			}
			returnType = null;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			TryEvalRoot(buildContext, scriptContext, options, null);
			if (_Expressions == null || _Expressions.Count == 0) return null;
			if (_Expressions.Count == 1) return _Expressions[0];
			return Expression.Block(_Expressions);
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(_Root);
			PoolManage.Return(_LastResult);
			_Root = null;
			_Current = null;
			_LastResult = null;
			_Expressions?.Clear();
		}

		public void SetLastResult(object data, Type dataType)
		{
			Clear();
			_LastResult = PoolManage.CreateObjectData(data, dataType);
		}

		/// <summary>
		/// <para>如果context不为空，则实时计算；</para>
		/// <para>如果当前节点是数据节点，并且又添加数据节点，则计算已有树并返回计算数据，然后清空树再添加数据节点</para>
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public TreeBuilder Add(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, ITreeNode node)
		{
			if (node == null) return this;
			if (node is OperatorNode operatorNode && !operatorNode.IsFull())
			{
				return AddOperator(buildContext, scriptContext, options, control, operatorNode);
			}
			return AddData(buildContext, scriptContext, options, control, node);
		}

		public TreeBuilder AddData(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, object data, Type dataType)
		{
			return AddData(buildContext, scriptContext, options, control, PoolManage.CreateObjectData(data, dataType));
		}

		public TreeBuilder AddData(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, TreeBuilder dataNode)
		{
			return AddData(buildContext, scriptContext, options, control, (ITreeNode)dataNode);
		}

		public TreeBuilder AddData(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, ITreeNode dataNode)
		{
			if (_Current == null && _Root != null)
			{
				// 用分号构建根节点
				var root = PoolManage.CreateOperatorNode(";", 2, 0);
				root.Left = _Root;
				_Root = _Current = root;
			}
			if (_Current == null)
			{
				_Root = _Current = dataNode;
				return this;
			}
			if (_Current is OperatorNode operatorNode)
			{
				if (operatorNode.IsFull())
				{
					TryEvalRoot(buildContext, scriptContext, options, control);
					AddData(buildContext, scriptContext, options, control, dataNode);
					return this;
				}
				operatorNode.Right = dataNode;
				_Current = dataNode;
				return this;
			}
			TryEvalRoot(buildContext, scriptContext, options, control);
			AddData(buildContext, scriptContext, options, control, dataNode);
			return this;
		}

		public TreeBuilder AddOperator(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, int dataCount, int priority)
		{
			return AddOperator(buildContext, scriptContext, options, control, PoolManage.CreateOperatorNode(name, dataCount, priority));
		}

		public TreeBuilder AddOperator(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, OperatorNode operatorNode)
		{
			if (_Current == null && _Root != null)
			{
				// 用分号构建根节点
				var root = PoolManage.CreateOperatorNode(";", 2, 0);
				root.Left = _Root;
				_Root = _Current = root;
			}
			if (_Current == null)
			{
				_Root = _Current = operatorNode;
				return this;
			}
			if (_Current is OperatorNode currentOperatorNode)
			{
				if (currentOperatorNode.IsFull())
				{
					var current = _Current;
					while (current.Parent != null && current.Parent.Priority >= operatorNode.Priority)
					{
						current = current.Parent;
					}
					// 
					var pp = current.Parent;
					if (options.CreateFullTreeNode ?? false)
					{
						operatorNode.Left = current;
					}
					else
					{
						if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
						{
							var expr = current.Build(buildContext, scriptContext, options);
							PoolManage.Return(current);
							operatorNode.Left = PoolManage.CreateExpressionNode(expr);
						}
						else
						{
							// 计算节点
							var currentResult = current.Eval(scriptContext, options, control, out var currentType);
							PoolManage.Return(current);
							operatorNode.Left = PoolManage.CreateObjectData(currentResult, currentType);
						}
					}
					if (pp == null)
					{
						_Root = operatorNode;
					}
					else
					{
						pp.Right = operatorNode;
					}
				}
				else
				{
					currentOperatorNode.Right = operatorNode;
				}
				_Current = operatorNode;
				return this;
			}
			else
			{
				var current = _Current;
				while (current.Parent != null && current.Parent.Priority >= operatorNode.Priority)
				{
					current = current.Parent;
				}
				// 
				var pp = current.Parent;
				if ((options.CreateFullTreeNode ?? false) || !(current is OperatorNode) || operatorNode.Priority == DefaultSyntaxAnalyzer.ASSIGN)
				{
					operatorNode.Left = current;
				}
				else
				{
					if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
					{
						var expr = current.Build(buildContext, scriptContext, options);
						PoolManage.Return(current);
						operatorNode.Left = PoolManage.CreateExpressionNode(expr);
					}
					else
					{
						// 计算节点
						var currentResult = current.Eval(scriptContext, options, control, out var currentType);
						PoolManage.Return(current);
						operatorNode.Left = PoolManage.CreateObjectData(currentResult, currentType);
					}
				}
				_Current = operatorNode;
				if (pp == null)
				{
					_Root = operatorNode;
				}
				else
				{
					pp.Right = operatorNode;
				}
				return this;
			}
		}

		//public bool TryCurrentEndToken(ExpressionBuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string token)
		//{
		//	if (_Current is TreeBuilder treeBuilder && treeBuilder.EndToken == token)
		//	{
		//		treeBuilder.TryEvalRoot(buildContext, scriptContext, options, control);
		//		_Current = _Current.Parent;
		//		return true;
		//	}
		//	return false;
		//}

		//public void TrySeparateOneStatement(ExpressionBuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control)
		//{
		//	var c = _Current;
		//	while (c != null && (!(c is TreeBuilder treeBuilder) || !treeBuilder.MultiStatement))
		//	{
		//		c = c.Parent;
		//		if (c != null && c is IfNode) return;
		//	}
		//	if (c != null && c is TreeBuilder treeBuilder2 && treeBuilder2.MultiStatement)
		//	{
		//		treeBuilder2.TryEvalRoot(buildContext, scriptContext, options, control);
		//	}
		//}

		public void TryEvalCurrent(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control)
		{
			if (options.CreateFullTreeNode ?? false) return;
			if (_Current == null) return;
			if (_Current is OperatorNode operatorNode && !operatorNode.IsFull())
			{
				return;
			}
			TryEvalRoot(buildContext, scriptContext, options, control);
		}

		public void TryEvalRoot(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control)
		{
			if (options.CreateFullTreeNode ?? false)
			{
				if (_Root != null)
				{
					//// 用分号构建根节点
					//var root = PoolManage.CreateOperatorNode(";", 2, 0);
					//root.Left = _Root;
					//_Root = _Current = root;
					_Current = null;
				}
				return;
			}
			//if (_Root == null)
			//{
			//	Clear();
			//	_LastResult = null;
			//}
			//else
			if (_Root != null)
			{
				if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
				{
					var expr = _Root.Build(buildContext, scriptContext, options);
					if (_Expressions == null) _Expressions = new List<Expression>();
					_Expressions.Add(expr);
					PoolManage.Return(_Root);
					_Root = _Current = null;
				}
				else
				{
					var result = _Root.Eval(scriptContext, options, control, out var resultType);
					Clear();
					_LastResult = PoolManage.CreateObjectData(result, resultType);
				}
			}
		}

		public ITreeNode EvalRoot(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control)
		{
			if (options.CreateFullTreeNode ?? false)
			{
				var r = _Root;
				_Root = _Current = _LastResult = null;
				return r;
			}
			if (_Root == null)
			{
				return null;
			}
			if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
			{
				var expr = _Root.Build(buildContext, scriptContext, options);
				PoolManage.Return(_Root);
				_Root = _Current = null;
				return PoolManage.CreateExpressionNode(expr);
			}
			else
			{
				var result = _Root.Eval(scriptContext, options, control, out var resultType);
				Clear();
				return PoolManage.CreateObjectData(result, resultType);
			}
		}

		public bool IsFullStatement()
		{
			if (_Current == null) return false;
			if (_Current is OperatorNode op)
			{
				return op.IsFull();
			}
			return true;
		}

		//public void EvalRoot(ExpressionBuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control)
		//{
		//	if (options.CreateTreeNodeOnly ?? false)
		//	{
		//		// 用分号构建根节点
		//		var root = PoolManage.CreateOperatorNode(";", 2, 0);
		//		root.Left = _Root;
		//		_Root = _Current = root;
		//		return;
		//	}
		//	//if (_Root == null)
		//	//{
		//	//	Clear();
		//	//	_LastResult = null;
		//	//}
		//	//else
		//	if (_Root != null)
		//	{
		//		if ((options.CompileMode ?? ECompileMode.None) == ECompileMode.All)
		//		{
		//			var expr = _Root.Build(buildContext, scriptContext, options);
		//			if (_Expressions == null) _Expressions = new List<Expression>();
		//			_Expressions.Add(expr);
		//			PoolManage.Return(_Root);
		//			_Root = _Current = null;
		//		}
		//		else
		//		{
		//			var result = _Root.Eval(scriptContext, options, control, out var resultType);
		//			Clear();
		//			_LastResult = PoolManage.CreateObjectData(result, resultType);
		//		}
		//	}
		//}
	}
}
