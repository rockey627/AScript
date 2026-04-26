using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		/// 推导式
		/// </summary>
		public ForeachNode ForeachNode { get; set; }
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

			Expression[] itemExprs;
			Type elementType = null;
			var objType = typeof(object);

			if (this.Items != null)
			{
				// Build each item expression
				itemExprs = new Expression[Items.Count];
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
			else if (this.ForeachNode != null)
			{
				// 推导式：使用 Enumerable.Select 直接创建集合
				var tempBuildContext = new BuildContext(buildContext);
				var collectionExpr = this.ForeachNode.Collection.Build(tempBuildContext, scriptContext, options);

				// 获取集合元素类型
				var elementType2 = ScriptUtils.GetElementType(collectionExpr.Type);
				if (elementType2 == null)
				{
					elementType2 = typeof(object);
				}

				// 创建循环变量
				this.ForeachNode.VarDefine.SystemType = elementType2;
				var itemVar = this.ForeachNode.VarDefine.Build(tempBuildContext, scriptContext, options);

				// 构建 body 表达式
				var bodyExpr = this.ForeachNode.Body.Build(tempBuildContext, scriptContext, options);

				// 构建 lambda: x => body
				var lambda = Expression.Lambda(bodyExpr, (ParameterExpression)itemVar);

				// 获取 Enumerable.Select 泛型方法
				var selectMethod = ExpressionUtils.Method_Enumerable_Select1;
				selectMethod = selectMethod.MakeGenericMethod(elementType2, bodyExpr.Type);

				// 生成表达式: Enumerable.Select(collection, lambda)
				var selectExpr = Expression.Call(null, selectMethod, collectionExpr, lambda);

				// 根据 CollectionType 决定是 ToArray 还是 ToList 并直接返回
				if (CollectionType == typeof(Array))
				{
					var toArrayMethod = ExpressionUtils.Method_Enumerable_ToArray;
					toArrayMethod = toArrayMethod.MakeGenericMethod(bodyExpr.Type);
					return Expression.Call(null, toArrayMethod, selectExpr);
				}
				else
				{
					// List<T>
					var toListMethod = ExpressionUtils.Method_Enumerable_ToList;
					toListMethod = toListMethod.MakeGenericMethod(bodyExpr.Type);
					return Expression.Call(null, toListMethod, selectExpr);
				}
			}
			else
			{
				// 空集合
				if (CollectionType == typeof(Array))
				{
					return Expression.NewArrayInit(typeof(object));
				}
				else
				{
					var listType = typeof(List<object>);
					return Expression.New(listType);
				}
			}
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.CollectionType == null)
			{
				throw new Exception("unknown collection type");
			}

			object[] itemValues;
			Type elementType = null;
			var objType = typeof(object);

			if (this.Items != null)
			{
				// Evaluate all items to get their values
				itemValues = new object[Items.Count];
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
			}
			else if (this.ForeachNode != null)
			{
				// 推导式：遍历集合并收集 body 结果
				var listResult = this.ForeachNode.Collection.Eval(context, options, control, out var collectionType2);
				if (listResult == null)
				{
					throw new NullReferenceException("foreach collection is null");
				}
				if (!(listResult is IEnumerable en))
				{
					throw new Exception($"invalid foreach collection {collectionType2}");
				}

				var tempContext = ScriptContext.Create(context);
				var tempController = new EvalControl(control, true);
				var varType = this.ForeachNode.VarDefine.SystemType ?? typeof(object);
				this.ForeachNode.VarDefine.Eval(tempContext, options, out _);

				var results = new List<object>();
				foreach (var item in en)
				{
					tempContext.SetVar(this.ForeachNode.VarDefine.Name, item, item == null ? varType : null);
					var bodyResult = this.ForeachNode.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out var bodyType);
					results.Add(bodyResult);
				}

				itemValues = results.ToArray();
				if (itemValues.Length > 0 && itemValues[0] != null)
				{
					elementType = itemValues[0].GetType();
				}
			}
			else
			{
				itemValues = new object[0];
				elementType = typeof(object);
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
				returnType = arr.GetType();
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
				//list.Add(Convert.ChangeType(itemValues[i], elementType));
				list.Add(itemValues[i]);
			}
			returnType = listType;
			return list;
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Items);
			PoolManage.Return(this.ForeachNode);

			this.Items = null;
			this.ForeachNode = null;
			this.ElementType = null;
			this.CollectionType = null;
		}
	}
}
