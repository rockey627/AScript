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

		public LuaTable Metatable
		{
			get
			{
				_dict.TryGetValue("__metatable", out var metatable);
				return (LuaTable)metatable;
			}
			set => this["__metatable"] = value;
		}

		public IList<object> Array => _list;

		public long ArrayLength
		{
			get
			{
				int n = 0;
				for (int i = 0; i < _list.Count; i++)
				{
					if (_list[i] == null) break;
					n++;
				}
				return n;
			}
		}

		public object this[object key]
		{
			get
			{
				if (key is int index)
				{
					if (index >= 1 && index <= _list.Count)
					{
						var result = _list[index - 1];
						if (result != null) return result;
					}
				}
				else if (key is long longIndex)
				{
					if (longIndex >= 1 && longIndex <= _list.Count)
					{
						var result = _list[(int)longIndex - 1];
						if (result != null) return result;
					}
				}
				else if (_dict.TryGetValue(key, out var value) && value != null)
				{
					return value;
				}
				// 
				if (TryGetFromIndex(key, out var value2))
				{
					return value2;
				}
				return null;
			}
			set
			{
				if (key is int index)
				{
					if (index >= 1)
					{
						if (_list.Count >= index && _list[index - 1] != null || !TrySetToNewIndex(key, value))
						{
							while (_list.Count < index)
							{
								_list.Add(null);
							}
							_list[index - 1] = value;
						}
					}
				}
				else if (key is long longIndex)
				{
					if (longIndex >= 1)
					{
						int baseIndex = (int)longIndex - 1;
						if (_list.Count > baseIndex && _list[baseIndex] != null || !TrySetToNewIndex(key, value))
						{
							while (_list.Count < longIndex)
							{
								_list.Add(null);
							}
							_list[baseIndex] = value;
						}
					}
				}
				else
				{
					if (_dict.ContainsKey(key) || !TrySetToNewIndex(key, value))
					{
						_dict[key] = value;
					}
				}
			}
		}

		private bool TryGetFromIndex(object key, out object value)
		{
			var indexObj = this.Metatable?["__index"];
			if (indexObj is LuaTable indexTable)
			{
				if (ReferenceEquals(indexTable, this))
				{
					value = null;
					return false;
				}
				value = indexTable[key];
				return true;
			}
			if (indexObj is Delegate del)
			{
				value = del.DynamicInvoke(this, key);
				return true;
			}
			if (indexObj is IFunctionObject functionObject)
			{
				value = functionObject.DynamicInvoke(this, key);
				return true;
			}
			value = null;
			return false;
		}

		private bool TrySetToNewIndex(object key, object value)
		{
			var indexObj = this.Metatable?["__newindex"];
			if (indexObj is LuaTable indexTable)
			{
				if (ReferenceEquals(indexTable, this))
				{
					return false;
				}
				indexTable[key] = value;
				return true;
			}
			if (indexObj is Delegate del)
			{
				del.DynamicInvoke(this, key, value);
				return true;
			}
			if (indexObj is IFunctionObject functionObject)
			{
				functionObject.DynamicInvoke(this, key, value);
				return true;
			}
			return false;
		}

		// table.insert(t, value) - 在列表末尾插入元素
		public static void insert(object table, object value)
		{
			var luaTable = (LuaTable)table;
			insert(luaTable, luaTable._list.Count + 1, value);
		}

		// table.insert(t, pos, value) - 在指定位置插入元素
		// pos: 1-based 索引，支持负数（-1 表示倒数第一个元素之前）
		public static void insert(object table, object pos, object value)
		{
			var position = Convert.ToInt64(pos);
			insert((LuaTable)table, position, value);
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

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _dict.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_dict[binder.Name] = value;
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var v = this[binder.Name];
			if (v is CustomFunctionObject customFunctionObject)
			{
				//v = customFunctionObject.Compile(null);
				//this[binder.Name] = v;
				var parametersLength = customFunctionObject.Function.ArgNames?.Length ?? 0;
				int argsLength = args == null ? 0 : args.Length;
				bool needsSelfInsert = parametersLength - argsLength >= 1;
				if (needsSelfInsert)
				{
					if (argsLength == 0)
					{
						args = new object[] { this };
					}
					else
					{
						var newArgs = new object[argsLength + 1];
						newArgs[0] = this;
						System.Array.Copy(args, 0, newArgs, 1, args.Length);
						args = newArgs;
					}
				}
				result = customFunctionObject.DynamicInvoke(args);
				return true;
			}
			if (v is Delegate del)
			{
				// 检查是否需要插入 self 参数
				var parameters = del.Method.GetParameters();
				bool needsSelfInsert = false;
				int argsLength = args == null ? 0 : args.Length;
				if (parameters.Length > 0)
				{
					int index = 0;
					if (parameters[index].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
					{
						index++;
					}
					if (//parameters[index].ParameterType == typeof(LuaTable) &&
						parameters.Length - index > argsLength)
					{
						needsSelfInsert = true;
					}
				}
				if (needsSelfInsert)
				{
					if (argsLength == 0)
					{
						args = new object[] { this };
					}
					else
					{
						var newArgs = new object[argsLength + 1];
						newArgs[0] = this;
						System.Array.Copy(args, 0, newArgs, 1, argsLength);
						args = newArgs;
					}
				}
				result = del.DynamicInvoke(args);
				return true;
			}
			return base.TryInvokeMember(binder, args, out result);
		}

		public static string concat(LuaTable table1, LuaTable table2)
		{
			var addObj = table1.Metatable?["__concat"] ?? table2.Metatable?["__concat"];
			if (addObj is Delegate del)
			{
				return (string)del.DynamicInvoke(table1, table2);
			}
			if (addObj is IFunctionObject functionObject)
			{
				return (string)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __concat function is not exists");
		}

		public static LuaTable operator +(LuaTable table1, LuaTable table2)
		{
			var addObj = table1.Metatable?["__add"] ?? table2.Metatable?["__add"];
			if (addObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (addObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __add function is not exists");
		}

		public static LuaTable operator -(LuaTable table1, LuaTable table2)
		{
			var subObj = table1.Metatable?["__sub"] ?? table2.Metatable?["__sub"];
			if (subObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (subObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __sub function is not exists");
		}

		public static LuaTable operator *(LuaTable table1, LuaTable table2)
		{
			var mulObj = table1.Metatable?["__mul"] ?? table2.Metatable?["__mul"];
			if (mulObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (mulObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __mul function is not exists");
		}

		public static LuaTable operator /(LuaTable table1, LuaTable table2)
		{
			var divObj = table1.Metatable?["__div"] ?? table2.Metatable?["__div"];
			if (divObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (divObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __div function is not exists");
		}

		public static LuaTable operator %(LuaTable table1, LuaTable table2)
		{
			var modObj = table1.Metatable?["__mod"] ?? table2.Metatable?["__mod"];
			if (modObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (modObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __mod function is not exists");
		}

		public static LuaTable operator -(LuaTable table)
		{
			var unmObj = table.Metatable?["__unm"];
			if (unmObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table);
			}
			if (unmObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table);
			}
			throw new Exceptions.ScriptRuntimeException($"table __unm function is not exists");
		}

		public static bool operator ==(LuaTable table1, LuaTable table2)
		{
			if (ReferenceEquals(table1, null)) return ReferenceEquals(table2, null);
			if (ReferenceEquals(table2, null)) return false;
			if (ReferenceEquals(table1, table2)) return true;
			var eqObj = table1.Metatable?["__eq"] ?? table2.Metatable?["__eq"];
			if (eqObj is Delegate del)
			{
				return (bool)del.DynamicInvoke(table1, table2);
			}
			if (eqObj is IFunctionObject functionObject)
			{
				return (bool)functionObject.DynamicInvoke(table1, table2);
			}
			return false;
		}

		public static bool operator !=(LuaTable table1, LuaTable table2)
		{
			return !(table1 == table2);
		}

		public static bool operator <(LuaTable table1, LuaTable table2)
		{
			var ltObj = table1.Metatable?["__lt"];
			if (ltObj is Delegate del)
			{
				return (bool)del.DynamicInvoke(table1, table2);
			}
			if (ltObj is IFunctionObject functionObject)
			{
				return (bool)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __lt function is not exists");
		}

		public static bool operator >(LuaTable table1, LuaTable table2)
		{
			var gtObj = table1.Metatable?["__gt"] ?? table2.Metatable?["__gt"];
			if (gtObj is Delegate del)
			{
				return (bool)del.DynamicInvoke(table1, table2);
			}
			if (gtObj is IFunctionObject functionObject)
			{
				return (bool)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __lt function is not exists");
		}

		public static bool operator <=(LuaTable table1, LuaTable table2)
		{
			var leObj = table1.Metatable?["__le"] ?? table2.Metatable?["__le"];
			if (leObj is Delegate del)
			{
				return (bool)del.DynamicInvoke(table1, table2);
			}
			if (leObj is IFunctionObject functionObject)
			{
				return (bool)functionObject.DynamicInvoke(table1, table2);
			}
			// In Lua, a <= b is defined as not (b < a)
			var ltObj = table1.Metatable?["__lt"];
			if (ltObj != null)
			{
				throw new Exceptions.ScriptRuntimeException($"table __le function is not exists");
			}
			throw new Exceptions.ScriptRuntimeException($"table __le function is not exists");
		}

		public static bool operator >=(LuaTable table1, LuaTable table2)
		{
			var geObj = table1.Metatable?["__ge"] ?? table2.Metatable?["__ge"];
			if (geObj is Delegate del)
			{
				return (bool)del.DynamicInvoke(table1, table2);
			}
			if (geObj is IFunctionObject functionObject)
			{
				return (bool)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __ge function is not exists");
		}

		public static LuaTable operator &(LuaTable table1, LuaTable table2)
		{
			var bandObj = table1.Metatable?["__band"] ?? table2.Metatable?["__band"];
			if (bandObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (bandObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __band function is not exists");
		}

		public static LuaTable operator |(LuaTable table1, LuaTable table2)
		{
			var borObj = table1.Metatable?["__bor"] ?? table2.Metatable?["__bor"];
			if (borObj is Delegate del)
			{
				return (LuaTable)del.DynamicInvoke(table1, table2);
			}
			if (borObj is IFunctionObject functionObject)
			{
				return (LuaTable)functionObject.DynamicInvoke(table1, table2);
			}
			throw new Exceptions.ScriptRuntimeException($"table __bor function is not exists");
		}

		//public static LuaTable operator ^(LuaTable table1, LuaTable table2)
		//{
		//	var bxorObj = table1.Metatable?["__bxor"] ?? table2.Metatable?["__bxor"];
		//	if (bxorObj is Delegate del)
		//	{
		//		return (LuaTable)del.DynamicInvoke(table1, table2);
		//	}
		//	if (bxorObj is IFunctionObject functionObject)
		//	{
		//		return (LuaTable)functionObject.DynamicInvoke(table1, table2);
		//	}
		//	throw new Exceptions.ScriptRuntimeException($"table __bxor function is not exists");
		//}

		//public static LuaTable operator <<(LuaTable table1, LuaTable table2)
		//{
		//	var shlObj = table1.Metatable?["__shl"];
		//	if (shlObj is Delegate del)
		//	{
		//		return (LuaTable)del.DynamicInvoke(table1, table2);
		//	}
		//	if (shlObj is IFunctionObject functionObject)
		//	{
		//		return (LuaTable)functionObject.DynamicInvoke(table1, table2);
		//	}
		//	throw new Exceptions.ScriptRuntimeException($"table __shl function is not exists");
		//}

		//public static LuaTable operator >>(LuaTable table1, LuaTable table2)
		//{
		//	var shrObj = table1.Metatable?["__shr"];
		//	if (shrObj is Delegate del)
		//	{
		//		return (LuaTable)del.DynamicInvoke(table1, table2);
		//	}
		//	if (shrObj is IFunctionObject functionObject)
		//	{
		//		return (LuaTable)functionObject.DynamicInvoke(table1, table2);
		//	}
		//	throw new Exceptions.ScriptRuntimeException($"table __shr function is not exists");
		//}

		public static object operator ~(LuaTable table)
		{
			var bnotObj = table.Metatable?["__bnot"];
			if (bnotObj is Delegate del)
			{
				return del.DynamicInvoke(table);
			}
			if (bnotObj is IFunctionObject functionObject)
			{
				return functionObject.DynamicInvoke(table);
			}
			throw new Exceptions.ScriptRuntimeException($"table __bnot function is not exists");
		}

		public override string ToString()
		{
			var tostringObj = this.Metatable?["__tostring"];
			if (tostringObj is Delegate del)
			{
				return (string)del.DynamicInvoke(this);
			}
			if (tostringObj is IFunctionObject functionObject)
			{
				return (string)functionObject.DynamicInvoke(this);
			}
			return base.ToString();
		}
	}
}
