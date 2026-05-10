using AScript.Nodes;
using System;

namespace AScript
{
	public class CustomFunctionObject : IFunctionObject
	{
		public CustomFunction Function;
		private readonly ScriptContext _scriptContext;

		private ITreeNode[] _nodes;

		public CustomFunctionObject(CustomFunction customFunction, ScriptContext scriptContext)
		{
			this.Function = customFunction;
			this._scriptContext = scriptContext;
		}

		public object DynamicInvoke(params object[] args)
		{
			return DynamicInvoke(_scriptContext, args);
		}

		public object DynamicInvoke(ScriptContext context, params object[] args)
		{
			if (args != null && args.Length > 0)
			{
				if (_nodes == null || _nodes.Length != args.Length)
				{
					_nodes = new ITreeNode[args.Length];
				}
				for (int i = 0; i < args.Length; i++)
				{
					_nodes[i] = PoolManage.CreateObjectNode(args[i]);
				}
			}
			try
			{
				return Function.Eval(context, null, null, _nodes, out _);
			}
			finally
			{
				if (_nodes != null && _nodes.Length > 0)
				{
					PoolManage.Return(_nodes);
				}
			}
		}

		public Delegate CreateDelegate(Type delegateType, BuildOptions options, Type[] argTypes, Type returnType)
		{
			return Script.Lambda(delegateType, _scriptContext, options, Function.Body, argTypes ?? Function.ArgTypes, Function.ArgNames, returnType ?? Function.ReturnType).Compile();
		}

		public Delegate CreateDelegate(Type delegateType, BuildOptions options)
		{
			return CreateDelegate(delegateType, options, Function.ArgTypes, Function.ReturnType);
		}
	}
}
