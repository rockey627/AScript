using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Lang.Lua.Nodes
{
	/// <summary>
	/// 数值for循环节点
	/// </summary>
	public class LuaForNumberNode : TreeNode
	{
		public ITreeNode VarNode { get; set; }
		public ITreeNode StartNode { get; set; }
		public ITreeNode EndNode { get; set; }
		public ITreeNode StepNode { get; set; }
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var varName = ((VariableNode)VarNode).Name;
			var startObj = StartNode.Eval(context, options, null, out _);
			var endObj = EndNode.Eval(context, options, null, out _);
			var stepObj = StepNode == null ? (object)1 : StepNode.Eval(context, options, null, out _);

			var tempContext = ScriptContext.Create(context);
			var tempControl = new EvalControl(control, true);

			if (ScriptUtils.IsIntegerType(startObj.GetType()) 
				&& ScriptUtils.IsIntegerType(endObj.GetType()) 
				&& ScriptUtils.IsIntegerType(stepObj.GetType()))
			{
				var start = Convert.ToInt64(startObj);
				var end = Convert.ToInt64(endObj);
				var step = Convert.ToInt64(stepObj);
				if (step > 0)
				{
					for (var i = start; i <= end; i += step)
					{
						tempContext.SetVar(varName, i);
						if (this.Body != null)
						{
							this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out _);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
				else
				{
					for (var i = start; i >= end; i += step)
					{
						tempContext.SetVar(varName, i);
						if (this.Body != null)
						{
							this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out _);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
			}
			else
			{
				var start = Convert.ToDouble(startObj);
				var end = Convert.ToDouble(endObj);
				var step = Convert.ToDouble(stepObj);
				if (step > 0D)
				{
					for (var i = start; i <= end; i += step)
					{
						tempContext.SetVar(varName, i);
						if (this.Body != null)
						{
							this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out _);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
				else
				{
					for (var i = start; i >= end; i += step)
					{
						tempContext.SetVar(varName, i);
						if (this.Body != null)
						{
							this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out _);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
			}

			returnType = typeof(void);
			return null;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var varName = ((VariableNode)VarNode).Name;
			var startExpr = Expression.Convert(this.StartNode.Build(buildContext, scriptContext, options), typeof(double));
			var endExpr = Expression.Convert(this.EndNode.Build(buildContext, scriptContext, options), typeof(double));
			Expression stepExpr = this.StepNode == null
				? (Expression)Expression.Constant(1)
				: Expression.Convert(this.StepNode.Build(buildContext, scriptContext, options), typeof(double));

			var iVar = Expression.Variable(typeof(double), varName);
			buildContext.Variables[varName] = iVar;

			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// i = start
			var initAssign = Expression.Assign(iVar, startExpr);

			// 条件判断: step >= 0 ? i <= end : i >= end
			var stepNonNegative = Expression.GreaterThanOrEqual(stepExpr, Expression.Constant(0.0));
			var condition = Expression.Condition(
				stepNonNegative,
				Expression.LessThanOrEqual(iVar, endExpr),
				Expression.GreaterThanOrEqual(iVar, endExpr)
			);

			// i += step
			var increment = Expression.AddAssign(iVar, stepExpr);

			// 循环体
			var bodyBuildContext = new BuildContext(buildContext)
			{
				ContinueLabel = continueLabel,
				BreakLabel = breakLabel
			};
			Expression bodyExpression;
			if (this.Body == null)
			{
				bodyExpression = Expression.Empty();
			}
			else
			{
				bodyExpression = this.Body.Build(bodyBuildContext, scriptContext, options);
				bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);
			}

			// 循环体块: body; continue_label: i += step
			var loopBody = Expression.Block(bodyExpression, Expression.Label(continueLabel), increment);

			// 完整循环: i = start; while(condition) { body; i += step }
			var loop = Expression.Loop(
				Expression.IfThenElse(condition, loopBody, Expression.Break(breakLabel)),
				breakLabel
			);

			return Expression.Block(new[] { iVar }, initAssign, loop);
		}

		public override void Clear()
		{
			base.Clear();

			this.VarNode = null;
			this.StartNode = null;
			this.EndNode = null;
			this.StepNode = null;
			this.Body = null;
		}
	}
}
