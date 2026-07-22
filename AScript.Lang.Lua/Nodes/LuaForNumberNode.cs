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

			object bodyResult = null;
			Type bodyReturnType = null;

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
							bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyReturnType);
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
							bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyReturnType);
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
							bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyReturnType);
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
							bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyReturnType);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
			}

			returnType = bodyReturnType;
			return bodyResult;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var varName = ((VariableNode)VarNode).Name;

			// 构建 start, end, step 表达式
			var startExpr = this.StartNode.Build(buildContext, scriptContext, options);
			var endExpr = this.EndNode.Build(buildContext, scriptContext, options);
			var stepExpr = this.StepNode == null
				? (Expression)Expression.Constant(1L)
				: this.StepNode.Build(buildContext, scriptContext, options);

			// 创建临时变量保存计算结果，确保循环前只计算一次
			var startVar = Expression.Variable(startExpr.Type, "__start");
			var endVar = Expression.Variable(endExpr.Type, "__end");
			var stepVar = Expression.Variable(stepExpr.Type, "__step");

			// 调用 ScriptUtils.IsIntegerType 进行运行时检查
			var isAllInteger = Expression.AndAlso(
				Expression.Call(ExpressionUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(startVar, ExpressionUtils.Method_Object_GetType)),
				Expression.AndAlso(
					Expression.Call(ExpressionUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(endVar, ExpressionUtils.Method_Object_GetType)),
					Expression.Call(ExpressionUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(stepVar, ExpressionUtils.Method_Object_GetType))
				)
			);

			// 构建整数循环体 (Int64)
			var int64Loop = BuildInt64Loop(buildContext, scriptContext, options, varName,
				Expression.Convert(startVar, typeof(long)),
				Expression.Convert(endVar, typeof(long)),
				Expression.Convert(stepVar, typeof(long)));

			// 构建浮点数循环体 (Double)
			var doubleLoop = BuildDoubleLoop(buildContext, scriptContext, options, varName,
				Expression.Convert(startVar, typeof(double)),
				Expression.Convert(endVar, typeof(double)),
				Expression.Convert(stepVar, typeof(double)));

			// 根据运行时类型检查选择不同的循环
			var conditionExpr = Expression.Condition(isAllInteger, int64Loop, doubleLoop);

			// 组合：先初始化临时变量，再执行循环
			return Expression.Block(
				new[] { startVar, endVar, stepVar },
				Expression.Assign(startVar, startExpr),
				Expression.Assign(endVar, endExpr),
				Expression.Assign(stepVar, stepExpr),
				conditionExpr);
		}

		private Expression BuildInt64Loop(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string varName, Expression startExpr, Expression endExpr, Expression stepExpr)
		{
			var iVar = Expression.Variable(typeof(long), varName);
			var localContext = new BuildContext(buildContext);
			localContext.Variables[varName] = iVar;

			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// i = start
			var initAssign = Expression.Assign(iVar, startExpr);

			// 条件判断: step > 0 ? i <= end : i >= end
			var stepPositive = Expression.GreaterThan(stepExpr, Expression.Constant(0L));
			var condition = Expression.Condition(
				stepPositive,
				Expression.LessThanOrEqual(iVar, endExpr),
				Expression.GreaterThanOrEqual(iVar, endExpr)
			);

			// i += step
			var increment = Expression.AddAssign(iVar, stepExpr);

			// 循环体
			Expression bodyExpression;
			if (this.Body == null)
			{
				bodyExpression = Expression.Empty();
			}
			else
			{
				var bodyBuildContext = new BuildContext(localContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				bodyExpression = this.Body.Build(bodyBuildContext, scriptContext, options);
				bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);
			}

			// 循环体块: body; continue_label: i += step
			var loopBody = bodyExpression == null ? (Expression)increment : Expression.Block(bodyExpression, Expression.Label(continueLabel), increment);

			// 完整循环
			var loop = Expression.Loop(
				Expression.IfThenElse(condition, loopBody, Expression.Break(breakLabel)),
				breakLabel
			);

			return Expression.Block(new[] { iVar }, initAssign, loop);
		}

		private Expression BuildDoubleLoop(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string varName, Expression startExpr, Expression endExpr, Expression stepExpr)
		{
			var iVar = Expression.Variable(typeof(double), varName);
			var localContext = new BuildContext(buildContext);
			localContext.Variables[varName] = iVar;

			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// i = start
			var initAssign = Expression.Assign(iVar, startExpr);

			// 条件判断: step > 0 ? i <= end : i >= end
			var stepPositive = Expression.GreaterThan(stepExpr, Expression.Constant(0.0));
			var condition = Expression.Condition(
				stepPositive,
				Expression.LessThanOrEqual(iVar, endExpr),
				Expression.GreaterThanOrEqual(iVar, endExpr)
			);

			// i += step
			var increment = Expression.AddAssign(iVar, stepExpr);

			// 循环体
			Expression bodyExpression;
			if (this.Body == null)
			{
				bodyExpression = Expression.Empty();
			}
			else
			{
				var bodyBuildContext = new BuildContext(localContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				bodyExpression = this.Body.Build(bodyBuildContext, scriptContext, options);
				bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);
			}

			// 循环体块: body; continue_label: i += step
			var loopBody = bodyExpression == null ? (Expression)increment : Expression.Block(bodyExpression, Expression.Label(continueLabel), increment);

			// 完整循环
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
