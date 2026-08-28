using AScript.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Nodes
{
	public class ForeachNode : TreeNode
	{
		public bool ForeachKey { get; set; }
		public DefineVarNode VarDefine { get; set; }
		public IList<DefineVarNode> VarDefines { get; set; }
		public ITreeNode Collection { get; set; }
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			if (this.VarDefine == null && this.VarDefines == null)
			{
				throw new ScriptAnalyzingException("require variable define in foreach statement");
			}
			if (this.Collection == null)
			{
				throw new ScriptAnalyzingException("require collection in foreach statement");
			}
			var mode = options.CompileMode;
			if (mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop))
			{
				// 编译循环
				return ScriptUtils.EvalWithCompile(context, options, control, this, out returnType);
				//var loopOptions = new BuildOptions(options)
				//{
				//	CompileMode = ECompileMode.All,
				//	UseCompletionResult = true,
				//	RewriteVariables = true,
				//	RewriteFunctions = false,
				//	Standalone = false
				//};
				//var loop = Script.Compile(null, context, loopOptions, this);
				//var loopResult = loop.DynamicInvoke(context);
				//if (loopResult is CompletionResult completionResult)
				//{
				//	if (completionResult.CompletionType == ECompletionType.Return)
				//	{
				//		control.Terminal = true;
				//	}
				//	returnType = completionResult.ValueType;
				//	return completionResult.Value;
				//}
				//returnType = loopResult?.GetType() ?? loop.Method.ReturnType;
				//return loopResult;
			}
			// 计算集合
			var listResult = this.Collection.Eval(context, options, control, out var listType);
			if (listResult == null)
			{
				returnType = null;
				return null;
			}
			if (!(listResult is IEnumerable en))
			{
				throw new ScriptAnalyzingException($"invalid foreach collection {listType}");
			}
			//
			object bodyResult = null;
			Type bodyType = null;
			if (this.Body != null)
			{
				var tempContext = ScriptContext.Create(context);
				var tempController = new EvalControl(control, true);
				// 定义变量
				if (this.VarDefines != null)
				{
					foreach (var vd in this.VarDefines)
					{
						if (IsNiming(vd)) continue;
						vd.Eval(tempContext, options, null, out _);
					}
				}
				else if (!IsNiming(VarDefine))
				{
					this.VarDefine.Eval(tempContext, options, null, out _);
				}
				// 循环
				if (this.ForeachKey && listResult is IDictionary<string, object> dict2)
				{
					// ForeachKey 为 true 且是 IDictionary，遍历 Keys
					foreach (var key in dict2.Keys)
					{
						if (!IsNiming(this.VarDefine))
						{
							this.VarDefine.Eval(tempContext, options, null, out var varType);
							tempContext.SetVar(this.VarDefine.Name, key, key == null ? varType : null);
						}
						bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else if (this.ForeachKey && listResult is IDictionary dict)
				{
					// ForeachKey 为 true 且是 IDictionary，遍历 Keys
					foreach (var key in dict.Keys)
					{
						if (!IsNiming(this.VarDefine))
						{
							this.VarDefine.Eval(tempContext, options, null, out var varType);
							tempContext.SetVar(this.VarDefine.Name, key, key == null ? varType : null);
						}
						bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else if (this.ForeachKey && listResult is IList list1)
				{
					// ForeachKey 为 true 且是 IList，使用 for 遍历索引
					for (int i = 0; i < list1.Count; i++)
					{
						if (!IsNiming(this.VarDefine))
						{
							this.VarDefine.Eval(tempContext, options, null, out var varType);
							tempContext.SetVar(this.VarDefine.Name, i, typeof(int));
						}
						bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else
				{
					// 默认遍历模式（遍历值）
					foreach (var item in en)
					{
						if (this.VarDefines != null)
						{
							// 解构列表项赋值到各个变量
							IList itemList = null;
							if (item is IList list)
							{
								itemList = list;
								//itemList = new List<object>(list.Count);
								//foreach (var i in list)
								//{
								//	itemList.Add(i);
								//}
							}
							else
							{
								// 支持 Tuple/ValueTuple 解构
								var itemType = item.GetType();
								if (itemType.IsGenericType)
								{
									var genericType = itemType.GetGenericTypeDefinition();
									if (genericType.Name.StartsWith("Tuple`"))
									{
										itemList = new List<object>();
										foreach (var prop in itemType.GetProperties())
										{
											itemList.Add(prop.GetValue(item));
										}
									}
#if !NET45
									else if (genericType.Name.StartsWith("ValueTuple`"))
									{
										itemList = new List<object>();
										foreach (var field in itemType.GetFields())
										{
											itemList.Add(field.GetValue(item));
										}
									}
#endif
								}
							}
							//if (itemList == null)
							//{
							//	throw new ScriptAnalyzingException($"cannot unpack item of type {item?.GetType()} into {this.VarDefines.Count} variables");
							//}
							//if (itemList.Count < this.VarDefines.Count)
							//{
							//	throw new ScriptAnalyzingException($"not enough values to unpack (expected {this.VarDefines.Count}, got {itemList.Count})");
							//}
							int itemIndex = 0;
							for (int i = 0; i < this.VarDefines.Count; i++)
							{
								var varDefine = this.VarDefines[i];
								if (varDefine == null) continue;
								if (!IsNiming(varDefine))
								{
									tempContext.SetVar(varDefine.Name, itemList == null || itemList.Count == 0 ? item : itemList[itemIndex++], null);
								}
								if (itemList == null || itemList.Count == 0 || itemIndex >= itemList.Count) break;
							}
						}
						else if (!IsNiming(this.VarDefine))
						{
							this.VarDefine.Eval(tempContext, options, null, out var varType);
							tempContext.SetVar(this.VarDefine.Name, item, item == null ? varType : null);
						}
						bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
			}
			returnType = bodyType;
			return bodyResult;
		}

		public override async Task<EvalResult> EvalAsync(ScriptContext context, BuildOptions options, EvalControl control, CancellationToken cancellationToken = default)
		{
			if (this.VarDefine == null && this.VarDefines == null)
			{
				throw new ScriptAnalyzingException("require variable define in foreach statement");
			}
			if (this.Collection == null)
			{
				throw new ScriptAnalyzingException("require collection in foreach statement");
			}
			var mode = options.CompileMode;
			bool compileLoop = mode.HasValue && ((mode.Value & ECompileMode.Loop) == ECompileMode.Loop);
			if (compileLoop)
			{
				// 编译循环
				var loopOptions = new BuildOptions(options)
				{
					CompileMode = ECompileMode.All,
					UseCompletionResult = true,
					RewriteVariables = true,
					RewriteFunctions = false,
					Standalone = false
				};
				var loop = Script.Compile(null, context, loopOptions, this);
				var loopResult = loop.DynamicInvoke(context);
				if (loopResult is EvalResult completionResult)
				{
					if (completionResult.CompletionType == ECompletionType.Return)
					{
						control.Terminal = true;
					}
					return completionResult;
				}
				return new EvalResult(loopResult, loopResult?.GetType() ?? loop.Method.ReturnType);
			}
			// 计算集合
			var evalResult = await this.Collection.EvalAsync(context, options, control, cancellationToken).ConfigureAwait(false);
			var listResult = evalResult.Value;
			var listType = evalResult.Type;
			if (listResult == null)
			{
				return default;
			}
			if (!(listResult is IEnumerable en))
			{
				throw new ScriptAnalyzingException($"invalid foreach collection {listType}");
			}
			//
			EvalResult bodyResult = default;
			if (this.Body != null)
			{
				var tempContext = ScriptContext.Create(context);
				var tempController = new EvalControl(control, true);
				// 定义变量
				if (this.VarDefines != null)
				{
					foreach (var vd in this.VarDefines)
					{
						if (IsNiming(vd)) continue;
						await vd.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
					}
				}
				else if (!IsNiming(this.VarDefine))
				{
					await this.VarDefine.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
				}
				// 循环
				if (this.ForeachKey && listResult is IDictionary<string, object> dict2)
				{
					// ForeachKey 为 true 且是 IDictionary，遍历 Keys
					foreach (var key in dict2.Keys)
					{
						cancellationToken.ThrowIfCancellationRequested();
						if (!IsNiming(this.VarDefine))
						{
							var varDefineResult = await this.VarDefine.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
							tempContext.SetVar(this.VarDefine.Name, key, key == null ? varDefineResult.Type : null);
						}
						bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempController, cancellationToken).ConfigureAwait(false);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else if (this.ForeachKey && listResult is IDictionary dict)
				{
					// ForeachKey 为 true 且是 IDictionary，遍历 Keys
					foreach (var key in dict.Keys)
					{
						cancellationToken.ThrowIfCancellationRequested();
						if (!IsNiming(this.VarDefine))
						{
							var varDefineResult = await this.VarDefine.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
							tempContext.SetVar(this.VarDefine.Name, key, key == null ? varDefineResult.Type : null);
						}
						bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempController, cancellationToken).ConfigureAwait(false);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else if (this.ForeachKey && listResult is IList list1)
				{
					// ForeachKey 为 true 且是 IList，使用 for 遍历索引
					for (int i = 0; i < list1.Count; i++)
					{
						cancellationToken.ThrowIfCancellationRequested();
						if (!IsNiming(this.VarDefine))
						{
							var varDefineResult = await this.VarDefine.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
							tempContext.SetVar(this.VarDefine.Name, i, typeof(int));
						}
						bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempController, cancellationToken).ConfigureAwait(false);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
				else
				{
					// 默认遍历模式（遍历值）
					foreach (var item in en)
					{
						cancellationToken.ThrowIfCancellationRequested();
						if (this.VarDefines != null)
						{
							// 解构列表项赋值到各个变量
							IList itemList = null;
							if (item is IList list)
							{
								itemList = list;
								//foreach (var i in list)
								//{
								//	itemList.Add(i);
								//}
							}
							else
							{
								// 支持 Tuple/ValueTuple 解构
								var itemType = item.GetType();
								if (itemType.IsGenericType)
								{
									var genericType = itemType.GetGenericTypeDefinition();
									if (genericType.Name.StartsWith("Tuple`"))
									{
										itemList = new List<object>();
										foreach (var prop in itemType.GetProperties())
										{
											itemList.Add(prop.GetValue(item));
										}
									}
#if !NET45
									else if (genericType.Name.StartsWith("ValueTuple`"))
									{
										itemList = new List<object>();
										foreach (var field in itemType.GetFields())
										{
											itemList.Add(field.GetValue(item));
										}
									}
#endif
								}
							}
							//if (itemList == null)
							//{
							//	throw new ScriptAnalyzingException($"cannot unpack item of type {item?.GetType()} into {this.VarDefines.Count} variables");
							//}
							//if (itemList.Count < this.VarDefines.Count)
							//{
							//	throw new ScriptAnalyzingException($"not enough values to unpack (expected {this.VarDefines.Count}, got {itemList.Count})");
							//}
							int itemIndex = 0;
							for (int i = 0; i < this.VarDefines.Count; i++)
							{
								var varDefine = this.VarDefines[i];
								if (varDefine == null) continue;
								if (!IsNiming(varDefine))
								{
									tempContext.SetVar(varDefine.Name, itemList == null || itemList.Count == 0 ? item : itemList[itemIndex++], null);
								}
								if (itemList == null || itemList.Count == 0 || itemIndex >= itemList.Count) break;
							}
						}
						else if (!IsNiming(this.VarDefine))
						{
							var varDefineResult = await this.VarDefine.EvalAsync(tempContext, options, null, cancellationToken).ConfigureAwait(false);
							tempContext.SetVar(this.VarDefine.Name, item, item == null ? varDefineResult.Type : null);
						}
						bodyResult = await this.Body.EvalAsync(ScriptContext.Create(tempContext), options, tempController, cancellationToken).ConfigureAwait(false);
						if (tempController.Terminal || tempController.Break) break;
						tempController.Continue = false;
					}
				}
			}
			return bodyResult;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();
			var listExpression = this.Collection.Build(tempBuildContext, scriptContext, options);
			if (listExpression.Type == typeof(object))
			{
				listExpression = Expression.Convert(listExpression, typeof(IEnumerable));
			}

			// ForeachKey 模式：IDictionary 遍历 Keys，IList 使用 for 遍历索引
			if (this.ForeachKey)
			{
				return BuildForeachKey(tempBuildContext, scriptContext, options, listExpression);
			}

			var getEnumeratorMethod = listExpression.Type == typeof(IEnumerable) ?
					typeof(IEnumerable).GetMethod("GetEnumerator") :
					typeof(IEnumerable<>).MakeGenericType(ScriptUtils.GetElementType(listExpression.Type)).GetMethod("GetEnumerator");
			var getEnumerator = Expression.Call(listExpression, getEnumeratorMethod);
			var enumerator = Expression.Variable(getEnumerator.Method.ReturnType);
			var currentProperty = enumerator.Type.GetProperty("Current");
			var moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");

			if (this.VarDefines != null)
			{
				// VarDefines 解构模式
				var itemType = currentProperty.PropertyType;
				var elementTypes = new List<Type>();
				var isTuple = false;
				var isValueTuple = false;

				if (itemType.IsGenericType)
				{
					isTuple = itemType.Name.StartsWith("Tuple`");
					isValueTuple = itemType.Name.StartsWith("ValueTuple`");
					if (isTuple || isValueTuple)
					{
						elementTypes.AddRange(itemType.GetGenericArguments());
					}
				}

				// 为每个变量定义创建变量表达式
				var itemVars = new List<ParameterExpression>();
				for (int i = 0; i < this.VarDefines.Count; i++)
				{
					var elementType = i < elementTypes.Count ? elementTypes[i] : typeof(object);
					var vd = this.VarDefines[i];
					if (IsNiming(vd))
					{
						itemVars.Add(null);
					}
					else
					{
						vd.SystemType = elementType;
						var itemVar = (ParameterExpression)vd.Build(tempBuildContext, scriptContext, options);
						itemVars.Add(itemVar);
					}
				}

				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);
				if (body != null)
				{
					body = bodyBuildContext.BuildBlock(scriptContext, options, body);
				}

				// 构建解构赋值表达式列表
				var assignExpressions = new List<Expression>();
				var itemVar2 = Expression.Variable(itemType, "_item");
				assignExpressions.Add(Expression.Assign(itemVar2, Expression.Property(enumerator, currentProperty)));

				for (int i = 0; i < this.VarDefines.Count; i++)
				{
					if (itemVars[i] == null) continue;
					Expression memberAccess;
					if (isTuple)
					{
						var memberName = "Item" + (i + 1);
						var prop = itemType.GetProperty(memberName);
						memberAccess = Expression.Property(itemVar2, prop);
					}
					else if (isValueTuple)
					{
						var memberName = "Item" + (i + 1);
						var field = itemType.GetField(memberName);
						memberAccess = Expression.Field(itemVar2, field);
					}
					else if (!typeof(IList).IsAssignableFrom(itemType))
					{
						memberAccess = itemVar2;
						assignExpressions.Add(Expression.Assign(itemVars[i], memberAccess));
						break;
					}
					else
					{
						// 非 Tuple 类型，使用索引访问 IList
						memberAccess = Expression.Call(
							Expression.Convert(itemVar2, typeof(IList<object>)),
							typeof(IList<object>).GetMethod("get_Item"),
							Expression.Constant(i));
					}
					assignExpressions.Add(Expression.Assign(itemVars[i], memberAccess));
				}

				Expression loop;
				if (body == null) loop = Expression.Empty();
				else
				{
					var loopBody = Expression.Block(
						new[] { itemVar2 },
						Expression.IfThenElse(
							Expression.Call(enumerator, moveNextMethod),
							Expression.Block(assignExpressions.Concat(new[] { body, Expression.Label(continueLabel) })),
							Expression.Break(breakLabel)
						));
					loop = Expression.Loop(loopBody, breakLabel);
				}
				return Expression.Block(new[] { enumerator },
					tempBuildContext.BuildBlock(scriptContext, options, Expression.Assign(enumerator, getEnumerator), loop));
			}
			else
			{
				// 单变量模式
				Expression itemVar = null;
				if (!IsNiming(this.VarDefine))
				{
					this.VarDefine.SystemType = currentProperty.PropertyType;
					itemVar = this.VarDefine.Build(tempBuildContext, scriptContext, options);
				}
				//
				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);
				var bodyBlock = itemVar == null ?
					bodyBuildContext.BuildBlock(scriptContext, options,
							body,
							Expression.Label(continueLabel)) :
					bodyBuildContext.BuildBlock(scriptContext, options,
							Expression.Assign(itemVar, Expression.Property(enumerator, currentProperty)),
							body,
							Expression.Label(continueLabel));
				//
				var loopBody = Expression.Block(
					Expression.IfThenElse(
						Expression.Call(enumerator, moveNextMethod),
						bodyBlock,
						Expression.Break(breakLabel)
					));
				var loop = Expression.Loop(loopBody, breakLabel);
				return Expression.Block(new[] { enumerator },
					tempBuildContext.BuildBlock(scriptContext, options, Expression.Assign(enumerator, getEnumerator), loop));
			}
		}

		private Expression BuildForeachKey(BuildContext tempBuildContext, ScriptContext scriptContext, BuildOptions options, Expression listExpression)
		{
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();

			// 根据 listExpression.Type 静态类型判断是 IDictionary 还是 IList
			var listType = listExpression.Type;
			var isDictionary = typeof(IDictionary).IsAssignableFrom(listType);
			var isDictionary2 = typeof(IDictionary<string, object>).IsAssignableFrom(listType);
			var isList = typeof(IList).IsAssignableFrom(listType);

			if (isDictionary || isDictionary2)
			{
				// IDictionary 分支：遍历 Keys
				var dictType = isDictionary2 ? typeof(IDictionary<string, object>) : typeof(IDictionary);
				var enumerableType = isDictionary2 ? typeof(IEnumerable<string>) : typeof(IEnumerable);
				var enumeratorType = isDictionary2 ? typeof(IEnumerator<string>) : typeof(IEnumerator);
				var keysProperty = dictType.GetProperty("Keys");
				var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");

				var dictKeysExpr = Expression.Property(listExpression, keysProperty);
				var dictEnumeratorVar = Expression.Variable(enumeratorType);
				var getDictEnumerator = Expression.Call(dictKeysExpr, getEnumeratorMethod);
				var dictCurrentProperty = enumeratorType.GetProperty("Current");
				var moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");

				// 创建变量表达式
				this.VarDefine.SystemType = isDictionary2 ? typeof(string) : typeof(object);
				var itemVar = (ParameterExpression)this.VarDefine.Build(tempBuildContext, scriptContext, options);
				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);
				var dictLoopBody = Expression.Block(
					Expression.IfThenElse(
						Expression.Call(dictEnumeratorVar, moveNextMethod),
						bodyBuildContext.BuildBlock(scriptContext, options,
							Expression.Assign(itemVar, Expression.Property(dictEnumeratorVar, dictCurrentProperty)),
							body,
							Expression.Label(continueLabel)),
						Expression.Break(breakLabel)
					));
				var dictLoop = Expression.Loop(dictLoopBody, breakLabel);
				return Expression.Block(new[] { itemVar, dictEnumeratorVar },
					Expression.Assign(dictEnumeratorVar, getDictEnumerator),
					Expression.Block(dictLoop));
			}
			else if (isList)
			{
				// IList 分支：使用 for 遍历索引
				var countProperty = typeof(ICollection).GetProperty("Count");
				var listCount = Expression.Property(listExpression, countProperty);

				// 创建变量表达式
				this.VarDefine.SystemType = typeof(int);
				var itemVar = (ParameterExpression)this.VarDefine.Build(tempBuildContext, scriptContext, options);
				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);
				var listLoopBody = Expression.Block(
					Expression.IfThenElse(
						Expression.LessThan(itemVar, listCount),
						bodyBuildContext.BuildBlock(scriptContext, options,
							body,
							Expression.Label(continueLabel),
							Expression.PostIncrementAssign(itemVar)),
						Expression.Break(breakLabel)
					));
				var listLoop = Expression.Loop(listLoopBody, breakLabel);
				return Expression.Block(new[] { itemVar },
					Expression.Assign(itemVar, Expression.Constant(0)),
					Expression.Block(listLoop));
			}

			throw new NotSupportedException("ForeachKey is only supported for IDictionary and IList");
		}

		private static bool IsNiming(VariableNode varNode)
		{
			return varNode == null || string.IsNullOrEmpty(varNode.Name) || varNode.Name == "_";
		}

		public override void Clear()
		{
			base.Clear();

			if (this.VarDefines != null)
			{
				foreach (var vd in this.VarDefines)
				{
					PoolManage.Return(vd);
				}
			}
			PoolManage.Return(this.VarDefine);
			PoolManage.Return(this.Collection);
			PoolManage.Return(this.Body);

			this.VarDefine = null;
			this.VarDefines = null;
			this.Collection = null;
			this.Body = null;
			this.ForeachKey = false;
		}
	}
}
