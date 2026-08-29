using AScript.Values;
using System;
using System.Linq.Expressions;
using System.Reflection;
//using System.Runtime.CompilerServices;
//using Microsoft.CSharp.RuntimeBinder;

namespace AScript
{
	public class ExpressionUtils
	{
		[Obsolete]
		public static readonly Expression Constant_null = ScriptUtils.Constant_null;
		[Obsolete]
		public static readonly Expression Constant_false = ScriptUtils.Constant_false;
		[Obsolete]
		public static readonly Expression Constant_string_empty = ScriptUtils.Constant_string_empty;
		[Obsolete]
		public static readonly Expression Constant_typeof_double = ScriptUtils.Constant_typeof_double;
		[Obsolete]
		public static readonly MethodInfo Method_ScriptUtils_Convert = ScriptUtils.Method_ScriptUtils_Convert;
		[Obsolete]
		public static readonly MethodInfo Method_ScriptUtils_IsIntegerType = ScriptUtils.Method_ScriptUtils_IsIntegerType;
		[Obsolete]
		public static readonly MethodInfo Method_Math_Floor = ScriptUtils.Method_Math_Floor;
		[Obsolete]
		public static readonly MethodInfo Method_String_Concat_list = ScriptUtils.Method_String_Concat_list;
		[Obsolete]
		public static readonly MethodInfo Method_Object_ToString = ScriptUtils.Method_Object_ToString;
		[Obsolete]
		public static readonly MethodInfo Method_Object_GetType = ScriptUtils.Method_Object_GetType;
		[Obsolete]
		public static readonly PropertyInfo Property_DataRow_Item_String = ScriptUtils.Property_DataRow_Item_String;

