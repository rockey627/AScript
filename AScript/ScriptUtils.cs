using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AScript
{
	public class ScriptUtils
	{
		private static readonly ConcurrentDictionary<Type, int> _TypeSize = new ConcurrentDictionary<Type, int>
		{
			[typeof(byte)] = 10,
			[typeof(ushort)] = 20,
			[typeof(short)] = 30,
			[typeof(uint)] = 40,
			[typeof(int)] = 50,
			[typeof(long)] = 60,
			[typeof(ulong)] = 70,
			[typeof(float)] = 80,
			[typeof(decimal)] = 90,
			[typeof(double)] = 100,
			[typeof(string)] = 500,
		};

		public static bool IsIntegerType(Type type)
		{
			if (type == null) return false;
			return _TypeSize.TryGetValue(type, out var size) && size <= 70;
		}

		public static bool IsFloatType(Type type)
		{
			if (type == null) return false;
			return _TypeSize.TryGetValue(type, out var size) && size > 70 && size < 500;
		}

		public static bool IsNumberType(Type type)
		{
			if (type == null) return false;
			return _TypeSize.TryGetValue(type, out var size) && size < 500;
		}

		public static object Convert(object v, Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return System.Convert.ToBoolean(v);
				case TypeCode.Byte:
					return System.Convert.ToByte(v);
				case TypeCode.Char:
					return System.Convert.ToChar(v);
				case TypeCode.DateTime:
					return System.Convert.ToDateTime(v);
				case TypeCode.DBNull:
					return System.Convert.DBNull;
				case TypeCode.Decimal:
					return System.Convert.ToDecimal(v);
				case TypeCode.Double:
					return System.Convert.ToDouble(v);
				case TypeCode.Empty:
					return v;
				case TypeCode.Int16:
					return System.Convert.ToInt16(v);
				case TypeCode.Int32:
					return System.Convert.ToInt32(v);
				case TypeCode.Int64:
					return System.Convert.ToInt64(v);
				case TypeCode.Object:
					return v;
				case TypeCode.SByte:
					return System.Convert.ToSByte(v);
				case TypeCode.Single:
					return System.Convert.ToSingle(v);
				case TypeCode.String:
					return System.Convert.ToString(v);
				case TypeCode.UInt16:
					return System.Convert.ToUInt16(v);
				case TypeCode.UInt32:
					return System.Convert.ToUInt32(v);
				case TypeCode.UInt64:
					return System.Convert.ToUInt64(v);
				default:
					return v;
			}
		}

		public static bool IsMatchArgType(Type inType, Type defineType)
		{
			if (inType == null) return true;
			if (defineType == null) return true;
			if (inType == defineType) return true;
			if (IsNumberType(defineType)) return IsNumberType(inType);
			if (defineType.IsClass) return inType.IsSubclassOf(defineType);
			if (defineType.IsInterface) return inType.GetInterfaces().Contains(defineType);
			return false;
		}

		public static bool IsMatchArgTypes(IList<Type> inArgTypes, IList<Type> defineTypes)
		{
			return IsMatchArgTypes(inArgTypes, defineTypes, 0);
		}

		public static bool IsMatchArgTypes(IList<Type> inArgTypes, IList<Type> defineTypes, int defineStartIndex)
		{
			if (defineTypes == null || defineTypes.Count == 0)
			{
				return inArgTypes == null || inArgTypes.Count == 0;
			}
			if (inArgTypes == null || inArgTypes.Count == 0) return defineStartIndex == defineTypes.Count;
			if (inArgTypes.Count != defineTypes.Count - defineStartIndex) return false;
			for (int i = 0; i < inArgTypes.Count; i++)
			{
				if (!IsMatchArgType(inArgTypes[i], defineTypes[i + defineStartIndex])) return false;
			}
			return true;
		}

		public static object GetDefaultValue(Type targetType)
		{
			if (targetType == null) return null;
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
		}

		public static Type GetMaxType(Type type1, Type type2)
		{
			if (!_TypeSize.TryGetValue(type1, out var v1))
			{
				return null;
			}
			if (!_TypeSize.TryGetValue(type2, out var v2))
			{
				return null;
			}
			return v1 > v2 ? type1 : type2;
		}

		public static Type GetElementType(Type collectionType)
		{
			// 处理数组类型
			if (collectionType.IsArray)
			{
				return collectionType.GetElementType();
			}

			// 处理泛型集合（List<T>, Collection<T> 等）
			if (collectionType.IsGenericType)
			{
				var genericTypeDefinition = collectionType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(IEnumerable<>)
					|| genericTypeDefinition == typeof(ICollection<>)
					|| genericTypeDefinition == typeof(IList<>)
					|| genericTypeDefinition == typeof(List<>))
				{
					return collectionType.GetGenericArguments()[0];
				}
			}

			// 处理实现了 IEnumerable<T> 的类型
			var enumerableInterface = collectionType.GetInterfaces()
				.FirstOrDefault(i => i.IsGenericType
								  && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (enumerableInterface != null)
			{
				return enumerableInterface.GetGenericArguments()[0];
			}

			// 处理非泛型集合（如 ArrayList）
			if (typeof(IEnumerable).IsAssignableFrom(collectionType))
			{
				return typeof(object);
			}

			throw new Exception($"{collectionType} is not a enumerable type");
		}

		public static object GetValue(object instance, string propertyOrFieldName, out Type type)
		{
			object target;
			Type targetType;
			if (instance is TypeWrapper w)
			{
				// 静态属性赋值
				target = null;
				targetType = w.Type;
			}
			else
			{
				// 实例属性赋值
				target = instance;
				targetType = instance.GetType();
			}

			var p = targetType.GetProperty(propertyOrFieldName);
			if (p != null)
			{
				type = p.PropertyType;
				return p.GetValue(target);
			}

			var f = targetType.GetField(propertyOrFieldName);
			if (f != null)
			{
				type = f.FieldType;
				return f.GetValue(target);
			}

			throw new Exception($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
		}

		public static void SetValue(object instance, string propertyOrFieldName, object value)
		{
			object target;
			Type targetType;
			if (instance is TypeWrapper w)
			{
				// 静态属性赋值
				target = null;
				targetType = w.Type;
			}
			else
			{
				// 实例属性赋值
				target = instance;
				targetType = instance.GetType();
			}

			var p = targetType.GetProperty(propertyOrFieldName);
			if (p != null)
			{
				p.SetValue(target, value);
				return;
			}

			var f = targetType.GetField(propertyOrFieldName);
			if (f != null)
			{
				f.SetValue(target, value);
				return;
			}

			throw new Exception($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
		}

		public static object GetAndSetValue(object instance, string propertyOrFieldName, out Type type, Func<Type, object, object> valueFac)
		{
			object target;
			Type targetType;
			if (instance is TypeWrapper w)
			{
				// 静态属性赋值
				target = null;
				targetType = w.Type;
			}
			else
			{
				// 实例属性赋值
				target = instance;
				targetType = instance.GetType();
			}

			var p = targetType.GetProperty(propertyOrFieldName);
			if (p != null)
			{
				type = p.PropertyType;
				var value = p.GetValue(target);
				value = valueFac(type, value);
				p.SetValue(target, value);
				return value;
			}

			var f = targetType.GetField(propertyOrFieldName);
			if (f != null)
			{
				type = f.FieldType;
				var value = p.GetValue(target);
				value = valueFac(type, value);
				p.SetValue(target, value);
				return value;
			}

			throw new Exception($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
		}

		public static object EvalNumber(string number)
		{
			var lastChar = number[number.Length - 1];
			if (lastChar == 'm' || lastChar == 'M')
			{
				return decimal.Parse(number.Substring(0, number.Length - 1));
			}
			if (lastChar == 'd' || lastChar == 'D')
			{
				return double.Parse(number.Substring(0, number.Length - 1));
			}
			if (lastChar == 'f' || lastChar == 'F')
			{
				return float.Parse(number.Substring(0, number.Length - 1));
			}
			if (lastChar == 'L')
			{
				return long.Parse(number.Substring(0, number.Length - 1));
			}
			int dotIndex = number.IndexOf('.');
			if (dotIndex >= 0)
			{
				if (dotIndex == 0) return double.Parse("0" + number);
				return double.Parse(number);
			}
			if (number.Length >= 3)
			{
				var c0 = number[0];
				var c1 = number[1];
				if (c0 == '0' && (c1 == 'x' || c1 == 'X'))
				{
					if (number.Length <= 10) return System.Convert.ToInt32(number, 16);
					return System.Convert.ToInt64(number, 16);
				}
			}
			return int.Parse(number);
		}

		//public static string EvalString(string s)
		//{
		//	char sc = s[0];
		//	StringBuilder sb = new StringBuilder(s.Length - 2);
		//	bool preEscape = false;
		//	for (int i = 1; i < s.Length - 1; i++)
		//	{
		//		var c = s[i];
		//		if (c == '\\')
		//		{
		//			if (preEscape)
		//			{
		//				sb.Append('\\');
		//				preEscape = false;
		//			}
		//			else
		//			{
		//				preEscape = true;
		//			}
		//			continue;
		//		}
		//		if (preEscape)
		//		{
		//			preEscape = false;
		//			if (c == sc)
		//			{
		//				sb.Append(c);
		//				continue;
		//			}
		//			if (c == 'n')
		//			{
		//				sb.Append('\n');
		//				continue;
		//			}
		//			if (c == 'r')
		//			{
		//				sb.Append('\r');
		//				continue;
		//			}
		//			if (c == 't')
		//			{
		//				sb.Append('\t');
		//				continue;
		//			}
		//			throw new Exception("unknown string escape:" + s);
		//		}
		//		sb.Append(c);
		//	}
		//	return sb.ToString();
		//}


		/// <summary>
		/// 根据方法签名获取对应的 Delegate 类型
		/// </summary>
		/// <param name="method">方法信息</param>
		/// <returns>对应的 Delegate 类型</returns>
		public static Type GetDelegateType(MethodInfo method)
		{
			var parameters = method.GetParameters();
			var returnType = method.ReturnType;

			try
			{
				if (returnType == typeof(void))
				{
					if (parameters.Length == 0) return typeof(Action);
					if (parameters.Length == 1) return typeof(Action<>).MakeGenericType(parameters[0].ParameterType);
					if (parameters.Length == 2) return typeof(Action<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType);
					if (parameters.Length == 3) return typeof(Action<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType);
					if (parameters.Length == 4) return typeof(Action<,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType);
					if (parameters.Length == 5) return typeof(Action<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType);
					if (parameters.Length == 6) return typeof(Action<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, parameters[5].ParameterType);
					if (parameters.Length == 7) return typeof(Action<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, parameters[5].ParameterType, parameters[6].ParameterType);
					return null;
				}

				if (parameters.Length == 0) return typeof(Func<>).MakeGenericType(returnType);
				if (parameters.Length == 1) return typeof(Func<,>).MakeGenericType(parameters[0].ParameterType, returnType);
				if (parameters.Length == 2) return typeof(Func<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, returnType);
				if (parameters.Length == 3) return typeof(Func<,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, returnType);
				if (parameters.Length == 4) return typeof(Func<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, returnType);
				if (parameters.Length == 5) return typeof(Func<,,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, returnType);
				if (parameters.Length == 6) return typeof(Func<,,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, parameters[5].ParameterType, returnType);
				if (parameters.Length == 7) return typeof(Func<,,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, parameters[5].ParameterType, parameters[6].ParameterType, returnType);
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static Delegate CreateDelegate(MethodInfo methodInfo, object target = null)
		{
			if (methodInfo == null) return null;
			var delegateType = GetDelegateType(methodInfo);
			if (delegateType == null) return null;
			return target == null ? Delegate.CreateDelegate(delegateType, methodInfo) : Delegate.CreateDelegate(delegateType, target, methodInfo);
		}

		public static bool Contains(IEnumerable<string> list, string s)
		{
			if (list == null) return false;
			if (list is HashSet<string> set)
			{
				return set.Contains(s);
			}
			return list.Contains(s);
		}
	}
}
