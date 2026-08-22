using AScript.Functions;
using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	/// <summary>
	/// 上下文
	/// </summary>
	public class ScriptContext : BaseContext
	{
		public static readonly ScriptContext Root = new ScriptContext(null, true);

		static ScriptContext()
		{
			Root.AddFunc<ScriptContext, object, string, object>("__GetValue__", ScriptUtils.GetValue);
			Root.AddTokenHandler("#lang", new LangTokenHandler("#end"));
			Root.AddTokenHandler("@lang", new LangTokenHandler("@end"));
		}

		/// <summary>
		/// 上级
		/// </summary>
		public ScriptContext Parent { get; set; }

		/// <summary>
		/// 表达式中的临时变量
		/// </summary>
		private IDictionary<string, object> _TempVariables;
		/// <summary>
		/// 临时变量类型
		/// </summary>
		private IDictionary<string, Type> _TempVariableTypes;

		// 临时函数
		private IDictionary<string, List<Delegate>> _TempFunctions;
		// 临时函数
		private IDictionary<string, List<CustomFunction>> _CustomFunctions;

		// 事件
		private IDictionary<string, Delegate> _Events;

		private string[] _Langs;

		/// <summary>
		/// 指定关联的脚本语言
		/// </summary>
		public string[] Langs
		{
			get => _Langs ?? this.Parent?.Langs;
			set => _Langs = value;
		}

		/// <summary>
		/// 默认Parent为Root
		/// </summary>
		public ScriptContext() : this(Root, false) { }
		/// <summary>
		/// 默认Parent为Root
		/// </summary>
		/// <param name="threadSafely"></param>
		public ScriptContext(bool threadSafely) : this(Root, threadSafely) { }
		public ScriptContext(ScriptContext parent) : this(parent, false) { }
		public ScriptContext(ScriptContext parent, bool threadSafely) : base(threadSafely)
		{
			this.Parent = parent;
		}

		public static ScriptContext Create(bool threadSafely = false)
		{
			return new ScriptContext(Root, threadSafely);
		}

		public static ScriptContext Create(ScriptContext parent, bool threadSafely = false)
		{
			return new ScriptContext(parent, threadSafely);
		}

		private void Init_TempVariables()
		{
			if (_TempVariables == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_TempVariables == null)
						{
							_TempVariables = new ConcurrentDictionary<string, object>();
						}
					}
				}
				else
				{
					_TempVariables = new Dictionary<string, object>();
				}
			}
		}

		private void Init_TempVariableTypes()
		{
			if (_TempVariableTypes == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_TempVariableTypes == null)
						{
							_TempVariableTypes = new ConcurrentDictionary<string, Type>();
						}
					}
				}
				else
				{
					_TempVariableTypes = new Dictionary<string, Type>();
				}
			}
		}

		private void Init_TempFunctions()
		{
			if (_TempFunctions == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_TempFunctions == null)
						{
							_TempFunctions = new ConcurrentDictionary<string, List<Delegate>>();
						}
					}
				}
				else
				{
					_TempFunctions = new Dictionary<string, List<Delegate>>();
				}
			}
		}

		private void Init_CustomFunctions()
		{
			if (_CustomFunctions == null)
			{
				if (_ThreadSafely)
				{
					lock (this)
					{
						if (_CustomFunctions == null)
						{
							_CustomFunctions = new ConcurrentDictionary<string, List<CustomFunction>>();
						}
					}
				}
				else
				{
					_CustomFunctions = new Dictionary<string, List<CustomFunction>>();
				}
			}
		}

		public ITokenStream GetTokenStream(CharReader charReader)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var stream = lang.GetTokenStream(charReader);
						if (stream != null) return stream;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var stream = lang.GetTokenStream(charReader);
						if (stream != null) return stream;
					}
				}
			}
			return null;
		}

		public ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var synalyzer = lang.GetSyntaxAnalyzer();
						if (synalyzer != null) return synalyzer;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var synalyzer = lang.GetSyntaxAnalyzer();
						if (synalyzer != null) return synalyzer;
					}
				}
			}
			return null;
		}

		public int? GetOperatorPriority(string op)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var priority = lang.GetOperatorPriority(op);
						if (priority != null) return priority;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var priority = lang.GetOperatorPriority(op);
						if (priority != null) return priority;
					}
				}
			}
			if (DefaultSyntaxAnalyzer.OperatorPriorities.TryGetValue(op, out var p))
			{
				return p;
			}
			return null;
		}

		/// <summary>
		/// 当前语言是否动态语言
		/// </summary>
		/// <returns></returns>
		public bool? IsDynamicLang()
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						return lang.IsDynamic();
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						return lang.IsDynamic();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 当前语言是否忽略属性大小写
		/// </summary>
		/// <returns></returns>
		public bool? IsIgnoreCase()
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						return lang.IgnoreCase;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						return lang.IgnoreCase;
					}
				}
			}
			return null;
		}

		public bool IsKeywords(string word)
		{
			if (string.IsNullOrEmpty(word)) return false;
			var context = this;
			while (context != null)
			{
				var dict = context._TokenHandlerDict;
				if (dict != null && dict.ContainsKey(word)) return true;
				context = context.Parent;
			}

			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						return lang.IsKeywords(word);
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						return lang.IsKeywords(word);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 当前语言判断对象是否为真
		/// </summary>
		/// <returns></returns>
		public bool IsTrue(object obj)
		{
			if (obj is bool b) return b;
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						return lang.IsTrue(obj);
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						return lang.IsTrue(obj);
					}
				}
			}
			throw new Exceptions.ScriptRuntimeException($"invalid object of type '{obj?.GetType()}', expect bool type");
		}

		/// <summary>
		/// 对象内部成员（构造函数、属性、字段、方法）是否可用
		/// </summary>
		/// <param name="objType"></param>
		/// <returns></returns>
		public override bool? IsObjectMemberEnabled(Type objType)
		{
			var context = this;
			while (context != null)
			{
				//var dict = context._ObjectMemberEnabledDict;
				//if (dict != null && dict.TryGetValue(objType, out var enabled))
				//{
				//	return enabled;
				//}
				var enabled = context.IsObjectMemberEnabledCore(objType);
				if (enabled.HasValue) return enabled;
				context = context.Parent;
			}

			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var enabled = lang.IsObjectMemberEnabled(objType);
						if (enabled.HasValue) return enabled;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var enabled = lang.IsObjectMemberEnabled(objType);
						if (enabled.HasValue) return enabled;
					}
				}
			}
			return null;
		}

		public override IScriptModule GetModule(string name)
		{
			var context = this;
			while (context != null)
			{
				var module = context.Modules.Get(name);
				if (module != null) return module;
				context = context.Parent;
			}
			// 从语言环境获取模块
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var module = lang.GetModule(name);
						if (module != null) return module;
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var module = lang.GetModule(name);
						if (module != null) return module;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 清空所有数据
		/// </summary>
		public override void Clear()
		{
			ClearTemp();
			base.Clear();
		}

		/// <summary>
		/// 清空临时数据（临时变量、临时函数）
		/// </summary>
		public void ClearTemp()
		{
			ClearTempVariable();
			ClearTempFunction();
		}

		/// <summary>
		/// 清空临时变量
		/// </summary>
		public void ClearTempVariable()
		{
			this._TempVariables?.Clear();
			this._TempVariableTypes?.Clear();
		}

		/// <summary>
		/// 清空临时函数
		/// </summary>
		public void ClearTempFunction()
		{
			this._TempFunctions?.Clear();
			this._CustomFunctions?.Clear();
		}

		/// <summary>
		/// 获取变量所在的上下文
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="searchType"></param>
		/// <returns></returns>
		public ScriptContext GetOwnerContext(string variable, out object value, out Type type, bool searchType = false)
		{
			return GetOwnerContext(variable, out value, out type, out _, searchType);
		}

		/// <summary>
		/// 获取变量所在的上下文
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="modifier‌">变量修饰符</param>
		/// <param name="searchType"></param>
		/// <returns></returns>
		public ScriptContext GetOwnerContext(string variable, out object value, out Type type, out int modifier‌, bool searchType = false)
		{
			var context = this;
			do
			{
				var tempVariables = context._TempVariables;
				if (tempVariables != null && tempVariables.TryGetValue(variable, out value))
				{
					var tempVariableTypes = context._TempVariableTypes;
					if (tempVariableTypes == null || !tempVariableTypes.TryGetValue(variable, out type))
					{
						type = value == null ? typeof(object) : value.GetType();
					}
					modifier = context.GetVarModifier(variable);
					return context;
				}
				var variables = context._Variables;
				if (variables != null && variables.TryGetValue(variable, out value))
				{
					var variableTypes = context._VariableTypes;
					if (variableTypes == null || !variableTypes.TryGetValue(variable, out type))
					{
						type = value == null ? typeof(object) : value.GetType();
					}
					modifier = context.GetVarModifier(variable);
					return context;
				}
				var types = context._Types;
				if (searchType && types != null && types.TryGetValue(variable, out var c))
				{
					type = typeof(TypeWrapper);
					value = new TypeWrapper(variable, c);
					modifier = 0;
					return context;
				}
				context = context.Parent;
			} while (context != null);

			value = null;
			type = null;
			modifier = 0;
			return null;
		}

		/// <summary>
		/// 获取变量所在的上下文
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ScriptContext GetOwnerContext<T>(string variable, out T value)
		{
			var context = this;
			do
			{
				var tempVariables = context._TempVariables;
				if (tempVariables != null && tempVariables.TryGetValue(variable, out var v1))
				{
					value = (T)v1;
					return context;
				}
				var variables = context._Variables;
				if (variables != null && variables.TryGetValue(variable, out var v2))
				{
					value = (T)v2;
					return context;
				}
				context = context.Parent;
			} while (context != null);

			value = default;
			return null;
		}

		/// <summary>
		/// 获取变量所在的上下文
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="value"></param>
		/// <param name="modifier‌">变量修饰符</param>
		/// <returns></returns>
		public ScriptContext GetOwnerContext<T>(string variable, out T value, out int modifier‌)
		{
			var context = this;
			do
			{
				var tempVariables = context._TempVariables;
				if (tempVariables != null && tempVariables.TryGetValue(variable, out var v1))
				{
					value = (T)v1;
					modifier = context.GetVarModifier(variable);
					return context;
				}
				var variables = context._Variables;
				if (variables != null && variables.TryGetValue(variable, out var v2))
				{
					value = (T)v2;
					modifier = context.GetVarModifier(variable);
					return context;
				}
				context = context.Parent;
			} while (context != null);
			value = default;
			modifier = 0;
			return null;
		}

		public TDelegate GetEvent<TDelegate>(string name) where TDelegate : Delegate
		{
			var d = GetEvent(name, typeof(TDelegate));
			return (TDelegate)d;
		}

		public Delegate GetEvent(string name, Type delegateType)
		{
			string eventKey = $"{name}_{delegateType.GetHashCode()}";
			var context = this;
			while (context != null)
			{
				var events = context._Events;
				if (events != null && events.TryGetValue(eventKey, out var e))
				{
					return e;
				}
				context = context.Parent;
			}
			return null;
		}

		public void SetEvent(string name, Delegate eventHandler)
		{
			string eventKey = $"{name}_{eventHandler.GetType().GetHashCode()}";
			if (_Events == null)
			{
				_Events = new Dictionary<string, Delegate>();
			}
			_Events[eventKey] = eventHandler;
		}

		public TDelegate GetOrCreateEvent<TDelegate>(string name) where TDelegate : Delegate
		{
			var d = GetOrCreateEvent(name, typeof(TDelegate));
			return (TDelegate)d;
		}

		public Delegate GetOrCreateEvent(string name, Type delegateType)
		{
			string eventKey = $"{name}_{delegateType.GetHashCode()}";
			var context = this;
			while (context != null)
			{
				var events = context._Events;
				if (events != null && events.TryGetValue(eventKey, out var e))
				{
					return e;
				}
				context = context.Parent;
			}

			var argTypes = delegateType.GetMethod("Invoke").GetParameters().Select(a => a.ParameterType).ToArray();
			context = this;
			while (context != null)
			{
				var customFunctions = context._CustomFunctions;
				if (customFunctions != null && customFunctions.TryGetValue(name, out var list2))
				{
					// 移除未编译的临时函数，编译后缓存
					var func = GetAndRemoveFunc(list2, argTypes);
					if (func != null)
					{
						var del = func.Compile(delegateType, this, null);
						// 缓存编译结果
						context.AddTempFunc(name, del);
						if (context._Events == null)
						{
							context._Events = new Dictionary<string, Delegate>();
						}
						context._Events[eventKey] = del;
						return del;
					}
				}
				var tempFunctions = context._TempFunctions;
				if (tempFunctions != null && tempFunctions.TryGetValue(name, out var list1))
				{
					var func = GetFunc(list1, argTypes, out _, out _, out _);
					if (func != null)
					{
						if (context._Events == null)
						{
							context._Events = new Dictionary<string, Delegate>();
						}
						if (func.GetType() == delegateType)
						{
							context._Events[eventKey] = func;
						}
						else
						{
							func = Delegate.CreateDelegate(delegateType, func.Method);
							context._Events[eventKey] = func;
						}
						return func;
					}
				}
				var functions = context._Functions;
				if (functions != null && functions.TryGetValue(name, out var list3))
				{
					var func = GetFunc(list3, argTypes, out _, out _, out _);
					if (func != null)
					{
						if (context._Events == null)
						{
							context._Events = new Dictionary<string, Delegate>();
						}
						if (func.GetType() == delegateType)
						{
							context._Events[eventKey] = func;
						}
						else
						{
							func = Delegate.CreateDelegate(delegateType, func.Method);
							context._Events[eventKey] = func;
						}
						return func;
					}
				}
				context = context.Parent;
			}
			return null;
		}

		public override object EvalVar(string name, out Type type)
		{
			var context = GetOwnerContext(name, out var value, out type, true);
			if (context == null)
			{
				// 从语言上下文中搜索
				value = EvalVarFromLangs(name, out type);
			}
			if (type == null) return value;
			if (value == null && type.IsValueType)
			{
				// 值类型的变量未赋值，则取该变量时初始化
				value = ScriptUtils.GetDefaultValue(type);
				//(context ?? this)._TempVariables[name] = value;
			}
			return value;
		}

		public override bool TryEvalVar<T>(string name, out T value)
		{
			var context = GetOwnerContext<T>(name, out value);
			if (context != null) return true;
			// 从语言上下文中搜索
			return TryEvalVarFromLangs<T>(name, out value);
		}

		public object EvalVarFromLangs(string name, out Type type)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var value = lang.EvalVar(name, out type);
						if (type != null)
						{
							return value;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var value = lang.EvalVar(name, out type);
						if (type != null)
						{
							return value;
						}
					}
				}
			}
			type = null;
			return null;
		}

		public object EvalVarFromLangs(string name, out Type type, out int modifier)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						var value = lang.EvalVar(name, out type);
						if (type != null)
						{
							modifier = lang.GetVarModifier(name);
							return value;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						var value = lang.EvalVar(name, out type);
						if (type != null)
						{
							modifier = lang.GetVarModifier(name);
							return value;
						}
					}
				}
			}
			type = null;
			modifier = 0;
			return null;
		}

		public T EvalVarFromLangs<T>(string name)
		{
			return EvalVarFromLangs<T>(name, false);
		}

		public T EvalVarFromLangs<T>(string name, bool throwExceptionIfNotExists)
		{
			if (!TryEvalVarFromLangs<T>(name, out var value) && throwExceptionIfNotExists)
			{
				throw new Exceptions.ScriptRuntimeException($"variable '{name}' is not exists");
			}
			return value;
		}

		public bool TryEvalVarFromLangs<T>(string name, out T value)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						if (lang.TryEvalVar<T>(name, out value))
						{
							return true;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						if (lang.TryEvalVar<T>(name, out value))
						{
							return true;
						}
					}
				}
			}
			value = default;
			return false;
		}

		public bool TryEvalVarFromLangs<T>(string name, out T value, out int modifier)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var item in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(item, out var lang))
					{
						if (lang.TryEvalVar<T>(name, out value))
						{
							modifier = lang.GetVarModifier(name);
							return true;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						if (lang.TryEvalVar<T>(name, out value))
						{
							modifier = lang.GetVarModifier(name);
							return true;
						}
					}
				}
			}
			value = default;
			modifier = 0;
			return false;
		}

		public void EvalAction(BuildOptions options, EvalControl control, string name, IList<ITreeNode> args)
		{
			EvalFunc(options, control, name, false, args, out _);
		}

		public object EvalFunc(BuildOptions options, EvalControl control, string name, IList<ITreeNode> args)
		{
			return EvalFunc(options, control, name, args, out _);
		}

		public object EvalFunc(BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			return EvalFunc(options, control, name, isPrefix, args, out _);
		}

		public object EvalFunc(BuildOptions options, EvalControl control, string name, IList<ITreeNode> args, out Type returnType)
		{
			return EvalFunc(options, control, name, false, args, out returnType);
		}

		public Task<EvalResult> EvalFuncAsync(BuildOptions options, EvalControl control, string name, IList<ITreeNode> args, CancellationToken cancellationToken = default)
		{
			return EvalFuncAsync(options, control, name, false, args, cancellationToken);
		}

		public object EvalFunc(BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, out Type returnType)
		{
			var functionEvalArgs = FunctionEvalArgs.Create(this, options, control, name, isPrefix, args);
			try
			{
				var context = this;
				while (context != null)
				{
					// 事件
					context.OnFunctionEval(functionEvalArgs);
					if (functionEvalArgs.IsHandled)
					{
						returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return functionEvalArgs.Result;
					}
					// 自定义函数
					EvalFunc(functionEvalArgs, context._CustomFunctions);
					if (functionEvalArgs.IsHandled)
					{
						returnType = functionEvalArgs.ResultType;
						return functionEvalArgs.Result;
					}
					// 临时函数
					EvalFunc(functionEvalArgs, context._TempFunctions);
					if (functionEvalArgs.IsHandled)
					{
						returnType = functionEvalArgs.ResultType;
						return functionEvalArgs.Result;
					}
					// 全局函数
					EvalFunc(functionEvalArgs, context._Functions);
					if (functionEvalArgs.IsHandled)
					{
						returnType = functionEvalArgs.ResultType;
						return functionEvalArgs.Result;
					}
					// 
					EvalFunc(functionEvalArgs, context._FunctionEvaluators);
					if (functionEvalArgs.IsHandled)
					{
						returnType = functionEvalArgs.ResultType;
						return functionEvalArgs.Result;
					}
					context = context.Parent;
				}
				// 脚本语言环境
				var langs = this.Langs;
				if (langs == null || langs.Length == 0)
				{
					foreach (var item in Script.Langs.GetDefaults())
					{
						if (Script.Langs.TryGetValue(item, out var lang))
						{
							lang.EvalFunc(functionEvalArgs);
							if (functionEvalArgs.IsHandled)
							{
								returnType = functionEvalArgs.ResultType;
								return functionEvalArgs.Result;
							}
						}
					}
				}
				else
				{
					foreach (var langName in langs)
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							lang.EvalFunc(functionEvalArgs);
							if (functionEvalArgs.IsHandled)
							{
								returnType = functionEvalArgs.ResultType;
								return functionEvalArgs.Result;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				FunctionEvalArgs.Return(functionEvalArgs);
				throw;
			}
			try
			{
				// 获取Delegate变量
				var ownerContext = GetOwnerContext(name, out var value, out _, false);
				if (ownerContext == null)
				{
					value = EvalVarFromLangs(name, out _);
				}
				functionEvalArgs.EvalArgs();
				if (value is Delegate || value is CustomFunctionObject)
				{
					if (value is Delegate del)
					{
						returnType = del.Method.ReturnType;
						return del.DynamicInvoke(functionEvalArgs.ArgValues);
					}
					if (value is CustomFunctionObject customFunctionObject)
					{
						returnType = customFunctionObject.Function.ReturnType;
						return customFunctionObject.DynamicInvoke(this, functionEvalArgs.ArgValues);
					}
				}
				// dynamic对象
				if (functionEvalArgs.ArgValues != null && functionEvalArgs.ArgValues.Length > 0)
				{
					var arg0 = functionEvalArgs.ArgValues[0];
					if (arg0 is DynamicObject dynamicObject)
					{
						var dynamicArgs = new object[functionEvalArgs.ArgValues.Length - 1];
						Array.Copy(functionEvalArgs.ArgValues, 1, dynamicArgs, 0, dynamicArgs.Length);
						if (ScriptUtils.TryInvokeDynamicObject(dynamicObject, name, dynamicArgs, out var dynamicValue))
						{
							returnType = dynamicValue?.GetType() ?? typeof(object);
							return dynamicValue;
						}
					}
				}
				// 抛出未知函数异常
				var types = functionEvalArgs.ArgTypes;
				//// 判断前置/后置运算符或者函数调用
				//string funcName = isPrefix || args.Length > 1 || !DefaultAnalyzer.OperatorPriorities.ContainsKey(name) ?
				//	$"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})" :
				//	$"({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))}){name}";
				string funcName = types == null || types.Length == 0 ? $"{name}()" : $"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})";
				throw new Exceptions.ScriptRuntimeException($"unknown function: {funcName}");
			}
			finally
			{
				FunctionEvalArgs.Return(functionEvalArgs);
			}
		}

		public async Task<EvalResult> EvalFuncAsync(BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, CancellationToken cancellationToken = default)
		{
			var functionEvalArgs = FunctionEvalArgs.Create(this, options, control, name, isPrefix, args);
			try
			{
				var context = this;
				while (context != null)
				{
					// 事件
					context.OnFunctionEval(functionEvalArgs);
					if (functionEvalArgs.IsHandled)
					{
						var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return new EvalResult(functionEvalArgs.Result, returnType);
					}
					// 自定义函数
					await EvalFuncAsync(functionEvalArgs, context._CustomFunctions, cancellationToken).ConfigureAwait(false);
					if (functionEvalArgs.IsHandled)
					{
						var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return new EvalResult(functionEvalArgs.Result, returnType);
					}
					// 临时函数
					await EvalFuncAsync(functionEvalArgs, context._TempFunctions, cancellationToken).ConfigureAwait(false);
					if (functionEvalArgs.IsHandled)
					{
						var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return new EvalResult(functionEvalArgs.Result, returnType);
					}
					// 全局函数
					await EvalFuncAsync(functionEvalArgs, context._Functions, cancellationToken).ConfigureAwait(false);
					if (functionEvalArgs.IsHandled)
					{
						var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return new EvalResult(functionEvalArgs.Result, returnType);
					}
					// 
					await EvalFuncAsync(functionEvalArgs, context._FunctionEvaluators, cancellationToken).ConfigureAwait(false);
					if (functionEvalArgs.IsHandled)
					{
						var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
						return new EvalResult(functionEvalArgs.Result, returnType);
					}
					context = context.Parent;
				}
				// 脚本语言环境
				var langs = this.Langs;
				if (langs == null || langs.Length == 0)
				{
					foreach (var item in Script.Langs.GetDefaults())
					{
						if (Script.Langs.TryGetValue(item, out var lang))
						{
							await lang.EvalFuncAsync(functionEvalArgs, cancellationToken).ConfigureAwait(false);
							if (functionEvalArgs.IsHandled)
							{
								var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
								return new EvalResult(functionEvalArgs.Result, returnType);
							}
						}
					}
				}
				else
				{
					foreach (var langName in langs)
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							await lang.EvalFuncAsync(functionEvalArgs, cancellationToken).ConfigureAwait(false);
							if (functionEvalArgs.IsHandled)
							{
								var returnType = functionEvalArgs.ResultType ?? functionEvalArgs.Result?.GetType() ?? typeof(object);
								return new EvalResult(functionEvalArgs.Result, returnType);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				FunctionEvalArgs.Return(functionEvalArgs);
				throw;
			}
			try
			{
				// 获取Delegate变量
				var ownerContext = GetOwnerContext(name, out var value, out _, false);
				if (ownerContext == null)
				{
					value = EvalVarFromLangs(name, out _);
				}
				functionEvalArgs.EvalArgs();
				if (value is Delegate || value is CustomFunctionObject)
				{
					if (value is Delegate del)
					{
						var returnType = del.Method.ReturnType;
						var result = del.DynamicInvoke(functionEvalArgs.ArgValues);
						return new EvalResult(result, returnType);
					}
					if (value is CustomFunctionObject customFunctionObject)
					{
						var returnType = customFunctionObject.Function.ReturnType;
						var result = customFunctionObject.DynamicInvoke(this, functionEvalArgs.ArgValues);
						return new EvalResult(result, returnType);
					}
				}
				// dynamic对象
				if (functionEvalArgs.ArgValues != null && functionEvalArgs.ArgValues.Length > 0)
				{
					var arg0 = functionEvalArgs.ArgValues[0];
					if (arg0 is DynamicObject dynamicObject)
					{
						var dynamicArgs = new object[functionEvalArgs.ArgValues.Length - 1];
						Array.Copy(functionEvalArgs.ArgValues, 1, dynamicArgs, 0, dynamicArgs.Length);
						if (ScriptUtils.TryInvokeDynamicObject(dynamicObject, name, dynamicArgs, out var dynamicValue))
						{
							var returnType = dynamicValue?.GetType() ?? typeof(object);
							return new EvalResult(dynamicValue, returnType);
						}
					}
				}
				// 抛出未知函数异常
				var types = functionEvalArgs.ArgTypes;
				//// 判断前置/后置运算符或者函数调用
				//string funcName = isPrefix || args.Length > 1 || !DefaultAnalyzer.OperatorPriorities.ContainsKey(name) ?
				//	$"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})" :
				//	$"({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))}){name}";
				string funcName = types == null || types.Length == 0 ? $"{name}()" : $"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})";
				throw new Exceptions.ScriptRuntimeException($"unknown function: {funcName}");
			}
			finally
			{
				FunctionEvalArgs.Return(functionEvalArgs);
			}
		}

		protected static void EvalFunc(FunctionEvalArgs e, IDictionary<string, List<CustomFunction>> functions)
		{
			if (functions == null || !functions.TryGetValue(e.Name, out var list1))
			{
				return;
			}

			e.EvalArgs();

			var d = GetFunc(list1, e.ArgTypes);
			if (d == null)
			{
				return;
			}

			d.Eval(e);
		}

		protected static async Task EvalFuncAsync(FunctionEvalArgs e, IDictionary<string, List<CustomFunction>> functions, CancellationToken cancellationToken)
		{
			if (functions == null || !functions.TryGetValue(e.Name, out var list1))
			{
				return;
			}

			await e.EvalArgsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

			var d = GetFunc(list1, e.ArgTypes);
			if (d == null)
			{
				return;
			}

			await d.EvalAsync(e, cancellationToken).ConfigureAwait(false);
		}

		internal static void EvalFunc(FunctionEvalArgs e, IDictionary<string, List<Delegate>> functions)
		{
			if (functions == null || !functions.TryGetValue(e.Name, out var list3))
			{
				return;
			}

			e.EvalArgs(false);

			var d = GetFunc(list3, e.ArgTypes, out var useScriptContext, out var hasClosure, out var paramsIndex);
			if (d == null)
			{
				return;
			}

			var returnType = d.Method.ReturnType ?? typeof(object);
			//var argValues = e.ArgValues;
			//var argTypes = e.ArgTypes;
			//if (useScriptContext)
			//{
			//	var datas2 = new object[(argValues?.Length ?? 0) + 1];
			//	datas2[0] = this;
			//	if (argValues != null && argValues.Length > 0)
			//	{
			//		Array.Copy(argValues, 0, datas2, 1, argValues.Length);
			//	}
			//	argValues = datas2;
			//}
			//if (argValues != null && argValues.Length > 0)
			//{
			//	int startIndex = 0;
			//	if (hasClosure) startIndex++;
			//	if (useScriptContext) startIndex++;
			//	var parameters = d.Method.GetParameters();
			//	for (int i = 0; i < argValues.Length; i++)
			//	{
			//		if (i < startIndex) continue;
			//		var paramType = parameters[i].ParameterType;
			//		var dataType = argTypes[i - startIndex];
			//		if (dataType != paramType)
			//		{
			//			var data = argValues[hasClosure ? i - 1 : i];
			//			if (data is IConvertible && !paramType.IsInstanceOfType(data))
			//			{
			//				argValues[hasClosure ? i - 1 : i] = Convert.ChangeType(data, paramType);
			//			}
			//		}
			//	}
			//}
			//var result = d.DynamicInvoke(argValues);
			ParameterInfo[] parameters = null;
			if (e.ArgValues != null)
			{
				for (int i = 0; i < e.ArgValues.Length; i++)
				{
					var arg = e.ArgValues[i];
					if (ScriptUtils.IsDefineFuncNode(arg))
					{
						if (parameters == null) parameters = d.Method.GetParameters();
						e.ArgValues[i] = ScriptUtils.TryParseDelegateArg(e.Context, e.Options, e.Control, arg, parameters[i].ParameterType);
					}
					//if (arg is DefineFuncNode node)
					//{
					//	var f = node.Eval(e.Context, e.Options, e.Control, out _);
					//	if (f is CustomFunctionObject cfo)
					//	{
					//		if (parameters == null) parameters = d.Method.GetParameters();
					//		f = cfo.Compile(parameters[i].ParameterType, e.Options);
					//	}
					//	e.ArgValues[i] = f;
					//}
					//else if (arg is CustomFunction func)
					//{
					//	if (parameters == null) parameters = d.Method.GetParameters();
					//	e.ArgValues[i] = func.Compile(parameters[i].ParameterType, e.Context, e.Options);
					//}
					//else if (arg is CustomFunctionObject cfo)
					//{
					//	if (parameters == null) parameters = d.Method.GetParameters();
					//	e.ArgValues[i] = cfo.Compile(parameters[i].ParameterType, e.Options);
					//}
				}
			}
			var argValues = e.ArgValues;
			var argTypes = e.ArgTypes;
			if (paramsIndex >= 0)
			{
				if (parameters == null) parameters = d.Method.GetParameters();
				var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
				var paramsValues = new object[e.ArgValues.Length - paramsIndex];
				Array.Copy(e.ArgValues, paramsIndex, paramsValues, 0, paramsValues.Length);
				var paramsArr = Array.CreateInstance(itemType, paramsValues.Length);
				for (int i = 0; i < paramsValues.Length; i++)
				{
					paramsArr.SetValue(System.Convert.ChangeType(paramsValues[i], itemType), i);
				}
				var newValues = new object[paramsIndex + 1];
				var newTypes = new Type[newValues.Length];
				Array.Copy(e.ArgValues, 0, newValues, 0, paramsIndex);
				Array.Copy(e.ArgTypes, 0, newTypes, 0, paramsIndex);
				newValues[paramsIndex] = paramsArr;
				newTypes[paramsIndex] = paramsArr.GetType();
				argValues = newValues;
				argTypes = newTypes;
			}
			var result = ScriptUtils.DynamicInvoke(e.Context, d, argValues, argTypes, useScriptContext, hasClosure);
			e.SetResult(result, returnType);
		}

		internal static async Task EvalFuncAsync(FunctionEvalArgs e, IDictionary<string, List<Delegate>> functions, CancellationToken cancellationToken)
		{
			if (functions == null || !functions.TryGetValue(e.Name, out var list3))
			{
				return;
			}

			await e.EvalArgsAsync(false, cancellationToken).ConfigureAwait(false);

			var d = GetFunc(list3, e.ArgTypes, out var useScriptContext, out var hasClosure, out var paramsIndex);
			if (d == null)
			{
				return;
			}

			var returnType = d.Method.ReturnType ?? typeof(object);
			//var argValues = e.ArgValues;
			//var argTypes = e.ArgTypes;
			//if (useScriptContext)
			//{
			//	var datas2 = new object[(argValues?.Length ?? 0) + 1];
			//	datas2[0] = this;
			//	if (argValues != null && argValues.Length > 0)
			//	{
			//		Array.Copy(argValues, 0, datas2, 1, argValues.Length);
			//	}
			//	argValues = datas2;
			//}
			//if (argValues != null && argValues.Length > 0)
			//{
			//	int startIndex = 0;
			//	if (hasClosure) startIndex++;
			//	if (useScriptContext) startIndex++;
			//	var parameters = d.Method.GetParameters();
			//	for (int i = 0; i < argValues.Length; i++)
			//	{
			//		if (i < startIndex) continue;
			//		var paramType = parameters[i].ParameterType;
			//		var dataType = argTypes[i - startIndex];
			//		if (dataType != paramType)
			//		{
			//			var data = argValues[hasClosure ? i - 1 : i];
			//			if (data is IConvertible && !paramType.IsInstanceOfType(data))
			//			{
			//				argValues[hasClosure ? i - 1 : i] = Convert.ChangeType(data, paramType);
			//			}
			//		}
			//	}
			//}
			//var result = d.DynamicInvoke(argValues);
			ParameterInfo[] parameters = null;
			for (int i = 0; i < e.ArgValues.Length; i++)
			{
				var arg = e.ArgValues[i];
				if (ScriptUtils.IsDefineFuncNode(arg))
				{
					if (parameters == null) parameters = d.Method.GetParameters();
					e.ArgValues[i] = ScriptUtils.TryParseDelegateArg(e.Context, e.Options, e.Control, arg, parameters[i].ParameterType);
				}
				//if (arg is DefineFuncNode node)
				//{
				//	var f = (await node.EvalAsync(e.Context, e.Options, e.Control).ConfigureAwait(false)).Value;
				//	if (f is CustomFunctionObject cfo)
				//	{
				//		if (parameters == null) parameters = d.Method.GetParameters();
				//		f = cfo.Compile(parameters[i].ParameterType, e.Options);
				//	}
				//	e.ArgValues[i] = f;
				//}
				//else if (arg is CustomFunction func)
				//{
				//	if (parameters == null) parameters = d.Method.GetParameters();
				//	e.ArgValues[i] = func.Compile(parameters[i].ParameterType, e.Context, e.Options);
				//}
				//else if (arg is CustomFunctionObject cfo)
				//{
				//	if (parameters == null) parameters = d.Method.GetParameters();
				//	e.ArgValues[i] = cfo.Compile(parameters[i].ParameterType, e.Options);
				//}
			}
			var argValues = e.ArgValues;
			var argTypes = e.ArgTypes;
			if (paramsIndex >= 0)
			{
				if (parameters == null) parameters = d.Method.GetParameters();
				var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
				var paramsValues = new object[e.ArgValues.Length - paramsIndex];
				Array.Copy(e.ArgValues, paramsIndex, paramsValues, 0, paramsValues.Length);
				var paramsArr = Array.CreateInstance(itemType, paramsValues.Length);
				for (int i = 0; i < paramsValues.Length; i++)
				{
					paramsArr.SetValue(System.Convert.ChangeType(paramsValues[i], itemType), i);
				}
				var newValues = new object[paramsIndex + 1];
				var newTypes = new Type[newValues.Length];
				Array.Copy(e.ArgValues, 0, newValues, 0, paramsIndex);
				Array.Copy(e.ArgTypes, 0, newTypes, 0, paramsIndex);
				newValues[paramsIndex] = paramsArr;
				newTypes[paramsIndex] = paramsArr.GetType();
				argValues = newValues;
				argTypes = newTypes;
			}
			var result = ScriptUtils.DynamicInvoke(e.Context, d, argValues, argTypes, useScriptContext, hasClosure);
			e.SetResult(result, returnType);
		}

		/// <summary>
		/// 构建函数表达式
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="name"></param>
		/// <param name="isPrefix"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Expression BuildFunc(BuildContext buildContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			Expression[] argExprs = null;
			Type[] argTypes = null;

			// 从编译上下文环境中构建
			var tempBuildContext = buildContext;
			while (tempBuildContext != null)
			{
				var result = BuildFunc(buildContext, options, tempBuildContext.TempFunctions, name, args, ref argExprs, ref argTypes);
				if (result != null) return result;
				if (tempBuildContext.HasDelegateDefine(name))
				{
					if (args != null && args.Count > 0 && argExprs == null)
					{
						argExprs = new Expression[args.Count];
						argTypes = new Type[args.Count];
						for (int i = 0; i < args.Count; i++)
						{
							var arg = args[i].Build(buildContext, this, options);
							argExprs[i] = arg;
							argTypes[i] = arg.Type;
						}
					}
					var del = tempBuildContext.GetDelegateDefine(name, argTypes);
					if (del != null)
					{
						return Expression.Invoke(del, argExprs);
					}
				}
				tempBuildContext = tempBuildContext.Parent;
			}
			// 获取变量
			if (buildContext.TryGetVariableOrParameter(name, out var v))
			{
				if (!buildContext.LastTypes.TryGetValue(name, out var type))
				{
					type = v.Type;
				}
				if (typeof(Delegate).IsAssignableFrom(type))
				{
					if (args != null && args.Count > 0)
					{
						var parameters = type.GetMethod("Invoke").GetParameters();
						if (argExprs == null)
						{
							argExprs = new Expression[args.Count];
							for (int i = 0; i < args.Count; i++)
							{
								var arg = args[i].Build(buildContext, this, options);
								var p = parameters[i];
								if (arg.Type != p.ParameterType)
								{
									arg = Expression.Convert(arg, p.ParameterType);
								}
								argExprs[i] = arg;
							}
						}
					}
					if (type == v.Type)
					{
						return Expression.Invoke(v, argExprs);
					}
					return Expression.Invoke(Expression.Convert(v, type), argExprs);
				}
			}

			// 从脚本上下文环境中构建
			var functionBuildArgs = FunctionBuildArgs.Create(buildContext, this, options, control, name, isPrefix, args);
			try
			{
				var context = this;
				while (context != null)
				{
					// 事件
					context.OnFunctionBuild(functionBuildArgs);
					if (functionBuildArgs.Result != null)
					{
						return functionBuildArgs.Result;
					}
					// 临时函数
					var result = BuildFunc(buildContext, options, context._TempFunctions, name, args, ref argExprs, ref argTypes);
					if (result != null) return result;
					// 全局函数
					result = BuildFunc(buildContext, options, context._Functions, name, args, ref argExprs, ref argTypes);
					if (result != null) return result;
					// 
					BuildFunc(functionBuildArgs, _FunctionEvaluators);
					if (functionBuildArgs.Result != null)
					{
						return functionBuildArgs.Result;
					}
					// 
					context = context.Parent;
				}

				// 从脚本语言环境中构建
				var langs = this.Langs;
				if (langs == null || langs.Length == 0)
				{
					foreach (var langName in Script.Langs.GetDefaults())
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							lang.BuildFunc(functionBuildArgs);
							if (functionBuildArgs.Result != null)
							{
								return functionBuildArgs.Result;
							}
						}
					}
				}
				else
				{
					foreach (var langName in langs)
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							lang.BuildFunc(functionBuildArgs);
							if (functionBuildArgs.Result != null)
							{
								return functionBuildArgs.Result;
							}
						}
					}
				}
			}
			finally
			{
				FunctionBuildArgs.Return(functionBuildArgs);
			}

			//throw new Exceptions.ScriptRuntimeException("unkown function for build:" + name);
			// 构建context.EvalFunc方法调用
			return ExpressionUtils.BuildEval(buildContext, this, options, name, args);
			//string funcName = argTypes == null || argTypes.Length == 0 ? 
			//	$"{name}()" : 
			//	$"{name}({string.Join(",", argTypes.Select(a => (a ?? typeof(object)).FullName))})";
			//throw new Exceptions.ScriptRuntimeException($"unknown function: {funcName}");
		}

		/// <summary>
		/// 构建函数表达式
		/// </summary>
		/// <param name="buildContext"></param>
		/// <param name="options"></param>
		/// <param name="control"></param>
		/// <param name="name"></param>
		/// <param name="isPrefix"></param>
		/// <param name="args"></param>
		/// <param name="argExprs"></param>
		/// <param name="buildEvalEnabled"></param>
		/// <returns></returns>
		public Expression BuildFunc(BuildContext buildContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, Expression[] argExprs, bool buildEvalEnabled = true)
		{
			Type[] argTypes = null;

			// 从编译上下文环境中构建
			var tmpBuildContext = buildContext;
			while (tmpBuildContext != null)
			{
				var result = BuildFunc(buildContext, options, tmpBuildContext.TempFunctions, name, args, ref argExprs, ref argTypes);
				if (result != null) return result;
				if (tmpBuildContext.HasDelegateDefine(name))
				{
					if (args != null && args.Count > 0 && argExprs == null)
					{
						argExprs = new Expression[args.Count];
						argTypes = new Type[args.Count];
						for (int i = 0; i < args.Count; i++)
						{
							var arg = args[i].Build(buildContext, this, options);
							argExprs[i] = arg;
							argTypes[i] = arg.Type;
						}
					}
					var del = tmpBuildContext.GetDelegateDefine(name, argTypes);
					if (del != null)
					{
						return Expression.Invoke(del, argExprs);
					}
				}
				tmpBuildContext = tmpBuildContext.Parent;
			}

			// 从脚本上下文环境中构建
			var functionBuildArgs = FunctionBuildArgs.Create(buildContext, this, options, control, name, isPrefix, args, argExprs);
			try
			{
				var context = this;
				while (context != null)
				{
					// 事件
					context.OnFunctionBuild(functionBuildArgs);
					if (functionBuildArgs.Result != null)
					{
						return functionBuildArgs.Result;
					}
					// 临时函数
					var result = BuildFunc(buildContext, options, context._TempFunctions, name, args, ref argExprs, ref argTypes);
					if (result != null) return result;
					// 全局函数
					result = BuildFunc(buildContext, options, context._Functions, name, args, ref argExprs, ref argTypes);
					if (result != null) return result;
					// 
					BuildFunc(functionBuildArgs, _FunctionEvaluators);
					if (functionBuildArgs.Result != null)
					{
						return functionBuildArgs.Result;
					}
					// 
					context = context.Parent;
				}

				// 从脚本语言环境中构建
				var langs = this.Langs;
				if (langs == null || langs.Length == 0)
				{
					foreach (var langName in Script.Langs.GetDefaults())
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							lang.BuildFunc(functionBuildArgs);
							if (functionBuildArgs.Result != null)
							{
								return functionBuildArgs.Result;
							}
						}
					}
				}
				else
				{
					foreach (var langName in langs)
					{
						if (Script.Langs.TryGetValue(langName, out var lang))
						{
							lang.BuildFunc(functionBuildArgs);
							if (functionBuildArgs.Result != null)
							{
								return functionBuildArgs.Result;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exceptions.ScriptRuntimeException(ex.Message, ex);
			}
			finally
			{
				FunctionBuildArgs.Return(functionBuildArgs);
			}

			if (!buildEvalEnabled) return null;

			//throw new Exceptions.ScriptRuntimeException("unkown function for build:" + name);
			// 构建context.EvalFunc方法调用
			return ExpressionUtils.BuildEval(buildContext, this, options, name, argExprs);
		}

		internal Expression BuildFunc(BuildContext buildContext, BuildOptions options, IDictionary<string, List<Delegate>> functions, string name, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
		{
			if (functions == null || !functions.TryGetValue(name, out var list3)) return null;
			return BuildFunc(buildContext, options, list3, args, ref argExprs, ref argTypes);
			//if (argExprs == null && args != null && args.Count > 0)
			//{
			//	argExprs = new Expression[args.Count];
			//	argTypes = new Type[args.Count];
			//	for (int i = 0; i < args.Count; i++)
			//	{
			//		var arg = args[i];
			//		var expr = arg.Build(buildContext, this, options);
			//		argExprs[i] = expr;
			//		argTypes[i] = expr.Type;
			//		if (!(arg is ExpressionNode))
			//		{
			//			args[i] = PoolManage.CreateExpressionNode(expr);
			//		}
			//	}
			//}
			//else if (argExprs != null && argExprs.Length > 0)
			//{
			//	argTypes = new Type[argExprs.Length];
			//	for (int i = 0; i < argExprs.Length; i++)
			//	{
			//		argTypes[i] = argExprs[i].Type;
			//	}
			//}

			//var d = GetFunc(list3, argTypes, out var useScriptContext, out var hasClosure, out var paramsIndex);
			//if (d != null && argExprs != null && argExprs.Length > 0)
			//{
			//	ParameterInfo[] parameters = null;
			//	for (int i = 0; i < argExprs.Length; i++)
			//	{
			//		if (argExprs[i] == null && argTypes[i] == typeof(Delegate))
			//		{
			//			if (parameters == null) parameters = d.Method.GetParameters();
			//			if (!parameters[i].ParameterType.Name.StartsWith("Func`"))
			//			{
			//				((DefineFuncNode)args[i]).ReturnSystemType = typeof(void);
			//			}
			//			argExprs[i] = args[i].Build(buildContext, this, options);
			//		}
			//	}
			//}
			//return BuildFunc(d, argExprs, useScriptContext, hasClosure);
		}

		internal Expression BuildFunc(BuildContext buildContext, BuildOptions options, IList<Delegate> functions, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
		{
			if (argExprs == null && args != null && args.Count > 0)
			{
				argExprs = new Expression[args.Count];
				argTypes = new Type[args.Count];
				for (int i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					var expr = arg.Build(buildContext, this, options);
					argExprs[i] = expr;
					argTypes[i] = expr.Type;
					if (!(arg is ExpressionNode))
					{
						args[i] = PoolManage.CreateExpressionNode(expr);
					}
				}
			}
			else if (argExprs != null && argExprs.Length > 0)
			{
				argTypes = new Type[argExprs.Length];
				for (int i = 0; i < argExprs.Length; i++)
				{
					argTypes[i] = argExprs[i]?.Type ?? typeof(Delegate);
				}
			}

			var d = GetFunc(functions, argTypes, out var useScriptContext, out var hasClosure, out var paramsIndex);
			if (d != null && argExprs != null && argExprs.Length > 0)
			{
				ParameterInfo[] parameters = null;
				for (int i = 0; i < argExprs.Length; i++)
				{
					if (argExprs[i] == null && argTypes[i] == typeof(Delegate))
					{
						if (parameters == null) parameters = d.Method.GetParameters();
						if (!parameters[i].ParameterType.Name.StartsWith("Func`"))
						{
							((DefineFuncNode)args[i]).ReturnSystemType = typeof(void);
						}
						argExprs[i] = args[i].Build(buildContext, this, options);
					}
				}
				if (paramsIndex >= 0)
				{
					if (parameters == null) parameters = d.Method.GetParameters();
					var itemType = parameters[parameters.Length - 1].ParameterType.GetElementType();
					var paramsExprs = new Expression[argExprs.Length - paramsIndex];
					Array.Copy(argExprs, paramsIndex, paramsExprs, 0, paramsExprs.Length);
					for (int i = 0; i < paramsExprs.Length; i++)
					{
						var p = paramsExprs[i];
						if (p.Type != itemType)
						{
							paramsExprs[i] = Expression.Convert(p, itemType);
						}
					}
					var paramsArr = Expression.NewArrayInit(itemType, paramsExprs);
					var newExprs = new Expression[paramsIndex + 1];
					Array.Copy(argExprs, 0, newExprs, 0, paramsIndex);
					newExprs[paramsIndex] = paramsArr;
					argExprs = newExprs;
				}
			}
			return BuildFunc(d, argExprs, useScriptContext, hasClosure);
		}

		public Expression BuildFunc(Delegate d, Expression[] argExprs, bool useScriptContext, bool hasClosure)
		{
			if (d == null) return null;

			if (useScriptContext)
			{
				if (argExprs == null || argExprs.Length == 0)
				{
					argExprs = new Expression[] { Expression.Constant(this) };
				}
				else
				{
					var argExprs2 = new Expression[argExprs.Length + 1];
					argExprs2[0] = Expression.Constant(this);
					Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
					argExprs = argExprs2;
				}
			}

			//			if (hasClosure)
			//			{
			//				// 有闭包参数，只能通过DynamicInvoke调用，无法用Expression.Call调用
			//				if (argExprs == null)
			//				{
			//#if NETFRAMEWORK
			//					argExprs = new Expression[0];
			//#else
			//					argExprs = Array.Empty<Expression>();
			//#endif
			//				}
			//				else
			//				{
			//					for (int i = 0; i < argExprs.Length; i++)
			//					{
			//						// 转object类型
			//						argExprs[i] = Expression.Convert(argExprs[i], typeof(object));
			//					}
			//				}
			//				var call = Expression.Call(Expression.Constant(d), ExpressionUtils.Method_Delegate_DynamicInvoke, Expression.NewArrayInit(typeof(object), argExprs));
			//				if (d.Method.ReturnType == typeof(object))
			//				{
			//					return call;
			//				}
			//				return Expression.Convert(call, d.Method.ReturnType);
			//			}

			//			//if (hasClosure && d.Target != null)
			//			//{
			//			//	if (argExprs == null || argExprs.Length == 0)
			//			//	{
			//			//		argExprs = new Expression[] { Expression.Constant(d.Target) };
			//			//	}
			//			//	else
			//			//	{
			//			//		var argExprs2 = new Expression[argExprs.Length + 1];
			//			//		argExprs2[0] = Expression.Constant(d.Target);
			//			//		Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
			//			//		argExprs = argExprs2;
			//			//	}
			//			//}
			//			//System.Runtime.CompilerServices.Closure
			//			if (d.Target == null)
			//			{
			//				return Expression.Call(d.Method, argExprs);
			//			}
			//			return Expression.Call(Expression.Constant(d.Target), d.Method, argExprs);

			if (argExprs != null)
			{
				var parameters = d.Method.GetParameters();
				for (int i = 0; i < argExprs.Length; i++)
				{
					var p = parameters[hasClosure ? i + 1 : i];
					var arg = argExprs[i];
					if (!typeof(Delegate).IsAssignableFrom(arg.Type) && arg.Type != p.ParameterType)
					{
						argExprs[i] = Expression.Convert(arg, p.ParameterType);
					}
				}
			}
			return Expression.Invoke(Expression.Constant(d), argExprs);
		}

		protected Expression BuildFunc(BuildContext buildContext, BuildOptions options, IDictionary<string, List<Expression>> functions, string name, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
		{
			if (functions == null || !functions.TryGetValue(name, out var list3)) return null;

			if (argExprs == null)
			{
				int argsCount = args == null ? 0 : args.Count;
				argExprs = new Expression[argsCount];
				argTypes = new Type[argsCount];
				for (int i = 0; i < argsCount; i++)
				{
					var arg = args[i];
					var expr = arg.Build(buildContext, this, options);
					argExprs[i] = expr;
					argTypes[i] = expr.Type;
				}
			}
			else if (argTypes == null)
			{
				argTypes = new Type[argExprs.Length];
				for (int i = 0; i < argExprs.Length; i++)
				{
					argTypes[i] = argExprs[i]?.Type ?? typeof(Delegate);
				}
			}

			var d = GetFunc(list3, argTypes, out var useScriptContext, out var hasClosure);
			if (d == null) return null;

			if (useScriptContext)
			{
				var argExprs2 = new Expression[argExprs.Length + 1];
				argExprs2[0] = Expression.Constant(this);
				Array.Copy(argExprs, 0, argExprs2, 1, argExprs.Length);
				argExprs = argExprs2;
			}

			//if (hasClosure)
			//{
			//	// 有闭包参数，只能通过DynamicInvoke调用，无法用Expression.Call调用
			//	for (int i = 0; i < argExprs.Length; i++)
			//	{
			//		// 转object类型
			//		argExprs[i] = Expression.Convert(argExprs[i], typeof(object));
			//	}
			//	var call = Expression.Invoke(d, argExprs);
			//	if (d.ReturnType == typeof(object))
			//	{
			//		return call;
			//	}
			//	return Expression.Convert(call, d.ReturnType);
			//}

			//if (d.Target == null)
			//{
			//	return Expression.Call(d.Method, argExprs);
			//}
			if (argExprs != null)
			{
				if (d is LambdaExpression lambda)
				{
					var parameters = lambda.Parameters;
					for (int i = 0; i < argExprs.Length; i++)
					{
						var p = parameters[hasClosure ? i + 1 : i];
						var arg = argExprs[i];
						if (arg.Type != p.Type)
						{
							argExprs[i] = Expression.Convert(arg, p.Type);
						}
					}
				}
				else
				{
					var parameters = d.Type.GetMethod("Invoke").GetParameters();
					for (int i = 0; i < argExprs.Length; i++)
					{
						var p = parameters[hasClosure ? i + 1 : i];
						var arg = argExprs[i];
						if (arg.Type != p.ParameterType)
						{
							argExprs[i] = Expression.Convert(arg, p.ParameterType);
						}
					}
				}
			}
#if NET45
			// NET45框架下如果有闭包参数，不直接调用LambdaExpression，需要Expression.Quote包装
			return hasClosure ?
				Expression.Invoke(Expression.Quote(d), argExprs) :
				Expression.Invoke(d, argExprs);
#else
			return Expression.Invoke(d, argExprs);
			//return Expression.Invoke(Expression.Quote(d), argExprs);
#endif
		}

		public object EvalFunc(string name, params object[] args)
		{
			Type[] argTypes;
			if (args == null || args.Length == 0)
			{
				argTypes = null;
			}
			else
			{
				argTypes = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					argTypes[i] = args[i]?.GetType();
				}
			}
			return EvalFunc(name, args, argTypes);
		}

		public object EvalFunc(string name, IList<object> argValues, IList<Type> argTypes)
		{
			return EvalFunc(name, false, argValues, argTypes, out _);
		}

		public Task<EvalResult> EvalFuncAsync(string name, IList<object> argValues, IList<Type> argTypes)
		{
			return EvalFuncAsync(null, name, false, argValues, argTypes);
		}

		public object EvalFunc(string name, IList<object> argValues, IList<Type> argTypes, out Type returnType)
		{
			return EvalFunc(name, false, argValues, argTypes, out returnType);
		}

		public object EvalFunc(string name, bool isPrefix, IList<object> argValues, IList<Type> argTypes)
		{
			return EvalFunc(name, isPrefix, argValues, argTypes, out _);
		}

		public object EvalFunc(string name, bool isPrefix, IList<object> argValues, IList<Type> argTypes, out Type returnType)
		{
			return EvalFunc(null, name, isPrefix, argValues, argTypes, out returnType);
		}

		public object EvalFunc(BuildOptions options, string name, bool isPrefix, IList<object> argValues, IList<Type> argTypes, out Type returnType)
		{
			var argCount = argValues == null ? 0 : argValues.Count;
			var args = new ITreeNode[argCount];
			for (int i = 0; i < argCount; i++)
			{
				var argValue = argValues[i];
				if (argValue is ITreeNode node)
				{
					args[i] = node;
				}
				else
				{
					args[i] = PoolManage.CreateObjectNode(argValue, argValue?.GetType() ?? argTypes?[i]);
				}
			}
			var result = EvalFunc(options ?? new BuildOptions(Script.DefaultOptions), null, name, isPrefix, args, out returnType);
			PoolManage.Return(args);
			return result;
		}

		public async Task<EvalResult> EvalFuncAsync(BuildOptions options, string name, bool isPrefix, IList<object> argValues, IList<Type> argTypes)
		{
			var argCount = argValues == null ? 0 : argValues.Count;
			var args = new ITreeNode[argCount];
			for (int i = 0; i < argCount; i++)
			{
				var argValue = argValues[i];
				if (argValue is ITreeNode node)
				{
					args[i] = node;
				}
				else
				{
					args[i] = PoolManage.CreateObjectNode(argValue, argTypes[i]);
				}
			}
			var result = await EvalFuncAsync(options ?? new BuildOptions(Script.DefaultOptions), null, name, isPrefix, args).ConfigureAwait(false);
			PoolManage.Return(args);
			return result;
		}

		public static void EvalFunc(FunctionEvalArgs e, IDictionary<string, IList<IFunctionEvaluator>> functionEvaluators)
		{
			if (functionEvaluators == null || !functionEvaluators.TryGetValue(e.Name, out var list)) return;

			IFunctionEvaluator paramsFunc = null;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var item = list[i];
				if (item is NonGenericFunction nonGenericFunction)
				{
					e.EvalArgs(false);
					if (!ScriptUtils.IsMatchArgTypes(e.ArgTypes, nonGenericFunction.Method, out _, out _, out var paramsIndex))
					{
						continue;
					}
					if (paramsIndex >= 0)
					{
						paramsFunc = nonGenericFunction;
						continue;
					}
				}
				item.Eval(e);
				if (e.IsHandled) return;
			}

			paramsFunc?.Eval(e);
		}

		public static async Task EvalFuncAsync(FunctionEvalArgs e, IDictionary<string, IList<IFunctionEvaluator>> functionEvaluators, CancellationToken cancellationToken)
		{
			if (functionEvaluators == null || !functionEvaluators.TryGetValue(e.Name, out var list)) return;

			IFunctionEvaluator paramsFunc = null;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var item = list[i];
				if (item is NonGenericFunction nonGenericFunction)
				{
					await e.EvalArgsAsync(false, cancellationToken).ConfigureAwait(false);
					if (!ScriptUtils.IsMatchArgTypes(e.ArgTypes, nonGenericFunction.Method, out _, out _, out var paramsIndex))
					{
						continue;
					}
					if (paramsIndex >= 0)
					{
						paramsFunc = nonGenericFunction;
						continue;
					}
				}
				if (item is IAsyncFunctionEvaluator asyncFunctionEvaluator)
				{
					await asyncFunctionEvaluator.EvalAsync(e, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					item.Eval(e);
				}
				if (e.IsHandled) return;
			}

			if (paramsFunc is IAsyncFunctionEvaluator paramAsyncFunctionEvaluator)
			{
				await paramAsyncFunctionEvaluator.EvalAsync(e, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				paramsFunc?.Eval(e);
			}
		}

		public static void BuildFunc(FunctionBuildArgs e, IDictionary<string, IList<IFunctionEvaluator>> functionEvaluators)
		{
			if (functionEvaluators == null || !functionEvaluators.TryGetValue(e.Name, out var list)) return;

			IFunctionEvaluator paramsFunc = null;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var item = list[i];
				if (item is NonGenericFunction nonGenericFunction)
				{
					var exprs = e.BuildArgs();
					if (!ScriptUtils.IsMatchArgTypes(exprs, nonGenericFunction.Method, out _, out _, out var paramsIndex))
					{
						continue;
					}
					if (paramsIndex >= 0)
					{
						paramsFunc = nonGenericFunction;
						continue;
					}
				}
				if (item is IFunctionBuilder builder)
				{
					builder.Build(e);
					if (e.Result != null) return;
				}
			}

			if (paramsFunc is IFunctionBuilder paramBuilder)
			{
				paramBuilder.Build(e);
			}
		}

		public override Type EvalType(string name)
		{
			if (string.IsNullOrEmpty(name)) return null;
			if (name.EndsWith("[]"))
			{
				var itemType = EvalType(name.Substring(0, name.Length - 2));
				if (itemType == null) return null;
				return itemType.MakeArrayType();
			}
			var context = this;
			while (context != null)
			{
				var types = context._Types;
				if (types != null && types.TryGetValue(name, out var type))
				{
					return type;
				}
				context = context.Parent;
			}
			// 
			return EvalTypeFromLangs(name);
		}

		private Type EvalTypeFromLangs(string name)
		{
			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				foreach (var langName in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(langName, out var lang))
					{
						var type = lang.EvalType(name);
						if (type != null) return type;
					}
				}
			}
			else
			{
				foreach (var langName in langs)
				{
					if (Script.Langs.TryGetValue(langName, out var lang))
					{
						var type = lang.EvalType(name);
						if (type != null) return type;
					}
				}
			}
			return null;
		}

		public override bool HasFunc(string name)
		{
			var context = this;
			while (context != null)
			{
				var customFunctions = context._CustomFunctions;
				if (customFunctions != null && customFunctions.ContainsKey(name)) return true;
				var tempFunctions = context._TempFunctions;
				if (tempFunctions != null && tempFunctions.ContainsKey(name)) return true;
				var functions = context._Functions;
				if (functions != null && functions.ContainsKey(name)) return true;
				context = context.Parent;
			}
			return false;
		}

		private static CustomFunction GetFunc(IList<CustomFunction> list, IList<Type> argTypes)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				if (ScriptUtils.IsMatchArgTypes(argTypes, d.ArgTypes))
				{
					return d;
				}
			}
			return null;
		}

		private static CustomFunction GetAndRemoveFunc(IList<CustomFunction> list, IList<Type> argTypes)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				if (ScriptUtils.IsMatchArgTypes(argTypes, d.ArgTypes))
				{
					list.RemoveAt(i);
					return d;
				}
			}
			return null;
		}

		private static Delegate GetFunc(IList<Delegate> list, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure, out int paramsIndex)
		{
			//int argTypesCount = argTypes == null ? 0 : argTypes.Count;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				if (ScriptUtils.IsMatchArgTypes(argTypes, d.Method, out useScriptContext, out hasClosure, out paramsIndex))
				{
					return d;
				}
			}
			hasClosure = false;
			useScriptContext = false;
			paramsIndex = -1;
			return null;
		}

		public static Expression GetFunc(List<Expression> list, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure)
		{
			int argTypesCount = argTypes == null ? 0 : argTypes.Count;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				if (d is LambdaExpression lambda)
				{
					if (ScriptUtils.IsMatchArgTypes(argTypes, lambda, out useScriptContext, out hasClosure))
					{
						return d;
					}
				}
				else
				{
					if (ScriptUtils.IsMatchArgTypes(argTypes, d.Type.GetMethod("Invoke"), out useScriptContext, out hasClosure, out _))
					{
						return d;
					}
				}
			}
			hasClosure = false;
			useScriptContext = false;
			return null;
		}

		public Delegate GetFunc(string name, params Type[] argTypes)
		{
			return GetFunc(name, (IList<Type>)argTypes);
		}

		public Delegate GetFunc(string name, IList<Type> argTypes)
		{
			return GetFunc(name, argTypes, out _, out _, out _);
		}

		public Delegate GetFunc(string name, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure, out int paramsIndex)
		{
			var context = this;
			while (context != null)
			{
				var customFunctions = context._CustomFunctions;
				if (customFunctions != null && customFunctions.TryGetValue(name, out var list2))
				{
					// 移除未编译的临时函数，编译后缓存
					var func = GetAndRemoveFunc(list2, argTypes);
					if (func != null)
					{
						useScriptContext = false;
						hasClosure = false;
						paramsIndex = -1;
						var del = func.Compile(this, null);
						// 缓存编译结果
						context.AddTempFunc(name, del);
						return del;
					}
				}
				var tempFunctions = context._TempFunctions;
				if (tempFunctions != null && tempFunctions.TryGetValue(name, out var list1))
				{
					var func = GetFunc(list1, argTypes, out useScriptContext, out hasClosure, out paramsIndex);
					if (func != null) return func;
				}
				var functions = context._Functions;
				if (functions != null && functions.TryGetValue(name, out var list3))
				{
					var func = GetFunc(list3, argTypes, out useScriptContext, out hasClosure, out paramsIndex);
					if (func != null) return func;
				}
				context = context.Parent;
			}
			hasClosure = false;
			useScriptContext = false;
			paramsIndex = -1;
			return null;
		}

		public Func<TReturn> GetFunc<TReturn>(string name)
		{
			var del = GetFunc(name);
			return ScriptUtils.ConvertDelegate<Func<TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<TReturn> f) return f;
			//// 类型不匹配时，调用原始委托并转换返回值类型
			//return () => (TReturn)del.DynamicInvoke();
		}

		public Func<T1, TReturn> GetFunc<T1, TReturn>(string name)
		{
			var del = GetFunc(name, typeof(T1));
			return ScriptUtils.ConvertDelegate<Func<T1, TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<T1, TReturn> f) return f;
			//return (a1) => (TReturn)del.DynamicInvoke(a1);
		}

		public Func<T1, T2, TReturn> GetFunc<T1, T2, TReturn>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2));
			return ScriptUtils.ConvertDelegate<Func<T1, T2, TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<T1, T2, TReturn> f) return f;
			//return (a1, a2) => (TReturn)del.DynamicInvoke(a1, a2);
		}

		public Func<T1, T2, T3, TReturn> GetFunc<T1, T2, T3, TReturn>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
			return ScriptUtils.ConvertDelegate<Func<T1, T2, T3, TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<T1, T2, T3, TReturn> f) return f;
			//return (a1, a2, a3) => (TReturn)del.DynamicInvoke(a1, a2, a3);
			//return (Func<T1, T2, T3, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
		}

		public Func<T1, T2, T3, T4, TReturn> GetFunc<T1, T2, T3, T4, TReturn>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
			return ScriptUtils.ConvertDelegate<Func<T1, T2, T3, T4, TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<T1, T2, T3, T4, TReturn> f) return f;
			//return (a1, a2, a3, a4) => (TReturn)del.DynamicInvoke(a1, a2, a3, a4);
			//return (Func<T1, T2, T3, T4, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public Func<T1, T2, T3, T4, T5, TReturn> GetFunc<T1, T2, T3, T4, T5, TReturn>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
			return ScriptUtils.ConvertDelegate<Func<T1, T2, T3, T4, T5, TReturn>>(del);
			//if (del == null) return null;
			//if (del is Func<T1, T2, T3, T4, T5, TReturn> f) return f;
			//return (a1, a2, a3, a4, a5) => (TReturn)del.DynamicInvoke(a1, a2, a3, a4, a5);
			//return (Func<T1, T2, T3, T4, T5, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
		}

		public Action GetAction(string name)
		{
			var del = GetFunc(name);
			return ScriptUtils.ConvertDelegate<Action>(del);
			//if (del == null) return null;
			//if (del is Action act) return act;
			//return () => del.DynamicInvoke();
			//return (Action)GetFunc(name);
		}

		public Action<T1> GetAction<T1>(string name)
		{
			var del = GetFunc(name, typeof(T1));
			return ScriptUtils.ConvertDelegate<Action<T1>>(del);
			//if (del == null) return null;
			//if (del is Action<T1> act) return act;
			//return (a1) => del.DynamicInvoke(a1);
			//return (Action<T1>)GetFunc(name, typeof(T1));
		}

		public Action<T1, T2> GetAction<T1, T2>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2));
			return ScriptUtils.ConvertDelegate<Action<T1, T2>>(del);
			//if (del == null) return null;
			//if (del is Action<T1, T2> act) return act;
			//return (a1, a2) => del.DynamicInvoke(a1, a2);
			//return (Action<T1, T2>)GetFunc(name, typeof(T1), typeof(T2));
		}

		public Action<T1, T2, T3> GetAction<T1, T2, T3>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
			return ScriptUtils.ConvertDelegate<Action<T1, T2, T3>>(del);
			//if (del == null) return null;
			//if (del is Action<T1, T2, T3> act) return act;
			//return (a1, a2, a3) => del.DynamicInvoke(a1, a2, a3);
			//return (Action<T1, T2, T3>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
		}

		public Action<T1, T2, T3, T4> GetAction<T1, T2, T3, T4>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
			return ScriptUtils.ConvertDelegate<Action<T1, T2, T3, T4>>(del);
			//if (del == null) return null;
			//if (del is Action<T1, T2, T3, T4> act) return act;
			//return (a1, a2, a3, a4) => del.DynamicInvoke(a1, a2, a3, a4);
			//return (Action<T1, T2, T3, T4>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public Action<T1, T2, T3, T4, T5> GetAction<T1, T2, T3, T4, T5>(string name)
		{
			var del = GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
			return ScriptUtils.ConvertDelegate<Action<T1, T2, T3, T4, T5>>(del);
			//if (del == null) return null;
			//if (del is Action<T1, T2, T3, T4, T5> act) return act;
			//return (a1, a2, a3, a4, a5) => del.DynamicInvoke(a1, a2, a3, a4, a5);
			//return (Action<T1, T2, T3, T4, T5>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
		}

		public override void SetVar(string name, object value, Type valueType)
		{
			base.SetVar(name, value, valueType);
			if (this._TempVariables != null && this._TempVariables.ContainsKey(name))
			{
				// 覆盖临时变量值
				this._TempVariables[name] = value;
				SetTempVarType(name, value, valueType);
			}
		}

		private void SetTempVarType(string name, object value, Type type)
		{
			if (value != null && type == value.GetType())
			{
				type = null;
			}
			if (type == null)
			{
				this._TempVariableTypes?.Remove(name);
			}
			else
			{
				Init_TempVariableTypes();
				this._TempVariableTypes[name] = type;
			}
		}

		public override void RemoveVar(string name)
		{
			base.RemoveVar(name);
			this._TempVariables?.Remove(name);
			this._TempVariableTypes?.Remove(name);
		}

		public void RemoveTempVar(string name)
		{
			this._TempVariables?.Remove(name);
			this._TempVariableTypes?.Remove(name);
		}

		public void SetTempVar(string name, object value, bool searchContext)
		{
			SetTempVar(name, value, null, searchContext);
		}

		public void SetTempVar<T>(string name, T value, bool searchContext)
		{
			SetTempVar(name, value, typeof(T), searchContext);
		}

		public void SetTempVar(string name, object value, Type valueType, bool searchContext)
		{
			var context = searchContext ? (GetOwnerContext(name, out _, out _) ?? this) : this;
			Modifiers.ThrowIfReadOnly(name, context.GetVarModifier(name));
			context.Init_TempVariables();
			context._TempVariables[name] = value;
			context.SetTempVarType(name, value, valueType);
			//context.Init_TempVariableTypes();
			//context._TempVariableTypes[name] = valueType ?? value?.GetType() ?? typeof(object);
		}

		public void SetTempConst(string name, object value, Type valueType, bool searchContext)
		{
			var context = searchContext ? (GetOwnerContext(name, out _, out _) ?? this) : this;
			context.Init_TempVariables();
			context._TempVariables[name] = value;
			context.SetTempVarType(name, value, valueType);
			//context.Init_TempVariableTypes();
			//context._TempVariableTypes[name] = valueType ?? value?.GetType() ?? typeof(object);
			context.SetVarModifier(name, Modifiers.READONLY);
		}

		public void SetTempConst(string name, object value, bool searchContext)
		{
			SetTempConst(name, value, null, searchContext);
		}

		public void SetTempConst<T>(string name, T value, bool searchContext)
		{
			SetTempConst(name, value, typeof(T), searchContext);
		}

		/// <summary>
		/// 获取变量的声明类型
		/// </summary>
		/// <param name="name">变量名</param>
		/// <returns>变量的声明类型，如果不存在则返回 null</returns>
		public Type GetVarType(string name)
		{
			//// 先检查临时变量
			//if (_TempVariableTypes != null && _TempVariableTypes.TryGetValue(name, out var type))
			//{
			//	return type;
			//}
			//// 检查普通变量
			//if (_VariableTypes != null && _VariableTypes.TryGetValue(name, out type))
			//{
			//	return type;
			//}
			//// 检查父上下文
			//return Parent?.GetVarType(name);

			var context = GetOwnerContext(name, out _, out var type, true);
			if (context == null)
			{
				// 从语言上下文中搜索
				EvalVarFromLangs(name, out type);
			}
			return type;
		}

		public void HandleToken(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			if (e.IsHandled) return;

			var context = this;
			while (context != null)
			{
				var tokenHandlerDict = context._TokenHandlerDict;
				if (tokenHandlerDict != null && tokenHandlerDict.TryGetValue(e.CurrentToken.Value, out var handler))
				{
					handler.Build(analyzer, e);
					if (e.IsHandled) return;
				}
				var tokenHandlers = context._TokenHandlers;
				if (tokenHandlers != null)
				{
					for (int i = 0; i < tokenHandlers.Count; i++)
					{
						tokenHandlers[i].Build(analyzer, e);
						if (e.IsHandled) return;
					}
				}
				context = context.Parent;
			}

			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				// 所有可兼容脚本语言
				foreach (var langName in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(langName, out var lang))
					{
						lang.HandleToken(analyzer, e);
						if (e.IsHandled) return;
					}
				}
			}
			else
			{
				// 指定脚本语言
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						lang.HandleToken(analyzer, e);
						if (e.IsHandled) return;
					}
				}
			}
		}

		public async Task HandleTokenAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken = default)
		{
			if (e.IsHandled) return;

			var context = this;
			while (context != null)
			{
				var tokenHandlerDict = context._TokenHandlerDict;
				if (tokenHandlerDict != null && tokenHandlerDict.TryGetValue(e.CurrentToken.Value, out var handler))
				{
					if (handler is IAsyncTokenHandler asyncTokenHandler)
					{
						await asyncTokenHandler.BuildAsync(analyzer, e, cancellationToken).ConfigureAwait(false);
					}
					else
					{
						handler.Build(analyzer, e);
					}
					if (e.IsHandled) return;
				}
				var tokenHandlers = context._TokenHandlers;
				if (tokenHandlers != null)
				{
					for (int i = 0; i < tokenHandlers.Count; i++)
					{
						var handler2 = tokenHandlers[i];
						if (handler2 is IAsyncTokenHandler asyncTokenHandler)
						{
							await asyncTokenHandler.BuildAsync(analyzer, e, cancellationToken).ConfigureAwait(false);
						}
						else
						{
							handler2.Build(analyzer, e);
						}
						if (e.IsHandled) return;
					}
				}
				context = context.Parent;
			}

			var langs = this.Langs;
			if (langs == null || langs.Length == 0)
			{
				// 所有可兼容脚本语言
				foreach (var langName in Script.Langs.GetDefaults())
				{
					if (Script.Langs.TryGetValue(langName, out var lang))
					{
						await lang.HandleTokenAsync(analyzer, e, cancellationToken).ConfigureAwait(false);
						if (e.IsHandled) return;
					}
				}
			}
			else
			{
				// 指定脚本语言
				for (int i = 0; i < langs.Length; i++)
				{
					if (Script.Langs.TryGetValue(langs[i], out var lang))
					{
						await lang.HandleTokenAsync(analyzer, e, cancellationToken).ConfigureAwait(false);
						if (e.IsHandled) return;
					}
				}
			}
		}

		public void AddTempFunc(string name, Delegate d)
		{
			Init_TempFunctions();
			if (!_TempFunctions.TryGetValue(name, out var list))
			{
				if (_TempFunctions is ConcurrentDictionary<string, List<Delegate>> con)
				{
					list = con.GetOrAdd(name, key => new List<Delegate>());
				}
				else
				{
					_TempFunctions[name] = list = new List<Delegate>();
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

		///// <summary>
		///// 如果target为null，则添加类型中的公开静态方法，否则添加实例公开方法
		///// </summary>
		///// <param name="type"></param>
		///// <param name="target">实例对象</param>
		//public void AddTempFunc(Type type, object target = null)
		//{
		//	var methods = target == null ? type.GetMethods(BindingFlags.Public | BindingFlags.Static) : type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
		//	foreach (var method in methods)
		//	{
		//		// 跳过属性访问器等特殊方法
		//		if (method.IsSpecialName) continue;
		//		// 
		//		var del = ScriptUtils.CreateDelegate(method, target);
		//		if (del != null)
		//		{
		//			AddTempFunc(method.Name, del);
		//		}
		//	}
		//}

		//public void AddTempFunc(MethodInfo method, object target = null)
		//{
		//	var del = ScriptUtils.CreateDelegate(method, target);
		//	if (del != null)
		//	{
		//		AddTempFunc(method.Name, del);
		//	}
		//}

		//public void AddTempFunc(string name, MethodInfo method, object target = null)
		//{
		//	var del = ScriptUtils.CreateDelegate(method, target);
		//	if (del != null)
		//	{
		//		AddTempFunc(string.IsNullOrEmpty(name) ? method.Name : name, del);
		//	}
		//}

		public void AddFunc(string name, CustomFunction customFunction)
		{
			if (string.IsNullOrEmpty(name) || name == "_") return;
			Init_CustomFunctions();
			if (!_CustomFunctions.TryGetValue(name, out var list))
			{
				if (_CustomFunctions is ConcurrentDictionary<string, List<CustomFunction>> con)
				{
					list = con.GetOrAdd(name, key => new List<CustomFunction>());
				}
				else
				{
					_CustomFunctions[name] = list = new List<CustomFunction>();
				}
			}
			if (_ThreadSafely)
			{
				lock (this)
				{
					list.Add(customFunction);
				}
			}
			else
			{
				list.Add(customFunction);
			}
		}

	}
}
