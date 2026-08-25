using System;
using System.Collections.Generic;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptExportModule
	{
		private readonly ScriptContext _context;

		public Dictionary<string, object> NamedDict { get; private set; } = new Dictionary<string, object>();
		public object Default { get; set; }

		public JavaScriptExportModule(ScriptContext context)
		{
			_context = context;
		}

		public static JavaScriptExportModule GetInstance(ScriptContext context)
		{
			return (JavaScriptExportModule)context.EvalVar("__module__", searchParent: false);
		}

		public static JavaScriptExportModule GetOrCreateInstance(ScriptContext context)
		{
			var module = GetInstance(context);
			if (module == null)
			{
				module = new JavaScriptExportModule(context);
				context.SetConst("__module__", module);
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
