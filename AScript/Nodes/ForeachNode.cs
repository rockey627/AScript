using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
    public class ForeachNode : TreeNode
	{
		public DefineVarNode VarDefine { get; set; }
		public ITreeNode Collection { get; set; }
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl controll, out Type returnType)
		{
			if (this.VarDefine == null)
			{
				throw new Exception("require variable define in foreach statement");
			}
			if (this.Collection == null)
			{
				throw new Exception("require collection in foreach statement");
			}
			// 计算集合
			var listResult = this.Collection.Eval(context, options, controll, out var listType);
			if (listResult == null)
			{
				throw new NullReferenceException("foreach collection is null");
			}
			if (!(listResult is IEnumerable en))
			{
				throw new Exception($"invalid foreach collection {listType}");
			}
			// 
			object bodyResult = null;
			Type bodyType = null;
			if (this.Body != null)
			{
				var tempContext = ScriptContext.Create(context);
				var tempController = new EvalControl(controll, true);
				// 定义变量
				this.VarDefine.Eval(tempContext, options, out var varType);
				// 循环
				foreach (var item in en)
				{
					tempContext.SetVar(this.VarDefine.Name, item, item == null ? varType : null);
					bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
					if (tempController.Terminal || tempController.Break) break;
					tempController.Continue = false;
				}
			}
			returnType = bodyType;
			return bodyResult;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();
			var listExpression = this.Collection.Build(tempBuildContext, scriptContext, options);

			var getEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(ScriptUtils.GetElementType(listExpression.Type)).GetMethod("GetEnumerator");
			var getEnumerator = Expression.Call(listExpression, getEnumeratorMethod);
			var enumerator = Expression.Variable(getEnumerator.Method.ReturnType);
			var currentProperty = enumerator.Type.GetProperty("Current");
			var moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
			this.VarDefine.SystemType = currentProperty.PropertyType;
			var itemVar = this.VarDefine.Build(tempBuildContext, scriptContext, options);
			// 
			var bodyBuildContext = new BuildContext(tempBuildContext)
			{
				ContinueLabel = continueLabel,
				BreakLabel = breakLabel
			};
			var body = this.Body.Build(bodyBuildContext, scriptContext, options);
			// 
			var loopBody = Expression.Block(
				Expression.IfThenElse(
					Expression.Call(enumerator, moveNextMethod),
					bodyBuildContext.BuildBlock(scriptContext, options,
						Expression.Assign(itemVar, Expression.Property(enumerator, currentProperty)),
						body,
						Expression.Label(continueLabel)),
					Expression.Break(breakLabel)
				));
			var loop = Expression.Loop(loopBody, breakLabel);
			return Expression.Block(new[] { enumerator },
				tempBuildContext.BuildBlock(scriptContext, options, Expression.Assign(enumerator, getEnumerator), loop));
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.VarDefine);
			PoolManage.Return(this.Collection);
			PoolManage.Return(this.Body);

			this.VarDefine = null;
			this.Collection = null;
			this.Body = null;
		}
	}
}
