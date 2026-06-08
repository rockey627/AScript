using AScript.Lang.Sql.Nodes;
using AScript.Nodes;
using AScript.Syntaxs;
using System;
using System.Collections.Generic;
using System.Data;

namespace AScript.Lang.Sql.TokenHandlers
{
	public class SqlCreateTokenHandler : ITokenHandler
	{
		public static readonly SqlCreateTokenHandler Instance = new SqlCreateTokenHandler();

		private static readonly HashSet<string> _ProcedureEndTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end" };

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			e.End = true;
			if (e.TreeBuilder.IsFullStatement())
			{
				e.TokenReader.Push(e.CurrentToken);
				return;
			}

			var actionToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if ("procedure".Equals(actionToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				e.CurrentToken = actionToken.Value;
				BuildProcedure(analyzer, e);
				return;
			}
			if ("function".Equals(actionToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				e.CurrentToken = actionToken.Value;
				BuildFunction(analyzer, e);
				return;
			}
			if ("table".Equals(actionToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				e.CurrentToken = actionToken.Value;
				BuildTable(analyzer, e);
				return;
			}

			throw new Exceptions.ScriptAnalyzingException($"invalid expression '{actionToken.Value.Value}' at ({actionToken.Value.Line},{actionToken.Value.Column})");
		}

		/// <summary>
		/// create procedure 存储过程名称(参数1 类型, 参数2 类型)
		/// begin
		/// 
		/// end
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		private void BuildProcedure(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			var nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
			}
			var args = e.Ignore ? null : new List<DefineVarNode>();
			if (nextToken.Value.IsSymbol("("))
			{
				// mysql 解析参数
				nextToken = e.TokenReader.Read();
				if (nextToken.HasValue && nextToken.Value.IsSymbol(")"))
				{
					nextToken = e.TokenReader.Read();
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
					while (true)
					{
						var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						if (varToken.Value.IsSymbol("in", StringComparison.OrdinalIgnoreCase)
							|| varToken.Value.IsSymbol("out", StringComparison.OrdinalIgnoreCase))
						{
							varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						}
						var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						args?.Add(PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (nextToken.Value.IsSymbol(",")) continue;
						if (nextToken.Value.IsSymbol(")"))
						{
							nextToken = e.TokenReader.Read();
							if (!nextToken.HasValue)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
							}
							break;
						}
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
				}
			}
			else
			{
				// sqlserver 参数解析
				if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
				{
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
				}
				else
				{
					e.TokenReader.Push(nextToken.Value);
					while (true)
					{
						var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
						if (args != null)
						{
							args.Add(PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
						}
						nextToken = e.TokenReader.Read();
						if (nextToken.HasValue && nextToken.Value.IsSymbol("output", StringComparison.OrdinalIgnoreCase))
						{
							nextToken = e.TokenReader.Read();
						}
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (nextToken.Value.IsSymbol(",")) continue;
						if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
						{
							nextToken = e.TokenReader.Read();
							break;
						}
						throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
					}
				}
			}
			// 
			ITreeNode body;
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			if (nextToken.Value.IsSymbol("begin", StringComparison.OrdinalIgnoreCase))
			{
				body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ProcedureEndTokens);
				analyzer.ValidateNextToken(e.TokenReader, "end", StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
				body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			}
			// 
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new DefineFuncNode { Name = nameToken.Value.Value, Args = args.ToArray(), Body = body });
			}
		}

		/// <summary>
		/// create function 函数名称(参数1 类型, 参数2 类型)
		/// returns 返回类型
		/// begin
		/// 
		/// end
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		private void BuildFunction(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			analyzer.ValidateNextToken(e.TokenReader, "(");
			var args = e.Ignore ? null : new List<DefineVarNode>();
			// 解析参数
			var nextToken = e.TokenReader.Read();
			if (nextToken.HasValue && nextToken.Value.IsSymbol(")"))
			{
				//nextToken = e.TokenReader.Read();
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
				while (true)
				{
					var varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					//if (varToken.Value.IsSymbol("in", StringComparison.OrdinalIgnoreCase))
					//{
					//	varToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					//}
					var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
					args?.Add(PoolManage.CreateDefineVarNode(varToken.Value.Value, typeToken.Value.Value));
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
					if (nextToken.Value.IsSymbol(",")) continue;
					if (nextToken.Value.IsSymbol(")"))
					{
						//nextToken = e.TokenReader.Read();
						//if (!nextToken.HasValue)
						//{
						//	throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						//}
						break;
					}
					throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
			}
			// 

			ITreeNode body;
			analyzer.ValidateNextToken(e.TokenReader, "returns", StringComparison.OrdinalIgnoreCase);
			var returnTypeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			nextToken = e.TokenReader.Read();
			if (!nextToken.HasValue)
			{
				throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
			}
			if (nextToken.Value.IsSymbol("as", StringComparison.OrdinalIgnoreCase))
			{
				nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
				}
			}
			var createFullOptions = (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
			if (nextToken.Value.IsSymbol("begin", StringComparison.OrdinalIgnoreCase))
			{
				body = analyzer.BuildMultiStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore, _ProcedureEndTokens);
				analyzer.ValidateNextToken(e.TokenReader, "end", StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				e.TokenReader.Push(nextToken.Value);
				body = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
			}
			// 
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, new DefineFuncNode { Name = nameToken.Value.Value, ReturnType = returnTypeToken.Value.Value, Args = args.ToArray(), Body = body });
			}
		}

