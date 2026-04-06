using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;
using AScript.Nodes;

namespace AScript
{
	public class FunctionEvalArgs : EventArgs
	{
		private static readonly ConcurrentQueue<FunctionEvalArgs> _pool = new ConcurrentQueue<FunctionEvalArgs>();

		/// <summary>
		/// 当前执行上下文
		/// </summary>
		public ScriptContext Context { get; private set; }
		/// <summary>
		/// 编译选项
		/// </summary>
		public BuildOptions Options { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public EvalControl Control { get; private set; }
		/// <summary>
		/// 函数名
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// 是否前置运算符
		/// </summary>
		public bool IsPrefix { get; private set; }
		/// <summary>
		/// 参数列表
		/// </summary>
		public IList<ITreeNode> Args { get; private set; }
		/// <summary>
		/// 是否已执行
		/// </summary>
		public bool IsHandled { get; private set; }
		/// <summary>
		/// 执行结果
		/// </summary>
		public object Result { get; private set; }
		/// <summary>
		/// 结果类型
		/// </summary>
		public Type ResultType { get; private set; }

		//public FunctionEvalArgs(ScriptContext context, BuildOptions options, EvalControl control, string name, IList<ITreeNode> args)
		//	: this(context, options, control, name, false, args)
		//{
		//}
		public FunctionEvalArgs(ScriptContext context, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			this.Context = context;
			this.Options = options;
			this.Control = control;
			this.Name = name;
			this.IsPrefix = isPrefix;
			this.Args = args;
		}

		public void SetResult(object result, Type resultType)
		{
			this.Result = result;
			this.ResultType = resultType ?? result?.GetType() ?? typeof(object);
			this.IsHandled = true;
		}

		public void SetResult(object result)
		{
			SetResult(result, null);
		}

		public void SetResult<T>(T result)
		{
			SetResult(result, typeof(T));
		}

		public static FunctionEvalArgs Create(ScriptContext context, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			if (_pool.TryDequeue(out var e))
			{
				e.Context = context;
				e.Options = options;
				e.Control = control;
				e.Name = name;
				e.IsPrefix = isPrefix;
				e.Result = null;
				e.ResultType = null;
				e.Args = args;
				e.IsHandled = false;
				return e;
			}
			return new FunctionEvalArgs(context, options, control, name, isPrefix, args);
		}

		internal static void Return(FunctionEvalArgs e)
		{
			e.Context = null;
			e.Options = null;
			e.Control = null;
			e.Name = null;
			e.Result = null;
			e.ResultType = null;
			e.Args = null;
			e.IsHandled = false;
			if (_pool.Count < 10)
			{
				_pool.Enqueue(e);
			}
		}
	}
}
