using AScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
		/// 
		/// </summary>
		public Type SystemType { get; set; }
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
			Type type;
			if (this.SystemType != null)
			{
				type = this.SystemType;
			}
			else if (string.IsNullOrEmpty(this.Name) && this.ArrayDimension == 0)
			{
				// 匿名类型
				string[] fieldNames = null;
				Expression[] fieldValues = null;
				if (this.InitProperties != null && this.InitProperties.Count > 0)
				{
					fieldNames = new string[this.InitProperties.Count];
					fieldValues = new Expression[this.InitProperties.Count];

					for (int i = 0; i < this.InitProperties.Count; i++)
					{
						var propInit = this.InitProperties[i];
						if (propInit is OperatorNode opNode && opNode.Name == "=")
						{
							if (opNode.Left is VariableNode propNameNode)
							{
								fieldNames[i] = propNameNode.Name;
								var propValue = opNode.Right.Build(buildContext, scriptContext, options);
								fieldValues[i] = propValue;
							}
							else
							{
								throw new ScriptAnalyzingException("invalid expression near new");
							}
						}
						else if (propInit is OperatorNode dotNode && (dotNode.Name == "." || dotNode.Name == "?."))
						{
							// 成员访问表达式: a.Name -> 提取属性名
							if (dotNode.Right is VariableNode propNameNode)
							{
								fieldNames[i] = propNameNode.Name;
								var propValue = propInit.Build(buildContext, scriptContext, options);
								fieldValues[i] = propValue;
							}
							else
							{
								throw new ScriptAnalyzingException("invalid expression near new");
							}
						}
						else if (propInit is VariableNode varNode)
						{
							fieldNames[i] = varNode.Name;
							fieldValues[i] = propInit.Build(buildContext, scriptContext, options);
						}
						else
						{
							throw new ScriptAnalyzingException("invalid expression near new");
						}
					}

				}
				return Script.AnonymousTypes.CreateObject(fieldNames, fieldValues);
			}
			else if (!string.IsNullOrEmpty(this.Name))
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
				type = scriptContext.EvalType(name);
				if (type == null)
				{
					throw new ScriptAnalyzingException($"unknow type {this.Name}");
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
							throw new ScriptAnalyzingException($"unknown type '{typeName}'");
						}
						genericTypes[i] = type0;
					}
					type = type.MakeGenericType(genericTypes);
				}
			}
			else type = null;

			if (type == typeof(ExpandoObject))
			{
				// 创建匿名类型对象 ExpandoObject
				var instanceVar = Expression.Variable(typeof(ExpandoObject), "anon");
				var statements = new List<Expression>();
				statements.Add(Expression.Assign(instanceVar, Expression.New(typeof(ExpandoObject))));

				if (this.InitProperties != null)
				{
					foreach (var propInit in this.InitProperties)
					{
						if (propInit is OperatorNode opNode && opNode.Name == "=")
						{
							var propValue = opNode.Right.Build(buildContext, scriptContext, options);
							if (opNode.Left is VariableNode propNameNode)
							{
								// 使用 IDictionary interface 添加属性
								statements.Add(Expression.Call(
									Expression.Convert(instanceVar, typeof(IDictionary<string, object>)),
									ScriptUtils.Method_IDictionary_string_object_Add,
									Expression.Constant(propNameNode.Name),
									propValue.Type == typeof(object) ? propValue : Expression.Convert(propValue, typeof(object))
								));
							}
						}
					}
				}

				statements.Add(instanceVar);
				return Expression.Block(new[] { instanceVar }, statements);
			}

			Expression[] argValues;
			Type[] argTypes;
			if (this.Args == null)
			{
				argValues = null;
				argTypes = Type.EmptyTypes;
			}
			else
			{
				argValues = new Expression[this.Args.Count];
				argTypes = new Type[this.Args.Count];
				for (int i = 0; i < this.Args.Count; i++)
				{
					//var argValue = this.Args[i].Build(buildContext, scriptContext, options);
					//argValues[i] = argValue;
					//argTypes[i] = argValue.Type;
					var arg = this.Args[i];
					if (ScriptUtils.IsDefineFuncNode(arg))
					{
						argValues[i] = null;
						argTypes[i] = typeof(Delegate);
					}
					else
					{
						var argValue = arg.Build(buildContext, scriptContext, options);
						argValues[i] = argValue;
						argTypes[i] = argValue.Type;
					}
				}
			}

			if (this.ArrayDimension > 0)
			{
				var elementType = type;
				Expression result = null;
				if (argValues != null && argValues.Length > 0 && elementType != null)
				{
					result = Expression.NewArrayBounds(elementType, argValues[0]);
				}
				if (this.InitProperties == null || this.InitProperties.Count == 0)
				{
					if (elementType == null)
					{
						throw new Exceptions.ScriptRuntimeException("invalid expression new []");
					}
					if (result == null)
					{
						result = Expression.NewArrayBounds(elementType, Expression.Constant(0));
					}
					return result;
				}
				// 
				if (result == null)
				{
					var elements = new Expression[this.InitProperties.Count];
					bool same = true;
					for (int i = 0; i < this.InitProperties.Count; i++)
					{
						var propInit = this.InitProperties[i];
						var elemExpr = propInit.Build(buildContext, scriptContext, options);
						// 将元素转换为数组元素类型
						if (elemExpr.Type != elementType && elementType != null)
						{
							elemExpr = Expression.Convert(elemExpr, elementType);
						}
						elements[i] = elemExpr;
						if (same && i > 0) same = elemExpr.Type == elements[0].Type;
					}
					if (elementType == null) elementType = same ? elements[0].Type : typeof(object);
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

			ConstructorInfo con = null;
			if (scriptContext.IsObjectMemberEnabled(type) ?? true)
			{
				con = type.GetConstructor(argTypes);
			}
			Expression instance;
			if (con != null)
			{
				instance = Expression.New(con, argValues);
			}
			else if (!string.IsNullOrEmpty(this.Name))
			{
				// 调用方法：new_XXX
				instance = scriptContext.BuildFunc(buildContext, options, null, $"new_{this.Name}", false, this.Args, argValues, false);
			}
			else instance = null;
			if (instance == null)
			{
				if (argTypes == null || argTypes.Length == 0)
				{
					throw new Exceptions.ScriptRuntimeException($"unkown {type.Name}()");
				}
				throw new Exceptions.ScriptRuntimeException($"unkown {type.Name}({string.Join(",", argTypes.Select(a => a.Name))})");
			}

			// 初始化属性列表
			if (this.InitProperties != null && this.InitProperties.Count > 0)
			{
				var initBindings = new List<MemberAssignment>();
				var elementInitializers = new List<Expression>();
				var indexAssignments = new List<Expression>();

				// 检查是否是 IEnumerable 类型（List, Dictionary 等集合）
				var isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
					&& type != typeof(string);

				var flags = BindingFlags.Public | BindingFlags.Instance;
				if (scriptContext.IsIgnoreCase() ?? false)
				{
					flags |= BindingFlags.IgnoreCase;
				}
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
								var property = type.GetProperty(propNameNode.Name, flags);
								if (propValue.Type != property.PropertyType)
								{
									propValue = ScriptUtils.Convert(propValue, property.PropertyType);
								}
								initBindings.Add(Expression.Bind(property, propValue));
							}
						}
						else if (opNode.Name == "." || opNode.Name == "?.")
						{
							var propValue = opNode.Build(buildContext, scriptContext, options);
							var property = type.GetProperty((opNode.Right as VariableNode).Name, flags);
							initBindings.Add(Expression.Bind(property, propValue));
						}
						else if (opNode.Name == "[]")
						{
							// 索引器赋值: ["key"] = value 或 [0] = value
							var indexExpr = opNode.Left.Build(buildContext, scriptContext, options);
							var valueExpr = opNode.Right.Build(buildContext, scriptContext, options);

							// 创建索引赋值表达式
							var itemProperty = Expression.Property(instance, "Item", indexExpr);
							var indexAssign = Expression.Assign(
								itemProperty,
								valueExpr.Type == itemProperty.Type ? valueExpr : Expression.Convert(valueExpr, itemProperty.Type)
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
					else if (propInit is VariableNode varNode)
					{
						var propValue = varNode.Build(buildContext, scriptContext, options);
						var property = type.GetProperty(varNode.Name, flags);
						initBindings.Add(Expression.Bind(property, propValue));
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
						var methodParameter0Type = addMethod.GetParameters()[0].ParameterType;
						foreach (var elem in elementInitializers)
						{
							if (elem.Type != methodParameter0Type)
							{
								statements.Add(Expression.Call(instanceVar, addMethod, Expression.Convert(elem, methodParameter0Type)));
							}
							else
							{
								statements.Add(Expression.Call(instanceVar, addMethod, elem));
							}
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
				if (initBindings.Count > 0 && (instance is NewExpression newExpr))
				{
					return Expression.MemberInit(newExpr, initBindings);
				}
			}

			return instance;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			Type type;
			if (this.SystemType != null)
			{
				type = this.SystemType;
			}
			else if (string.IsNullOrEmpty(this.Name) && this.ArrayDimension == 0)
			{
				// 匿名类型
				string[] fieldNames = null;
				Type[] fieldTypes = null;
				object[] fieldValues = null;
				if (this.InitProperties != null && this.InitProperties.Count > 0)
				{
					fieldNames = new string[this.InitProperties.Count];
					fieldTypes = new Type[this.InitProperties.Count];
					fieldValues = new object[this.InitProperties.Count];

					for (int i = 0; i < this.InitProperties.Count; i++)
					{
						var propInit = this.InitProperties[i];
						if (propInit is OperatorNode opNode && opNode.Name == "=")
						{
							if (opNode.Left is VariableNode propNameNode)
							{
								fieldNames[i] = propNameNode.Name;
								fieldValues[i] = opNode.Right.Eval(context, options, control, out var propType);
								fieldTypes[i] = propType;
							}
							else
							{
								throw new ScriptAnalyzingException("invalid expression near new");
							}
						}
						else if (propInit is OperatorNode dotNode && dotNode.Name == ".")
						{
							// 成员访问表达式: a.Name -> 提取属性名
							if (dotNode.Right is VariableNode propNameNode)
							{
								fieldNames[i] = propNameNode.Name;
								fieldValues[i] = propInit.Eval(context, options, control, out var propType);
								fieldTypes[i] = propType;
							}
							else
							{
								throw new ScriptAnalyzingException("invalid expression near new");
							}
						}
						else if (propInit is VariableNode varNode)
						{
							fieldNames[i] = varNode.Name;
							fieldValues[i] = propInit.Eval(context, options, control, out var propType);
							fieldTypes[i] = propType;
						}
						else
						{
							throw new ScriptAnalyzingException("invalid expression near new");
						}
					}
				}
				returnType = Script.AnonymousTypes.CreateType(fieldNames, fieldTypes);
				return Activator.CreateInstance(returnType, fieldValues);
			}
			else if (!string.IsNullOrEmpty(this.Name))
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
				type = context.EvalType(name);
				if (type == null)
				{
					throw new ScriptAnalyzingException($"unknow type {name}");
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
							throw new ScriptAnalyzingException($"unknown type '{typeName}'");
						}
						genericTypes[i] = type0;
					}
					type = type.MakeGenericType(genericTypes);
				}
			}
			else type = null;
			// 
			if (type == typeof(ExpandoObject))
			{
				// 创建匿名类型对象 ExpandoObject
				dynamic expando = new ExpandoObject();
				var dict = expando as IDictionary<string, object>;

				if (this.InitProperties != null)
				{
					foreach (var propInit in this.InitProperties)
					{
						if (propInit is OperatorNode opNode && opNode.Name == "=")
						{
							var propValue = opNode.Right.Eval(context, options, control, out _);
							if (opNode.Left is VariableNode propNameNode)
							{
								dict[propNameNode.Name] = propValue;
							}
						}
					}
				}

				returnType = typeof(ExpandoObject);
				return expando;
			}
			object[] argValues;
			Type[] argTypes;
			if (this.Args == null)
			{
				argValues = null;
				argTypes = Type.EmptyTypes;
			}
			else
			{
				argValues = new object[this.Args.Count];
				argTypes = new Type[this.Args.Count];
				for (int i = 0; i < this.Args.Count; i++)
				{
					var arg = this.Args[i];
					if (ScriptUtils.IsDefineFuncNode(arg))
					{
						argValues[i] = arg;
						argTypes[i] = typeof(Delegate);
					}
					else
					{
						argValues[i] = arg.Eval(context, options, control, out var argType);
						argTypes[i] = argType;
					}
				}
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
				if (elementType == null)
				{
					if (this.InitProperties == null || this.InitProperties.Count == 0)
					{
						throw new Exceptions.ScriptRuntimeException("invalid expression new []");
					}
					var initValues = new object[this.InitProperties.Count];
					for (int i = 0; i < this.InitProperties.Count; i++)
					{
						initValues[i] = this.InitProperties[i].Eval(context, options, control, out var valueType);
						if (elementType == null)
						{
							elementType = valueType;
						}
						else if (elementType != typeof(object) && valueType != elementType)
						{
							elementType = typeof(object);
						}
					}
					var array = Array.CreateInstance(elementType, length);
					for (int i = 0; i < initValues.Length; i++)
					{
						array.SetValue(initValues[i], i);
					}
					returnType = array.GetType();
					return array;
				}
				else
				{
					var array = Array.CreateInstance(elementType, length);
					if (this.InitProperties != null && this.InitProperties.Count > 0)
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
			}

			returnType = type;

			ConstructorInfo con = null;
			if (context.IsObjectMemberEnabled(type) ?? true)
			{
				con = type.GetConstructor(argTypes);
			}
			object instance;
			if (con != null)
			{
				instance = con.Invoke(argValues);
			}
			else if (!string.IsNullOrEmpty(this.Name))
			{
				// 调用方法：new_XXX
				instance = context.EvalFunc($"new_{this.Name}", argValues, argTypes);
			}
			else
			{
				if (argTypes == null || argTypes.Length == 0)
				{
					throw new Exceptions.ScriptRuntimeException($"unkown {type.Name}()");
				}
				throw new Exceptions.ScriptRuntimeException($"unkown {type.Name}({string.Join(",", argTypes.Select(a => a.Name))})");
			}

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
							else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
							{
								dynamic d = instance;
								d.Add(itemValue);
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
