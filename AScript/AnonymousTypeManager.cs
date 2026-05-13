using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace AScript
{
	/// <summary>
	/// 匿名类型管理器
	/// </summary>
	public class AnonymousTypeManager
	{
		private ModuleBuilder _ModuleBuilder;

		private readonly ConcurrentDictionary<string, Type> _TypeCache = new ConcurrentDictionary<string, Type>();

		/// <summary>
		/// 默认是否定义泛型类型
		/// </summary>
		public bool DefaultUseGeneric { get; set; } = true;

		/// <summary>
		/// 
		/// </summary>
		public AnonymousTypeManager() : this("<>AScript__AnonymousAssembly") { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyName">定义匿名类型的程序集名称</param>
		public AnonymousTypeManager(string assemblyName)
		{
			//var assemblyName = new AssemblyName { Name = "<>AScript__AnonymousAssembly" };
			_ModuleBuilder = AssemblyBuilder
				.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
				.DefineDynamicModule(assemblyName);
		}

		/// <summary>
		/// 创建匿名类型实例
		/// </summary>
		/// <param name="fieldNames"></param>
		/// <param name="fieldValues"></param>
		/// <param name="useGeneric">是否定义泛型类型</param>
		public NewExpression CreateObject(string[] fieldNames, Expression[] fieldValues, bool? useGeneric = null)
		{
			Type[] fieldTypes = fieldValues.Select(f => f.Type).ToArray();
			Type type = CreateType(fieldNames, fieldTypes, useGeneric ?? this.DefaultUseGeneric);
			ConstructorInfo constructor = type.GetConstructors()[0];
			return Expression.New(constructor, fieldValues);
		}

#if NETSTANDARD2_0_OR_GREATER
		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public Type CreateType((string name, Type type) field)
		{
			return CreateType(new[] { field.name }, new[] { field.type });
		}

		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="field1"></param>
		/// <param name="field2"></param>
		/// <returns></returns>
		public Type CreateType((string name, Type type) field1, (string name, Type type) field2)
		{
			return CreateType(new[] { field1.name, field2.name }, new[] { field1.type, field2.type });
		}

		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="field1"></param>
		/// <param name="field2"></param>
		/// <param name="field3"></param>
		/// <returns></returns>
		public Type CreateType((string name, Type type) field1, (string name, Type type) field2, (string name, Type type) field3)
		{
			return CreateType(new[] { field1.name, field2.name, field3.name }, new[] { field1.type, field2.type, field3.type });
		}

		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public Type CreateType(params (string name, Type type)[] fields)
		{
			return CreateType((IList<(string, Type)>)fields);
		}

		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public Type CreateType(IList<(string name, Type type)> fields)
		{
			string[] fieldNames;
			Type[] fieldTypes;
			if (fields == null || fields.Count == 0)
			{
				fieldNames = null;
				fieldTypes = null;
			}
			else
			{
				fieldNames = new string[fields.Count];
				fieldTypes = new Type[fields.Count];
				for (int i = 0; i < fields.Count; i++)
				{
					fieldNames[i] = fields[i].name;
					fieldTypes[i] = fields[i].type;
				}
			}
			return CreateType(fieldNames, fieldTypes);
		}
#endif

		/// <summary>
		/// 创建空匿名类型：没有任何字段
		/// </summary>
		/// <returns></returns>
		public Type CreateType()
		{
			return CreateType(null, null);
		}

		/// <summary>
		/// 创建匿名类型
		/// </summary>
		/// <param name="fieldNames">字段名列表</param>
		/// <param name="fieldTypes">字段类型列表</param>
		/// <param name="useGeneric">是否定义泛型类型</param>
		public Type CreateType(string[] fieldNames, Type[] fieldTypes, bool? useGeneric = null)
		{
			bool useGen = useGeneric ?? this.DefaultUseGeneric;
			string key = GetCacheKey(fieldNames, fieldTypes, useGen);
			if (!_TypeCache.TryGetValue(key, out var resultType))
			{
				// 不能用_TypeCache.GetOrAdd方法，此方法仍然可能多次创建类型导致类型重复异常
				// 暂时没有更好的使用key加锁的方案，只能全局加锁
				lock (_TypeCache)
				{
					if (!_TypeCache.TryGetValue(key, out resultType))
					{
						resultType = CreateTypeCore(fieldNames, fieldTypes, useGen);
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

		private Type CreateTypeCore(string[] fieldNames, Type[] fieldTypes, bool useGeneric)
		{
			string typeName = fieldNames == null || fieldNames.Length == 0 ?
				"<>f__AnonymousType" + _TypeCache.Count :
				"<>f__AnonymousType" + _TypeCache.Count + "`" + fieldNames.Length;

			TypeBuilder typeBuilder = _ModuleBuilder.DefineType(
				typeName,
				TypeAttributes.Public | TypeAttributes.Serializable,
				null,
				Type.EmptyTypes);

			Type[] parameterTypes;
			if (useGeneric && fieldNames != null && fieldNames.Length > 0)
			{
				// 定义泛型参数
				string[] genericParamNames = fieldNames.Select(n => $"<{n}>__TPar").ToArray();
				parameterTypes = typeBuilder.DefineGenericParameters(genericParamNames);
			}
			else
			{
				parameterTypes = fieldTypes;
			}

			// 定义属性和字段
			var fields = GenerateFields(typeBuilder, fieldNames, parameterTypes);
			// 定义构造函数
			GenerateConstructor(typeBuilder, parameterTypes, fields);
			// 生成Equals方法
			GenerateEquals(typeBuilder, fields);
			// 生成GetHashCode方法
			GenerateHashCode(typeBuilder, fields);
			// 生成ToString方法
			GenerateToString(typeBuilder, fields);

			return typeBuilder.CreateTypeInfo().AsType();
		}

		/// <summary>
		/// 生成类型缓存的键
		/// </summary>
		protected virtual string GetCacheKey(string[] fieldNames, Type[] fieldTypes, bool useGeneric)
		{
			if (fieldNames == null || fieldNames.Length == 0) return ";";
			if (useGeneric)
			{
				return string.Join(";", fieldNames);
			}
			return string.Join(";", fieldNames) + "|" + string.Join(";", fieldTypes.Select(a => string.IsNullOrEmpty(a.FullName) ? a.Name : a.FullName));
		}

		/// <summary>
		/// 生成属性和字段列表
		/// </summary>
		/// <param name="typeBuilder"></param>
		/// <param name="fieldNames"></param>
		/// <param name="parameterTypes"></param>
		/// <returns></returns>
		private static FieldBuilder[] GenerateFields(TypeBuilder typeBuilder, string[] fieldNames, Type[] parameterTypes)
		{
			if (fieldNames == null || fieldNames.Length == 0) return null;

			var fieldBuilders = new FieldBuilder[fieldNames.Length];
			for (int i = 0; i < fieldNames.Length; i++)
			{
				Type fieldType = parameterTypes[i];
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

				fieldBuilders[i] = fieldBuilder;
			}
			return fieldBuilders;
		}

		/// <summary>
		/// 生成构造函数
		/// </summary>
		/// <param name="typeBuilder"></param>
		/// <param name="parameterTypes"></param>
		/// <param name="fields"></param>
		private static void GenerateConstructor(TypeBuilder typeBuilder, Type[] parameterTypes, FieldBuilder[] fields)
		{
			var constructorIL = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				parameterTypes).GetILGenerator();

			// 为每个字段生成字段、属性和构造函数逻辑
			if (fields != null && fields.Length > 0)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					// 构造函数中加载参数并存储到字段
					constructorIL.Emit(OpCodes.Ldarg_S, 0);
					constructorIL.Emit(OpCodes.Ldarg_S, i + 1);
					constructorIL.Emit(OpCodes.Stfld, fields[i]);
				}
			}

			constructorIL.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// 生成Equals方法
		/// </summary>
		private static void GenerateEquals(TypeBuilder typeBuilder, IList<FieldBuilder> fields)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				"Equals",
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
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
			il.Emit(OpCodes.Isinst, typeBuilder);
			il.Emit(OpCodes.Brfalse, returnFalse);

			// 对每个字段进行比较
			var objectEquals = typeof(object).GetMethod("Equals", new[] { typeof(object), typeof(object) });

			foreach (var field in fields)
			{
				Type fieldType = field.FieldType;
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, field);
				il.Emit(OpCodes.Box, fieldType);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldfld, field);
				il.Emit(OpCodes.Box, fieldType);
				il.Emit(OpCodes.Call, objectEquals);
				il.Emit(OpCodes.Brfalse, returnFalse);
			}

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(returnFalse);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// 生成GetHashCode方法
		/// </summary>
		private static void GenerateHashCode(TypeBuilder typeBuilder, IList<FieldBuilder> fields)
		{
			var methodBuilder = typeBuilder.DefineMethod(
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

				// 泛型情况：box 后调用 GetHashCode
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, field);
				il.Emit(OpCodes.Box, field.FieldType);
				Label hasValueLabel = il.DefineLabel();
				Label nextLabel = il.DefineLabel();

				// 检查是否为空（box 后的 nullable 如果没有值会变成 null）
				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Brtrue_S, hasValueLabel);

				// null 值，pop 并加载 0
				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Br_S, nextLabel);

				il.MarkLabel(hasValueLabel);
				il.EmitCall(OpCodes.Callvirt, getHashCodeMethod, null);

				il.MarkLabel(nextLabel);

				il.Emit(OpCodes.Xor);
			}

			il.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// 生成ToString方法
		/// </summary>
		private static void GenerateToString(TypeBuilder typeBuilder, IList<FieldBuilder> fields)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				"ToString",
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
				typeof(string),
				Type.EmptyTypes);
			ILGenerator il = methodBuilder.GetILGenerator();

			if (fields == null || fields.Count == 0)
			{
				il.Emit(OpCodes.Ldstr, "{ }");
				il.Emit(OpCodes.Ret);
				return;
			}

			// 使用 StringBuilder 构建字符串
			var stringBuilderType = typeof(System.Text.StringBuilder);
			var stringBuilderCtor = stringBuilderType.GetConstructor(Type.EmptyTypes);
			var appendMethod = stringBuilderType.GetMethod("Append", new[] { typeof(string) });
			var toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);

			// 创建 StringBuilder 实例
			il.Emit(OpCodes.Newobj, stringBuilderCtor);

			// 添加起始大括号
			il.Emit(OpCodes.Ldstr, "{ ");
			il.Emit(OpCodes.Callvirt, appendMethod);

			// 添加每个字段: FieldName = value
			for (int i = 0; i < fields.Count; i++)
			{
				var field = fields[i];

				if (i > 0)
				{
					// 添加逗号和空格
					//il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldstr, ", ");
					il.Emit(OpCodes.Callvirt, appendMethod);
				}

				// 添加字段名
				//il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldstr, GetPropertyName(field.Name));
				il.Emit(OpCodes.Callvirt, appendMethod);

				// 添加 " = "
				//il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Ldstr, " = ");
				il.Emit(OpCodes.Callvirt, appendMethod);

				// 添加字段值
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, field);
				il.Emit(OpCodes.Box, field.FieldType);
				il.Emit(OpCodes.Callvirt, toStringMethod);
				il.Emit(OpCodes.Callvirt, appendMethod);
			}

			// 添加结束大括号
			//il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldstr, " }");
			il.Emit(OpCodes.Callvirt, appendMethod);

			// 调用 StringBuilder.ToString()
			il.Emit(OpCodes.Callvirt, stringBuilderType.GetMethod("ToString", Type.EmptyTypes));
			il.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// 从字段名获取属性名（去掉前缀下划线）
		/// </summary>
		private static string GetPropertyName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName) || fieldName.Length <= 1)
				return fieldName;
			return fieldName[0] == '_' ? fieldName.Substring(1) : fieldName;
		}
	}
}