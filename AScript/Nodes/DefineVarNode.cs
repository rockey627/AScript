using AScript.Exceptions;
using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class DefineVarNode : VariableNode
	{
		public Type SystemType { get; set; }
		public string Type { get; set; }
		public int Modifier { get; set; }

		public DefineVarNode() { }
		public DefineVarNode(string name) : base(name) { }
		public DefineVarNode(string name, string type, Type systemType) : base(name)
		{
			this.Type = type;
			this.SystemType = systemType;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var definedType = this.SystemType ?? context.EvalType(this.Type);
			if (definedType == null)
			{
				throw new ScriptAnalyzingException("unknown type:" + this.Type);
			}
			if (Modifiers.IsReadOnly(this.Modifier))
			{
				context.SetTempConst(this.Name, null, definedType, false);
			}
			else
			{
				context.SetTempVar(this.Name, null, definedType, false);
			}
			returnType = definedType;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var type = this.SystemType ?? scriptContext.EvalType(this.Type);
			if (type == null)
			{
				throw new ScriptAnalyzingException("unknown type:" + this.Type);
			}
			if (buildContext.Variables.TryGetValue(this.Name, out var existsVar))
			{
				if (existsVar.Type != typeof(object) && type != typeof(object) && existsVar.Type != type)
				{
					throw new ScriptRuntimeException($"variable '{this.Name}' is exists");
				}
				return existsVar;
			}
			var v = Expression.Variable(type, this.Name);
			buildContext.Variables[this.Name] = v;
			buildContext.LocalVariables.Add(this.Name);
			if (this.Modifier != 0)
			{
				buildContext.VariableModifiers[this.Name] = this.Modifier;
			}
			return v;
		}

		public override void Clear()
		{
			base.Clear();

			this.Type = null;
			this.SystemType = null;
			this.Modifier = 0;
		}
	}
}
