using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript
{
	/// <summary>
	/// 封装指针类型
	/// </summary>
	public class Pointer
	{
		public Type ObjectType { get; private set; }
		public IAddress Address { get; private set; }

		public Pointer(Type objType, IAddress address)
		{
			this.ObjectType = objType;
			this.Address = address;
		}

		public static IAddress CreateArrayAddress(object array, object index)
		{
			return new ArrayAddress(array, index);
		}

		public static IAddress CreatePropertyAddress(object target, string propertyName)
		{
			return new PropertyAddress(target, propertyName);
		}

		public static IAddress CreateVariableAddress(string name)
		{
			return new VariableAddress(name);
		}

		public interface IAddress
		{
			object GetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options);
			object SetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, object value);
		}

		public class ArrayAddress : IAddress
		{
			public object Array { get; private set; }
			public object Index { get; private set; }

			public ArrayAddress(object array, object index)
			{
				this.Array = array;
				this.Index = index;
			}

			public object GetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
			{
				throw new NotImplementedException();
			}

			public object SetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, object value)
			{
				throw new NotImplementedException();
			}
		}

		public class PropertyAddress : IAddress
		{
			public object Target { get; private set; }
			public string PropertyName { get; private set; }

			public PropertyAddress(object target, string propertyName)
			{
				this.Target = target;
				this.PropertyName = propertyName;
			}

			public object GetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
			{
				if (this.Target is Expression expr)
				{
					return Expression.Property(expr, this.PropertyName);
				}
				return ScriptUtils.GetValue(this.Target, this.PropertyName, out _);
			}

			public object SetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, object value)
			{
				if (this.Target is Expression expr)
				{
					var prop = Expression.Property(expr, this.PropertyName);
					Expression valueExpr;
					if (value is Expression ve) valueExpr = ve;
					else valueExpr = Expression.Constant(value);
					return Expression.Assign(prop, ScriptUtils.Convert(valueExpr, prop.Type));
				}
				ScriptUtils.SetValue(this.Target, this.PropertyName, value);
				return value;
			}
		}

		public class VariableAddress : IAddress
		{
			public string Name { get; private set; }

			public VariableAddress(string name)
			{
				this.Name = name;
			}

			public object GetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
			{
				if (buildContext != null && buildContext.TryGetVariableOrParameter(this.Name, out var value))
				{
					return value;
				}
				return scriptContext.EvalVar(this.Name);
			}

			public object SetValue(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, object value)
			{
				if (buildContext != null)
				{
					var left = new VariableNode(this.Name).BuildForAssign(buildContext, scriptContext, options, out _, out _);
					Expression valueExpr;
					if (value is Expression ve) valueExpr = ve;
					else valueExpr = Expression.Constant(value);
					return Expression.Assign(left, ScriptUtils.Convert(valueExpr, left.Type));
				}
				scriptContext.SetTempVar(this.Name, value, true);
				return value;
			}
		}
	}

	public class Pointer<T> : Pointer
	{
		public Pointer(IAddress address) : base(typeof(T), address) { }
	}
}
