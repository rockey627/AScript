using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class StringConcatNode : TreeNode
	{
		public IList<ITreeNode> Args { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.Args == null || this.Args.Count == 0)
			{
				return ExpressionUtils.Constant_null;
			}
			var arr = new Expression[this.Args.Count];
			for (int i = 0; i < this.Args.Count; i++)
			{
				var v = this.Args[i].Build(buildContext, scriptContext, options);
				Expression vs;
				if (v.Type.IsValueType)
				{
					vs = Expression.Call(v, ExpressionUtils.Method_Object_ToString);
				}
				else
				{
					var testNull = Expression.ReferenceEqual(v, ExpressionUtils.Constant_null);
					vs = Expression.Condition(testNull, ExpressionUtils.Constant_string_empty, Expression.Call(v, ExpressionUtils.Method_Object_ToString));
				}
				arr[i] = vs;
			}
			var arrExpr = Expression.NewArrayInit(typeof(string), arr);
			return Expression.Call(null, ExpressionUtils.Method_String_Concat_list, arrExpr);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			returnType = typeof(string);
			if (this.Args == null || this.Args.Count == 0)
			{
				return null;
			}
			var arr = new string[this.Args.Count];
			for (int i = 0; i < this.Args.Count; i++)
			{
				arr[i] = this.Args[i].Eval(context, options, control, out _)?.ToString();
			}
			return string.Concat(arr);
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Args);

			this.Args = null;
		}
	}
}
