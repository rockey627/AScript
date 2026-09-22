using AScript.Exceptions;
using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class DefineFuncNode : TreeNode
	{
		public string Name { get; set; }
		public string ReturnType { get; set; }
		public Type ReturnSystemType { get; set; }
		public DefineVarNode[] Args { get; set; }
		public ITreeNode Body { get; set; }
		public Type DelegateType { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var funcReturnType = this.ReturnSystemType;
			if (funcReturnType == null && !string.IsNullOrEmpty(this.ReturnType))
			{
				funcReturnType = context.EvalType(this.ReturnType);
				if (funcReturnType == null)
				{
					throw new ScriptAnalyzingException($"unknown type {this.ReturnType}");
				}
			}
			var compileMode = (options ?? Script.DefaultOptions).CompileMode ?? ECompileMode.None;
			if ((compileMode & ECompileMode.Function) == ECompileMode.Function)
			{
				var tempBuildContext = new BuildContext
				{
					RewriteLocalVariables = false,
					ReturnType = funcReturnType,
					IsMain = true
				};
				if (this.Args != null)
				{
					for (int i = 0; i < this.Args.Length; i++)
					{
						var arg = this.Args[i];
						var type = arg.SystemType ?? context.EvalType(arg.Type);
						if (type == null)
						{
							throw new ScriptAnalyzingException($"unknown parameter type {arg.Type} in function {this.Name}");
						}
						tempBuildContext.Parameters[arg.Name] = Expression.Parameter(type, arg.Name);
					}
				}
				var funcOptions = new BuildOptions(options) { CompileMode = ECompileMode.All };
				var body = this.Body.Build(tempBuildContext, context, funcOptions);
				var func = tempBuildContext.Compile(context, options, body);
				returnType = func.GetType();
				if (!IsAnonymous(this.Name))
				{
					context.AddTempFunc(this.Name, func);
				}
				return func;
			}
			else
			{
				string[] argNames;
				Type[] argTypes;
				if (this.Args != null && this.Args.Length > 0)
				{
					argNames = new string[this.Args.Length];
					argTypes = new Type[this.Args.Length];
					for (int i = 0; i < this.Args.Length; i++)
					{
						var arg = this.Args[i];
						var type = arg.SystemType ?? context.EvalType(arg.Type);
						if (type == null)
						{
							throw new ScriptAnalyzingException($"unknown parameter type {arg.Type} in function {this.Name}");
						}
						argNames[i] = arg.Name;
						argTypes[i] = type;
					}
				}
				else
				{
					argNames = null;
					argTypes = null;
				}
				var customFunc = new CustomFunction(funcReturnType, argNames, argTypes, this.Body);
				if (!IsAnonymous(this.Name))
				{
					context.AddFunc(this.Name, customFunc);
				}
				returnType = typeof(CustomFunctionObject);
				return new CustomFunctionObject(customFunc, context);
			}
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var funcReturnType = this.ReturnSystemType;
			if (funcReturnType == null && !string.IsNullOrEmpty(this.ReturnType))
			{
				funcReturnType = scriptContext.EvalType(this.ReturnType);
				if (funcReturnType == null)
				{
					throw new ScriptAnalyzingException($"unknown type {this.ReturnType}");
				}
			}
			var tempBuildContext = new BuildContext(buildContext)
			{
				RewriteLocalVariables = false,
				ReturnType = funcReturnType,
				DelegateType = this.DelegateType,
				IsMain = true
			};
			Type[] argTypes = null;
			if (this.Args != null && this.Args.Length > 0)
			{
				argTypes = new Type[this.Args.Length];
				for (int i = 0; i < this.Args.Length; i++)
				{
					var arg = this.Args[i];
					var type = arg.SystemType ?? scriptContext.EvalType(arg.Type);
					if (type == null)
					{
						throw new ScriptAnalyzingException($"unknown parameter type {arg.Type} in function {this.Name}");
					}
					argTypes[i] = type;
					string argName = arg.Name;
					// 匿名参数名：_1、_2、_3 ...
					if (IsAnonymous(argName)) argName = "_" + i;
					tempBuildContext.Parameters[argName] = Expression.Parameter(type, argName);
				}
			}
			// 匿名函数不生成函数头定义
			bool isAnonymous = IsAnonymous(this.Name);
			var delegateDefine = isAnonymous ? null : buildContext.AddDelegateDefine(this.Name, argTypes, funcReturnType);
			var buildOptions = new BuildOptions(options) { UseCompletionResult = false };
			var body = this.Body.Build(tempBuildContext, scriptContext, buildOptions);
			// 如果函数未定义返回类型，但是有递归调用，此时无法自动根据函数体推导返回类型，强制定义为object类型
			if (funcReturnType == null && delegateDefine?.Variable != null)
			{
				tempBuildContext.ReturnType = typeof(object);
			}
			// 生成LambdaExpression
			var lambda = tempBuildContext.Build(scriptContext, buildOptions, body);
			if (isAnonymous) return lambda;
			// 将函数赋值给临时函数变量
			var tmpVar = delegateDefine?.Variable ?? Expression.Variable(lambda.Type);
			var assign = Expression.Assign(tmpVar, lambda);
			int hashCode = tmpVar.GetHashCode();
			string tmpVarName = hashCode > 0 ? $"<>$tmpVar_{hashCode}" : $"<>$tmpVar__{-hashCode}";
			buildContext.Variables[tmpVarName] = tmpVar;
			buildContext.LocalVariables.Add(tmpVarName);
			buildContext.PrevExpressions.Add(assign);
			// 添加到编译上下文
			buildContext.AddTempFunc(this.Name, tmpVar);
			// 回写到脚本上下文
			if (buildContext.RewriteLocalVariables && (options?.RewriteFunctions ?? true) && !(options?.Standalone ?? false))
			{
				var addTempFuncExpression = Expression.Call(
					buildContext.GetScriptContextParameter(),
					ScriptUtils.Method_ScriptContext_AddTempFunc,
					Expression.Constant(this.Name),
					tmpVar);
				return Expression.Block(addTempFuncExpression, tmpVar);
			}
			// 返回函数引用
			return tmpVar;
		}

		private static bool IsAnonymous(string name)
		{
			return string.IsNullOrEmpty(name) || name == "_";
		}

		//public override void Clear()
		//{
		//	base.Clear();

		//	PoolManage.Return(this.Args);
		//	//PoolManage.Return(this.Body);

		//	this.Name = null;
		//	this.ReturnType = null;
		//	this.ReturnSystemType = null;
		//	this.Args = null;
		//	this.Body = null;
		//}
	}
}
