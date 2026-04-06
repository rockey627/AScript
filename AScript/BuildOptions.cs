using System;

namespace AScript
{
	public class BuildOptions
	{
		private ECompileMode? _CompileMode;
		private bool? _ThrowIfVariableNotExists;
		private bool? _DynamicVariableType;
		private bool? _CreateFullTreeNode;
		private bool? _RewriteVariables;
		private bool? _RewriteFunctions;

		/// <summary>
		/// 
		/// </summary>
		public BuildOptions Parent { get; set; }

		/// <summary>
		/// 先完整构建表达式树，再编译或执行
		/// </summary>
		public bool? CreateFullTreeNode
		{
			get => _CreateFullTreeNode ?? this.Parent?.CreateFullTreeNode;
			set => _CreateFullTreeNode = value;
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
		/// 如果变量不存在是否抛异常，否则返回变量默认值，默认为false
		/// </summary>
		public bool? ThrowIfVariableNotExists
		{
			get => _ThrowIfVariableNotExists ?? this.Parent?.ThrowIfVariableNotExists;
			set => _ThrowIfVariableNotExists = value;
		}
		/// <summary>
		/// 是否支持动态变量类型，默认为false
		/// </summary>
		public bool? DynamicVariableType
		{
			get => _DynamicVariableType ?? this.Parent?.DynamicVariableType;
			set => _DynamicVariableType = value;
		}
		/// <summary>
		/// 是否回写变量到上下文，默认为true，如果设置为false可提高性能
		/// </summary>
		public bool? RewriteVariables
		{
			get => _RewriteVariables ?? this.Parent?.RewriteVariables;
			set => _RewriteVariables = value;
		}
		/// <summary>
		/// 是否回写函数到上下文，默认为true，如果设置为false可提高性能
		/// </summary>
		public bool? RewriteFunctions
		{
			get => _RewriteFunctions ?? this.Parent?.RewriteFunctions;
			set => _RewriteFunctions = value;
		}

		public BuildOptions() { }
		public BuildOptions(BuildOptions parent)
		{
			this.Parent = parent;
		}
	}
}
