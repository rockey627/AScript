using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

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

		// table.concat(list) - 连接表中所有字符串元素
		// table.concat(list, sep) - 使用分隔符连接
		// table.concat(list, sep, i) - 从第 i 个元素开始连接
		// table.concat(list, sep, i, j) - 连接从 i 到 j 的元素
		public static string concat(LuaTable table)
		{
			return concat(table, null, 1, table._list.Count);
		}

		public static string concat(LuaTable table, object sep)
		{
			return concat(table, (string)sep, 1L, table._list.Count);
		}

		public static string concat(LuaTable table, object sep, object start)
		{
			var startIdx = Convert.ToInt64(start);
			return concat(table, (string)sep, startIdx, table._list.Count);
		}

		public static string concat(LuaTable table, object sep, object start, object end)
		{
			var sepStr = (string)sep;
			var startIdx = Convert.ToInt64(start);
			var endIdx = Convert.ToInt64(end);
			return concat(table, sepStr, startIdx, endIdx);
		}

		public static string concat(LuaTable table, string sep, long start, long end)
		{
			if (table._list.Count == 0 || start > end)
				return "";

			// Lua 使用 1-based 索引，转换为 0-based
			var actualStart = start > 0 ? (int)start - 1 : Math.Max(0, table._list.Count + (int)start);
			var actualEnd = end > 0 ? (int)end - 1 : table._list.Count + (int)end;

			// 边界检查
			if (actualStart < 0) actualStart = 0;
			if (actualEnd >= table._list.Count) actualEnd = table._list.Count - 1;
			if (actualStart > actualEnd)
				return "";

			var sb = new StringBuilder();
			for (int i = actualStart; i <= actualEnd; i++)
			{
				if (i > actualStart && !string.IsNullOrEmpty(sep))
					sb.Append(sep);

				var value = table._list[i];
				if (value != null)
				{
					sb.Append(value.ToString());
				}
			}
			return sb.ToString();
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
