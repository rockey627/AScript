using AScript.Functions;
using AScript.Readers;
using AScript.Syntaxs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AScript
{
	public class ScriptLang : BaseContext
	{
		public ConcurrentDictionary<Type, bool> ObjectMemberEnabledDict { get; private set; } = new ConcurrentDictionary<Type, bool>();

		/// <summary>
		/// 
		/// </summary>
		public ScriptLang() : base(true) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreCase">关键字是否忽略大小写</param>
		public ScriptLang(bool ignoreCase) : base(true, ignoreCase) { }

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
		public virtual ISyntaxAnalyzer GetSyntaxAnalyzer()
		{
			return null;
		}

		/// <summary>
		/// 自定义操作符
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		public virtual int? GetOperatorPriority(string op)
		{
			return null;
		}

		/// <summary>
		/// 动态语言特性
		/// </summary>
		/// <returns>false表示静态语言；true表示动态语言</returns>
		public virtual bool IsDynamic()
		{
			return false;
		}

		public virtual bool IsKeywords(string word)
		{
			var dict = _TokenHandlerDict;
			return dict != null && dict.ContainsKey(word);
		}

		/// <summary>
		/// 对象内部成员（构造函数、属性、字段、方法）是否可用
		/// </summary>
		/// <returns></returns>
		public virtual bool IsObjectMemberEnabled(Type objType)
		{
			if (this.ObjectMemberEnabledDict.TryGetValue(objType, out var enable))
			{
				return enable;
			}
			return true;
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

		//protected static Delegate GetFunc(List<Delegate> list, IList<Type> argTypes, out bool useScriptContext)
		//{
		//	for (int i = list.Count - 1; i >= 0; i--)
		//	{
		//		var d = list[i];
		//		if (ScriptUtils.IsMatchArgTypes(argTypes, d.Method, out useScriptContext, out _))
		//		{
		//			return d;
		//		}
		//		//var defineArgTypes = d.Method.GetParameters()
		//		//	.Where(a => a.ParameterType.FullName != "System.Runtime.CompilerServices.Closure")
		//		//	.Select(a => a.ParameterType).ToArray();
		//		//if (ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes))
		//		//{
		//		//	useScriptContext = false;
		//		//	return d;
		//		//}
		//		//if (defineArgTypes.Length > 0
		//		//	&& argTypes.Count == defineArgTypes.Length - 1
		//		//	&& ScriptUtils.IsMatchArgType(defineArgTypes[0], typeof(ScriptContext))
		//		//	&& ScriptUtils.IsMatchArgTypes(argTypes, defineArgTypes, 1))
		//		//{
		//		//	// ScriptContext开头的参数匹配
		//		//	useScriptContext = true;
		//		//	return d;
		//		//}
		//	}
		//	useScriptContext = false;
		//	return null;
		//}

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
			ScriptContext.EvalFunc(e, _Functions);
			if (e.IsHandled) return;

			ScriptContext.EvalFunc(e, _FunctionEvaluators);
			if (e.IsHandled) return;

			OnFunctionEval(e);
		}

		public async Task EvalFuncAsync(FunctionEvalArgs e, CancellationToken cancellationToken = default)
		{
			await ScriptContext.EvalFuncAsync(e, _Functions, cancellationToken).ConfigureAwait(false);
			if (e.IsHandled) return;

			await ScriptContext.EvalFuncAsync(e, _FunctionEvaluators, cancellationToken).ConfigureAwait(false);
			if (e.IsHandled) return;

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

			ScriptContext.BuildFunc(e, _FunctionEvaluators);
			if (e.Result != null) return;

			OnFunctionBuild(e);
		}

		public void AddType(string name, Type type, bool memberEnabled)
		{
			AddType(name, type);
			this.ObjectMemberEnabledDict[type] = memberEnabled;
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

		public async Task HandleTokenAsync(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e, CancellationToken cancellationToken = default)
		{
			if (e.IsHandled) return;
			if (_TokenHandlerDict != null && _TokenHandlerDict.TryGetValue(e.CurrentToken.Value, out var handler))
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
			if (_TokenHandlers != null)
			{
				for (int i = 0; i < _TokenHandlers.Count; i++)
				{
					var handler2 = _TokenHandlers[i];
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
		}
	}
}
