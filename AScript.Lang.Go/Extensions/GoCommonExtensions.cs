using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public static List<object> append(List<object> slice, params object[] items)
		{
			if (slice == null) slice = new List<object>();
			foreach (var item in items)
			{
				slice.Add(item);
			}
			return slice;
		}

		/// <summary>
		/// len 函数 - 返回长度
		/// </summary>
		public static long len(object obj)
		{
			if (obj == null) return 0;
			if (obj is string s) return s.Length;
			if (obj is List<object> l) return l.Count;
			if (obj is Dictionary<object, object> d) return d.Count;
			if (obj is Array a) return a.Length;
			if (obj is ICollection<object> c) return c.Count;
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

		/// <summary>
		/// new 函数 - 分配类型
		/// </summary>
		public static object @new(Type type)
		{
			if (type == typeof(int) || type == typeof(long)) return 0L;
			if (type == typeof(float) || type == typeof(double)) return 0.0;
			if (type == typeof(bool)) return false;
			if (type == typeof(string)) return "";
			return Activator.CreateInstance(type);
		}

		/// <summary>
		/// make 函数 - 创建slice/map/chan
		/// </summary>
		public static object make(Type type, params object[] args)
		{
			if (type == typeof(List<object>))
			{
				if (args.Length == 0) return new List<object>();
				if (args.Length == 1) return new List<object>((int)(long)args[0]);
				return new List<object>((int)(long)args[0]) { Capacity = (int)(long)args[1] };
			}
			if (type == typeof(Dictionary<object, object>))
			{
				return new Dictionary<object, object>();
			}
			if (type.Name == "Channel`1")
			{
				// 通道类型
				return new Channel<object>();
			}
			return null;
		}

		/// <summary>
		/// delete 函数 - 从map删除元素
		/// </summary>
		public static void delete(Dictionary<object, object> map, object key)
		{
			if (map != null && key != null)
			{
				map.Remove(key);
			}
		}

		/// <summary>
		/// println 函数
		/// </summary>
		public static void println(params object[] args)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				if (i > 0) sb.Append(" ");
				sb.Append(args[i]?.ToString() ?? "<nil>");
			}
			Console.WriteLine(sb.ToString());
		}

		/// <summary>
		/// print 函数
		/// </summary>
		public static void @print(params object[] args)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				if (i > 0) sb.Append(" ");
				sb.Append(args[i]?.ToString() ?? "<nil>");
			}
			Console.Write(sb.ToString());
		}

		/// <summary>
		/// panic 函数
		/// </summary>
		public static void panic(object msg)
		{
			throw new Exception(msg?.ToString() ?? "panic");
		}

		/// <summary>
		/// recover 函数 - 暂时不支持
		/// </summary>
		public static object recover()
		{
			return null;
		}

		/// <summary>
		/// close 函数 - 关闭通道
		/// </summary>
		public static void close<T>(Channel<T> ch)
		{
			ch?.Close();
		}

		/// <summary>
		/// copy 函数 - 复制slice
		/// </summary>
		public static long copy(List<object> dst, List<object> src)
		{
			if (dst == null || src == null) return 0;
			int count = Math.Min(dst.Count, src.Count);
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
			return count;
		}

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
