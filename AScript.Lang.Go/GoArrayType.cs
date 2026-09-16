using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript.Lang.Go
{
	public class GoArrayType : Type
	{
		private string _Name;

		public Type RealType { get; private set; }
		public Type RefType { get; private set; }

		public Type ItemType { get; set; }
		public ITreeNode Length { get; set; }
		public ITreeNode Capacity { get; set; }

		public GoArrayType(bool isArray, Type itemType, string itemTypeName, ITreeNode length, ITreeNode capacity)
		{
			this.ItemType = itemType;
			this.Length = length;
			this.Capacity = capacity;
			if (isArray)
			{
				if (itemType is GoArrayType goArrayType)
				{
					RealType = goArrayType.RealType.MakeArrayType();
					RefType = itemType.MakeArrayType();
				}
				else
				{
					RealType = RefType = itemType.MakeArrayType();
				}
				// 
				if (length == null)
				{
					this._Name = $"[...]{itemTypeName}";
				}
				else if (length is ObjectNode objectNode)
				{
					this._Name = $"[{objectNode.Data}]{itemTypeName}";
				}
				else if (length is ExpressionNode expressionNode && expressionNode.Expr is ConstantExpression constantExpression)
				{
					this._Name = $"[{constantExpression.Value}]{itemTypeName}";
				}
				else
				{
					this._Name = $"[xxx]{itemTypeName}";
				}
			}
			else
			{
				if (itemType is GoArrayType goArrayType)
				{
					RealType = typeof(List<>).MakeGenericType(goArrayType.RealType);
					RefType = typeof(List<>).MakeGenericType(itemType);
				}
				else
				{
					RealType = RefType = typeof(List<>).MakeGenericType(itemType);
				}
				// 
				this._Name = $"[]{itemTypeName}";
			}
		}

		public override Assembly Assembly => RealType.Assembly;

		public override string AssemblyQualifiedName => RealType.AssemblyQualifiedName;

		public override Type BaseType => RealType.BaseType;

		public override string FullName => RealType.FullName;

		public override Guid GUID => RealType.GUID;

		public override Module Module => RealType.Module;

		public override string Namespace => RealType.Namespace;

		public override Type UnderlyingSystemType => RealType.UnderlyingSystemType;

		public override string Name => _Name;

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return RealType.GetConstructors(bindingAttr);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return RealType.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return RealType.GetCustomAttributes(attributeType, inherit);
		}

		public override Type GetElementType()
		{
			return RealType.GetElementType();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			return RealType.GetEvent(name, bindingAttr);
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			return RealType.GetEvents(bindingAttr);
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return RealType.GetField(name, bindingAttr);
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return RealType.GetFields(bindingAttr);
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			return RealType.GetInterface(name, ignoreCase);
		}

		public override Type[] GetInterfaces()
		{
			return RealType.GetInterfaces();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return RealType.GetMembers(bindingAttr);
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return RealType.GetMethods(bindingAttr);
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return RealType.GetNestedType(name, bindingAttr);
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return RealType.GetNestedTypes(bindingAttr);
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return RealType.GetProperties(bindingAttr);
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return RealType.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return RealType.IsDefined(attributeType, inherit);
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return RealType.Attributes;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return RealType.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return RealType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return RealType.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
		}

		protected override bool HasElementTypeImpl()
		{
			return RealType.HasElementType;
		}

		protected override bool IsArrayImpl()
		{
			return RealType.IsArray;
		}

		protected override bool IsByRefImpl()
		{
			return RealType.IsByRef;
		}

		protected override bool IsCOMObjectImpl()
		{
			return RealType.IsCOMObject;
		}

		protected override bool IsPointerImpl()
		{
			return RealType.IsPointer;
		}

		protected override bool IsPrimitiveImpl()
		{
			return RealType.IsPrimitive;
		}
	}
}