		/// <summary>
		/// CREATE TABLE IF NOT EXISTS 表名 (字段1 数据类型，字段2 数据类型)
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="e"></param>
		private void BuildTable(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			bool checkNotExists = false;
			var tableToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
			if ("if".Equals(tableToken.Value.Value, StringComparison.OrdinalIgnoreCase))
			{
				analyzer.ValidateNextToken(e.TokenReader, "not", StringComparison.OrdinalIgnoreCase);
				analyzer.ValidateNextToken(e.TokenReader, "exists", StringComparison.OrdinalIgnoreCase);
				tableToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				checkNotExists = true;
			}

			analyzer.ValidateNextToken(e.TokenReader, "(");
			var columns = e.Ignore ? null : new List<DataColumn>();
			while (true)
			{
				var nameToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				var typeToken = analyzer.ValidateNextToken(e.TokenReader, ETokenType.Word);
				var type = e.ScriptContext.EvalType(typeToken.Value.Value) ?? throw new Exceptions.ScriptAnalyzingException($"unkown type '{typeToken.Value.Value}'");
				int? length = null;
				bool autoIncrement = false;
				long? autoIncrementSeed = null;
				long? autoIncrementStep = null;
				bool? unique = null;
				object defaultValue = null;
				bool nullable = true;
				var nextToken = e.TokenReader.Read();
				if (!nextToken.HasValue)
				{
					throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
				}
				if (nextToken.Value.IsSymbol("("))
				{
					// 长度
					length = int.Parse(analyzer.ValidateNextToken(e.TokenReader, ETokenType.Number).Value.Value);
					analyzer.ValidateNextToken(e.TokenReader, ")");
					nextToken = e.TokenReader.Read();
					if (!nextToken.HasValue)
					{
						throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
					}
				}
				while (true)
				{
					if (nextToken.Value.IsSymbol("not", StringComparison.OrdinalIgnoreCase))
					{
						analyzer.ValidateNextToken(e.TokenReader, "null", StringComparison.OrdinalIgnoreCase);
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						nullable = false;
					}
					else if (nextToken.Value.IsSymbol("null", StringComparison.OrdinalIgnoreCase))
					{
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
					}
					else if (nextToken.Value.IsSymbol("AUTO_INCREMENT", StringComparison.OrdinalIgnoreCase))
					{
						// MySql自增
						autoIncrement = true;
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
					}
					else if (nextToken.Value.IsSymbol("IDENTITY", StringComparison.OrdinalIgnoreCase))
					{
						autoIncrement = true;
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						if (nextToken.Value.IsSymbol("("))
						{
							autoIncrementSeed = long.Parse(analyzer.ValidateNextToken(e.TokenReader, ETokenType.Number).Value.Value);
							analyzer.ValidateNextToken(e.TokenReader, ",");
							autoIncrementStep = long.Parse(analyzer.ValidateNextToken(e.TokenReader, ETokenType.Number).Value.Value);
							analyzer.ValidateNextToken(e.TokenReader, ")");
							nextToken = e.TokenReader.Read();
							if (!nextToken.HasValue)
							{
								throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
							}
						}
					}
					else if (nextToken.Value.IsSymbol("PRIMARY", StringComparison.OrdinalIgnoreCase))
					{
						analyzer.ValidateNextToken(e.TokenReader, "key", StringComparison.OrdinalIgnoreCase);
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
						unique = true;
					}
					else if (nextToken.Value.IsSymbol("default", StringComparison.OrdinalIgnoreCase))
					{
						var createFullOptions = (e.Options.CompileMode ?? ECompileMode.None) == ECompileMode.None || (e.Options.CreateFullTreeNode ?? false) ? e.Options : new BuildOptions(e.Options) { CreateFullTreeNode = true };
						var defaultNode = analyzer.BuildOneStatement(e.BuildContext, e.ScriptContext, createFullOptions, e.TokenReader, e.Control, e.Ignore);
						if (defaultNode != null)
						{
							defaultValue = defaultNode.Eval(e.ScriptContext, e.Options, e.Control, out _);
						}
						nextToken = e.TokenReader.Read();
						if (!nextToken.HasValue)
						{
							throw new Exceptions.ScriptAnalyzingException($"invalid expression near '{e.CurrentToken.Value}' at ({e.TokenReader.CharReader.CurrentLine},{e.TokenReader.CharReader.CurrentColumn})");
						}
					}
					else if (nextToken.Value.IsSymbol(",")) break;
					else if (nextToken.Value.IsSymbol(")")) break;
					else throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
				}
				// 
				if (columns != null)
				{
					var column = new DataColumn(nameToken.Value.Value, type) { AllowDBNull = nullable };
					if (length.HasValue) column.MaxLength = length.Value;
					if (autoIncrement)
					{
						column.AutoIncrement = autoIncrement;
						if (!autoIncrementSeed.HasValue) autoIncrementSeed = 1;
					}
					if (autoIncrementSeed.HasValue) column.AutoIncrementSeed = autoIncrementSeed.Value;
					if (autoIncrementStep.HasValue) column.AutoIncrementStep = autoIncrementStep.Value;
					if (unique.HasValue) column.Unique = unique.Value;
					if (defaultValue != null) column.DefaultValue = defaultValue;
					columns.Add(column);
				}
				if (nextToken.Value.IsSymbol(",")) continue;
				if (nextToken.Value.IsSymbol(")")) break;
				throw new Exceptions.ScriptAnalyzingException($"invalid expression '{nextToken.Value.Value}' at ({nextToken.Value.Line},{nextToken.Value.Column})");
			}

			if (!e.Ignore)
			{
				var createTableNode = new SqlCreateTableNode { Name = tableToken.Value.Value, CheckNotExists = checkNotExists, Columns = columns };
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, createTableNode);
			}
		}
	}
}
