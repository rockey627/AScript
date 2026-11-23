using AScript.Nodes;
using System;
using System.Linq.Expressions;

namespace AScript.Operators
{
	/// <summary>
	/// 取址操作：&amp;a
	/// </summary>
	public class PointerAddressOperator : IFunctionEvaluator, IFunctionBuilder
	{
		public static readonly PointerAddressOperator Instance = new PointerAddressOperator();

		public void Build(FunctionBuildArgs e)
		{
			if (e.GetArgsCount() != 1 || !e.IsPrefix) return;

			var a0 = e.Args[0];
			if (a0 is VariableNode v0)
			{
				var addr = Pointer.CreateVariableAddress(v0.Name);
				var expr = v0.Build(e.BuildContext, e.ScriptContext, e.Options);
				var pointerType = typeof(Pointer<>).MakeGenericType(expr.Type);
				var pointer = Expression.Constant(Activator.CreateInstance(pointerType, addr), pointerType);
				//e.Result = Expression.New(ScriptUtils.Constructor_Pointer, Expression.Constant(expr.Type), Expression.Constant(addr));
				// 编译模式下变量是局部变量，指针在运行期是按名称到上下文里取值的，
				// 这里先把变量同步到上下文，保证指针即使传给其他函数也能正确解引用
				var sync = BuildSyncVariable(e, v0.Name, expr);
				e.Result = sync == null ? (Expression)pointer : Expression.Block(sync, pointer);
				return;
			}
		}

		/// <summary>
		/// 把变量同步到脚本上下文
		/// </summary>
		private static Expression BuildSyncVariable(FunctionBuildArgs e, string name, Expression value)
		{
			// 只读变量不能写回
			if (e.BuildContext.VariableModifiers.TryGetValue(name, out var modifier)
				&& Modifiers.IsReadOnly(modifier)) return null;
			var scriptContext = e.BuildContext.GetScriptContextParameter(true);
			var valueExpr = value.Type.IsValueType ? Expression.Convert(value, typeof(object)) : value;
			var typeExpr = value.Type.IsValueType ? ScriptUtils.Constant_null_Type : Expression.Constant(value.Type);
			return Expression.Call(
				scriptContext,
				ScriptUtils.Method_ScriptContext_SetTempVar,
				Expression.Constant(name),
				valueExpr,
				typeExpr,
				ScriptUtils.Constant_true);
		}

		public void Eval(FunctionEvalArgs e)
		{
			if (e.GetArgsCount() != 1 || !e.IsPrefix) return;

			var a0 = e.Args[0];
			if (a0 is VariableNode v0)
			{
				var addr = Pointer.CreateVariableAddress(v0.Name);
				var type = e.Context.GetVarType(v0.Name) ?? typeof(object);
				var pointerType = typeof(Pointer<>).MakeGenericType(type);
				e.SetResult(Activator.CreateInstance(pointerType, addr));
				return;
			}
		}
	}
}
