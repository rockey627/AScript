using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
				var bodyContext = new BuildContext(buildContext);
				var bodyExpr = this.TryBody.Build(bodyContext, scriptContext, options);
				tryExpr = bodyContext.BuildBlock(scriptContext, options, bodyExpr);
			}
			else
			{
				tryExpr = Expression.Empty();
			}

			// Build catch blocks
			var catchBlocks = new CatchBlock[this.CatchNodes == null ? 0 : this.CatchNodes.Count];
			if (this.CatchNodes != null)
			{
				for (int i = 0; i < this.CatchNodes.Count; i++)
				{
					var catchNode = this.CatchNodes[i];
					var exVarType = typeof(Exception);
					if (catchNode.Item1 != null)
					{
						if (catchNode.Item1.SystemType == null)
						{
							if (!string.IsNullOrEmpty(catchNode.Item1.Name))
							{
								exVarType = scriptContext.EvalType(catchNode.Item1.Name);
								if (exVarType == null)
								{
									throw new Exceptions.ScriptRuntimeException($"unkown exception type '{catchNode.Item1.Name}'");
								}
							}
						}
						else
						{
							exVarType = catchNode.Item1.SystemType;
						}
					}
					var exVarName = catchNode.Item1?.Name;

					// Create a new context for the catch block to isolate variables
					var catchContext = new BuildContext(buildContext);
					ParameterExpression exVar = null;
					if (!string.IsNullOrEmpty(exVarName))
					{
						exVar = Expression.Variable(exVarType, exVarName);
						catchContext.Variables[exVarName] = exVar;
						catchContext.LocalVariables.Add(exVarName);
					}

					Expression catchBody;
					if (catchNode.Item2 != null)
					{
						var catchBodyExpr = catchNode.Item2.Build(catchContext, scriptContext, options);
						catchBody = catchContext.BuildBlock(scriptContext, options, catchBodyExpr);
					}
					else
					{
						catchBody = Expression.Empty();
					}

					var catchBlock = exVar == null ? Expression.Catch(exVarType, catchBody) : Expression.Catch(exVar, catchBody);
					catchBlocks[i] = catchBlock;
				}
			}

			//// If no catch nodes, add a general catch for Exception
			//if (catchBlocks.Count == 0)
			//{
			//	var catchContext = new BuildContext(buildContext);
			//	var exVar = Expression.Variable(typeof(Exception), "ex");
			//	catchContext.Variables["ex"] = exVar;
			//	catchContext.LocalVariables.Add("ex");
			//	var catchBlock = Expression.Catch(exVar, Expression.Empty());
			//	catchBlocks.Add(catchBlock);
			//}

			// Build finally body
			Expression finallyExpr;
			if (this.FinallyBody != null)
			{
				var finallyContext = new BuildContext(buildContext);
				var finallyBody = this.FinallyBody.Build(finallyContext, scriptContext, options);
				finallyExpr = finallyContext.BuildBlock(scriptContext, options, finallyBody);
			}
			else
			{
				finallyExpr = Expression.Empty();
			}

			return Expression.TryCatchFinally(tryExpr, finallyExpr, catchBlocks);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			object result = null;
			returnType = null;
			Exception caughtException = null;

			// Execute try body
			if (this.TryBody != null)
			{
				try
				{
					result = this.TryBody.Eval(context, options, control, out returnType);
				}
				catch (Exception ex)
				{
					caughtException = ex;
				}
			}

			// Execute catch handlers if exception occurred
			if (caughtException != null && this.CatchNodes != null)
			{
				var catchNode = this.CatchNodes.FirstOrDefault(a => IsMatched(context, a.Item1, caughtException));
				if (catchNode != null)
				{
					if (catchNode.Item2 != null)
					{
						var catchContext = ScriptContext.Create(context);
						if (!string.IsNullOrEmpty(catchNode.Item1.Name))
						{
							catchContext.SetVar(catchNode.Item1.Name, caughtException);
						}
						result = catchNode.Item2.Eval(catchContext, options, control, out returnType);
					}
					caughtException = null;
				}
			}

			// Execute finally body
			this.FinallyBody?.Eval(context, options, control, out _);

			// If an exception occurred but was not caught, rethrow it
			if (caughtException != null)
			{
				throw caughtException;
			}

			return result;
		}

		private bool IsMatched(ScriptContext context, DefineVarNode exNode, Exception ex)
		{
			if (exNode.SystemType != null)
			{
				return exNode.SystemType.IsAssignableFrom(ex.GetType());
			}
			if (!string.IsNullOrEmpty(exNode.Type))
			{
				var systemType = context.EvalType(exNode.Type);
				if (systemType != null)
				{
					return systemType.IsAssignableFrom(ex.GetType());
				}
				return ex.GetType().Name == ex.GetType().Name;
			}
			return true;
		}

		public override void Clear()
		{
			base.Clear();

			if (this.TryBody != null)
			{
				PoolManage.Return(this.TryBody);
				this.TryBody = null;
			}

			if (this.FinallyBody != null)
			{
				PoolManage.Return(this.FinallyBody);
				this.FinallyBody = null;
			}

			if (this.CatchNodes != null)
			{
				foreach (var catchNode in this.CatchNodes)
				{
					PoolManage.Return(catchNode.Item1);
					PoolManage.Return(catchNode.Item2);
				}
				this.CatchNodes.Clear();
				this.CatchNodes = null;
			}
		}
	}
}
