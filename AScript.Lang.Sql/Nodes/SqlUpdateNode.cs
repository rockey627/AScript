using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
				var condition = new SqlQueryNodeVisitor(buildContext, scriptContext, queryNode).Visit(this.Condition);
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
			var visitor = new ValueBuildTreeNodeVisitor(itemVar);
			for (int i = 0; i < this.Fields.Count; i++)
			{
				var value = visitor.Visit(this.Values[i]).Build(buildContext, scriptContext, options);
				var expr = visitor.SetValue(this.Fields[i], value);
				if (expr != null) updateStatements.Add(expr);
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
				var condition = new SqlQueryNodeVisitor(null, context, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList
			var list = (IList)context.EvalFunc(options, control, "ToList", new ITreeNode[] { sourceNode });
			if (list.Count == 0) return 0;

			var visitor = new ValueEvalTreeNodeVisitor(list[0]);
			for (int i = 0; i < this.Values.Count; i++)
			{
				visitor.Visit(this.Values[i]);
			}
			foreach (var item in list)
			{
				visitor.Item = item;
				for (int i = 0; i < this.Fields.Count; i++)
				{
					var value = this.Values[i].Eval(context, options, control, out var valueType);
					visitor.TrySetValue(this.Fields[i], value);
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

		private class ValueBuildTreeNodeVisitor : SqlTreeNodeVisitor
		{
			private readonly ParameterExpression _VarExpr;
			private readonly Dictionary<string, PropertyInfo> _PropertyDict = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

			public ValueBuildTreeNodeVisitor(ParameterExpression varExpr)
			{
				_VarExpr = varExpr;
			}

			public override ITreeNode VisitVariableNode(VariableNode variableNode)
			{
				if (variableNode.Parent is OperatorNode operatorNode
					&& (operatorNode.Name == "." || operatorNode.Name == "?."))
				{
					return base.VisitVariableNode(variableNode);
				}
				if (TryGetValue(variableNode.Name, out var v))
				{
					return new ExpressionNode(v);
				}
				return base.VisitVariableNode(variableNode);
			}

			public bool TryGetValue(string name, out Expression value)
			{
				if (typeof(DataRow).IsAssignableFrom(_VarExpr.Type))
				{
					value = Expression.Property(_VarExpr, ScriptUtils.Property_DataRow_Item_String, Expression.Constant(name));
					return true;
				}
				if (TryGetProperty(name, out var p))
				{
					value = Expression.Property(_VarExpr, p);
					return true;
				}
				value = null;
				return false;
			}

			public Expression SetValue(string name, Expression value)
			{
				if (typeof(DataRow).IsAssignableFrom(_VarExpr.Type))
				{
					var rowColumn = Expression.Property(_VarExpr, ScriptUtils.Property_DataRow_Item_String, Expression.Constant(name));
					if (value.Type != typeof(object))
					{
						value = Expression.Convert(value, typeof(object));
					}
					return Expression.Assign(rowColumn, value);
				}
				if (TryGetProperty(name, out var p))
				{
					var propExpr = Expression.Property(_VarExpr, p);
					return Expression.Assign(propExpr, value);
				}
				return null;
			}

			private bool TryGetProperty(string name, out PropertyInfo property)
			{
				if (_PropertyDict.TryGetValue(name, out property))
				{
					return property != null;
				}
				property = _VarExpr.Type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				_PropertyDict[name] = property;
				return property != null;
			}
		}

		private class ValueEvalTreeNodeVisitor : SqlTreeNodeVisitor
		{
			private readonly Dictionary<string, PropertyInfo> _PropertyDict = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

			public object Item { get; set; }

			public ValueEvalTreeNodeVisitor() { }
			public ValueEvalTreeNodeVisitor(object item)
			{
				this.Item = item;
			}

			public override ITreeNode VisitVariableNode(VariableNode variableNode)
			{
				if (variableNode.Parent is OperatorNode operatorNode
					&& (operatorNode.Name == "." || operatorNode.Name == "?."))
				{
					return base.VisitVariableNode(variableNode);
				}
				var f = TryGetValue(variableNode.Name);
				//if (TryGetValue(variableNode.Name, out var v))
				if (f != null)
				{
					return new FuncObjectNode(f);
				}
				return base.VisitVariableNode(variableNode);
			}

			public Func<object> TryGetValue(string name)
			{
				if (this.Item is DataRow dataRow)
				{
					if (dataRow.Table.Columns.Contains(name))
					{
						//value = dataRow[name];
						//return true;
						return () => (this.Item as DataRow)[name];
					}
					//value = null;
					//return false;
					return null;
				}
				if (TryGetProperty(name, out var p))
				{
					//value = p.GetValue(this.Item);
					//return true;
					return ()=> p.GetValue(this.Item);
				}
				//value = null;
				//return false;
				return null;
			}

			public bool TrySetValue(string name, object value)
			{
				if (this.Item is DataRow dataRow)
				{
					if (dataRow.Table.Columns.Contains(name))
					{
						dataRow[name] = value;
						return true;
					}
					return false;
				}
				if (TryGetProperty(name, out var p))
				{
					p.SetValue(this.Item, value);
					return true;
				}
				return false;
			}

			private bool TryGetProperty(string name, out PropertyInfo property)
			{
				if (_PropertyDict.TryGetValue(name, out property))
				{
					return property != null;
				}
				property = this.Item.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				_PropertyDict[name] = property;
				return property != null;
			}
		}
	}
}
