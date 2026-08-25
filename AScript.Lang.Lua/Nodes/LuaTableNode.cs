using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Lua.Nodes
{
	/// <summary>
	/// { 'h1', a=5, 'h2' } => 1:'h1', a:5, 2:'h2'
	/// </summary>
	public class LuaTableNode : TreeNode
	{
		private static readonly PropertyInfo LuaTable_Item_Property = typeof(LuaTable).GetProperty("Item");

		public IList<ITreeNode> Items { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tableType = typeof(LuaTable);

			// 如果 Items 为空或 null，直接返回空表
			if (this.Items == null || this.Items.Count == 0)
			{
				return Expression.New(tableType);
			}

			// 创建表实例变量
			var instanceVar = Expression.Variable(tableType, "table");

			// 构建初始化语句
			var statements = new List<Expression>();
			statements.Add(Expression.Assign(instanceVar, Expression.New(tableType)));

			long index = 1;
			foreach (var item in this.Items)
			{
				if (item is OperatorNode op && op.Name == "=")
				{
					if (op.Left is CollectionNode colNode)
					{
						// 键值对: [key] = value
						var keyExpr = colNode.Items[0].Build(buildContext, scriptContext, options);
						if (keyExpr.Type.IsValueType)
						{
							keyExpr = Expression.Convert(keyExpr, typeof(object));
						}
						var valueExpr = op.Right.Build(buildContext, scriptContext, options);
						if (valueExpr.Type.IsValueType)
						{
							valueExpr = Expression.Convert(valueExpr, typeof(object));
						}
						statements.Add(Expression.Call(instanceVar, LuaTable_Item_Property.SetMethod, keyExpr, valueExpr));
					}
					else
					{
						// 键值对: key = value
						var key = ((VariableNode)op.Left).Name;
						var keyExpr = Expression.Constant(key);
						var valueExpr = op.Right.Build(buildContext, scriptContext, options);
						if (valueExpr.Type.IsValueType)
						{
							valueExpr = Expression.Convert(valueExpr, typeof(object));
						}
						statements.Add(Expression.Call(instanceVar, LuaTable_Item_Property.SetMethod, keyExpr, valueExpr));
					}
				}
				else
				{
					// 数组元素: [index] = value
					var keyExpr = Expression.Convert(Expression.Constant(index++), typeof(object));
					var valueExpr = item?.Build(buildContext, scriptContext, options) ?? ScriptUtils.Constant_null;
					if (valueExpr.Type.IsValueType)
					{
						valueExpr = Expression.Convert(valueExpr, typeof(object));
					}
					statements.Add(Expression.Call(instanceVar, LuaTable_Item_Property.SetMethod, keyExpr, valueExpr));
				}
			}

			statements.Add(instanceVar);
			return Expression.Block(new[] { instanceVar }, statements);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var table = new LuaTable();
			returnType = table.GetType();
			if (this.Items == null || this.Items.Count == 0)
			{
				return table;
			}
			long index = 1L;
			foreach (var item in this.Items)
			{
				if (item is OperatorNode op && op.Name == "=")
				{
					if (op.Left is CollectionNode colNode)
					{
						var key = colNode.Items[0].Eval(context, options, control, out _);
						var value = op.Right.Eval(context, options, control, out _);
						table[key] = value;
					}
					else
					{
						// 键值对: key = value
						var key = ((VariableNode)op.Left).Name;
						var value = op.Right.Eval(context, options, control, out _);
						table[key] = value;
					}
				}
				else
				{
					table[index++] = item?.Eval(context, options, control, out _);
				}
			}
			return table;
		}

		public override void Clear()
		{
			base.Clear();

			this.Items = null;
		}
	}
}
