using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlInsertNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<string> Columns { get; set; }
		public IList<IList<ITreeNode>> Values { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var sourceExpr = this.Source.Build(buildContext, scriptContext, options);

			Type elementType = null;
			MethodInfo addRangeMethod = null;
			MethodInfo addMethod = null;

			if (sourceExpr.Type.IsGenericType)
			{
				var genericType = sourceExpr.Type.GetGenericTypeDefinition();
				if (genericType == typeof(IList<>) || genericType == typeof(List<>))
				{
					elementType = sourceExpr.Type.GenericTypeArguments[0];
				}
			}

			addRangeMethod = sourceExpr.Type.GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "AddRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (addRangeMethod != null)
			{
				if (elementType == null)
				{
					elementType = addRangeMethod.GetParameters()[0].ParameterType.GetGenericArguments()[0];
				}
			}
			else
			{
				addMethod = sourceExpr.Type.GetMethod("Add");
				if (addMethod == null) throw new Exceptions.ScriptAnalyzingException("insert target must have AddRange or Add method");
				if (elementType == null)
				{
					elementType = addMethod.GetParameters()[0].ParameterType;
				}
			}

			if (elementType == null)
			{
				throw new Exceptions.ScriptAnalyzingException("insert target must have AddRange or Add method");
			}

			// 创建数组存储新元素表达式
			var newItemExprs = new Expression[this.Values.Count];
			for (int i = 0; i < this.Values.Count; i++)
			{
				var rowValues = this.Values[i];
				var properties = new ITreeNode[rowValues.Count];
				for (int j = 0; j < rowValues.Count; j++)
				{
					//var valueExpr = rowValues[j].Build(buildContext, scriptContext, options);
					var assign = new OperatorNode("=", 0, 2)
					{
						Left = new VariableNode(this.Columns[j]),
						//Right = new ObjectNode(valueExpr, valueExpr.Type)
						Right = rowValues[j]
					};
					properties[j] = assign;
				}
				var newNode = new NewNode { SystemType = elementType, InitProperties = properties };
				newItemExprs[i] = newNode.Build(buildContext, scriptContext, options);
			}

			// 生成添加元素的表达式
			if (addRangeMethod != null)
			{
				// 创建数组变量
				var arrayCreate = Expression.NewArrayInit(elementType, newItemExprs);
				return Expression.Block(Expression.Call(sourceExpr, addRangeMethod, arrayCreate), Expression.Constant(this.Values.Count));
			}

			// 使用 Add 方法逐个添加
			var statements = new Expression[newItemExprs.Length + 1];
			for (int i = 0; i < newItemExprs.Length; i++)
			{
				var itemExpr = newItemExprs[i];
				statements[i] = Expression.Call(sourceExpr, addMethod, itemExpr);
			}
			statements[newItemExprs.Length] = Expression.Constant(this.Values.Count);
			return Expression.Block(statements);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var source = this.Source.Eval(context, options, control, out returnType);
			if (source == null) throw new Exceptions.ScriptRuntimeException("insert target is null");

			Type elementType = null;
			MethodInfo addRangeMethod = null;
			MethodInfo addMethod = null;

			if (source is IList list)
			{
				if (source.GetType().IsGenericType)
				{
					var genericType = source.GetType().GetGenericTypeDefinition();
					if (genericType == typeof(IList<>) || genericType == typeof(List<>))
					{
						elementType = source.GetType().GenericTypeArguments[0];
					}
				}
			}

			if (elementType == null)
			{
				addRangeMethod = source.GetType().GetMethods()
					.FirstOrDefault(m =>
					{
						if (m.Name != "AddRange") return false;
						var p0 = m.GetParameters()[0];
						if (!p0.ParameterType.IsGenericType) return false;
						return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
					});
				if (addRangeMethod != null)
				{
					elementType = addRangeMethod.GetParameters()[0].ParameterType.GetGenericArguments()[0];
				}
				else
				{
					addMethod = source.GetType().GetMethod("Add");
					if (addMethod == null) throw new Exceptions.ScriptRuntimeException("insert target must have AddRange or Add method");
					elementType = addMethod.GetParameters()[0].ParameterType;
				}
			}

			if (elementType == null)
			{
				throw new Exceptions.ScriptRuntimeException("insert target must have AddRange or Add method");
			}

			var newItems = Array.CreateInstance(elementType, this.Values.Count);
			for (int i = 0; i < this.Values.Count; i++)
			{
				var rowValues = this.Values[i];
				var properties = new ITreeNode[rowValues.Count];
				for (int j = 0; j < rowValues.Count; j++)
				{
					var assign = new OperatorNode("=", 0, 2)
					{
						Left = new VariableNode(this.Columns[j]),
						Right = rowValues[j]
					};
					properties[j] = assign;
				}
				var newNode = new NewNode { SystemType = elementType, InitProperties = properties };
				var newItem = newNode.Eval(context, options, control, out _);
				newItems.SetValue(newItem, i);
			}

			if (addRangeMethod != null)
			{
				addRangeMethod.Invoke(source, new object[] { newItems });
			}
			else if (source is IList list2)
			{
				foreach (var item in newItems)
				{
					list2.Add(item);
				}
			}
			else
			{
				foreach (var item in newItems)
				{
					addMethod.Invoke(source, new[] { item });
				}
			}
			returnType = typeof(int);
			return this.Values.Count;
		}
	}
}
