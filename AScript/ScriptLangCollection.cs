using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AScript
{
	public class ScriptLangCollection
	{
		private readonly ConcurrentDictionary<string, ScriptLang> _LangDict = new ConcurrentDictionary<string, ScriptLang>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// 默认语言列表
		/// </summary>
		private readonly List<string> _DefaultList = new List<string>();

		private IReadOnlyList<string> _defaults;

		public int Count => _LangDict.Count;
		public ICollection<string> Keys => _LangDict.Keys;
		public ICollection<ScriptLang> Values => _LangDict.Values;

		public ScriptLang this[string name]
		{
			get => _LangDict[name];
			set => _LangDict[name] = value;
		}

		/// <summary>
		/// 添加脚本言语，如果已存在则返回false
		/// </summary>
		/// <param name="name"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public bool TryAdd(string name, ScriptLang lang)
		{
			return _LangDict.TryAdd(name, lang);
		}

		/// <summary>
		/// 添加脚本言语并添加到默认语言列表，如果已存在则返回false
		/// </summary>
		/// <param name="name"></param>
		/// <param name="lang"></param>
		/// <param name="addToDefault"></param>
		/// <returns></returns>
		public bool TryAdd(string name, ScriptLang lang, bool addToDefault)
		{
			bool r = TryAdd(name, lang);
			if (addToDefault)
			{
				AddDefault(name);
			}
			return r;
		}

		public bool TryGetValue(string name, out ScriptLang lang)
		{
			return _LangDict.TryGetValue(name, out lang);
		}

		/// <summary>
		/// 移除脚本语言
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool TryRemove(string name)
		{
			return _LangDict.TryRemove(name, out _);
		}

		/// <summary>
		/// 移除脚本语言
		/// </summary>
		/// <param name="name"></param>
		/// <param name="lang"></param>
		/// <returns></returns>
		public bool TryRemove(string name, out ScriptLang lang)
		{
			return _LangDict.TryRemove(name, out lang);
		}

		/// <summary>
		/// 添加默认语言
		/// </summary>
		/// <param name="name"></param>
		public void AddDefault(string name)
		{
			if (string.IsNullOrEmpty(name)) return;
			lock (_DefaultList)
			{
				if (!_DefaultList.Any(a => a.Equals(name, StringComparison.OrdinalIgnoreCase)))
				{
					_DefaultList.Add(name);
					_defaults = null;
				}
			}
		}

		/// <summary>
		/// 设置默认语言
		/// </summary>
		/// <param name="name"></param>
		public void SetDefault(string name)
		{
			lock (_DefaultList)
			{
				_DefaultList.Clear();
				if (!string.IsNullOrEmpty(name))
				{
					_DefaultList.Add(name);
				}
				_defaults = null;
			}
		}

		/// <summary>
		/// 设置默认语言
		/// </summary>
		/// <param name="names"></param>
		public void SetDefault(params string[] names)
		{
			SetDefault((IEnumerable<string>)names);
		}

		/// <summary>
		/// 设置默认语言
		/// </summary>
		/// <param name="names"></param>
		public void SetDefault(IEnumerable<string> names)
		{
			lock (_DefaultList)
			{
				_DefaultList.Clear();
				if (names != null)
				{
					_DefaultList.AddRange(names);
				}
				_defaults = null;
			}
		}

		/// <summary>
		/// 是否为默认语言
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsDefault(string name)
		{
			if (string.IsNullOrEmpty(name)) return false;
			lock (_DefaultList)
			{
				return _DefaultList.Any(a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
			}
		}

		/// <summary>
		/// 移除默认语言
		/// </summary>
		/// <param name="name"></param>
		public void RemoveDefault(string name)
		{
			if (string.IsNullOrEmpty(name)) return;
			lock (_DefaultList)
			{
				int c = _DefaultList.RemoveAll(a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
				if (c > 0) _defaults = null;
			}
		}

		/// <summary>
		/// 移除默认语言
		/// </summary>
		/// <param name="names"></param>
		public void RemoveDefault(IEnumerable<string> names)
		{
			if (names == null) return;
			lock (_DefaultList)
			{
				int c = _DefaultList.Count;
				foreach (var name in names)
				{
					_DefaultList.RemoveAll(a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
				}
				if (_DefaultList.Count < c)
				{
					_defaults = null;
				}
			}
		}

		/// <summary>
		/// 获取默认语言
		/// </summary>
		/// <returns></returns>
		public IReadOnlyList<string> GetDefaults()
		{
			if (_defaults == null)
			{
				lock (_DefaultList)
				{
					if (_defaults == null)
					{
						_defaults = new ReadOnlyCollection<string>(_DefaultList.ToArray());
					}
				}
			}
			return _defaults;
		}
	}
}
