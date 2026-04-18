using AScript.Nodes;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlLikeNode : TreeNode
	{
		private static readonly Expression Constant_StringComparison_OrdinalIgnoreCase = Expression.Constant(StringComparison.OrdinalIgnoreCase);
		private static readonly MethodInfo Method_String_EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });
		private static readonly MethodInfo Method_String_StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });
#if NETSTANDARD2_1_OR_GREATER
		private static readonly MethodInfo Method_String_Contains = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });
#else
		private static readonly MethodInfo Method_String_Contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
#endif
		private static readonly MethodInfo Method_String_Equals = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(StringComparison) });

		public ITreeNode Arg1 { get; set; }
		public string Arg2 { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var s1 = this.Arg1.Build(buildContext, scriptContext, options);
			return LikeBuild(s1, this.Arg2);
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var s1 = this.Arg1.Eval(context, options, control, out _)?.ToString();
			returnType = typeof(bool);
			return LikeEval(s1, this.Arg2);
		}

		// 这里只考虑首尾%的情况
		public static bool LikeEval(string s1, string s2)
		{
			if (string.IsNullOrEmpty(s1)) return false;
			if (string.IsNullOrEmpty(s2)) return true;
			Parse(s2, out int mode, out string pattern);
			if (mode == 1)
			{
				return s1.EndsWith(pattern, StringComparison.OrdinalIgnoreCase);
			}
			if (mode == 2)
			{
				return s1.StartsWith(pattern, StringComparison.OrdinalIgnoreCase);
			}
			if (mode == 3)
			{
#if NETSTANDARD2_1_OR_GREATER
				return s1.Contains(pattern, StringComparison.OrdinalIgnoreCase);
#else
				return s1.Contains(pattern);
#endif
			}
			return s1.Equals(pattern, StringComparison.OrdinalIgnoreCase);
		}

		public static Expression LikeBuild(Expression s1, string s2)
		{
			if (s2 == null) return ExpressionUtils.Constant_false;
			var s1NotNull = Expression.NotEqual(s1, ExpressionUtils.Constant_null);
			if (s2 == "") return s1NotNull;
			Parse(s2, out int mode, out string pattern);
			if (mode == 1)
			{
				var endsWith = Expression.Call(s1, Method_String_EndsWith, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
				return Expression.AndAlso(s1NotNull, endsWith);
			}
			if (mode == 2)
			{
				var startsWith = Expression.Call(s1, Method_String_StartsWith, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
				return Expression.AndAlso(s1NotNull, startsWith);
			}
			if (mode == 3)
			{
#if NETSTANDARD2_1_OR_GREATER
				var contains = Expression.Call(s1, Method_String_Contains, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
#else
				var contains = Expression.Call(s1, Method_String_Contains, Expression.Constant(pattern));
#endif
				return Expression.AndAlso(s1NotNull, contains);
			}
			var equals = Expression.Call(s1, Method_String_Equals, Expression.Constant(pattern), Constant_StringComparison_OrdinalIgnoreCase);
			return Expression.AndAlso(s1NotNull, equals);
		}

		private static void Parse(string s, out int mode, out string pattern)
		{
			if (s[0] == '%')
			{
				if (s[s.Length - 1] == '%')
				{
					mode = 3;
					pattern = s.Substring(1, s.Length - 2);
					return;
				}
				mode = 1;
				pattern = s.Substring(1);
			}
			else if (s[s.Length - 1] == '%')
			{
				mode = 2;
				pattern = s.Substring(0, s.Length - 1);
			}
			else
			{
				mode = 0;
				pattern = s;
			}
		}
	}
}
