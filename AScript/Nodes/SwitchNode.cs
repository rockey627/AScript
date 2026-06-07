using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class SwitchNode : TreeNode
	{
		//public bool ReturnValue { get; set; }
		//public bool AutoBreak { get; set; }
		public ITreeNode SwitchValue { get; set; }
		public ITreeNode DefaultBody { get; set; }
		/// <summary>
		/// (testValue, body)
		/// </summary>
		public IList<Tuple<IList<ITreeNode>, ITreeNode>> Cases { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			//var tempBuildContext = new BuildContext(buildContext)
			//{
			//	BreakLabel = Expression.Label()
			//};
			var switchValue = this.SwitchValue.Build(buildContext, scriptContext, options);
			var blockNode = new BlockNode();
			Expression defaultBody;
			if (this.DefaultBody == null) defaultBody = null;
			else
			{
				blockNode.Block = this.DefaultBody;
				defaultBody = blockNode.Build(buildContext, scriptContext, options);
			}
			SwitchCase[] cases = null;
			if (this.Cases != null)
			{
				cases = new SwitchCase[this.Cases.Count];
				for (int i = 0; i < this.Cases.Count; i++)
				{
					var c = this.Cases[i];
					var test = c.Item1.Select(a => a.Build(buildContext, scriptContext, options)).ToList();
					blockNode.Block = c.Item2;
					var body = blockNode.Build(buildContext, scriptContext, options);
					cases[i] = Expression.SwitchCase(body, test);
				}
			}
			//if (this.ReturnValue)
			//{
			//	Type returnType;
			//	if (defaultBody != null)
			//	{
			//		returnType = defaultBody.Type;
			//	}
			//	else if (cases != null && cases.Length > 0)
			//	{
			//		returnType = cases[0].Body.Type;
			//	}
			//	else
			//	{
			//		return null;
			//	}
			//	var v = Expression.Variable(returnType);
			//	var defaultExpr = defaultBody == null ? null : Expression.Assign(v, defaultBody);
			//	if (cases != null && cases.Length > 0)
			//	{
			//		for (int i = 0; i < cases.Length; i++)
			//		{
			//			var c = cases[i];
			//			var assignExpr = Expression.Assign(v, c.Body);
			//			c.Update(c.TestValues, assignExpr);
			//		}
			//	}
			//	if (defaultExpr == null)
			//	{
			//		return Expression.Block(new[] { v }, Expression.Switch(switchValue, cases), v);
			//	}
			//	return Expression.Block(new[] { v }, Expression.Switch(switchValue, defaultExpr, cases), v);
			//}
			if (defaultBody == null)
			{
				Type returnType;
				if (cases != null && cases.Length > 0)
				{
					returnType = cases[0].Body.Type;
				}
				else returnType = null;
				if (returnType != null && returnType != typeof(void))
				{
					defaultBody = Expression.Constant(ScriptUtils.GetDefaultValue(returnType));
				}
			}
			if (defaultBody == null)
			{
				return Expression.Switch(switchValue, cases);
			}
			return Expression.Switch(switchValue, defaultBody, cases);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var switchValue = this.SwitchValue.Eval(context, options, control, out _);
			//var tempController = new EvalControl(control, true);
			if (this.Cases != null)
			{
				for (int i = 0; i < this.Cases.Count; i++)
				{
					var c = this.Cases[i];
					var t = c.Item1.Select(a => a.Eval(context, options, control, out _)).Distinct();
					var set = new HashSet<object>();
					foreach (var item in t)
					{
						set.Add(item);
					}
					if (set.Contains(switchValue))
					{
						return new BlockNode(c.Item2).Eval(context, options, control, out returnType);
						//var v = c.Item2.Eval(context, options, control, out returnType);
						//if ((this.AutoBreak || tempController.Break || tempController.Terminal))
						//{
						//	return v;
						//}
					}
				}
			}
			if (this.DefaultBody != null)
			{
				return new BlockNode(this.DefaultBody).Eval(context, options, control, out returnType);
			}
			returnType = null;
			return null;
		}
	}
}
