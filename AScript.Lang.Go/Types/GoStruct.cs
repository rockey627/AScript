using System;
using System.Collections.Generic;

namespace AScript.Lang.Go.Types
{
	public class GoStruct
	{
		public string Name { get; private set; }
		public List<GoStruct> Bases { get; private set; } = new List<GoStruct>();
		public IDictionary<string, Type> Fields { get; private set; } = new Dictionary<string, Type>();
		public IDictionary<string, List<Delegate>> Methods { get; private set; } = new Dictionary<string, List<Delegate>>();

		public GoStruct(string name)
		{
			Name = name;
		}

		public GoStruct AddBase(GoStruct baseStruct)
		{
			Bases.Add(baseStruct);
			return this;
		}

		public GoStruct AddField(string name, Type type)
		{
			Fields[name] = type;
			return this;
		}

		public GoStruct AddMethod(string name, Delegate method)
		{
			if (!Methods.TryGetValue(name, out var list))
			{
				list = new List<Delegate>();
				Methods[name] = list;
			}
			list.Add(method);
			return this;
		}

		public GoStructInstance CreateInstance()
		{
			return new GoStructInstance(this);
		}

		public T CreateInstance<T>() where T : GoStructInstance
		{
			return (T)Activator.CreateInstance(typeof(T), this);
		}
	}
}
