using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AScript.Lang.JavaScript
{
	/// <summary>
	/// js导出模块
	/// </summary>
	public class JavaScriptExportModule
	{
		private readonly ScriptContext _context;
		private IDictionary<string, object> _named;

		/// <summary>
		/// 命名导出
		/// </summary>
		public IDictionary<string, object> named
		{
			get
			{
				if (_named == null)
				{
					_named = new Dictionary<string, object>();
				}
				return _named;
			}
			set => _named = value;
		}
		/// <summary>
		/// 默认导出
		/// </summary>
		public object exports { get; set; }

		public JavaScriptExportModule() { }
		public JavaScriptExportModule(ScriptContext context)
		{
			_context = context;
		}

		public JavaScriptExportModule Export(string name, object value)
		{
			this.named[name] = value;
			return this;
		}

		public JavaScriptExportModule Export<T>(string name, T value)
		{
			this.named[name] = value;
			return this;
		}

		public JavaScriptExportModule ExportDefault(object value)
		{
			this.exports = value;
			return this;
		}

		public JavaScriptExportModule ExportDefault<T>(T value)
		{
			this.exports = value;
			return this;
		}

		public JavaScriptExportModule ExportDefaultByNamed(params string[] names)
		{
			return ExportDefaultByNamed((IEnumerable<string>)names);
		}

		public JavaScriptExportModule ExportDefaultByNamed(IEnumerable<string> names)
		{
			IDictionary<string, object> value = new ExpandoObject();
			foreach (var name in names)
			{
				value[name] = this.named[name];
			}
			return ExportDefault(value);
		}

		public static JavaScriptExportModule InstallModule(ScriptContext context, string moduleName)
		{
			string key = $"__export_module_{moduleName}__";
			var module = (JavaScriptExportModule)context.EvalVar(key);
			if (module == null)
			{
				var m = context.GetModule(moduleName);
				if (m is FileScriptModule)
				{
					var moduleContext = new ScriptContext(context);
					moduleContext.InstallModule(moduleName, m);
					module = GetOrCreateInstance(moduleContext);
				}
				else
				{
					var v = context.InstallModule(moduleName, m);
					if (v is JavaScriptExportModule jsm)
					{
						module = jsm;
					}
					else
					{
						module = new JavaScriptExportModule(context) { exports = v };
					}
				}
				context.SetConst(key, module);
			}
			return module;
		}

		public static JavaScriptExportModule GetInstance(ScriptContext context)
		{
			return (JavaScriptExportModule)context.EvalVar("__export_module__", searchParent: false);
		}

		public static JavaScriptExportModule GetOrCreateInstance(ScriptContext context)
		{
			var module = GetInstance(context);
			if (module == null)
			{
				module = new JavaScriptExportModule(context);
				context.SetConst("__export_module__", module);
			}
			return module;
		}
	}
}
