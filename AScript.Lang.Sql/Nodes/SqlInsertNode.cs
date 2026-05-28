using AScript.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Sql.Nodes
{
	public class SqlInsertNode : TreeNode
	{
		public ITreeNode Source { get; set; }
		public IList<VariableNode> Columns { get; set; }
		public IList<IList<ITreeNode>> Values { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			throw new NotImplementedException();
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var source = this.Source.Eval(context, options, control, out returnType);
			if (source == null) throw new Exceptions.ScriptRuntimeException("insert target is null");

			Type elementType = null;
			MethodInfo addRangeMethod = null;
			MethodInfo addMethod = null;

			if (source is IList list)
			{
				if (source.GetType().IsGenericType)
				{
					var genericType = source.GetType().GetGenericTypeDefinition();
					if (genericType == typeof(IList<>) || genericType == typeof(List<>))
					{
						elementType = source.GetType().GenericTypeArguments[0];
					}
				}
			}

			if (elementType == null)
			{
				addRangeMethod = source.GetType().GetMethods()
					.FirstOrDefault(m =>
					{
						if (m.Name != "AddRange") return false;
						var p0 = m.GetParameters()[0];
						if (!p0.ParameterType.IsGenericType) return false;
						return p0.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
					});
				if (addRangeMethod != null)
				{
					elementType = addRangeMethod.GetParameters()[0].ParameterType.GetGenericArguments()[0];
				}
				else
				{
					addMethod = source.GetType().GetMethod("Add");
					if (addMethod == null) throw new Exceptions.ScriptRuntimeException("insert target must have AddRange or Add method");
					elementType = addMethod.GetParameters()[0].ParameterType;
				}
			}

			if (elementType == null)
			{
				throw new Exceptions.ScriptRuntimeException("insert target must have AddRange or Add method");
			}

			var columnNames = new string[this.Columns.Count];
			for (int i = 0; i < this.Columns.Count; i++)
			{
				columnNames[i] = this.Columns[i].Name;
			}

			var newItems = Array.CreateInstance(elementType, this.Values.Count);
			for (int i = 0; i < this.Values.Count; i++)
			{
				var rowValues = this.Values[i];
				var properties = new List<ITreeNode>();
				for (int j = 0; j < rowValues.Count; j++)
				{
					var value = rowValues[j].Eval(context, options, control, out var valueType);
					var assign = new OperatorNode("=", 0, 2)
					{
						Left = new VariableNode(columnNames[j]),
						Right = new ObjectNode(value, valueType)
					};
					properties.Add(assign);
				}
				var newNode = new NewNode { SystemType = elementType, InitProperties = properties };
				var newItem = newNode.Eval(context, options, control, out _);
				//newItems[i] = newItem;
				newItems.SetValue(newItem, i);
			}
			

			if (addRangeMethod != null)
			{
				addRangeMethod.Invoke(source, new object[] { newItems });
			}
			else if (source is IList list2)
			{
				foreach (var item in newItems)
				{
					list2.Add(item);
				}
			}
			else
			{
				foreach (var item in newItems)
				{
					addMethod.Invoke(source, new[] { item });
				}
			}
			returnType = typeof(void);
			return null;
		}
	}
}
