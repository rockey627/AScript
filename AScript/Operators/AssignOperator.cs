using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using AScript.Nodes;
using System.Collections;
using System.Linq;
using AScript.Exceptions;
using System.Dynamic;

namespace AScript.Operators
{
	public class AssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly AssignOperator Instance = new AssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0 = e.Args[0];
			if (arg0 is VariableNode v)
			{
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				e.Result = HandleVariableAssign(e, v, right);
			}
			else if (arg0 is OperatorNode opNode && opNode.Name == "[]")
			{
				// 索引器赋值
				var obj = opNode.Left.Build(e.BuildContext, e.ScriptContext, e.Options);
				var idx = opNode.Right.Build(e.BuildContext, e.ScriptContext, e.Options);
				var value = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);

				// 如果idx是object类型，转换为实际需要的类型
				if (idx.Type == typeof(object))
				{
					var indexType = GetIndexType(obj.Type);
					if (indexType != null)
					{
						idx = Expression.Convert(idx, indexType);
					}
				}

				// 如果value是object类型，需要转换为元素的实际类型
				Expression valueExpr = value;
				if (value.Type == typeof(object))
				{
					var elementType = GetElementType(obj.Type);
					if (elementType != null)
					{
						valueExpr = Expression.Convert(value, elementType);
					}
				}

