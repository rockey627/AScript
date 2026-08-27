using AScript.Exceptions;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class ForNode : TreeNode
	{
		public ITreeNode Init { get; set; }
		public ITreeNode Condition { get; set; }
		public ITreeNode Body { get; set; }
		public ITreeNode Post { get; set; }

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
			// 执行初始语句
			if (this.Init != null)
			{
				this.Init.Eval(tempContext, options, null, out _);
			}
			// 执行循环
			object bodyResult = null;
			Type bodyReturnType = null;
			while (true)
			{
				// 条件判断
				if (this.Condition != null)
				{
					var conditionResult = this.Condition.Eval(tempContext, null, null, out var conditionType);
					if (conditionType != null)
					{
						if (!(conditionResult is bool b))
						{
							throw new ScriptAnalyzingException($"invalid for condition [{conditionType}], must be bool");
						}
						if (!b) break;
					}
				}
				// 执行body
				if (this.Body != null)
				{
					bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempControl, out bodyReturnType);
					if (tempControl.Terminal || tempControl.Break) break;
					tempControl.Continue = false;
				}
				// 执行后置语句
				if (this.Post != null)
				{
					this.Post.Eval(tempContext, options, null, out _);
				}
			}
			returnType = bodyReturnType;
			return bodyResult;
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			var mode = options.CompileMode;
			bool compileLoop = mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop);
			if (compileLoop)
			{
				// 编译循环
				var loopOptions = new BuildOptions(options)
				{
					CompileMode = ECompileMode.All,
					UseCompletionResult = true,
					RewriteVariables = true,
					RewriteFunctions = false,
					Standalone = false
				};
				var loop = Script.Compile(null, context, loopOptions, this);
				var loopResult = loop.DynamicInvoke(context);
				if (loopResult is CompletionResult completionResult)
				{
					if (completionResult.CompletionType == ECompletionType.Return)
					{
						control.Terminal = true;
					}
					return new EvalResult(completionResult.Value, completionResult.ValueType);
				}
				return new EvalResult(loopResult, loopResult?.GetType() ?? loop.Method.ReturnType);
			}
			// 
			var tempContext = ScriptContext.Create(context);
			var tempControl = new EvalControl(control, true);
			// 执行初始语句
			if (this.Init != null)
			{
				await this.Init.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
			}
			// 执行循环
			EvalResult bodyResult = default;
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				// 条件判断
				if (this.Condition != null)
				{
					var evalResult = await this.Condition.EvalAsync(tempContext, null, null, cancellationToken).ConfigureAwait(false);
					var conditionResult = evalResult.Value;
					var conditionType = evalResult.Type;
					if (conditionType != null)
					{
						if (!(conditionResult is bool b))
						{
							throw new ScriptAnalyzingException($"invalid for condition [{conditionType}], must be bool");
						}
						if (!b) break;
					}
				}
				// 执行body
				if (this.Body != null)
				{
					bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempControl, cancellationToken).ConfigureAwait(false);
					if (tempControl.Terminal || tempControl.Break) break;
					tempControl.Continue = false;
				}
				// 执行后置语句
				if (this.Post != null)
				{
					await this.Post.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
				}
			}
			return bodyResult;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			// 初始化语句
			Expression initExpression = this.Init?.Build(tempBuildContext, scriptContext, options);
			// 条件判断语句
			Expression conditionExpression = this.Condition?.Build(tempBuildContext, scriptContext, options);
			// 后置语句
			Expression postExpression;
			if (this.Post == null)
			{
				postExpression = null;
			}
			else
			{
				var postBuildContext = new BuildContext(tempBuildContext);
				postExpression = this.Post?.Build(postBuildContext, scriptContext, options);
				postExpression = postBuildContext.BuildBlock(scriptContext, options, postExpression);
			}
			// 循环体
			var breakLabel = Expression.Label();
			Expression bodyExpression;
			BuildContext bodyBuildContext;
			if (this.Body == null)
			{
				bodyExpression = null;
				bodyBuildContext = null;
			}
			else
			{
				bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = Expression.Label(),
					BreakLabel = breakLabel
				};
				bodyExpression = this.Body?.Build(bodyBuildContext, scriptContext, options);
				bodyExpression = bodyBuildContext.BuildBlock(scriptContext, options, bodyExpression);
			}
			// 
			var continueLabel = bodyBuildContext?.ContinueLabel;
			Expression loopBlockExpression = Block(bodyExpression, Expression.Label(continueLabel), postExpression);
			Expression conditionBlockExpression = Expression.IfThenElse(conditionExpression, loopBlockExpression, Expression.Break(breakLabel));
			var loopExpression = Expression.Loop(conditionBlockExpression, breakLabel);
			if (initExpression == null)
			{
				return tempBuildContext.BuildBlock(scriptContext, options, loopExpression);
			}
			return tempBuildContext.BuildBlock(scriptContext, options, initExpression, loopExpression);
		}

		private static Expression Block(Expression bodyExpression, Expression labelExpression, Expression postExpression)
		{
			if (bodyExpression == null)
			{
				if (postExpression == null)
				{
					return labelExpression ?? Expression.Empty();
				}
				return Expression.Block(labelExpression, postExpression);
			}
			if (postExpression == null)
			{
				return Expression.Block(bodyExpression, labelExpression);
			}
			return Expression.Block(bodyExpression, labelExpression, postExpression);
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Init);
			PoolManage.Return(this.Condition);
			PoolManage.Return(this.Body);
			PoolManage.Return(this.Post);

			this.Init = null;
			this.Condition = null;
			this.Body = null;
			this.Post = null;
		}
	}
}
