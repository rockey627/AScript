using AScript.Exceptions;
using System;
using System.CodeDom;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class WhileNode : TreeNode
	{
		public ITreeNode Condition { get; set; }
		public ITreeNode Body { get; set; }
		/// <summary>
		/// 是否是do ... while(condition)循环
		/// </summary>
		public bool IsDoWhile { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var mode = options.CompileMode;
			if (mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop))
			{
				// 编译循环
				return ScriptUtils.EvalWithCompile(context, options, control, this, out returnType);
				//var loopOptions = new BuildOptions(options)
				//{
				//	CompileMode = ECompileMode.All,
				//	UseCompletionResult = true,
				//	RewriteVariables = true,
				//	RewriteFunctions = false,
				//	Standalone = false
				//};
				//var loop = Script.Compile(null, context, loopOptions, this);
				//var loopResult = loop.DynamicInvoke(context);
				//if (loopResult is CompletionResult completionResult)
				//{
				//	if (completionResult.CompletionType == ECompletionType.Return)
				//	{
				//		control.Terminal = true;
				//	}
				//	returnType = completionResult.ValueType;
				//	return completionResult.Value;
				//}
				//returnType = loopResult?.GetType() ?? loop.Method.ReturnType;
				//return loopResult;
			}
			// 
			var tempContext = ScriptContext.Create(context);
			var tempControl = new EvalControl(control, true);
			object bodyResult = null;
			Type bodyType = null;
			while (true)
			{
				if (!IsDoWhile && !EvalCondition(tempContext, options))
				{
					break;
				}
				if (this.Body != null)
				{
					bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyType);
					if (tempControl.Terminal || tempControl.Break) break;
					if (tempControl.Continue)
					{
						tempControl.Continue = false;
						continue;
					}
				}
				if (IsDoWhile && !EvalCondition(tempContext, options))
				{
					break;
				}
			}
			returnType = bodyType;
			return bodyResult;
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			var mode = options.CompileMode;
			bool compileLoop = mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop);
			if (compileLoop)
			{
				// 编译循环
				var loopResult = ScriptUtils.EvalWithCompile(context, options, control, this, out var returnType);
				return new EvalResult(loopResult, returnType);
				//var loopOptions = new BuildOptions(options)
				//{
				//	CompileMode = ECompileMode.All,
				//	UseCompletionResult = true,
				//	RewriteVariables = true,
				//	RewriteFunctions = false,
				//	Standalone = false
				//};
				//var loop = Script.Compile(null, context, loopOptions, this);
				//var loopResult = loop.DynamicInvoke(context);
				//if (loopResult is CompletionResult completionResult)
				//{
				//	if (completionResult.CompletionType == ECompletionType.Return)
				//	{
				//		control.Terminal = true;
				//	}
				//	return new EvalResult(completionResult.Value, completionResult.ValueType);
				//}
				//return new EvalResult(loopResult, loopResult?.GetType() ?? loop.Method.ReturnType);
			}
			// 
			var tempContext = ScriptContext.Create(context);
			var tempControl = new EvalControl(control, true);
			EvalResult bodyResult = default;
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (!IsDoWhile && !(await EvalConditionAsync(tempContext, options, cancellationToken).ConfigureAwait(false)))
				{
					break;
				}
				if (this.Body != null)
				{
					bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempControl, cancellationToken).ConfigureAwait(false);
					if (tempControl.Terminal || tempControl.Break) break;
					if (tempControl.Continue)
					{
						tempControl.Continue = false;
						continue;
					}
				}
				if (IsDoWhile && !(await EvalConditionAsync(tempContext, options, cancellationToken).ConfigureAwait(false)))
				{
					break;
				}
			}
			return bodyResult;
		}

		private bool EvalCondition(ScriptContext context, BuildOptions options)
		{
			if (this.Condition == null) return true;
			var conditionResult = this.Condition.Eval(context, options, null, out var conditionType);
			//if (!(conditionResult is bool b))
			//{
			//	throw new ScriptAnalyzingException($"invalid if condition type {conditionType}");
			//}
			//return b;
			return context.IsTrue(conditionResult);
		}

		private async Task<bool> EvalConditionAsync(ScriptContext context, BuildOptions options, CancellationToken cancellationToken)
		{
			if (this.Condition == null) return true;
			var evalResult = await this.Condition.EvalAsync(context, options, null, cancellationToken).ConfigureAwait(false);
			//var conditionResult = evalResult.Value;
			//var conditionType = evalResult.Type;
			//if (!(conditionResult is bool b))
			//{
			//	throw new ScriptAnalyzingException($"invalid if condition type {conditionType}");
			//}
			//return b;
			return context.IsTrue(evalResult.Value);
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			// 条件
			Expression conditionExpression = this.Condition.Build(tempBuildContext, scriptContext, options);
			if (conditionExpression.Type != typeof(bool))
			{
				if (conditionExpression.Type.IsValueType) conditionExpression = Expression.Convert(conditionExpression, typeof(object));
				conditionExpression = Expression.Call(buildContext.GetScriptContextParameter(), ScriptUtils.Method_ScriptContext_IsTrue, conditionExpression);
			}
			// 循环体
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();
			var bodyBuildContext = new BuildContext(tempBuildContext)
			{
				ContinueLabel = continueLabel,
				BreakLabel = breakLabel
			};
			Expression bodyExpression = this.Body?.Build(bodyBuildContext, scriptContext, options);
			if (bodyExpression == null) bodyExpression = Expression.Empty();
			else bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);

			Expression loop;
			if (IsDoWhile)
			{
				// do { body } while(condition);
				// 结构: loop: body; if(condition) goto loop else break
				loop = Expression.Loop(
					Expression.Block(bodyExpression, Expression.IfThenElse(conditionExpression, Expression.Goto(continueLabel), Expression.Break(breakLabel))),
					breakLabel,
					continueLabel);
			}
			else
			{
				// while(condition) { body }
				// 结构: loop: if(condition) { body; goto loop } else break
				loop = Expression.Loop(
					Expression.IfThenElse(conditionExpression, Expression.Block(bodyExpression, Expression.Goto(continueLabel)), Expression.Break(breakLabel)),
					breakLabel,
					continueLabel);
			}
			//return loop;
			return tempBuildContext.BuildBlock(scriptContext, options, loop);
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Condition);
			PoolManage.Return(this.Body);
			this.Condition = null;
			this.Body = null;
		}
	}
}
