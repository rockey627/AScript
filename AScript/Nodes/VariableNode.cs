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
			if (buildContext.TryGetVariableOrParameter(this.Name, out var varExpr, out _, out bool outer))
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
				return varExpr;
			}
			if (options.DynamicVariableType ?? false)
			{
				// 定义变量
				varExpr = Expression.Variable(typeof(object), this.Name);
				// 从ScriptContext中取值
				var call = Expression.Call(buildContext.GetScriptContextParameter(), ExpressionUtils.Method_ScriptContext_EvalVar, Expression.Constant(this.Name));
				// 赋值v变量
				var assign = Expression.Assign(varExpr, call);
				buildContext.Variables[this.Name] = varExpr;
				//buildContext.PrevExpressions.Add(ExpressionUtils.ConsoleWriteLine("111"));
				buildContext.PrevExpressions.Add(assign);
				//buildContext.PrevExpressions.Add(ExpressionUtils.ConsoleWriteLine("222"));
				//buildContext.PrevExpressions.Add(ExpressionUtils.ConsoleWriteLine(varExpr));
				//buildContext.PrevExpressions.Add(ExpressionUtils.ConsoleWriteLine("333"));
				return varExpr;
			}
			else
			{
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
		}

		public override void Clear()
		{
			base.Clear();

			this.Name = null;
		}
	}
}
