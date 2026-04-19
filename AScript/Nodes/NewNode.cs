using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using AScript;
using AScript.Operators;

namespace AScript.Nodes
{
	public class NewNode : TreeNode
	{
		public string Name { get; set; }
		/// <summary>
		/// 泛型类型列表
		/// </summary>
		public IList<string> GenericTypes { get; set; }
		/// <summary>
		/// 参数列表
		/// </summary>
		public IList<ITreeNode> Args { get; set; }
		/// <summary>
		/// 属性初始化列表
		/// </summary>
		public IList<ITreeNode> InitProperties { get; set; }
		/// <summary>
		/// 数组维度，0表示非数组，1表示一维数组，2表示二维数组等
		/// </summary>
		public int ArrayDimension { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			string name = this.Name;
			if (this.GenericTypes != null && this.GenericTypes.Count > 0)
			{
				if (this.GenericTypes.Count == 1)
				{
					name = $"{this.Name}<>";
				}
				else
				{
					name = this.Name + "<" + new string(',', this.GenericTypes.Count - 1) + ">";
				}
			}
			var type = scriptContext.EvalType(name);
			if (type == null)
			{
				throw new Exception($"unknow type {this.Name}");
			}
			Expression[] argValues;
			Type[] argTypes;
			if (this.Args == null)
			{
				argValues = null;
#if NETSTANDARD
				argTypes = Array.Empty<Type>();
#else
				argTypes = new Type[0];
#endif
			}
			else
			{
				argValues = new Expression[this.Args.Count];
				argTypes = new Type[this.Args.Count];
				for (int i = 0; i < this.Args.Count; i++)
				{
					var argValue = this.Args[i].Build(buildContext, scriptContext, options);
					argValues[i] = argValue;
					argTypes[i] = argValue.Type;
				}
			}
			if (this.GenericTypes != null && this.GenericTypes.Count > 0)
			{
				var genericTypes = new Type[this.GenericTypes.Count];
				for (int i = 0; i < this.GenericTypes.Count; i++)
				{
					var typeName = this.GenericTypes[i];
					var type0 = scriptContext.EvalType(typeName);
					if (type0 == null)
					{
						throw new Exception($"unknown type '{typeName}'");
					}
					genericTypes[i] = type0;
				}
				type = type.MakeGenericType(genericTypes);
			}

			if (this.ArrayDimension > 0)
			{
				var elementType = type;
				var elements = new List<Expression>();
				Expression result = null;
				if (argValues != null && argValues.Length > 0)
				{
					result = Expression.NewArrayBounds(elementType, argValues[0]);
				}
				if (this.InitProperties == null || this.InitProperties.Count == 0)
				{
					if (result == null)
					{
						result = Expression.NewArrayBounds(elementType, Expression.Constant(0));
					}
					return result;
				}
				// 
				if (result == null)
				{
					foreach (var propInit in this.InitProperties)
					{
						var elemExpr = propInit.Build(buildContext, scriptContext, options);
						// 将元素转换为数组元素类型
						if (elemExpr.Type != elementType)
						{
							elemExpr = Expression.Convert(elemExpr, elementType);
						}
						elements.Add(elemExpr);
					}
					return Expression.NewArrayInit(elementType, elements);
				}
				// 设置数组项
				// 创建实例变量
				var instanceVar = Expression.Variable(result.Type, "instance");
				var statements = new List<Expression>(2 + this.InitProperties.Count);
				statements.Add(Expression.Assign(instanceVar, result));
				for (int i = 0; i < this.InitProperties.Count; i++)
				{
					var arrayAccess = Expression.ArrayAccess(instanceVar, Expression.Constant(i));
					var item = this.InitProperties[i].Build(buildContext, scriptContext, options);
					statements.Add(Expression.Assign(arrayAccess, item));
				}
				statements.Add(instanceVar);
				return Expression.Block(new[] { instanceVar }, statements); ;
			}

			var con = type.GetConstructor(argTypes);
			var newExpr = Expression.New(con, argValues);

			// 初始化属性列表
			if (this.InitProperties != null && this.InitProperties.Count > 0)
			{
				var initBindings = new List<MemberAssignment>();
				var elementInitializers = new List<Expression>();
				var indexAssignments = new List<Expression>();

				// 检查是否是 IEnumerable 类型（List, Dictionary 等集合）
				var isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
					&& type != typeof(string);

				foreach (var propInit in this.InitProperties)
				{
					if (propInit is OperatorNode opNode)
					{
						if (opNode.Name == "=")
						{
							// 属性赋值: propName = value
							var propValue = opNode.Right.Build(buildContext, scriptContext, options);
							// 属性访问: obj.propName
							if (opNode.Left is VariableNode propNameNode)
							{
								var property = type.GetProperty(propNameNode.Name, BindingFlags.Public | BindingFlags.Instance);
								if (property != null)
								{
									initBindings.Add(Expression.Bind(property, propValue));
								}
							}
						}
						else if (opNode.Name == "[]")
						{
							// 索引器赋值: ["key"] = value 或 [0] = value
							var indexExpr = opNode.Left.Build(buildContext, scriptContext, options);
							var valueExpr = opNode.Right.Build(buildContext, scriptContext, options);

							// 创建索引赋值表达式
							var indexAssign = Expression.Assign(
								Expression.Property(newExpr, "Item", indexExpr),
								valueExpr
							);
							indexAssignments.Add(indexAssign);
						}
						else if (isCollection)
						{
							// 集合初始化器: 直接添加到集合中
							var itemExpr = propInit.Build(buildContext, scriptContext, options);
							elementInitializers.Add(itemExpr);
						}
					}
					else if (isCollection)
					{
						// 集合初始化器: 直接添加到集合中
						var itemExpr = propInit.Build(buildContext, scriptContext, options);
						elementInitializers.Add(itemExpr);
					}
				}

				// 处理集合元素初始化
				if (elementInitializers.Count > 0 && isCollection)
				{
					// 使用列表的 Add 方法添加元素
					var addMethod = type.GetMethod("Add");
					if (addMethod != null)
					{
						// 创建实例变量
						var instanceVar = Expression.Variable(type, "instance");

						// 创建实例
						Expression createInstance;
						if (initBindings.Count > 0)
						{
							createInstance = Expression.MemberInit(Expression.New(con, argValues ?? new Expression[0]), initBindings);
						}
						else
						{
							createInstance = argValues != null ? Expression.New(con, argValues) : Expression.New(con);
						}

						// 构建添加元素的表达式列表
						var statements = new List<Expression>(1 + elementInitializers.Count + indexAssignments.Count + 1)
						{
							Expression.Assign(instanceVar, createInstance)
						};

						// 添加元素
						foreach (var elem in elementInitializers)
						{
							statements.Add(Expression.Call(instanceVar, addMethod, elem));
						}

						// 添加索引器赋值
						foreach (var idxAssign in indexAssignments)
						{
							// 索引赋值需要重新构建，使用 instanceVar 代替 newExpr
							var binaryExpr = idxAssign as BinaryExpression;
							if (binaryExpr != null)
							{
								var indexExpr = ((IndexExpression)binaryExpr.Left).Arguments[0];
								var valueExpr = binaryExpr.Right;
								var assignWithVar = Expression.Assign(
									Expression.Property(instanceVar, "Item", indexExpr),
									valueExpr
								);
								statements.Add(assignWithVar);
							}
						}

						// 返回实例
						statements.Add(instanceVar);

						return Expression.Block(new[] { instanceVar }, statements);
					}
				}

				// 处理索引器赋值 (Dictionary/List 索引器)
				if (indexAssignments.Count > 0)
				{
					var instanceVar = Expression.Variable(type, "instance");

					Expression createInstance;
					if (initBindings.Count > 0)
					{
						createInstance = Expression.MemberInit(Expression.New(con, argValues ?? new Expression[0]), initBindings);
					}
					else
					{
						createInstance = argValues != null ? Expression.New(con, argValues) : Expression.New(con);
					}

					var statements = new List<Expression>
					{
						Expression.Assign(instanceVar, createInstance)
					};

					// 添加索引器赋值
					foreach (var idxAssign in indexAssignments)
					{
						var binaryExpr = idxAssign as BinaryExpression;
						if (binaryExpr != null)
						{
							var indexExpr = ((IndexExpression)binaryExpr.Left).Arguments[0];
							var valueExpr = binaryExpr.Right;
							var assignWithVar = Expression.Assign(
								Expression.Property(instanceVar, "Item", indexExpr),
								valueExpr
							);
							statements.Add(assignWithVar);
						}
					}

					statements.Add(instanceVar);

					return Expression.Block(new[] { instanceVar }, statements);
				}

				// 处理属性绑定
				if (initBindings.Count > 0)
				{
					return Expression.MemberInit(newExpr, initBindings);
				}
			}

			return newExpr;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			string name = this.Name;
			if (this.GenericTypes != null && this.GenericTypes.Count > 0)
			{
				if (this.GenericTypes.Count == 1)
				{
					name = $"{this.Name}<>";
				}
				else
				{
					name = this.Name + "<" + new string(',', this.GenericTypes.Count - 1) + ">";
				}
			}
			var type = context.EvalType(name);
			if (type == null)
			{
				throw new Exception($"unknow type {name}");
			}
			object[] argValues;
			Type[] argTypes;
			if (this.Args == null)
			{
				argValues = null;
#if NETSTANDARD
				argTypes = Array.Empty<Type>();
#else
				argTypes = new Type[0];
#endif
			}
			else
			{
				argValues = new object[this.Args.Count];
				argTypes = new Type[this.Args.Count];
				for (int i = 0; i < this.Args.Count; i++)
				{
					argValues[i] = this.Args[i].Eval(context, options, control, out var argType);
					argTypes[i] = argType;
				}
			}
			if (this.GenericTypes != null && this.GenericTypes.Count > 0)
			{
				var genericTypes = new Type[this.GenericTypes.Count];
				for (int i = 0; i < this.GenericTypes.Count; i++)
				{
					var typeName = this.GenericTypes[i];
					var type0 = context.EvalType(typeName);
					if (type0 == null)
					{
						throw new Exception($"unknown type '{typeName}'");
					}
					genericTypes[i] = type0;
				}
				type = type.MakeGenericType(genericTypes);
			}

			if (this.ArrayDimension > 0)
			{
				int length = 0;
				if (argValues != null && argValues.Length > 0)
				{
					length = Convert.ToInt32(argValues[0]);
				}
				else if (this.InitProperties != null)
				{
					length = this.InitProperties.Count;
				}
				var elementType = type;
				var array = Array.CreateInstance(elementType, length);
				if (this.InitProperties != null)
				{
					for (int i = 0; i < this.InitProperties.Count; i++)
					{
						var itemValue = this.InitProperties[i].Eval(context, options, control, out _);
						if (itemValue != null)
						{
							try
							{
								itemValue = Convert.ChangeType(itemValue, elementType);
							}
							catch { }
						}
						array.SetValue(itemValue, i);
					}
				}
				returnType = array.GetType();
				return array;
			}

			returnType = type;

			var con = type.GetConstructor(argTypes);
			var instance = con.Invoke(argValues);

			// 初始化属性列表
			if (this.InitProperties != null)
			{
				foreach (var propInit in this.InitProperties)
				{
					if (propInit is OperatorNode opNode)
					{
						if (opNode.Name == "=")
						{
							// 属性赋值: propName = value
							var propValue = opNode.Right.Eval(context, options, control, out _);
							// 属性访问: obj.propName
							if (opNode.Left is VariableNode propNameNode)
							{
								ScriptUtils.SetValue(instance, propNameNode.Name, propValue);
							}
						}
						else if (opNode.Name == "[]")
						{
							// 索引器赋值: ["key"] = value 或 [0] = value
							var indexKey = opNode.Left.Eval(context, options, control, out _);
							var propValue = opNode.Right.Eval(context, options, control, out _);

							// 尝试添加到字典
							if (instance is System.Collections.IDictionary dict)
							{
								dict[indexKey] = propValue;
							}
							// 尝试添加到IList
							else if (instance is System.Collections.IList list)
							{
								int index = Convert.ToInt32(indexKey);
								if (index >= 0 && index < list.Count)
								{
									list[index] = propValue;
								}
								else
								{
									list.Add(propValue);
								}
							}
							else
							{
								// 动态调用索引器
								dynamic d = instance;
								d[indexKey] = propValue;
							}
						}
					}
					else
					{
						// 集合初始化器: 直接添加到集合中
						var itemValue = propInit.Eval(context, options, control, out _);
						if (itemValue != null)
						{
							// 尝试添加到IDictionary
							if (instance is System.Collections.IDictionary dict && itemValue is System.Collections.DictionaryEntry entry)
							{
								dict[entry.Key] = entry.Value;
							}
							// 尝试添加到IList
							else if (instance is System.Collections.IList list)
							{
								list.Add(itemValue);
							}
						}
					}
				}
			}

			return instance;
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Args);
			PoolManage.Return(this.InitProperties);

			this.Name = null;
			this.GenericTypes = null;
			this.Args = null;
			this.InitProperties = null;
			this.ArrayDimension = 0;
		}
	}
}
