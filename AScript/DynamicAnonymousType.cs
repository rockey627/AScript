using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace AScript
{
	/// <summary>
	/// 动态创建匿名类型
	/// </summary>
	public static class DynamicAnonymousType
	{
		public static bool DefaultUseNonGenericAnonymousType = true;

		private static readonly ModuleBuilder _ModuleBuilder;

		private static readonly ConcurrentDictionary<string, Type> _TypeCache = new ConcurrentDictionary<string, Type>();

		static DynamicAnonymousType()
		{
			var assemblyName = new AssemblyName { Name = "<>AScript__AnonymousAssembly" };
			_ModuleBuilder = AssemblyBuilder
				.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
				.DefineDynamicModule(assemblyName.Name);
		}

		/// <summary>
		/// 创建匿名类型实例
		/// </summary>
		/// <param name="fieldNames"></param>
		/// <param name="fieldValues"></param>
		/// <param name="useNonGenericAnonymousType">是否定义非泛型类型</param>
		public static NewExpression CreateObject(string[] fieldNames, Expression[] fieldValues, bool? useNonGenericAnonymousType = null)
		{
			// 提取字段类型列表
			Type[] fieldTypes = fieldValues.Select(f => f.Type).ToArray();
			Type dynamicType = CreateType(fieldNames, fieldTypes, useNonGenericAnonymousType ?? DefaultUseNonGenericAnonymousType);

			ConstructorInfo constructor = dynamicType.GetConstructors()[0];
			return Expression.New(constructor, fieldValues);
			//PropertyInfo[] properties = dynamicType.GetProperties();

			//return Expression.New(constructor, fieldValues, properties);
		}

		/// <summary>
		/// 创建动态类型
		/// </summary>
		/// <param name="fieldNames"></param>
		/// <param name="fieldTypes"></param>
		/// <param name="useNonGenericAnonymousType">是否定义非泛型类型</param>
		public static Type CreateType(string[] fieldNames, Type[] fieldTypes, bool? useNonGenericAnonymousType = null)
		{
			string key = GetCacheKey(fieldNames, fieldTypes, useNonGenericAnonymousType ?? DefaultUseNonGenericAnonymousType);
			if (!_TypeCache.TryGetValue(key, out var resultType))
			{
				lock (_TypeCache)
				{
					if (!_TypeCache.TryGetValue(key, out resultType))
					{
						resultType = CreateTypeCore(fieldNames, fieldTypes, useNonGenericAnonymousType ?? DefaultUseNonGenericAnonymousType);
						_TypeCache[key] = resultType;
					}
				}
			}
			if (resultType.IsGenericTypeDefinition)
			{
				return resultType.MakeGenericType(fieldTypes);
			}
			return resultType;
		}

		private static Type CreateTypeCore(string[] fieldNames, Type[] fieldTypes, bool useNonGenericAnonymousType)
		{
			string typeName = fieldNames == null || fieldNames.Length == 0 ?
				"<>f__AnonymousType" + _TypeCache.Count :
				"<>f__AnonymousType" + _TypeCache.Count + "`" + fieldNames.Length;

			GenericTypeParameterBuilder[] genericParameters = null;
			TypeBuilder typeBuilder = _ModuleBuilder.DefineType(
				typeName,
				TypeAttributes.Public | TypeAttributes.Serializable,
				null,
				Type.EmptyTypes);

			// 定义泛型参数
			if (!useNonGenericAnonymousType && fieldNames != null && fieldNames.Length > 0)
			{
				string[] genericParamNames = fieldNames.Select(n => $"<{n}>__TPar").ToArray();
				genericParameters = typeBuilder.DefineGenericParameters(genericParamNames);
			}

			// 获取字段类型
			Type[] types;
			if (genericParameters != null)
			{
				types = new Type[genericParameters.Length];
				for (int i = 0; i < genericParameters.Length; i++)
				{
					types[i] = genericParameters[i];
				}
			}
			else
			{
				types = fieldTypes;
			}

			// 定义构造函数
			var constructorIL = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				types).GetILGenerator();

			FieldBuilder[] fieldBuilders = null;

			// 为每个字段生成字段、属性和构造函数逻辑
			if (fieldNames != null && fieldNames.Length > 0)
			{
				fieldBuilders = new FieldBuilder[fieldNames.Length];
				for (int i = 0; i < fieldNames.Length; i++)
				{
					Type fieldType = types[i];
					string fieldName = fieldNames[i];

					// 定义私有字段
					var fieldBuilder = typeBuilder.DefineField("_" + fieldName, fieldType, FieldAttributes.Private);

					// 定义属性
					PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
						fieldName,
						PropertyAttributes.None,
						fieldType,
						Type.EmptyTypes);

					// 定义get方法
					MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
					MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
						"get_" + fieldName,
						attributes,
						fieldType,
						Type.EmptyTypes);

					ILGenerator getIL = getMethodBuilder.GetILGenerator();
					getIL.Emit(OpCodes.Ldarg_0);
					getIL.Emit(OpCodes.Ldfld, fieldBuilder);
					getIL.Emit(OpCodes.Ret);

					propertyBuilder.SetGetMethod(getMethodBuilder);

					// 构造函数中加载参数并存储到字段
					constructorIL.Emit(OpCodes.Ldarg_S, 0);
					constructorIL.Emit(OpCodes.Ldarg_S, i + 1);
					constructorIL.Emit(OpCodes.Stfld, fieldBuilder);

					fieldBuilders[i] = fieldBuilder;
				}
			}

			constructorIL.Emit(OpCodes.Ret);

			// 生成Equals方法
			GenerateEquals(typeBuilder, fieldBuilders, useNonGenericAnonymousType);

			// 生成GetHashCode方法
			GenerateHashCode(typeBuilder, fieldBuilders, useNonGenericAnonymousType);

			return typeBuilder.CreateTypeInfo().AsType();
		}

		/// <summary>
		/// 生成类型缓存的键
		/// </summary>
		private static string GetCacheKey(string[] fieldNames, Type[] fieldTypes, bool useNonGenericAnonymousType)
		{
			if (fieldNames == null || fieldNames.Length == 0) return ";";
			if (!useNonGenericAnonymousType)
			{
				return string.Join(";", fieldNames);
			}
			return string.Join(";", fieldNames) + "|" + string.Join(";", fieldTypes.Select(a => string.IsNullOrEmpty(a.FullName) ? a.Name : a.FullName));
			//return string.Join(";", fields.Select(f =>
			//	f.Item1 + (string.IsNullOrEmpty(f.Item2.FullName) ? f.Item2.Name : f.Item2.FullName)));
		}

		/// <summary>
		/// 生成Equals方法
		/// </summary>
		private static void GenerateEquals(TypeBuilder tb, IList<FieldBuilder> fields, bool useNonGenericAnonymousType)
		{
			var methodBuilder = tb.DefineMethod(
				"Equals",
				MethodAttributes.Public | MethodAttributes.Virtual,
				typeof(bool),
				new Type[] { typeof(object) });
			ILGenerator il = methodBuilder.GetILGenerator();

			if (fields == null || fields.Count == 0)
			{
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Ret);
				return;
			}

			Label returnFalse = il.DefineLabel();

			// 检查参数类型
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Isinst, tb);
			il.Emit(OpCodes.Brfalse, returnFalse);

			// 对每个字段进行比较
			var objectEquals = typeof(object).GetMethod("Equals", new[] { typeof(object), typeof(object) });

			foreach (var field in fields)
			{
				//Type equalityComparerType = typeof(EqualityComparer<>);//.MakeGenericType(field.FieldType);
				//MethodInfo defaultMethod = equalityComparerType.GetMethod("get_Default");
				//MethodInfo[] methods = equalityComparerType.GetMethods();
				//MethodInfo equalsMethod = methods.First(m => m.Name == "Equals");

				if (useNonGenericAnonymousType)
				{
					Type fieldType = field.FieldType;
					//Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(fieldType);
					//MethodInfo defaultMethod = equalityComparerType.GetMethod("get_Default");
					//MethodInfo equalsMethod = equalityComparerType.GetMethod("Equals", new Type[] { fieldType, fieldType });

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, field);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldfld, field);
					if (fieldType.IsValueType)
					{
						il.Emit(OpCodes.Ceq);
					}
					else
					{
						il.Emit(OpCodes.Call, objectEquals);
						//il.Emit(OpCodes.Call, defaultMethod);
						//il.Emit(OpCodes.Callvirt, equalsMethod);
					}
					il.Emit(OpCodes.Brfalse, returnFalse);
				}
				else
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, field);
					il.Emit(OpCodes.Ldarg_1);
					//il.Emit(OpCodes.Castclass, tb);
					il.Emit(OpCodes.Ldfld, field);
					//if (field.FieldType.IsValueType)
					//{
					il.Emit(OpCodes.Ceq);
					//}
					//else
					//{
					//il.Emit(OpCodes.Call, objectEquals);
					//}
					//il.Emit(OpCodes.Call, defaultMethod);
					//il.Emit(OpCodes.Callvirt, equalsMethod);
					il.Emit(OpCodes.Brfalse, returnFalse);
			}
		}

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(returnFalse);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ret);
			//tb.DefineMethodOverride(methodBuilder, typeof(object).GetMethod(methodBuilder.Name, new[] { typeof(object) }));
		}

		/// <summary>
		/// 生成GetHashCode方法
		/// </summary>
		private static void GenerateHashCode(TypeBuilder tb, IList<FieldBuilder> fields, bool useNonGenericAnonymousType)
		{
			var methodBuilder = tb.DefineMethod(
				"GetHashCode",
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
				typeof(int),
				Type.EmptyTypes);
			ILGenerator il = methodBuilder.GetILGenerator();

			if (fields == null || fields.Count == 0)
			{
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ret);
				return;
			}

			il.Emit(OpCodes.Ldc_I4_0);

			MethodInfo getHashCodeMethod = typeof(object).GetMethod("GetHashCode");
			for (int i = 0; i < fields.Count; i++)
			{
				FieldInfo field = fields[i];

				if (useNonGenericAnonymousType)
				{
					// 使用 EqualityComparer<T>.Default.GetHashCode()
					Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(field.FieldType);
					MethodInfo defaultMethod = equalityComparerType.GetMethod("get_Default");
					MethodInfo getHashCodeMethod1 = equalityComparerType.GetMethod("GetHashCode", new Type[] { field.FieldType });

					il.EmitCall(OpCodes.Call, defaultMethod, null);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, field);
					il.EmitCall(OpCodes.Callvirt, getHashCodeMethod1, null);
				}
				else
				{
					// 对于泛型类型定义，字段类型是泛型参数，直接 box 后调用 GetHashCode
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, field);
					//il.Emit(OpCodes.Box, fieldType);
					il.EmitCall(OpCodes.Callvirt, getHashCodeMethod, null);
				}

				il.Emit(OpCodes.Xor);
			}

			il.Emit(OpCodes.Ret);
		}
	}
}