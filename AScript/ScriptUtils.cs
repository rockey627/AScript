using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript
{
	public class ScriptUtils
	{
		public static readonly MethodInfo Method_ScriptUtils_Add = typeof(ScriptUtils).GetMethod("Add", new[] { typeof(object), typeof(object) });

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

		public static T Convert<T>(object v)
		{
			return (T)Convert(v, typeof(T));
		}

		public static bool IsMatchArgType(Type inType, Type defineType)
		{
			if (inType == null) return true;
			if (defineType == null) return true;
			if (inType == defineType) return true;
			if (defineType == typeof(object)) return true;
			if (typeof(Delegate).IsAssignableFrom(inType)) return typeof(Delegate).IsAssignableFrom(defineType);
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

		public static bool IsMatchArgTypes(IList<Type> inArgTypes, MethodBase method, out bool useScriptContext, out bool hasClosure, out int paramsIndex)
		{
			int argTypesCount = inArgTypes == null ? 0 : inArgTypes.Count;
			var methodParameters = method.GetParameters();
			if (methodParameters.Length == argTypesCount && argTypesCount == 0)
			{
				useScriptContext = false;
				hasClosure = false;
				paramsIndex = -1;
				return true;
			}

			bool hasParams = false;
			ParameterInfo lastParam = null;
			if (methodParameters.Length > 0)
			{
				lastParam = methodParameters[methodParameters.Length - 1];
				if (lastParam.IsDefined(typeof(ParamArrayAttribute), false))
				{
					hasParams = true;
				}
			}

			if (!hasParams && methodParameters.Length < argTypesCount)
			{
				useScriptContext = false;
				hasClosure = false;
				paramsIndex = -1;
				return false;
			}

			int index = 0;
			hasClosure = false;
			useScriptContext = false;
			if (methodParameters[index].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
			{
				index++;
				hasClosure = true;
			}
			if (methodParameters.Length > index && methodParameters[index].ParameterType == typeof(ScriptContext))
			{
				index++;
				useScriptContext = true;
			}
			if (!hasParams && methodParameters.Length - argTypesCount > index
				|| hasParams && methodParameters.Length - argTypesCount - 1 > index)
			{
				paramsIndex = -1;
				return false;
			}
			bool matched = true;
			int j;
			for (j = 0; j < argTypesCount; j++)
			{
				if (!IsMatchArgType(inArgTypes[j], methodParameters[j + index].ParameterType))
				{
					matched = false;
					break;
				}
			}
			paramsIndex = hasParams ? j : -1;
			if (matched || !hasParams || j + index < methodParameters.Length - 1) return matched;
			return true;
		}

		public static bool IsMatchArgTypes(IList<Expression> inArgs, MethodBase method, out bool useScriptContext, out bool hasClosure, out int paramsIndex)
		{
			int argTypesCount = inArgs == null ? 0 : inArgs.Count;
			var methodParameters = method.GetParameters();
			if (methodParameters.Length == argTypesCount && argTypesCount == 0)
			{
				useScriptContext = false;
				hasClosure = false;
				paramsIndex = -1;
				return true;
			}

			bool hasParams = false;
			ParameterInfo lastParam = null;
			if (methodParameters.Length > 0)
			{
				lastParam = methodParameters[methodParameters.Length - 1];
				if (lastParam.IsDefined(typeof(ParamArrayAttribute), false))
				{
					hasParams = true;
				}
			}

			if (!hasParams && methodParameters.Length < argTypesCount)
			{
				useScriptContext = false;
				hasClosure = false;
				paramsIndex = -1;
				return false;
			}

			int index = 0;
			hasClosure = false;
			useScriptContext = false;
			if (methodParameters[index].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
			{
				index++;
				hasClosure = true;
			}
			if (methodParameters.Length > index && methodParameters[index].ParameterType == typeof(ScriptContext))
			{
				index++;
				useScriptContext = true;
			}
			//if (methodParameters.Length - argTypesCount > index)
			if (!hasParams && methodParameters.Length - argTypesCount > index
				|| hasParams && methodParameters.Length - argTypesCount - 1 > index)
			{
				paramsIndex = -1;
				return false;
			}
			bool matched = true;
			int j;
			for (j = 0; j < argTypesCount; j++)
			{
				if (!IsMatchArgType(inArgs[j]?.Type ?? typeof(Delegate), methodParameters[j + index].ParameterType))
				{
					matched = false;
					break;
				}
			}
			paramsIndex = hasParams ? j : -1;
			if (matched || !hasParams || j + index < methodParameters.Length - 1) return matched;
			return true;
		}

		public static bool IsMatchArgTypes(IList<Type> inArgTypes, LambdaExpression lambda, out bool useScriptContext, out bool hasClosure)
		{
			int argTypesCount = inArgTypes == null ? 0 : inArgTypes.Count;
			var methodParameters = lambda.Parameters;
			if (methodParameters.Count < argTypesCount)
			{
				useScriptContext = false;
				hasClosure = false;
				return false;
			}
			if (methodParameters.Count == argTypesCount)
			{
				if (argTypesCount == 0)
				{
					useScriptContext = false;
					hasClosure = false;
					return true;
				}
			}
			int index = 0;
			hasClosure = false;
			useScriptContext = false;
			if (methodParameters[index].Type.FullName == "System.Runtime.CompilerServices.Closure")
			{
				index++;
				hasClosure = true;
			}
			if (methodParameters[index].Type == typeof(ScriptContext))
			{
				index++;
				useScriptContext = true;
			}
			if (methodParameters.Count - argTypesCount > index)
			{
				return false;
			}
			bool matched = true;
			for (int j = 0; j < argTypesCount; j++)
			{
				if (!IsMatchArgType(inArgTypes[j], methodParameters[j + index].Type))
				{
					matched = false;
					break;
				}
			}
			return matched;
		}

		public static bool IsMatchArgTypes(IList<Expression> inArgs, LambdaExpression lambda, out bool useScriptContext, out bool hasClosure)
		{
			int argTypesCount = inArgs == null ? 0 : inArgs.Count;
			var methodParameters = lambda.Parameters;
			if (methodParameters.Count < argTypesCount)
			{
				useScriptContext = false;
				hasClosure = false;
				return false;
			}
			if (methodParameters.Count == argTypesCount)
			{
				if (argTypesCount == 0)
				{
					useScriptContext = false;
					hasClosure = false;
					return true;
				}
			}
			int index = 0;
			hasClosure = false;
			useScriptContext = false;
			if (methodParameters[index].Type.FullName == "System.Runtime.CompilerServices.Closure")
			{
				index++;
				hasClosure = true;
			}
			if (methodParameters[index].Type == typeof(ScriptContext))
			{
				index++;
				useScriptContext = true;
			}
			if (methodParameters.Count - argTypesCount > index)
			{
				return false;
			}
			bool matched = true;
			for (int j = 0; j < argTypesCount; j++)
			{
				if (!IsMatchArgType(inArgs[j]?.Type ?? typeof(Delegate), methodParameters[j + index].Type))
				{
					matched = false;
					break;
				}
			}
			return matched;
		}

		public static object DynamicInvoke(ScriptContext context, Delegate d, object[] argValues, IList<Type> argTypes, bool useScriptContext, bool hasClosure)
		{
			if (useScriptContext)
			{
				var datas2 = new object[(argValues?.Length ?? 0) + 1];
				datas2[0] = context;
				if (argValues != null && argValues.Length > 0)
				{
					Array.Copy(argValues, 0, datas2, 1, argValues.Length);
				}
				argValues = datas2;
			}
			if (argValues != null && argValues.Length > 0)
			{
				int startIndex = 0;
				if (hasClosure) startIndex++;
				if (useScriptContext) startIndex++;
				var parameters = d.Method.GetParameters();
				for (int i = 0; i < argValues.Length; i++)
				{
					if (i < startIndex) continue;
					var paramType = parameters[i].ParameterType;
					var dataType = argTypes[i - startIndex];
					if (dataType != paramType)
					{
						var data = argValues[hasClosure ? i - 1 : i];
						if (data is IConvertible && !paramType.IsInstanceOfType(data))
						{
							argValues[hasClosure ? i - 1 : i] = System.Convert.ChangeType(data, paramType);
						}
					}
				}
			}
			return d.DynamicInvoke(argValues);
		}

		public static object DynamicInvoke(ScriptContext context, MethodInfo method, object target, object[] argValues, Type[] argTypes, bool useScriptContext, bool hasClosure, int paramsIndex)
		{
			if (paramsIndex >= 0)
			{
				if (argValues == null || argValues.Length == 0)
				{
					argValues = new object[] { null };
					argTypes = new Type[] { null };
				}
				else
				{
					var parameters = method.GetParameters();
					var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
					var paramsValues = new object[argValues.Length - paramsIndex];
					Array.Copy(argValues, paramsIndex, paramsValues, 0, paramsValues.Length);
					var paramsArr = Array.CreateInstance(itemType, paramsValues.Length);
					for (int i = 0; i < paramsValues.Length; i++)
					{
						paramsArr.SetValue(System.Convert.ChangeType(paramsValues[i], itemType), i);
					}
					var newValues = new object[paramsIndex + 1];
					var newTypes = new Type[newValues.Length];
					Array.Copy(argValues, 0, newValues, 0, paramsIndex);
					Array.Copy(argTypes, 0, newTypes, 0, paramsIndex);
					newValues[paramsIndex] = paramsArr;
					newTypes[paramsIndex] = paramsArr.GetType();
					argValues = newValues;
					argTypes = newTypes;
				}
			}
			if (useScriptContext)
			{
				var datas2 = new object[(argValues?.Length ?? 0) + 1];
				datas2[0] = context;
				if (argValues != null && argValues.Length > 0)
				{
					Array.Copy(argValues, 0, datas2, 1, argValues.Length);
				}
				argValues = datas2;
			}
			if (argValues != null && argValues.Length > 0)
			{
				int startIndex = 0;
				if (hasClosure) startIndex++;
				if (useScriptContext) startIndex++;
				var parameters = method.GetParameters();
				for (int i = 0; i < argValues.Length; i++)
				{
					if (i < startIndex) continue;
					int argTypeIndex = i - startIndex;
					var paramType = parameters[i].ParameterType;
					var dataType = argTypes[argTypeIndex];
					if (dataType == null)
					{
						argTypes[argTypeIndex] = paramType;
					}
					else if (dataType != paramType)
					{
						var arg = argValues[hasClosure ? i - 1 : i];
						if (arg is IConvertible && !paramType.IsInstanceOfType(arg))
						{
							argValues[hasClosure ? i - 1 : i] = System.Convert.ChangeType(arg, paramType);
						}
					}
				}
			}
			return method.Invoke(target, argValues);
		}

		public static object DynamicInvoke(ScriptContext context, BuildOptions options, EvalControl control, MethodInfo method, object target, object[] argValues, Type[] argTypes, bool useScriptContext, bool hasClosure, int paramsIndex)
		{
			if (argValues != null && argValues.Length > 0)
			{
				ParameterInfo[] parameters = null;
				for (int i = 0; i < argValues.Length; i++)
				{
					var arg = argValues[i];
					if (IsDefineFuncNode(arg))
					{
						if (parameters == null) parameters = method.GetParameters();
						argValues[i] = TryParseDelegateArg(context, options, control, arg, parameters[i].ParameterType);
					}
				}
			}
			return DynamicInvoke(context, method, target, argValues, argTypes, useScriptContext, hasClosure, paramsIndex);
		}

		public static Expression BuildInvoke(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, MethodInfo method, object target, IList<ITreeNode> argNodes, Expression[] argExprs, bool useScriptContext, bool hasClosure, int paramsIndex)
		{
			if (useScriptContext)
			{
				if (argExprs == null || argExprs.Length == 0)
				{
					argExprs = new Expression[] { Expression.Constant(scriptContext) };
				}
				else
				{
					var argExprs2 = new Expression[argExprs.Length + 1];
					argExprs2[0] = Expression.Constant(scriptContext);
					Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
					argExprs = argExprs2;
				}
			}
			var parameters = method.GetParameters();
			if (paramsIndex >= 0)
			{
				var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
				var paramsExprs = new Expression[argExprs.Length - paramsIndex];
				Array.Copy(argExprs, paramsIndex, paramsExprs, 0, paramsExprs.Length);
				for (int i = 0; i < paramsExprs.Length; i++)
				{
					var p = paramsExprs[i];
					if (p.Type != itemType)
					{
						paramsExprs[i] = Expression.Convert(p, itemType);
					}
				}
				var paramsArr = Expression.NewArrayInit(itemType, paramsExprs);
				var newExprs = new Expression[paramsIndex + 1];
				Array.Copy(argExprs, 0, newExprs, 0, paramsIndex);
				newExprs[paramsIndex] = paramsArr;
				argExprs = newExprs;
			}
			if (argExprs != null && argExprs.Length > 0)
			{
				for (int i = 0; i < argExprs.Length; i++)
				{
					var p = parameters[hasClosure ? i + 1 : i];
					var arg = argExprs[i];
					if (arg == null && typeof(Delegate).IsAssignableFrom(p.ParameterType))
					{
						var defineFuncNode = (DefineFuncNode)argNodes[i];
						if (p.ParameterType != typeof(Delegate))
						{
							var invokeMethod = p.ParameterType.GetMethod("Invoke");
							var ps = invokeMethod.GetParameters();
							for (int j = 0; j < ps.Length; j++)
							{
								defineFuncNode.Args[j].SystemType = ps[j].ParameterType;
							}
							defineFuncNode.ReturnSystemType = invokeMethod.ReturnType;
						}
						argExprs[i] = arg = defineFuncNode.Build(buildContext, scriptContext, options) ?? Expression.Constant(null, p.ParameterType);
					}
					else if (arg != null && arg.Type != p.ParameterType)
					{
						argExprs[i] = Expression.Convert(arg, p.ParameterType);
					}
				}
			}
			return target == null ? Expression.Call(method, argExprs) : Expression.Call(target is Expression targetExpr ? targetExpr : Expression.Constant(target), method, argExprs);
		}

		public static object TryParseDelegateArg(ScriptContext context, BuildOptions options, EvalControl control, object arg, Type delegateType)
		{
			if (arg is DefineFuncNode node)
			{
				arg = node.Eval(context, options, control, out _);
			}
			if (arg is CustomFunction func)
			{
				return func.Compile(delegateType, context, options);
			}
			if (arg is CustomFunctionObject cfo)
			{
				return cfo.Compile(delegateType, options);
			}
			return arg;
		}

		public static bool IsDefineFuncNode(object obj)
		{
			return obj is DefineFuncNode || obj is CustomFunction || obj is CustomFunctionObject;
		}

		public static object GetDefaultValue(Type targetType)
		{
			if (targetType == null || targetType == typeof(void)) return null;
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

			throw new Exceptions.ScriptRuntimeException($"{collectionType} is not a enumerable type");
		}

		public static object GetValue(object instance, string propertyOrFieldName, out Type type, bool throwExceptionIfNotExists = true)
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
				if (dict.TryGetValue(propertyOrFieldName, out var value))
				{
					type = value?.GetType() ?? typeof(object);
				}
				else
				{
					type = null;
				}
				return value;
			}

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

			if (throwExceptionIfNotExists)
			{
				throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
			}

			type = null;
			return null;
		}

		public static void SetValue(object instance, string propertyOrFieldName, object value)
		{
			object target;
			Type targetType;
			var flags = BindingFlags.Public | BindingFlags.IgnoreCase;
			if (instance is TypeWrapper w)
			{
				// 静态属性赋值
				target = null;
				targetType = w.Type;
				flags |= BindingFlags.Static;
			}
			else
			{
				// 实例属性赋值
				target = instance;
				targetType = instance.GetType();
				flags |= BindingFlags.Instance;
			}

			if (instance is DataRow dataRow)
			{
				dataRow[propertyOrFieldName] = value;
				return;
			}

			if (instance is ExpandoObject)
			{
				((IDictionary<string, object>)instance)[propertyOrFieldName] = value;
				return;
			}

			var p = targetType.GetProperty(propertyOrFieldName, flags);
			if (p != null)
			{
				if (value != null && value.GetType() != p.PropertyType)
				{
					value = Convert(value, p.PropertyType);
				}
				p.SetValue(target, value);
				return;
			}

			var f = targetType.GetField(propertyOrFieldName, flags);
			if (f != null)
			{
				if (value != null && value.GetType() != f.FieldType)
				{
					value = Convert(value, f.FieldType);
				}
				f.SetValue(target, value);
				return;
			}

			var m = targetType.GetMethod("dynamic_set");
			if (m != null)
			{
				m.Invoke(target, new object[] { propertyOrFieldName, value });
				return;
			}

			if (target is DynamicObject)
			{
				DynamicSetMember(target, propertyOrFieldName, value);
				return;
			}

			throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
		}

		public static void DynamicSetMember(object target, string memberName, object value)
		{
			dynamic d = target;
			d[memberName] = value;
			//var binder = Microsoft.CSharp.RuntimeBinder.Binder.SetMember(
			//	Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None,
			//	memberName,
			//	target.GetType(),
			//	new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null) }
			//);
			//var callSite = System.Runtime.CompilerServices.CallSite<Action<System.Runtime.CompilerServices.CallSite, object, object>>.Create(binder);
			//callSite.Target(callSite, target, value);

			//var binder = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null);
			//var callInfo = new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] { binder };
			//var setMemberBinder = Microsoft.CSharp.RuntimeBinder.CSharpSetMemberBinder.Create(new Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags(), "PropertyName", type, callInfo);

			//// 使用反射调用 TrySetMember 方法
			//var method = typeof(DynamicObject).GetMethod("TrySetMember", BindingFlags.Instance | BindingFlags.NonPublic);
			//bool result = (bool)method.Invoke(obj, new object[] { setMemberBinder, "ValueToSet" }); // 注意这里的 "ValueToSet" 应替换为具体的值或对象
		}

		public static object GetAndSetValue(object instance, string propertyOrFieldName, out Type type, Func<MemberInfo, Type, object, object> valueFac)
		{
			object target;
			Type targetType;
			var flags = BindingFlags.Public | BindingFlags.IgnoreCase;
			if (instance is TypeWrapper w)
			{
				// 静态属性赋值
				target = null;
				targetType = w.Type;
				flags |= BindingFlags.Static;
			}
			else
			{
				// 实例属性赋值
				target = instance;
				targetType = instance.GetType();
				flags |= BindingFlags.Instance;
			}

			if (instance is DataRow dataRow)
			{
				var value = dataRow[propertyOrFieldName];
				type = dataRow.Table.Columns[propertyOrFieldName].DataType;
				value = valueFac(null, type, value);
				dataRow[propertyOrFieldName] = value;
				return value;
			}

			if (instance is ExpandoObject)
			{
				var dict = (IDictionary<string, object>)instance;
				var value = dict[propertyOrFieldName];
				type = value?.GetType();
				value = valueFac(null, type, value);
				dict[propertyOrFieldName] = value;
				return value;
			}

			var p = targetType.GetProperty(propertyOrFieldName, flags);
			if (p != null)
			{
				type = p.PropertyType;
				var value = p.GetValue(target);
				value = valueFac(p, type, value);
				p.SetValue(target, value);
				return value;
			}

			var f = targetType.GetField(propertyOrFieldName, flags);
			if (f != null)
			{
				type = f.FieldType;
				var value = p.GetValue(target);
				value = valueFac(f, type, value);
				p.SetValue(target, value);
				return value;
			}

			var e = targetType.GetEvent(propertyOrFieldName, flags);
			if (e != null)
			{
				type = typeof(void);
				valueFac(e, e.EventHandlerType, null);
				return null;
			}

			throw new Exceptions.ScriptRuntimeException($"unknow Property or Field {targetType.Name}.{propertyOrFieldName}");
		}

		/// <summary>
		/// 索引器赋值
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="idx"></param>
		/// <param name="valueFac"></param>
		/// <returns></returns>
		public static object GetAndSetValue(object instance, object idx, Func<object, object> valueFac)
		{
			if (instance is DataRow dataRow)
			{
				if (idx is int n)
				{
					var value = valueFac(dataRow[n]);
					dataRow[n] = value;
					return value;
				}
				else
				{
					string name = (string)idx;
					var value = valueFac(dataRow[name]);
					dataRow[name] = value;
					return value;
				}
			}
			if (instance is Array array)
			{
				// 数组赋值
				int index = System.Convert.ToInt32(idx);
				var value = valueFac(array.GetValue(index));
				array.SetValue(value, index);
				return value;
			}
			if (instance is IDictionary dict)
			{
				// Dictionary赋值
				var value = valueFac(dict[idx]);
				dict[idx] = value;
				return value;
			}
			if (instance is IList list)
			{
				int i = System.Convert.ToInt32(idx);
				var value = valueFac(list[i]);
				list[i] = value;
				return value;
			}

			{
				// 其他类型使用动态调用
				dynamic dObj = instance;
				var value = valueFac(dObj[idx]);
				dObj[idx] = value;
				return value;
			}
		}

		public static object GetValue(ScriptContext context, object instance, string propertyOrFieldName)
		{
			var flags = BindingFlags.Public | BindingFlags.IgnoreCase;
			// 实例属性
			var target = instance;
			var targetType = instance.GetType();
			flags |= BindingFlags.Instance;

			if (instance is DataRow dataRow)
			{
				//type = dataRow.Table.Columns[propertyOrFieldName].DataType;
				return dataRow[propertyOrFieldName];
			}

			if (instance is ExpandoObject)
			{
				var dict = (IDictionary<string, object>)instance;
				//var value = dict[propertyOrFieldName];
				dict.TryGetValue(propertyOrFieldName, out var value);
				//type = value?.GetType();
				return value;
			}

			if (context.IsObjectMemberEnabled(targetType) ?? true)
			{
				var p = targetType.GetProperty(propertyOrFieldName, flags);
				if (p != null)
				{
					//type = p.PropertyType;
					return p.GetValue(target);
				}

				var f = targetType.GetField(propertyOrFieldName, flags);
				if (f != null)
				{
					//type = f.FieldType;
					return f.GetValue(target);
				}

				var m = targetType.GetMethod("dynamic_get");
				if (m != null)
				{
					return m.Invoke(target, new object[] { propertyOrFieldName });
				}

				if (target is DynamicObject)
				{
					dynamic d = target;
					return d[propertyOrFieldName];
				}
			}

			return context.EvalFunc($"get_{propertyOrFieldName}", new[] { instance }, new[] { targetType }, out _);
		}

		public static object EvalNumber(string number)
		{
			return EvalNumber(number, false);
		}

		public static object EvalNumber(string number, bool int2long)
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
				if (c0 == '0')
				{
					if (c1 == 'x' || c1 == 'X')
					{
						if (number.Length <= 10 && !int2long) return System.Convert.ToInt32(number, 16);
						return System.Convert.ToInt64(number, 16);
					}
					if (c1 == 'b' || c1 == 'B')
					{
						var binStr = number.Substring(2);
						if (binStr.Length <= 32 && !int2long) return System.Convert.ToInt32(binStr, 2);
						return System.Convert.ToInt64(binStr, 2);
					}
				}
			}
			if (int2long) return long.Parse(number);
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
		//			throw new Exceptions.ScriptRuntimeException("unknown string escape:" + s);
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
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 根据方法签名获取对应的 Delegate 类型
		/// </summary>
		/// <returns>对应的 Delegate 类型</returns>
		public static Type GetDelegateType(Type[] argTypes, Type returnType)
		{
			if (argTypes == null) argTypes = Type.EmptyTypes;
			try
			{
				if (returnType == typeof(void))
				{
					if (argTypes.Length == 0) return typeof(Action);
					if (argTypes.Length == 1) return typeof(Action<>).MakeGenericType(argTypes[0]);
					if (argTypes.Length == 2) return typeof(Action<,>).MakeGenericType(argTypes[0], argTypes[1]);
					if (argTypes.Length == 3) return typeof(Action<,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2]);
					if (argTypes.Length == 4) return typeof(Action<,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3]);
					if (argTypes.Length == 5) return typeof(Action<,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4]);
					if (argTypes.Length == 6) return typeof(Action<,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4], argTypes[5]);
					if (argTypes.Length == 7) return typeof(Action<,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4], argTypes[5], argTypes[6]);
					return null;
				}

				if (argTypes.Length == 0) return typeof(Func<>).MakeGenericType(returnType);
				if (argTypes.Length == 1) return typeof(Func<,>).MakeGenericType(argTypes[0], returnType);
				if (argTypes.Length == 2) return typeof(Func<,,>).MakeGenericType(argTypes[0], argTypes[1], returnType);
				if (argTypes.Length == 3) return typeof(Func<,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], returnType);
				if (argTypes.Length == 4) return typeof(Func<,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], returnType);
				if (argTypes.Length == 5) return typeof(Func<,,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4], returnType);
				if (argTypes.Length == 6) return typeof(Func<,,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4], argTypes[5], returnType);
				if (argTypes.Length == 7) return typeof(Func<,,,,,>).MakeGenericType(argTypes[0], argTypes[1], argTypes[2], argTypes[3], argTypes[4], argTypes[5], argTypes[6], returnType);
				return null;
			}
			catch
			{
				return null;
			}
		}

		public static Delegate CreateDelegate(MethodInfo methodInfo, object target = null)
		{
			if (methodInfo == null) return null;
			if (methodInfo.IsGenericMethod) return null;
			var delegateType = GetDelegateType(methodInfo);
			if (delegateType == null) return null;
			return target == null ? Delegate.CreateDelegate(delegateType, methodInfo) : Delegate.CreateDelegate(delegateType, target, methodInfo);
		}

		public static Delegate ConvertDelegate(Delegate del, Type targetDelegateType)
		{
			if (del == null) return null;
			if (del.GetType() == targetDelegateType) return del;
			var delMethod = del.Method ?? del.GetType().GetMethod("Invoke");
			var tarMethod = targetDelegateType.GetMethod("Invoke");
			ParameterExpression[] tarArgs;
			Expression[] delArgs;
			var delParameters = delMethod.GetParameters();
			int delIndex = 0;
			// 忽略闭包参数
			if (delParameters.Length > 0 && delParameters[0].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
			{
				delIndex = 1;
			}
			// 
			if (delParameters.Length == delIndex)
			{
				tarArgs = ExpressionUtils.Empty_ParameterExpressions;
				delArgs = ExpressionUtils.Empty_Expressions;
			}
			else
			{
				var tarParameters = tarMethod.GetParameters();
				tarArgs = new ParameterExpression[tarParameters.Length];
				delArgs = new Expression[tarParameters.Length];
				for (int i = 0; i < tarParameters.Length; i++)
				{
					var delParam = delParameters[i + delIndex];
					var tarParam = tarParameters[i];
					var tarArg = Expression.Parameter(tarParam.ParameterType);
					tarArgs[i] = tarArg;
					if (delParam.ParameterType == tarParam.ParameterType)
					{
						delArgs[i] = tarArg;
					}
					else
					{
						delArgs[i] = Expression.Convert(tarArg, delParam.ParameterType);
					}
				}
			}
			Expression invoke = Expression.Invoke(Expression.Constant(del), delArgs);
			if (delMethod.ReturnType == typeof(void))
			{
				if (tarMethod.ReturnType != typeof(void))
				{
					var defaultValue = Expression.Default(tarMethod.ReturnType);
					var block = Expression.Block(invoke, defaultValue);
					invoke = block;
				}
			}
			else if (tarMethod.ReturnType != typeof(void) && tarMethod.ReturnType != delMethod.ReturnType)
			{
				invoke = Expression.Convert(invoke, tarMethod.ReturnType);
			}
			return Expression.Lambda(targetDelegateType, invoke, tarArgs).Compile();
		}

		public static TDelegate ConvertDelegate<TDelegate>(Delegate del) where TDelegate : Delegate
		{
			return (TDelegate)ConvertDelegate(del, typeof(TDelegate));
		}

		public static Expression ConvertDelegate(Expression expr, Type targetDelegateType)
		{
			if (expr is LambdaExpression lambda)
			{
				Delegate d;
				if (lambda.Type == targetDelegateType)
				{
					d = lambda.Compile();
				}
				else
				{
					d = Expression.Lambda(targetDelegateType, lambda.Body, lambda.Parameters).Compile();
				}
				return Expression.Constant(d);
			}
			if (expr.Type == targetDelegateType) return expr;
			var method = expr.Type.GetMethod("Invoke");
			var methodArgs = method.GetParameters();
			var parameters = new ParameterExpression[methodArgs.Length];
			for (int i = 0; i < methodArgs.Length; i++)
			{
				parameters[i] = Expression.Parameter(methodArgs[i].ParameterType);
			}
			return Expression.Lambda(targetDelegateType, Expression.Invoke(expr, parameters), parameters);
			//var func = Expression.Lambda(targetDelegateType, Expression.Invoke(expr, parameters), parameters).Compile();
			//return Expression.Constant(func);
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

		/// <summary>
		/// 列表切片赋值（运行时执行）
		/// </summary>
		/// <param name="list">目标列表</param>
		/// <param name="start">起始索引（支持负数）</param>
		/// <param name="end">结束索引（支持负数）</param>
		/// <param name="values">要赋值的值列表</param>
		/// <returns>赋值后的值列表</returns>
		public static void SliceAssign(IList list, int start, int end, IList values)
		{
			if (list == null || values == null) return;

			int listLen = list.Count;

			// 负数索引从结尾计算
			if (start < 0) start = listLen + start;
			if (end < 0) end = listLen + end;

			for (int i = start; i < end && i - start < values.Count; i++)
			{
				list[i] = values[i - start];
			}
		}

		public static bool IsVariableExists(BuildContext buildContext, ScriptContext scriptContext, string varName)
		{
			if (buildContext != null
				&& buildContext.TryGetVariableOrParameter(varName, out _))
			{
				return true;
			}
			if (scriptContext != null
				&& scriptContext.GetOwnerContext(varName, out _, out _, searchType: false) != null)
			{
				return true;
			}
			scriptContext.EvalVarFromLangs(varName, out var type);
			return type != null;
		}

		/// <summary>
		/// 动态调用 DynamicObject 的方法（运行时执行）
		/// </summary>
		public static bool TryInvokeDynamicObject(DynamicObject target, string methodName, object[] args, out object result)
		{
			var binder = Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
						Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None,
						methodName, // 成员名称，对应你的方法名或属性名等
						new[] { typeof(object[]), typeof(object) },
						null,
						new[]
						{
							Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null),
							Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.IsOut, null)
						}
					);
			var dynamicArgs = new object[] { binder, args, null };
			var success = (bool)ExpressionUtils.Method_DynamicObject_TryInvokeMember.Invoke(target, dynamicArgs);
			if (success)
			{
				result = dynamicArgs[2];
				return true;
			}
			result = null;
			return false;
		}

		public static object InvokeDynamicObject(DynamicObject target, string methodName, object[] args)
		{
			TryInvokeDynamicObject(target, methodName, args, out var result);
			return result;
		}

		/// <summary>
		/// 构建动态调用 DynamicObject 方法的表达式
		/// </summary>
		public static Expression BuildDynamicObject(BuildContext buildContext, ScriptContext scriptContext, Expression target, string methodName, Expression[] argExprs)
		{
			// 构建数组表达式: new object[] { arg1, arg2, ... }
			Expression argsArray;
			if (argExprs == null || argExprs.Length == 0)
			{
				argsArray = Expression.NewArrayBounds(typeof(object), Expression.Constant(0));
			}
			else
			{
				var initializers = new Expression[argExprs.Length];
				for (int i = 0; i < argExprs.Length; i++)
				{
					initializers[i] = argExprs[i].Type.IsValueType
						? Expression.Convert(argExprs[i], typeof(object))
						: argExprs[i];
				}
				argsArray = Expression.NewArrayInit(typeof(object), initializers);
			}

			// 调用辅助方法: ScriptUtils.InvokeDynamicObject(target, methodName, args)
			var method = typeof(ScriptUtils).GetMethod("InvokeDynamicObject", new[] { typeof(DynamicObject), typeof(string), typeof(object[]) });
			if (target.Type == typeof(object))
			{
				target = Expression.Convert(target, typeof(DynamicObject));
			}
			return Expression.Call(method, target, Expression.Constant(methodName), argsArray);
		}

		public static object Add(object v1, object v2)
		{
			return ((dynamic)v1) + ((dynamic)v2);
		}

		public static Expression Add(Expression v1, Expression v2)
		{
			return null;
		}
	}
}
