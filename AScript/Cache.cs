using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AScript
{
	public class Cache<T> : IDisposable
	{
		public bool IsDisposing { get; private set; }
		public bool IsDisposed { get; private set; }

		// 定时器/版本号/数据
		private readonly ConcurrentDictionary<string, Tuple<Timer, string, T>> _Cache = new ConcurrentDictionary<string, Tuple<Timer, string, T>>();

		private void Timer_Callback(object state)
		{
			if (_Cache.TryRemove((string)state, out var t))
			{
				t.Item1?.Dispose();
			}
		}

		public bool TryGetValue(string key, out T value)
		{
			if (_Cache.TryGetValue(key, out var t))
			{
				value = t.Item3;
				return true;
			}
			value = default;
			return false;
		}

		public bool TryGetValue(string key, string version, out T value)
		{
			if (_Cache.TryGetValue(key, out var t) && t.Item2 == version)
			{
				value = t.Item3;
				return true;
			}
			value = default;
			return false;
		}

		public bool TryRemove(string key, out T value)
		{
			if (_Cache.TryRemove(key, out var t))
			{
				t.Item1?.Dispose();
				value = t.Item3;
				return true;
			}
			value = default;
			return false;
		}

		public void Remove(string key)
		{
			if (_Cache.TryRemove(key, out var t))
			{
				t.Item1?.Dispose();
			}
		}

		public bool Contains(string key)
		{
			return _Cache.ContainsKey(key);
		}

		public void SetValue(string key, T value, int timeout = -1, string version = null)
		{
			Timer timer = null;
			if (_Cache.TryGetValue(key, out var t))
			{
				timer = t.Item1;
				if (timeout <= 0 && timer != null)
				{
					timer.Dispose();
				}
			}

			if (timeout > 0)
			{
				if (timer == null)
				{
					timer = new Timer(Timer_Callback, key, timeout, -1);
				}
				else
				{
					timer.Change(timeout, -1);
				}
			}

			_Cache[key] = Tuple.Create(timer, version, value);
		}

		public void Dispose()
		{
			this.IsDisposing = true;
			foreach (var item in _Cache.Values)
			{
				item.Item1?.Dispose();
			}
			_Cache.Clear();
			this.IsDisposed = true;
		}
	}
}
