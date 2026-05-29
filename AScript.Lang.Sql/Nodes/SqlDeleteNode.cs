using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlDeleteNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public ITreeNode Condition { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var source = this.Source.Eval(context, options, control, out _);
			returnType = typeof(int);
			if (source == null) return 0;

			ITreeNode sourceNode = new ObjectNode(source);
			if (this.Condition != null)
			{
				var queryNode = new QueryNode();
				queryNode.AddFrom("__query__", sourceNode);
				var condition = new SqlTreeNodeVisitor(null, context, queryNode).Visit(this.Condition);
				queryNode.AddWhere(condition);
				sourceNode = queryNode;
			}

			// ToList
			var list = (IList)context.EvalFunc(options, control, "ToList", new ITreeNode[] { sourceNode });
			if (list.Count == 0) return 0;

			// 更新数据源
			var removeRangeMethod = source.GetType().GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "RemoveRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (removeRangeMethod != null)
			{
				removeRangeMethod.Invoke(source, new object[] { list });
				return list.Count;
			}

			if (source is IList sourceList)
			{
				foreach (var item in list)
				{
					sourceList.Remove(item);
				}
			}
			else
			{
				var removeMethod = source.GetType().GetMethod("Remove");
				foreach (var item in list)
				{
					removeMethod.Invoke(source, new[] { item });
				}
			}

			return list.Count;
		}

		private Type GetItemType(Type type)
		{
			if (type.IsGenericType)
			{
				return type.GenericTypeArguments[0];
			}
			return null;
		}

	}
}
