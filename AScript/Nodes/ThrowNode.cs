using AScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AScript.Nodes
{
	public class ThrowNode : TreeNode
	{
		private static readonly ConstructorInfo Construct_ScriptCustomException_object = typeof(ScriptCustomException).GetConstructor(new[] { typeof(object) });

		public ITreeNode ExceptionNode { get; set; }

		public ThrowNode() { }
		public ThrowNode (ITreeNode exceptionNode)
		{
			this.ExceptionNode = exceptionNode;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var d = this.ExceptionNode.Build(buildContext, scriptContext, options);
			if (typeof(Exception).IsAssignableFrom(d.Type))
			{
				return Expression.Throw(d);
			}
			var ex = Expression.New(Construct_ScriptCustomException_object, d);
			return Expression.Throw(ex);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var d = this.ExceptionNode.Eval(context, options, control, out _);
			if (d is Exception ex) throw ex;
			throw new Exceptions.ScriptCustomException(d);
		}

		public override void Clear()
		{
			base.Clear();

			if (this.ExceptionNode != null)
			{
				PoolManage.Return(this.ExceptionNode);
				this.ExceptionNode = null;
			}
		}
	}
}
