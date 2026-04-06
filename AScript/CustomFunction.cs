using AScript.Nodes;
using System;
using System.Collections.Generic;

namespace AScript
{
	public class CustomFunction
	{
		public string Name { get; private set; }
		public string[] ArgNames { get; private set; }
		public Type[] ArgTypes { get; private set; }
		public Type ReturnType { get; private set; }
		public ITreeNode Body { get; private set; }

		public CustomFunction(string name, Type returnType, string[] argNames, Type[] argTypes, ITreeNode body)
		{
			this.Name = name;
			this.ReturnType = returnType;
			this.ArgNames = argNames;
			this.ArgTypes = argTypes;
			this.Body = body;
		}

		public object Eval(ScriptContext context, BuildOptions options, EvalControl control, IList<ITreeNode> args, out Type returnType)
		{
			//var tempContext = ScriptContext.Create(context);
			var tempContext = context;
			// 填充参数
			if (this.ArgNames != null)
			{
				for (int i = 0; i < this.ArgNames.Length; i++)
				{
					tempContext.SetVar(this.ArgNames[i], args[i].Eval(context, options, control, out var type), type);
				}
			}
			if (this.Body == null)
			{
				returnType = null;
				return null;
			}
			return this.Body.Eval(tempContext, options, new EvalControl(), out returnType);
		}
	}
}
