using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript
{
	public class ScriptLang
	{
		/// <summary>
		/// 函数运算事件
		/// </summary>
		public event EventHandler<FunctionEvalArgs> FunctionEval;
		/// <summary>
		/// 函数编译事件
		/// </summary>
		public event EventHandler<FunctionBuildArgs> FunctionBuild;

		/// <summary>
		/// 函数处理器（包含操作符处理）
		/// </summary>
		private ConcurrentDictionary<string, IList<IFunctionEvaluator>> _FunctionEvaluators;
		/// <summary>
		/// 语句处理器
		/// </summary>
		private ConcurrentDictionary<string, ITokenHandler> _TokenHandlerDict;

		private List<ITokenHandler> _TokenHandlers;

		/// <summary>
		/// 程序集
		/// </summary>
		private ConcurrentDictionary<string, Assembly> _Assemblies;
		/// <summary>
		/// 类型定义
		/// </summary>
		private ConcurrentDictionary<string, Type> _Types;
		/// <summary>
		/// 全局变量
		/// </summary>
		private ConcurrentDictionary<string, object> _Variables;
		/// <summary>
		/// 全局变量类型
		/// </summary>
		private ConcurrentDictionary<string, Type> _VariableTypes;
		/// <summary>
		/// 函数列表
		/// </summary>
		private ConcurrentDictionary<string, List<Delegate>> _Functions;

		private readonly bool _IgnoreCase;

		/// <summary>
		/// 关键字（函数名、变量名、类名）是否忽略大小写
		/// </summary>
		public bool IgnoreCase => _IgnoreCase;

		/// <summary>
		/// 语言兼容性，表示是否与其他语言兼容运行，如果不兼容则需指定语言执行
		/// </summary>
		public bool Compatible { get; set; } = true;

		/// <summary>
		/// 
		/// </summary>
		public ScriptLang() { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreCase">关键字是否忽略大小写</param>
		public ScriptLang(bool ignoreCase)
		{
			this._IgnoreCase = ignoreCase;
		}

		private void Init_FunctionEvaluators()
		{
			if (_FunctionEvaluators == null)
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
		}

		private void Init_TokenHandlerDict()
		{
			if (_TokenHandlerDict == null)
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
		}

		private void Init_TokenHandlers()
		{
			if (_TokenHandlers == null)
			{
				lock (this)
				{
					if (_TokenHandlers == null)
					{
						_TokenHandlers = new List<ITokenHandler>();
					}
				}
			}
		}

		private void Init_Assemblies()
		{
			if (_Assemblies == null)
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
		}

		private void Init_Types()
		{
			if (_Types == null)
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
		}

		private void Init_Variables()
		{
			if (_Variables == null)
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
		}

		private void Init_VariableTypes()
		{
			if (_VariableTypes == null)
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
		}

		private void Init_Functions()
		{
			if (_Functions == null)
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
		}

		/// <summary>
		/// 自定义分词器
		/// </summary>
		/// <param name="charReader"></param>
		/// <returns></returns>
		public virtual ITokenStream GetTokenStream(CharReader charReader)
		{
			return null;
		}

		/// <summary>
		/// 自定义语法分析器
		/// </summary>
		/// <returns></returns>
		public virtual DefaultSyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return null;
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
				return new TypeWrapper(mytype);
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

		protected static Delegate GetFunc(List<Delegate> list, IList<Type> argTypes, out bool useScriptContext)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				var defineArgTypes = d.Method.GetParameters()
					.Where(a => a.ParameterType.FullName != "System.Runtime.CompilerServices.Closure")
					.Select(a => a.ParameterType).ToArray();
				if (ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes))
				{
					useScriptContext = false;
					return d;
				}
				if (defineArgTypes.Length > 0
					&& argTypes.Count == defineArgTypes.Length - 1
					&& ScriptUtils.IsMatchArgType(defineArgTypes[0], typeof(ScriptContext))
					&& ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes, 1))
				{
					// ScriptContext开头的参数匹配
					useScriptContext = true;
					return d;
				}
			}
			useScriptContext = false;
			return null;
		}

		//public Delegate GetFunc(string name, IList<Type> argTypes, out bool useScriptContext)
		//{
		//	if (Functions.TryGetValue(name, out var list))
		//	{
		//		return GetFunc(list, argTypes, out useScriptContext);
		//	}
		//	useScriptContext = false;
		//	return null;
		//}

		public void EvalFunc(FunctionEvalArgs e)
		{
			if (_Functions != null && _Functions.TryGetValue(e.Name, out var list2))
			{
				var types = new Type[e.Args.Count];
				var datas = new object[e.Args.Count];
				for (int i = 0; i < e.Args.Count; i++)
				{
					var arg = e.Args[i];
					var value = arg.Eval(e.Context, e.Options, e.Control, out var type);
					datas[i] = value;
					types[i] = type;
					if (!(arg is ObjectNode))
					{
						PoolManage.Return(arg);
						e.Args[i] = PoolManage.CreateObjectData(value, type);
					}
				}
				var d = GetFunc(list2, types, out var useScriptContext);
				if (d != null)
				{
					var returnType = d.Method.ReturnType ?? typeof(object);
					if (useScriptContext)
					{
						var datas2 = new object[datas.Length + 1];
						datas2[0] = e.Context;
						Array.Copy(datas, 0, datas2, 1, datas.Length);
						datas = datas2;
					}
					e.SetResult(d.DynamicInvoke(datas), returnType);
					return;
				}
			}
			// 
			OnFunctionEval(e);
		}

		public void BuildFunc(FunctionBuildArgs e)
		{
			if (_Functions != null && _Functions.TryGetValue(e.Name, out var list2))
			{
				var argExprs = (e.ArgExprs is Expression[] eas) ? eas : e.ArgExprs?.ToArray();
				Type[] argTypes = null;
				var expr = e.ScriptContext.BuildFunc(e.BuildContext, e.Options, list2, e.Args, ref argExprs, ref argTypes);
				if (expr != null)
				{
					e.Result = expr;
					return;
				}
			}
			OnFunctionBuild(e);
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

		public void RemoveType(string name)
		{
			this._Types?.TryRemove(name, out _);
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
			this._Assemblies?.TryRemove(name, out _);
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

		public virtual void RemoveVar(string name)
		{
			this._Variables?.TryRemove(name, out _);
			this._VariableTypes?.TryRemove(name, out _);
		}

		public void AddFunc(string name, IFunctionEvaluator evaluator)
		{
			Init_FunctionEvaluators();
			if (!_FunctionEvaluators.TryGetValue(name, out var list))
			{
				list = _FunctionEvaluators.GetOrAdd(name, k => new List<IFunctionEvaluator>());
			}
			lock (list)
			{
				list.Add(evaluator);
			}
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
		/// <param name="methodNameMap">方法名映射</param>
		public void AddFunc(Type type, Func<MethodInfo, string> methodNameMap)
		{
			AddFunc(type, null, methodNameMap);
		}

		/// <summary>
		/// 如果target为null，则添加类型中的公开静态方法，否则添加实例公开方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">实例对象</param>
		/// <param name="methodNameMap">方法名映射</param>
		public void AddFunc(Type type, object target, Func<MethodInfo, string> methodNameMap)
		{
			var methods = target == null ? type.GetMethods(BindingFlags.Public | BindingFlags.Static) : type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach (var method in methods)
			{
				// 跳过属性访问器等特殊方法
				if (method.IsSpecialName) continue;
				// 
				var del = ScriptUtils.CreateDelegate(method, target);
				if (del != null)
				{
					AddFunc(methodNameMap?.Invoke(method) ?? method.Name, del);
				}
			}
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
		public void AddFunc<TType>(TType instance)
		{
			AddFunc(typeof(TType), instance);
		}

		public void AddFunc(MethodInfo method, object target = null)
		{
			var del = ScriptUtils.CreateDelegate(method, target);
			if (del != null)
			{
				AddFunc(method.Name, del);
			}
		}

		public void AddFunc(string name, MethodInfo method, object target = null)
		{
			var del = ScriptUtils.CreateDelegate(method, target);
			if (del != null)
			{
				AddFunc(string.IsNullOrEmpty(name) ? method.Name : name, del);
			}
		}

		public void AddFunc(string name, Delegate d)
		{
			Init_Functions();
			if (!_Functions.TryGetValue(name, out var list))
			{
				list = _Functions.GetOrAdd(name, key => new List<Delegate>());
			}
			lock (this)
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

			if (_FunctionEvaluators != null && _FunctionEvaluators.TryGetValue(e.Name, out var list))
			{
				foreach (var item in list)
				{
					item.Eval(e);
					if (e.IsHandled) return;
				}
			}

			this.FunctionEval?.Invoke(this, e);
		}

		protected virtual void OnFunctionBuild(FunctionBuildArgs e)
		{
			if (e.Result != null) return;

			if (_FunctionEvaluators != null && _FunctionEvaluators.TryGetValue(e.Name, out var list))
			{
				foreach (var item in list)
				{
					if (item is IFunctionBuilder builder)
					{
						builder.Build(e);
						if (e.Result != null) return;
					}
				}
			}

			this.FunctionBuild?.Invoke(this, e);
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

		public void HandleToken(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			if (e.IsHandled) return;
			if (_TokenHandlerDict != null && _TokenHandlerDict.TryGetValue(e.CurrentToken.Value, out var handler))
			{
				handler.Build(analyzer, e);
				if (e.IsHandled) return;
			}
			if (_TokenHandlers != null)
			{
				for (int i = 0; i < _TokenHandlers.Count; i++)
				{
					_TokenHandlers[i].Build(analyzer, e);
					if (e.IsHandled) return;
				}
			}
		}
	}
}
