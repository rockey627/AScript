using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class CollectionNode : TreeNode
	{
		/// <summary>
		/// 集合中的值列表
		/// </summary>
		public IList<ITreeNode> Items { get; set; }
		/// <summary>
		/// 集合中的元素类型，如果为null则取Items中的值类型
		/// </summary>
		public Type ElementType { get; set; }
		/// <summary>
		/// <![CDATA[如果是数组则为typeof(Array)，如果是列表则为typeof(List<>)]]>
		/// </summary>
		public Type CollectionType { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			return null;
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.CollectionType == null)
			{
				throw new Exception("unknown collection type");
			}

			// Evaluate all items to get their values
			var itemValues = new object[Items.Count];
			Type elementType = null;
			var objType = typeof(object);
			for (int i = 0; i < Items.Count; i++)
			{
				itemValues[i] = Items[i].Eval(context, options, control, out var itemType);
				if (this.ElementType == null)
				{
					if (elementType == null)
					{
						elementType = itemType;
					}
					else if (elementType != objType && itemType != elementType)
					{
						elementType = objType;
					}
				}
			}

			// Determine element type
			if (this.ElementType != null)
			{
				elementType = this.ElementType;
			}

			if (CollectionType == typeof(Array))
			{
				// Create array
				var arr = Array.CreateInstance(elementType, itemValues.Length);
				for (int i = 0; i < itemValues.Length; i++)
				{
					arr.SetValue(Convert.ChangeType(itemValues[i], elementType), i);
				}
				returnType = typeof(Array);
				return arr;
			}

			// Handle List<T> - determine the actual list type to create
			Type listType;
			if (CollectionType.IsGenericType && CollectionType.GetGenericTypeDefinition() == typeof(List<>))
			{
				listType = CollectionType;
			}
			else
			{
				listType = typeof(List<>).MakeGenericType(elementType);
			}

			var list = (IList)Activator.CreateInstance(listType, itemValues.Length);
			for (int i = 0; i < itemValues.Length; i++)
			{
				list.Add(Convert.ChangeType(itemValues[i], elementType));
			}
			returnType = listType;
			return list;
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Items);

			this.Items = null;
			this.ElementType = null;
			this.CollectionType = null;
		}
	}
}
