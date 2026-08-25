using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Nodes
{
	public class JavaScriptExportNode : TreeNode
	{
		public string Name { get; set; }
		public bool Default { get; set; }
		public ITreeNode Value { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var module = JavaScriptExportModule.GetOrCreateInstance(context);
			var value = this.Value.Eval(context, options, control, out returnType);
			if (this.Default)
			{
				module.Default = value;
			}
			else
			{
				module.NamedDict[this.Name] = value;
			}
			return value;
		}

		public override void Clear()
		{
			base.Clear();

			this.Name = null;
			this.Default = false;
			this.Value?.Clear();
		}
	}
}
