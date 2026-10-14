using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace AScript
{
	public class ClassDefine : Type
	{
		private readonly string _Name;
		private readonly bool _IsValueType;
		private readonly Guid _Guid = Guid.NewGuid();
		private readonly IDictionary<string, CustomPropertyInfo> _Properties = new Dictionary<string, CustomPropertyInfo>();

		public List<ClassDefine> Bases { get; private set; } = new List<ClassDefine>();
		public IDictionary<string, List<Delegate>> Methods { get; private set; } = new Dictionary<string, List<Delegate>>();

		public override Guid GUID => _Guid;

		public override System.Reflection.Module Module => null;

		public override System.Reflection.Assembly Assembly => null;

		public override string FullName => _Name;

		public override string Namespace => string.Empty;

		public override string AssemblyQualifiedName => string.Empty;

		public override Type BaseType => typeof(object);

		public override Type UnderlyingSystemType => throw new NotImplementedException();

		public override string Name => _Name;

		public ClassDefine(string name)
		{
			this._Name = name;
		}
		public ClassDefine(string name, bool isValueType)
		{
			this._Name = name;
			this._IsValueType = isValueType;
		}

		public ClassDefine AddBase(ClassDefine baseDefine)
		{
			this.Bases.Add(baseDefine);
			return this;
		}

		public ClassDefine AddProperty(string name, Type type)
		{
			this._Properties[name] = new CustomPropertyInfo(name, type, this);
			return this;
		}

		public ClassDefine AddMethod(string name, Delegate method)
		{
			if (!this.Methods.TryGetValue(name, out var list))
			{
				list = new List<Delegate>();
				this.Methods[name] = list;
			}
			list.Add(method);
			return this;
		}

		public ClassObject CreateInstance()
		{
			return new ClassObject(this);
		}

		public ClassObject CreateDefaultValue()
		{
			return this.IsValueType ? CreateInstance() : null;
		}

		//public Type GetField(string name)
		//{
		//	if (this.Fields.TryGetValue(name, out var field))
		//	{
		//		return field;
		//	}

		//	foreach (var baseDefine in this.Bases)
		//	{
		//		var baseField = baseDefine.GetField(name);
		//		if (baseField != null) return baseField;
		//	}

		//	return null;
		//}

		public List<Delegate> GetMethods(string name)
		{
			if (this.Methods.TryGetValue(name, out var list)) return list;

			foreach (var baseDefine in Bases)
			{
				var method = baseDefine.GetMethods(name);
				if (method != null && method.Count > 0) return method;
			}

			return null;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ClassDefine define)) return false;
			return this.Name.Equals(define.Name);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotImplementedException();
		}

		protected override System.Reflection.ConstructorInfo GetConstructorImpl(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		protected override System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
		{
			throw new NotImplementedException();
		}

		public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			return null;
		}

		public override Type[] GetInterfaces()
		{
			return null;
		}

		public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr)
		{
			return null;
		}

		public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr)
		{
			return null;
		}

		protected override System.Reflection.PropertyInfo GetPropertyImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, System.Reflection.ParameterModifier[] modifiers)
		{
			if (_Properties.TryGetValue(name, out var field))
			{
				return field;
			}

			foreach (var baseDefine in this.Bases)
			{
				var baseField = baseDefine.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
				if (baseField != null) return baseField;
			}

			return null;
		}

		public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		public override Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr)
		{
			return null;
		}

		public override Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr)
		{
			return null;
		}

		public override System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr)
		{
			throw new NotImplementedException();
		}

		protected override System.Reflection.TypeAttributes GetAttributeFlagsImpl()
		{
			return System.Reflection.TypeAttributes.Public;
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return false;
		}

		protected override bool IsPointerImpl()
		{
			return false;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		protected override bool IsCOMObjectImpl()
		{
			return false;
		}

		public override Type GetElementType()
		{
			return null;
		}

		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return null;
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		protected override bool IsValueTypeImpl()
		{
			return _IsValueType;
		}

		public class CustomPropertyInfo : PropertyInfo
		{
			private readonly string _Name;
			private readonly Type _PropertyType;
			private readonly ClassDefine _DeclaringType;

			public override Type PropertyType => _PropertyType;

			public override PropertyAttributes Attributes => throw new NotImplementedException();

			public override bool CanRead => true;

			public override bool CanWrite => true;

			public override string Name => _Name;

			public override Type DeclaringType => _DeclaringType;

			public override Type ReflectedType => _DeclaringType;

			public CustomPropertyInfo(string name, Type propertyType, ClassDefine declaringType)
			{
				_Name = name;
				_PropertyType = propertyType;
				_DeclaringType = declaringType;
			}

			public override MethodInfo[] GetAccessors(bool nonPublic)
			{
				throw new NotImplementedException();
			}

			public override object[] GetCustomAttributes(bool inherit)
			{
				throw new NotImplementedException();
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherit)
			{
				throw new NotImplementedException();
			}

			public override MethodInfo GetGetMethod(bool nonPublic)
			{
				throw new NotImplementedException();
			}

			public override ParameterInfo[] GetIndexParameters()
			{
				throw new NotImplementedException();
			}

			public override MethodInfo GetSetMethod(bool nonPublic)
			{
				throw new NotImplementedException();
			}

			public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
			{
				((ClassObject)obj).TryGetMember(_Name, out var result);
				return result;
			}

			public override bool IsDefined(Type attributeType, bool inherit)
			{
				return false;
			}

			public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
			{
				((ClassObject)obj).TrySetMember(_Name, value);
			}
		}

		//public class FieldInfo
		//{
		//	public string Name { get; private set; }
		//	public Type Type { get; private set; }
		//	public ClassDefine Define { get; private set; }

		//	public bool IsValueType => this.Type == null ? this.Define.IsValueType : this.Type.IsValueType;

		//	public FieldInfo(string name, Type type)
		//	{
		//		this.Name = name;
		//		this.Type = type;
		//	}
		//	public FieldInfo(string name, ClassDefine define)
		//	{
		//		this.Name = name;
		//		this.Define = define;
		//	}

		//	public object CreateDefaultValue()
		//	{
		//		if (this.Type != null)
		//		{
		//			return this.Type.IsValueType ? Activator.CreateInstance(this.Type) : null;
		//		}
		//		return this.Define.CreateDefaultValue();
		//	}

		//	public bool IsMatchedValue(object value)
		//	{
		//		if (value == null) return true;
		//		var valueType = value.GetType();
		//		if (this.Define != null) return valueType == typeof(ClassObject);
		//		if (valueType == typeof(ClassObject)) return false;
		//		return ScriptUtils.IsMatchArgType(valueType, this.Type);
		//	}
		//}
	}
}
