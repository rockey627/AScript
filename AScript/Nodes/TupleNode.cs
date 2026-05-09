using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class TupleNode : TreeNode
	{
		public IList<ITreeNode> Items { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var itemValues = this.Items.Select(item => item.Build(buildContext, scriptContext, options)).ToList();
			var itemTypes = itemValues.Select(e => e.Type).ToArray();

#if NETFRAMEWORK
			var tupleType = typeof(Tuple<>).MakeGenericType(itemTypes);
			var ctor = tupleType.GetConstructor(itemTypes);
			return Expression.New(ctor, itemValues);
#else
			var tupleType = typeof(ValueTuple<>).MakeGenericType(itemTypes);
			var createMethod = typeof(ValueTuple).GetMethods().First(m => m.Name == "Create" && m.GetParameters().Length == itemTypes.Length);
			var genericCreateMethod = createMethod.MakeGenericMethod(itemTypes);
			return Expression.Call(genericCreateMethod, itemValues.ToArray());
#endif
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			var itemValues = new object[this.Items.Count];
			var itemTypes = new Type[this.Items.Count];

			for (int i = 0; i < this.Items.Count; i++)
			{
				itemValues[i] = this.Items[i].Eval(context, options, control, out var itemType);
				itemTypes[i] = itemType;
			}

#if NETFRAMEWORK
			returnType = typeof(Tuple<>).MakeGenericType(itemTypes);
			return typeof(Tuple).GetMethod("Create").MakeGenericMethod(itemTypes).Invoke(null, itemValues);
#else
			returnType = typeof(ValueTuple<>).MakeGenericType(itemTypes);
			var createMethod = typeof(ValueTuple).GetMethods().First(m => m.Name == "Create" && m.GetParameters().Length == itemTypes.Length);
			return createMethod.MakeGenericMethod(itemTypes).Invoke(null, itemValues);
#endif
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Items);

			this.Items?.Clear();
		}
	}
}
