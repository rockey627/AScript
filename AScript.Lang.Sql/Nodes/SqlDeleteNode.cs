using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlDeleteNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public ITreeNode Condition { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var source = this.Source.Build(buildContext, scriptContext, options);
			var itemType = GetItemType(source.Type);
			if (itemType == null) throw new Exceptions.ScriptAnalyzingException("unkown item type of source");

			ITreeNode sourceNode = new ExpressionNode(source);
			if (this.Condition != null)
			{
				var queryNode = new QueryNode();
				queryNode.AddFrom("__query__", sourceNode);
				var condition = new SqlQueryNodeVisitor(buildContext, scriptContext, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList()
			var list = scriptContext.BuildFunc(buildContext, options, null, "ToList", false, new ITreeNode[] { sourceNode });
			var listVar = Expression.Variable(list.Type);
			var assignList = Expression.Assign(listVar, list);

			var removeRangeMethod = source.Type.GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "RemoveRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (removeRangeMethod != null)
			{
				return Expression.Block(new[] { listVar },
					assignList,
					Expression.Call(source, removeRangeMethod, listVar),
					Expression.Property(listVar, "Count"));
			}

			// foreach 循环
			var enumeratorVar = Expression.Variable(typeof(IEnumerator<>).MakeGenericType(itemType), "enumerator");
			var getEnumerator = Expression.Call(listVar, typeof(IEnumerable<>).MakeGenericType(itemType).GetMethod("GetEnumerator"));
			var moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
			var currentProperty = typeof(IEnumerator<>).MakeGenericType(itemType).GetProperty("Current");
			var itemVar = Expression.Variable(itemType, "item");

			var breakLabel = Expression.Label("break");
			var continueLabel = Expression.Label("continue");

			// 循环体：删除元素
			var deleteStatements = new List<Expression>();

			// 使用循环调用 Remove 删除
			var removeMethod = source.Type.GetMethod("Remove");
			var loopBody2 = Expression.Block(new[] { itemVar },
				Expression.IfThenElse(
					Expression.Call(enumeratorVar, moveNextMethod),
					Expression.Block(
						Expression.Assign(itemVar, Expression.Property(enumeratorVar, currentProperty)),
						Expression.Call(source, removeMethod, itemVar)
					),
					Expression.Break(breakLabel)
				));
			var loop2 = Expression.Loop(loopBody2, breakLabel, continueLabel);
			var foreachBlock2 = Expression.Block(new[] { enumeratorVar },
				Expression.Assign(enumeratorVar, getEnumerator),
				loop2);

			return Expression.Block(new[] { listVar },
				assignList,
				foreachBlock2,
				Expression.Property(listVar, "Count"));
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var source = this.Source.Eval(context, options, control, out _);
			returnType = typeof(int);
			if (source == null) return 0;

			ITreeNode sourceNode = new ObjectNode(source);
			if (this.Condition != null)
			{
				var queryNode = new QueryNode();
				queryNode.AddFrom("__query__", sourceNode);
				var condition = new SqlQueryNodeVisitor(null, context, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList
			var list = (IList)context.EvalFunc(options, control, "ToList", new ITreeNode[] { sourceNode });
			if (list.Count == 0) return 0;

			// 更新数据源
			var removeRangeMethod = source.GetType().GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "RemoveRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (removeRangeMethod != null)
			{
				removeRangeMethod.Invoke(source, new object[] { list });
				return list.Count;
			}

			if (source is IList sourceList)
			{
				foreach (var item in list)
				{
					sourceList.Remove(item);
				}
			}
			else
			{
				var removeMethod = source.GetType().GetMethod("Remove");
				foreach (var item in list)
				{
					removeMethod.Invoke(source, new[] { item });
				}
			}

			return list.Count;
		}

		private Type GetItemType(Type type)
		{
			if (typeof(SqlTable).IsAssignableFrom(type))
			{
				return typeof(DataRow);
			}
			if (type.IsGenericType)
			{
				return type.GenericTypeArguments[0];
			}
			return null;
		}

	}
}
