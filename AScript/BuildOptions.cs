using System;

namespace AScript
{
	public class BuildOptions
	{
		private ECompileMode? _CompileMode;
		private bool? _ThrowIfVariableNotExists;
		private bool? _CreateFullTreeNode;
		private bool? _CreateFullStatement;
		private bool? _RewriteVariables;
		private bool? _RewriteFunctions;
		private bool? _Dynamic;
		private bool? _Standalone;
		private bool? _UseCompletionResult;

		/// <summary>
		/// 
		/// </summary>
		public BuildOptions Parent { get; set; }

		/// <summary>
		/// 先完整构建表达式树，再编译或执行
		/// </summary>
		public bool? CreateFullTreeNode
		{
			get => _CreateFullStatement ?? _CreateFullTreeNode ?? this.Parent?.CreateFullTreeNode;
			set => _CreateFullTreeNode = value;
		}
		public bool? CreateFullStatement
		{
			get => _CreateFullStatement ?? this.Parent?.CreateFullStatement;
			set => _CreateFullStatement = value;
		}
		/// <summary>
		/// <para>编译模式</para>
		/// <para>表达式比较长或者有for/while/foreach等循环语句时，开启编译模式能显著提升性能</para>
		/// </summary>
		public ECompileMode? CompileMode
		{
			get => _CompileMode ?? this.Parent?.CompileMode;
			set => _CompileMode = value;
		}
		/// <summary>
		/// 如果变量不存在是否抛异常，否则返回变量默认值
		/// </summary>
		public bool? ThrowIfVariableNotExists
		{
			get => _ThrowIfVariableNotExists ?? this.Parent?.ThrowIfVariableNotExists;
			set => _ThrowIfVariableNotExists = value;
		}
		/// <summary>
		/// <para>是否回写变量到上下文（默认为true）</para>
		/// <para>编译执行模式下有效</para>
		/// <para>如果设置为false，则脚本中定义的变量或者修改变量，不回写到上下文，可提高性能</para>
		/// </summary>
		public bool? RewriteVariables
		{
			get => _RewriteVariables ?? this.Parent?.RewriteVariables;
			set => _RewriteVariables = value;
		}
		/// <summary>
		/// <para>是否回写函数到上下文（默认为true）</para>
		/// <para>如果设置为false，则脚本中定义的函数不回写到上下文，可提高性能</para>
		/// </summary>
		public bool? RewriteFunctions
		{
			get => _RewriteFunctions ?? this.Parent?.RewriteFunctions;
			set => _RewriteFunctions = value;
		}
		/// <summary>
		/// 是否启用动态语言特性
		/// </summary>
		public bool? Dynamic
		{
			get => _Dynamic ?? this.Parent?.Dynamic;
			set => _Dynamic = value;
		}
		/// <summary>
		/// <para>编译结果是否脱离上下文（默认为false）</para>
		/// <para>在编译模式下生效，编译结果不缓存</para>
		/// <para>如果为true，则编译生成的Lambda或委托没有ScriptContext参数，依赖于上下文的变量值将直接编译到结果中</para>
		/// <para>当编译结果明确不依赖上下文时，可设置Standalone为true，提高性能</para>
		/// </summary>
		public bool? Standalone
		{
			get => _Standalone ?? this.Parent?.Standalone;
			set => _Standalone = value;
		}
		/// <summary>
		/// 是否使用<c>CompletionResult</c>返回结果对象
		/// </summary>
		public bool? UseCompletionResult
		{
			get => _UseCompletionResult ?? this.Parent?.UseCompletionResult;
			set => _UseCompletionResult = value;
		}

		public BuildOptions() { }
		public BuildOptions(BuildOptions parent)
		{
			this.Parent = parent;
		}
	}
}
