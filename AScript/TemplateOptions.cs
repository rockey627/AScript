using System;

namespace AScript
{
	public class TemplateOptions
	{
		private ECompileMode? _CompileMode;
		private bool? _ThrowIfVariableNotExists;
		private string _OutputVar;
		private string _ModelVar;
		private string _LoadMethod;
		private string _IncludeMethod;
		private Func<object, string, string> _CacheVersion;

		public TemplateOptions Parent { get; set; }

		/// <summary>
		/// 编译模式
		/// </summary>
		public ECompileMode? CompileMode
		{
			get => _CompileMode ?? this.Parent?.CompileMode;
			set => _CompileMode = value;
		}
		/// <summary>
		/// 如果变量不存在则抛异常，否则返回变量默认值
		/// </summary>
		public bool? ThrowIfVariableNotExists
		{
			get => _ThrowIfVariableNotExists ?? this.Parent?.ThrowIfVariableNotExists;
			set => _ThrowIfVariableNotExists = value;
		}
		/// <summary>
		/// 输出流变量名
		/// </summary>
		public string OutputVar
		{
			get => string.IsNullOrEmpty(_OutputVar) ? this.Parent?.OutputVar : _OutputVar;
			set => _OutputVar = value;
		}
		/// <summary>
		/// 模型对象变量名
		/// </summary>
		public string ModelVar
		{
			get => string.IsNullOrEmpty(_ModelVar) ? this.Parent?.ModelVar : _ModelVar;
			set => _ModelVar = value;
		}
		/// <summary>
		/// 加载模板文件的方法名（独立渲染模板）
		/// </summary>
		public string LoadMethod
		{
			get => string.IsNullOrEmpty(_LoadMethod) ? this.Parent?.LoadMethod : _LoadMethod;
			set => _LoadMethod = value;
		}
		/// <summary>
		/// 包含模板文件的方法名（在宿主模板中渲染）
		/// </summary>
		public string IncludeMethod
		{
			get => string.IsNullOrEmpty(_IncludeMethod) ? this.Parent?.IncludeMethod : _IncludeMethod;
			set => _IncludeMethod = value;
		}
		/// <summary>
		/// 动态计算模板版本号，参数为<c>(template, cacheKey)</c>
		/// </summary>
		public Func<object, string, string> CacheVersion
		{
			get => _CacheVersion ?? this.Parent?.CacheVersion;
			set => _CacheVersion = value;
		}

		public TemplateOptions() { }
		public TemplateOptions(TemplateOptions parent)
		{
			this.Parent = parent;
		}
	}
}
