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
					//ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
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
				if (!string.IsNullOrEmpty(this.Name) && this.Name != "_")
				{
					context.AddTempFunc(this.Name, func);
				}
				return func;
			}
			else
			{
				string[] argNames;// = new string[this.Args.Length];
				Type[] argTypes;// = new Type[this.Args.Length];
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
				//string name = this.Name;
				//if (this.Name == "_")
				//{
				//	int hashCode = this.Body.GetHashCode();
				//	if (hashCode < 0)
				//	{
				//		name += "_" + (-hashCode);
				//	}
				//	else
				//	{
				//		name += hashCode;
				//	}
				//	name += DateTime.Now.ToString("HHmmssfff");
				//}
				var customFunc = new CustomFunction(funcReturnType, argNames, argTypes, this.Body);
				if (!string.IsNullOrEmpty(this.Name) && this.Name != "_")
				{
					context.AddFunc(this.Name, customFunc);
				}
				returnType = typeof(CustomFunctionObject);
				return new CustomFunctionObject(customFunc, context);
				//var d = ExpressionUtils.CompileEval(context, name, argTypes, funcReturnType);
				//returnType = d.GetType();
				//return d;
			}
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			//// 构建临时上下文
			//var tempScriptContextExpression = Expression.Call(ExpressionUtils.Method_ScriptContext_Create2, buildContext.ScriptContextParameter ?? ExpressionUtils.Parameter_ScriptContext, ExpressionUtils.Constant_false);
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
				//ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
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
					if (argName == "_") argName += i;
					tempBuildContext.Parameters[argName] = Expression.Parameter(type, argName);
				}
			}
			var delegateDefine = IsNiming(this.Name) ? null : buildContext.AddDelegateDefine(this.Name, argTypes, funcReturnType);
			//var buildOptions = new BuildOptions(options) { DynamicVariableType = true };
			var body = this.Body.Build(tempBuildContext, scriptContext, options);
			// 有闭包参数，只能通过DynamicInvoke调用，无法用Expression.Call调用
			//var d = tempBuildContext.Compile(scriptContext, body);
			//var dExpr = Expression.Constant(d);
			if (funcReturnType == null && delegateDefine?.Variable != null)
			{
				tempBuildContext.ReturnType = typeof(object);
			}
			var lambda = tempBuildContext.Build(scriptContext, options, body);
			var tmpVar = delegateDefine?.Variable ?? Expression.Variable(lambda.Type);
			var assign = Expression.Assign(tmpVar, lambda);
			int hashCode = tmpVar.GetHashCode();
			string tmpVarName = hashCode > 0 ? $"<>$tmpVar_{hashCode}" : $"<>$tmpVar__{-hashCode}";
			buildContext.Variables[tmpVarName] = tmpVar;
			buildContext.LocalVariables.Add(tmpVarName);
			buildContext.PrevExpressions.Add(assign);
			//if (delegateDefine?.Variable != null)
			//{
				//var assignDefine = Expression.Assign(delegateDefine.Variable, lambda);
				//var ps1 = new ParameterExpression[lambda.Parameters.Count];
				//for (int i = 0; i < ps1.Length; i++)
				//{
				//	ps1[i] = Expression.Parameter(lambda.Parameters[i].Type);
				//}
				//var selfBlock = Expression.Block(new[] { delegateDefine.Variable }, 
				//	assignDefine, 
				//	Expression.Invoke(delegateDefine.Variable, ps1));
				//var newD = Expression.Lambda(delegateDefine.Variable.Type, selfBlock, ps1);
				//lambda = newD;
			//}
//#if NET45
//			// NET45框架下，如果Lambda有闭包参数直接Invoke会报错：System.Security.VerificationException:操作可能会破坏运行时稳定性
//			// 需要Expression.Quote来包装
//			Expression quoteExpr;
//			ParameterExpression[] ps;
//			if (lambda == null)
//			{
//				quoteExpr = null;
//				ps = null;
//			}
//			else
//			{
//				quoteExpr = Expression.Quote(lambda);
//				ps = new ParameterExpression[lambda.Parameters.Count];
//				for (int i = 0; i < ps.Length; i++)
//				{
//					ps[i] = Expression.Parameter(lambda.Parameters[i].Type);
//				}
//			}
//			var dExpr = tempBuildContext.DelegateType == null ?
//				Expression.Lambda(quoteExpr == null ? (Expression)Expression.Empty() : Expression.Invoke(quoteExpr, ps), ps) :
//				Expression.Lambda(tempBuildContext.DelegateType, Expression.Invoke(quoteExpr, ps), ps);
//#else
//			var dExpr = lambda;
//#endif
			if (!IsNiming(this.Name))
			{
				buildContext.AddTempFunc(this.Name, tmpVar);
				// 将方法添加到上下文
				if (buildContext.RewriteLocalVariables && (options?.RewriteFunctions ?? true) && !(options?.Standalone ?? false))
				{
					var addTempFuncExpression = Expression.Call(
						buildContext.GetScriptContextParameter(),
						ExpressionUtils.Method_ScriptContext_AddTempFunc,
						Expression.Constant(this.Name),
						tmpVar);
					return Expression.Block(addTempFuncExpression, tmpVar);
				}
			}
			return tmpVar;

			//return Expression.Constant(d);
			//var lambda = tempBuildContext.Build(scriptContext, body);
			//// 编译
			//var lambdaInstance = Expression.Constant(lambda);
			//var compileExpression = Expression.Call(lambdaInstance, ExpressionUtils.Method_LambdaExpression_Compile);
			//// 将方法赋值到临时变量
			//var tempResultVariable = Expression.Variable(compileExpression.Type);
			//var tempResultAssignExpression = Expression.Assign(tempResultVariable, compileExpression);
			//// 将方法添加到上下文
			//var addTempFuncExpression = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_AddTempFunc, Expression.Constant(this.Name), tempResultVariable);
			//buildContext.TempFunctions[this.Name] = tempResultVariable;
			//return Expression.Block(new[] { tempResultVariable }, tempResultAssignExpression, addTempFuncExpression, tempResultVariable); ;

			//return ExpressionUtils.BuildEval(buildContext, options, null, this);
		}

		private static bool IsNiming(string name)
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
