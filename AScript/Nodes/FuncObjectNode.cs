using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class FuncObjectNode : TreeNode
	{
		public Func<object> Func { get; set; }
		public Type ObjectType { get; set; }

		public FuncObjectNode() { }
		public FuncObjectNode(Func<object> func)
		{
			this.Func = func;
		}
		public FuncObjectNode(Func<object> func, Type objectType) : this(func)
		{
			this.ObjectType = objectType;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Expression.Call(this.Func.Method);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var v = this.Func();
			returnType = this.ObjectType ?? v?.GetType() ?? typeof(object);
			return v;
		}
	}
}
