using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Sql.Nodes
{
	/// <summary>
	/// 
	/// </summary>
	public class SqlFromNode : TreeNode
	{
		public static readonly MethodInfo Method_Enumerable_Where = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static) // 静态公共方法
			.FirstOrDefault(m =>
			{
				if (m.Name != "Where" || !m.IsGenericMethodDefinition) return false;
				var parameters = m.GetParameters();
				var p0 = parameters[0];
				var p1 = parameters[1];
				// 第1参数：IEnumerable<T>（泛型类型为IEnumerable<>）
				// 第2参数：Func<T,bool>（泛型类型为Func<,>，且返回值为bool）
				return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
					p1.ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
					p1.ParameterType.GetGenericArguments()[1] == typeof(bool);
			});

#if NETSTANDARD
		public IList<(ITreeNode, string)> Tables { get; set; }
#else
		public IList<Tuple<ITreeNode, string>> Tables { get; set; }
#endif
		public ITreeNode Where { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var table = this.Tables[0].Item1.Build(buildContext, scriptContext, options);
			return null;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var table = this.Tables[0].Item1.Eval(context, options, control, out returnType);
			if (table == null) return table;
			if (table is IList list && list.Count == 0) return table;
			var itemType = returnType.HasElementType ? returnType.GetElementType() : returnType.GenericTypeArguments[0];
			var enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
			returnType = enumerableType;
			var whereMethod = Method_Enumerable_Where.MakeGenericMethod(itemType);
			var predicate = ScriptEngine.GetCurrent(context).Compile(this.Where, new[] { itemType }, new[] { this.Tables[0].Item2 }, typeof(bool));
			return whereMethod.Invoke(null, new[] { table, predicate });
		}

		public override void Clear()
		{
			base.Clear();

			this.Tables = null;
			this.Where = null;
		}
	}
}
