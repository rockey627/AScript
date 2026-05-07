using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AScript.Nodes;

namespace AScript.Functions
{
	/// <summary>
	/// 泛型方法
	/// </summary>
	public class GenericFunction : IFunctionEvaluator, IFunctionBuilder
	{
		public object Target { get; private set; }
		public MethodInfo Method { get; private set; }

		public GenericFunction(MethodInfo method) : this(null, method) { }
		public GenericFunction(object target, MethodInfo method)
		{
			this.Target = target;
			this.Method = method;
		}

		public void Build(FunctionBuildArgs e)
		{
		}

		private Type GetGenericType(Type paremterType, Type argType)
		{
			var type = GetGenericType0(paremterType, argType);
			if (type == null)
			{
				type = GetGenericType1(paremterType, argType);
			}
			return type;
		}

		private Type GetGenericType0(Type paremterType, Type argType)
		{
			var type = argType;
			while (type != null && type != typeof(object))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == paremterType.GetGenericTypeDefinition())
				{
					return type;
				}
				type = type.BaseType;
			}
			return null;
		}

		private Type GetGenericType1(Type paremterType, Type argType)
		{
			foreach (var type in argType.GetInterfaces())
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == paremterType.GetGenericTypeDefinition())
				{
					return type;
				}
			}
			return null;
		}

		public void Eval(FunctionEvalArgs e)
		{
			var parameters = this.Method.GetParameters();
			int argsCount = e.Args == null ? 0 : e.Args.Count;
			if (argsCount != parameters.Length) return;

			// 第一步：获取所有参数的运行时类型
			var argTypes = new Type[e.Args.Count];
			var argValues = new object[e.Args.Count];
			for (int i = 0; i < e.Args.Count; i++)
			{
				var arg = e.Args[i];
				if (arg is DefineFuncNode)
				{
					argTypes[i] = typeof(Delegate);
				}
				else
				{
					argValues[i] = arg.Eval(e.Context, e.Options, e.Control, out var argType);
					argTypes[i] = argType;
				}
			}

			// 第二步：从参数类型推导泛型类型参数
			var genericArgs = this.Method.GetGenericArguments();
			var typeArguments = new Type[genericArgs.Length];
			int typeArgumentsFillCount = 0;
			for (int i = 0; i < parameters.Length; i++)
			{
				var paramType = parameters[i].ParameterType;
				var argType = argTypes[i];
				if (!paramType.IsGenericType)
				{
					if (paramType.IsAssignableFrom(argType)) continue;
					return;
				}
				var type0 = GetGenericType(paramType, argType);
				if (type0 == null) return;

				var paramGeneric = paramType.GetGenericTypeDefinition();
				var argGeneric = type0.GetGenericTypeDefinition();
				var paramArgs = paramType.GetGenericArguments();
				var argGenericArgs = type0.GetGenericArguments();
				for (int j = 0; j < paramArgs.Length && j < argGenericArgs.Length; j++)
				{
					if (paramArgs[j].IsGenericParameter && typeArguments[paramArgs[j].GenericParameterPosition] == null)
					{
						typeArgumentsFillCount++;
						typeArguments[paramArgs[j].GenericParameterPosition] = argGenericArgs[j];
					}
				}
				if (typeArgumentsFillCount == typeArguments.Length) break;
			}

			// 对于仍未确定的类型参数，默认 object
			for (int i = 0; i < typeArguments.Length; i++)
			{
				if (typeArguments[i] == null)
					typeArguments[i] = typeof(object);
			}

			// 第三步：创建具体化的泛型方法
			var concreteMethod = this.Method.MakeGenericMethod(typeArguments);
			parameters = concreteMethod.GetParameters();

			// 第四步：处理参数，构建 Expression<Func<,>> 如果需要
			var convertedArgs = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				var paramType = parameters[i].ParameterType;
				var argValue = argValues[i];

				if (!paramType.IsGenericType) continue;

				// 检查是否是 Expression<Func<,>>
				if (paramType.GetGenericTypeDefinition() == typeof(Expression<>))
				{
					if (!(e.Args[i] is DefineFuncNode defineFuncNode)) return;
					var innerType = paramType.GetGenericArguments()[0];
					if (innerType.IsGenericType && innerType.GetGenericTypeDefinition() == typeof(Func<,>))
					{
						// 需要构建 Expression<Func<T, bool>>
						// 从 innerType 获取 Func 的泛型参数
						var funcGenericArgs = innerType.GetGenericArguments();
						var elementType = funcGenericArgs[0];

						// 创建参数表达式
						var paramExpr = Expression.Parameter(elementType, defineFuncNode.Args[0].Name);

						// 构建临时上下文
						var tempBuildContext = new BuildContext
						{
							RewriteLocalVariables = false,
							ReturnType = funcGenericArgs.Length > 1 ? funcGenericArgs[1] : typeof(object),
							IsMain = true
						};
						tempBuildContext.Parameters[defineFuncNode.Args[0].Name] = paramExpr;

						// 构建函数体
						var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
						var body = defineFuncNode.Body.Build(tempBuildContext, e.Context, funcOptions);

						// 构建 Expression<Func<T, bool>>
						var lambdaExpr = Expression.Lambda(body, paramExpr);
						convertedArgs[i] = lambdaExpr;
						continue;
					}
					return;
				}

				// 普通参数处理
				if (argValue != null && !paramType.IsAssignableFrom(argValue.GetType()))
				{
					if (argValue is IQueryable && paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(IQueryable<>))
					{
						convertedArgs[i] = argValue;
					}
					else
					{
						try
						{
							convertedArgs[i] = Convert.ChangeType(argValue, paramType);
						}
						catch
						{
							// 类型转换失败，参数不匹配
							return;
						}
					}
				}
				else
				{
					convertedArgs[i] = argValue;
				}
			}

			var result = concreteMethod.Invoke(this.Target, convertedArgs);
			e.SetResult(result, concreteMethod.ReturnType);
		}
	}
}
