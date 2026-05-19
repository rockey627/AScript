using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class TupleNode : TreeNode
	{
		private static readonly MethodInfo Method_Tuple_2;
		private static readonly MethodInfo Method_Tuple_3;
		private static readonly MethodInfo Method_Tuple_4;
		private static readonly MethodInfo Method_Tuple_5;
		private static readonly MethodInfo Method_Tuple_6;
		private static readonly MethodInfo Method_Tuple_7;
		private static readonly MethodInfo Method_Tuple_8;

		static TupleNode()
		{
#if NETFRAMEWORK
			var methods = typeof(Tuple).GetMethods();
			foreach (var method in methods)
			{
				if (method.Name != "Create") continue;
				int count = method.GetParameters().Length;
				switch (count)
				{
					case 2:
						Method_Tuple_2 = method;
						break;
					case 3:
						Method_Tuple_3 = method;
						break;
					case 4:
						Method_Tuple_4 = method;
						break;
					case 5:
						Method_Tuple_5 = method;
						break;
					case 6:
						Method_Tuple_6 = method;
						break;
					case 7:
						Method_Tuple_7 = method;
						break;
					case 8:
						Method_Tuple_8 = method;
						break;
					default:
						break;
				}
			}
#else
			var methods = typeof(ValueTuple).GetMethods();
			foreach (var method in methods)
			{
				if (method.Name != "Create") continue;
				int count = method.GetParameters().Length;
				switch (count)
				{
					case 2:
						Method_Tuple_2 = method;
						break;
					case 3:
						Method_Tuple_3 = method;
						break;
					case 4:
						Method_Tuple_4 = method;
						break;
					case 5:
						Method_Tuple_5 = method;
						break;
					case 6:
						Method_Tuple_6 = method;
						break;
					case 7:
						Method_Tuple_7 = method;
						break;
					case 8:
						Method_Tuple_8 = method;
						break;
					default:
						break;
				}
			}
#endif
		}

		public IList<ITreeNode> Items { get; set; }

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var itemValues = new Expression[this.Items.Count];
			var itemTypes = new Type[this.Items.Count];

			for (int i = 0; i < this.Items.Count; i++)
			{
				var expr = this.Items[i].Build(buildContext, scriptContext, options);
				itemValues[i] = expr;
				itemTypes[i] = expr.Type;
			}

			return BuildTuple(itemValues, itemTypes);
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
			
			var v = CreateTuple(itemValues, itemTypes);
			returnType = v.GetType();
			return v;
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			var itemValues = new object[this.Items.Count];
			var itemTypes = new Type[this.Items.Count];

			for (int i = 0; i < this.Items.Count; i++)
			{
				var result = await this.Items[i].EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
				itemValues[i] = result.Value;
				itemTypes[i] = result.Type;
			}

			var v = CreateTuple(itemValues, itemTypes);
			var returnType = v.GetType();
			return new EvalResult(v, returnType);
		}

		public static object CreateTuple(object[] values, Type[] types)
		{
			var createMethod = GetMethod(types.Length);
			return createMethod.MakeGenericMethod(types).Invoke(null, values);
		}

		public static Expression BuildTuple(Expression[] itemValues, Type[] itemTypes)
		{
			var createMethod = GetMethod(itemTypes.Length);
			var genericCreateMethod = createMethod.MakeGenericMethod(itemTypes);
			return Expression.Call(genericCreateMethod, itemValues);
		}

		private static MethodInfo GetMethod(int count)
		{
			switch (count)
			{
				case 2: return Method_Tuple_2;
				case 3: return Method_Tuple_3;
				case 4: return Method_Tuple_4;
				case 5: return Method_Tuple_5;
				case 6: return Method_Tuple_6;
				case 7: return Method_Tuple_7;
				case 8: return Method_Tuple_8;
				default:
					return null;
			}
		}

		public override void Clear()
		{
			base.Clear();

			PoolManage.Return(this.Items);

			this.Items?.Clear();
		}
	}
}
