using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AScript.Lang.Go.Nodes
{
	/// <summary>
	/// go多线程执行
	/// </summary>
	public class GoGoNode : AScript.Nodes.TreeNode
	{
		public AScript.Nodes.ITreeNode Body { get; set; }

		//private static readonly ConstructorInfo Constructor_Action = typeof(Action).GetConstructor()
		private static readonly MethodInfo Method_Task_Run = typeof(Task).GetMethod("Run", new[] { typeof(Action) });
		private static readonly MethodInfo Method_Task_Run_T = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(a=>
		{
			if (a.Name != "Run") return false;
			if (!a.IsGenericMethod) return false;
			var parameters = a.GetParameters();
			if (parameters.Length != 1) return false;
			var p0 = parameters[0];
			if (!p0.ParameterType.IsGenericParameter) return false;
			return p0.ParameterType.GetGenericArguments()[0] == a.GetGenericArguments()[0];
		});

		public override Expression Build(BuildContext buildContext, ScriptContext scriptContext, BuildOptions options)
		{
			var tmpBuildContext = new BuildContext(buildContext)
			{
				ScriptContextParameter = Expression.Variable(typeof(ScriptContext)),
				RewriteLocalVariables = false,
				IsMain = true
			};
			var body = this.Body.Build(tmpBuildContext, scriptContext, options);
			var lambda = tmpBuildContext.Build(scriptContext, options, body);
			// Task.Run(()=>...)
			if (lambda.ReturnType == typeof(void))
			{
				return Expression.Call(Method_Task_Run, lambda);
			}
			else
			{
				var genericMethod = Method_Task_Run_T.MakeGenericMethod(lambda.ReturnType);
				return Expression.Call(genericMethod, lambda);
			}
		}

		public override object Eval(ScriptContext context, BuildOptions options, EvalControl control, out Type returnType)
		{
			returnType = typeof(Task<object>);
			return Task.Run(() => this.Body.Eval(context, options, new EvalControl(), out _));
		}
	}
}
