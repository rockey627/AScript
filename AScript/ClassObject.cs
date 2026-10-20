using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace AScript
{
	public class ClassObject : DynamicObject
	{
		private readonly ClassDefine _define;
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		public object this[string name]
		{
			get
			{
				if (TryGetMember(name, out var result))
				{
					return result;
				}
				throw new Exceptions.ScriptRuntimeException($"unknow Property '{name}' of custom class '{_define.Name}'");
			}
			set
			{
				if (!TrySetMember(name, value))
				{
					throw new Exceptions.ScriptRuntimeException($"unknow Property '{name}' of custom class '{_define.Name}'");
				}
			}
		}

		public ClassObject(ClassDefine define)
		{
			_define = define;
		}

		public ClassDefine Define => _define;

		public bool TryGetMember(string name, out object result)
		{
			// 先从实例字典中查找
			if (_values.TryGetValue(name, out result))
			{
				return true;
			}

			// 从结构体字段定义中查找（递归）
			var field = _define.GetProperty(name);
			if (field != null)
			{
				result = ScriptUtils.GetDefaultValue(field.PropertyType);
				return true;
			}

			result = null;
			return false;
		}

		public bool TrySetMember(string name, object value)
		{
			var field = _define.GetProperty(name);
			if (field == null) return false;
			var valueType = value is ClassObject classObject ? classObject._define : value?.GetType();
			if (!ScriptUtils.IsMatchArgType(valueType, field.PropertyType)) return false;
			_values[name] = value;
			return true;
		}

		public bool TryInvokeMember(string name, object[] args, out object result)
		{
			Delegate method = null;

			// 先从实例字典中查找方法
			if (_values.TryGetValue(name, out var value) && value is Delegate del)
			{
				method = del;
			}

			// 从结构体方法定义中查找（递归）
			if (method == null)
			{
				var methods = _define.GetMethods(name);
				if (methods != null)
				{
					Type[] argTypes;
					if (args != null && args.Length > 0)
					{
						argTypes = new Type[args.Length + 1];
						argTypes[0] = _define;
						for (int i = 0; i < args.Length; i++)
						{
							argTypes[i + 1] = args[i]?.GetType();
						}
					}
					else
					{
						argTypes = new Type[] { _define };
					}
					method = methods.FirstOrDefault(a => ScriptUtils.IsMatchArgTypes(argTypes, a.Method, out _, out _, out _));
				}
			}

			if (method != null)
			{
				object[] newArgs;
				if (args == null || args.Length == 0)
				{
					newArgs = new object[] { this };
				}
				else
				{
					newArgs = new object[args.Length + 1];
					newArgs[0] = this;
					Array.Copy(args, 0, newArgs, 1, args.Length);
				}
				result = method.DynamicInvoke(newArgs);
				return true;
			}

			result = null;
			return false;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return TryGetMember(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			return TrySetMember(binder.Name, value);
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			if (TryInvokeMember(binder.Name, args, out result))
			{
				return true;
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}
}
