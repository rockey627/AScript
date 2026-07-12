using AScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class TryNode : TreeNode
	{
		public ITreeNode TryBody { get; set; }
		public ITreeNode FinallyBody { get; set; }
		public IList<Tuple<DefineVarNode, ITreeNode>> CatchNodes { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			// Build try body
			Expression tryExpr;
			if (this.TryBody != null)
			{
				tryExpr = this.TryBody.Build(buildContext, scriptContext, options);
				tryExpr = buildContext.BuildBlock(scriptContext, options, tryExpr);
			}
			else
			{
				tryExpr = Expression.Empty();
			}

			// Build catch blocks
			var catchBlocks = new List<CatchBlock>();
			if (this.CatchNodes != null)
			{
				foreach (var catchNode in this.CatchNodes)
				{
					var exVarType = catchNode.Item1?.SystemType ?? typeof(Exception);
					var exVarName = catchNode.Item1?.Name ?? "ex";

					// Create a new context for the catch block to isolate variables
					var catchContext = new BuildContext(buildContext);
					var exVar = Expression.Variable(exVarType, exVarName);
					catchContext.Variables[exVarName] = exVar;
					catchContext.LocalVariables.Add(exVarName);

					Expression catchBody;
					if (catchNode.Item2 != null)
					{
						catchBody = catchNode.Item2.Build(catchContext, scriptContext, options);
						catchBody = catchContext.BuildBlock(scriptContext, options, catchBody);
					}
					else
					{
						catchBody = Expression.Empty();
					}

					var catchBlock = Expression.Catch(exVar, catchBody);
					catchBlocks.Add(catchBlock);
				}
			}

			// If no catch nodes, add a general catch for Exception
			if (catchBlocks.Count == 0)
			{
				var catchContext = new BuildContext(buildContext);
				var exVar = Expression.Variable(typeof(Exception), "ex");
				catchContext.Variables["ex"] = exVar;
				catchContext.LocalVariables.Add("ex");
				var catchBlock = Expression.Catch(exVar, Expression.Empty());
				catchBlocks.Add(catchBlock);
			}

			// Build finally body
			Expression finallyExpr;
			if (this.FinallyBody != null)
			{
				finallyExpr = this.FinallyBody.Build(buildContext, scriptContext, options);
				finallyExpr = buildContext.BuildBlock(scriptContext, options, finallyExpr);
			}
			else
			{
				finallyExpr = Expression.Empty();
			}

			return Expression.TryCatchFinally(tryExpr, finallyExpr, catchBlocks.ToArray());
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			object result = null;
			returnType = null;
			Exception caughtException = null;

			// Execute try body
			try
			{
				if (this.TryBody != null)
				{
					result = this.TryBody.Eval(context, options, control, out returnType);
				}
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			// Execute catch handlers if exception occurred
			if (caughtException != null && this.CatchNodes != null)
			{
				foreach (var catchNode in this.CatchNodes)
				{
					var catchExType = catchNode.Item1?.SystemType ?? typeof(Exception);
					if (catchExType.IsAssignableFrom(caughtException.GetType()))
					{
						var catchContext = ScriptContext.Create(context);
						var exVarName = catchNode.Item1?.Name ?? "ex";
						var exVarType = catchNode.Item1?.SystemType ?? typeof(Exception);
						catchContext.SetTempVar(exVarName, caughtException, exVarType, false);

						if (catchNode.Item2 != null)
						{
							result = catchNode.Item2.Eval(catchContext, options, control, out returnType);
						}
						caughtException = null; // Exception was caught
						break;
					}
				}
			}

			// Execute finally body
			object finallyResult = null;
			Type finallyReturnType = null;
			if (this.FinallyBody != null)
			{
				finallyResult = this.FinallyBody.Eval(context, options, control, out finallyReturnType);
			}

			// If an exception occurred but was not caught, rethrow it
			if (caughtException != null)
			{
				throw caughtException;
			}

			// Finally's result doesn't override the try/catch result
			return result;
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			object result = null;
			Type returnType = null;
			Exception caughtException = null;

			// Execute try body
			try
			{
				if (this.TryBody != null)
				{
					var evalResult = await this.TryBody.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
					result = evalResult.Value;
					returnType = evalResult.Type;
				}
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			// Execute catch handlers if exception occurred
			if (caughtException != null && this.CatchNodes != null)
			{
				foreach (var catchNode in this.CatchNodes)
				{
					var catchExType = catchNode.Item1?.SystemType ?? typeof(Exception);
					if (catchExType.IsAssignableFrom(caughtException.GetType()))
					{
						var catchContext = ScriptContext.Create(context);
						var exVarName = catchNode.Item1?.Name ?? "ex";
						var exVarType = catchNode.Item1?.SystemType ?? typeof(Exception);
						catchContext.SetTempVar(exVarName, caughtException, exVarType, false);

						if (catchNode.Item2 != null)
						{
							var evalResult = await catchNode.Item2.EvalAsync(catchContext, options, control, cancellationToken).ConfigureAwait(false);
							result = evalResult.Value;
							returnType = evalResult.Type;
						}
						caughtException = null; // Exception was caught
						break;
					}
				}
			}

			// Execute finally body
			if (this.FinallyBody != null)
			{
				await this.FinallyBody.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			}

			// If an exception occurred but was not caught, rethrow it
			if (caughtException != null)
			{
				throw caughtException;
			}

			return new EvalResult(result, returnType);
		}

		public override void Clear()
		{
			base.Clear();

			if (this.TryBody != null)
			{
				this.TryBody.Clear();
				this.TryBody = null;
			}

			if (this.FinallyBody != null)
			{
				this.FinallyBody.Clear();
				this.FinallyBody = null;
			}

			if (this.CatchNodes != null)
			{
				foreach (var catchNode in this.CatchNodes)
				{
					if (catchNode.Item1 != null)
					{
						catchNode.Item1.Clear();
					}
					if (catchNode.Item2 != null)
					{
						catchNode.Item2.Clear();
					}
				}
				this.CatchNodes = null;
			}
		}
	}
}
