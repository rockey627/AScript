using AScript.Values;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		private Dictionary<string, Type> _LastTypes;
		private Dictionary<string, int> _VariableModifiers;
		private HashSet<string> _LocalVariables;
		private HashSet<string> _ChangedVariables;
		private Dictionary<string, Expression> _Events;

		// 用于递归函数定义
		private Dictionary<string, List<DelegateDefine>> _DelegateDefines;

		private LabelTarget _ContinueLabel;
		private LabelTarget _BreakLabel;

		private bool _UsedScriptContext;

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

		public BuildContext Main
		{
			get
			{
				var r = this;
				while (r.Parent != null)
				{
					if (r.IsMain) return r;
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
		/// 生成的委托类型，为null表示自动
		/// </summary>
		public Type DelegateType { get; set; }
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
		public HashSet<string> ChangedVariables
		{
			get
			{
				if (_ChangedVariables == null)
				{
					_ChangedVariables = new HashSet<string>();
				}
				return _ChangedVariables;
			}
		}

		public Dictionary<string, Type> LastTypes
		{
			get
			{
				if (_LastTypes == null)
				{
					_LastTypes = new Dictionary<string, Type>();
				}
				return _LastTypes;
			}
		}
		public Dictionary<string, int> VariableModifiers
		{
			get
			{
				if (_VariableModifiers == null)
				{
					_VariableModifiers = new Dictionary<string, int>();
				}
				return _VariableModifiers;
			}
		}
		/// <summary>
		/// 本地语句块内的变量
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
		public Dictionary<string, List<Expression>> TempFunctions { get; set; }
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
			return TryGetVariableOrParameter(name, out v, out ownerBuildContext, out outer, out _);
		}

		public bool TryGetVariableOrParameter(string name, out ParameterExpression v, out BuildContext ownerBuildContext, out bool outer, out Type lastType)
		{
			var context = this;
			outer = false;
			do
			{
				if (context._Variables != null
					&& context._Variables.TryGetValue(name, out v))
				{
					ownerBuildContext = context;
					if (context._LastTypes == null) lastType = null;
					else context._LastTypes.TryGetValue(name, out lastType);
					return true;
				}
				if (context._Parameters != null
					&& context._Parameters.TryGetValue(name, out v))
				{
					ownerBuildContext = context;
					if (context._LastTypes == null) lastType = null;
					else context._LastTypes.TryGetValue(name, out lastType);
					return true;
				}
				if (context.IsMain) outer = true;
				context = context.Parent;
			} while (context != null);
			v = null;
			ownerBuildContext = null;
			lastType = null;
			return false;
		}

		public void ThrowIfReadOnly(string name)
		{
			if (_VariableModifiers == null) return;
			if (_VariableModifiers.TryGetValue(name, out var modifier))
			{
				Modifiers.ThrowIfReadOnly(name, modifier);
			}
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

		public void AddTempFunc(string name, Expression d)
		{
			List<Expression> list;
			if (this.TempFunctions == null)
			{
				this.TempFunctions = new Dictionary<string, List<Expression>>();
				this.TempFunctions[name] = list = new List<Expression>();
			}
			else if (!this.TempFunctions.TryGetValue(name, out list))
			{
				this.TempFunctions[name] = list = new List<Expression>();
			}
			list.Add(d);
		}

		public bool HasFunc(string name)
		{
			var context = this;
			do
			{
				if (context.TempFunctions != null
					&& context.TempFunctions.ContainsKey(name))
				{
					return true;
				}
				context = context.Parent;
			} while (context != null);
			return false;
		}

		public DelegateDefine AddDelegateDefine(string name, Type[] argTypes, Type returnType)
		{
			List<DelegateDefine> list;
			if (_DelegateDefines == null)
			{
				_DelegateDefines = new Dictionary<string, List<DelegateDefine>>();
				_DelegateDefines[name] = list = new List<DelegateDefine>();
			}
			else if (!_DelegateDefines.TryGetValue(name, out list))
			{
				_DelegateDefines[name] = list = new List<DelegateDefine>();
			}
			var delegateDefine = new DelegateDefine(name, argTypes, returnType);
			list.Add(delegateDefine);
			return delegateDefine;
			//return null;
		}

		public bool HasDelegateDefine(string name)
		{
			return _DelegateDefines != null && _DelegateDefines.ContainsKey(name);
		}

		public ParameterExpression GetDelegateDefine(string name, IList<Type> inArgTypes)
		{
			if (_DelegateDefines == null || _DelegateDefines.Count == 0) return null;
			if (!_DelegateDefines.TryGetValue(name, out var list)) return null;
			if (list.Count == 0) return null;
			var delegateDefine = list.FirstOrDefault(a => ScriptUtils.IsMatchArgTypes(inArgTypes, a.ArgTypes));
			if (delegateDefine == null) return null;
			if (delegateDefine.Variable == null)
			{
				delegateDefine.Variable = Expression.Variable(ScriptUtils.GetDelegateType(delegateDefine.ArgTypes, delegateDefine.ReturnType ?? typeof(object)), delegateDefine.Name);
			}
			return delegateDefine.Variable;
		}

		public Expression GetEvent(ScriptContext scriptContext, string name, Type delegateType)
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

			var d = scriptContext.GetEvent(name, delegateType);
			if (d != null) return Expression.Constant(d);
			return null;
		}

		public Expression GetOrCreateEvent(ScriptContext scriptContext, string name, Type delegateType, out bool isLocal)
		{
			string eventKey = $"{name}_{delegateType.GetHashCode()}";
			var context = this;
			while (context != null)
			{
				var events = context._Events;
				if (events != null && events.TryGetValue(eventKey, out var e))
				{
					isLocal = true;
					return e;
				}
				context = context.Parent;
			}

			var argTypes = delegateType.GetMethod("Invoke").GetParameters().Select(a => a.ParameterType).ToArray();
			context = this;
			while (context != null)
			{
				var tempFunctions = context.TempFunctions;
				if (tempFunctions != null && tempFunctions.TryGetValue(name, out var list3))
				{
					var func = ScriptContext.GetFunc(list3, argTypes, out _, out _);
					if (func != null)
					{
						if (context._Events == null)
						{
							context._Events = new Dictionary<string, Expression>();
						}
						var expr = ScriptUtils.ConvertDelegate(func, delegateType);
						if (expr != func)
						{
							var tmpVar = Expression.Variable(expr.Type);
							context.PrevExpressions.Add(Expression.Assign(tmpVar, expr));
							int hashCode = tmpVar.GetHashCode();
							string tmpVarName = hashCode > 0 ? $"<>$tmpVar_{hashCode}" : $"<>$tmpVar__{-hashCode}";
							context.Variables[tmpVarName] = tmpVar;
							context.LocalVariables.Add(tmpVarName);
							expr = tmpVar;
						}
						context._Events[eventKey] = expr;
						isLocal = true;
						return expr;
						//if (func is LambdaExpression lambda)
						//{
						//	Delegate d;
						//	if (lambda.Type == delegateType)
						//	{
						//		d = lambda.Compile();
						//	}
						//	else
						//	{
						//		d = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters).Compile();
						//	}
						//	var expr = Expression.Constant(d);
						//	context._Events[eventKey] = expr;
						//	isLocal = true;
						//	return expr;
						//}
					}
				}
				context = context.Parent;
			}

			isLocal = false;
			var del = scriptContext.GetOrCreateEvent(name, delegateType);
			if (del != null) return Expression.Constant(del);
			return null;
		}

		public ParameterExpression GetScriptContextParameter(bool forUse = true)
		{
			if (forUse) this._UsedScriptContext = true;
			var context = this;
			do
			{
				if (context.ScriptContextParameter != null)
				{
					return context.ScriptContextParameter;
				}
				context = context.Parent;
			} while (context != null);
			return ScriptUtils.Parameter_ScriptContext;
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

		private List<Expression> TryExpandBodies(Expression[] bodies)
		{
			if (bodies == null || bodies.Length == 0) return null;
			List<Expression> expandBodies = null;
			for (int i = 0; i < bodies.Length; i++)
			{
				var body = bodies[i];
				if (body is BlockExpression blockExpression && blockExpression.Variables.Count == 0 && blockExpression.Type != typeof(void))
				{
					if (expandBodies == null)
					{
						expandBodies = new List<Expression>();
						for (int j = 0; j < i; j++)
						{
							expandBodies.Add(bodies[j]);
						}
					}
					var exprs = blockExpression.Expressions;
					for (int j = 0; j < exprs.Count; j++)
					{
						expandBodies.Add(exprs[j]);
					}
				}
				else if (expandBodies != null)
				{
					expandBodies.Add(body);
				}
			}
			return expandBodies;
		}

		/// <summary>
		/// 构建Block表达式
		/// </summary>
		/// <param name="scriptContext"></param>
		/// <param name="options"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public Expression BuildBlock(ScriptContext scriptContext, BuildOptions options, params Expression[] body)
		{
			var scriptContextParameter = GetScriptContextParameter(false);
			int _VariablesCount = _Variables == null ? 0 : _Variables.Count;
			int _PrevExpressionsCount = _PrevExpressions == null ? 0 : _PrevExpressions.Count;
			List<Expression> expandBodies;
			if (_PrevExpressionsCount == 0 && (!_UsedScriptContext || scriptContextParameter == ScriptUtils.Parameter_ScriptContext) && this.ReturnVariableExpression == null)
			{
				if (_VariablesCount == 0)
				{
					if (body == null || body.Length == 0)
					{
						if (this.ReturnType == null || this.ReturnType == typeof(void))
						{
							return Expression.Empty();
						}
						return Expression.Default(this.ReturnType);
					}
					if (this.ReturnType == null || this.ReturnType == body[body.Length - 1].Type)
					{
						if (body.Length == 1)
						{
							return body[0];
						}
						expandBodies = TryExpandBodies(body);
						return expandBodies == null ? Expression.Block(body) : Expression.Block(expandBodies);
					}
				}
				//else
				//{
				//	return Expression.Lambda(Expression.Block(_Variables.Values, body), parameters);
				//}
			}
			expandBodies = TryExpandBodies(body);
			// 
			List<ParameterExpression> variables;
			Expression variableAssignExpression;
			bool standalone = options.Standalone ?? false;
			if (_UsedScriptContext && !standalone && scriptContextParameter != ScriptUtils.Parameter_ScriptContext && this.ScriptContextParameter != null)
			{
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
					variableAssignExpression = Expression.Assign(scriptContextParameter, Expression.Call(ScriptUtils.Method_ScriptContext_Create2, this.Parent.GetScriptContextParameter(), ScriptUtils.Constant_false));
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
			// 
			List<Expression> list = new List<Expression>();
			if (variableAssignExpression != null)
			{
				list.Add(variableAssignExpression);
			}
			if (_PrevExpressionsCount > 0)
			{
				list.AddRange(_PrevExpressions);
			}
			if (expandBodies != null) list.AddRange(expandBodies);
			else if (body != null && body.Length > 0) list.AddRange(body);
			// 
			int lastExpressionIndex = -1;
			Expression lastExpression = null;
			if (list != null && list.Count > 0)
			{
				lastExpressionIndex = list.Count - 1;
				lastExpression = list[lastExpressionIndex];
				// 如果最后一条表达式是void类型（如return语句），不处理返回值赋值
				if (lastExpression.Type != typeof(void))
				{
					if (this.ReturnType != null && this.ReturnType != typeof(void) && this.ReturnType != lastExpression.Type)
					{
						lastExpression = ScriptUtils.Convert(lastExpression, this.ReturnType);
						list[lastExpressionIndex] = lastExpression;
					}
					else
					{
						lastExpression = AValue.GetExpression(lastExpression);
						list[lastExpressionIndex] = lastExpression;
					}
					// 无论是否有本地变量，都需要将最后一个表达式的值作为返回值
					//if (this.ReturnVariableExpression == null)
					//{
					//	this.ReturnVariableExpression = Expression.Variable(lastExpression.Type);
					//}
					//if (lastExpression.Type != this.ReturnVariableExpression.Type)
					//{
					//	lastExpression = Expression.Convert(lastExpression, this.ReturnVariableExpression.Type);
					//}
					//list[lastExpressionIndex] = Expression.Assign(this.ReturnVariableExpression, lastExpression);
				}
			}
			// return label
			if (this.ReturnLabel != null)
			{
				list.Add(Expression.Label(this.ReturnLabel));
			}
			// 变量回写
			if (_VariablesCount > 0 && !standalone && (options?.RewriteVariables ?? true))
			{
				foreach (var v in _Variables.Values)
				{
					if (string.IsNullOrEmpty(v.Name)) continue;
					bool searchParent = _LocalVariables == null || !_LocalVariables.Contains(v.Name);
					if (!searchParent)
					{
						if (!this.RewriteLocalVariables)
						{
							// 不回写本地变量
							continue;
						}
					}
					else if (_ChangedVariables == null || !_ChangedVariables.Contains(v.Name)) continue;
					if (_VariableModifiers != null && _VariableModifiers.TryGetValue(v.Name, out var modifier)
						&& Modifiers.IsReadOnly(modifier))
					{
						list.Add(Expression.Call(
							scriptContextParameter,
							ScriptUtils.Method_ScriptContext_SetTempConst,
							Expression.Constant(v.Name),
							Expression.Convert(v, typeof(object)),
							Expression.Constant(v.Type),
							Expression.Constant(searchParent)));
					}
					else
					{
						list.Add(Expression.Call(
							scriptContextParameter,
							ScriptUtils.Method_ScriptContext_SetTempVar,
							Expression.Constant(v.Name),
							Expression.Convert(v, typeof(object)),
							Expression.Constant(v.Type),
							Expression.Constant(searchParent)));
					}
				}
			}
			if (this.ReturnType != typeof(void) && lastExpressionIndex > -1 && list.Count - 1 > lastExpressionIndex && lastExpression.Type != typeof(void))
			{
				if (this.ReturnVariableExpression == null)
				{
					this.ReturnVariableExpression = Expression.Variable(lastExpression.Type);
				}
				else if (lastExpression.Type != this.ReturnVariableExpression.Type)
				{
					lastExpression = Expression.Convert(lastExpression, this.ReturnVariableExpression.Type);
				}
				list[lastExpressionIndex] = Expression.Assign(this.ReturnVariableExpression, lastExpression);
			}
			if (this.ReturnVariableExpression != null)
			{
				if (this.ReturnType != null && this.ReturnType != typeof(void) && this.ReturnType != this.ReturnVariableExpression.Type)
				{
					list.Add(ScriptUtils.Convert(this.ReturnVariableExpression, this.ReturnType));
				}
				else
				{
					list.Add(this.ReturnVariableExpression);
				}
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
			if (this.ReturnType == typeof(void))
			{
				return Expression.Block(this.ReturnType, variables, list);
			}
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
			var scriptContextParameter = GetScriptContextParameter(false);
			if (!this.IsMain && !(options.Standalone ?? false) && scriptContextParameter == ScriptUtils.Parameter_ScriptContext)
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
			if (block == null)
			{
				//if (parameters.Length == 0) return null;
				block = Expression.Empty();
			}
			return this.DelegateType == null ? Expression.Lambda(block, parameters) : Expression.Lambda(this.DelegateType, block, parameters);
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
			var bodys = body == null ? null : new[] { body };
			return Build(scriptContext, options, bodys).Compile();
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
