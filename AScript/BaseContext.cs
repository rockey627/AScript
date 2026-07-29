using AScript.Functions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript
{
	public abstract class BaseContext
	{
		protected readonly bool _ThreadSafely;
		private readonly bool _IgnoreCase;

		/// <summary>
		/// 是否线程安全
		/// </summary>
		public bool ThreadSafely => _ThreadSafely;
		/// <summary>
		/// 关键字（函数名、变量名、类名）是否忽略大小写
		/// </summary>
		public bool IgnoreCase => _IgnoreCase;

		/// <summary>
		/// 函数运算事件
		/// </summary>
		public event EventHandler<FunctionEvalArgs> FunctionEval;
		/// <summary>
		/// 函数编译事件
		/// </summary>
		public event EventHandler<FunctionBuildArgs> FunctionBuild;

		// 函数运算
		protected IDictionary<string, IList<IFunctionEvaluator>> _FunctionEvaluators;
		// 语句处理
		protected IDictionary<string, ITokenHandler> _TokenHandlerDict;

		protected IList<ITokenHandler> _TokenHandlers;

		/// <summary>
		/// 程序集
		/// </summary>
		protected IDictionary<string, Assembly> _Assemblies;
		/// <summary>
		/// 类型定义
		/// </summary>
		protected IDictionary<string, Type> _Types;
		/// <summary>
		/// 全局变量
		/// </summary>
		protected IDictionary<string, object> _Variables;
		/// <summary>
		/// 全局变量类型
		/// </summary>
		protected IDictionary<string, Type> _VariableTypes;
		/// <summary>
		/// 变量修饰符
		/// </summary>
		protected IDictionary<string, int> _VariableModifiers;

		// 支持函数重载
		protected IDictionary<string, List<Delegate>> _Functions;

		protected IDictionary<string, IScriptModule> _Modules;
		protected IDictionary<Type, bool> _ObjectMemberEnabledDict;

		protected BaseContext(bool threadSafely)
		{
			this._ThreadSafely = threadSafely;
		}
		protected BaseContext(bool threadSafely, bool ignoreCase) : this(threadSafely)
		{
			this._IgnoreCase = ignoreCase;
		}

		private void Init_FunctionEvaluators()
		{
			if (_FunctionEvaluators == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_FunctionEvaluators == null)
						{
							_FunctionEvaluators = _IgnoreCase ?
								new ConcurrentDictionary<string, IList<IFunctionEvaluator>>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, IList<IFunctionEvaluator>>();
						}
					}
				}
				else
				{
					_FunctionEvaluators = _IgnoreCase ?
						new Dictionary<string, IList<IFunctionEvaluator>>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, IList<IFunctionEvaluator>>();
				}
			}
		}

		private void Init_TokenHandlerDict()
		{
			if (_TokenHandlerDict == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_TokenHandlerDict == null)
						{
							_TokenHandlerDict = _IgnoreCase ?
								new ConcurrentDictionary<string, ITokenHandler>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, ITokenHandler>();
						}
					}
				}
				else
				{
					_TokenHandlerDict = _IgnoreCase ?
						new Dictionary<string, ITokenHandler>(StringComparer.Ordinal) :
						new Dictionary<string, ITokenHandler>();
				}
			}
		}

		private void Init_TokenHandlers()
		{
			if (_TokenHandlers == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_TokenHandlers == null)
						{
							_TokenHandlers = new List<ITokenHandler>();
						}
					}
				}
				else
				{
					_TokenHandlers = new List<ITokenHandler>();
				}
			}
		}

		private void Init_Assemblies()
		{
			if (_Assemblies == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Assemblies == null)
						{
							_Assemblies = _IgnoreCase ?
								new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, Assembly>();
						}
					}
				}
				else
				{
					_Assemblies = _IgnoreCase ?
						new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, Assembly>();
				}
			}
		}

		private void Init_Types()
		{
			if (_Types == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Types == null)
						{
							_Types = _IgnoreCase ?
								new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, Type>();
						}
					}
				}
				else
				{
					_Types = _IgnoreCase ?
						new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, Type>();
				}
			}
		}

		private void Init_Variables()
		{
			if (_Variables == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Variables == null)
						{
							_Variables = _IgnoreCase ?
								new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, object>();
						}
					}
				}
				else
				{
					_Variables = _IgnoreCase ?
						new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, object>();
				}
			}
		}

		private void Init_VariableTypes()
		{
			if (_VariableTypes == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_VariableTypes == null)
						{
							_VariableTypes = _IgnoreCase ?
								new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, Type>();
						}
					}
				}
				else
				{
					_VariableTypes = _IgnoreCase ?
						new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, Type>();
				}
			}
		}

		private void Init_VariableModifiers()
		{
			if (_VariableModifiers == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_VariableModifiers == null)
						{
							_VariableModifiers = _IgnoreCase ?
								new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, int>();
						}
					}
				}
				else
				{
					_VariableModifiers = _IgnoreCase ?
						new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, int>();
				}
			}
		}

		private void Init_Functions()
		{
			if (_Functions == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Functions == null)
						{
							_Functions = _IgnoreCase ?
								new ConcurrentDictionary<string, List<Delegate>>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, List<Delegate>>();
						}
					}
				}
				else
				{
					_Functions = _IgnoreCase ?
						new Dictionary<string, List<Delegate>>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, List<Delegate>>();
				}
			}
		}

		private void Init_Modules()
		{
			if (_Modules == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_Modules == null)
						{
							_Modules = _IgnoreCase ?
								new ConcurrentDictionary<string, IScriptModule>(StringComparer.OrdinalIgnoreCase) :
								new ConcurrentDictionary<string, IScriptModule>();
						}
					}
				}
				else
				{
					_Modules = _IgnoreCase ?
						new Dictionary<string, IScriptModule>(StringComparer.OrdinalIgnoreCase) :
						new Dictionary<string, IScriptModule>();
				}
			}
		}

		private void Init_ObjectMemberEnabledDict()
		{
			if (_ObjectMemberEnabledDict == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_ObjectMemberEnabledDict == null)
						{
							_ObjectMemberEnabledDict = new ConcurrentDictionary<Type, bool>();
						}
					}
				}
				else
				{
					_ObjectMemberEnabledDict = new Dictionary<Type, bool>();
				}
			}
		}

		public void AddModule(string name, IScriptModule obj)
		{
			Init_Modules();
			_Modules[name] = obj;
		}

		public void RemoveModule(string name)
		{
			_Modules?.Remove(name);
		}

		public virtual IScriptModule GetModule(string name)
		{
			var modules = _Modules;
			if (modules == null) return null;
			modules.TryGetValue(name, out var module);
			return module;
		}

		public bool TryInstallModule(string name)
		{
			return TryInstallModule(name, out _);
		}

		public bool TryInstallModule(string name, out object obj)
		{
			var module = GetModule(name);
			if (module == null)
			{
				obj = null;
				return false;
			}
			obj = InstallModule(name, module);
			return true;
		}

		/// <summary>
		/// 安装模块，如果父级上下文或者所在语言环境已安装则不会重复安装
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object InstallModule(string name)
		{
			var module = GetModule(name);
			if (module == null) return null;
			return InstallModule(name, module);
		}

		public void UninstallModule(string name)
		{
			var module = GetModule(name);
			if (module == null) return;
			UninstallModule(name, module);
		}

		public object InstallModule(string name, IScriptModule module)
		{
			string key = $"__module_{name}__";
			var instance = this.EvalVar(key, out var type);
			if (type == null)
			{
				instance = module.Install(this);
				type = instance?.GetType() ?? typeof(object);
				this.SetVar(key, instance, type);
			}
			return instance;
		}

		public virtual void UninstallModule(string name, IScriptModule module)
		{
			this.RemoveVar($"__module_{name}__");
			module.Uninstall(this);
		}

		public object InstallModule(IScriptModule module)
		{
			if (module == null) return null;
			return InstallModule(module.GetType().Name, module);
		}

		public void UninstallModule(IScriptModule module)
		{
			if (module == null) return;
			UninstallModule(module.GetType().Name, module);
		}

		public void SetObjectMemberEnabled(Type objType, bool? objectMemberEnabled)
		{
			if (!objectMemberEnabled.HasValue && _ObjectMemberEnabledDict != null)
			{
				_ObjectMemberEnabledDict.Remove(objType);
			}
			else
			{
				Init_ObjectMemberEnabledDict();
				_ObjectMemberEnabledDict[objType] = objectMemberEnabled.Value;
			}
		}

		/// <summary>
		/// 对象内部成员（构造函数、属性、字段、方法）是否可用
		/// </summary>
		/// <returns></returns>
		public virtual bool? IsObjectMemberEnabled(Type objType)
		{
			//var dict = _ObjectMemberEnabledDict;
			//if (dict == null) return null;
			//if (dict.TryGetValue(objType, out var enable))
			//{
			//	return enable;
			//}
			//return null;
			return IsObjectMemberEnabledCore(objType);
		}

		protected virtual bool? IsObjectMemberEnabledCore(Type objType)
		{
			if (objType == null) return null;
			var dict = _ObjectMemberEnabledDict;
			if (dict == null || dict.Count == 0) return null;
			if (dict.TryGetValue(objType, out var enable))
			{
				return enable;
			}
			var interfaces = objType.GetInterfaces();
			foreach (var i in interfaces)
			{
				if (dict.TryGetValue(i, out enable))
				{
					//dict[objType] = enable;
					return enable;
				}
			}
			var r = IsObjectMemberEnabledCore(objType.BaseType);
			//if (r.HasValue)
			//{
			//	dict[objType] = r.Value;
			//}
			return r;
		}

		/// <summary>
		/// 清空所有数据
		/// </summary>
		public virtual void Clear()
		{
			this._Assemblies?.Clear();
			this._Types?.Clear();
			this._Variables?.Clear();
			this._VariableTypes?.Clear();
			this._Functions?.Clear();
			this._Modules?.Clear();
			this._ObjectMemberEnabledDict.Clear();
		}

		public void AddType(string name, Type type)
		{
			Init_Types();
			this._Types[name] = type;
		}

		public void AddType(Type type)
		{
			AddType(type.Name, type);
		}

		public void AddType<T>(string name)
		{
			AddType(name, typeof(T));
		}

		public void AddType<T>()
		{
			AddType(typeof(T));
		}

		public void AddType(string name, Type type, bool memberEnabled)
		{
			AddType(name, type);
			SetObjectMemberEnabled(type, memberEnabled);
		}

		public void AddType(Type type, bool memberEnabled)
		{
			AddType(type.Name, type, memberEnabled);
		}

		public void AddType<T>(string name, bool memberEnabled)
		{
			AddType(name, typeof(T), memberEnabled);
		}

		public void AddType<T>(bool memberEnabled)
		{
			AddType(typeof(T), memberEnabled);
		}

		public void RemoveType(string name)
		{
			this._Types?.Remove(name);
		}

		public void RemoveType(string name, bool removeMemberEnabled)
		{
			var types = this._Types;
			if (types == null) return;
			var objectMemberEnabledDict = this._ObjectMemberEnabledDict;
			if (!removeMemberEnabled || objectMemberEnabledDict == null)
			{
				types.Remove(name);
				return;
			}
			if (!types.TryGetValue(name, out var type)) return;
			types.Remove(name);
			objectMemberEnabledDict.Remove(type);
		}

		public void AddAssembly(string name, Assembly assembly)
		{
			Init_Assemblies();
			this._Assemblies[name] = assembly;
		}

		public void AddAssembly(Assembly assembly)
		{
			AddAssembly(assembly.GetName().Name, assembly);
		}

		public void RemoveAssembly(string name)
		{
			this._Assemblies?.Remove(name);
		}

		/// <summary>
		/// 设置变量
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetVar(string name, object value)
		{
			SetVar(name, value, null);
		}

		/// <summary>
		/// 设置变量
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="valueType"></param>
		public virtual void SetVar(string name, object value, Type valueType)
		{
			if (valueType == null)
			{
				valueType = value?.GetType() ?? typeof(object);
			}
			Init_Variables();
			Init_VariableTypes();
			this._Variables[name] = value;
			this._VariableTypes[name] = valueType;
			this._VariableModifiers?.Remove(name);
		}

		/// <summary>
		/// 设置变量
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetVar<T>(string name, T value)
		{
			SetVar(name, value, typeof(T));
		}

		/// <summary>
		/// 设置常量（脚本中不可修改该常量）
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetConst(string name, object value)
		{
			SetConst(name, value, null);
		}

		/// <summary>
		/// 设置常量（脚本中不可修改该常量）
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="valueType"></param>
		public void SetConst(string name, object value, Type valueType)
		{
			SetVar(name, value, valueType);
			SetVarModifier(name, Modifiers.READONLY);
		}

		/// <summary>
		/// 设置常量（脚本中不可修改该常量）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetConst<T>(string name, T value)
		{
			SetConst(name, value, typeof(T));
		}

		/// <summary>
		/// 设置变量修饰符
		/// </summary>
		/// <param name="name"></param>
		/// <param name="modifier"></param>
		protected void SetVarModifier(string name, int modifier)
		{
			Init_VariableModifiers();
			this._VariableModifiers[name] = modifier;
		}

		/// <summary>
		/// 获取变量修饰符
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected int GetVarModifier(string name)
		{
			var modifiers = this._VariableModifiers;
			if (modifiers == null) return 0;
			modifiers.TryGetValue(name, out var modifier);
			return modifier;
		}

		/// <summary>
		/// 删除变量
		/// </summary>
		/// <param name="name"></param>
		public virtual void RemoveVar(string name)
		{
			this._Variables?.Remove(name);
			this._VariableTypes?.Remove(name);
			this._VariableModifiers?.Remove(name);
		}

		public object EvalVar(string name)
		{
			return EvalVar(name, out _);
		}

		public virtual object EvalVar(string name, out Type type)
		{
			if (_Variables != null && _Variables.TryGetValue(name, out var v))
			{
				if (_VariableTypes == null)
				{
					type = v?.GetType();
				}
				else if (!_VariableTypes.TryGetValue(name, out type))
				{
					type = v?.GetType();
				}
				return v;
			}
			// 没有变量，则查找类
			var mytype = EvalType(name);
			if (mytype != null)
			{
				type = typeof(TypeWrapper);
				return new TypeWrapper(name, mytype);
			}
			type = null;
			return null;
		}

		public virtual Type EvalType(string name)
		{
			if (_Types != null && _Types.TryGetValue(name, out var type))
			{
				return type;
			}
			return null;
		}

		public void AddTokenHandler(string name, ITokenHandler handler)
		{
			Init_TokenHandlerDict();
			_TokenHandlerDict[name] = handler;
		}

		public void AddTokenHandler(ITokenHandler handler)
		{
			Init_TokenHandlers();
			_TokenHandlers.Add(handler);
		}

		public virtual bool HasFunc(string name)
		{
			var functions = _Functions;
			if (functions == null) return false;
			return functions.ContainsKey(name);
		}

		public void AddFunc(string name, IFunctionEvaluator func)
		{
			Init_FunctionEvaluators();
			if (!_FunctionEvaluators.TryGetValue(name, out var list))
			{
				if (_FunctionEvaluators is ConcurrentDictionary<string, IList<IFunctionEvaluator>> condict)
				{
					list = condict.GetOrAdd(name, k => new List<IFunctionEvaluator>());
				}
				else
				{
					_FunctionEvaluators[name] = list = new List<IFunctionEvaluator>();
				}
			}
			if (_FunctionEvaluators is ConcurrentDictionary<string, IList<IFunctionEvaluator>>)
			{
				lock (list)
				{
					list.Add(func);
				}
			}
			else
			{
				list.Add(func);
			}
		}

		public void AddLambda(string name, LambdaExpression lambda)
		{
			AddFunc(name, new LambdaFunction(lambda));
		}

		public void AddLambda<TFunc>(string name, Expression<TFunc> lambda) where TFunc : Delegate
		{
			AddFunc(name, new LambdaFunction(lambda));
		}

		/// <summary>
		/// 添加类型中的所有公开静态方法
		/// </summary>
		/// <param name="type"></param>
		public void AddFunc(Type type)
		{
			AddFunc(type, null, null);
		}

		/// <summary>
		/// 如果target为null，则添加类型中的公开静态方法，否则添加实例公开方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">实例对象</param>
		public void AddFunc(Type type, object target)
		{
			AddFunc(type, target, null);
		}

		/// <summary>
		/// 添加类型中的所有公开静态方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodNameMap">方法名映射，如果返回名称为空则不添加该方法</param>
		public void AddFunc(Type type, Func<MethodInfo, string> methodNameMap)
		{
			AddFunc(type, null, methodNameMap);
		}

		/// <summary>
		/// 如果target为null，则添加类型中的公开静态方法，否则添加实例公开方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">实例对象</param>
		/// <param name="methodNameMap">方法名映射，如果返回名称为空则不添加该方法</param>
		public void AddFunc(Type type, object target, Func<MethodInfo, string> methodNameMap)
		{
			var methods = target == null ? type.GetMethods(BindingFlags.Public | BindingFlags.Static) : type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach (var method in methods)
			{
				// 跳过属性访问器等特殊方法
				if (method.IsSpecialName) continue;
				// 方法名
				string name;
				if (methodNameMap == null) name = method.Name;
				else
				{
					name = methodNameMap(method);
					if (string.IsNullOrEmpty(name)) continue;
				}
				if (target == null && type.IsAbstract && type.IsSealed
					&& (typeof(LambdaExpression).IsAssignableFrom(method.ReturnType) || typeof(IList<LambdaExpression>).IsAssignableFrom(method.ReturnType))
					&& method.GetParameters().Length == 0)
				{
					var result = method.Invoke(null, null);
					if (result is LambdaExpression lambda) AddLambda(name, lambda);
					else if (result is IList<LambdaExpression> list)
					{
						foreach (var item in list)
						{
							AddLambda(name, item);
						}
					}
				}
				else
				{
					AddFunc(name, method, target);
				}
			}
		}

		/// <summary>
		/// 添加类型中的所有公开静态方法
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		public void AddFunc<TType>()
		{
			AddFunc(typeof(TType));
		}

		/// <summary>
		/// 添加类型中的所有公开实例方法
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="instance">实例</param>
		public void AddFunc<TType>(TType instance)
		{
			AddFunc(typeof(TType), instance);
		}

		/// <summary>
		/// 添加类型中的所有公开静态方法
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="methodNameMap">方法名映射</param>
		public void AddFunc<TType>(Func<MethodInfo, string> methodNameMap)
		{
			AddFunc(typeof(TType), methodNameMap);
		}

		/// <summary>
		/// 添加类型中的所有公开实例方法
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="instance">实例</param>
		/// <param name="methodNameMap">方法名映射</param>
		public void AddFunc<TType>(TType instance, Func<MethodInfo, string> methodNameMap)
		{
			AddFunc(typeof(TType), instance, methodNameMap);
		}

		/// <summary>
		/// 添加方法
		/// </summary>
		/// <param name="method"></param>
		/// <param name="target"></param>
		public void AddFunc(MethodInfo method, object target = null)
		{
			AddFunc(method.Name, method, target);
		}

		/// <summary>
		/// 添加方法
		/// </summary>
		/// <param name="name"></param>
		/// <param name="method"></param>
		/// <param name="target"></param>
		public void AddFunc(string name, MethodInfo method, object target = null)
		{
			if (string.IsNullOrEmpty(name)) name = method.Name;
			if (method.IsGenericMethod)
			{
				// 泛型方法
				AddFunc(name, new GenericFunction(method, target));
			}
			else
			{
				//var del = ScriptUtils.CreateDelegate(method, target);
				//if (del != null) AddFunc(name, del);
				AddFunc(name, new NonGenericFunction(method, target));
			}
		}

		public void AddFunc(string name, Delegate d)
		{
			Init_Functions();
			if (!_Functions.TryGetValue(name, out var list))
			{
				if (_Functions is ConcurrentDictionary<string, List<Delegate>> con)
				{
					list = con.GetOrAdd(name, key => new List<Delegate>());
				}
				else
				{
					_Functions[name] = list = new List<Delegate>();
				}
			}
			if (_ThreadSafely)
			{
				lock (this)
				{
					list.Add(d);
				}
			}
			else
			{
				list.Add(d);
			}
		}

		public void AddFunc<TReturn>(string name, Func<TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddFunc<T1, TReturn>(string name, Func<T1, TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddFunc<T1, T2, TReturn>(string name, Func<T1, T2, TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddFunc<T1, T2, T3, TReturn>(string name, Func<T1, T2, T3, TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddFunc<T1, T2, T3, T4, TReturn>(string name, Func<T1, T2, T3, T4, TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddFunc<T1, T2, T3, T4, T5, TReturn>(string name, Func<T1, T2, T3, T4, T5, TReturn> func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddAction(string name, Action func)
		{
			AddFunc(name, (Delegate)func);
		}

		public void AddAction<T1>(string name, Action<T1> action)
		{
			AddFunc(name, (Delegate)action);
		}

		public void AddAction<T1, T2>(string name, Action<T1, T2> action)
		{
			AddFunc(name, (Delegate)action);
		}

		public void AddAction<T1, T2, T3>(string name, Action<T1, T2, T3> action)
		{
			AddFunc(name, (Delegate)action);
		}

		public void AddAction<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
		{
			AddFunc(name, (Delegate)action);
		}

		public void AddAction<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> action)
		{
			AddFunc(name, (Delegate)action);
		}

		protected virtual void OnFunctionEval(FunctionEvalArgs e)
		{
			if (e.IsHandled) return;
			this.FunctionEval?.Invoke(this, e);
		}

		protected virtual void OnFunctionBuild(FunctionBuildArgs e)
		{
			if (e.Result != null) return;
			this.FunctionBuild?.Invoke(this, e);
		}
	}
}
