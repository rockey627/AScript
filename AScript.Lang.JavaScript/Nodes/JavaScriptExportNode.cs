using AScript.Nodes;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.JavaScript.Nodes
{
	public class JavaScriptExportNode : TreeNode
	{
		private static readonly MethodInfo Method_JavaScriptExportModule_GetOrCreateInstance = typeof(JavaScriptExportModule).GetMethod("GetOrCreateInstance", BindingFlags.Public | BindingFlags.Static);

		public string Name { get; set; }
		public bool Default { get; set; }
		public ITreeNode Value { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			// 调用 GetOrCreateInstance(scriptContext) 获取 module
			var moduleExpr = Expression.Call(Method_JavaScriptExportModule_GetOrCreateInstance, buildContext.GetScriptContextParameter());
			var module = Expression.Variable(moduleExpr.Type);
			var moduleAssign = Expression.Assign(module, moduleExpr);

			// 对 Value 调用 Build 获取值的表达式
			var valueExpr = this.Value.Build(buildContext, scriptContext, options);
			var value = Expression.Variable(valueExpr.Type);
			var valueAssign = Expression.Assign(value, valueExpr);

			if (this.Default)
			{
				// module.exports = valueExpr
				var assign = Expression.Assign(Expression.Property(module, "exports"), value);
				return Expression.Block(new[] { module, value }, moduleAssign, valueAssign, assign, value);
			}
			else
			{
				// module.NamedDict[this.Name] = valueExpr
				var namedDictExpr = Expression.Property(module, "NamedDict");
				var assign = Expression.Assign(Expression.Property(namedDictExpr, ScriptUtils.Property_IDictionary_String_Object_Item, Expression.Constant(this.Name)), value);
				return Expression.Block(new[] { module, value }, moduleAssign, valueAssign, assign, value);
			}
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var module = JavaScriptExportModule.GetOrCreateInstance(context);
			var value = this.Value.Eval(context, options, control, out returnType);
			if (this.Default)
			{
				module.exports = value;
			}
			else
			{
				module.named[this.Name] = value;
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
