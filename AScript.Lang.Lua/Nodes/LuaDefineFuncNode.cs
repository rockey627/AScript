using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.Lua.Nodes
{
	public class LuaDefineFuncNode : DefineFuncNode
	{
		public string ClassName { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (string.IsNullOrEmpty(ClassName))
			{
				return base.Eval(context, options, control, out returnType);
			}
			var table = context.EvalVar(this.ClassName);
			if (!(table is LuaTable luaTable))
			{
				throw new Exceptions.ScriptRuntimeException($"invalid expression 'function {this.ClassName}:{this.Name}', {this.ClassName} is not a table");
			}
			string fieldName = this.Name;
			this.Name = null;
			try
			{
				var del = base.Eval(context, options, control, out _);
				luaTable[fieldName] = del;
				returnType = del.GetType();
				return del;
			}
			finally
			{
				this.Name = fieldName;
			}
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (string.IsNullOrEmpty(this.ClassName))
			{
				return base.Build(buildContext, scriptContext, options);
			}
		}

		public override void Clear()
		{
			base.Clear();

			this.ClassName = null;
		}
	}
}
