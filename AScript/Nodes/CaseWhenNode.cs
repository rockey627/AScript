using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class CaseWhenNode : TreeNode
	{
		public ITreeNode CaseValue { get; set; }
		public ITreeNode ElseBody { get; set; }
		/// <summary>
		/// (testValue, body)
		/// </summary>
		public IList<Tuple<IList<ITreeNode>, ITreeNode>> Whens { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var switchValue = this.CaseValue.Build(buildContext, scriptContext, options);
			var blockNode = new BlockNode();
			Expression defaultBody;
			if (this.ElseBody == null) defaultBody = null;
			else
			{
				blockNode.Block = this.ElseBody;
				defaultBody = blockNode.Build(buildContext, scriptContext, options);
			}
			if (this.Whens == null || this.Whens.Count == 0)
			{
				return defaultBody;
			}

			// if-then-else 链，以支持 EF Core 翻译
			// case p.Age when 20 then 1 else 2 end =>  p.Age == 20 ? 1 : 2
			Expression result = defaultBody;

			for (int i = this.Whens.Count - 1; i >= 0; i--)
			{
				var c = this.Whens[i];
				// 生成 switchValue == testValue 条件
				var testExpressions = c.Item1.Select(a => a.Build(buildContext, scriptContext, options)).ToList();
				Expression condition;
				if (testExpressions.Count == 1)
				{
					condition = MakeEqualExpression(switchValue, testExpressions[0]);
				}
				else
				{
					// 多个测试值用 OR 连接: switchValue == v1 || switchValue == v2
					condition = null;
					foreach (var testExpr in testExpressions)
					{
						var eq = MakeEqualExpression(switchValue, testExpr);
						condition = condition == null ? eq : Expression.OrElse(condition, eq);
					}
				}
				blockNode.Block = c.Item2;
				var body = blockNode.Build(buildContext, scriptContext, options);
				if (result == null)
				{
					result = Expression.Constant(ScriptUtils.GetDefaultValue(body.Type));
				}
				result = Expression.Condition(condition, body, result);
			}
			return result;
		}

		///<summary>
		/// 创建相等比较表达式，自动处理类型不匹配的情况
		/// </summary>
		private static Expression MakeEqualExpression(Expression left, Expression right)
		{
			if (left.Type == right.Type)
			{
				return Expression.Equal(left, right);
			}
			// 类型不匹配时，尝试转换为共同类型
			if (left.Type == typeof(object))
			{
				left = Expression.Convert(left, right.Type);
			}
			else if (right.Type == typeof(object))
			{
				right = Expression.Convert(right, left.Type);
			}
			return Expression.Equal(left, right);
		}

		//public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		//{
		//	//var tempBuildContext = new BuildContext(buildContext)
		//	//{
		//	//	BreakLabel = Expression.Label()
		//	//};
		//	var switchValue = this.SwitchValue.Build(buildContext, scriptContext, options);
		//	var blockNode = new BlockNode();
		//	Expression defaultBody;
		//	if (this.DefaultBody == null) defaultBody = null;
		//	else
		//	{
		//		blockNode.Block = this.DefaultBody;
		//		defaultBody = blockNode.Build(buildContext, scriptContext, options);
		//	}
		//	SwitchCase[] cases = null;
		//	if (this.Cases != null)
		//	{
		//		cases = new SwitchCase[this.Cases.Count];
		//		for (int i = 0; i < this.Cases.Count; i++)
		//		{
		//			var c = this.Cases[i];
		//			var test = c.Item1.Select(a => a.Build(buildContext, scriptContext, options)).ToList();
		//			blockNode.Block = c.Item2;
		//			var body = blockNode.Build(buildContext, scriptContext, options);
		//			cases[i] = Expression.SwitchCase(body, test);
		//		}
		//	}
		//	//if (this.ReturnValue)
		//	//{
		//	//	Type returnType;
		//	//	if (defaultBody != null)
		//	//	{
		//	//		returnType = defaultBody.Type;
		//	//	}
		//	//	else if (cases != null && cases.Length > 0)
		//	//	{
		//	//		returnType = cases[0].Body.Type;
		//	//	}
		//	//	else
		//	//	{
		//	//		return null;
		//	//	}
		//	//	var v = Expression.Variable(returnType);
		//	//	var defaultExpr = defaultBody == null ? null : Expression.Assign(v, defaultBody);
		//	//	if (cases != null && cases.Length > 0)
		//	//	{
		//	//		for (int i = 0; i < cases.Length; i++)
		//	//		{
		//	//			var c = cases[i];
		//	//			var assignExpr = Expression.Assign(v, c.Body);
		//	//			c.Update(c.TestValues, assignExpr);
		//	//		}
		//	//	}
		//	//	if (defaultExpr == null)
		//	//	{
		//	//		return Expression.Block(new[] { v }, Expression.Switch(switchValue, cases), v);
		//	//	}
		//	//	return Expression.Block(new[] { v }, Expression.Switch(switchValue, defaultExpr, cases), v);
		//	//}
		//	if (defaultBody == null)
		//	{
		//		Type returnType;
		//		if (cases != null && cases.Length > 0)
		//		{
		//			returnType = cases[0].Body.Type;
		//		}
		//		else returnType = null;
		//		if (returnType != null && returnType != typeof(void))
		//		{
		//			defaultBody = Expression.Constant(ScriptUtils.GetDefaultValue(returnType));
		//		}
		//	}
		//	if (defaultBody == null)
		//	{
		//		return Expression.Switch(switchValue, cases);
		//	}
		//	return Expression.Switch(switchValue, defaultBody, cases);
		//}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var switchValue = this.CaseValue.Eval(context, options, control, out _);
			//var tempController = new EvalControl(control, true);
			if (this.Whens != null)
			{
				for (int i = 0; i < this.Whens.Count; i++)
				{
					var c = this.Whens[i];
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
			if (this.ElseBody != null)
			{
				return new BlockNode(this.ElseBody).Eval(context, options, control, out returnType);
			}
			returnType = null;
			return null;
		}
	}
}