		public static readonly MethodInfo Method_Add = typeof(ExpressionUtils).GetMethod("Add", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_Subtract = typeof(ExpressionUtils).GetMethod("Subtract", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_Divide = typeof(ExpressionUtils).GetMethod("Divide", new[] { typeof(object), typeof(object), typeof(bool) });
		public static readonly MethodInfo Method_Multiply = typeof(ExpressionUtils).GetMethod("Multiply", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_Modulo = typeof(ExpressionUtils).GetMethod("Modulo", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_And = typeof(ExpressionUtils).GetMethod("And", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_Or = typeof(ExpressionUtils).GetMethod("Or", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_XOr = typeof(ExpressionUtils).GetMethod("XOr", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_Not = typeof(ExpressionUtils).GetMethod("Not", new[] { typeof(object) });
		public static readonly MethodInfo Method_Equal = typeof(ExpressionUtils).GetMethod("Equal", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_NotEqual = typeof(ExpressionUtils).GetMethod("NotEqual", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_GreaterThan = typeof(ExpressionUtils).GetMethod("GreaterThan", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_GreaterThanOrEqual = typeof(ExpressionUtils).GetMethod("GreaterThanOrEqual", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_LessThan = typeof(ExpressionUtils).GetMethod("LessThan", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_LessThanOrEqual = typeof(ExpressionUtils).GetMethod("LessThanOrEqual", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_LeftShift = typeof(ExpressionUtils).GetMethod("LeftShift", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_RightShift = typeof(ExpressionUtils).GetMethod("RightShift", new[] { typeof(object), typeof(object) });
		public static readonly MethodInfo Method_setItem = typeof(ExpressionUtils).GetMethod("setItem", new[] { typeof(object), typeof(object), typeof(object) });

		public static void setItem(object obj, object index, object value)
		{
			if (obj is AValue ov) obj = ov.Get();
			if (index is AValue iv) index = iv.Get();
			if (value is AValue vv) value = vv.Get();
			((dynamic)obj)[(dynamic)index] = (dynamic)value;
		}

		public static Expression setItem(Expression obj, Expression index, Expression value)
		{
			return Expression.Call(Method_setItem, obj, index, value);
		}

		public static object Add(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (v1 == null) return v2;
			if (v2 == null) return v1;
			return ((dynamic)v1) + ((dynamic)v2);
		}

		public static object Subtract(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (v1 == null)
			{
				if (v2 == null) return null;
				v1 = ScriptUtils.GetDefaultValue(v2.GetType());
			}
			if (v2 == null)
			{
				v2 = ScriptUtils.GetDefaultValue(v1.GetType());
			}
			return ((dynamic)v1) - ((dynamic)v2);
		}

		public static object Divide(object v1, object v2, bool isDouble)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (isDouble)
			{
				if (v1 != null && ScriptUtils.IsNumberType(v1.GetType())
					&& v2 != null && ScriptUtils.IsNumberType(v2.GetType()))
				{
					double d1 = Convert.ToDouble(v1);
					double d2 = Convert.ToDouble(v2);
					return d1 / d2;
				}
			}
			return ((dynamic)v1) / ((dynamic)v2);
		}

		public static object Multiply(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (v1 == null)
			{
				if (v2 == null) return null;
				v1 = ScriptUtils.GetDefaultValue(v2.GetType());
			}
			if (v2 == null)
			{
				v2 = ScriptUtils.GetDefaultValue(v1.GetType());
			}
			return ((dynamic)v1) * ((dynamic)v2);
		}

		public static object Modulo(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) % ((dynamic)v2);
		}

		public static object And(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) & ((dynamic)v2);
		}

		public static object Or(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) | ((dynamic)v2);
		}

		public static object XOr(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) ^ ((dynamic)v2);
		}

		public static object Not(object v1)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			return !((dynamic)v1);
		}

		public static bool Equal(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (v1 == null) return v2 == null;
			if (v2 == null) return false;
#if !NET45
			if (v1.GetType().Name.StartsWith("ValueTuple`"))
			{
				return v1.Equals(v2);
			}
			if (v2.GetType().Name.StartsWith("ValueTuple`"))
			{
				return false;
			}
#endif
			return ((dynamic)v1) == ((dynamic)v2);
		}

		public static bool NotEqual(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			if (v1 == null) return v2 != null;
			if (v2 == null) return true;
#if !NET45
			if (v1.GetType().Name.StartsWith("ValueTuple`"))
			{
				return !v1.Equals(v2);
			}
			if (v2.GetType().Name.StartsWith("ValueTuple`"))
			{
				return true;
			}
#endif
			return ((dynamic)v1) != ((dynamic)v2);
		}

		public static bool GreaterThan(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) > ((dynamic)v2);
		}

		public static bool GreaterThanOrEqual(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) >= ((dynamic)v2);
		}

		public static bool LessThan(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) < ((dynamic)v2);
		}

		public static bool LessThanOrEqual(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) <= ((dynamic)v2);
		}

		public static object LeftShift(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) << ((dynamic)v2);
		}

		public static object RightShift(object v1, object v2)
		{
			if (v1 is AValue a1) v1 = a1.Get();
			if (v2 is AValue a2) v2 = a2.Get();
			return ((dynamic)v1) >> ((dynamic)v2);
		}

		public static Expression LessThan(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_LessThan, v1, v2);
			}
			return Expression.LessThan(v1, v2);
		}

		public static Expression LessThanOrEqual(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_LessThanOrEqual, v1, v2);
			}
			return Expression.LessThanOrEqual(v1, v2);
		}

