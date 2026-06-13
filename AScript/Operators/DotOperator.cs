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
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Build(e.BuildContext, e.ScriptContext, e.Options);
			var fieldName = ((VariableNode)e.Args[1]).Name;
			e.Result = GetValue(e, arg0, fieldName, this.Nullable);
		}

		private static Expression GetValue(FunctionBuildArgs e, Expression instance, string propertyOrFieldName, bool nullable = false)
		{
			if (instance.Type == typeof(TypeWrapper))
			{
				// 调用静态类属性或字段
				var targetType = ((TypeWrapper)((ConstantExpression)instance).Value).Type;
				var property = targetType.GetProperty(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					return Expression.Property(null, property);
				}

				var field = targetType.GetField(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public);
				return Expression.Field(null, field);
			}

			if (typeof(DataRow).IsAssignableFrom(instance.Type))
			{
				return Expression.Property(instance, ExpressionUtils.Property_DataRow_Item_String, Expression.Constant(propertyOrFieldName));
			}

			if (typeof(ExpandoObject).IsAssignableFrom(instance.Type))
			{
				var d = Expression.Convert(instance, typeof(IDictionary<string, object>));
				return Expression.Property(d, ExpressionUtils.Property_IDictionary_String_Object_Item, Expression.Constant(propertyOrFieldName));
			}

			// 变量的属性或字段
			if (e.ScriptContext.IsObjectPropertyEnabled(instance.Type))
			{
				if (nullable)
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
				return Expression.PropertyOrField(instance, propertyOrFieldName);
			}

			return e.ScriptContext.BuildFunc(e.BuildContext, e.Options, e.Control, $"get_{propertyOrFieldName}", false, new[] { new ExpressionNode(instance) });
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.Args.Count != 2) return;
			if (!(e.Args[1] is VariableNode)) return;

			var arg0 = e.Args[0].Eval(e.Context, e.Options, e.Control, out var type0);
			if (this.Nullable && arg0 == null)
			{
				e.SetResult(null);
				return;
			}
			var fieldName = ((VariableNode)e.Args[1]).Name;
			var value = GetValue(e, arg0, fieldName, out var type);
			e.SetResult(value, type);
		}

		private static object GetValue(FunctionEvalArgs e, object instance, string propertyOrFieldName, out Type type)
		{
			object target;
			Type targetType;
			var flags = BindingFlags.Public | BindingFlags.IgnoreCase;
			if (instance is TypeWrapper w)
			{
				// 静态属性
				target = null;
				targetType = w.Type;
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

			if (e.Context.IsObjectPropertyEnabled(targetType))
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

				throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
			}

			return e.Context.EvalFunc($"get_{propertyOrFieldName}", new[] { instance }, new[] { targetType }, out type);
		}
	}
}
