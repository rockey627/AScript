using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using AScript.Nodes;

namespace AScript.Operators
{
	public class DotOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly DotOperator Instance = new DotOperator();

		public bool Nullable { get; set; }

		public DotOperator() { }
		public DotOperator(bool nullable)
		{
			this.Nullable = nullable;
		}

		public void Build(FunctionBuildArgs e)
		{
			if (e.Args.Count != 2) return;
			if (e.Args[1] is VariableNode varNode1)
			{
				var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
				var fieldName = varNode1.Name;

				if (typeof(Pointer).IsAssignableFrom(arg0.Type))
				{
					var realType = arg0.Type.GetGenericArguments()[0];
					var addr = Expression.Property(arg0, "Address");
					var obj = Expression.Call(addr, ScriptUtils.Method_Pointer_Address_GetValue, e.BuildContext.GetScriptContextParameter());
					arg0 = Expression.Convert(obj, realType);
				}

				e.Result = GetValue(e, arg0, fieldName);
			}
			else if (e.Args[1] is CallFuncNode funcNode1)
			{
				if (this.Nullable)
				{
					e.Result = BuildNullableCall(e, funcNode1);
				}
				else
				{
					funcNode1.Target = e.Args[0];
					e.Result = funcNode1.Build(e.BuildContext, e.ScriptContext, e.Options);
				}
			}
		}

		/// <summary>
		/// 构建 ?. 方法调用表达式：实例为 null 时结果为 null，否则调用实例方法
		/// </summary>
		private Expression BuildNullableCall(FunctionBuildArgs e, CallFuncNode funcNode1)
		{
			var instance = e.BuildArgs(0);
			if (instance == null) return null;

			// 实例为常量 null 时直接短路，不需要构建调用
			if (instance is ConstantExpression constant && constant.Value == null)
			{
				return ScriptUtils.Constant_null;
			}

			// 值类型实例不可能为 null，不需要判空
			if (instance.Type.IsValueType && System.Nullable.GetUnderlyingType(instance.Type) == null)
			{
				return BuildCall(e, funcNode1, PoolManage.CreateExpressionNode(instance));
			}

			// 使用临时变量保存实例，保证实例只被求值一次
			var tmp = Expression.Variable(instance.Type);
			var call = BuildCall(e, funcNode1, PoolManage.CreateExpressionNode(tmp));
			if (call == null) return null;

			var nullCheck = Expression.Equal(tmp, Expression.Constant(null, tmp.Type));
			if (call.Type == typeof(void))
			{
				return Expression.Block(new[] { tmp },
					Expression.Assign(tmp, instance),
					Expression.Condition(nullCheck, Expression.Empty(), call));
			}
			// 值类型的返回值需要使用 Nullable<> 承载 null
			var resultType = call.Type;
			if (resultType.IsValueType && System.Nullable.GetUnderlyingType(resultType) == null)
			{
				resultType = typeof(Nullable<>).MakeGenericType(resultType);
			}
			return Expression.Block(new[] { tmp },
				Expression.Assign(tmp, instance),
				Expression.Condition(nullCheck, Expression.Constant(null, resultType), Expression.Convert(call, resultType)));
		}

		private static Expression BuildCall(FunctionBuildArgs e, CallFuncNode funcNode1, ITreeNode target)
		{
			// 复制调用节点，避免修改原始节点的 Target
			var callNode = new CallFuncNode
			{
				Name = funcNode1.Name,
				Method = funcNode1.Method,
				Delegate = funcNode1.Delegate,
				Args = funcNode1.Args,
				Target = target
			};
			return callNode.Build(e.BuildContext, e.ScriptContext, e.Options);
		}

