using AScript.Nodes;
using AScript.Readers;
using AScript.Syntaxs;
using AScript.TokenHandlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AScript
{
	/// <summary>
	/// 上下文
	/// </summary>
	public class ScriptContext
	{
		public static readonly ScriptContext Root = new ScriptContext(null, true);

		static ScriptContext()
		{
			Root.AddTokenHandler("#lang", LangTokenHandler.Instance);
		}

		/// <summary>
		/// 上级
		/// </summary>
		public ScriptContext Parent { get; set; }

		/// <summary>
		/// 函数运算事件
		/// </summary>
		public event EventHandler<FunctionEvalArgs> FunctionEval;
		/// <summary>
		/// 函数编译事件
		/// </summary>
		public event EventHandler<FunctionBuildArgs> FunctionBuild;

		private bool _ThreadSafely;

		// 函数运算
		private IDictionary<string, IList<IFunctionEvaluator>> _FunctionEvaluators;
		// 语句处理
		private IDictionary<string, ITokenHandler> _TokenHandlerDict;

		private IList<ITokenHandler> _TokenHandlers;

		/// <summary>
		/// 程序集
		/// </summary>
		private IDictionary<string, Assembly> _Assemblies;
		/// <summary>
		/// 类型定义
		/// </summary>
		private IDictionary<string, Type> _Types;
		/// <summary>
		/// 全局变量
		/// </summary>
		private IDictionary<string, object> _Variables;
		/// <summary>
		/// 全局变量类型
		/// </summary>
		private IDictionary<string, Type> _VariableTypes;

		/// <summary>
		/// 表达式中的临时变量
		/// </summary>
		private IDictionary<string, object> _TempVariables;
		/// <summary>
		/// 临时变量类型
		/// </summary>
		private IDictionary<string, Type> _TempVariableTypes;

		// 支持函数重载
		private IDictionary<string, List<Delegate>> _Functions;
		// 临时函数
		private IDictionary<string, List<Delegate>> _TempFunctions;
		// 临时函数
		private IDictionary<string, List<CustomFunction>> _CustomFunctions;

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
		public ScriptContext(bool threadSafely) : this(Root, threadSafely)
		{
			this._ThreadSafely = threadSafely;
		}
		public ScriptContext(ScriptContext parent) : this(parent, false) { }
		public ScriptContext(ScriptContext parent, bool threadSafely)
		{
			this.Parent = parent;
			_ThreadSafely = threadSafely;
		}

		public static ScriptContext Create(bool threadSafely = false)
		{
			return new ScriptContext(Root, threadSafely);
		}

		public static ScriptContext Create(ScriptContext parent, bool threadSafely = false)
		{
			return new ScriptContext(parent, threadSafely);
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
							_FunctionEvaluators = new ConcurrentDictionary<string, IList<IFunctionEvaluator>>();
						}
					}
				}
				else
				{
					_FunctionEvaluators = new Dictionary<string, IList<IFunctionEvaluator>>();
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
							_TokenHandlerDict = new ConcurrentDictionary<string, ITokenHandler>();
						}
					}
				}
				else
				{
					_TokenHandlerDict = new Dictionary<string, ITokenHandler>();
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
							_Assemblies = new ConcurrentDictionary<string, Assembly>();
						}
					}
				}
				else
				{
					_Assemblies = new Dictionary<string, Assembly>();
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
							_Types = new ConcurrentDictionary<string, Type>();
						}
					}
				}
				else
				{
					_Types = new Dictionary<string, Type>();
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
							_Variables = new ConcurrentDictionary<string, object>();
						}
					}
				}
				else
				{
					_Variables = new Dictionary<string, object>();
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
							_VariableTypes = new ConcurrentDictionary<string, Type>();
						}
					}
				}
				else
				{
					_VariableTypes = new Dictionary<string, Type>();
				}
			}
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
							_Functions = new ConcurrentDictionary<string, List<Delegate>>();
						}
					}
				}
				else
				{
					_Functions = new Dictionary<string, List<Delegate>>();
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

		public virtual ITokenStream GetTokenStream(CharReader charReader)
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

		public virtual ISyntaxAnalyzer GetSyntaxAnalyzer()
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

		public virtual int? GetOperatorPriority(string op)
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
		/// 清空所有数据
		/// </summary>
		public virtual void Clear()
		{
			ClearTemp();
			this._Assemblies?.Clear();
			this._Types?.Clear();
			this._Variables?.Clear();
			this._VariableTypes?.Clear();
			this._Functions?.Clear();
		}

		/// <summary>
		/// 清空临时数据（临时变量、临时函数）
		/// </summary>
		public virtual void ClearTemp()
		{
			ClearTempVariable();
			ClearTempFunction();
		}

		/// <summary>
		/// 清空临时变量
		/// </summary>
		public virtual void ClearTempVariable()
		{
			this._TempVariables?.Clear();
			this._TempVariableTypes?.Clear();
		}

		/// <summary>
		/// 清空临时函数
		/// </summary>
		public virtual void ClearTempFunction()
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
		public virtual ScriptContext GetOwnerContext(string variable, out object value, out Type type, bool searchType = false)
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
					return context;
				}
				var types = context._Types;
				if (searchType && types != null && types.TryGetValue(variable, out var c))
				{
					type = typeof(TypeWrapper);
					value = new TypeWrapper(c);
					return context;
				}
				context = context.Parent;
			} while (context != null);

			if (!searchType)
			{
				value = null;
				type = null;
				return null;
			}
			var tt = EvalTypeFromLangs(variable);
			if (tt == null)
			{
				value = null;
				type = null;
				return null;
			}
			type = typeof(TypeWrapper);
			value = new TypeWrapper(tt);
			return null;
		}

		public object EvalVar(string name)
		{
			return EvalVar(name, out _);
		}

		public object EvalVar(string name, out Type type)
		{
			var context = GetOwnerContext(name, out var value, out type, true);
			//if (context == null)
			//{
			//	// 没有变量，则查找类
			//	var mytype = EvalType(name);
			//	if (mytype != null)
			//	{
			//		type = typeof(TypeWrapper);
			//		return new TypeWrapper(mytype);
			//	}
			//}
			if (type == null) return value;
			if (value == null && type.IsValueType)
			{
				// 值类型的变量未赋值，则取该变量时初始化
				value = ScriptUtils.GetDefaultValue(type);
				context._TempVariables[name] = value;
			}
			return value;
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

		public virtual object EvalFunc(BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args, out Type returnType)
		{
			var functionEvalArgs = FunctionEvalArgs.Create(this, options, control, name, isPrefix, args);
			var context = this;
			object[] datas = null;
			Type[] types = null;
			try
			{
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
					if (EvalFunc(options, control, context._CustomFunctions, name, isPrefix, args, ref datas, ref types, out var result, out returnType))
					{
						return result;
					}
					// 临时函数
					if (EvalFunc(options, control, context._TempFunctions, name, isPrefix, args, ref datas, ref types, out result, out returnType))
					{
						return result;
					}
					// 全局函数
					if (EvalFunc(options, control, context._Functions, name, isPrefix, args, ref datas, ref types, out result, out returnType))
					{
						return result;
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
			finally
			{
				FunctionEvalArgs.Return(functionEvalArgs);
			}
			// 抛出未知函数异常
			if (types == null)
			{
				if (args == null || args.Count == 0)
				{
#if NETFRAMEWORK
					types = new Type[0];
#else
					types = Array.Empty<Type>();
#endif
				}
				else
				{
					types = new Type[args.Count];
					for (int i = 0; i < args.Count; i++)
					{
						args[i].Eval(this, options, control, out var type);
						types[i] = type;
					}
				}
			}
			//// 判断前置/后置运算符或者函数调用
			//string funcName = isPrefix || args.Length > 1 || !DefaultAnalyzer.OperatorPriorities.ContainsKey(name) ?
			//	$"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})" :
			//	$"({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))}){name}";
			string funcName = $"{name}({string.Join(",", types.Select(a => (a ?? typeof(object)).FullName))})";
			throw new Exception($"unknown function: {funcName}");
		}

		protected bool EvalFunc(BuildOptions options, EvalControl control, IDictionary<string, List<CustomFunction>> functions, string name, bool isPrefix, IList<ITreeNode> args, ref object[] datas, ref Type[] types, out object result, out Type returnType)
		{
			if (functions == null || !functions.TryGetValue(name, out var list1))
			{
				result = null;
				returnType = null;
				return false;
			}

			if (types == null && args != null && args.Count > 0)
			{
				types = new Type[args.Count];
				datas = new object[args.Count];
				for (int i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					var value = arg.Eval(this, options, control, out var type);
					datas[i] = value;
					types[i] = type;
					if (!(arg is ObjectNode))
					{
						PoolManage.Return(arg);
						args[i] = PoolManage.CreateObjectData(value, type);
					}
				}
			}

			var d = GetFunc(list1, types);
			if (d == null)
			{
				result = null;
				returnType = null;
				return false;
			}

			result = d.Eval(this, options, control, args, out returnType);
			return true;
		}

		protected bool EvalFunc(BuildOptions options, EvalControl control, IDictionary<string, List<Delegate>> functions, string name, bool isPrefix, IList<ITreeNode> args, ref object[] argValues, ref Type[] argTypes, out object result, out Type returnType)
		{
			if (functions == null || !functions.TryGetValue(name, out var list3))
			{
				result = null;
				returnType = null;
				return false;
			}

			if (argTypes == null && args != null && args.Count > 0)
			{
				argTypes = new Type[args.Count];
				argValues = new object[args.Count];
				for (int i = 0; i < args.Count; i++)
				{
					var arg = args[i];
					var value = arg.Eval(this, options, control, out var type);
					argValues[i] = value;
					argTypes[i] = type;
					if (!(arg is ObjectNode))
					{
						PoolManage.Return(arg);
						args[i] = PoolManage.CreateObjectData(value, type);
					}
				}
			}

			var d = GetFunc(list3, argTypes, out var useScriptContext, out _);
			if (d == null)
			{
				result = null;
				returnType = null;
				return false;
			}

			returnType = d.Method.ReturnType ?? typeof(object);
			if (useScriptContext)
			{
				var datas2 = new object[(argValues?.Length ?? 0) + 1];
				datas2[0] = this;
				//datas2[0] = Create(this);
				if (argValues != null && argValues.Length > 0)
				{
					Array.Copy(argValues, 0, datas2, 1, argValues.Length);
				}
				argValues = datas2;
			}
			result = d.DynamicInvoke(argValues);
			return true;
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
		public virtual Expression BuildFunc(BuildContext buildContext, BuildOptions options, EvalControl control, string name, bool isPrefix, IList<ITreeNode> args)
		{
			Expression[] argExprs = null;
			Type[] argTypes = null;

			// 从编译上下文环境中构建
			var tempBuildContext = buildContext;
			while (tempBuildContext != null)
			{
				var result = BuildFunc(tempBuildContext, options, tempBuildContext.TempFunctions, name, args, ref argExprs, ref argTypes);
				if (result != null) return result;
				tempBuildContext = tempBuildContext.Parent;
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

			//throw new Exception("unkown function for build:" + name);
			// 构建context.EvalFunc方法调用
			return ExpressionUtils.BuildEval(buildContext, this, options, name, args);
			//string funcName = argTypes == null || argTypes.Length == 0 ? 
			//	$"{name}()" : 
			//	$"{name}({string.Join(",", argTypes.Select(a => (a ?? typeof(object)).FullName))})";
			//throw new Exception($"unknown function: {funcName}");
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
		public virtual Expression BuildFunc(BuildContext buildContext, BuildOptions options, EvalControl control, string name, bool isPrefix, Expression[] args, bool buildEvalEnabled = true)
		{
			Type[] argTypes = null;

			// 从编译上下文环境中构建
			var tmpBuildContext = buildContext;
			while (tmpBuildContext != null)
			{
				var result = BuildFunc(tmpBuildContext, options, tmpBuildContext.TempFunctions, name, null, ref args, ref argTypes);
				if (result != null) return result;
				tmpBuildContext = tmpBuildContext.Parent;
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
					var result = BuildFunc(buildContext, options, context._TempFunctions, name, null, ref args, ref argTypes);
					if (result != null) return result;
					// 全局函数
					result = BuildFunc(buildContext, options, context._Functions, name, null, ref args, ref argTypes);
					if (result != null) return result;
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

			if (!buildEvalEnabled) return null;

			//throw new Exception("unkown function for build:" + name);
			// 构建context.EvalFunc方法调用
			return ExpressionUtils.BuildEval(buildContext, this, options, name, args);
		}

		public Expression BuildFunc(BuildContext buildContext, BuildOptions options, IDictionary<string, List<Delegate>> functions, string name, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
		{
			if (functions == null || !functions.TryGetValue(name, out var list3)) return null;

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
						PoolManage.Return(arg);
						args[i] = PoolManage.CreateExpressionNode(expr);
					}
				}
			}
			else if (argExprs != null && argExprs.Length > 0)
			{
				argTypes = new Type[argExprs.Length];
				for (int i = 0; i < argExprs.Length; i++)
				{
					argTypes[i] = argExprs[i].Type;
				}
			}

			var d = GetFunc(list3, argTypes, out var useScriptContext, out var hasClosure);
			return BuildFunc(d, argExprs, useScriptContext, hasClosure);
		}

		public Expression BuildFunc(BuildContext buildContext, BuildOptions options, IList<Delegate> functions, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
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
						PoolManage.Return(arg);
						args[i] = PoolManage.CreateExpressionNode(expr);
					}
				}
			}
			else if (argExprs != null && argExprs.Length > 0)
			{
				argTypes = new Type[argExprs.Length];
				for (int i = 0; i < argExprs.Length; i++)
				{
					argTypes[i] = argExprs[i].Type;
				}
			}

			var d = GetFunc(functions, argTypes, out var useScriptContext, out var hasClosure);
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
					if (arg.Type != p.ParameterType)
					{
						argExprs[i] = Expression.Convert(arg, p.ParameterType);
					}
				}
			}
			return Expression.Invoke(Expression.Constant(d), argExprs);
		}

		protected Expression BuildFunc(BuildContext buildContext, BuildOptions options, IDictionary<string, List<LambdaExpression>> functions, string name, IList<ITreeNode> args, ref Expression[] argExprs, ref Type[] argTypes)
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
					argTypes[i] = argExprs[i].Type;
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
				var parameters = d.Parameters;
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
			return Expression.Invoke(d, argExprs);
		}

		//public virtual Expression BuildEval(string name, Type[] argTypes)
		//{
		//	throw new NotImplementedException();
		//}

		public object EvalFunc(string name, IList<object> argValues, IList<Type> argTypes)
		{
			return EvalFunc(name, false, argValues, argTypes, out _);
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
			var argCount = argValues == null ? 0 : argValues.Count;
			var args = new ITreeNode[argCount];
			for (int i = 0; i < argCount; i++)
			{
				args[i] = PoolManage.CreateObjectData(argValues[i], argTypes[i]);
			}
			var result = EvalFunc(null, null, name, isPrefix, args, out returnType);
			PoolManage.Return(args);
			return result;
		}

		public virtual Type EvalType(string name)
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

		private static Delegate GetFunc(IList<Delegate> list, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure)
		{
			int argTypesCount = argTypes == null ? 0 : argTypes.Count;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				var methodParameters = d.Method.GetParameters();
				//var defineArgTypes = methodParameters
				//	.Where(a => a.ParameterType.FullName != "System.Runtime.CompilerServices.Closure")
				//	.Select(a => a.ParameterType).ToArray();
				//hasClosure = defineArgTypes.Length < methodParameters.Length;
				//if (ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes))
				//{
				//	useScriptContext = false;
				//	return d;
				//}
				//if (defineArgTypes.Length > 0
				//	&& (argTypes == null ? 0 : argTypes.Count) == defineArgTypes.Length - 1
				//	&& ScriptUtils.IsMatchArgType(defineArgTypes[0], typeof(ScriptContext))
				//	&& ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes, 1))
				//{
				//	// ScriptContext开头的参数匹配
				//	useScriptContext = true;
				//	return d;
				//}
				if (methodParameters.Length < argTypesCount) continue;
				if (methodParameters.Length == argTypesCount)
				{
					if (argTypesCount == 0)
					{
						useScriptContext = false;
						hasClosure = false;
						return d;
					}
				}
				int index = 0;
				hasClosure = false;
				useScriptContext = false;
				if (methodParameters[index].ParameterType.FullName == "System.Runtime.CompilerServices.Closure")
				{
					index++;
					hasClosure = true;
				}
				if (methodParameters.Length > index && methodParameters[index].ParameterType == typeof(ScriptContext))
				{
					index++;
					useScriptContext = true;
				}
				if (methodParameters.Length - argTypesCount > index)
				{
					continue;
				}
				bool matched = true;
				for (int j = 0; j < argTypesCount; j++)
				{
					if (!ScriptUtils.IsMatchArgType(argTypes[j], methodParameters[j + index].ParameterType))
					{
						matched = false;
						break;
					}
				}
				if (matched)
				{
					return d;
				}
			}
			hasClosure = false;
			useScriptContext = false;
			return null;
		}

		private static LambdaExpression GetFunc(List<LambdaExpression> list, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure)
		{
			int argTypesCount = argTypes == null ? 0 : argTypes.Count;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var d = list[i];
				var methodParameters = d.Parameters;
				//var defineArgTypes = methodParameters
				//	.Where(a => a.Type.FullName != "System.Runtime.CompilerServices.Closure")
				//	.Select(a => a.Type).ToArray();
				//hasClosure = defineArgTypes.Length < methodParameters.Count;
				//if (ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes))
				//{
				//	useScriptContext = false;
				//	return d;
				//}
				//if (defineArgTypes.Length > 0
				//	&& argTypesCount == defineArgTypes.Length - 1
				//	&& ScriptUtils.IsMatchArgType(defineArgTypes[0], typeof(ScriptContext))
				//	&& ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes, 1))
				//{
				//	// ScriptContext开头的参数匹配
				//	useScriptContext = true;
				//	return d;
				//}
				if (methodParameters.Count < argTypesCount) continue;
				if (methodParameters.Count == argTypesCount)
				{
					if (argTypesCount == 0)
					{
						useScriptContext = false;
						hasClosure = false;
						return d;
					}
				}
				int index = 0;
				hasClosure = false;
				useScriptContext = false;
				if (methodParameters[index].Type.FullName == "System.Runtime.CompilerServices.Closure")
				{
					index++;
					hasClosure = true;
				}
				if (methodParameters[index].Type == typeof(ScriptContext))
				{
					index++;
					useScriptContext = true;
				}
				if (methodParameters.Count - argTypesCount > index)
				{
					continue;
				}
				bool matched = true;
				for (int j = 0; j < argTypesCount; j++)
				{
					if (!ScriptUtils.IsMatchArgType(argTypes[j], methodParameters[j + index].Type))
					{
						matched = false;
						break;
					}
				}
				if (matched)
				{
					return d;
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
			return GetFunc(name, argTypes, out _, out _);
		}

		public Delegate GetFunc(string name, IList<Type> argTypes, out bool useScriptContext, out bool hasClosure)
		{
			var context = this;
			while (context != null)
			{
				var tempFunctions = context._TempFunctions;
				if (tempFunctions != null && tempFunctions.TryGetValue(name, out var list1))
				{
					var func = GetFunc(list1, argTypes, out useScriptContext, out hasClosure);
					if (func != null) return func;
				}
				var functions = context._Functions;
				if (functions != null && functions.TryGetValue(name, out var list2))
				{
					var func = GetFunc(list2, argTypes, out useScriptContext, out hasClosure);
					if (func != null) return func;
				}
				context = context.Parent;
			}
			hasClosure = false;
			useScriptContext = false;
			return null;
		}

		public Func<TReturn> GetFunc<TReturn>(string name)
		{
			return (Func<TReturn>)GetFunc(name);
		}

		public Func<T1, TReturn> GetFunc<T1, TReturn>(string name)
		{
			return (Func<T1, TReturn>)GetFunc(name, typeof(T1));
		}

		public Func<T1, T2, TReturn> GetFunc<T1, T2, TReturn>(string name)
		{
			return (Func<T1, T2, TReturn>)GetFunc(name, typeof(T1), typeof(T2));
		}

		public Func<T1, T2, T3, TReturn> GetFunc<T1, T2, T3, TReturn>(string name)
		{
			return (Func<T1, T2, T3, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
		}

		public Func<T1, T2, T3, T4, TReturn> GetFunc<T1, T2, T3, T4, TReturn>(string name)
		{
			return (Func<T1, T2, T3, T4, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public Func<T1, T2, T3, T4, T5, TReturn> GetFunc<T1, T2, T3, T4, T5, TReturn>(string name)
		{
			return (Func<T1, T2, T3, T4, T5, TReturn>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
		}

		public Action GetAction(string name)
		{
			return (Action)GetFunc(name);
		}

		public Action<T1> GetAction<T1>(string name)
		{
			return (Action<T1>)GetFunc(name, typeof(T1));
		}

		public Action<T1, T2> GetAction<T1, T2>(string name)
		{
			return (Action<T1, T2>)GetFunc(name, typeof(T1), typeof(T2));
		}

		public Action<T1, T2, T3> GetAction<T1, T2, T3>(string name)
		{
			return (Action<T1, T2, T3>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3));
		}

		public Action<T1, T2, T3, T4> GetAction<T1, T2, T3, T4>(string name)
		{
			return (Action<T1, T2, T3, T4>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public Action<T1, T2, T3, T4, T5> GetAction<T1, T2, T3, T4, T5>(string name)
		{
			return (Action<T1, T2, T3, T4, T5>)GetFunc(name, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
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
			this._Types?.Remove(name);
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
		public void SetVar(string name, object value, Type valueType)
		{
			if (valueType == null)
			{
				valueType = value?.GetType() ?? typeof(object);
			}
			Init_Variables();
			Init_VariableTypes();
			this._Variables[name] = value;
			this._VariableTypes[name] = valueType;
			if (this._TempVariables != null && this._TempVariables.ContainsKey(name))
			{
				// 覆盖临时变量值
				this._TempVariables[name] = value;
				Init_TempVariableTypes();
				this._TempVariableTypes[name] = valueType;
			}
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

		public void RemoveVar(string name)
		{
			this._Variables?.Remove(name);
			this._VariableTypes?.Remove(name);
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
			Init_TempVariables();
			Init_TempVariableTypes();
			context._TempVariables[name] = value;
			context._TempVariableTypes[name] = valueType ?? value?.GetType() ?? typeof(object);
		}

		/// <summary>
		/// 获取变量的声明类型
		/// </summary>
		/// <param name="name">变量名</param>
		/// <returns>变量的声明类型，如果不存在则返回 null</returns>
		public Type GetVarType(string name)
		{
			// 先检查临时变量
			if (_TempVariableTypes != null && _TempVariableTypes.TryGetValue(name, out var type))
			{
				return type;
			}
			// 检查普通变量
			if (_VariableTypes != null && _VariableTypes.TryGetValue(name, out type))
			{
				return type;
			}
			// 检查父上下文
			return Parent?.GetVarType(name);
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

		/// <summary>
		/// 如果target为null，则添加类型中的公开静态方法，否则添加实例公开方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">实例对象</param>
		public void AddTempFunc(Type type, object target = null)
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
					AddTempFunc(method.Name, del);
				}
			}
		}

		public void AddTempFunc(MethodInfo method, object target = null)
		{
			var del = ScriptUtils.CreateDelegate(method, target);
			if (del != null)
			{
				AddTempFunc(method.Name, del);
			}
		}

		public void AddTempFunc(string name, MethodInfo method, object target = null)
		{
			var del = ScriptUtils.CreateDelegate(method, target);
			if (del != null)
			{
				AddTempFunc(string.IsNullOrEmpty(name) ? method.Name : name, del);
			}
		}

		public void AddFunc(CustomFunction customFunction)
		{
			Init_CustomFunctions();
			if (!_CustomFunctions.TryGetValue(customFunction.Name, out var list))
			{
				if (_CustomFunctions is ConcurrentDictionary<string, List<CustomFunction>> con)
				{
					list = con.GetOrAdd(customFunction.Name, key => new List<CustomFunction>());
				}
				else
				{
					_CustomFunctions[customFunction.Name] = list = new List<CustomFunction>();
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
				// 创建方法委托
				var del = ScriptUtils.CreateDelegate(method, target);
				if (del != null) AddFunc(name, del);
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
			var del = ScriptUtils.CreateDelegate(method, target);
			if (del != null)
			{
				AddFunc(method.Name, del);
			}
		}

		/// <summary>
		/// 添加方法
		/// </summary>
		/// <param name="name"></param>
		/// <param name="method"></param>
		/// <param name="target"></param>
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

			var functionEvaluators = _FunctionEvaluators;
			if (functionEvaluators != null && functionEvaluators.TryGetValue(e.Name, out var list))
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

			var functionEvaluators = _FunctionEvaluators;
			if (functionEvaluators != null && functionEvaluators.TryGetValue(e.Name, out var list))
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
	}
}
