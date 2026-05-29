using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlUpdateNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<string> Fields { get; set; }
		public IList<ITreeNode> Values { get; set; }
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
				var condition = new SqlTreeNodeVisitor(null, scriptContext, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList()
			var list = scriptContext.BuildFunc(buildContext, options, null, "ToList", false, new ITreeNode[] { sourceNode });
			var listVar = Expression.Variable(list.Type);
			var assignList = Expression.Assign(listVar, list);

			// foreach 循环
			var enumeratorVar = Expression.Variable(typeof(IEnumerator<>).MakeGenericType(itemType), "enumerator");
			var getEnumerator = Expression.Call(listVar, typeof(IEnumerable<>).MakeGenericType(itemType).GetMethod("GetEnumerator"));
			var moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
			var currentProperty = typeof(IEnumerator<>).MakeGenericType(itemType).GetProperty("Current");
			var itemVar = Expression.Variable(itemType, "item");

			var breakLabel = Expression.Label("break");
			var continueLabel = Expression.Label("continue");

			// 循环体：更新字段
			var updateStatements = new List<Expression>();
			var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var visitor = new ValueBuildTreeNodeVisitor(itemVar);
			for (int i = 0; i < this.Fields.Count; i++)
			{
				var prop = Expression.PropertyOrField(itemVar, this.Fields[i]);
				var valueNode = visitor.Visit(this.Values[i]);
				var value = valueNode.Build(buildContext, scriptContext, options);
				if (value.Type != prop.Type)
				{
					value = ExpressionUtils.Convert(value, prop.Type);
				}
				updateStatements.Add(Expression.Assign(prop, value));
			}

			var loopBody = Expression.Block(new[] { itemVar },
				Expression.IfThenElse(
					Expression.Call(enumeratorVar, moveNextMethod),
					Expression.Block(
						Expression.Assign(itemVar, Expression.Property(enumeratorVar, currentProperty)),
						updateStatements.Count > 0 ? Expression.Block(updateStatements) : (Expression)Expression.Empty()
					),
					Expression.Break(breakLabel)
				));
			var loop = Expression.Loop(loopBody, breakLabel, continueLabel);
			var foreachBlock = Expression.Block(new[] { enumeratorVar },
				Expression.Assign(enumeratorVar, getEnumerator),
				loop);

			// 更新数据源
			var updateRangeMethod = source.GetType().GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "UpdateRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (updateRangeMethod != null)
			{
				return Expression.Block(new[] { listVar },
					assignList,
					foreachBlock,
					Expression.Call(source, updateRangeMethod, listVar),
					Expression.Property(listVar, "Count"));
			}

			return Expression.Block(new[] { listVar },
				assignList,
				foreachBlock,
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
				var condition = new SqlTreeNodeVisitor(null, context, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList
			var list = (IList)context.EvalFunc(options, control, "ToList", new ITreeNode[] { sourceNode });
			// 
			foreach (var item in list)
			{
				var properties = item.GetType().GetProperties();
				var dict = properties.Where(p => p.CanWrite).ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
				var visitor = new ValueEvalTreeNodeVisitor(item);
				for (int i = 0; i < this.Fields.Count; i++)
				{
					string field = this.Fields[i];
					if (dict.TryGetValue(field, out var p))
					{
						var value = visitor.Visit(this.Values[i]).Eval(context, options, control, out var valueType);
						p.SetValue(item, value);
					}
				}
			}

			// 更新数据源
			var updateRangeMethod = source.GetType().GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "UpdateRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (updateRangeMethod != null)
			{
				updateRangeMethod.Invoke(source, new object[] { list });
			}

			return list.Count;
		}

		private Type GetItemType(Type type)
		{
			if (type.IsGenericType)
			{
				return type.GenericTypeArguments[0];
			}
			return null;
		}

		private class ValueBuildTreeNodeVisitor : TreeNodeVisitor
		{
			private readonly ParameterExpression _VarExpr;
			private readonly Dictionary<string, PropertyInfo> _PropertyDict;

			public ValueBuildTreeNodeVisitor(ParameterExpression varExpr)
			{
				_VarExpr = varExpr;
				_PropertyDict = varExpr.Type.GetProperties().Where(a => a.CanRead).ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
			}

			public override ITreeNode VisitVariableNode(VariableNode variableNode)
			{
				if (variableNode.Parent is OperatorNode operatorNode
					&& (operatorNode.Name == "." || operatorNode.Name == "?."))
				{
					return base.VisitVariableNode(variableNode);
				}
				if (_PropertyDict.TryGetValue(variableNode.Name, out var p))
				{
					return new ExpressionNode(Expression.Property(_VarExpr, p));
				}
				return base.VisitVariableNode(variableNode);
			}
		}

		private class ValueEvalTreeNodeVisitor : TreeNodeVisitor
		{
			private readonly object _Item;
			private readonly Dictionary<string, PropertyInfo> _PropertyDict;

			public ValueEvalTreeNodeVisitor(object item)
			{
				_Item = item;
				_PropertyDict = item.GetType().GetProperties().Where(a => a.CanRead).ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
			}

			public override ITreeNode VisitVariableNode(VariableNode variableNode)
			{
				if (variableNode.Parent is OperatorNode operatorNode
					&& (operatorNode.Name == "." || operatorNode.Name == "?."))
				{
					return base.VisitVariableNode(variableNode);
				}
				if (_PropertyDict.TryGetValue(variableNode.Name, out var p))
				{
					return new ObjectNode(p.GetValue(_Item));
				}
				return base.VisitVariableNode(variableNode);
			}
		}
	}
}