		public static Expression LeftShift(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_LeftShift, v1, v2);
			}
			return Expression.LeftShift(v1, v2);
		}

		public static Expression RightShift(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_RightShift, v1, v2);
			}
			return Expression.RightShift(v1, v2);
		}

		public static Expression GreaterThanOrEqual(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_GreaterThanOrEqual, v1, v2);
			}
			return Expression.GreaterThanOrEqual(v1, v2);
		}

		public static Expression GreaterThan(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_GreaterThan, v1, v2);
			}
			return Expression.GreaterThan(v1, v2);
		}

		public static Expression Add(Expression v1, Expression v2)
		{
			if (!ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Add, v1, v2);
			}
			if (v2.Type == typeof(string) && v2.Type == typeof(string))
			{
				// 字符串相加使用string.Concat方法
				return Expression.Call(null, ScriptUtils.Method_String_Concat2, v1, v2);
			}
			return Expression.Add(v1, v2);
		}

		public static Expression Subtract(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Subtract, v1, v2);
			}
			return Expression.Subtract(v1, v2);
		}

		public static Expression Divide(Expression v1, Expression v2, bool isDouble)
		{
			if (ScriptUtils.IsNumberType(v1.Type) && ScriptUtils.IsNumberType(v2.Type))
			{
				if (isDouble)
				{
					if (v1.Type != typeof(double)) v1 = Expression.Convert(v1, typeof(double));
					if (v2.Type != typeof(double)) v2 = Expression.Convert(v2, typeof(double));
				}
				if (v1.Type == typeof(object) || v2.Type == typeof(object)
					|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
				{
					if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
					if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
					return Expression.Call(Method_Divide, v1, v2, Expression.Constant(isDouble));
				}
				return Expression.Divide(v1, v2);
			}
			return Expression.Call(Method_Divide, v1, v2, Expression.Constant(isDouble));
		}

		public static Expression Multiply(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Multiply, v1, v2);
			}
			return Expression.Multiply(v1, v2);
		}

		public static Expression Modulo(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Modulo, v1, v2);
			}
			return Expression.Modulo(v1, v2);
		}

		public static Expression And(Expression v1, Expression v2)
		{
			if (!ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_And, v1, v2);
			}
			return Expression.And(v1, v2);
		}

		public static Expression Or(Expression v1, Expression v2)
		{
			if (!ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Or, v1, v2);
			}
			return Expression.Or(v1, v2);
		}

		public static Expression XOr(Expression v1, Expression v2)
		{
			if (!ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_XOr, v1, v2);
			}
			return Expression.ExclusiveOr(v1, v2);
		}

		public static Expression Not(Expression v1)
		{
			if (v1.Type == typeof(object))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				return Expression.Call(Method_Not, v1);
			}
			return Expression.Not(v1);
		}

		public static Expression Equal(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| v1.Type != v2.Type && !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_Equal, v1, v2);
			}
#if !NET45
			if (v1.Type.Name.StartsWith("ValueTuple`") && v1.Type == v2.Type)
			{
				return Expression.Call(v1, v1.Type.GetMethod("Equals", new[] { v1.Type }), v2);
			}
