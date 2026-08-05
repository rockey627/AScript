using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

		public object[] ArgValues { get; private set; }

		public Type[] ArgTypes { get; private set; }
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
			this.ResultType = result?.GetType() ?? resultType ?? typeof(object);
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
				e.ArgValues = null;
				e.ArgTypes = null;
				e.IsHandled = false;
				return e;
			}
			return new FunctionEvalArgs(context, options, control, name, isPrefix, args);
		}

		public object EvalArgs(int i, out Type type)
		{
			if (this.ArgValues != null)
			{
				type = this.ArgTypes[i];
				return this.ArgValues[i];
			}
			return this.Args[i].Eval(this.Context, this.Options, this.Control, out type);
		}

		public void EvalArgs(bool evalDefineFuncNode = true)
		{
			if (this.ArgValues != null) return;
			if (this.Args == null || this.Args.Count == 0) return;
			this.ArgValues = new object[this.Args.Count];
			this.ArgTypes = new Type[this.Args.Count];
			for (int i = 0; i < this.Args.Count; i++)
			{
				var arg = this.Args[i];
				if (!evalDefineFuncNode && arg is DefineFuncNode)
				{
					this.ArgValues[i] = arg;
					this.ArgTypes[i] = typeof(Delegate);
				}
				else
				{
					var value = arg.Eval(this.Context, this.Options, this.Control, out var type);
					var valueType = value?.GetType() ?? type;
					this.ArgValues[i] = value;
					this.ArgTypes[i] = value is CustomFunctionObject ? typeof(Delegate) : valueType;
					if (!(arg is ObjectNode))
					{
						this.Args[i] = PoolManage.CreateObjectNode(value, valueType);
					}
				}
			}
		}

		public async Task EvalArgsAsync(bool evalDefineFuncNode = true, CancellationToken cancellationToken = default)
		{
			if (this.ArgValues != null) return;
			if (this.Args == null || this.Args.Count == 0) return;
			this.ArgValues = new object[this.Args.Count];
			this.ArgTypes = new Type[this.Args.Count];
			for (int i = 0; i < this.Args.Count; i++)
			{
				var arg = this.Args[i];
				if (!evalDefineFuncNode && (arg is DefineFuncNode || arg is CustomFunctionObject))
				{
					this.ArgValues[i] = arg;
					this.ArgTypes[i] = typeof(Delegate);
				}
				else
				{
					var result = await arg.EvalAsync(this.Context, this.Options, this.Control, cancellationToken).ConfigureAwait(false);
					var valueType = result.Value?.GetType() ?? result.Type;
					this.ArgValues[i] = result.Value;
					this.ArgTypes[i] = result.Value is CustomFunctionObject ? typeof(Delegate) : valueType;
					if (!(arg is ObjectNode))
					{
						this.Args[i] = PoolManage.CreateObjectNode(result.Value, valueType);
					}
				}
			}
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
			e.ArgValues = null;
			e.ArgTypes = null;
			e.IsHandled = false;
			if (_pool.Count < 10)
			{
				_pool.Enqueue(e);
			}
		}
	}
}
