using System;
using System.Collections.Generic;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptExportModule
	{
		private readonly ScriptContext _context;

		public Dictionary<string, object> NamedDict { get; private set; } = new Dictionary<string, object>();
		public object exports { get; set; }

		public JavaScriptExportModule(ScriptContext context)
		{
			_context = context;
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

		///// <summary>
		///// 动态获取值
		///// </summary>
		///// <param name="name"></param>
		///// <param name="result"></param>
		///// <returns></returns>
		//public bool TryGetValue(string name, out object result)
		//{
		//	if (this.Names.Contains(name))
		//	{
		//		result = _context.EvalVar(name, out var type, searchParent: false);
		//		return type != null;
		//	}
		//	result = null;
		//	return false;
		//}

		//public override bool TryGetMember(GetMemberBinder binder, out object result)
		//{
		//	if (TryGetMember(binder.Name, out result)) return true;
		//	return base.TryGetMember(binder, out result);
		//}


	}
}
