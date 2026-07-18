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
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out _, out bool outer, out var lastType))
			{
				//if (outer)
				//{
				//	// 跨函数，需要定义临时变量从ScriptContext上下文获取
				//	var call = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_EvalVar, Expression.Constant(this.Name));
				//	// 赋值v变量
				//	var v = Expression.Convert(call, varExpr.Type);
				//	var assign = Expression.Assign(varExpr, v);
				//	buildContext.Variables[this.Name] = varExpr;
				//	buildContext.PrevExpressions.Add(assign);
				//}
				if (lastType == null) return varExpr;
				return Expression.Convert(varExpr, lastType);
			}
			//scriptContext.GetOwnerContext(this.Name, out var value, out var type, true);
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
				varExpr = Expression.Variable(type, this.Name);
				// 从ScriptContext中取值
				var call = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_EvalVar, Expression.Constant(this.Name));
				var assign = Expression.Assign(varExpr, Expression.Convert(call, type));
				buildContext.Variables[this.Name] = varExpr;
				buildContext.PrevExpressions.Add(assign);
				return varExpr;
			}
		}

		public ParameterExpression BuildForAssign(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, out BuildContext ownerBuildContext, out Type lastType)
		{
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out ownerBuildContext, out _, out lastType))
			{
				ownerBuildContext.ThrowIfReadOnly(this.Name);
				ownerBuildContext.ChangedVariables.Add(this.Name);
				return varExpr;
			}
			// 是否在执行上下文中存在变量
			var ownerContext = scriptContext.GetOwnerContext(this.Name, out _, out var type, out int modifier);
			if (ownerContext == null)
			{
				buildContext.LocalVariables.Add(this.Name);
			}
			else
			{
				Modifiers.ThrowIfReadOnly(this.Name, modifier);
				// 标记变量有变化
				buildContext.ChangedVariables.Add(this.Name);
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
			var call = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_EvalVar, Expression.Constant(this.Name));
			var assign = Expression.Assign(varExpr, Expression.Convert(call, type));
			buildContext.Variables[this.Name] = varExpr;
			buildContext.PrevExpressions.Add(assign);
			return varExpr;
		}

		public override void Clear()
		{
			base.Clear();

			this.Name = null;
		}
	}
}
