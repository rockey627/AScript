using System;
using System.Collections;
using System.Collections.Generic;
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
			var parameters = this.Method.GetParameters();
			int argsCount = e.GetArgsCount();
			if (argsCount != parameters.Length) return;

			// 第一步：获取所有参数的表达式类型
			var argExpressions = new Expression[argsCount];
			var argTypes = new Type[argsCount];
			for (int i = 0; i < argsCount; i++)
			{
				if (e.Args != null && e.Args[i] is DefineFuncNode)
				{
					//argTypes[i] = typeof(Delegate);
				}
				else
				{
					argExpressions[i] = e.BuildArgs(i);
					argTypes[i] = argExpressions[i].Type;
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

				if (!paramType.IsGenericType && !paramType.IsGenericParameter)
				{
					if (e.Args != null && e.Args[i] is DefineFuncNode) return;
					if (paramType.IsAssignableFrom(argType)) continue;
					return;
				}

				if (paramType.IsGenericParameter)
				{
					if (e.Args != null && e.Args[i] is DefineFuncNode) return;
					if (typeArguments[paramType.GenericParameterPosition] == null)
					{
						typeArgumentsFillCount++;
						typeArguments[paramType.GenericParameterPosition] = argType;
					}
					continue;
				}

				var paramGeneric = paramType.GetGenericTypeDefinition();
				if (paramGeneric == typeof(Expression<>))
				{
					if (!(e.Args[i] is DefineFuncNode defineFuncNode)) return;
					var innerType = paramType.GetGenericArguments()[0];
					var innerDefinition = innerType.GetGenericTypeDefinition();
					if (innerType.IsGenericType)
					{
						// 比如：Expression<Func<TSource, TKey>>，TSource已由前面的参数推导出来，TKey类型由defineFuncNode实际返回值来推导
						var innerGens = innerType.GetGenericArguments();
						int innerArgsCount = innerGens.Length;
						if (innerType.Name.StartsWith("Func`")) innerArgsCount -= 1;
						int defineArgsCount = defineFuncNode.Args == null ? 0 : defineFuncNode.Args.Length;
						if (innerArgsCount != defineArgsCount) return;
						var types = new Type[innerArgsCount];
						for (int j = 0; j < innerArgsCount; j++)
						{
							var g = innerGens[j];
							Type type;
							if (g.IsGenericParameter)
							{
								type = typeArguments[g.GenericParameterPosition];
								if (type == null) return;
							}
							else type = g;
							types[j] = type;
						}
						// 构建临时上下文
						var tempBuildContext = new BuildContext
						{
							RewriteLocalVariables = false,
							IsMain = true
						};
						var paramExprs = new ParameterExpression[types.Length];
						for (int j = 0; j < types.Length; j++)
						{
							// 创建参数表达式
							var defineArgName = defineFuncNode.Args[j].Name;
							var paramExpr = Expression.Parameter(types[j], defineArgName);
							tempBuildContext.Parameters[defineArgName] = paramExpr;
							paramExprs[j] = paramExpr;
						}
						// 构建函数体
						var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
						var body = defineFuncNode.Body.Build(tempBuildContext, e.ScriptContext, funcOptions);
						// 构建 Expression<Func<T, bool>>
						var returnGen = innerGens[innerGens.Length - 1];
						if (returnGen.IsGenericType)
						{
							var returnType0 = GetGenericType(returnGen, body.Type);
							if (returnType0 == null) return;
							var returnParamArgs = returnGen.GetGenericArguments();
							var returnArgGenericArgs = returnType0.GetGenericArguments();
							for (int j = 0; j < returnParamArgs.Length && j < returnArgGenericArgs.Length; j++)
							{
								var p = returnParamArgs[j];
								if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
								{
									typeArgumentsFillCount++;
									typeArguments[p.GenericParameterPosition] = returnArgGenericArgs[j];
								}
							}
						}
						else if (!returnGen.IsGenericParameter)
						{
							if (!returnGen.IsAssignableFrom(body.Type)) return;
							if (returnGen != body.Type)
							{
								tempBuildContext.ReturnType = returnGen;
							}
						}
						var lambdaExpr = tempBuildContext.Build(e.ScriptContext, funcOptions, body);
						argExpressions[i] = lambdaExpr;
						if (returnGen.IsGenericParameter)
						{
							if (typeArguments[returnGen.GenericParameterPosition] == null)
							{
								typeArgumentsFillCount++;
								typeArguments[returnGen.GenericParameterPosition] = lambdaExpr.ReturnType;
							}
						}
						continue;
					}
					return;
				}

				if (paramType.Name.StartsWith("Func`") || paramType.Name == "Action" || paramType.Name.StartsWith("Action`"))
				{
					if (!(e.Args[i] is DefineFuncNode defineFuncNode)) return;
					// 比如：Func<TSource, TKey>，TSource已由前面的参数推导出来，TKey类型由defineFuncNode实际返回值来推导
					var innerGens = paramType.GetGenericArguments();
					int innerArgsCount = innerGens.Length;
					if (paramType.Name.StartsWith("Func`")) innerArgsCount -= 1;
					int defineArgsCount = defineFuncNode.Args == null ? 0 : defineFuncNode.Args.Length;
					if (innerArgsCount != defineArgsCount) return;
					var types = new Type[innerArgsCount];
					for (int j = 0; j < innerArgsCount; j++)
					{
						var g = innerGens[j];
						Type type;
						if (g.IsGenericParameter)
						{
							type = typeArguments[g.GenericParameterPosition];
							if (type == null) return;
						}
						else type = g;
						types[j] = type;
					}
					// 构建临时上下文
					var tempBuildContext = new BuildContext
					{
						RewriteLocalVariables = false,
						IsMain = true
					};
					var paramExprs = new ParameterExpression[types.Length];
					for (int j = 0; j < types.Length; j++)
					{
						// 创建参数表达式
						var defineArgName = defineFuncNode.Args[j].Name;
						var paramExpr = Expression.Parameter(types[j], defineArgName);
						tempBuildContext.Parameters[defineArgName] = paramExpr;
						paramExprs[j] = paramExpr;
					}
					// 构建函数体
					var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
					var body = defineFuncNode.Body.Build(tempBuildContext, e.ScriptContext, funcOptions);
					// 构建 Expression<Func<T, bool>>
					var returnGen = innerGens[innerGens.Length - 1];
					if (returnGen.IsGenericType)
					{
						var returnType0 = GetGenericType(returnGen, body.Type);
						if (returnType0 == null) return;
						var returnParamArgs = returnGen.GetGenericArguments();
						var returnArgGenericArgs = returnType0.GetGenericArguments();
						for (int j = 0; j < returnParamArgs.Length && j < returnArgGenericArgs.Length; j++)
						{
							var p = returnParamArgs[j];
							if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
							{
								typeArgumentsFillCount++;
								typeArguments[p.GenericParameterPosition] = returnArgGenericArgs[j];
							}
						}
					}
					else if (!returnGen.IsGenericParameter)
					{
						if (!returnGen.IsAssignableFrom(body.Type)) return;
						if (returnGen != body.Type)
						{
							tempBuildContext.ReturnType = returnGen;
						}
					}
					var lambdaExpr = tempBuildContext.Build(e.ScriptContext, funcOptions, body);
					argExpressions[i] = lambdaExpr;
					if (returnGen.IsGenericParameter)
					{
						if (typeArguments[returnGen.GenericParameterPosition] == null)
						{
							typeArgumentsFillCount++;
							typeArguments[returnGen.GenericParameterPosition] = lambdaExpr.ReturnType;
						}
					}
					continue;
				}

				if (e.Args != null && e.Args[i] is DefineFuncNode) return;

				var type0 = GetGenericType(paramType, argType);
				if (type0 == null) return;

				var paramGenericArgs = paramType.GetGenericArguments();
				var argGenericArgs = type0.GetGenericArguments();
				for (int j = 0; j < paramGenericArgs.Length && j < argGenericArgs.Length; j++)
				{
					var p = paramGenericArgs[j];
					if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
					{
						typeArgumentsFillCount++;
						typeArguments[p.GenericParameterPosition] = argGenericArgs[j];
					}
				}
			}

			if (typeArgumentsFillCount < typeArguments.Length) return;

			// 第三步：创建具体化的泛型方法
			var concreteMethod = this.Method.MakeGenericMethod(typeArguments);

			// 第五步：构建方法调用表达式
			var resultExpr = Expression.Call(this.Target != null ? Expression.Constant(this.Target) : null, concreteMethod, argExpressions);
			e.Result = resultExpr;
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
					//argTypes[i] = typeof(Delegate);
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

				if (!paramType.IsGenericType && !paramType.IsGenericParameter)
				{
					if (e.Args[i] is DefineFuncNode) return;
					if (paramType.IsAssignableFrom(argType)) continue;
					return;
				}

				if (paramType.IsGenericParameter)
				{
					if (e.Args[i] is DefineFuncNode) return;
					if (typeArguments[paramType.GenericParameterPosition] == null)
					{
						typeArgumentsFillCount++;
						typeArguments[paramType.GenericParameterPosition] = argType;
					}
					continue;
				}

				var paramGeneric = paramType.GetGenericTypeDefinition();
				if (paramGeneric == typeof(Expression<>))
				{
					if (!(e.Args[i] is DefineFuncNode defineFuncNode)) return;
					var innerType = paramType.GetGenericArguments()[0];
					if (innerType.IsGenericType)
					{
						// 比如：Expression<Func<TSource, TKey>>，TSource已由前面的参数推导出来，TKey类型由defineFuncNode实际返回值来推导
						var innerGens = innerType.GetGenericArguments();
						int innerArgsCount = innerGens.Length;
						if (innerType.Name.StartsWith("Func`")) innerArgsCount -= 1;
						int defineArgsCount = defineFuncNode.Args == null ? 0 : defineFuncNode.Args.Length;
						if (innerArgsCount != defineArgsCount) return;
						var types = new Type[innerArgsCount];
						for (int j = 0; j < innerArgsCount; j++)
						{
							var g = innerGens[j];
							Type type;
							if (g.IsGenericParameter)
							{
								type = typeArguments[g.GenericParameterPosition];
								if (type == null) return;
							}
							else type = g;
							types[j] = type;
						}
						// 构建临时上下文
						var tempBuildContext = new BuildContext
						{
							RewriteLocalVariables = false,
							IsMain = true
						};
						var paramExprs = new ParameterExpression[types.Length];
						for (int j = 0; j < types.Length; j++)
						{
							// 创建参数表达式
							var defineArgName = defineFuncNode.Args[j].Name;
							var paramExpr = Expression.Parameter(types[j], defineArgName);
							tempBuildContext.Parameters[defineArgName] = paramExpr;
							paramExprs[j] = paramExpr;
						}
						// 构建函数体
						var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
						var body = defineFuncNode.Body.Build(tempBuildContext, e.Context, funcOptions);
						// 构建 Expression<Func<T, bool>>
						var returnGen = innerGens[innerGens.Length - 1];
						if (returnGen.IsGenericType)
						{
							var returnType0 = GetGenericType(returnGen, body.Type);
							if (returnType0 == null) return;
							var returnParamArgs = returnGen.GetGenericArguments();
							var returnArgGenericArgs = returnType0.GetGenericArguments();
							for (int j = 0; j < returnParamArgs.Length && j < returnArgGenericArgs.Length; j++)
							{
								var p = returnParamArgs[j];
								if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
								{
									typeArgumentsFillCount++;
									typeArguments[p.GenericParameterPosition] = returnArgGenericArgs[j];
								}
							}
						}
						else if (!returnGen.IsGenericParameter)
						{
							if (!returnGen.IsAssignableFrom(body.Type)) return;
							if (returnGen != body.Type)
							{
								tempBuildContext.ReturnType = returnGen;
							}
						}
						var lambdaExpr = tempBuildContext.Build(e.Context, funcOptions, body);
						argValues[i] = lambdaExpr;
						if (returnGen.IsGenericParameter)
						{
							if (typeArguments[returnGen.GenericParameterPosition] == null)
							{
								typeArgumentsFillCount++;
								typeArguments[returnGen.GenericParameterPosition] = lambdaExpr.ReturnType;
							}
						}
						continue;
					}
					return;
				}

				if (paramType.Name.StartsWith("Func`") || paramType.Name == "Action" || paramType.Name.StartsWith("Action`"))
				{
					if (!(e.Args[i] is DefineFuncNode defineFuncNode)) return;
					// 比如：Func<TSource, TKey>，TSource已由前面的参数推导出来，TKey类型由defineFuncNode实际返回值来推导
					var innerGens = paramType.GetGenericArguments();
					int innerArgsCount = innerGens.Length;
					if (paramType.Name.StartsWith("Func`")) innerArgsCount -= 1;
					int defineArgsCount = defineFuncNode.Args == null ? 0 : defineFuncNode.Args.Length;
					if (innerArgsCount != defineArgsCount) return;
					var types = new Type[innerArgsCount];
					for (int j = 0; j < innerArgsCount; j++)
					{
						var g = innerGens[j];
						Type type;
						if (g.IsGenericParameter)
						{
							type = typeArguments[g.GenericParameterPosition];
							if (type == null) return;
						}
						else type = g;
						types[j] = type;
					}
					// 构建临时上下文
					var tempBuildContext = new BuildContext
					{
						RewriteLocalVariables = false,
						IsMain = true
					};
					var paramExprs = new ParameterExpression[types.Length];
					for (int j = 0; j < types.Length; j++)
					{
						// 创建参数表达式
						var defineArgName = defineFuncNode.Args[j].Name;
						var paramExpr = Expression.Parameter(types[j], defineArgName);
						tempBuildContext.Parameters[defineArgName] = paramExpr;
						paramExprs[j] = paramExpr;
					}
					// 构建函数体
					var funcOptions = new BuildOptions(e.Options) { CompileMode = ECompileMode.All };
					var body = defineFuncNode.Body.Build(tempBuildContext, e.Context, funcOptions);
					// 构建 Func<T, bool>
					var returnGen = innerGens[innerGens.Length - 1];
					if (returnGen.IsGenericType)
					{
						var returnType0 = GetGenericType(returnGen, body.Type);
						if (returnType0 == null) return;
						var returnParamArgs = returnGen.GetGenericArguments();
						var returnArgGenericArgs = returnType0.GetGenericArguments();
						for (int j = 0; j < returnParamArgs.Length && j < returnArgGenericArgs.Length; j++)
						{
							var p = returnParamArgs[j];
							if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
							{
								typeArgumentsFillCount++;
								typeArguments[p.GenericParameterPosition] = returnArgGenericArgs[j];
							}
						}
					}
					else if (!returnGen.IsGenericParameter)
					{
						if (!returnGen.IsAssignableFrom(body.Type)) return;
						if (returnGen != body.Type)
						{
							tempBuildContext.ReturnType = returnGen;
						}
					}
					var lambdaExpr = tempBuildContext.Build(e.Context, funcOptions, body);
					argValues[i] = lambdaExpr.Compile();
					if (returnGen.IsGenericParameter)
					{
						if (typeArguments[returnGen.GenericParameterPosition] == null)
						{
							typeArgumentsFillCount++;
							typeArguments[returnGen.GenericParameterPosition] = lambdaExpr.ReturnType;
						}
					}
					continue;
				}

				if (e.Args[i] is DefineFuncNode) return;

				var type0 = GetGenericType(paramType, argType);
				if (type0 == null) return;

				var paramArgs = paramType.GetGenericArguments();
				var argGenericArgs = type0.GetGenericArguments();
				for (int j = 0; j < paramArgs.Length && j < argGenericArgs.Length; j++)
				{
					var p = paramArgs[j];
					if (p.IsGenericParameter && typeArguments[p.GenericParameterPosition] == null)
					{
						typeArgumentsFillCount++;
						typeArguments[p.GenericParameterPosition] = argGenericArgs[j];
					}
				}
			}

			if (typeArgumentsFillCount < typeArguments.Length) return;

			// 第三步：创建具体化的泛型方法
			var concreteMethod = this.Method.MakeGenericMethod(typeArguments);

			var result = concreteMethod.Invoke(this.Target, argValues);
			e.SetResult(result, concreteMethod.ReturnType);
		}

		private static Type GetGenericType(Type paremterType, Type argType)
		{
			if (argType.IsArray)
			{
				argType = typeof(IList<>).MakeGenericType(argType.GetElementType());
			}
			var type = GetGenericType0(paremterType, argType);
			if (type == null)
			{
				type = GetGenericType1(paremterType, argType);
			}
			return type;
		}

		private static Type GetGenericType0(Type paremterType, Type argType)
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

		private static Type GetGenericType1(Type paremterType, Type argType)
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
	}
}
