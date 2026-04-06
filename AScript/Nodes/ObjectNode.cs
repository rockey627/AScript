using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class ObjectNode : TreeNode
	{
		public object Data { get; set; }
		public Type DataType { get; set; }

		public ObjectNode() { }
		public ObjectNode(object data) : this(data, null) { }
		public ObjectNode(object data, Type dataType)
		{
			this.Data = data;
			this.DataType = dataType ;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			returnType = this.DataType ?? this.Data?.GetType() ?? typeof(object);
			return this.Data;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return Expression.Constant(this.Data, this.DataType ?? this.Data?.GetType() ?? typeof(object));
		}

		public override void Clear()
		{
			base.Clear();

			this.Data = null;
			this.DataType = null;
		}
	}
}
