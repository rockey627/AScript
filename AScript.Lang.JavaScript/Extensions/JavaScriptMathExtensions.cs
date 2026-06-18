using System;
using System.Linq;
using System.Threading;

namespace AScript.Lang.JavaScript.Extensions
{
	public static class JavaScriptMathExtensions
	{
		private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

		// ========== Constants ==========

		public static double Math_get_E() => Math.E;
		public static double Math_get_PI() => Math.PI;
		public static double Math_get_SQRT2() => Math.Sqrt(2);
		public static double Math_get_SQRT1_2() => Math.Sqrt(0.5);
		public static double Math_get_LN2() => Math.Log(2);
		public static double Math_get_LN10() => Math.Log(10);
		public static double Math_get_LOG2E() => Math.Log(Math.E, 2);
		public static double Math_get_LOG10E() => Math.Log10(Math.E);

		// ========== abs ==========

		public static double Math_abs(double v) => Math.Abs(v);

		// ========== Trigonometric functions ==========

		public static double Math_acos(double x) => Math.Acos(x);
		public static double Math_asin(double x) => Math.Asin(x);
		public static double Math_atan(double x) => Math.Atan(x);
		public static double Math_atan2(double y, double x) => Math.Atan2(y, x);
		public static double Math_cos(double x) => Math.Cos(x);
		public static double Math_sin(double x) => Math.Sin(x);
		public static double Math_tan(double x) => Math.Tan(x);

		// ========== Hyperbolic functions ==========

		public static double Math_cosh(double x) => Math.Cosh(x);
		public static double Math_sinh(double x) => Math.Sinh(x);
		public static double Math_tanh(double x) => Math.Tanh(x);

		// ========== Inverse hyperbolic functions (not in .NET Math) ==========

		public static double Math_acosh(double x)
		{
			if (x < 1) return double.NaN;
			return Math.Log(x + Math.Sqrt(x * x - 1));
		}

		public static double Math_asinh(double x)
		{
			return Math.Log(x + Math.Sqrt(x * x + 1));
		}

		public static double Math_atanh(double x)
		{
			if (x < -1 || x > 1) return double.NaN;
			if (x == 1) return double.PositiveInfinity;
			if (x == -1) return double.NegativeInfinity;
			return 0.5 * Math.Log((1 + x) / (1 - x));
		}

		// ========== Power and root functions ==========

		public static double Math_pow(double x, double y) => Math.Pow(x, y);
		public static double Math_sqrt(double x) => Math.Sqrt(x);

		public static double Math_cbrt(double x)
		{
			return Math.Sign(x) * Math.Pow(Math.Abs(x), 1.0 / 3.0);
		}

		public static double Math_exp(double x) => Math.Exp(x);

		public static double Math_expm1(double x)
		{
			// exp(x) - 1, more accurate for small x
			if (Math.Abs(x) < 1e-5) return x + 0.5 * x * x;
			return Math.Exp(x) - 1;
		}

		// =========_ Logarithm functions ==========

		public static double Math_log(double x) => Math.Log(x);
		public static double Math_log10(double x) => Math.Log10(x);

		public static double Math_log2(double x)
		{
			if (x <= 0) return double.NaN;
			return Math.Log(x) / Math.Log(2);
		}

		public static double Math_log1p(double x)
		{
			// log(1 + x), more accurate for small x
			if (Math.Abs(x) < 1e-5) return x - 0.5 * x * x;
			return Math.Log(1 + x);
		}

		// ========== Rounding functions ==========

		public static double Math_ceil(double x) => Math.Ceiling(x);
		public static double Math_floor(double x) => Math.Floor(x);

		public static double Math_round(double x)
		{
			return Math.Floor(x + 0.5);
		}

		public static double Math_trunc(double x)
		{
			return Math.Truncate(x);
		}

		public static double Math_sign(double x)
		{
			if (x > 0) return 1;
			if (x < 0) return -1;
			return 0;
		}

		// =========_ Other functions ==========

		public static double Math_hypot(double x, double y)
		{
			return Math.Sqrt(x * x + y * y);
		}

		public static double Math_hypot(double x, double y, double z)
		{
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public static double Math_random() => _random.Value.NextDouble();

		public static long Math_imul(long a, long b)
		{
			return (long)((ulong)a * (ulong)b);
		}

		public static double Math_clz32(long x)
		{
			// Count leading zeros in 32-bit representation
			if (x == 0) return 32;
			uint u = (uint)x;
			int count = 0;
			if ((u & 0xFFFF0000) == 0) { count += 16; u <<= 16; }
			if ((u & 0xFF000000) == 0) { count += 8; u <<= 8; }
			if ((u & 0xF0000000) == 0) { count += 4; u <<= 4; }
			if ((u & 0xC0000000) == 0) { count += 2; u <<= 2; }
			if ((u & 0x80000000) == 0) { count += 1; }
			return count;
		}

		public static double Math_fround(double x)
		{
			return (float)x;
		}

		// =========_ max/min ==========

		public static double Math_max(double a, double b) => Math.Max(a, b);

		public static double Math_max(double a, double b, double c)
		{
			return Math.Max(Math.Max(a, b), c);
		}

		public static double Math_max(double a, double b, double c, double d)
		{
			return Math.Max(Math.Max(Math.Max(a, b), c), d);
		}

		public static double Math_max(params double[] arr)
		{
			return arr.Max();
		}

		public static double Math_min(double a, double b) => Math.Min(a, b);

		public static double Math_min(double a, double b, double c)
		{
			return Math.Min(Math.Min(a, b), c);
		}

		public static double Math_min(double a, double b, double c, double d)
		{
			return Math.Min(Math.Min(Math.Min(a, b), c), d);
		}

		public static double Math_min(params double[] arr)
		{
			return arr.Min();
		}

		public static string Math_toString()
		{
			return "[Math]";
		}
	}
}