		private Expression GetValue(FunctionBuildArgs e, Expression instance, string propertyOrFieldName)
		{
			if (instance.Type == typeof(TypeWrapper))
			{
				// 调用静态类属性或字段
				var wrapper = (TypeWrapper)((ConstantExpression)instance).Value;
				var targetType = wrapper.Type;
				if (e.ScriptContext.IsObjectMemberEnabled(targetType) ?? true)
				{
					var property = targetType.GetProperty(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
					if (property != null) return Expression.Property(null, property);

					var field = targetType.GetField(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
					if (field != null) return Expression.Field(null, field);
				}
				var staticExpr = e.ScriptContext.BuildFunc(e.BuildContext, e.Options, e.Control, $"{wrapper.Name}_get_{propertyOrFieldName}", false, null);
				if (staticExpr != null) return staticExpr;
				throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
			}

			if (typeof(DataRow).IsAssignableFrom(instance.Type))
			{
				return Expression.Property(instance, ScriptUtils.Property_DataRow_Item_String, Expression.Constant(propertyOrFieldName));
			}

			if (typeof(ExpandoObject).IsAssignableFrom(instance.Type))
			{
				var d = Expression.Convert(instance, typeof(IDictionary<string, object>));
				return Expression.Property(d, ScriptUtils.Property_IDictionary_String_Object_Item, Expression.Constant(propertyOrFieldName));
			}

			if (instance.Type == typeof(object))
			{
				return e.ScriptContext.BuildFunc(e.BuildContext, e.Options, e.Control, "<>__GetValue", false, null, new[] { instance, Expression.Constant(propertyOrFieldName), Expression.Constant(this.Nullable) });
			}

			// 变量的属性或字段
			if (e.ScriptContext.IsObjectMemberEnabled(instance.Type) ?? true)
			{
				if (this.Nullable)
				{
					// ?. 判断
					var propOrField = Expression.PropertyOrField(instance, propertyOrFieldName);
					var propType = propOrField.Type;
					// 值类型需要返回 Nullable<>
					if (propType.IsValueType && System.Nullable.GetUnderlyingType(propType) == null)
					{
						propType = typeof(Nullable<>).MakeGenericType(propType);
					}
					var nullCheck = Expression.Equal(instance, Expression.Constant(null, instance.Type));
					return Expression.Condition(nullCheck, Expression.Constant(null, propType), Expression.Convert(propOrField, propType));
				}
				//return Expression.PropertyOrField(instance, propertyOrFieldName);

				//var property = instance.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
				//if (property != null) return Expression.Property(instance, property);

				//var field = instance.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
				//if (field != null) return Expression.Field(instance, field);

				//property = instance.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
				//if (property != null) return Expression.Property(instance, property);

				//field = instance.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
				//if (field != null) return Expression.Field(instance, field);

				var memberExpr = ScriptUtils.PropertyOrField(instance, propertyOrFieldName);
				if (memberExpr != null) return memberExpr;

				var m = instance.Type.GetMethod("dynamic_get");
				if (m != null)
				{
					return Expression.Call(instance, m, Expression.Constant(propertyOrFieldName));
				}

				if (typeof(DynamicObject).IsAssignableFrom(instance.Type))
				{
					return Expression.Property(instance, "Item", Expression.Constant(propertyOrFieldName));
				}
			}

			var expr = e.ScriptContext.BuildFunc(e.BuildContext, e.Options, e.Control, $"get_{propertyOrFieldName}", false, new[] { new ExpressionNode(instance) });
			if (expr != null) return expr;

			throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {instance.Type.Name}.{propertyOrFieldName}");
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (e.Args[1] is VariableNode varNode1)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				if (this.Nullable && arg0 == null)
				{
					e.SetResult(null);
					return;
				}
				var fieldName = varNode1.Name;
				var value = GetValue(e, arg0, fieldName, out var type);
				e.SetResult(value, type);
			}
			else if (e.Args[1] is CallFuncNode funcNode1)
			{
				var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
				if (this.Nullable && arg0 == null)
				{
					e.SetResult(null);
					return;
				}
				funcNode1.Target = arg0;
				e.SetResult(funcNode1.Eval(e.Context, e.Options, e.Control, out var type), type);
			}
		}

		private object GetValue(FunctionEvalArgs e, object instance, string propertyOrFieldName, out Type type)
		{
			object target;
			Type targetType;
			var flags = BindingFlags.Public | BindingFlags.IgnoreCase;
			TypeWrapper wrapper = null;
			if (instance is TypeWrapper w)
			{
				// 静态属性
				target = null;
				targetType = w.Type;
				wrapper = w;
				flags |= BindingFlags.Static;
			}
			else
			{
				// 实例属性
				target = instance;
				targetType = instance.GetType();
				flags |= BindingFlags.Instance;
			}

			if (instance is DataRow dataRow)
			{
				type = dataRow.Table.Columns[propertyOrFieldName].DataType;
				return dataRow[propertyOrFieldName];
			}

			if (instance is ExpandoObject)
			{
				var dict = (IDictionary<string, object>)instance;
				//var value = dict[propertyOrFieldName];
				dict.TryGetValue(propertyOrFieldName, out var value);
				type = value?.GetType();
				return value;
			}

			if (e.Context.IsObjectMemberEnabled(targetType) ?? true)
			{
				var p = targetType.GetProperty(propertyOrFieldName, flags);
				if (p != null)
				{
					type = p.PropertyType;
					return p.GetValue(target);
				}

				var f = targetType.GetField(propertyOrFieldName, flags);
				if (f != null)
				{
					type = f.FieldType;
					return f.GetValue(target);
				}

				var m = targetType.GetMethod("dynamic_get");
				if (m != null)
				{
					type = m.ReturnType;
					return m.Invoke(target, new object[] { propertyOrFieldName });
				}

				if (target is DynamicObject dynamicTarget)
				{
					var itemProperty = targetType.GetProperty("Item", new[] { typeof(string) });
					if (itemProperty == null)
					{
						itemProperty = targetType.GetProperty("Item", new[] { typeof(object) });
					}
					if (itemProperty != null)
					{
						type = itemProperty.PropertyType;
						return itemProperty.GetValue(target, new object[] { propertyOrFieldName });
					}
					//if (ScriptUtils.TryGetMemberDynamicObject(dynamicTarget, propertyOrFieldName, out var result))
					//{
					//	type = result?.GetType() ?? typeof(object);
					//	return result;
					//}
				}
			}

			if (target == null)
			{
				return e.Context.EvalFunc($"{wrapper.Name}_get_{propertyOrFieldName}", null, null, out type);
			}
			return e.Context.EvalFunc($"get_{propertyOrFieldName}", new[] { instance }, new[] { targetType }, out type);
		}
	}
}
