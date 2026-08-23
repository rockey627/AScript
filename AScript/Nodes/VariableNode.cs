using AScript.Exceptions;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace AScript.Nodes
{
	public class VariableNode : TreeNode
	{
		public string Name { get; set; }

		public VariableNode() { }
		public VariableNode(string name)
		{
			this.Name = name;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var value = context.EvalVar(this.Name, out returnType);
			if (returnType == null && context.HasFunc(this.Name))
			{
				value = new ScriptFunctionObject(context, this.Name);
				returnType = value.GetType();
				return value;
			}
			if (returnType == null && (options.ThrowIfVariableNotExists ?? false))
			{
				throw new ScriptAnalyzingException($"variable {this.Name} is not exists");
			}
			return value;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out _, out _, out var lastType))
			{
				if (lastType == null || lastType == varExpr.Type) return varExpr;
				if (lastType == typeof(object)) return varExpr;
				// 基本类型不转换
				if (Type.GetTypeCode(lastType) != TypeCode.Object) return varExpr;
				// 对象类型要转换
				return Expression.Convert(varExpr, lastType);
				//return varExpr;
			}
			var value = scriptContext.EvalVar(this.Name, out var type);
			if (type == null)
			{
				if (buildContext.HasFunc(this.Name) || scriptContext.HasFunc(this.Name))
				{
					return Expression.Constant(new ScriptFunctionObject(scriptContext, this.Name));
				}
				if (options.ThrowIfVariableNotExists ?? false)
				{
					throw new ScriptAnalyzingException($"variable {this.Name} is not exists");
				}
				type = typeof(object);
			}
			if (type == typeof(TypeWrapper))
			{
				return Expression.Constant(value);
			}
			else
			{
				if (type == typeof(object) && value != null)
				{
					type = value.GetType();
				}
				var mainBuildContext = buildContext.Main;
				varExpr = Expression.Variable(type, this.Name);
				if (options.Standalone ?? false)
				{
					var assign = Expression.Assign(varExpr, Expression.Constant(value, type));
					mainBuildContext.PrevExpressions.Add(assign);
					mainBuildContext.LocalVariables.Add(this.Name);
				}
				else
				{
					// 从ScriptContext中取值
					var call = BuildCallEvalVarExpression(mainBuildContext.Root, this.Name, type);
					var assign = Expression.Assign(varExpr, call);
					mainBuildContext.PrevExpressions.Add(assign);
				}
				mainBuildContext.Variables[this.Name] = varExpr;
				return varExpr;
			}
		}

		private static Expression BuildCallEvalVarExpression(BuildContext buildContext, string name, Type type)
		{
			if (type == null || type == typeof(object))
			{
				return Expression.Call(buildContext.GetScriptContextParameter(), ScriptUtils.Method_ScriptContext_EvalVar, Expression.Constant(name));
			}
			var method = ScriptUtils.Make_ScriptContext_EvalVarT_Method(type);
			return Expression.Call(buildContext.GetScriptContextParameter(), method, Expression.Constant(name));
		}

		public ParameterExpression BuildForAssign(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, out BuildContext ownerBuildContext, out Type lastType)
		{
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out ownerBuildContext, out _, out lastType))
			{
				ownerBuildContext.ThrowIfReadOnly(this.Name);
				ownerBuildContext.ChangedVariables.Add(this.Name);
				return varExpr;
			}
			var mainBuildContext = buildContext.Main;
			// 是否在执行上下文中存在变量
			var ownerContext = scriptContext.GetOwnerContext(this.Name, out var value, out var type, out int modifier);
			if (type == null)
			{
				scriptContext.EvalVarFromLangs(this.Name, out type, out modifier);
			}
			if (ownerContext == null)
			{
				buildContext.LocalVariables.Add(this.Name);
			}
			else
			{
				Modifiers.ThrowIfReadOnly(this.Name, modifier);
				// 标记变量有变化
				mainBuildContext.ChangedVariables.Add(this.Name);
			}
			if (type == null)
			{
				//if (options.ThrowIfVariableNotExists ?? false)
				//{
				//	throw new ScriptAnalyzingException($"variable {this.Name} is not exists");
				//}
				//type = typeof(object);
				return null;
			}
			varExpr = Expression.Variable(type, this.Name);
			// 从ScriptContext中取值
			//var call = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_EvalVar, Expression.Constant(this.Name));
			//var assign = Expression.Assign(varExpr, Expression.Convert(call, type));
			if (options.Standalone ?? false)
			{
				var assign = Expression.Assign(varExpr, Expression.Constant(value, type));
				mainBuildContext.PrevExpressions.Add(assign);
				mainBuildContext.LocalVariables.Add(this.Name);
			}
			else
			{
				var call = BuildCallEvalVarExpression(mainBuildContext.Root, this.Name, type);
				var assign = Expression.Assign(varExpr, call);
				mainBuildContext.PrevExpressions.Add(assign);
			}
			mainBuildContext.Variables[this.Name] = varExpr;
			return varExpr;
		}

		public override void Clear()
		{
			base.Clear();

			this.Name = null;
		}
	}
}
