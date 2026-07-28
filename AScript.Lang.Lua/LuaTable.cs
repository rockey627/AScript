using System;
using System.Collections.Generic;
using System.Dynamic;

namespace AScript.Lang.Lua
{
	public class LuaTable : DynamicObject
	{
		private readonly List<object> _list = new List<object>();
		private readonly Dictionary<object, object> _dict = new Dictionary<object, object>();

		public object this[object key]
		{
			get
			{
				if (key is int index)
				{
					if (index >= 1 && index <= _list.Count)
					{
						return _list[index - 1];
					}
				}
				else if (key is long longIndex)
				{
					if (longIndex >= 1 && longIndex <= _list.Count)
					{
						return _list[(int)longIndex - 1];
					}
				}
				else if (_dict.TryGetValue(key, out var value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (key is int index)
				{
					if (index >= 1)
					{
						while (_list.Count < index)
						{
							_list.Add(null);
						}
						_list[index - 1] = value;
					}
				}
				else if (key is long longIndex)
				{
					if (longIndex >= 1)
					{
						while (_list.Count < longIndex)
						{
							_list.Add(null);
						}
						_list[(int)longIndex - 1] = value;
					}
				}
				else
				{
					if (value == null)
					{
						_dict.Remove(key);
					}
					else
					{
						_dict[key] = value;
					}
				}
			}
		}

		public static object remove(LuaTable table, object key)
		{
			if (key is int index)
			{
				if (index >= 1 && index <= table._list.Count)
				{
					var value = table._list[index - 1];
					table._list.RemoveAt(index - 1);
					return value;
				}
			}
			else if (key is long longIndex)
			{
				if (longIndex >= 1 && longIndex <= table._list.Count)
				{
					var value = table._list[(int)longIndex - 1];
					table._list.RemoveAt((int)longIndex - 1);
					return value;
				}
			}
			else if (table._dict.TryGetValue(key, out var value))
			{
				table._dict.Remove(key);
				return value;
			}
			return null;
		}

		public static object remove(LuaTable table, long index)
		{
			if (index >= 1 && index <= table._list.Count)
			{
				var value = table._list[(int)index - 1];
				table._list.RemoveAt((int)index - 1);
				return value;
			}
			return null;
		}

		public IList<object> Array => _list;

		//public object dynamic_get(object key)
		//{
		//	return this[key];
		//}

		//public void dynamic_set(object key, object value)
		//{
		//	this[key] = value;
		//}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _dict.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_dict[binder.Name] = value;
			return true;
		}
	}
}
