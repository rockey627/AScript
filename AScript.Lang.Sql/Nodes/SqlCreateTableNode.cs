using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlCreateTableNode : TreeNode
	{
		public string Name { get; set; }
		public bool CheckNotExists { get; set; }
		public IList<DataColumn> Columns { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (buildContext.TryGetVariableOrParameter(this.Name, out var v))
			{
				if (this.CheckNotExists) return v;
				throw new Exceptions.ScriptRuntimeException($"table[{this.Name}] is exists");
			}
			var t = scriptContext.EvalVar(this.Name, out var type);
			if (t != null)
			{
				if (this.CheckNotExists) return Expression.Constant(t, type);
				throw new Exceptions.ScriptRuntimeException($"table[{this.Name}] is exists");
			}
			v = Expression.Variable(typeof(SqlTable), this.Name);
			buildContext.Variables[this.Name] = v;
			buildContext.LocalVariables.Add(this.Name);
			var table = new SqlTable(this.Name);
			for (int i = 0; i < this.Columns.Count; i++)
			{
				table.Columns.Add(this.Columns[i]);
			}
			return Expression.Assign(v, Expression.Constant(table));
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var t = context.EvalVar(this.Name, out returnType);
			if (t != null)
			{
				if (this.CheckNotExists) return t;
				throw new Exceptions.ScriptRuntimeException($"table[{this.Name}] is exists");
			}
			var table = new SqlTable(this.Name);
			for (int i = 0; i < this.Columns.Count; i++)
			{
				table.Columns.Add(this.Columns[i]);
			}
			context.SetTempVar(this.Name, table, false);
			returnType = typeof(SqlTable);
			return table;
		}
	}
}
