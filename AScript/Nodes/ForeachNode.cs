using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AScript.Nodes
{
	public class ForeachNode : TreeNode
	{
		public DefineVarNode VarDefine { get; set; }
		public IList<DefineVarNode> VarDefines { get; set; }
		public ITreeNode Collection { get; set; }
		public ITreeNode Body { get; set; }

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl controll, out Type returnType)
		{
			if (this.VarDefine == null && this.VarDefines == null)
			{
				throw new Exception("require variable define in foreach statement");
			}
			if (this.Collection == null)
			{
				throw new Exception("require collection in foreach statement");
			}
			// 计算集合
			var listResult = this.Collection.Eval(context, options, controll, out var listType);
			if (listResult == null)
			{
				throw new NullReferenceException("foreach collection is null");
			}
			if (!(listResult is IEnumerable en))
			{
				throw new Exception($"invalid foreach collection {listType}");
			}
			//
			object bodyResult = null;
			Type bodyType = null;
			if (this.Body != null)
			{
				var tempContext = ScriptContext.Create(context);
				var tempController = new EvalControl(controll, true);
				// 定义变量
				if (this.VarDefines != null)
				{
					foreach (var vd in this.VarDefines)
					{
						vd.Eval(tempContext, options, out _);
					}
				}
				else
				{
					this.VarDefine.Eval(tempContext, options, out _);
				}
				// 循环
				foreach (var item in en)
				{
					if (this.VarDefines != null)
					{
						// 解构列表项赋值到各个变量
						var itemList = new List<object>();
						if (item is IList list)
						{
							foreach (var i in list)
							{
								itemList.Add(i);
							}
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
									foreach (var prop in itemType.GetProperties())
									{
										itemList.Add(prop.GetValue(item));
									}
								}
#if NETSTANDARD
								else if (genericType.Name.StartsWith("ValueTuple`"))
								{
									foreach (var field in itemType.GetFields())
									{
										itemList.Add(field.GetValue(item));
									}
								}
#endif
							}
						}
						if (itemList == null)
						{
							throw new Exception($"cannot unpack item of type {item?.GetType()} into {this.VarDefines.Count} variables");
						}
						if (itemList.Count < this.VarDefines.Count)
						{
							throw new Exception($"not enough values to unpack (expected {this.VarDefines.Count}, got {itemList.Count})");
						}
						for (int i = 0; i < this.VarDefines.Count; i++)
						{
							tempContext.SetVar(this.VarDefines[i].Name, itemList[i], null);
						}
					}
					else
					{
						this.VarDefine.Eval(tempContext, options, out var varType);
						tempContext.SetVar(this.VarDefine.Name, item, item == null ? varType : null);
					}
					bodyResult = this.Body.Eval(ScriptContext.Create(tempContext), options, tempController, out bodyType);
					if (tempController.Terminal || tempController.Break) break;
					tempController.Continue = false;
				}
			}
			returnType = bodyType;
			return bodyResult;
		}

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tempBuildContext = new BuildContext(buildContext);
			var breakLabel = Expression.Label();
			var continueLabel = Expression.Label();
			var listExpression = this.Collection.Build(tempBuildContext, scriptContext, options);

			var getEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(ScriptUtils.GetElementType(listExpression.Type)).GetMethod("GetEnumerator");
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
					var genericTypeDefinition = itemType.GetGenericTypeDefinition();
					isTuple = genericTypeDefinition.Name.StartsWith("Tuple`");
					isValueTuple = genericTypeDefinition.Name.StartsWith("ValueTuple`");
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
					vd.SystemType = elementType;
					var itemVar = (ParameterExpression)vd.Build(tempBuildContext, scriptContext, options);
					itemVars.Add(itemVar);
				}

				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);

				// 构建解构赋值表达式列表
				var assignExpressions = new List<Expression>();
				var itemVar2 = Expression.Variable(itemType, "_item");
				assignExpressions.Add(Expression.Assign(itemVar2, Expression.Property(enumerator, currentProperty)));

				for (int i = 0; i < this.VarDefines.Count; i++)
				{
					var memberName = "Item" + (i + 1);
					Expression memberAccess;
					if (isTuple)
					{
						var prop = itemType.GetProperty(memberName);
						memberAccess = Expression.Property(itemVar2, prop);
					}
					else if (isValueTuple)
					{
						var field = itemType.GetField(memberName);
						memberAccess = Expression.Field(itemVar2, field);
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

				var loopBody = Expression.Block(
					new[] { itemVar2 },
					Expression.IfThenElse(
						Expression.Call(enumerator, moveNextMethod),
						Expression.Block(assignExpressions.Concat(new[] { body, Expression.Label(continueLabel) })),
						Expression.Break(breakLabel)
					));
				var loop = Expression.Loop(loopBody, breakLabel);
				return Expression.Block(new[] { enumerator },
					tempBuildContext.BuildBlock(scriptContext, options, Expression.Assign(enumerator, getEnumerator), loop));
			}
			else
			{
				// 单变量模式
				this.VarDefine.SystemType = currentProperty.PropertyType;
				var itemVar = this.VarDefine.Build(tempBuildContext, scriptContext, options);
				//
				var bodyBuildContext = new BuildContext(tempBuildContext)
				{
					ContinueLabel = continueLabel,
					BreakLabel = breakLabel
				};
				var body = this.Body.Build(bodyBuildContext, scriptContext, options);
				//
				var loopBody = Expression.Block(
					Expression.IfThenElse(
						Expression.Call(enumerator, moveNextMethod),
						bodyBuildContext.BuildBlock(scriptContext, options,
							Expression.Assign(itemVar, Expression.Property(enumerator, currentProperty)),
							body,
							Expression.Label(continueLabel)),
						Expression.Break(breakLabel)
					));
				var loop = Expression.Loop(loopBody, breakLabel);
				return Expression.Block(new[] { enumerator },
					tempBuildContext.BuildBlock(scriptContext, options, Expression.Assign(enumerator, getEnumerator), loop));
			}
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
		}
	}
}
