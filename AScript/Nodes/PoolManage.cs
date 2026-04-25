using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	/// <summary>
	/// 对象池管理，避免创建大量临时对象
	/// </summary>
	public sealed class PoolManage
	{
		//public static readonly PoolManager Instance = new PoolManager();

		// 临时数据对象池，避免创建大量临时数据
		private static readonly ConcurrentQueue<ObjectNode> _ObjectDataPool = new ConcurrentQueue<ObjectNode>();
		private static readonly ConcurrentQueue<DefineVarNode> _DefineVarNodePool = new ConcurrentQueue<DefineVarNode>();
		private static readonly ConcurrentQueue<VariableNode> _VariableDataPool = new ConcurrentQueue<VariableNode>();
		private static readonly ConcurrentQueue<OperatorNode> _OperatorNodePool = new ConcurrentQueue<OperatorNode>();
		private static readonly ConcurrentQueue<BlockNode> _BlockNodePool = new ConcurrentQueue<BlockNode>();
		private static readonly ConcurrentQueue<TreeBuilder> _TreeBuilderPool = new ConcurrentQueue<TreeBuilder>();
		private static readonly ConcurrentQueue<ExpressionNode> _ExpressionNodePool = new ConcurrentQueue<ExpressionNode>();

		//private static readonly HashSet<object> objset = new HashSet<object>();

		public static int Max { get; set; } = 20;

		private PoolManage() { }

		public static ObjectNode CreateObjectNode(object data)
		{
			return CreateObjectNode(data, null);
		}

		public static ObjectNode CreateObjectNode(object data, Type dataType)
		{
			if (_ObjectDataPool.TryDequeue(out var obj))
			{
				//objset.Remove(obj);
				obj.Data = data;
				obj.DataType = dataType;
				return obj;
			}
			return new ObjectNode(data, dataType);
		}

		public static DefineVarNode CreateDefineVarNode(string name, string type, Type systemType = null)
		{
			if (_DefineVarNodePool.TryDequeue(out var d))
			{
				//objset.Remove(d);
				d.Name = name;
				d.Type = type;
				d.SystemType = systemType;
				return d;
			}
			return new DefineVarNode { Name = name, Type = type, SystemType = systemType };
		}

		public static VariableNode CreateVariableNode(string name)
		{
			if (_VariableDataPool.TryDequeue(out var v))
			{
				//objset.Remove(v);
				v.Name = name;
				return v;
			}
			return new VariableNode(name);
		}

		public static OperatorNode CreateOperatorNode(string name, int dataCount, int priority)
		{
			if (_OperatorNodePool.TryDequeue(out var node))
			{
				//objset.Remove(node);
				node.Name = name;
				node.DataCount = dataCount;
				node.Priority = priority;
				return node;
			}
			return new OperatorNode { Name = name, DataCount = dataCount, Priority = priority };
		}

		public static TreeBuilder CreateTreeBuilder()
		{
			if (_TreeBuilderPool.TryDequeue(out var builder))
			{
				//objset.Remove(builder);
				return builder;
			}
			return new TreeBuilder();
		}

		public static ExpressionNode CreateExpressionNode(Expression expr)
		{
			if (_ExpressionNodePool.TryDequeue(out var node))
			{
				node.Expr = expr;
				return node;
			}
			return new ExpressionNode { Expr = expr };
		}

		public static BlockNode CreateBlockNode(ITreeNode body)
		{
			if (_BlockNodePool.TryDequeue(out var node))
			{
				node.Block = body;
				return node;
			}
			return new BlockNode { Block = body };
		}

		internal static void Return(IList<ITreeNode> nodes)
		{
			if (nodes == null || nodes.Count == 0) return;
			for (int i = 0; i < nodes.Count; i++)
			{
				Return(nodes[i]);
			}
		}

		internal static void Return(ITreeNode node)
		{
			if (node == null) return;
			//if (objset.Contains(node))
			//{
			//	throw new Exception("objset");
			//}
			node.Clear();
			if (node is ObjectNode objectData)
			{
				if (_ObjectDataPool.Count < Max)
				{
					//objset.Add(node);
					_ObjectDataPool.Enqueue(objectData);
				}
			}
			else if (node is DefineVarNode defineVarNode)
			{
				if (_DefineVarNodePool.Count < Max)
				{
					//objset.Add(node);
					_DefineVarNodePool.Enqueue(defineVarNode);
				}
			}
			else if (node is VariableNode variableData)
			{
				if (_VariableDataPool.Count < Max)
				{
					//objset.Add(node);
					_VariableDataPool.Enqueue(variableData);
				}
			}
			else if (node is OperatorNode operatorNode)
			{
				if (_OperatorNodePool.Count < Max)
				{
					//objset.Add(node);
					_OperatorNodePool.Enqueue(operatorNode);
				}
			}
			else if (node is TreeBuilder treeBuilder)
			{
				if (_TreeBuilderPool.Count < Max)
				{
					//objset.Add(node);
					_TreeBuilderPool.Enqueue(treeBuilder);
				}
			}
			else if (node is ExpressionNode expressionNode)
			{
				if (_ExpressionNodePool.Count < Max)
				{
					_ExpressionNodePool.Enqueue(expressionNode);
				}
			}
			else if (node is BlockNode blockNode)
			{
				if (_BlockNodePool.Count < Max)
				{
					_BlockNodePool.Enqueue(blockNode);
				}
			}
		}
	}
}
