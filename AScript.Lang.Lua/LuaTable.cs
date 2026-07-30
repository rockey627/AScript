using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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

		// table.insert(t, value) - 在列表末尾插入元素
		public static void insert(LuaTable table, object value)
		{
			insert(table, table._list.Count + 1, value);
		}

		// table.insert(t, pos, value) - 在指定位置插入元素
		// pos: 1-based 索引，支持负数（-1 表示倒数第一个元素之前）
		public static void insert(LuaTable table, object pos, object value)
		{
			var position = Convert.ToInt64(pos);
			insert(table, position, value);
		}

		public static void insert(LuaTable table, long pos, object value)
		{
			var count = table._list.Count;

			// 处理负数索引（Lua 风格：-1 表示最后一个元素之后）
			var actualPos = pos;
			if (pos < 0)
			{
				// 负数索引：-1 插入到末尾，-2 插入到倒数第一个元素之前
				actualPos = count + pos + 1;
			}

			// 边界处理
			if (actualPos < 1)
				actualPos = 1;
			if (actualPos > count + 1)
				actualPos = count + 1;

			table._list.Insert((int)actualPos - 1, value);
		}

		/// <summary>
		/// 移除最后一个元素
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static object remove(LuaTable table)
		{
			if (table._list.Count > 0)
			{
				int index = table._list.Count - 1;
				var value = table._list[index];
				table._list.RemoveAt(index);
				return value;
			}
			return null;
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
					if (value is bool b)
					{
						sb.Append(b ? "true" : "false");
					}
					else
					{
						sb.Append(value.ToString());
					}
				}
			}
			return sb.ToString();
		}

		// table.sort(table) - 对列表进行排序（升序）
		public static void sort(LuaTable table)
		{
			sort(table, null);
		}

		// table.sort(table, comp) - 使用自定义比较函数排序
		// comp(a, b) 返回 true 表示 a < b
		public static void sort(LuaTable table, Func<object, object, bool> comp)
		{
			if (table._list.Count <= 1)
				return;

			if (comp == null)
			{
				// 默认升序排序
				table._list.Sort(CompareDefault);
			}
			else
			{
				// 使用自定义比较函数
				// 注意：这里假设 comp 是一个可调用的委托/函数
				// 在实际实现中可能需要通过脚本引擎调用
				table._list.Sort((a, b) => comp(a, b) ? -1 : 1);
			}
		}

		// 默认比较函数：用于升序排序
		// 规则：数字 < 字符串 < 布尔 < 其他
		// 同类型之间使用 < 比较
		private static int CompareDefault(object a, object b)
		{
			if (a == null && b == null) return 0;
			if (a == null) return -1;
			if (b == null) return 1;

			// 类型优先级：数字(1) < 字符串(2) < 布尔(3) < 其他(4)
			var typeRankA = GetTypeRank(a);
			var typeRankB = GetTypeRank(b);

			if (typeRankA != typeRankB)
				return typeRankA.CompareTo(typeRankB);

			// 同类型比较
			if (a is long longA && b is long longB)
				return longA.CompareTo(longB);
			if (a is double doubleA && b is double doubleB)
				return doubleA.CompareTo(doubleB);
			if (a is string strA && b is string strB)
				return string.CompareOrdinal(strA, strB);
			if (a is bool boolA && b is bool boolB)
				return boolA.CompareTo(boolB);

			// 对于其他类型，使用 ToString 进行比较
			return string.CompareOrdinal(a.ToString(), b.ToString());
		}

		private static int GetTypeRank(object obj)
		{
			if (obj is long || obj is int || obj is double || obj is float)
				return 1;
			if (obj is string)
				return 2;
			if (obj is bool)
				return 3;
			return 4;
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
