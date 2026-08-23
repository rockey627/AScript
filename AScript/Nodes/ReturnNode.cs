using AScript.Exceptions;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
    public class ReturnNode : TreeNode
	{
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (control == null)
			{
				throw new ScriptAnalyzingException("unsupport return");
			}
			try
			{
				if (this.Body == null)
				{
					returnType = null;
					return null;
				}
				return this.Body.Eval(context, options, control, out returnType);
			}
			finally
			{
				control.Terminal = true;
			}
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (control == null)
			{
				throw new ScriptAnalyzingException("unsupport return");
			}
			try
			{
				if (this.Body == null)
				{
					return default;
				}
				return await this.Body.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				control.Terminal = true;
			}
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var body = this.Body?.Build(buildContext, scriptContext, options);
			var returnBuildContext = buildContext.GetReturnBuildContext();
			if (returnBuildContext.ReturnLabel == null)
			{
				//returnBuildContext.ReturnLabel = body == null ? Expression.Label() : Expression.Label(body.Type);
				returnBuildContext.ReturnLabel = Expression.Label();
			}
			if (body == null)
			{
				if (options.UseCompletionResult ?? false)
				{
					body = Expression.New(ScriptUtils.Constructor_CompletionResult_1, Expression.Constant(ECompletionType.Return));
				}
				else
				{
					return Expression.Return(returnBuildContext.ReturnLabel);
				}
			}
			if (body.Type != typeof(CompletionResult) && (options.UseCompletionResult ?? false))
			{
				if (body.Type.IsValueType) body = Expression.Convert(body, typeof(object));
				var result = Expression.New(ScriptUtils.Constructor_CompletionResult_2, Expression.Constant(ECompletionType.Return), body);
				body = result;
			}
			if (returnBuildContext.ReturnVariableExpression == null)
			{
				returnBuildContext.ReturnVariableExpression = Expression.Variable(body.Type);
			}
			else if (body.Type != returnBuildContext.ReturnVariableExpression.Type)
			{
				body = Expression.Convert(body, returnBuildContext.ReturnVariableExpression.Type);
			}
			return Expression.Block(
				Expression.Assign(returnBuildContext.ReturnVariableExpression, body),
				Expression.Return(returnBuildContext.ReturnLabel));
			//Expression.Return(returnBuildContext.ReturnLabel, returnBuildContext.ReturnVariableExpression));
		}
	}
}
