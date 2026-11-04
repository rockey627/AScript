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

		public static IAddress CreateObjectAddress(object obj)
		{
			return new ObjectAddress(obj);
		}

		public interface IAddress
		{
			object GetValue(ScriptContext scriptContext);
			object SetValue(ScriptContext scriptContext, object value);
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

			public object GetValue(ScriptContext scriptContext)
			{
				throw new NotImplementedException();
			}

			public object SetValue(ScriptContext scriptContext, object value)
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

			public object GetValue(ScriptContext scriptContext)
			{
				return ScriptUtils.GetValue(this.Target, this.PropertyName, out _);
			}

			public object SetValue(ScriptContext scriptContext, object value)
			{
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

			public object GetValue(ScriptContext scriptContext)
			{
				return scriptContext.EvalVar(this.Name);
			}

			public object SetValue(ScriptContext scriptContext, object value)
			{
				scriptContext.SetTempVar(this.Name, value, true);
				return value;
			}
		}

		public class ObjectAddress : IAddress
		{
			public object Object { get; private set; }
			
			public ObjectAddress(object obj)
			{
				this.Object = obj;
			}

			public object GetValue(ScriptContext scriptContext)
			{
				return this.Object;
			}

			public object SetValue(ScriptContext scriptContext, object value)
			{
				throw new NotSupportedException("unsupport to assign object pointer");
			}
		}
	}

	public class Pointer<T> : Pointer
	{
		public Pointer(IAddress address) : base(typeof(T), address) { }
	}
}
