using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class CollectionNode : TreeNode
	{
		/// <summary>
		/// 集合中的值列表
		/// </summary>
		public IList<ITreeNode> Items { get; set; }
		/// <summary>
		/// 集合中的元素类型，如果为null则取Items中的值类型
		/// </summary>
		public Type ElementType { get; set; }
		/// <summary>
		/// <![CDATA[如果是数组则为typeof(Array)，如果是列表则为typeof(List<>)]]>
		/// </summary>
		public Type CollectionType { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.CollectionType == null)
			{
				throw new Exception("unknown collection type");
			}

			// Build each item expression
			var itemExprs = new Expression[Items.Count];
			Type elementType = null;
			var objType = typeof(object);
			for (int i = 0; i < Items.Count; i++)
			{
				var itemExpr = Items[i].Build(buildContext, scriptContext, options);
				itemExprs[i] = itemExpr;
				if (this.ElementType == null)
				{
					if (elementType == null)
					{
						elementType = itemExpr.Type;
					}
					else if (elementType != objType && itemExpr.Type != elementType)
					{
						elementType = objType;
					}
				}
			}

			// Determine element type
			if (this.ElementType != null)
			{
				elementType = this.ElementType;
			}
			else if (elementType == null)
			{
				elementType = objType;
			}

			if (CollectionType == typeof(Array))
			{
				// Create array expression: new[] { elem1, elem2, ... }
				var convertedExprs = new Expression[itemExprs.Length];
				for (int i = 0; i < itemExprs.Length; i++)
				{
					if (itemExprs[i].Type != elementType)
					{
						convertedExprs[i] = Expression.Convert(itemExprs[i], elementType);
					}
					else
					{
						convertedExprs[i] = itemExprs[i];
					}
				}
				return Expression.NewArrayInit(elementType, convertedExprs);
			}

			// Handle List<T>
			Type listType;
			if (CollectionType.IsGenericType && CollectionType.GetGenericTypeDefinition() == typeof(List<>))
			{
				listType = CollectionType;
				elementType = CollectionType.GetGenericArguments()[0];
			}
			else
			{
				listType = typeof(List<>).MakeGenericType(elementType);
			}

			// Get Add method
			var addMethod = listType.GetMethod("Add");
			if (addMethod == null)
			{
				throw new Exception($"type {listType.Name} does not have an Add method");
			}

			// Create instance variable
			var instanceVar = Expression.Variable(listType, "instance");

			// Build statements: instance = new List<T>(); instance.Add(elem1); instance.Add(elem2); ...; return instance;
			var statements = new List<Expression>(2 + itemExprs.Length);
			var constructorInfo = listType.GetConstructor(new[] { typeof(int) });
			statements.Add(Expression.Assign(instanceVar, Expression.New(constructorInfo, new[] { Expression.Constant(itemExprs.Length) })));

			for (int i = 0; i < itemExprs.Length; i++)
			{
				Expression itemExpr = itemExprs[i];
				if (itemExpr.Type != elementType)
				{
					itemExpr = Expression.Convert(itemExpr, elementType);
				}
				statements.Add(Expression.Call(instanceVar, addMethod, itemExpr));
			}

			statements.Add(instanceVar);

			return Expression.Block(new[] { instanceVar }, statements);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.CollectionType == null)
			{
				throw new Exception("unknown collection type");
			}

			// Evaluate all items to get their values
			var itemValues = new object[Items.Count];
			Type elementType = null;
			var objType = typeof(object);
			for (int i = 0; i < Items.Count; i++)
			{
				itemValues[i] = Items[i].Eval(context, options, control, out var itemType);
				if (this.ElementType == null)
				{
					if (elementType == null)
					{
						elementType = itemType;
					}
					else if (elementType != objType && itemType != elementType)
					{
						elementType = objType;
					}
				}
			}

			// Determine element type
			if (this.ElementType != null)
			{
				elementType = this.ElementType;
			}
			else if (elementType == null)
			{
				elementType = objType;
			}

			if (CollectionType == typeof(Array))
			{
				// Create array
				var arr = Array.CreateInstance(elementType, itemValues.Length);
				for (int i = 0; i < itemValues.Length; i++)
				{
					arr.SetValue(Convert.ChangeType(itemValues[i], elementType), i);
				}
				returnType = typeof(Array);
				return arr;
			}

			// Handle List<T> - determine the actual list type to create
			Type listType;
			if (CollectionType.IsGenericType && CollectionType.GetGenericTypeDefinition() == typeof(List<>))
			{
				listType = CollectionType;
				elementType = CollectionType.GetGenericArguments()[0];
			}
			else
			{
				listType = typeof(List<>).MakeGenericType(elementType);
			}

			var list = (IList)Activator.CreateInstance(listType, itemValues.Length);
			for (int i = 0; i < itemValues.Length; i++)
			{
				list.Add(Convert.ChangeType(itemValues[i], elementType));
			}
			returnType = listType;
			return list;
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Items);

			this.Items = null;
			this.ElementType = null;
			this.CollectionType = null;
		}
	}
}
