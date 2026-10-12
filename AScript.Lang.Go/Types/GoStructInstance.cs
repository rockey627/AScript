using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace AScript.Lang.Go.Types
{
	public class GoStructInstance : DynamicObject
	{
		private readonly GoStruct _struct;
		private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

		public GoStructInstance(GoStruct goStruct)
		{
			_struct = goStruct;
		}

		public GoStruct Struct => _struct;

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var name = binder.Name;

			// 先从实例字典中查找
			if (_dict.TryGetValue(name, out result))
			{
				return true;
			}

			// 从结构体字段定义中查找（递归）
			var fieldType = _struct.GetFieldType(name);
			if (fieldType != null)
			{
				result = fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null;
				return true;
			}

			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (!_dict.ContainsKey(binder.Name))
			{
				if (_struct.GetFieldType(binder.Name) == null)
				{
					return false;
				}
			}
			_dict[binder.Name] = value;
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var name = binder.Name;
			Delegate method = null;

			// 先从实例字典中查找方法
			if (_dict.TryGetValue(name, out var value) && value is Delegate del)
			{
				method = del;
			}

			// 从结构体方法定义中查找（递归）
			if (method == null)
			{
				var methods = _struct.GetMethods(name);
				if (methods != null)
				{
					Type[] argTypes = null;
					if (args != null && args.Length > 0)
					{
						argTypes = new Type[args.Length];
						for (int i = 0; i < args.Length; i++)
						{
							argTypes[i] = args[i]?.GetType();
						}
					}

					method = methods.FirstOrDefault(a => ScriptUtils.IsMatchArgTypes(argTypes, a.Method, out _, out _, out _));
				}
			}

			if (method != null)
			{
				result = method.DynamicInvoke(args);
				return true;
			}

			return base.TryInvokeMember(binder, args, out result);
		}
	}
}
