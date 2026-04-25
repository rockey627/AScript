using System;
using System.Linq.Expressions;

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
			if (returnType == null && (options.ThrowIfVariableNotExists ?? false))
			{
				throw new Exception($"variable {this.Name} is not exists");
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
			scriptContext.GetOwnerContext(this.Name, out var value, out var type, true);
			if (type == null)
			{
				if (options.ThrowIfVariableNotExists ?? false)
				{
					throw new Exception($"variable {this.Name} is not exists");
				}
				type = typeof(object);
			}
			if (type == typeof(TypeWrapper))
			{
				return Expression.Constant(value);
			}
			else
			{
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
			// 是否在执行上下文中存在变量
			var ownerContext = scriptContext.GetOwnerContext(this.Name, out _, out var type);
			if (ownerContext == null)
			{
				buildContext.LocalVariables.Add(this.Name);
			}
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out ownerBuildContext, out _, out lastType))
			{
				return varExpr;
			}
			if (type == null)
			{
				//if (options.ThrowIfVariableNotExists ?? false)
				//{
				//	throw new Exception($"variable {this.Name} is not exists");
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
