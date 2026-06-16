using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AScript.Nodes;

namespace AScript
{
	public class FunctionBuildArgs : EventArgs
	{
		private static readonly ConcurrentQueue<FunctionBuildArgs> _pool = new ConcurrentQueue<FunctionBuildArgs>();

		/// <summary>
		/// 
		/// </summary>
		public BuildContext BuildContext { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public ScriptContext ScriptContext { get; private set; }
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
		/// 
		/// </summary>
		public IList<Expression> ArgExprs { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public Expression Result { get; set; }

		//public FunctionBuildArgs(ExpressionBuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, IList<ITreeNode> args)
		//	: this(buildContext, scriptContext, options, control, name, false, args)
		//{
		//}
		public FunctionBuildArgs(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			this.BuildContext = buildContext;
			this.ScriptContext = scriptContext;
			this.Options = options;
			this.Control = control;
			this.Name = name;
			this.IsPrefix = isPrefix;
			this.Args = args;
		}
		public FunctionBuildArgs(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, IList<Expression> argExprs)
		{
			this.BuildContext = buildContext;
			this.ScriptContext = scriptContext;
			this.Options = options;
			this.Control = control;
			this.Name = name;
			this.IsPrefix = isPrefix;
			this.Args = args;
			this.ArgExprs = argExprs;
		}

		public int GetArgsCount()
		{
			if (this.ArgExprs != null) return this.ArgExprs.Count;
			if (this.Args != null) return this.Args.Count;
			return 0;
		}

		public Expression BuildArgs(int index)
		{
			if (this.ArgExprs != null) return index < this.ArgExprs.Count ? this.ArgExprs[index] : null;
			return index < this.Args.Count ? this.Args[index]?.Build(this.BuildContext, this.ScriptContext, this.Options) : null;
		}

		public IList<Expression> BuildArgs()
		{
			if (this.ArgExprs == null && this.Args != null && this.Args.Count > 0)
			{
				this.ArgExprs = new Expression[this.Args.Count];
				for (int i = 0; i < this.Args.Count; i++)
				{
					this.ArgExprs[i] = this.Args[i]?.Build(this.BuildContext, this.ScriptContext, this.Options);
				}
			}
			return this.ArgExprs;
		}

		public static FunctionBuildArgs Create(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			if (_pool.TryDequeue(out var e))
			{
				e.BuildContext = buildContext;
				e.ScriptContext = scriptContext;
				e.Options = options;
				e.Control = control;
				e.Name = name;
				e.IsPrefix = isPrefix;
				e.Args = args;
				e.ArgExprs = null;
				e.Result = null;
				return e;
			}
			return new FunctionBuildArgs(buildContext, scriptContext, options, control, name, isPrefix, args);
		}

		public static FunctionBuildArgs Create(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, IList<Expression> argExprs)
		{
			if (_pool.TryDequeue(out var e))
			{
				e.BuildContext = buildContext;
				e.ScriptContext = scriptContext;
				e.Options = options;
				e.Control = control;
				e.Name = name;
				e.IsPrefix = isPrefix;
				e.Args = args;
				e.ArgExprs = argExprs;
				e.Result = null;
				return e;
			}
			return new FunctionBuildArgs(buildContext, scriptContext, options, control, name, isPrefix, args, argExprs);
		}

		internal static void Return(FunctionBuildArgs e)
		{
			if (e == null) return;
			e.BuildContext = null;
			e.ScriptContext = null;
			e.Options = null;
			e.Control = null;
			e.Name = null;
			e.Args = null;
			e.ArgExprs = null;
			e.Result = null;
			if (_pool.Count < 10)
			{
				_pool.Enqueue(e);
			}
		}
	}
}
