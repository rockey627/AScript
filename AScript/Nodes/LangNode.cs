using System;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class LangNode : TreeNode
	{
		public string[] Langs { get; set; }
		public ITreeNode Body { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			if (this.Body == null) return null;
			var oldLangs = scriptContext.Langs;
			var oldDynamic = buildContext.Dynamic;
			scriptContext.Langs = this.Langs;
			buildContext.Dynamic = scriptContext.Dynamic ?? scriptContext.IsDynamicLang();
			try
			{
				return this.Body.Build(buildContext, scriptContext, options);
			}
			finally
			{
				scriptContext.Langs = oldLangs;
				buildContext.Dynamic = oldDynamic;
			}
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.Body == null)
			{
				returnType = null;
				return null;
			}
			var oldLangs = context.Langs;
			context.Langs = this.Langs;
			try
			{
				return this.Body.Eval(context, options, control, out returnType);
			}
			finally
			{
				context.Langs = oldLangs;
			}
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Body);

			this.Langs = null;
			this.Body = null;
		}
	}
}
