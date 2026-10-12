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

			// 从结构体字段定义中查找
			if (_struct.Fields.TryGetValue(name, out var fieldType))
			{
				// 返回默认值
				result = fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null;
				return true;
			}

			// 从结构体方法定义中查找
			if (_struct.Methods.TryGetValue(name, out var methods))
			{
				result = methods.FirstOrDefault();
				return result != null;
			}

			// 从基类中查找
			foreach (var baseStruct in _struct.Bases)
			{
				if (baseStruct.Fields.TryGetValue(name, out var baseFieldType))
				{
					result = baseFieldType.IsValueType ? Activator.CreateInstance(baseFieldType) : null;
					return true;
				}
				if (baseStruct.Methods.TryGetValue(name, out var baseMethods))
				{
					result = baseMethods.FirstOrDefault();
					if (result != null) return true;
				}
			}

			result = null;
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
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

			// 从结构体方法定义中查找
			if (method == null && _struct.Methods.TryGetValue(name, out var methods))
			{
				method = methods.FirstOrDefault();
			}

			// 从基类方法中查找
			if (method == null)
			{
				foreach (var baseStruct in _struct.Bases)
				{
					if (baseStruct.Methods.TryGetValue(name, out var baseMethods))
					{
						method = baseMethods.FirstOrDefault();
						if (method != null) break;
					}
				}
			}

			if (method != null)
			{
				// 处理参数数量匹配
				var parameters = method.Method.GetParameters();
				int argsLength = args == null ? 0 : args.Length;
				int parametersLength = parameters.Length;

				// 检查第一个参数是否为 Closure 类型
				if (parameters.Length > 0 &&
					parameters[0].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
				{
					parametersLength -= 1;
				}

				// 如果参数不足，填充 null
				if (parametersLength > argsLength)
				{
					var newArgs = new object[parametersLength];
					if (argsLength > 0)
					{
						System.Array.Copy(args, newArgs, argsLength);
					}
					args = newArgs;
				}

				result = method.DynamicInvoke(args);
				return true;
			}

			return base.TryInvokeMember(binder, args, out result);
		}

		public object GetField(string name)
		{
			if (_dict.TryGetValue(name, out var value))
			{
				return value;
			}

			if (_struct.Fields.TryGetValue(name, out var fieldType))
			{
				return fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null;
			}

			foreach (var baseStruct in _struct.Bases)
			{
				if (baseStruct.Fields.TryGetValue(name, out var baseFieldType))
				{
					return baseFieldType.IsValueType ? Activator.CreateInstance(baseFieldType) : null;
				}
			}

			return null;
		}

		public void SetField(string name, object value)
		{
			_dict[name] = value;
		}

		public object InvokeMethod(string name, params object[] args)
		{
			Delegate method = null;

			if (_dict.TryGetValue(name, out var value) && value is Delegate del)
			{
				method = del;
			}

			if (method == null && _struct.Methods.TryGetValue(name, out var methods))
			{
				method = methods.FirstOrDefault();
			}

			if (method == null)
			{
				foreach (var baseStruct in _struct.Bases)
				{
					if (baseStruct.Methods.TryGetValue(name, out var baseMethods))
					{
						method = baseMethods.FirstOrDefault();
						if (method != null) break;
					}
				}
			}

			if (method != null)
			{
				return method.DynamicInvoke(args);
			}

			throw new MissingMemberException($"Method '{name}' not found");
		}
	}
}
