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

		public virtual bool IsTrue(object obj)
		{
			if (obj is bool b) return b;
			throw new Exceptions.ScriptRuntimeException($"invalid object of type '{obj?.GetType()}', expect bool type");
		}

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
