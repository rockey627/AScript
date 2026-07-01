using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AScript.Lang.JavaScript.Extensions
{
	public class JavaScriptJsonExtensionObject : IScriptExtensionObject
	{
		public void Init(BaseContext context)
		{
			context.FunctionEval += Context_FunctionEval;
			context.AddType("JSON", typeof(JsonConvert));
			context.AddFunc(typeof(JavaScriptJsonExtensions));
		}

		private void Context_FunctionEval(object sender, FunctionEvalArgs e)
		{
			if (e.IsHandled) return;

			if (string.IsNullOrEmpty(e.Name) || e.Name == "_") return;
			if (e.Args == null || e.Args.Count != 1) return;
			e.EvalArgs(false);
			if (!(e.ArgValues[0] is JToken json)) return;
			if (e.Name.StartsWith("get_"))
			{
				string property = e.Name.Substring(4);
				var v = json[property];
				if (v is JValue jv)
				{
					e.SetResult(jv.Value);
				}
				else e.SetResult(v);
				return;
			}
		}
	}
}
