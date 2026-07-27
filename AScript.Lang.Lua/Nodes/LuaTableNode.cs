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
		private static readonly MethodInfo Method_Dictionary_Add_object_object = typeof(Dictionary<object, object>).GetMethod("Add", new[] { typeof(object), typeof(object) });

		public IList<ITreeNode> Items { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var dictType = typeof(Dictionary<object, object>);

			// 如果 Items 为空或 null，直接返回空字典
			if (this.Items == null || this.Items.Count == 0)
			{
				return Expression.New(dictType);
			}

			// 创建字典实例变量
			var instanceVar = Expression.Variable(dictType, "table");

			// 构建初始化语句
			var statements = new List<Expression>();
			statements.Add(Expression.Assign(instanceVar, Expression.New(dictType)));

			long index = 1;
			foreach (var item in this.Items)
			{
				if (item is OperatorNode op && op.Name == "=")
				{
					// 键值对: key = value
					var key = ((VariableNode)op.Left).Name;
					var valueExpr = op.Right.Build(buildContext, scriptContext, options);
					if (valueExpr.Type.IsValueType)
					{
						valueExpr = Expression.Convert(valueExpr, typeof(object));
					}
					statements.Add(Expression.Call(instanceVar, Method_Dictionary_Add_object_object,
						Expression.Constant(key), valueExpr));
				}
				else
				{
					// 数组元素: [index] = value
					var valueExpr = item?.Build(buildContext, scriptContext, options) ?? ExpressionUtils.Constant_null;
					if (valueExpr.Type.IsValueType)
					{
						valueExpr = Expression.Convert(valueExpr, typeof(object));
					}
					statements.Add(Expression.Call(instanceVar, Method_Dictionary_Add_object_object,
						Expression.Convert(Expression.Constant(index++), typeof(object)), valueExpr));
				}
			}

			statements.Add(instanceVar);
			return Expression.Block(new[] { instanceVar }, statements);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var dict = new Dictionary<object, object>();
			returnType = dict.GetType();
			if (this.Items == null || this.Items.Count == 0)
			{
				return dict;
			}
			long index = 1L;
			foreach (var item in this.Items)
			{
				if (item is OperatorNode op && op.Name == "=")
				{
					var key = ((VariableNode)op.Left).Name;
					var value = op.Right.Eval(context, options, control, out _);
					dict[key] = value;
				}
				else
				{
					dict[index++] = item?.Eval(context, options, control, out _);
				}
			}
			return dict;
		}

		public override void Clear()
		{
			base.Clear();

			this.Items = null;
		}
	}
}