#endif
			return Expression.Equal(v1, v2);
		}

		public static Expression NotEqual(Expression v1, Expression v2)
		{
			if (v1.Type == typeof(object) || v2.Type == typeof(object)
				|| v1.Type != v2.Type && !ScriptUtils.ConvertMaxType(ref v1, ref v2))
			{
				if (v1.Type.IsValueType) v1 = Expression.Convert(v1, typeof(object));
				if (v2.Type.IsValueType) v2 = Expression.Convert(v2, typeof(object));
				return Expression.Call(Method_NotEqual, v1, v2);
			}
#if !NET45
			if (v1.Type.Name.StartsWith("ValueTuple`") && v1.Type == v2.Type)
			{
				return Expression.Not(Expression.Call(v1, v1.Type.GetMethod("Equals", new[] { v1.Type }), v2));
			}
#endif
			return Expression.NotEqual(v1, v2);
		}

		public static Expression PlusAssign(Expression left, Expression right, Type leftRealType = null)
		{
			Expression leftExpr = left;
			Expression rightExpr = right;
			Expression result;
			if (leftRealType != null && leftRealType != typeof(object))
			{
				leftExpr = Expression.Convert(leftExpr, leftRealType);
			}
			if (!ScriptUtils.ConvertMaxType(ref leftExpr, ref rightExpr))
			{
				// dynamic方式作用+=无效
				//e.Result = Expression.Dynamic(ExpressionUtils.Binder_AddAssign, typeof(object), left, right);
				//result = Expression.Dynamic(Binder_Add, typeof(object), leftExpr, rightExpr);
				//result = Add(leftExpr, rightExpr);
				if (leftExpr.Type.IsValueType) leftExpr = Expression.Convert(leftExpr, typeof(object));
				if (rightExpr.Type.IsValueType) rightExpr = Expression.Convert(rightExpr, typeof(object));
				result = Expression.Call(Method_Add, leftExpr, rightExpr);
			}
			else if (leftExpr.Type == typeof(string))
			{
				// 字符串相加使用string.Concat方法
				if (rightExpr.Type == typeof(string))
				{
					result = Expression.Call(ScriptUtils.Method_String_Concat2, leftExpr, rightExpr);
				}
				else
				{
					if (rightExpr.Type != typeof(object))
					{
						rightExpr = Expression.Convert(rightExpr, typeof(object));
					}
					result = Expression.Call(ScriptUtils.Method_String_Concat2_object, leftExpr, rightExpr);
				}
			}
			else if (left.Type == typeof(object))
			{
				result = Expression.Add(leftExpr, rightExpr);
			}
			else
			{
				return Expression.AddAssign(left, rightExpr);
			}

			if (result.Type != left.Type)
			{
				return Expression.Assign(left, Expression.Convert(result, left.Type));
			}
			else
			{
				return Expression.Assign(left, result);
			}
		}

		public static Expression SubtractAssign(Expression left, Expression right, Type leftRealType = null)
		{
			Expression leftExpr = left;
			Expression rightExpr = right;
			Expression result;
			if (leftRealType != null && leftRealType != typeof(object))
			{
				leftExpr = Expression.Convert(leftExpr, leftRealType);
			}
			if (!ScriptUtils.ConvertMaxType(ref leftExpr, ref rightExpr))
			{
				//result = Expression.Dynamic(Binder_Subtract, typeof(object), leftExpr, rightExpr);
				//result = Subtract(leftExpr, rightExpr);
				if (leftExpr.Type.IsValueType) leftExpr = Expression.Convert(leftExpr, typeof(object));
				if (rightExpr.Type.IsValueType) rightExpr = Expression.Convert(rightExpr, typeof(object));
				result = Expression.Call(Method_Subtract, leftExpr, rightExpr);
			}
			else if (left.Type == typeof(object))
			{
				result = Expression.Subtract(leftExpr, rightExpr);
			}
			else
			{
				return Expression.SubtractAssign(left, rightExpr);
			}

			if (result.Type != left.Type)
			{
				return Expression.Assign(left, Expression.Convert(result, left.Type));
			}
			else
			{
				return Expression.Assign(left, result);
			}
		}

		//// 相等==
		//public static readonly CallSiteBinder Binder_Equal = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Equal, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 不相等!=
		//public static readonly CallSiteBinder Binder_NotEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.NotEqual, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 加+
		//public static readonly CallSiteBinder Binder_Add = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Add, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 减-
		//public static readonly CallSiteBinder Binder_Subtract = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Subtract, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 乘*
		//public static readonly CallSiteBinder Binder_Multiply = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Multiply, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 除/
		//public static readonly CallSiteBinder Binder_Divide = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Divide, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 取模%
		//public static readonly CallSiteBinder Binder_Modulo = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Modulo, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 与&
		//public static readonly CallSiteBinder Binder_And = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.And, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 或|
		//public static readonly CallSiteBinder Binder_Or = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Or, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 异或^
		//public static readonly CallSiteBinder Binder_XOr = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.ExclusiveOr, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 左移<<
		//public static readonly CallSiteBinder Binder_LeftShift = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.LeftShift, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 右移>>
		//public static readonly CallSiteBinder Binder_RightShift = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.RightShift, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 非~
		//public static readonly CallSiteBinder Binder_Not = Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.Not, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

		//// 大于
		//public static readonly CallSiteBinder Binder_GreaterThan = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.GreaterThan, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 小于
		//public static readonly CallSiteBinder Binder_LessThan = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.LessThan, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 小于或等于
		//public static readonly CallSiteBinder Binder_LessThanOrEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.LessThanOrEqual, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
		//// 大于或等于
		//public static readonly CallSiteBinder Binder_GreaterThanOrEqual = Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(
		//	CSharpBinderFlags.None, ExpressionType.GreaterThanOrEqual, null,
		//	new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

	}
}