				// 判断obj类型并生成相应的索引器赋值表达式
				if (obj.Type.IsArray)
				{
					// 数组赋值
					e.Result = Expression.Assign(Expression.ArrayAccess(obj, idx), valueExpr);
				}
				else
				{
					var objType = obj.Type == typeof(ExpandoObject) ? typeof(IDictionary<string, object>) : obj.Type;
					// 尝试使用索引器（Item属性）赋值
					var indexer = objType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
					if (indexer != null)
					{
						var p0 = indexer.SetMethod.GetParameters()[0];
						if (idx.Type != p0.ParameterType)
						{
							idx = Expression.Convert(idx, p0.ParameterType);
						}
						var property = Expression.Property(obj, indexer, idx);
						if (valueExpr.Type != property.Type)
						{
							valueExpr = Expression.Convert(valueExpr, property.Type);
						}
						e.Result = Expression.Assign(property, valueExpr);
					}
					else
					{
						// 尝试使用set_Item方法
						var setItemMethod = objType.GetMethod("set_Item");
						if (setItemMethod != null)
						{
							var p0 = setItemMethod.GetParameters()[0];
							if (idx.Type != p0.ParameterType)
							{
								idx = Expression.Convert(idx, p0.ParameterType);
							}
							e.Result = Expression.Call(obj, setItemMethod, idx, valueExpr);
						}
						else
						{
							// 使用动态表达式进行动态赋值
							e.Result = Expression.Dynamic(
								IndexSetBinder,
								typeof(object),
								obj,
								idx,
								value);
						}
					}
				}
			}
			else if (arg0 is CallFuncNode callFuncNode && callFuncNode.Name == "[:]")
			{
				// 切片赋值 a[1:3] = [10, 20]
				// callFuncNode.Args[0] 是列表
				// callFuncNode.Args[1] 是起始索引
				// callFuncNode.Args[2] 是 end 索引
				// e.Args[1] 是要赋值的值列表
				var list = callFuncNode.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var start = callFuncNode.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				var end = callFuncNode.Args[2].Build(e.BuildContext, e.ScriptContext, e.Options);
				var values = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);

				var listExpr = Expression.Convert(list, typeof(IList));
				var startExpr = Expression.Convert(start, typeof(int));
				var endExpr = Expression.Convert(end, typeof(int));
				var valuesExpr = Expression.Convert(values, typeof(IList));
				e.Result = Expression.Call(ExpressionUtils.Method_ScriptUtils_SliceAssign, listExpr, startExpr, endExpr, valuesExpr);
			}
			else if (arg0 is TupleNode tupleNode)
			{
				// 元组解构
				HandleTupleBuild(e, tupleNode.Items);
			}
			else if (arg0 is CollectionNode collectionNode)
			{
				var value = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (collectionNode.CollectionType == typeof(object))
				{
					e.Result = BuildDeconstructObjectProperty(e, collectionNode, value);
				}
				else
				{
					e.Result = BuildDeconstructArray(e, collectionNode, value);
				}
			}
			else
			{
				var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				if (right.Type != left.Type)
				{
					right = Expression.Convert(right, left.Type);
				}
				e.Result = Expression.Assign(left, right);
			}
			//else if (arg0 is OperatorNode opNode && opNode.Name == "." && opNode.Right is VariableNode opRightNode)
			//{
			//	// 属性赋值
			//	var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			//	var opLeftValue = opNode.Left.Build(e.BuildContext, e.ScriptContext, e.Options);
			//	e.Result = ExpressionUtils.SetValue(opLeftValue, opRightNode.Name, right);
			//}
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0 = e.Args[0];

			if (arg0 is VariableNode varNode)
			{
				string varName = varNode.Name;
				var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type);

				if (varName == "_")
				{
					e.SetResult(value, type);
					return;
				}

				// 获取变量名和声明类型
				Type declaredType;
				int modifier = 0;
				if (arg0 is DefineVarNode def)
				{
					modifier = def.Modifier;
					declaredType = def.SystemType ?? e.Context.EvalType(def.Type);
					// 先设置变量类型
					if (declaredType != null && declaredType != typeof(object) && declaredType != typeof(void))
					{
						e.Context.SetTempVar(varName, null, declaredType, false);
					}
				}
				else
				{
					declaredType = e.Context.GetVarType(varName);
				}

				if (e.Options.Dynamic ?? e.Context.IsDynamicLang() ?? false)
				{
					// 动态语言
				}
				// 如果声明了类型，进行类型转换
				else if (declaredType != null && declaredType != typeof(object) && declaredType != typeof(void)
					&& type != null && type != declaredType && type != typeof(void))
				{
					try
					{
						value = Convert.ChangeType(value, declaredType);
						type = declaredType;
					}
					catch
					{
						// 转换失败时保留原值
					}
				}

				//if (value is Delegate del)
				//{
				//	e.Context.AddTempFunc(varName, del);
				//}
				//else if (value is CustomFunctionObject customFunctionObject)
				//{
				//	e.Context.AddFunc(customFunctionObject.Function);
				//}
				if (Modifiers.IsReadOnly(modifier))
				{
					e.Context.SetTempConst(varName, value, type, false);
				}
				else
				{
					e.Context.SetTempVar(varName, value, type, true);
				}
				e.SetResult(value, type);
				return;
			}
			else if (arg0 is OperatorNode opNode)
			{
				if (opNode.Name == "." && opNode.Right is VariableNode opRightNode)
				{
					// 属性赋值
					var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type);
					var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					ScriptUtils.SetValue(opLeftValue, opRightNode.Name, value);
					e.SetResult(value, type);
					return;
				}
				if (opNode.Name == "[]")
				{
					// 设置索引值
					var obj = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					var idx = opNode.Right.Eval(e.Context, e.Options, e.Control, out _);
					var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type);

					// 根据obj类型处理索引器赋值
					if (obj is Array array)
					{
						// 数组赋值
						int index = Convert.ToInt32(idx);
						array.SetValue(value, index);
					}
					else if (obj is IDictionary dict)
					{
						// Dictionary赋值
						dict[idx] = value;
					}
					else if (obj is IList list)
					{
						list[Convert.ToInt32(idx)] = value;
					}
					else if (obj is ExpandoObject expandoObj)
					{
						(expandoObj as IDictionary<string, object>)[idx.ToString()] = value;
					}
					else if (obj != null)
					{
						// 其他类型使用动态调用
						dynamic dObj = obj;
						dObj[idx] = value;
					}

					e.SetResult(value, type);
					return;
				}
			}
			else if (arg0 is CallFuncNode callFuncNode && callFuncNode.Name == "[:]")
			{
				// 切片赋值 a[1:3] = [10, 20]
				// callFuncNode.Args[0] 是列表
				// callFuncNode.Args[1] 是起始索引
				// callFuncNode.Args[2] 是 end 索引
				// e.Args[1] 是要赋值的值列表
				var list = callFuncNode.Args[0].Eval(e.Context, e.Options, e.Control, out var type);
				var start = callFuncNode.Args[1].Eval(e.Context, e.Options, e.Control, out _);
				var end = callFuncNode.Args[2].Eval(e.Context, e.Options, e.Control, out _);
				var values = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);

				ScriptUtils.SliceAssign(list as IList, Convert.ToInt32(start), Convert.ToInt32(end), values as IList);

				e.SetResult(list, list == null ? type : list.GetType());
				return;
			}
			else if (arg0 is TupleNode tupleNode0)
			{
				// 元组解构
				DecontructTuple(e, tupleNode0, e.Args[1]);
				return;
			}
			else if (arg0 is CollectionNode collectionNode)
			{
				if (collectionNode.CollectionType == typeof(object))
				{
					DecontructObjectProperty(e, collectionNode, e.Args[1]);
				}
				else
				{
					DecontructArray(e, collectionNode, e.Args[1]);
				}
				return;
			}
		}

		private void Decontruct(FunctionEvalArgs e, ITreeNode item, object value, Type valueType = null)
		{
			if (item is DefineVarNode defineVarNode)
			{
				if (value is ITreeNode treeNode)
				{
					value = treeNode.Eval(e.Context, e.Options, e.Control, out valueType);
				}
				if (!string.IsNullOrEmpty(defineVarNode.Name) && defineVarNode.Name != "_")
				{
					if (Modifiers.IsReadOnly(defineVarNode.Modifier))
					{
						e.Context.SetTempConst(defineVarNode.Name, value, valueType, false);
					}
					else
					{
						e.Context.SetTempVar(defineVarNode.Name, value, valueType, false);
					}
				}
				return;
			}
			if (item is VariableNode variableNode)
			{
				if (value is ITreeNode treeNode)
				{
					value = treeNode.Eval(e.Context, e.Options, e.Control, out valueType);
				}
				if (!string.IsNullOrEmpty(variableNode.Name) && variableNode.Name != "_")
				{
					e.Context.SetTempVar(variableNode.Name, value, valueType, true);
				}
				return;
			}
			if (item is OperatorNode operatorNode)
			{
				if (operatorNode.Name == "=")
				{
					// 默认值
					if (valueType == null)
					{
						value = operatorNode.Right.Eval(e.Context, e.Options, e.Control, out valueType);
					}
					Decontruct(e, operatorNode.Left, value, valueType);
					return;
				}
				throw new Exceptions.ScriptRuntimeException($"unsupport decontruct {operatorNode.Name}");
			}
			if (item is TupleNode tupleNode)
			{
				DecontructTuple(e, tupleNode, value);
				return;
			}
			if (item is CollectionNode collectionNode)
			{
				if (collectionNode.CollectionType == typeof(object))
				{
					DecontructObjectProperty(e, collectionNode, value);
				}
				else
				{
					DecontructArray(e, collectionNode, value);
				}
				return;
			}
			throw new Exceptions.ScriptRuntimeException($"unsupport decontruct {item.GetType().Name}");
		}

		/// <summary>
		/// 解构元组
		/// </summary>
		/// <param name="e"></param>
		/// <param name="item"></param>
		/// <param name="value"></param>
		/// <exception cref="ScriptAnalyzingException"></exception>
		private void DecontructTuple(FunctionEvalArgs e, TupleNode item, object value)
		{
			if (value is TupleNode tupleNode)
			{
				//if (item.Items.Count > tupleNode.Items.Count)
				//{
				//	throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
				//}
				int minCount = Math.Min(item.Items.Count, tupleNode.Items.Count);
				//var itemValues = new object[arg0Items.Count];
				//var itemTypes = new Type[arg0Items.Count];
				for (int i = 0; i < minCount; i++)
				{
					var value0 = tupleNode.Items[i].Eval(e.Context, e.Options, e.Control, out var valueType0);
					if (i < item.Items.Count)
					{
						//itemValues[i] = value;
						//itemTypes[i] = itemType;
						//var varName = ((VariableNode)item.Items[i]).Name;
						//if (varName != "_")
						//{
						//	e.Context.SetTempVar(varName, value0, itemType, searchContext ?? !(arg0Items[i] is DefineVarNode));
						//}
						Decontruct(e, item.Items[i], value0, valueType0);
					}
				}
				for (int i = minCount; i < item.Items.Count; i++)
				{
					item.Items[i].Eval(e.Context, e.Options, e.Control, out _);
				}
				for (int i = minCount; i < tupleNode.Items.Count; i++)
				{
					tupleNode.Items[i].Eval(e.Context, e.Options, e.Control, out _);
				}
				// 返回元组
				//e.SetResult(TupleNode.CreateTuple(itemValues, itemTypes));
				e.SetResult(null, typeof(void));
				return;
			}
			if (value is ITreeNode treeNode)
			{
				value = treeNode.Eval(e.Context, e.Options, e.Control, out _);
			}
			var arg1Type = value?.GetType();
			var arg1TypeName = arg1Type?.Name;
			if (arg1TypeName != null && (arg1TypeName.StartsWith("ValueTuple`") || arg1TypeName.StartsWith("Tuple`")))
			{
				bool isValueTuple = arg1TypeName.StartsWith("ValueTuple`");
				int arg1FieldCount = isValueTuple ? arg1Type.GetFields().Length : arg1Type.GetProperties().Length;
				if (item.Items.Count > arg1FieldCount)
				{
					throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
				}
				//var itemValues = arg0Items.Count == arg1FieldCount ? null : new object[arg0Items.Count];
				//var itemTypes = arg0Items.Count == arg1FieldCount ? null : new Type[arg0Items.Count];
				for (int i = 0; i < item.Items.Count; i++)
				{
					//var varName = ((VariableNode)arg0Items[i]).Name;
					//if (varName == "_" && itemValues == null) continue;
					//if (varName == "_") continue;
					object value0;
					Type valueType0;
					if (isValueTuple)
					{
						var info = arg1Type.GetField($"Item{i + 1}");
						value0 = info.GetValue(value);
						valueType0 = info.FieldType;
					}
					else
					{
						var info = arg1Type.GetProperty($"Item{i + 1}");
						value0 = info.GetValue(value);
						valueType0 = info.PropertyType;
					}
					Decontruct(e, item.Items[i], value0, valueType0);
					//if (itemValues != null)
					//{
					//	itemValues[i] = value;
					//	itemTypes[i] = itemType;
					//}
					//if (varName != "_")
					//{
					//	e.Context.SetTempVar(varName, value, itemType, searchContext ?? !(arg0Items[i] is DefineVarNode));
					//}
				}
				// 返回元组
				//e.SetResult(itemValues == null ? arg1 : TupleNode.CreateTuple(itemValues, itemTypes));
				e.SetResult(null, typeof(void));
				return;
			}

			throw new ScriptAnalyzingException("invalid expression near =");
		}

		/// <summary>
		/// 解构对象属性或字典：var { name, age } = new Person { name = 'tom', age = 20 }
		/// </summary>
		/// <param name="e"></param>
		/// <param name="item"></param>
		/// <param name="value"></param>
		private void DecontructObjectProperty(FunctionEvalArgs e, CollectionNode item, object value)
		{
			if (value is ITreeNode treeNode)
			{
				value = treeNode.Eval(e.Context, e.Options, e.Control, out _);
			}
			for (int i = 0; i < item.Items.Count; i++)
			{
				//var varNode = arg0Items[i] as VariableNode;
				//if (varNode == null)
				//{
				//	throw new ScriptAnalyzingException("invalid expression near =, expected variable name");
				//}
				//if (varNode.Name == "_") continue;

				//object value0;
				//Type valueType0;
				//if (valueTop == null)
				//{
				//	value0 = null;
				//	valueType0 = typeof(object);
				//}
				//else
				//{
				//	value0 = ScriptUtils.GetValue(valueTop, varNode.Name, out valueType0);
				//}
				DecontructObjectProperty(e, item.Items[i], value);
				//e.Context.SetTempVar(varNode.Name, value0, valueType0, searchContext ?? !(arg0Items[i] is DefineVarNode));
			}
			e.SetResult(null, typeof(void));
		}

		private void DecontructObjectProperty(FunctionEvalArgs e, ITreeNode item, object value)
		{
			if (item is VariableNode variableNode)
			{
				if (!string.IsNullOrEmpty(variableNode.Name) && variableNode.Name != "_")
				{
					object value0;
					Type valueType0;
					if (value == null)
					{
						value0 = null;
						valueType0 = null;
					}
					else
					{
						value0 = ScriptUtils.GetValue(value, variableNode.Name, out valueType0, false);
					}
					Decontruct(e, item, value0, valueType0);
				}
				return;
			}
			if (item is OperatorNode operatorNode)
			{
				if (operatorNode.Name == "=" && operatorNode.Left is VariableNode variableNode1)
				{
					if (!string.IsNullOrEmpty(variableNode1.Name) && variableNode1.Name != "_")
					{
						object value0;
						Type valueType0;
						if (value == null)
						{
							value0 = null;
							valueType0 = null;
						}
						else
						{
							value0 = ScriptUtils.GetValue(value, variableNode1.Name, out valueType0, false);
						}
						if (valueType0 == null)
						{
							value0 = operatorNode.Right.Eval(e.Context, e.Options, e.Control, out valueType0);
						}
						Decontruct(e, operatorNode.Left, value0, valueType0);
					}
					return;
				}
			}
			else if (item is PropertyMapNode propertyMapNode)
			{
				var value0 = ScriptUtils.GetValue(value, propertyMapNode.PropertyName, out var valueType0, false);
				Decontruct(e, propertyMapNode.MapNode, value0, valueType0);
				return;
			}
			throw new Exceptions.ScriptRuntimeException($"unsupport decontruct {item.GetType().Name}");
		}

		/// <summary>
		/// 列表解构：var { name1, name2 } = ['tom', 'tony', 'jim']
		/// </summary>
		/// <param name="e"></param>
		/// <param name="item"></param>
		/// <param name="value"></param>
		private void DecontructArray(FunctionEvalArgs e, CollectionNode item, object value)
		{
			if (value is ITreeNode treeNode)
			{
				value = treeNode.Eval(e.Context, e.Options, e.Control, out _);
			}
			if (value == null)
			{
				throw new ScriptAnalyzingException("invalid expression near =, right side is null");
			}

			if (!(value is IList list))
			{
				throw new ScriptAnalyzingException("invalid expression near =, right side is not a list");
			}

			for (int i = 0; i < item.Items.Count; i++)
			{
				var item0 = item.Items[i];
				if (item0 == null) continue;
				//var varNode = item0 as VariableNode;
				//if (varNode == null)
				//{
				//	throw new ScriptAnalyzingException("invalid expression near =, expected variable name");
				//}
				//if (varNode.Name == "_") continue;

				object value0 = i < list.Count ? list[i] : null;
				Type valueType0 = i < list.Count ? list[i]?.GetType() ?? typeof(object) : null;
				//e.Context.SetTempVar(varNode.Name, value, valueType, searchContext ?? !(arg0Items[i] is DefineVarNode));
				Decontruct(e, item0, value0, valueType0);
			}
			e.SetResult(null, typeof(void));
		}

		private Expression HandleVariableAssign(FunctionBuildArgs e, VariableNode arg0Node, Expression right, bool? searchContext = null)
		{
			if (arg0Node.Name == "_") return right;

			ParameterExpression left = null;
			// 获取变量声明的类型（如果有）
			Type declaredType = null;
			BuildContext ownerBuildContext = null;
			if (arg0Node is DefineVarNode defineVar)
			{
				declaredType = defineVar.SystemType ?? e.ScriptContext.EvalType(defineVar.Type);
				if (declaredType != null && (declaredType == typeof(object) || declaredType == typeof(void)))
				{
					declaredType = null;
				}
				left = null;
				e.BuildContext.LocalVariables.Add(arg0Node.Name);
				if (defineVar.Modifier != 0)
				{
					e.BuildContext.VariableModifiers[arg0Node.Name] = defineVar.Modifier;
				}
			}
			else
			{
				if (!(searchContext ?? true))
				{
					e.BuildContext.LocalVariables.Add(arg0Node.Name);
				}
				//e.BuildContext.TryGetVariableOrParameter(v.Name, out left, out ownerBuildContext, out _);
				//// 是否在执行上下文中存在变量
				//var ownerContext = e.ScriptContext.GetOwnerContext(v.Name, out _, out _);
				//if (ownerContext == null)
				//{
				//	e.BuildContext.LocalVariables.Add(v.Name);
				//}
				left = arg0Node.BuildForAssign(e.BuildContext, e.ScriptContext, e.Options, out ownerBuildContext, out _);
			}

			if (declaredType == null)
			{
				if (left != null)
				{
					declaredType = left.Type;
				}
				else if (e.Options.Dynamic ?? e.ScriptContext.IsDynamicLang() ?? false)
				{
					declaredType = typeof(object);
				}
			}

			// 记录最新类型
			if (declaredType == typeof(object) && right.Type != typeof(object))
			{
				(ownerBuildContext ?? e.BuildContext).LastTypes[arg0Node.Name] = right.Type;
			}

			// 如果声明了类型，进行类型转换
			Expression rightExpr = right;
			if (declaredType != null && right.Type != declaredType)
			{
				rightExpr = Expression.Convert(right, declaredType);
			}

			if (left == null)
			{
				// 定义变量
				left = Expression.Variable(declaredType ?? right.Type, arg0Node.Name);
				e.BuildContext.Variables[arg0Node.Name] = left;
			}
			if (right is LambdaExpression lambdaExpression)
			{
				(ownerBuildContext ?? e.BuildContext).AddTempFunc(left.Name, lambdaExpression);
			}
			return Expression.Assign(left, rightExpr);
		}

		private void HandleTupleBuild(FunctionBuildArgs e, IList<ITreeNode> arg0Items)
		{
			var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
			var rightType = right.Type;
			var rightTypeName = rightType.Name;
			bool isValueTuple = rightTypeName.StartsWith("ValueTuple`");
			bool isTuple = rightTypeName.StartsWith("Tuple`");
			if (!isValueTuple && !isTuple)
			{
				throw new ScriptAnalyzingException("invalid expression near =, right side is not a tuple");
			}

			int rightFieldCount = isValueTuple ? rightType.GetFields().Length : rightType.GetProperties().Length;
			//if (arg0Items.Count > rightFieldCount)
			//{
			//	throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
			//}
			int minCount = Math.Min(arg0Items.Count, rightFieldCount);
			var expressions = new List<Expression>(arg0Items.Count);
			for (int i = 0; i < minCount; i++)
			{
				var item = arg0Items[i];
				var value = isValueTuple ? Expression.Field(right, $"Item{i + 1}") : Expression.Property(right, $"Item{i + 1}");
				var expr = BuildDeconstruct(e, item, value);
				if (expr != null) expressions.Add(expr);
			}
			for (int i = minCount; i < arg0Items.Count; i++)
			{
				expressions.Add(arg0Items[i].Build(e.BuildContext, e.ScriptContext, e.Options));
			}

			e.Result = Expression.Block(typeof(void), expressions);
		}

		private Expression BuildDeconstruct(FunctionBuildArgs e, ITreeNode item, Expression right)
		{
			if (item is DefineVarNode defNode)
			{
				if (defNode.Name == "_") return null;
				return HandleVariableAssign(e, defNode, right);
			}
			if (item is VariableNode varNode)
			{
				if (varNode.Name == "_") return null;
				return HandleVariableAssign(e, varNode, right);
			}
			if (item is TupleNode tupleNode)
			{
				var rightType = right.Type;
				var rightTypeName = rightType.Name;
				bool isValueTuple = rightTypeName.StartsWith("ValueTuple`");
				bool isTuple = rightTypeName.StartsWith("Tuple`");
				if (!isValueTuple && !isTuple)
				{
					throw new ScriptAnalyzingException("invalid expression near =, right side is not a tuple");
				}
				int rightFieldCount = isValueTuple ? rightType.GetFields().Length : rightType.GetProperties().Length;
				if (tupleNode.Items.Count > rightFieldCount)
				{
					throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
				}
				var expressions = new List<Expression>(tupleNode.Items.Count);
				for (int i = 0; i < tupleNode.Items.Count; i++)
				{
					var value = isValueTuple ? Expression.Field(right, $"Item{i + 1}") : Expression.Property(right, $"Item{i + 1}");
					var expr = BuildDeconstruct(e, tupleNode.Items[i], value);
					if (expr != null) expressions.Add(expr);
				}
				return Expression.Block(typeof(void), expressions);
			}
			if (item is CollectionNode collectionNode)
			{
				if (collectionNode.CollectionType == typeof(object))
				{
					return BuildDeconstructObjectProperty(e, collectionNode, right);
				}
				else
				{
					return BuildDeconstructArray(e, collectionNode, right);
				}
			}
			if (item is OperatorNode opNode)
			{
				if (opNode.Name == "=" && opNode.Left is VariableNode leftVar)
				{
					// 重命名语法 { name: n } = obj
					if (leftVar.Name == "_") return null;
					//var value = ExpressionUtils.GetValue(right, leftVar.Name);
					if (right == null)
					{
						right = opNode.Right.Build(e.BuildContext, e.ScriptContext, e.Options);
					}
					else
					{
						var defaultValue = opNode.Right.Build(e.BuildContext, e.ScriptContext, e.Options);
						var tmpVar = Expression.Variable(right.Type);
						var tmpAssign = Expression.Assign(tmpVar, right);
						if (defaultValue.Type != right.Type)
						{
							defaultValue = Expression.Convert(defaultValue, right.Type);
						}
						var right2 = Expression.Condition(Expression.Equal(tmpVar, ExpressionUtils.Constant_null), defaultValue, tmpVar);
						right = Expression.Block(new[] { tmpVar }, tmpAssign, right2);
					}
					return BuildDeconstruct(e, leftVar, right);
				}
				throw new ScriptRuntimeException($"unsupport deconstruct {opNode.Name}");
			}
			throw new ScriptRuntimeException($"unsupport deconstruct {item.GetType().Name}");
		}

		/// <summary>
		/// 解构对象属性或字典：var { name, age } = new Person { name = 'tom', age = 20 };
		/// </summary>
		/// <param name="e"></param>
		/// <param name="collectionNode"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ScriptAnalyzingException"></exception>
		private Expression BuildDeconstructObjectProperty(FunctionBuildArgs e, CollectionNode collectionNode, Expression value)
		{
			var expressions = new List<Expression>(collectionNode.Items.Count);
			for (int i = 0; i < collectionNode.Items.Count; i++)
			{
				var item = collectionNode.Items[i];
				if (item is VariableNode varNode)
				{
					if (varNode.Name == "_") continue;
					var value0 = ExpressionUtils.GetValue(value, varNode.Name);
					expressions.Add(BuildDeconstruct(e, item, value0));
				}
				else if (item is OperatorNode opNode && opNode.Name == "=" && opNode.Left is VariableNode leftVar)
				{
					if (leftVar.Name == "_") continue;
					var defaultValue = opNode.Right.Build(e.BuildContext, e.ScriptContext, e.Options);
					var value0 = ExpressionUtils.GetValue(value, leftVar.Name, defaultValue: defaultValue);
					expressions.Add(BuildDeconstruct(e, leftVar, value0));
				}
				else if (item is PropertyMapNode propertyMapNode)
				{
					var value0 = ExpressionUtils.GetValue(value, propertyMapNode.PropertyName);
					expressions.Add(BuildDeconstruct(e, propertyMapNode.MapNode, value0));
				}
				else
				{
					throw new ScriptAnalyzingException("invalid expression near =, expected variable name");
				}
			}
			return Expression.Block(typeof(void), expressions);
		}

		/// <summary>
		/// 列表解构：var { name1, name2 } = ['tom', 'tony', 'jim']
		/// </summary>
		/// <param name="e"></param>
		/// <param name="collectionNode"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ScriptAnalyzingException"></exception>
		private Expression BuildDeconstructArray(FunctionBuildArgs e, CollectionNode collectionNode, Expression value)
		{
			var rightType = value.Type;
			var collectionType = rightType;
			var isArray = rightType.IsArray;
			var isList = typeof(IList).IsAssignableFrom(rightType);
			if (!isArray && !isList && rightType == typeof(object))
			{
				value = Expression.Convert(value, typeof(IList));
				rightType = value.Type;
				collectionType = typeof(ICollection);
				isList = true;
			}
			if (!isArray && !isList)
			{
				throw new ScriptAnalyzingException("invalid expression near =, right side is not a list");
			}
			// 获取集合长度
			Expression countExpr;
			if (isArray)
			{
				countExpr = Expression.ArrayLength(value);
			}
			else
			{
				var countProperty = collectionType.GetProperty("Count");
				if (countProperty != null)
				{
					countExpr = Expression.Property(value, countProperty);
				}
				else
				{
					var getCountMethod = collectionType.GetMethod("get_Count");
					countExpr = Expression.Call(value, getCountMethod);
				}
			}
			var expressions = new List<Expression>(collectionNode.Items.Count);
			for (int i = 0; i < collectionNode.Items.Count; i++)
			{
				var item = collectionNode.Items[i];
				if (item == null) continue;
				// 判断索引是否在边界内
				var indexExpr = Expression.Constant(i);
				var isInBounds = Expression.LessThan(indexExpr, countExpr);
				Expression value0;
				if (isArray)
				{
					value0 = Expression.Condition(
						isInBounds,
						Expression.ArrayAccess(value, indexExpr),
						Expression.Constant(null, typeof(object))
					);
				}
				else
				{
					var indexer = rightType.GetProperty("Item");
					Expression getValueExpr;
					if (indexer != null)
					{
						getValueExpr = Expression.Property(value, indexer, indexExpr);
					}
					else
					{
						var getItemMethod = rightType.GetMethod("get_Item");
						getValueExpr = Expression.Call(value, getItemMethod, indexExpr);
					}
					value0 = Expression.Condition(
						isInBounds,
						getValueExpr,
						Expression.Constant(null, typeof(object))
					);
				}
				var expr = BuildDeconstruct(e, item, value0);
				if (expr != null) expressions.Add(expr);
			}
			return Expression.Block(typeof(void), expressions);
		}

		/// <summary>
		/// 动态索引赋值Binder
		/// </summary>
		private static readonly CallSiteBinder IndexSetBinder = Microsoft.CSharp.RuntimeBinder.Binder.SetIndex(
			CSharpBinderFlags.None,
			typeof(object),
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

		/// <summary>
		/// 获取索引类型
		/// </summary>
		private static Type GetIndexType(Type containerType)
		{
			if (containerType.IsArray)
			{
				return typeof(int);
			}
			if (containerType.IsGenericType)
			{
				var args = containerType.GetGenericArguments();
				if (containerType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
					|| containerType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
				{
					return args[0];
				}
				var indexer = containerType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
				if (indexer != null)
				{
					var indexParams = indexer.GetIndexParameters();
					if (indexParams.Length > 0)
					{
						return indexParams[0].ParameterType;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 获取元素类型
		/// </summary>
		private static Type GetElementType(Type containerType)
		{
			if (containerType.IsArray)
			{
				return containerType.GetElementType();
			}
			if (containerType.IsGenericType)
			{
				var args = containerType.GetGenericArguments();
				if (containerType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
					|| containerType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
				{
					return args[1];
				}
				var indexer = containerType.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
				if (indexer != null)
				{
					return indexer.PropertyType;
				}
			}
			return null;
		}

		//private static bool IsDictionaryType(Type type)
		//{

		//}
	}
}
