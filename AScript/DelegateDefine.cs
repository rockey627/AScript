using System;
using System.Linq.Expressions;

namespace AScript
{
	/// <summary>
	/// int fib(int n)
	/// </summary>
	public class DelegateDefine
	{
		public string Name { get; private set; }
		public Type[] ArgTypes { get; private set; }
		public Type ReturnType { get; private set; }
		public ParameterExpression Variable { get; set; }

		public DelegateDefine(string name, Type[] argTypes, Type returnType)
		{
			this.Name = name;
			this.ArgTypes = argTypes;
			this.ReturnType = returnType;
		}
	}
}
