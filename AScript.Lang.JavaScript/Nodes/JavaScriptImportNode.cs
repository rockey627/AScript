using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AScript.Lang.JavaScript.Nodes
{
	public class JavaScriptImportNode : TreeNode
	{
		public string FromModule { get; set; }
		public IList<string> DefaultVariables { get; set; }
		public IList<VariableItem> Variables { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var module = InstallModule(scriptContext, this.FromModule);
			var statements = new List<Expression>((this.DefaultVariables?.Count ?? 0) + (this.Variables?.Count ?? 0));
			if (this.DefaultVariables != null)
			{
				var defaultValue = Expression.Constant(module?.Default);
				foreach (var defaultVariable in this.DefaultVariables)
				{
					var variable = Expression.Variable(defaultValue.Type);
					buildContext.Variables[defaultVariable] = variable;
					buildContext.VariableModifiers[defaultVariable] = Modifiers.READONLY;
					buildContext.LocalVariables.Add(defaultVariable);
					statements.Add(Expression.Assign(variable, defaultValue));
				}
			}
			if (this.Variables != null)
			{
				for (int i = 0; i < this.Variables.Count; i++)
				{
					string name = this.Variables[i].Name;
					string alias = this.Variables[i].Alias;
					if (name == "*")
					{
						var variable = Expression.Variable(typeof(ReadOnlyDictionary<string, object>));
						buildContext.Variables[alias] = variable;
						buildContext.VariableModifiers[alias] = Modifiers.READONLY;
						buildContext.LocalVariables.Add(alias);
						if (module == null)
						{
							statements.Add(Expression.Assign(variable, Expression.Constant(null, variable.Type)));
						}
						else
						{
							statements.Add(Expression.Assign(variable, Expression.Constant(new ReadOnlyDictionary<string, object>(module.NamedDict))));
						}
					}
					else
					{
						object value;
						if (module == null)
						{
							value = null;
						}
						else
						{
							module.NamedDict.TryGetValue(name, out value);
						}
						var variable = Expression.Variable(value?.GetType() ?? typeof(object));
						buildContext.Variables[alias] = variable;
						buildContext.VariableModifiers[alias] = Modifiers.READONLY;
						buildContext.LocalVariables.Add(alias);
						statements.Add(Expression.Assign(variable, Expression.Constant(value)));
					}
				}
			}
			if (statements.Count == 0)
			{
				return Expression.Empty();
			}
			if (statements.Count == 1)
			{
				return statements[0];
			}
			return Expression.Block(statements);
		}

		private static JavaScriptExportModule InstallModule(ScriptContext context, string moduleName)
		{
			string key = $"__export_module_{moduleName}__";
			var module = (JavaScriptExportModule)context.EvalVar(key);
			if (module == null)
			{
				var moduleContext = new ScriptContext { Langs = context.Langs };
				moduleContext.InstallModule(moduleName);
				module = JavaScriptExportModule.GetInstance(moduleContext);
				context.SetConst(key, module);
			}
			return module;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var module = InstallModule(context, this.FromModule);

			if (this.DefaultVariables != null)
			{
				foreach (var defaultVariable in this.DefaultVariables)
				{
					context.SetConst(defaultVariable, module?.Default);
				}
			}
			if (this.Variables != null)
			{
				for (int i = 0; i < this.Variables.Count; i++)
				{
					string name = this.Variables[i].Name;
					string alias = this.Variables[i].Alias;
					if (name == "*")
					{
						if (module == null)
						{
							context.SetConst(alias, null);
						}
						else
						{
							context.SetConst(alias, new ReadOnlyDictionary<string, object>(module.NamedDict));
						}
					}
					else
					{
						if (module == null)
						{
							context.SetConst(alias, null);
						}
						else
						{
							module.NamedDict.TryGetValue(name, out var value);
							context.SetConst(alias, value);
						}
					}
				}
			}

			returnType = typeof(void);
			return null;
		}

		public override void Clear()
		{
			base.Clear();

			this.FromModule = null;
			if (this.DefaultVariables != null)
			{
				this.DefaultVariables.Clear();
				this.DefaultVariables = null;
			}
			if (this.Variables != null)
			{
				this.Variables.Clear();
				this.Variables = null;
			}
		}

		public struct VariableItem
		{
			public string Name;
			public string Alias;

			public VariableItem(string name, string alias)
			{
				this.Name = name;
				this.Alias = alias;
			}
		}
	}
}
