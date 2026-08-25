using System;

namespace AScript
{
	public class DynamicVariable
	{
		private readonly ScriptContext _context;
		private readonly string _name;
		private readonly bool _searchParent;

		public DynamicVariable(ScriptContext context, string name, bool searchParent)
		{
			_context = context;
			_name = name;
			_searchParent = searchParent;
		}

		public object GetValue()
		{
			return GetValue(out _);
		}

		public object GetValue(out Type type)
		{
			return _context.EvalVar(_name, out type, _searchParent);
		}
	}
}
