using AScript.Nodes;
using AScript;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Lua.Nodes
{
	public class LuaDefineFuncNode : DefineFuncNode
	{
		private static readonly PropertyInfo LuaTable_Item_Property = typeof(LuaTable).GetProperty("Item");

		public string ClassName { get; set; }
		public ITreeNode ClassNode { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.ClassNode == null)
			{
				return base.Eval(context, options, control, out returnType);
			}
			var table = this.ClassNode.Eval(context, options, control, out _);
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
			if (this.ClassNode == null)
			{
				return base.Build(buildContext, scriptContext, options);
			}
			// 获取 table 的 Expression
			var luaTableExpr = this.ClassNode.Build(buildContext, scriptContext, options);
			// 保存 fieldName 并设置 this.Name = null
			string fieldName = this.Name;
			this.Name = null;
			try
			{
				// 调用 base.Build 获取 lambda 表达式
				var lambdaExpr = base.Build(buildContext, scriptContext, options);

				// 构建 table 字段赋值表达式: luaTable[fieldName] = lambdaExpr
				var keyExpr = Expression.Constant(fieldName);
				var valueExpr = lambdaExpr.Type.IsValueType ? Expression.Convert(lambdaExpr, typeof(object)) : lambdaExpr;
				var propertyExpr = Expression.Property(luaTableExpr, LuaTable_Item_Property, keyExpr);
				var assignExpr = Expression.Assign(propertyExpr, valueExpr);

				// 返回块表达式：先赋值到 table，再返回 lambdaExpr
				return Expression.Block(assignExpr, lambdaExpr);
			}
			finally
			{
				this.Name = fieldName;
			}
		}

		public override void Clear()
		{
			base.Clear();

			this.ClassNode = null;
		}
	}
}
