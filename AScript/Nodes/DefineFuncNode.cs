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

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var funcReturnType = this.ReturnSystemType;
			if (funcReturnType == null && !string.IsNullOrEmpty(this.ReturnType))
			{
				funcReturnType = context.EvalType(this.ReturnType);
				if (funcReturnType == null)
				{
					throw new Exception($"unknown type {this.ReturnType}");
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
						var type = context.EvalType(arg.Type);
						if (type == null)
						{
							throw new Exception($"unknown parameter type {arg.Type} in function {this.Name}");
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
						var type = context.EvalType(arg.Type);
						if (type == null)
						{
							throw new Exception($"unknown parameter type {arg.Type} in function {this.Name}");
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
				string name = this.Name;
				if (this.Name == "_")
				{
					int hashCode = this.Body.GetHashCode();
					if (hashCode < 0)
					{
						name += "_" + (-hashCode);
					}
					else
					{
						name += hashCode;
					}
					name += DateTime.Now.ToString("HHmmssfff");
				}
				var customFunc = new CustomFunction(name, funcReturnType, argNames, argTypes, this.Body);
				if (!string.IsNullOrEmpty(this.Name) && this.Name != "_")
				{
					context.AddCustomFunc(customFunc);
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
					throw new Exception($"unknown type {this.ReturnType}");
				}
			}
			var tempBuildContext = new BuildContext(buildContext)
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
					var type = scriptContext.EvalType(arg.Type);
					if (type == null)
					{
						throw new Exception($"unknown parameter type {arg.Type} in function {this.Name}");
					}
					tempBuildContext.Parameters[arg.Name] = Expression.Parameter(type, arg.Name);
				}
			}
			//var buildOptions = new BuildOptions(options) { DynamicVariableType = true };
			var body = this.Body.Build(tempBuildContext, scriptContext, options);
			// 有闭包参数，只能通过DynamicInvoke调用，无法用Expression.Call调用
			//var d = tempBuildContext.Compile(scriptContext, body);
			//var dExpr = Expression.Constant(d);
			var d = tempBuildContext.Build(scriptContext, options, body);
			var dExpr = d;
			if (!string.IsNullOrEmpty(this.Name) && this.Name != "_")
			{
				buildContext.AddTempFunc(this.Name, d);
				// 将方法添加到上下文
				if (buildContext.RewriteLocalVariables && (options?.RewriteFunctions ?? true))
				{
					var addTempFuncExpression = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_AddTempFunc, Expression.Constant(this.Name), dExpr);
					return Expression.Block(addTempFuncExpression, dExpr);
				}
			}
			return dExpr;

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

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Args);
			PoolManage.Return(this.Body);

			this.Name = null;
			this.ReturnType = null;
			this.ReturnSystemType = null;
			this.Args = null;
			this.Body = null;
		}
	}
}
