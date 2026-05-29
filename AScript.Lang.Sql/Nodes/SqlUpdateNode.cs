using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlUpdateNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<string> Fields { get; set; }
		public IList<ITreeNode> Values { get; set; }
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
			var list = (IEnumerable)context.EvalFunc(options, control, "ToList", new ITreeNode[] { sourceNode });
			// 
			int count = 0;
			foreach (var item in list)
			{
				count++;
				var tmpContext = new ScriptContext(context);
				var properties = item.GetType().GetProperties();
				foreach (var p in properties)
				{
					tmpContext.SetVar(p.Name, p.GetValue(item), p.PropertyType);
				}
				var dict = properties.ToDictionary(a => a.Name);
				for (int i = 0; i < this.Fields.Count; i++)
				{
					string field = this.Fields[i];
					if (dict.TryGetValue(field, out var p))
					{
						var value = this.Values[i].Eval(tmpContext, options, control, out var valueType);
						p.SetValue(item, value);
					}
				}
			}

			// 更新数据源
			var updateRangeMethod = source.GetType().GetMethods()
				.FirstOrDefault(m =>
				{
					if (m.Name != "UpdateRange") return false;
					var p0 = m.GetParameters()[0];
					if (!p0.ParameterType.IsGenericType) return false;
					return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
				});
			if (updateRangeMethod != null)
			{
				updateRangeMethod.Invoke(source, new object[] { list });
			}

			return count;
		}

		private Type GetItemType(Type type)
		{
			if (type.IsGenericType)
			{
				var genericType = type.GetGenericTypeDefinition();
				if (genericType == typeof(IList<>) || genericType == typeof(List<>))
				{
					return type.GenericTypeArguments[0];
				}
			}
			return null;
		}
	}
}
