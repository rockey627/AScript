using AScript.Nodes;
using AScript.Readers;
using System;
using System.Collections.Concurrent;

namespace AScript
{
	public class TokenAnalyzingArgs : EventArgs
	{
		private static readonly ConcurrentQueue<TokenAnalyzingArgs> _pool = new ConcurrentQueue<TokenAnalyzingArgs>();

		/// <summary>
		/// 
		/// </summary>
		public BuildContext BuildContext { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public ScriptContext ScriptContext { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public TokenReader TokenReader { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public TreeBuilder TreeBuilder { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public EvalControl Control { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public BuildOptions Options { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public Token CurrentToken { get; internal set; }

		public bool IsHandled { get; set; }

		/// <summary>
		/// 是否忽略
		/// </summary>
		public bool Ignore { get; internal set; }

		/// <summary>
		/// 是否结束一条语句
		/// </summary>
		public bool End { get; set; }

		public TokenAnalyzingArgs(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, TreeBuilder treeBuilder, TokenReader tokenReader)
		{
			this.BuildContext = buildContext;
			this.ScriptContext = scriptContext;
			this.TokenReader = tokenReader;
			this.TreeBuilder = treeBuilder;
			this.Control = control;
			this.Options = options;
		}
		public TokenAnalyzingArgs(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, TreeBuilder treeBuilder, TokenReader tokenReader, Token currentToken)
		{
			this.BuildContext = buildContext;
			this.ScriptContext = scriptContext;
			this.TokenReader = tokenReader;
			this.TreeBuilder = treeBuilder;
			this.Control = control;
			this.Options = options;
			this.CurrentToken = currentToken;
		}

		public static TokenAnalyzingArgs Create(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, TreeBuilder treeBuilder, TokenReader tokenReader)
		{
			if (_pool.TryDequeue(out var e))
			{
				e.BuildContext = buildContext;
				e.ScriptContext = scriptContext;
				e.TokenReader = tokenReader;
				e.TreeBuilder = treeBuilder;
				e.Control = control;
				e.Options = options;
				e.Ignore = false;
				e.End = false;
				e.IsHandled = false;
				return e;
			}
			return new TokenAnalyzingArgs(buildContext, scriptContext, options, control, treeBuilder, tokenReader);
		}

		public static TokenAnalyzingArgs Create(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options, EvalControl control, TreeBuilder treeBuilder, TokenReader tokenReader, Token currentToken)
		{
			if (_pool.TryDequeue(out var e))
			{
				e.BuildContext = buildContext;
				e.ScriptContext = scriptContext;
				e.TokenReader = tokenReader;
				e.TreeBuilder = treeBuilder;
				e.Control = control;
				e.Options = options;
				e.Ignore = false;
				e.End = false;
				e.CurrentToken = currentToken;
				e.IsHandled = false;
				return e;
			}
			return new TokenAnalyzingArgs(buildContext, scriptContext, options, control, treeBuilder, tokenReader, currentToken);
		}

		internal static void Return(TokenAnalyzingArgs e)
		{
			e.BuildContext = null;
			e.ScriptContext = null;
			e.TokenReader = null;
			e.TreeBuilder = null;
			e.Control = null;
			e.Options = null;
			//e.Ignore = false;
			//e.End = false;
			//e.IsHandled = false;
			//e.CurrentToken = default;

			if (_pool.Count < 10)
			{
				_pool.Enqueue(e);
			}
		}
	}
}
