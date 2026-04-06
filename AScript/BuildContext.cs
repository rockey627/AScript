using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript
{
	/// <summary>
	/// 
	/// </summary>
	public class BuildContext
	{
		private List<Expression> _PrevExpressions;
		private Dictionary<string, ParameterExpression> _Variables;
		private Dictionary<string, ParameterExpression> _Parameters;
		private HashSet<string> _LocalVariables;

		private LabelTarget _ContinueLabel;
		private LabelTarget _BreakLabel;

		public BuildContext Parent { get; set; }

		public BuildContext Root
		{
			get
			{
				var r = this;
				while (r.Parent != null)
				{
					r = r.Parent;
				}
				return r;
			}
		}

		/// <summary>
		/// 指定ScriptContext参数
		/// </summary>
		public ParameterExpression ScriptContextParameter { get; set; }
		/// <summary>
		/// 指定返回类型
		/// </summary>
		public Type ReturnType { get; set; }
		/// <summary>
		/// 前置表达式列表
		/// </summary>
		public List<Expression> PrevExpressions
		{
			get
			{
				if (_PrevExpressions == null)
				{
					_PrevExpressions = new List<Expression>();
				}
				return _PrevExpressions;
			}
		}
		/// <summary>
		/// 变量列表
		/// </summary>
		public Dictionary<string, ParameterExpression> Variables
		{
			get
			{
				if (_Variables == null)
				{
					_Variables = new Dictionary<string, ParameterExpression>();
				}
				return _Variables;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public HashSet<string> LocalVariables
		{
			get
			{
				if (_LocalVariables == null)
				{
					_LocalVariables = new HashSet<string>();
				}
				return _LocalVariables;
			}
		}
		/// <summary>
		/// 参数列表
		/// </summary>
		public Dictionary<string, ParameterExpression> Parameters
		{
			get
			{
				if (_Parameters == null)
				{
					_Parameters = new Dictionary<string, ParameterExpression>();
				}
				return _Parameters;
			}
		}
		/// <summary>
		/// 编译的临时函数
		/// </summary>
		//public Dictionary<string, List<Delegate>> TempFunctions { get; set; }
		public Dictionary<string, List<LambdaExpression>> TempFunctions { get; set; }
		/// <summary>
		/// 是否回写本地变量
		/// </summary>
		public bool RewriteLocalVariables { get; set; }

		public ParameterExpression ReturnVariableExpression { get; set; }

		public LabelTarget ContinueLabel
		{
			get
			{
				var c = this;
				do
				{
					if (c._ContinueLabel != null)
					{
						return c._ContinueLabel;
					}
					c = c.Parent;
				} while (c != null);
				return null;
			}
			set
			{
				_ContinueLabel = value;
			}
		}
		public LabelTarget BreakLabel
		{
			get
			{
				var c = this;
				do
				{
					if (c._BreakLabel != null)
					{
						return c._BreakLabel;
					}
					c = c.Parent;
				} while (c != null);
				return null;
			}
			set
			{
				_BreakLabel = value;
			}
		}
		public LabelTarget ReturnLabel { get; set; }

		public bool IsMain { get; set; }

		public BuildContext()
		{
			this.RewriteLocalVariables = true;
		}
		public BuildContext(BuildContext parent)
		{
			this.Parent = parent;
			this.RewriteLocalVariables = false;
		}

		public BuildContext GetReturnBuildContext()
		{
			var c = this;
			while (c.Parent != null)
			{
				if (c.IsMain)
				{
					break;
				}
				c = c.Parent;
			}
			return c;
		}

		public bool TryGetVariable(string name, out ParameterExpression v)
		{
			var context = this;
			do
			{
				if (context._Variables != null
					&& context._Variables.TryGetValue(name, out v))
				{
					return true;
				}
				context = context.Parent;
			} while (context != null);
			v = null;
			return false;
		}

		public bool TryGetParameter(string name, out ParameterExpression p)
		{
			var context = this;
			do
			{
				if (context._Parameters != null
					&& context._Parameters.TryGetValue(name, out p))
				{
					return true;
				}
				context = context.Parent;
			} while (context != null);
			p = null;
			return false;
		}

		public bool TryGetVariableOrParameter(string name, out ParameterExpression v)
		{
			return TryGetVariableOrParameter(name, out v, out _, out _);
		}

		public bool TryGetVariableOrParameter(string name, out ParameterExpression v, out BuildContext ownerBuildContext, out bool outer)
		{
			var context = this;
			outer = false;
			do
			{
				if (context._Variables != null
					&& context._Variables.TryGetValue(name, out v))
				{
					ownerBuildContext = context;
					return true;
				}
				if (context._Parameters != null
					&& context._Parameters.TryGetValue(name, out v))
				{
					ownerBuildContext = context;
					return true;
				}
				if (context.IsMain) outer = true;
				context = context.Parent;
			} while (context != null);
			v = null;
			ownerBuildContext = null;
			return false;
		}

		//public void AddTempFunc(string name, LambdaExpression d)
		//{
		//	List<LambdaExpression> list;
		//	if (this.TempFunctions == null)
		//	{
		//		this.TempFunctions = new Dictionary<string, List<LambdaExpression>>();
		//		this.TempFunctions[name] = list = new List<LambdaExpression>();
		//	}
		//	else if (!this.TempFunctions.TryGetValue(name, out list))
		//	{
		//		this.TempFunctions[name] = list = new List<LambdaExpression>();
		//	}
		//	list.Add(d);
		//}

		//public void AddTempFunc(string name, Delegate d)
		//{
		//	List<Delegate> list;
		//	if (this.TempFunctions == null)
		//	{
		//		this.TempFunctions = new Dictionary<string, List<Delegate>>();
		//		this.TempFunctions[name] = list = new List<Delegate>();
		//	}
		//	else if (!this.TempFunctions.TryGetValue(name, out list))
		//	{
		//		this.TempFunctions[name] = list = new List<Delegate>();
		//	}
		//	list.Add(d);
		//}

		public void AddTempFunc(string name, LambdaExpression d)
		{
			List<LambdaExpression> list;
			if (this.TempFunctions == null)
			{
				this.TempFunctions = new Dictionary<string, List<LambdaExpression>>();
				this.TempFunctions[name] = list = new List<LambdaExpression>();
			}
			else if (!this.TempFunctions.TryGetValue(name, out list))
			{
				this.TempFunctions[name] = list = new List<LambdaExpression>();
			}
			list.Add(d);
		}

		public ParameterExpression GetScriptContextParameter()
		{
			var context = this;
			do
			{
				if (context.ScriptContextParameter != null)
				{
					return context.ScriptContextParameter;
				}
				context = context.Parent;
			} while (context != null);
			return ExpressionUtils.Parameter_ScriptContext;
		}

		///// <summary>
		///// 构建Block表达式
		///// </summary>
		///// <param name="body"></param>
		///// <returns></returns>
		//public Expression BuildBlock(params Expression[] body)
		//{
		//	//
		//	int _VariablesCount = _Variables == null ? 0 : _Variables.Count;
		//	int _PrevExpressionsCount = _PrevExpressions == null ? 0 : _PrevExpressions.Count;
		//	if (_PrevExpressionsCount == 0)
		//	{
		//		if (_VariablesCount == 0)
		//		{
		//			if (body == null || body.Length == 0) return null;
		//			return body.Length == 1 ? body[0] : Expression.Block(body);
		//		}
		//		else
		//		{
		//			return Expression.Block(_Variables.Values, body);
		//		}
		//	}
		//	// 变量
		//	int blockCount = _PrevExpressionsCount + (body == null ? 0 : body.Length);
		//	List<Expression> list;
		//	if (blockCount == _PrevExpressionsCount)
		//	{
		//		list = _PrevExpressions;
		//	}
		//	else
		//	{
		//		list = new List<Expression>(blockCount);
		//		if (_PrevExpressionsCount > 0)
		//		{
		//			list.AddRange(_PrevExpressions);
		//		}
		//		if (body != null && body.Length > 0) list.AddRange(body);
		//	}
		//	//
		//	if (_VariablesCount == 0)
		//	{
		//		return Expression.Block(list);
		//	}
		//	return Expression.Block(_Variables.Values, list);
		//}

		/// <summary>
		/// 构建Block表达式
		/// </summary>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public Expression BuildBlock(ScriptContext scriptContext, BuildOptions options, params Expression[] body)
		{
			var scriptContextParameter = GetScriptContextParameter();
			int _VariablesCount = _Variables == null ? 0 : _Variables.Count;
			int _PrevExpressionsCount = _PrevExpressions == null ? 0 : _PrevExpressions.Count;
			if (_PrevExpressionsCount == 0 && scriptContextParameter == ExpressionUtils.Parameter_ScriptContext && this.ReturnVariableExpression == null)
			{
				if (_VariablesCount == 0)
				{
					if (body.Length == 1)
					{
						return body[0];
					}
					return Expression.Block(body);
				}
				//else
				//{
				//	return Expression.Lambda(Expression.Block(_Variables.Values, body), parameters);
				//}
			}
			// 变量
			int blockCount = _PrevExpressionsCount + (body == null ? 0 : body.Length);
			// 变量回写语句数量
			if (_VariablesCount > 0 && (options?.RewriteVariables ?? true))
			{
				blockCount += _VariablesCount;
				if (!this.RewriteLocalVariables && this._LocalVariables != null)
				{
					blockCount -= this._LocalVariables.Count;
				}
			}
			if (this.ReturnLabel != null) blockCount++;
			if (this.ReturnVariableExpression != null) blockCount++;
			List<ParameterExpression> variables;
			Expression variableAssignExpression;
			if (scriptContextParameter != ExpressionUtils.Parameter_ScriptContext && this.ScriptContextParameter != null)
			{
				// 增加参数赋值语句
				blockCount++;
				variables = new List<ParameterExpression>(_VariablesCount + (_VariablesCount == 0 ? 1 : 2));
				variables.Add(scriptContextParameter);
				if (_VariablesCount > 0)
				{
					variables.AddRange(_Variables.Values);
				}
				if (this.Parent == null)
				{
					variableAssignExpression = Expression.Assign(scriptContextParameter, Expression.Constant(scriptContext));
				}
				else
				{
					variableAssignExpression = Expression.Assign(scriptContextParameter, Expression.Call(ExpressionUtils.Method_ScriptContext_Create2, this.Parent.GetScriptContextParameter(), Expression.Constant(false)));
				}
			}
			else
			{
				if (_VariablesCount > 0)
				{
					variables = new List<ParameterExpression>(_VariablesCount + 1 + (this.ReturnVariableExpression == null ? 0 : 1));
					variables.AddRange(_Variables.Values);
				}
				else
				{
					variables = null;
				}
				variableAssignExpression = null;
			}
			if (this.ReturnLabel != null) blockCount++;
			List<Expression> list;
			if (blockCount == _PrevExpressionsCount)
			{
				list = _PrevExpressions;
			}
			else
			{
				list = new List<Expression>(blockCount);
				if (variableAssignExpression != null)
				{
					list.Add(variableAssignExpression);
				}
				if (_PrevExpressionsCount > 0)
				{
					list.AddRange(_PrevExpressions);
				}
				if (body != null && body.Length > 0) list.AddRange(body);
			}
			if (list != null && list.Count > 0)
			{
				var lastExpression = list[list.Count - 1];
				// 如果最后一条表达式是void类型（如return语句），不处理返回值赋值
				if (lastExpression.Type != typeof(void))
				{
					if (this.ReturnType != null && this.ReturnType != typeof(object) && this.ReturnType != lastExpression.Type)
					{
						lastExpression = Expression.Convert(lastExpression, this.ReturnType);
						list[list.Count - 1] = lastExpression;
					}
					// 无论是否有本地变量，都需要将最后一个表达式的值作为返回值
					if (this.ReturnVariableExpression == null)
					{
						this.ReturnVariableExpression = Expression.Variable(lastExpression.Type);
					}
					list[list.Count - 1] = Expression.Assign(this.ReturnVariableExpression, lastExpression);
				}
			}
			// return label
			if (this.ReturnLabel != null)
			{
				list.Add(Expression.Label(this.ReturnLabel));
			}
			// 变量回写
			if (_VariablesCount > 0 && (options?.RewriteVariables ?? true))
			{
				foreach (var v in _Variables.Values)
				{
					bool searchParent = _LocalVariables == null || !_LocalVariables.Contains(v.Name);
					if (!searchParent && !this.RewriteLocalVariables)
					{
						// 不回写本地变量
						continue;
					}
					list.Add(Expression.Call(
						scriptContextParameter,
						ExpressionUtils.Method_ScriptContext_SetTempVar,
						Expression.Constant(v.Name),
						Expression.Convert(v, typeof(object)),
						Expression.Constant(v.Type),
						Expression.Constant(searchParent)));
				}
			}
			if (this.ReturnVariableExpression != null)
			{
				list.Add(this.ReturnVariableExpression);
				if (variables == null)
				{
					variables = new List<ParameterExpression> { this.ReturnVariableExpression };
				}
				else
				{
					variables.Add(this.ReturnVariableExpression);
				}
			}
			// 
			return Expression.Block(variables, list);
		}

		/// <summary>
		/// 构建Lambda表达式
		/// </summary>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public LambdaExpression Build(ScriptContext scriptContext, BuildOptions options, params Expression[] body)
		{
			// 函数参数列表
			ParameterExpression[] parameters;
			int parameterIndex;
			int _ParameterCount = _Parameters == null ? 0 : _Parameters.Count;
			var scriptContextParameter = GetScriptContextParameter();
			if (!this.IsMain && scriptContextParameter == ExpressionUtils.Parameter_ScriptContext)
			{
				parameterIndex = 1;
				parameters = new ParameterExpression[_ParameterCount + 1];
				parameters[0] = scriptContextParameter;
			}
			else
			{
				parameterIndex = 0;
				parameters = new ParameterExpression[_ParameterCount];
			}
			if (_Parameters != null)
			{
				foreach (var item in _Parameters.Values)
				{
					parameters[parameterIndex++] = item;
				}
			}
			// 
			var block = BuildBlock(scriptContext, options, body);
			return Expression.Lambda(block, parameters);
		}

		/// <summary>
		/// 编译生成委托实例
		/// </summary>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public Delegate Compile(ScriptContext scriptContext, BuildOptions options, Expression body)
		{
			return Build(scriptContext, options, body).Compile();
		}

		public void Clear()
		{
			_PrevExpressions?.Clear();
			_Parameters?.Clear();
			_LocalVariables?.Clear();
			_Variables?.Clear();
			TempFunctions?.Clear();
		}
	}
}
