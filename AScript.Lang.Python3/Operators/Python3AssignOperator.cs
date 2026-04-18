using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using AScript.Nodes;

namespace AScript.Lang.Python3.Operators
{
	public class Python3AssignOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly Python3AssignOperator Instance = new Python3AssignOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			var arg0 = e.Args[0];
			if (arg0 is VariableNode v)
			{
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
				//
				ParameterExpression left;
				// 获取变量声明的类型（如果有）
				Type declaredType = null;
				if (arg0 is DefineVarNode defineVar)
				{
					declaredType = defineVar.SystemType ?? e.ScriptContext.EvalType(defineVar.Type);
					if (declaredType != null && (declaredType == typeof(object) || declaredType == typeof(void)))
					{
						declaredType = null;
					}
					left = null;
					e.BuildContext.LocalVariables.Add(v.Name);
				}
				else
				{
					e.BuildContext.TryGetVariableOrParameter(v.Name, out left);
					// 是否在执行上下文中存在变量
					var ownerContext = e.ScriptContext.GetOwnerContext(v.Name, out _, out _);
					if (ownerContext == null)
					{
						e.BuildContext.LocalVariables.Add(v.Name);
					}
				}

				//if (declaredType == null && left != null)
				//{
				//	declaredType = left.Type;
				//}
				if (declaredType == null)
				{
					declaredType = typeof(object);
				}

				// 如果声明了类型，进行类型转换
				Expression rightExpr = right;
				if (right.Type != declaredType)
				{
					rightExpr = Expression.Convert(right, declaredType);
				}

				if (left == null)
				{
					// 定义变量
					//left = Expression.Variable(declaredType ?? right.Type, v.Name);
					left = Expression.Variable(declaredType, v.Name);
					e.BuildContext.Variables[v.Name] = left;
				}
				e.Result = Expression.Assign(left, rightExpr);
			}
			else if (arg0 is OperatorNode opNode && opNode.Name == "[")
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
						e.Result = Expression.Assign(Expression.Property(obj, indexer, idx), valueExpr);
					}
					else
					{
						// 尝试使用set_Item方法
						var setItemMethod = obj.Type.GetMethod("set_Item");
						if (setItemMethod != null)
						{
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
			else
			{
				var left = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var right = e.Args[1].Build(e.BuildContext, e.ScriptContext, e.Options);
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

			if (arg0 is VariableNode || arg0 is DefineVarNode)
			{
				// 获取变量名和声明类型
				string varName;
				Type declaredType = null;
				VariableNode varNode = null;

				if (arg0 is DefineVarNode def)
				{
					varName = def.Name;
					declaredType = def.SystemType ?? e.Context.EvalType(def.Type);
					// 先设置变量类型
					if (declaredType != null && declaredType != typeof(object) && declaredType != typeof(void))
					{
						e.Context.SetTempVar(varName, null, declaredType, false);
					}
				}
				else
				{
					varNode = (VariableNode)arg0;
					varName = varNode.Name;
					//declaredType = e.Context.GetVarType(varName);
				}

				var value = e.Args[1].Eval(e.Context, e.Options, e.Control, out var type);

				// 如果声明了类型，进行类型转换
				if (declaredType != null && declaredType != typeof(object) && declaredType != typeof(void)
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

				e.SetResult(value, type);
				e.Context.SetTempVar(varName, value, type, true);
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
				}
				else if (opNode.Name == "[")
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
					else if (obj is System.Collections.IDictionary dict)
					{
						// Dictionary赋值
						dict[idx] = value;
					}
					else if (obj != null)
					{
						// 其他类型使用动态调用
						dynamic dObj = obj;
						dObj[idx] = value;
					}

					e.SetResult(value, type);
				}
			}
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
	}
}
