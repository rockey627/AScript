using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
			var mode = options.CompileMode;
			if (mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop))
			{
				// 编译循环
				return ScriptUtils.EvalWithCompile(context, options, control, this, out returnType);
			}
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
					ScriptContext loopContext = null;
					for (var i = start; i <= end; i += step)
					{
						if (this.Body != null)
						{
							tempContext.SetVar(varName, i);
							if (loopContext == null)
							{
								loopContext = ScriptContext.Create(tempContext);
							}
							else loopContext.Clear();
							bodyResult = this.Body.Eval(loopContext, options, tempControl, out bodyReturnType);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
				else
				{
					ScriptContext loopContext = null;
					for (var i = start; i >= end; i += step)
					{
						if (this.Body != null)
						{
							tempContext.SetVar(varName, i);
							if (loopContext == null)
							{
								loopContext = ScriptContext.Create(tempContext);
							}
							else loopContext.Clear();
							bodyResult = this.Body.Eval(loopContext, options, tempControl, out bodyReturnType);
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
					ScriptContext loopContext = null;
					for (var i = start; i <= end; i += step)
					{
						if (this.Body != null)
						{
							tempContext.SetVar(varName, i);
							if (loopContext == null)
							{
								loopContext = ScriptContext.Create(tempContext);
							}
							else loopContext.Clear();
							bodyResult = this.Body.Eval(loopContext, options, tempControl, out bodyReturnType);
							if (tempControl.Terminal || tempControl.Break) break;
							tempControl.Continue = false;
						}
					}
				}
				else
				{
					ScriptContext loopContext = null;
					for (var i = start; i >= end; i += step)
					{
						if (this.Body != null)
						{
							tempContext.SetVar(varName, i);
							if (loopContext == null)
							{
								loopContext = ScriptContext.Create(tempContext);
							}
							else loopContext.Clear();
							bodyResult = this.Body.Eval(loopContext, options, tempControl, out bodyReturnType);
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
			var startVar = startExpr is ConstantExpression || startExpr is ParameterExpression ? null : Expression.Variable(startExpr.Type, "__start");
			var endVar = endExpr is ConstantExpression || endExpr is ParameterExpression ? null : Expression.Variable(endExpr.Type, "__end");
			var stepVar = stepExpr is ConstantExpression || stepExpr is ParameterExpression ? null : Expression.Variable(stepExpr.Type, "__step");
			var varList = new List<ParameterExpression>(3);
			var statements = new List<Expression>(4);
			if (startVar != null)
			{
				varList.Add(startVar);
				statements.Add(Expression.Assign(startVar, startExpr));
			}
			if (endVar != null)
			{
				varList.Add(endVar);
				statements.Add(Expression.Assign(endVar, endExpr));
			}
			if (stepVar != null)
			{
				varList.Add(stepVar);
				statements.Add(Expression.Assign(stepVar, stepExpr));
			}


			if (ScriptUtils.IsIntegerType(startExpr.Type)
				&& ScriptUtils.IsIntegerType(endExpr.Type)
				&& ScriptUtils.IsIntegerType(stepExpr.Type))
			{
				var int64Loop1 = BuildInt64Loop(buildContext, scriptContext, options, varName,
					startExpr.Type == typeof(long) ? (Expression)startVar ?? startExpr : Expression.Convert(startVar ?? startExpr, typeof(long)),
					endExpr.Type == typeof(long) ? (Expression)endVar ?? endExpr : Expression.Convert(endVar ?? endExpr, typeof(long)),
					stepVar ?? stepExpr);
				if (statements.Count == 0) return int64Loop1;
				statements.Add(int64Loop1);
				return Expression.Block(varList, statements);
			}

			if (ScriptUtils.IsNumberType(startExpr.Type)
				&& ScriptUtils.IsNumberType(endExpr.Type)
				&& ScriptUtils.IsNumberType(stepExpr.Type))
			{
				var doubleLoop1 = BuildDoubleLoop(buildContext, scriptContext, options, varName,
					startExpr.Type == typeof(double) ? (Expression)startVar ?? startExpr : Expression.Convert(startVar ?? startExpr, typeof(double)),
					endExpr.Type == typeof(double) ? (Expression)endVar ?? endExpr : Expression.Convert(endVar ?? endExpr, typeof(double)),
					stepVar ?? stepExpr);
				if (statements.Count == 0) return doubleLoop1;
				statements.Add(doubleLoop1);
				return Expression.Block(varList, statements);
			}

			// 调用 ScriptUtils.IsIntegerType 进行运行时检查
			var isAllInteger = Expression.AndAlso(
				Expression.Call(ScriptUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(startVar ?? startExpr, ScriptUtils.Method_Object_GetType)),
				Expression.AndAlso(
					Expression.Call(ScriptUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(endVar ?? endExpr, ScriptUtils.Method_Object_GetType)),
					Expression.Call(ScriptUtils.Method_ScriptUtils_IsIntegerType, Expression.Call(stepVar ?? stepExpr, ScriptUtils.Method_Object_GetType))
				)
			);

			// 构建整数循环体 (Int64)
			var int64Loop = BuildInt64Loop(buildContext, scriptContext, options, varName,
				startExpr.Type == typeof(long) ? (Expression)startVar ?? startExpr : Expression.Convert(startVar ?? startExpr, typeof(long)),
				endExpr.Type == typeof(long) ? (Expression)endVar ?? endExpr : Expression.Convert(endVar ?? endExpr, typeof(long)),
				stepVar ?? stepExpr);

			// 构建浮点数循环体 (Double)
			var doubleLoop = BuildDoubleLoop(buildContext, scriptContext, options, varName,
				startExpr.Type == typeof(double) ? (Expression)startVar ?? startExpr : Expression.Convert(startVar ?? startExpr, typeof(double)),
				endExpr.Type == typeof(double) ? (Expression)endVar ?? endExpr : Expression.Convert(endVar ?? endExpr, typeof(double)),
				stepVar ?? stepExpr);

			// 根据运行时类型检查选择不同的循环
			var conditionExpr = Expression.Condition(isAllInteger, int64Loop, doubleLoop);

			// 组合：先初始化临时变量，再执行循环
			if (statements.Count == 0) return conditionExpr;
			statements.Add(conditionExpr);
			return Expression.Block(varList, statements);
		}

		private Expression BuildInt64Loop(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string varName, Expression startExpr, Expression endExpr, Expression stepExpr)
		{
			var iVar = Expression.Variable(typeof(long), varName);
			// 循环变量，数值for循环内部改变iVar值不会影响循环次数
			var iVar2 = Expression.Variable(typeof(long), $"__{varName}_2_");
			var localContext = new BuildContext(buildContext);
			localContext.Variables[varName] = iVar;
			localContext.Variables[iVar2.Name] = iVar2;
			localContext.LocalVariables.Add(varName);
			localContext.LocalVariables.Add(iVar2.Name);

			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// i = start
			var initAssign = Expression.Assign(iVar2, startExpr);

			// 条件判断: step > 0 ? i <= end : i >= end
			Expression condition;
			if (stepExpr is ConstantExpression stepConst)
			{
				long step = Convert.ToInt64(stepConst.Value);
				if (step > 0L)
				{
					condition = Expression.LessThanOrEqual(iVar2, endExpr);
				}
				else
				{
					condition = Expression.GreaterThanOrEqual(iVar2, endExpr);
				}
			}
			else
			{
				if (stepExpr.Type != typeof(long)) stepExpr = Expression.Convert(stepExpr, typeof(long));
				var stepPositive = Expression.GreaterThan(stepExpr, Expression.Constant(0L));
				condition = Expression.Condition(
					stepPositive,
					Expression.LessThanOrEqual(iVar2, endExpr),
					Expression.GreaterThanOrEqual(iVar2, endExpr)
				);
			}

			// i += step
			if (stepExpr.Type != typeof(long)) stepExpr = Expression.Convert(stepExpr, typeof(long));
			var increment = Expression.AddAssign(iVar2, stepExpr);

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
				if (bodyExpression != null)
				{
					var iVarAssign = Expression.Assign(iVar, iVar2);
					bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, iVarAssign, bodyExpression);
				}
			}

			// 循环体块: body; continue_label: i += step
			var loopBody = bodyExpression == null ? (Expression)increment : Expression.Block(bodyExpression, Expression.Label(continueLabel), increment);

			// 完整循环
			var loop = Expression.Loop(
				Expression.IfThenElse(condition, loopBody, Expression.Break(breakLabel)),
				breakLabel
			);

			return Expression.Block(new[] { iVar, iVar2 }, initAssign, loop);
			//return localContext.BuildBlock(scriptContext, options, initAssign, loop);
		}

		private Expression BuildDoubleLoop(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, string varName, Expression startExpr, Expression endExpr, Expression stepExpr)
		{
			var iVar = Expression.Variable(typeof(double), varName);
			// 循环变量，数值for循环内部改变iVar值不会影响循环次数
			var iVar2 = Expression.Variable(typeof(double), $"__{varName}_2_");
			var localContext = new BuildContext(buildContext);
			localContext.Variables[varName] = iVar;
			localContext.Variables[iVar2.Name] = iVar2;
			localContext.LocalVariables.Add(varName);
			localContext.LocalVariables.Add(iVar2.Name);

			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// i = start
			var initAssign = Expression.Assign(iVar2, startExpr);

			// 条件判断: step > 0 ? i <= end : i >= end
			Expression condition;
			if (stepExpr is ConstantExpression stepConst)
			{
				double step = Convert.ToDouble(stepConst.Value);
				if (step > 0D)
				{
					condition = Expression.LessThanOrEqual(iVar2, endExpr);
				}
				else
				{
					condition = Expression.GreaterThanOrEqual(iVar2, endExpr);
				}
			}
			else
			{
				if (stepExpr.Type != typeof(double)) stepExpr = Expression.Convert(stepExpr, typeof(double));
				var stepPositive = Expression.GreaterThan(stepExpr, Expression.Constant(0.0));
				condition = Expression.Condition(
					stepPositive,
					Expression.LessThanOrEqual(iVar2, endExpr),
					Expression.GreaterThanOrEqual(iVar2, endExpr)
				);
			}

			// i += step
			if (stepExpr.Type != typeof(double)) stepExpr = Expression.Convert(stepExpr, typeof(double));
			var increment = Expression.AddAssign(iVar2, stepExpr);

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
				if (bodyExpression != null)
				{
					var iVarAssign = Expression.Assign(iVar, iVar2);
					bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, iVarAssign, bodyExpression);
				}
			}

			// 循环体块: body; continue_label: i += step
			var loopBody = bodyExpression == null ? (Expression)increment : Expression.Block(bodyExpression, Expression.Label(continueLabel), increment);

			// 完整循环
			var loop = Expression.Loop(
				Expression.IfThenElse(condition, loopBody, Expression.Break(breakLabel)),
				breakLabel
			);

			return Expression.Block(new[] { iVar, iVar2 }, initAssign, loop);
			//return localContext.BuildBlock(scriptContext, options, initAssign, loop);
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
