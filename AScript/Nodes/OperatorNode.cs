using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class OperatorNode : TreeNode
	{
		private static readonly ConcurrentQueue<ITreeNode[]> _queue1 = new ConcurrentQueue<ITreeNode[]>();
		private static readonly ConcurrentQueue<ITreeNode[]> _queue2 = new ConcurrentQueue<ITreeNode[]>();

		private ITreeNode _Left;
		private ITreeNode _Right;

		public string Name { get; set; }
		public int Priority { get; set; }
		public int DataCount { get; set; }

		public ITreeNode Left
		{
			get => _Left;
			set
			{
				if (value != null) value.Parent = this;
				_Left = value;
			}
		}
		public ITreeNode Right
		{
			get => _Right;
			set
			{
				if (value != null) value.Parent = this;
				_Right = value;
			}
		}

		public bool IsFull()
		{
			return _Right != null || this.DataCount == GetArgsCount();
		}

		public bool IsPrefix()
		{
			return _Left == null && _Right != null;
		}

		private int GetArgsCount()
		{
			if (_Left == null)
			{
				return _Right == null ? 0 : 1;
			}
			return _Right == null ? 1 : 2;
		}

		private ITreeNode[] GetArgs(ScriptContext context)
		{
			int count = GetArgsCount();
			var datas = CreateArray(count);
			//var datas = new ITreeNode[count];
			if (this._Left != null)
			{
				datas[0] = this._Left;
			}
			if (this._Right != null)
			{
				int index = this._Left == null ? 0 : 1;
				datas[index] = this._Right;
			}
			return datas;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.Name == ";")
			{
				object result = null;
				Type type = null;
				if (this._Left != null)
				{
					result = this._Left.Eval(context, options, control, out type);
					if (control != null && (control.Continue || control.Break || control.Terminal))
					{
						returnType = type;
						return result;
					}
				}
				if (this._Right != null)
				{
					result = this._Right.Eval(context, options, control, out type);
				}
				returnType = type;
				return result;
			}
			else
			{
				var args = GetArgs(context);
				try
				{
					return context.EvalFunc(options, control, this.Name, IsPrefix(), args, out returnType);
				}
				finally
				{
					ReturnArray(args);
				}
			}
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.Name == ";")
			{
				//if (this.Left == null) return null;
				//var left = this.Left.Build(buildContext, scriptContext, options);
				//if (this.Right == null) return left;
				//var right = this.Right.Build(buildContext, scriptContext, options);
				//buildContext.PrevExpressions.Add(left);
				//return right;
				var list = new List<Expression>();
				var stack = new Stack<ITreeNode>();
				stack.Push(this);
				while (stack.Count > 0)
				{
					var op = stack.Pop();
					if (op is OperatorNode opNode && opNode.Name == ";")
					{
						if (opNode.Right != null) stack.Push(opNode.Right);
						if (opNode.Left != null) stack.Push(opNode.Left);
					}
					else
					{
						list.Add(op.Build(buildContext, scriptContext, options));
					}
				}
				if (list.Count == 0) return null;
				if (list.Count == 1) return list[0];
				return Expression.Block(list);
			}
			var args = GetArgs(scriptContext);
			try
			{
				//// 如果参数都是常量，则直接计算结果
				//bool isAllObject = true;
				//for (int i = 0; i < args.Length; i++)
				//{
				//	if (!(args[i] is ObjectNode))
				//	{
				//		isAllObject = false;
				//		break;
				//	}
				//}
				//if (isAllObject)
				//{
				//	var v = scriptContext.EvalFunc(options, null, this.Name, IsPrefix(), args, out var returnType);
				//	return Expression.Constant(v, returnType);
				//}
				// 
				return scriptContext.BuildFunc(buildContext, options, null, this.Name, IsPrefix(), args);
			}
			finally
			{
				ReturnArray(args);
			}
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(_Left);
			PoolManage.Return(_Right);
			_Left = null;
			_Right = null;
		}

		private static ITreeNode[] CreateArray(int count)
		{
			if (count == 1)
			{
				if (_queue1.TryDequeue(out var arr))
				{
					return arr;
				}
			}
			else
			{
				if (_queue2.TryDequeue(out var arr))
				{
					return arr;
				}
			}
			return new ITreeNode[count];
		}

		private static void ReturnArray(ITreeNode[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = null;
			}
			if (arr.Length == 1)
			{
				if (_queue1.Count < 10)
				{
					_queue1.Enqueue(arr);
				}
			}
			else
			{
				if (_queue2.Count < 10)
				{
					_queue2.Enqueue(arr);
				}
			}
		}
	}
}
