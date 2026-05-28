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
					// 尝试使用索引器（Item属性）赋值
					var indexer = obj.Type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);
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
						var setItemMethod = obj.Type.GetMethod("set_Item");
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
			else if (arg0 is CallFuncNode callFuncNode2 && callFuncNode2.Name == "var")
			{
				// 元组解构
				HandleTupleBuild(e, callFuncNode2.Args, false);
			}
			else if (arg0 is TupleNode tupleNode)
			{
				// 元组解构
				HandleTupleBuild(e, tupleNode.Items);
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
				Type declaredType = null;

				if (arg0 is DefineVarNode def)
				{
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

				e.SetResult(value, type);
				e.Context.SetTempVar(varName, value, type, true);
				return;
			}
			else if (arg0 is OperatorNode opNode)
			{
				if (opNode.Name == "." && opNode.Right is VariableNode opRightNode)
				{
					// 属性赋值
					var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type);
					var opLeftValue = opNode.Left.Eval(e.Context, e.Options, e.Control, out _);
					ScriptUtils.SetValue(opLeftValue, opRightNode.Name, value, e.Context.IsIgnoreCase() ?? false);
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
			else if (arg0 is CallFuncNode callFuncNode2 && callFuncNode2.Name == "var")
			{
				// 元组解构
				HandleTuple(e, callFuncNode2.Args, false);
				return;
			}
			else if (arg0 is TupleNode tupleNode0)
			{
				// 元组解构
				HandleTuple(e, tupleNode0.Items);
				return;
			}
		}

		private void HandleTuple(FunctionEvalArgs e, IList<ITreeNode> arg0Items, bool? searchContext = null)
		{
			if (e.Args[1] is TupleNode tupleNode)
			{
				if (arg0Items.Count > tupleNode.Items.Count)
				{
					throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
				}
				var itemValues = new object[arg0Items.Count];
				var itemTypes = new Type[arg0Items.Count];
				for (int i = 0; i < tupleNode.Items.Count; i++)
				{
					var value = tupleNode.Items[i].Eval(e.Context, e.Options, e.Control, out var itemType);
					if (i < arg0Items.Count)
					{
						itemValues[i] = value;
						itemTypes[i] = itemType;
						var varName = ((VariableNode)arg0Items[i]).Name;
						if (varName != "_")
						{
							e.Context.SetTempVar(varName, value, itemType, searchContext ?? !(arg0Items[i] is DefineVarNode));
						}
					}
				}
				// 返回元组
				e.SetResult(TupleNode.CreateTuple(itemValues, itemTypes));
				return;
			}
			var arg1 = e.Args[1].Eval(e.Context, e.Options, e.Control, out _);
			var arg1Type = arg1?.GetType();
			var arg1TypeName = arg1Type?.Name;
			if (arg1TypeName != null && (arg1TypeName.StartsWith("ValueTuple`") || arg1TypeName.StartsWith("Tuple`")))
			{
				bool isValueTuple = arg1TypeName.StartsWith("ValueTuple`");
				int arg1FieldCount = isValueTuple ? arg1Type.GetFields().Length : arg1Type.GetProperties().Length;
				if (arg0Items.Count > arg1FieldCount)
				{
					throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
				}
				var itemValues = arg0Items.Count == arg1FieldCount ? null : new object[arg0Items.Count];
				var itemTypes = arg0Items.Count == arg1FieldCount ? null : new Type[arg0Items.Count];
				for (int i = 0; i < arg0Items.Count; i++)
				{
					var varName = ((VariableNode)arg0Items[i]).Name;
					if (varName == "_" && itemValues == null) continue;
					object value;
					Type itemType;
					if (isValueTuple)
					{
						var info = arg1Type.GetField($"Item{i + 1}");
						value = info.GetValue(arg1);
						itemType = info.FieldType;
					}
					else
					{
						var info = arg1Type.GetProperty($"Item{i + 1}");
						value = info.GetValue(arg1);
						itemType = info.PropertyType;
					}
					if (itemValues != null)
					{
						itemValues[i] = value;
						itemTypes[i] = itemType;
					}
					if (varName != "_")
					{
						e.Context.SetTempVar(varName, value, itemType, searchContext ?? !(arg0Items[i] is DefineVarNode));
					}
				}
				// 返回元组
				e.SetResult(itemValues == null ? arg1 : TupleNode.CreateTuple(itemValues, itemTypes));
				return;
			}

			throw new ScriptAnalyzingException("invalid expression near =");
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

		private void HandleTupleBuild(FunctionBuildArgs e, IList<ITreeNode> arg0Items, bool? searchContext = null)
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
			if (arg0Items.Count > rightFieldCount)
			{
				throw new ScriptAnalyzingException("invalid expression near =, tuple length not matched");
			}

			var expressions = new List<Expression>(arg0Items.Count + 1);
			for (int i = 0; i < arg0Items.Count; i++)
			{
				var arg0Item = arg0Items[i] as VariableNode;
				if (arg0Item.Name == "_" && arg0Items.Count == rightFieldCount) continue;
				var value = isValueTuple ? Expression.Field(right, $"Item{i + 1}") : Expression.Property(right, $"Item{i + 1}");
				expressions.Add(HandleVariableAssign(e, arg0Item, value, searchContext));
			}

			if (arg0Items.Count == rightFieldCount)
			{
				expressions.Add(right);
			}
			else
			{
				expressions.Add(TupleNode.BuildTuple(expressions.ToArray(), expressions.Select(a => a.Type).ToArray()));
			}

			e.Result = Expression.Block(expressions);
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
