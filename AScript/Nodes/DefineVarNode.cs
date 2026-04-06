using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class DefineVarNode : VariableNode
	{
		public Type SystemType { get; set; }
		public string Type { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var definedType = this.SystemType ?? context.EvalType(this.Type);
			if (definedType == null)
			{
				throw new Exception("unknown type:" + this.Type);
			}
			context.SetTempVar(this.Name, null, definedType, false);
			returnType = definedType;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var type = this.SystemType ?? scriptContext.EvalType(this.Type);
			if (type == null)
			{
				throw new Exception("unknown type:" + this.Type);
			}
			var v = Expression.Variable(type, this.Name);
			buildContext.Variables[this.Name] = v;
			buildContext.LocalVariables.Add(this.Name);
			return v;
		}

		public override void Clear()
		{
			base.Clear();

			this.Type = null;
			this.SystemType = null;
		}
	}
}
