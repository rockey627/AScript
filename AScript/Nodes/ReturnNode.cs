using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class ReturnNode : TreeNode
	{
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (control == null)
			{
				throw new Exception("unsupport return");
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
				return Expression.Return(returnBuildContext.ReturnLabel);
			}
			if (returnBuildContext.ReturnVariableExpression == null)
			{
				returnBuildContext.ReturnVariableExpression = Expression.Variable(body.Type);
			}
			if (body.Type != returnBuildContext.ReturnVariableExpression.Type)
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
