using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript
{
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
