using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AScript.Lang.Go.Extensions
{
	/// <summary>
	/// Go语言常用内置函数
	/// </summary>
	public static class GoCommonExtensions
	{
		/// <summary>
		/// append 函数 - 向slice添加元素
		/// </summary>
		public static List<T> append<T>(List<T> slice, params T[] items)
		{
			var arr = new List<T>(slice.Count + items.Length);
			for (int i = 0; i < slice.Count; i++)
			{
				arr.Add(slice[i]);
			}
			for (int i = 0; i < items.Length; i++)
			{
				arr.Add(items[i]);
			}
			return arr;
		}

		/// <summary>
		/// append 函数 - 向slice添加元素
		/// </summary>
		public static List<T> append<T>(List<T> slice, ICollection<T> items)
		{
			var arr = new List<T>(slice.Count + items.Count);
			for (int i = 0; i < slice.Count; i++)
			{
				arr.Add(slice[i]);
			}
			foreach (var item in items)
			{
				arr.Add(item);
			}
			return arr;
		}

		public static List<T> append<T>(List<T> slice, T item1)
		{
			var arr = new List<T>(slice.Count + 1);
			for (int i = 0; i < slice.Count; i++)
			{
				arr.Add(slice[i]);
			}
			arr.Add(item1);
			return arr;
		}

		public static List<T> append<T>(List<T> slice, T item1, T item2)
		{
			var arr = new List<T>(slice.Count + 2);
			for (int i = 0; i < slice.Count; i++)
			{
				arr.Add(slice[i]);
			}
			arr.Add(item1);
			arr.Add(item2);
			return arr;
		}

		public static List<T> append<T>(List<T> slice, T item1, T item2, T item3)
		{
			var arr = new List<T>(slice.Count + 3);
			for (int i = 0; i < slice.Count; i++)
			{
				arr.Add(slice[i]);
			}
			arr.Add(item1);
			arr.Add(item2);
			arr.Add(item3);
			return arr;
		}

		public static void delete<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
		{
			dict.Remove(key);
		}

		/// <summary>
		/// len 函数 - 返回长度
		/// </summary>
		public static int len(object obj)
		{
			if (obj == null) return 0;
			if (obj is string s) return s.Length;
			if (obj is Array a) return a.Length;
			if (obj is IDictionary d) return d.Count;
			if (obj is ICollection l) return l.Count;
			return 0;
		}

		/// <summary>
		/// cap 函数 - 返回容量
		/// </summary>
		public static long cap(object obj)
		{
			if (obj is List<object> l) return l.Capacity;
			if (obj is Array a) return a.Length;
			return 0;
		}

		public static int copy<T>(List<T> dst, List<T> src)
		{
			int n = Math.Min(dst.Count, src.Count);
			for (int i = 0; i < n; i++)
			{
				dst[i] = src[i];
			}
			return n;
		}

#if NET45
		public static IEnumerable<Tuple<int, T>> range<T>(IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				yield return Tuple.Create(i, list[i]);
			}
		}

		public static IEnumerable<Tuple<TKey, TValue>> range<TKey, TValue>(IDictionary<TKey, TValue> dict)
		{
			foreach (var item in dict)
			{
				yield return Tuple.Create(item.Key, item.Value);
			}
		}
#else
		public static IEnumerable<(int, T)> range<T>(IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				yield return (i, list[i]);
			}
		}

		public static IEnumerable<(TKey, TValue)> range<TKey, TValue>(IDictionary<TKey, TValue> dict)
		{
			foreach (var item in dict)
			{
				yield return (item.Key, item.Value);
			}
		}
#endif

		///// <summary>
		///// panic 函数
		///// </summary>
		//public static void panic(object msg)
		//{
		//	throw new Exception(msg?.ToString() ?? "panic");
		//}

		///// <summary>
		///// recover 函数 - 暂时不支持
		///// </summary>
		//public static object recover()
		//{
		//	return null;
		//}

		///// <summary>
		///// close 函数 - 关闭通道
		///// </summary>
		//public static void close<T>(Channel<T> ch)
		//{
		//	ch?.Close();
		//}

		///// <summary>
		///// copy 函数 - 复制slice
		///// </summary>
		//public static long copy(List<object> dst, List<object> src)
		//{
		//	if (dst == null || src == null) return 0;
		//	int count = Math.Min(dst.Count, src.Count);
		//	for (int i = 0; i < count; i++)
		//	{
		//		dst[i] = src[i];
		//	}
		//	return count;
		//}

		/// <summary>
		/// complex 函数 - 创建复数
		/// </summary>
		public static Complex complex(double real, double imag)
		{
			return new Complex(real, imag);
		}

		/// <summary>
		/// real 函数 - 获取复数实部
		/// </summary>
		public static double real(Complex c)
		{
			return c?.Real ?? 0;
		}

		/// <summary>
		/// imag 函数 - 获取复数虚部
		/// </summary>
		public static double imag(Complex c)
		{
			return c?.Imag ?? 0;
		}

		/// <summary>
		/// 简单的复数类型
		/// </summary>
		public class Complex
		{
			public double Real { get; set; }
			public double Imag { get; set; }

			public Complex(double real, double imag)
			{
				Real = real;
				Imag = imag;
			}

			public override string ToString()
			{
				return $"{Real}+{Imag}i";
			}
		}

		/// <summary>
		/// 简单的通道实现
		/// </summary>
		public class Channel<T>
		{
			private readonly List<T> _queue = new List<T>();
			private bool _closed = false;
			private readonly object _lock = new object();

			public void Send(T value)
			{
				lock (_lock)
				{
					if (_closed) throw new Exception("send on closed channel");
					_queue.Add(value);
				}
			}

			public T Receive()
			{
				lock (_lock)
				{
					if (_queue.Count == 0)
					{
						if (_closed) return default(T);
						throw new Exception("receive on empty channel");
					}
					var value = _queue[0];
					_queue.RemoveAt(0);
					return value;
				}
			}

			public void Close()
			{
				lock (_lock)
				{
					_closed = true;
				}
			}

			public int Len()
			{
				lock (_lock)
				{
					return _queue.Count;
				}
			}

			public int Cap()
			{
				lock (_lock)
				{
					return _queue.Capacity;
				}
			}
		}
	}
}
