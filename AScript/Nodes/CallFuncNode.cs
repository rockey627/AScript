using AScript.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	/// <summary>
	/// 函数调用节点
	/// </summary>
	public class CallFuncNode : TreeNode
	{
		public string Name { get; set; }
		public MethodInfo Method { get; set; }
		public object Target { get; set; }
		public ITreeNode[] Args { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				if (this.Method != null)
				{
					object[] args = null;
					if (this.Args != null && this.Args.Length > 0)
					{
						args = new object[this.Args.Length];
						for (int i = 0; i < this.Args.Length; i++)
						{
							var arg = this.Args[i];
							if (arg == null)
							{
								args[i] = null;
							}
							else if (arg is ObjectNode objNode)
							{
								args[i] = objNode.Data;
							}
							else
							{
								args[i] = arg.Eval(context, options, control, out _);
							}
						}
					}
					returnType = this.Method.ReturnType;
					return this.Method.Invoke(this.Target, args);
				}
			}
			else if (this.Target != null)
			{
				object[] argValues;
				Type[] argTypes;
				if (this.Args == null || this.Args.Length == 0)
				{
					argValues = null;
					argTypes = Type.EmptyTypes;
				}
				else
				{
					argValues = new object[this.Args.Length];
					argTypes = new Type[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						var arg = this.Args[i];
						if (arg is DefineFuncNode)
						{
							argValues[i] = arg;
							argTypes[i] = typeof(Delegate);
						}
						else
						{
							argValues[i] = this.Args[i].Eval(context, options, null, out var argType);
							argTypes[i] = argType;
						}
					}
				}
				var v0 = ((ITreeNode)this.Target).Eval(context, options, null, out var t0);
				if (t0 == typeof(TypeWrapper))
				{
					var type = ((TypeWrapper)v0).Type;
					var methodInfo = type.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {type}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertObjectArguments(argValues, parameters);
					var result = methodInfo.Invoke(null, convertedArgs);
					returnType = methodInfo.ReturnType;
					return result;
				}
				else
				{
					var methodInfo = t0.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						var argTypes2 = new Type[argTypes.Length + 1];
						argTypes2[0] = typeof(ScriptContext);
						Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						methodInfo = t0.GetMethod(this.Name, argTypes2);
						if (methodInfo != null)
						{
							argTypes = argTypes2;
							var argValues2 = new object[argValues == null ? 1 : argValues.Length + 1];
							argValues2[0] = context;
							if (argValues != null && argValues.Length > 0)
							{
								Array.Copy(argValues, 0, argValues2, 1, argValues.Length);
							}
							argValues = argValues2;
						}
					}
					if (methodInfo == null)
					{
						var argValues2 = new object[argValues == null ? 1 : argValues.Length + 1];
						argValues2[0] = v0;
						if (argValues != null && argValues.Length > 0)
						{
							Array.Copy(argValues, 0, argValues2, 1, argValues.Length);
						}
						var argTypes2 = new Type[argTypes == null ? 1 : argTypes.Length + 1];
						argTypes2[0] = t0;
						if (argTypes != null && argTypes.Length > 0)
						{
							Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						}
						try
						{
							var result2 = context.EvalFunc(options, this.Name, false, argValues2, argTypes2, out var returnType2);
							if (returnType2 != null)
							{
								returnType = returnType2;
								return result2;
							}
						}
						catch (ScriptException ex)
						{
							throw;
						}
						catch (Exception ex)
						{
							throw new ScriptRuntimeException(ex.Message, ex);
						}
					}
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {t0}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertObjectArguments(argValues, parameters);
					var result = methodInfo.Invoke(v0, convertedArgs);
					returnType = methodInfo.ReturnType;
					return result;
				}
			}
			else
			{
				ITreeNode[] args = null;
				if (this.Args != null && this.Args.Length > 0)
				{
					args = new ITreeNode[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						args[i] = this.Args[i];
						//var arg = this.Args[i];
						//if (arg == null || arg is ObjectNode)
						//{
						//	args[i] = arg;
						//}
						//else
						//{
						//	var v = arg.Eval(context, options, control, out var type);
						//	args[i] = PoolManage.CreateObjectNode(v, type);
						//}
					}
				}
				var tmpContext = ScriptContext.Create(context);
				return tmpContext.EvalFunc(options, control, this.Name, args, out returnType);
			}
			returnType = null;
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				if (this.Method != null)
				{
					Expression[] argExprs = null;
					if (this.Args != null && this.Args.Length > 0)
					{
						argExprs = new Expression[this.Args.Length];
						for (int i = 0; i < this.Args.Length; i++)
						{
							var arg = this.Args[i];
							if (arg == null)
							{
								argExprs[i] = ExpressionUtils.Constant_null;
							}
							else if (arg is ExpressionNode exprNode)
							{
								argExprs[i] = exprNode.Expr;
							}
							else
							{
								argExprs[i] = arg.Build(buildContext, scriptContext, options);
							}
						}
					}
					return this.Target == null ?
						Expression.Call(this.Method, argExprs) :
						Expression.Call(Expression.Constant(this.Target), this.Method, argExprs);
				}
			}
			else if (this.Target != null)
			{
				Expression[] argExprs;
				Type[] argTypes;
				if (this.Args == null)
				{
					argExprs = null;
#if NETSTANDARD
					argTypes = Array.Empty<Type>();
#else
					argTypes = new Type[0];
#endif
				}
				else
				{
					argExprs = new Expression[this.Args.Length];
					argTypes = new Type[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						var arg = this.Args[i];
						if (arg is DefineFuncNode)
						{
							argTypes[i] = typeof(Delegate);
						}
						else
						{
							var argExpression = this.Args[i].Build(buildContext, scriptContext, options);
							argExprs[i] = argExpression;
							argTypes[i] = argExpression.Type;
						}
					}
				}
				var v0 = ((ITreeNode)this.Target).Build(buildContext, scriptContext, options);
				if (v0.Type == typeof(TypeWrapper))
				{
					var type = ((TypeWrapper)((ConstantExpression)v0).Value).Type;
					var methodInfo = type.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {type}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertArguments(argExprs, parameters);
					return Expression.Call(null, methodInfo, convertedArgs);
				}
				else
				{
					var methodInfo = v0.Type.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						var argTypes2 = new Type[argTypes.Length + 1];
						argTypes2[0] = typeof(ScriptContext);
						Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						methodInfo = v0.Type.GetMethod(this.Name, argTypes2);
						if (methodInfo != null)
						{
							argTypes = argTypes2;
							Expression[] argExpressions2;
							if (argExprs == null || argExprs.Length == 0)
							{
								argExpressions2 = new Expression[1];
							}
							else
							{
								argExpressions2 = new Expression[argExprs.Length + 1];
								Array.Copy(argExprs, 0, argExpressions2, 1, argExprs.Length);
							}
							argExpressions2[0] = buildContext.GetScriptContextParameter();
							argExprs = argExpressions2;
						}
					}
					if (methodInfo == null)
					{
						var argValues2 = new Expression[argExprs == null ? 1 : argExprs.Length + 1];
						argValues2[0] = v0;
						if (argExprs != null && argExprs.Length > 0)
						{
							Array.Copy(argExprs, 0, argValues2, 1, argExprs.Length);
						}
						var argTypes2 = new Type[argTypes == null ? 1 : argTypes.Length + 1];
						argTypes2[0] = v0.Type;
						if (argTypes != null && argTypes.Length > 0)
						{
							Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						}
						try
						{
							ITreeNode[] args = null;
							if (this.Args != null && this.Args.Length > 0)
							{
								args = new ITreeNode[this.Args.Length + 1];
								Array.Copy(this.Args, 0, args, 1, this.Args.Length);
							}
							var expr2 = scriptContext.BuildFunc(buildContext, options, null, this.Name, false, args, argValues2, buildEvalEnabled: false);
							if (expr2 != null)
							{
								return expr2;
							}
						}
						catch (ScriptException ex)
						{
							throw;
						}
						catch { }
					}
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {v0.Type}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertArguments(argExprs, parameters, expressionStartIndex: methodInfo.IsStatic ? 1 : 0);
					if (methodInfo.IsStatic) convertedArgs[0] = v0;
					return Expression.Call(methodInfo.IsStatic ? null : v0, methodInfo, convertedArgs);
				}
			}
			else
			{
				ITreeNode[] args = null;
				if (this.Args != null && this.Args.Length > 0)
				{
					args = new ITreeNode[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						args[i] = this.Args[i];
						//var arg = this.Args[i];
						//if (arg == null || arg is ExpressionNode)
						//{
						//	args[i] = arg;
						//}
						//else
						//{
						//	var v = arg.Build(buildContext, scriptContext, options);
						//	args[i] = PoolManage.CreateExpressionNode(v);
						//}
					}
				}
				return scriptContext.BuildFunc(buildContext, options, null, this.Name, false, args);
				//return ExpressionUtils.BuildCall(buildContext, scriptContext, options, this.Name, this.Args);
			}
			return null;
		}

		public override async Task<EvalResult> Eval2Async(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				if (this.Method != null)
				{
					object[] args = null;
					if (this.Args != null && this.Args.Length > 0)
					{
						args = new object[this.Args.Length];
						for (int i = 0; i < this.Args.Length; i++)
						{
							var arg = this.Args[i];
							if (arg == null)
							{
								args[i] = null;
							}
							else if (arg is ObjectNode objNode)
							{
								args[i] = objNode.Data;
							}
							else
							{
								args[i] = await arg.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
							}
						}
					}
					var returnType = this.Method.ReturnType;
					var result = this.Method.Invoke(this.Target, args);
					return new EvalResult(result, returnType);
				}
			}
			else if (this.Target != null)
			{
				object[] argValues;
				Type[] argTypes;
				if (this.Args == null || this.Args.Length == 0)
				{
					argValues = null;
					argTypes = Type.EmptyTypes;
				}
				else
				{
					argValues = new object[this.Args.Length];
					argTypes = new Type[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						var arg = this.Args[i];
						if (arg is DefineFuncNode)
						{
							argValues[i] = arg;
							argTypes[i] = typeof(Delegate);
						}
						else
						{
							var argResult = await this.Args[i].Eval2Async(context, options, null, cancellationToken).ConfigureAwait(false);
							argValues[i] = argResult.Value;
							argTypes[i] = argResult.Type;
						}
					}
				}
				var targetResult = await ((ITreeNode)this.Target).Eval2Async(context, options, null, cancellationToken).ConfigureAwait(false);
				var v0 = targetResult.Value;
				var t0 = targetResult.Type;
				if (t0 == typeof(TypeWrapper))
				{
					var type = ((TypeWrapper)v0).Type;
					var methodInfo = type.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {type}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertObjectArguments(argValues, parameters);
					var result = methodInfo.Invoke(null, convertedArgs);
					return new EvalResult(result, methodInfo.ReturnType);
				}
				else
				{
					var methodInfo = t0.GetMethod(this.Name, argTypes);
					if (methodInfo == null)
					{
						var argTypes2 = new Type[argTypes.Length + 1];
						argTypes2[0] = typeof(ScriptContext);
						Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						methodInfo = t0.GetMethod(this.Name, argTypes2);
						if (methodInfo != null)
						{
							argTypes = argTypes2;
							var argValues2 = new object[argValues == null ? 1 : argValues.Length + 1];
							argValues2[0] = context;
							if (argValues != null && argValues.Length > 0)
							{
								Array.Copy(argValues, 0, argValues2, 1, argValues.Length);
							}
							argValues = argValues2;
						}
					}
					if (methodInfo == null)
					{
						var argValues2 = new object[argValues == null ? 1 : argValues.Length + 1];
						argValues2[0] = v0;
						if (argValues != null && argValues.Length > 0)
						{
							Array.Copy(argValues, 0, argValues2, 1, argValues.Length);
						}
						var argTypes2 = new Type[argTypes == null ? 1 : argTypes.Length + 1];
						argTypes2[0] = t0;
						if (argTypes != null && argTypes.Length > 0)
						{
							Array.Copy(argTypes, 0, argTypes2, 1, argTypes.Length);
						}
						try
						{
							var result2 = context.EvalFunc(options, this.Name, false, argValues2, argTypes2, out var returnType2);
							if (returnType2 != null)
							{
								return new EvalResult(result2, returnType2);
							}
						}
						catch (ScriptException ex)
						{
							throw;
						}
						catch (Exception ex)
						{
							throw new ScriptRuntimeException(ex.Message, ex);
						}
					}
					if (methodInfo == null)
					{
						throw new ScriptAnalyzingException($"unknown function: {t0}.{this.Name}({string.Join(", ", argTypes.Select(t => t?.Name))})");
					}
					var parameters = methodInfo.GetParameters();
					var convertedArgs = Syntaxs.DefaultSyntaxAnalyzer.ConvertObjectArguments(argValues, parameters);
					var result = methodInfo.Invoke(v0, convertedArgs);
					return new EvalResult(result, methodInfo.ReturnType);
				}
			}
			else
			{
				ITreeNode[] args = null;
				if (this.Args != null && this.Args.Length > 0)
				{
					args = new ITreeNode[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						args[i] = this.Args[i];
					}
				}
				var tmpContext = ScriptContext.Create(context);
				return await tmpContext.EvalFunc2Async(options, control, this.Name, args, cancellationToken).ConfigureAwait(false);
			}
			return new EvalResult();
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Args);

			this.Name = null;
			this.Method = null;
			this.Target = null;
			this.Args = null;
		}
	}
}
