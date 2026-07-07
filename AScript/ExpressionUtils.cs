using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using AScript.Nodes;
using System.Linq;
using System.Dynamic;
using System.Data;

namespace AScript
{
	public class ExpressionUtils
	{
		public static readonly Expression Constant_zero = Expression.Constant(0);
		public static readonly Expression Constant_false = Expression.Constant(false);
		public static readonly Expression Constant_true = Expression.Constant(true);
		public static readonly Expression Constant_null = Expression.Constant(null);
		public static readonly Expression Constant_string_empty = Expression.Constant(string.Empty);
		public static readonly Expression Constant_typeof_double = Expression.Constant(typeof(double));

		public static readonly ParameterExpression Parameter_ScriptContext = Expression.Parameter(typeof(ScriptContext));

		public static readonly MethodInfo Method_ScriptUtils_Convert = typeof(ScriptUtils).GetMethod("Convert", new[] { typeof(object), typeof(Type) });
		public static readonly MethodInfo Method_ScriptUtils_SliceAssign = typeof(ScriptUtils).GetMethod("SliceAssign");

		public static readonly MethodInfo Method_ScriptContext_Create1 = typeof(ScriptContext).GetMethod("Create", new Type[] { typeof(bool) });
		public static readonly MethodInfo Method_ScriptContext_Create2 = typeof(ScriptContext).GetMethod("Create", new Type[] { typeof(ScriptContext), typeof(bool) });
		public static readonly MethodInfo Method_ScriptContext_EvalVar = typeof(ScriptContext).GetMethod("EvalVar", new Type[] { typeof(string) });
		public static readonly MethodInfo Method_ScriptContext_SetTempVar = typeof(ScriptContext).GetMethod("SetTempVar", new Type[] { typeof(string), typeof(object), typeof(Type), typeof(bool) });
		public static readonly MethodInfo Method_ScriptContext_EvalFunc_Values = typeof(ScriptContext).GetMethod("EvalFunc", new Type[] { typeof(string), typeof(IList<object>), typeof(IList<Type>) });
		public static readonly MethodInfo Method_ScriptContext_AddTempFunc = typeof(ScriptContext).GetMethod("AddTempFunc", new Type[] { typeof(string), typeof(Delegate) });

		public static readonly MethodInfo Method_ITreeNode_Eval = typeof(ITreeNode).GetMethod("Eval", new Type[] { typeof(ScriptContext), typeof(BuildOptions), typeof(EvalControl), typeof(Type).MakeByRefType() });

		public static readonly MethodInfo Method_LambdaExpression_Compile = typeof(LambdaExpression).GetMethod("Compile", new Type[0]);

		public static readonly MethodInfo Method_Delegate_DynamicInvoke = typeof(Delegate).GetMethod("DynamicInvoke", new Type[] { typeof(object[]) });

