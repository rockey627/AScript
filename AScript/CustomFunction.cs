using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	public class CustomFunction
	{
		public string[] ArgNames { get; private set; }
		public Type[] ArgTypes { get; set; }
		public Type ReturnType { get; set; }
		public ITreeNode Body { get; private set; }

		public CustomFunction(Type returnType, string[] argNames, Type[] argTypes, ITreeNode body)
		{
			this.ReturnType = returnType;
			this.ArgNames = argNames;
			this.ArgTypes = argTypes;
			this.Body = body;
		}

		public object Eval(ScriptContext context, BuildOptions options, EvalControl control, IList<ITreeNode> args, out Type returnType)
		{
			//var tempContext = ScriptContext.Create(context);
			var tempContext = context;
			// 填充参数
			if (this.ArgNames != null)
			{
				for (int i = 0; i < this.ArgNames.Length; i++)
				{
					tempContext.SetVar(this.ArgNames[i], args[i].Eval(context, options, control, out var type), type);
				}
			}
			if (this.Body == null)
			{
				returnType = null;
				return null;
			}
			return this.Body.Eval(tempContext, options, new EvalControl(), out returnType);
		}

		public void Eval(FunctionEvalArgs e)
		{
			e.EvalArgs();
			var tempContext = e.Context;
			// 填充参数
			if (this.ArgNames != null)
			{
				for (int i = 0; i < this.ArgNames.Length; i++)
				{
					tempContext.SetVar(this.ArgNames[i], e.ArgValues[i], e.ArgTypes[i]);
				}
			}
			if (this.Body == null)
			{
				e.SetResult(null, null);
				return;
			}
			var value = this.Body.Eval(tempContext, e.Options, new EvalControl(), out var returnType);
			e.SetResult(value, returnType);
		}

		public async Task EvalAsync(FunctionEvalArgs e, CancellationToken cancellationToken = default)
		{
			await e.EvalArgsAsync(cancellationToken: cancellationToken);
			var tempContext = e.Context;
			// 填充参数
			if (this.ArgNames != null)
			{
				for (int i = 0; i < this.ArgNames.Length; i++)
				{
					tempContext.SetVar(this.ArgNames[i], e.ArgValues[i], e.ArgTypes[i]);
				}
			}
			if (this.Body == null)
			{
				e.SetResult(null, null);
				return;
			}
			var result = await this.Body.Eval2Async(tempContext, e.Options, new EvalControl(), cancellationToken).ConfigureAwait(false);
			e.SetResult(result.Value, result.Type);
		}

		public Delegate Compile(Type delegateType, ScriptContext context, BuildOptions options)
		{
			return Script.Lambda(delegateType, context, options, this.Body, this.ArgTypes, this.ArgNames, this.ReturnType).Compile();
		}

		public Delegate Compile(ScriptContext context, BuildOptions options)
		{
			return Compile(null, context, options);
		}

		public Delegate Compile(ScriptContext context)
		{
			return Compile(null, context, null);
		}
	}
}
