using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AScript
{
	public class ScriptModuleCollection
	{
		private bool _ThreadSafely;
		private IDictionary<string, IScriptModule> _Modules;
		private List<string> _Dirs;

		/// <summary>
		/// 模块编译选项
		/// </summary>
		public BuildOptions Options { get; private set; } = new BuildOptions(Script.DefaultOptions);

		public ScriptModuleCollection(bool threadSafely)
		{
			_ThreadSafely = threadSafely;
		}

		private void Init_Modules()
		{
			if (_Modules == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Modules == null)
						{
							_Modules = new ConcurrentDictionary<string, IScriptModule>(StringComparer.OrdinalIgnoreCase);
						}
					}
				}
				else
				{
					_Modules = new Dictionary<string, IScriptModule>(StringComparer.OrdinalIgnoreCase);
				}
			}
		}

		private void Init_Dirs()
		{
			if (_Dirs == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Dirs == null)
						{
							_Dirs = new List<string>();
						}
					}
				}
				else
				{
					_Dirs = new List<string>();
				}
			}
		}

		public void Add(string name, IScriptModule obj)
		{
			Init_Modules();
			_Modules[name] = obj;
		}

		public void Add(string name, Action<ScriptModuleBuilder> builder)
		{
			var b = new ScriptModuleBuilder();
			builder.Invoke(b);
			Add(name, b.Build());
		}

		/// <summary>
		/// 添加模块目录
		/// </summary>
		/// <param name="dir"></param>
		public void AddDir(string dir)
		{
			Init_Dirs();
			string formatDir = new System.IO.DirectoryInfo(dir).FullName.ToLower();
			if (_ThreadSafely)
			{
				lock (this)
				{
					_Dirs.Add(formatDir);
				}
			}
			else
			{
				_Dirs.Add(formatDir);
			}
		}

		/// <summary>
		/// 移除模块目录
		/// </summary>
		/// <param name="dir"></param>
		public void RemoveDir(string dir)
		{
			if (_Dirs == null) return;
			string formatDir = new System.IO.DirectoryInfo(dir).FullName.ToLower();
			if (_ThreadSafely)
			{
				lock (this)
				{
					_Dirs.Remove(formatDir);
				}
			}
			else
			{
				_Dirs.Remove(formatDir);
			}
		}

		public void Remove(string name)
		{
			_Modules?.Remove(name);
		}

		public IScriptModule Get(string name)
		{
			var modules = _Modules;
			if (modules != null && modules.TryGetValue(name, out var module))
			{
				return module;
			}
			var file = GetFile(name);
			if (!string.IsNullOrEmpty(file))
			{
				return new FileScriptModule(file) { Options = new BuildOptions(this.Options) };
			}
			return null;
		}

		public void Clear()
		{
			_Modules?.Clear();
			_Dirs?.Clear();
		}

		private string GetFile(string name)
		{
			var dirs = _Dirs;
			if (dirs == null || string.IsNullOrEmpty(name)) return null;

			// 统一小写处理
			string lowerName = name.ToLower();
			// 分离目录和文件名
			string subPath = null;
			string fileName;
			int lastSlash = lowerName.LastIndexOf('/');
			if (lastSlash >= 0)
			{
				subPath = lowerName.Substring(0, lastSlash);
				fileName = lowerName.Substring(lastSlash + 1);
			}
			else
			{
				fileName = lowerName;
			}

			foreach (var dir in dirs)
			{
				string searchBase = dir;
				string searchSubPath = subPath;
				if (!string.IsNullOrEmpty(subPath))
				{
					searchSubPath = subPath.Replace('/', '\\');
					searchBase = System.IO.Path.Combine(dir, searchSubPath);
				}

				if (!System.IO.Directory.Exists(searchBase)) continue;

				try
				{
					foreach (var file in System.IO.Directory.EnumerateFiles(searchBase, "*", System.IO.SearchOption.TopDirectoryOnly))
					{
						string fileOnlyName = System.IO.Path.GetFileNameWithoutExtension(file).ToLower();
						if (fileOnlyName == fileName)
						{
							return file;
						}
					}
				}
				catch
				{
					// 忽略访问异常
				}
			}
			return null;
		}
	}
}