		public static readonly MethodInfo Method_String_Concat = typeof(string).GetMethod("Concat", new Type[] { typeof(string) });
		public static readonly MethodInfo Method_String_Concat_object = typeof(string).GetMethod("Concat", new Type[] { typeof(object) });
		public static readonly MethodInfo Method_String_Concat2 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
		public static readonly MethodInfo Method_String_Concat2_object = typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_String_Concat3 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });
		public static readonly MethodInfo Method_String_Concat3_object = typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object), typeof(object) });
		public static readonly MethodInfo Method_String_Concat4 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) });
		public static readonly MethodInfo Method_String_Concat4_object = typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object), typeof(object), typeof(object) });
		public static readonly MethodInfo Method_String_Concat_list = typeof(string).GetMethod("Concat", new Type[] { typeof(IEnumerable<string>) });
		public static readonly MethodInfo Method_String_Concat_list_object = typeof(string).GetMethod("Concat", new Type[] { typeof(IEnumerable<object>) });
		public static readonly MethodInfo Method_String_Concat_array = typeof(string).GetMethod("Concat", new Type[] { typeof(string[]) });
		public static readonly MethodInfo Method_String_Concat_array_object = typeof(string).GetMethod("Concat", new Type[] { typeof(object[]) });
		public static readonly MethodInfo Method_Object_ToString = typeof(object).GetMethod("ToString", new Type[0]);
		public static readonly MethodInfo Method_Object_Equals = typeof(object).GetMethod("Equals", new[] { typeof(object) });

		//public static readonly MethodInfo Method_Type_GetProperty_string = typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string) });

		public static readonly MethodInfo Method_Console_WriteLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) });

		public static readonly MethodInfo Method_Math_Power = typeof(Math).GetMethod("Power", new[] { typeof(double), typeof(double) });
		public static readonly MethodInfo Method_Math_Floor = typeof(Math).GetMethod("Floor", new[] { typeof(double) });

		public static readonly MethodInfo Method_Enumerable_Select1 = typeof(Enumerable).GetMethods().FirstOrDefault(a => a.Name == "Select" && a.GetParameters()[1].ParameterType.Name == "Func`2");
		public static readonly MethodInfo Method_Enumerable_Select2 = typeof(Enumerable).GetMethods().FirstOrDefault(a => a.Name == "Select" && a.GetParameters()[1].ParameterType.Name == "Func`3");
		public static readonly MethodInfo Method_Enumerable_ToArray = typeof(Enumerable).GetMethod("ToArray");
		public static readonly MethodInfo Method_Enumerable_ToList = typeof(Enumerable).GetMethod("ToList");

		public static readonly MethodInfo Method_IDictionary_string_object_Add = typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });

		public static readonly MethodInfo Method_Convert_ChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
		public static readonly MethodInfo Method_Convert_ToBoolean_object = typeof(Convert).GetMethod("ToBoolean", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToByte_object = typeof(Convert).GetMethod("ToByte", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToChar_object = typeof(Convert).GetMethod("ToChar", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToDateTime_object = typeof(Convert).GetMethod("ToDateTime", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToDecimal_object = typeof(Convert).GetMethod("ToDecimal", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToDouble_object = typeof(Convert).GetMethod("ToDouble", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToInt16_object = typeof(Convert).GetMethod("ToInt16", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToInt32_object = typeof(Convert).GetMethod("ToInt32", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToInt64_object = typeof(Convert).GetMethod("ToInt64", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToSByte_object = typeof(Convert).GetMethod("ToSByte", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToSingle_object = typeof(Convert).GetMethod("ToSingle", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToUInt16_object = typeof(Convert).GetMethod("ToUInt16", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToUInt32_object = typeof(Convert).GetMethod("ToUInt32", new[] { typeof(object) });
		public static readonly MethodInfo Method_Convert_ToUInt64_object = typeof(Convert).GetMethod("ToUInt64", new[] { typeof(object) });

		public static readonly PropertyInfo Property_TypeWrapper_Type = typeof(TypeWrapper).GetProperty("Type");

		public static readonly PropertyInfo Property_IDictionary_String_Object_Item = typeof(IDictionary<string, object>).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);

		public static readonly PropertyInfo Property_DataRow_Item_String = typeof(DataRow).GetProperty("Item", new[] { typeof(string) });

		// 相等==
		public static readonly CallSiteBinder Binder_Equal = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Equal, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 不相等!=
		public static readonly CallSiteBinder Binder_NotEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.NotEqual, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 加+
		public static readonly CallSiteBinder Binder_Add = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Add, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 减-
		public static readonly CallSiteBinder Binder_Subtract = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Subtract, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 乘*
		public static readonly CallSiteBinder Binder_Multiply = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Multiply, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 除/
		public static readonly CallSiteBinder Binder_Divide = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Divide, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 取模%
		public static readonly CallSiteBinder Binder_Modulo = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Modulo, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 与&
		public static readonly CallSiteBinder Binder_And = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.And, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 或|
		public static readonly CallSiteBinder Binder_Or = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.Or, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 异或^
		public static readonly CallSiteBinder Binder_XOr = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.ExclusiveOr, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 左移<<
		public static readonly CallSiteBinder Binder_LeftShift = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.LeftShift, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 右移>>
		public static readonly CallSiteBinder Binder_RightShift = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.RightShift, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 非~
		public static readonly CallSiteBinder Binder_Not = Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(
			CSharpBinderFlags.None, ExpressionType.Not, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

		// 大于
		public static readonly CallSiteBinder Binder_GreaterThan = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.GreaterThan, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 小于
		public static readonly CallSiteBinder Binder_LessThan = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.LessThan, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 小于或等于
		public static readonly CallSiteBinder Binder_LessThanOrEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.LessThanOrEqual, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		// 大于或等于
		public static readonly CallSiteBinder Binder_GreaterThanOrEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
			CSharpBinderFlags.None, ExpressionType.GreaterThanOrEqual, null,
			new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

		/// <summary>
		/// 调用node.Eval方法
		/// </summary>
		/// <param name="context"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static Expression BuildEval(BuildContext context, BuildOptions options, EvalControl control, ITreeNode node)
		{
			var returnTypeExpression = Expression.Variable(typeof(Type));
			var instanceExpression = Expression.Constant(node);
			var optionsExpression = Expression.Constant(options ?? Script.DefaultOptions);
			var controlExpression = Expression.Constant(control, typeof(EvalControl));
			var callExpression = Expression.Call(instanceExpression, Method_ITreeNode_Eval, context.GetScriptContextParameter(), optionsExpression, controlExpression, returnTypeExpression);
			return Expression.Block(new[] { returnTypeExpression }, callExpression);
		}

		//public static Expression Build(ExpressionBuildContext context, BuildOptions options, EvalControl control, OperatorNode operatorNode)
		//{
		//	var left = Build(context, operatorNode.Left);
		//	var right = Build(context, operatorNode.Right);
		//	switch (operatorNode.Name)
		//	{
		//		case ";":
		//			context.PrevExpressions.Add(left);
		//			return right;
		//		case "=":
		//			return Expression.Assign(left, right);
		//		case "+":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Add, typeof(object), left, right);
		//			}
		//			return Expression.Add(left, right);
		//		case "-":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Subtract, typeof(object), left, right);
		//			}
		//			return Expression.Subtract(left, right);
		//		case "*":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Multiply, typeof(object), left, right);
		//			}
		//			return Expression.Multiply(left, right);
		//		case "/":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Divide, typeof(object), left, right);
		//			}
		//			return Expression.Divide(left, right);
		//		case "&":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_And, typeof(object), left, right);
		//			}
		//			return Expression.And(left, right);
		//		case "|":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Or, typeof(object), left, right);
		//			}
		//			return Expression.Or(left, right);
		//		case "^":
		//			if (left.Type == typeof(object) || right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_XOr, typeof(object), left, right);
		//			}
		//			return Expression.ExclusiveOr(left, right);
		//		case "~":
		//			if (right.Type == typeof(object))
		//			{
		//				return Expression.Dynamic(Binder_Not, typeof(object), right);
		//			}
		//			return Expression.Not(right);
		//		default:
		//			return BuildEval(context, operatorNode);
		//	}
		//}

		/// <summary>
		/// 调用scriptContext.EvalFunc方法
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="name"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Expression BuildEval(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string name, IList<ITreeNode> args)
		{
			Expression[] argExprs;// = new Expression[args.Count];
			Expression[] argTypes;// = new Expression[args.Count];
			if (args == null || args.Count == 0)
			{
#if NETFRAMEWORK
				argExprs = new Expression[0];
				argTypes = argExprs;
#else
				argExprs = Array.Empty<Expression>();
				argTypes = Array.Empty<Expression>();
#endif
			}
			else
			{
				argExprs = new Expression[args.Count];
				argTypes = new Expression[args.Count];
				for (int i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					if (arg is DefineFuncNode)
					{
						argExprs[i] = Expression.Convert(Expression.Constant(arg), typeof(object));
						argTypes[i] = Expression.Constant(typeof(Delegate));
					}
					else
					{
						var value = arg.Build(buildContext, scriptContext, options);
						argExprs[i] = Expression.Convert(value, typeof(object));
						argTypes[i] = Expression.Constant(value.Type);
					}
				}
			}
			return Expression.Call(buildContext.GetScriptContextParameter(),
				Method_ScriptContext_EvalFunc_Values,
				Expression.Constant(name, typeof(string)),
				Expression.NewArrayInit(typeof(object), argExprs),
				Expression.NewArrayInit(typeof(Type), argTypes));
		}

		/// <summary>
		/// 调用scriptContext.EvalFunc方法
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="name"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Expression BuildEval(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string name, IList<Expression> args)
		{
			var argTypes = new Expression[args.Count];
			for (int i = 0; i < args.Count; i++)
			{
				var arg = args[i];
				argTypes[i] = Expression.Constant(arg.Type);
			}
			return Expression.Call(buildContext.GetScriptContextParameter(),
				Method_ScriptContext_EvalFunc_Values,
				Expression.Constant(name, typeof(string)),
				Expression.NewArrayInit(typeof(object), args),
				Expression.NewArrayInit(typeof(Type), argTypes));
		}

		/// <summary>
		/// 函数调用
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <param name="argTypes"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		public static Delegate CompileEval(ScriptContext context, string name, Type[] argTypes, Type returnType = null)
		{
			int c = argTypes == null ? 0 : argTypes.Length;
			Type funcType0;
			Type implType0;
			if (c == 0)
			{
				funcType0 = typeof(Func<>);
				implType0 = typeof(DelegateImpl<>);
			}
			else if (c == 1)
			{
				funcType0 = typeof(Func<,>);
				implType0 = typeof(DelegateImpl<,>);
			}
			else if (c == 2)
			{
				funcType0 = typeof(Func<,,>);
				implType0 = typeof(DelegateImpl<,,>);
			}
			else if (c == 3)
			{
				funcType0 = typeof(Func<,,,>);
				implType0 = typeof(DelegateImpl<,,,>);
			}
			else if (c == 4)
			{
				funcType0 = typeof(Func<,,,,>);
				implType0 = typeof(DelegateImpl<,,,,>);
			}
			else if (c == 5)
			{
				funcType0 = typeof(Func<,,,,,>);
				implType0 = typeof(DelegateImpl<,,,,,>);
			}
			else if (c == 6)
			{
				funcType0 = typeof(Func<,,,,,,>);
				implType0 = typeof(DelegateImpl<,,,,,,>);
			}
			else if (c == 7)
			{
				funcType0 = typeof(Func<,,,,,,,>);
				implType0 = typeof(DelegateImpl<,,,,,,,>);
			}
			else return null;
			var genTypes = new Type[c + 1];
			Array.Copy(argTypes, genTypes, argTypes.Length);
			genTypes[genTypes.Length - 1] = returnType ?? typeof(object);
			Type funcType = funcType0.MakeGenericType(genTypes);
			Type implType = implType0.MakeGenericType(genTypes);
			return Delegate.CreateDelegate(funcType, Activator.CreateInstance(implType, context, name), "Execute");
		}

		public static bool ConvertMaxType(ref Expression expr1, ref Expression expr2)
		{
			//if (expr1.Type == expr2.Type) return true;
			//if (expr1.Type == typeof(string))
			//{
			//	expr2 = Expression.Convert(expr2, typeof(string));
			//	return true;
			//}
			//if (expr2.Type == typeof(string))
			//{
			//	expr1 = Expression.Convert(expr1, typeof(string));
			//	return true;
			//}
			if (expr1.Type == typeof(object) && expr2.Type == typeof(string))
			{
				expr1 = Expression.Call(expr1, Method_Object_ToString);
				return true;
			}
			if (expr1.Type == typeof(string) && expr2.Type == typeof(object))
			{
				expr2 = Expression.Call(expr2, Method_Object_ToString);
				return true;
			}
			var type = ScriptUtils.GetMaxType(expr1.Type, expr2.Type);
			if (type == null) return false;
			if (expr1.Type != type)
			{
				if (type == typeof(string))
				{
					expr1 = Expression.Call(expr1, Method_Object_ToString);
				}
				else
				{
					expr1 = Expression.Convert(expr1, type);
				}
			}
			else if (expr2.Type != type)
			{
				if (type == typeof(string))
				{
					expr2 = Expression.Call(expr2, Method_Object_ToString);
				}
				else
				{
					expr2 = Expression.Convert(expr2, type);
				}
			}
			return true;
		}

		public static Expression GetValue(Expression instance, string propertyOrFieldName, bool nullable = false, Expression defaultValue = null)
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
				if (field != null)
				{
					return Expression.Field(null, field);
				}

				return defaultValue;
			}

			if (typeof(DataRow).IsAssignableFrom(instance.Type))
			{
				// 检查列是否存在
				var table = Expression.Property(instance, "Table");
				var cols = Expression.Property(table, "Columns");
				var containsMethod = typeof(DataColumnCollection).GetMethod("Contains", new[] { typeof(string) });
				var colExists = Expression.Call(cols, containsMethod, Expression.Constant(propertyOrFieldName));
				var item = Expression.Property(instance, Property_DataRow_Item_String, Expression.Constant(propertyOrFieldName));
				if (defaultValue == null)
				{
					defaultValue = Expression.Constant(DBNull.Value);
				}
				return Expression.Condition(colExists, item, defaultValue);
			}

			if (typeof(ExpandoObject).IsAssignableFrom(instance.Type))
			{
				var d = Expression.Convert(instance, typeof(IDictionary<string, object>));
				// 检查键是否存在
				var containsKeyMethod = typeof(IDictionary<string, object>).GetMethod("ContainsKey");
				var keyExists = Expression.Call(d, containsKeyMethod, Expression.Constant(propertyOrFieldName));
				var item = Expression.Property(d, Property_IDictionary_String_Object_Item, Expression.Constant(propertyOrFieldName));
				if (defaultValue == null)
				{
					defaultValue = Expression.Constant(null, typeof(object));
				}
				else if (defaultValue.Type != item.Type)
				{
					defaultValue = Expression.Convert(defaultValue, item.Type);
				}
				return Expression.Condition(keyExists, item, defaultValue);
			}

			// 变量的属性或字段 - 先检查是否存在
			if (defaultValue != null)
			{
				var prop = instance.Type.GetProperty(propertyOrFieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (prop == null)
				{
					var fld = instance.Type.GetField(propertyOrFieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (fld == null) return defaultValue;
				}
			}

			// 变量的属性或字段
			if (nullable)
			{
				// ?. 判断
				var propOrField = Expression.PropertyOrField(instance, propertyOrFieldName);
				var propType = propOrField.Type;
				// 值类型需要返回 Nullable<>
				if (propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
				{
					propType = typeof(Nullable<>).MakeGenericType(propType);
				}
				var nullCheck = Expression.Equal(instance, Expression.Constant(null, instance.Type));
				return Expression.Condition(nullCheck, Expression.Constant(null, propType), Expression.Convert(propOrField, propType));
			}

			// 当 instance.Type == typeof(object) 时，可能是 JavaScript 对象（IDictionary<string, object>）
			// 也可能是普通对象，使用动态绑定来处理不同运行时类型的属性访问
			if (instance.Type == typeof(object))
			{
				var getMemberBinder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
					CSharpBinderFlags.None, propertyOrFieldName, typeof(object),
					new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
				return Expression.Dynamic(getMemberBinder, typeof(object), instance);
			}

			return Expression.PropertyOrField(instance, propertyOrFieldName);
		}

		public static Expression SetValue(Expression instance, string propertyOrFieldName, Expression value)
		{
			if (instance.Type == typeof(TypeWrapper))
			{
				// 调用静态类属性或字段
				var targetType = ((TypeWrapper)((ConstantExpression)instance).Value).Type;
				var property = targetType.GetProperty(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					return Expression.Assign(Expression.Property(null, property), value);
				}

				var field = targetType.GetField(propertyOrFieldName, BindingFlags.Static | BindingFlags.Public);
				return Expression.Assign(Expression.Field(null, field), value);
			}

			if (typeof(DataRow).IsAssignableFrom(instance.Type))
			{
				var pi = Expression.Property(instance, Property_DataRow_Item_String, Expression.Constant(propertyOrFieldName));
				return Expression.Assign(pi, value);
			}

			// 变量的属性或字段
			return Expression.Assign(Expression.PropertyOrField(instance, propertyOrFieldName), value);
		}

		public static Expression Convert(Expression v, Type type)
		{
			if (v.Type == type || type.IsAssignableFrom(v.Type)) return v;

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToBoolean_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToBoolean", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToBoolean_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Byte:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToByte_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToByte", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToByte_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Char:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToChar_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToChar", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToChar_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.DateTime:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToDateTime_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToDateTime", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToDateTime_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.DBNull:
					return Expression.Constant(DBNull.Value);
				case TypeCode.Decimal:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToDecimal_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToDecimal", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToDecimal_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Double:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToDouble_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToDouble", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToDouble_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Empty:
					return v;
				case TypeCode.Int16:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToInt16_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToInt16", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToInt16_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Int32:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToInt32_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToInt32", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToInt32_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Int64:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToInt64_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToInt64", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToInt64_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Object:
					return Expression.Convert(v, type);
				case TypeCode.SByte:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToSByte_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToSByte", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToSByte_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.Single:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToSingle_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToSingle", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToSingle_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.String:
					return Expression.Call(v, Method_Object_ToString);
				case TypeCode.UInt16:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToUInt16_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToUInt16", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToUInt16_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.UInt32:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToUInt32_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToUInt32", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToUInt32_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				case TypeCode.UInt64:
					{
						MethodInfo method;
						if (v.Type == typeof(object))
						{
							method = Method_Convert_ToUInt64_object;
						}
						else
						{
							method = typeof(Convert).GetMethod("ToUInt64", new[] { v.Type });
							if (method == null)
							{
								method = Method_Convert_ToUInt64_object;
								v = Expression.Convert(v, typeof(object));
							}
						}
						return Expression.Call(method, v);
					}
				default:
					return Expression.Convert(v, type);
			}
		}

		public static Expression ConsoleWriteLine(string value)
		{
			return ConsoleWriteLine(Expression.Constant(value));
		}

		public static Expression ConsoleWriteLine(Expression value)
		{
			if (value.Type != typeof(object))
			{
				value = Expression.Convert(value, typeof(object));
			}
			return Expression.Call(Method_Console_WriteLine, value);
		}

		public static MemberExpression PropertyOrField(Expression instance, string propertyOrFieldName)
		{
			var property = instance.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
			if (property != null) return Expression.Property(instance, property);

			var field = instance.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
			if (field != null) return Expression.Field(instance, field);

			property = instance.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
			if (property != null) return Expression.Property(instance, property);

			field = instance.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
			if (field != null) return Expression.Field(instance, field);

			return null;
		}

		private class DelegateImplBase
		{
			private readonly ScriptContext _context;
			private readonly string _name;

			public DelegateImplBase(ScriptContext context, string name)
			{
				_context = context;
				_name = name;
			}

			public object Execute(object[] argValues, Type[] argTypes)
			{
				return _context.EvalFunc(_name, argValues, argTypes);
			}
		}

		private class DelegateImplBase<TReturn> : DelegateImplBase
		{
			public DelegateImplBase(ScriptContext context, string name) : base(context, name)
			{
			}

			public new TReturn Execute(object[] args, Type[] argTypes)
			{
				return (TReturn)base.Execute(args, argTypes);
			}
		}

		private class DelegateImpl<TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute()
			{
				return base.Execute(null, null);
			}
		}

		private class DelegateImpl<T1, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1)
			{
				return base.Execute(new object[] { arg1 }, new Type[] { typeof(T1) });
			}
		}

		private class DelegateImpl<T1, T2, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2)
			{
				return base.Execute(new object[] { arg1, arg2 }, new Type[] { typeof(T1), typeof(T2) });
			}
		}

		private class DelegateImpl<T1, T2, T3, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2, T3 arg3)
			{
				return base.Execute(new object[] { arg1, arg2, arg3 }, new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			}
		}

		private class DelegateImpl<T1, T2, T3, T4, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			{
				return base.Execute(new object[] { arg1, arg2, arg3, arg4 }, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			}
		}

		private class DelegateImpl<T1, T2, T3, T4, T5, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			{
				return base.Execute(new object[] { arg1, arg2, arg3, arg4, arg5 }, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
			}
		}

		private class DelegateImpl<T1, T2, T3, T4, T5, T6, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
			{
				return base.Execute(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
			}
		}

		private class DelegateImpl<T1, T2, T3, T4, T5, T6, T7, TReturn> : DelegateImplBase<TReturn>
		{
			public DelegateImpl(ScriptContext context, string name) : base(context, name)
			{
			}

			public TReturn Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
			{
				return base.Execute(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
			}
		}
	}
}
