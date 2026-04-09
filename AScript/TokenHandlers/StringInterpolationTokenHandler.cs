using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AScript.TokenHandlers
{
	/// <summary>
	/// 字符串内插功能：$"hello {name}"
	/// </summary>
	public class StringInterpolationTokenHandler : ITokenHandler
	{
		public static readonly StringInterpolationTokenHandler Instance = new StringInterpolationTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var _reader = e.TokenReader.CharReader;
			var c = _reader.Read();
			if (!c.HasValue) return;
			if (c.Value != '"' && c.Value != '\'')
			{
				_reader.Push(c.Value);
				return;
			}

			e.IsHandled = true;

			List<Expression> exprs = null;
			char startChar = c.Value;
			var _buffer = new StringBuilder();
			bool prevEscape = false;
			c = _reader.Read();
			while (c.HasValue)
			{
				if (c == '\\')
				{
					if (prevEscape)
					{
						prevEscape = false;
						_buffer.Append(c);
					}
					else
					{
						prevEscape = true;
					}
					c = _reader.Read();
					continue;
				}
				if (c == startChar && !prevEscape)
				{
					break;
				}

				if (!prevEscape)
				{
					if (c == '{')
					{
						c = _reader.Read();
						if (c == '{')
						{
							_buffer.Append(c);
							c = _reader.Read();
							continue;
						}

						_reader.Push(c.Value);
						// 插值计算
						var node = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, e.Options, e.TokenReader, e.Control, e.Ignore);
						if (!e.Options.CompileMode.HasValue || e.Options.CompileMode.Value != ECompileMode.All)
						{
							var obj = node.Eval(e.ScriptContext, e.Options, e.Control, out _);
							if (obj != null) _buffer.Append(obj.ToString());
						}
						else
						{
							var v = node.Build(e.BuildContext, e.ScriptContext, e.Options);
							Expression vs;
							if (v.Type.IsValueType)
							{
								vs = Expression.Call(v, ExpressionUtils.Method_Object_ToString);
							}
							else
							{
								var testNull = Expression.ReferenceEqual(v, ExpressionUtils.Constant_null);
								vs = Expression.Condition(testNull, ExpressionUtils.Constant_string_empty, Expression.Call(v, ExpressionUtils.Method_Object_ToString));
							}
							if (exprs == null) exprs = new List<Expression>();
							if (_buffer.Length > 0)
							{
								exprs.Add(Expression.Constant(_buffer.ToString()));
								_buffer.Clear();
							}
							exprs.Add(vs);
						}
						PoolManage.Return(node);
						analyzer.ValidateNextToken(e.TokenReader, "}");
						c = _reader.Read();
						continue;
					}

					if (c.Value == '}')
					{
						c = _reader.Read();
						_buffer.Append('}');
						if (c.Value == '}')
						{
							c = _reader.Read();
						}
						continue;
					}

					_buffer.Append(c);
					c = _reader.Read();
					continue;
				}

				prevEscape = false;
				if (c == startChar)
				{
					_buffer.Append(c);
				}
				else if (c == 'n')
				{
					_buffer.Append('\n');
				}
				else if (c == 'r')
				{
					_buffer.Append('\r');
				}
				else if (c == 't')
				{
					_buffer.Append('\t');
				}
				else throw new Exception("unknown string escape:\\" + c);

				c = _reader.Read();
			}
			if (!e.Options.CompileMode.HasValue || e.Options.CompileMode.Value != ECompileMode.All)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateObjectData(_buffer.ToString(), typeof(string)));
			}
			else
			{
				if (exprs == null || exprs.Count == 0)
				{
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateExpressionNode(Expression.Constant(_buffer.ToString())));
				}
				else
				{
					if (_buffer.Length > 0)
					{
						exprs.Add(Expression.Constant(_buffer.ToString()));
					}
					var a = Expression.NewArrayInit(typeof(string), exprs);
					e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, PoolManage.CreateExpressionNode(Expression.Call(null, ExpressionUtils.Method_String_Concat_list, a)));
				}
			}
		}

	}
}
