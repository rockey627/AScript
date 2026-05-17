using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using IronPython.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AScript.Test.Consoles
{
	internal class Program
	{
		public event EventHandler<EventArgs> Handled;
		public event MyHandle Handle2;

		public delegate void MyHandle(object sender, EventArgs e);

		protected virtual void OnHandled(EventArgs e)
		{
			this.Handled?.Invoke(this, e);
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			//Test01_Benchmark();
			//Test02();
			//Test03();
			//Test04();
			//Test05();
			//Test06();
			//Test07();
			//Test08_Z_1();
			//Test09_Antlr4();
			//Test10_Lambda();
			//Test11_Convert();
			//Test12_IronPython();
			//Test13_Convert();
			//Test14_Eval();
			//Test15();
			//Test16();
			//Test17();
			//Test18_CSharpScript();
			//Test19();
			//Test20();
			//Test21_ExpandoObject();
			//Test22();
			//Test23();
			Test24_Sqlite();
			Console.WriteLine("end");
			Console.ReadLine();
		}

		static void Test24_Sqlite()
		{
			using (var context = new TestSqliteContext())
			{
				context.Database.Migrate();

				context.Persons.ExecuteDelete();
				context.AddressInfos.ExecuteDelete();
				context.SaveChanges();

				context.Persons.AddRange(new[]
				{
					new Person{ Id = "1001", Name = "tom", Age = 20 },
					new Person{ Id = "1002", Name = "san", Age = 25 },
					new Person{ Id = "1003", Name = "tony", Age = 18 },
					new Person{ Id = "1004", Name = "tim", Age = 25 }
				});
				context.AddressInfos.AddRange(new[]
				{
					new AddressInfo{UserId = "1002", Address = "a" },
					new AddressInfo{UserId = "1004", Address = "b" },
					new AddressInfo{UserId = "1005", Address = "c" },
				});
				context.SaveChanges();

				//string s = @"
				//var persons = context.Persons;
				//var q = from a in persons
				//		orderby a.Age
				//		select new Person { a.Name, a.Age };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.AddType<Person>();
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IList>(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//string s = @"
				//var persons = context.Persons;
				//var q = from a in persons
				//		group a by a.Age into g
				//		select new { g.Key, Count1 = g.Count(), Total = g.Sum(k=>k.Age) };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IList>(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				string s = @"
				var q = from p in context.Persons
						join a in context.AddressInfos on p.Id equals a.UserId into aa
						from a in aa.DefaultIfEmpty()
						select new { p.Id, p.Name, p.Age, MyAddress = a.Address };
				q.ToList();
				";
				var script = new Script();
				script.Context.SetVar("context", context);
				var list = script.Eval<IList>(s);
				Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//string s = @"
				//var q = from a in context.AddressInfos
				//		join p in context.Persons on a.UserId equals p.Id into pp
				//		from p in pp.DefaultIfEmpty()
				//		select new { a.Id, a.Address, UserId=p.Id, p.Name, Age = (int?)p.Age };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.SetVar("context", context);
				//var list = script.Eval(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//var persons = context.Persons;
				//var q = from a in persons
				//		group a by a.Age into g
				//		select new { g.Key, Count1 = g.Count(), Total = g.Sum(k => k.Age) };
				//var list = q.ToList();
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//var cc = new { Name = "h" };
				//Expression<Func<Person, object>> f = p => new Person { Name = "hello" + p.Age };
				//Console.WriteLine(f.ToString());
			}
		}

		static void Test23_AnonymousTypeManager()
		{
			var a = new { Name = "tom", Age = 18 };
			var at = a.GetType();
			var atf = at.GetGenericTypeDefinition();
			Console.WriteLine(at);
			var anonymousTypes = new AnonymousTypeManager();
			var anonType = anonymousTypes.CreateType(
				new[] { "Name", "Age" },
				new[] { typeof(string), typeof(int) });
			var anonfType = anonType.GetGenericTypeDefinition();
			Console.WriteLine(anonType);
		}

		static void Test22()
		{
			var a = new { Name = "hh", Age = 18 };
			Console.WriteLine(a.GetType());
			var a2 = new { Name = 30, Age = "20" };
			Console.WriteLine(a2.GetType());
			var a3 = new { Age = 20, Name = "jim" };
			Console.WriteLine(a3.GetType());
			var dynamicType = DynamicClassBuilder.BuildDynamicType("DynamicPerson", ("Name", typeof(string)), ("Age", typeof(int)));
			dynamic instance = Activator.CreateInstance(dynamicType, new object[] { "Alice", 30 });
			Console.WriteLine(instance.GetType());
			//instance.Age = 20;
			Console.WriteLine(instance.Name + ":" + instance.Age);
		}

		public class DynamicClassBuilder
		{
			public static Type BuildDynamicType(string typeName, params (string Name, Type Type)[] properties)
			{
				var assemblyName = new AssemblyName(typeName);
				var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
				var moduleBuilder = assemblyBuilder.DefineDynamicModule(typeName);
				var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

				var fieldBuilders = new List<FieldBuilder>();
				foreach (var (name, type) in properties)
				{
					var fieldBuilder = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
					fieldBuilders.Add(fieldBuilder);
					var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);
					var getMethodBuilder = typeBuilder.DefineMethod("get_" + name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, type, Type.EmptyTypes);
					var getIl = getMethodBuilder.GetILGenerator();
					getIl.Emit(OpCodes.Ldarg_0);
					getIl.Emit(OpCodes.Ldfld, fieldBuilder);
					getIl.Emit(OpCodes.Ret);
					propertyBuilder.SetGetMethod(getMethodBuilder);

					//var setMethodBuilder = typeBuilder.DefineMethod("set_" + name, MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), new[] { type });
					//var setIl = setMethodBuilder.GetILGenerator();
					//setIl.Emit(OpCodes.Ldarg_0);
					//setIl.Emit(OpCodes.Ldarg_1);
					//setIl.Emit(OpCodes.Stfld, fieldBuilder);
					//setIl.Emit(OpCodes.Ret);
					//propertyBuilder.SetSetMethod(setMethodBuilder);
				}

				// 添加带有所有属性参数的构造函数
				var paramTypes = properties.Select(p => p.Type).ToArray();
				var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, paramTypes);
				var ctorIl = constructorBuilder.GetILGenerator();
				ctorIl.Emit(OpCodes.Ldarg_0);
				ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
				for (int i = 0; i < fieldBuilders.Count; i++)
				{
					ctorIl.Emit(OpCodes.Ldarg_0);
					ctorIl.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
					ctorIl.Emit(OpCodes.Stfld, fieldBuilders[i]);
				}
				ctorIl.Emit(OpCodes.Ret);

				return typeBuilder.CreateType();
			}
		}

		static void Test21_ExpandoObject()
		{
			dynamic a = new ExpandoObject();
			a.Name = "tom";
			((IDictionary<string, object>)a)["Age"] = 20;
			Console.WriteLine(a.GetType());
			Console.WriteLine(a.Name + ":" + a.Age);
		}

		static void Test20()
		{
			var (a, b, c) = ("1", "2", "3");
			Console.WriteLine(a + c);
		}

		// 无法序列化委托
		static void Test19()
		{
			// 创建一个委托实例
			Func<int, int> add = x => x + 5;

			// 序列化
			BinaryFormatter formatter = new BinaryFormatter();
			using (FileStream stream = new FileStream("delegate.dat", FileMode.Create))
			{
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				formatter.Serialize(stream, add);
#pragma warning restore SYSLIB0011 // 类型或成员已过时
			}

			// 反序列化
			using (FileStream stream = new FileStream("delegate.dat", FileMode.Open))
			{
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				Func<int, int> loadedDelegate = (Func<int, int>)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // 类型或成员已过时
				Console.WriteLine(loadedDelegate(10));  // 输出 15
			}
		}

		static void Test18_CSharpScript()
		{
			var r = CSharpScript.EvaluateAsync("1+2+3+4+5").Result;
			Console.WriteLine(r);
		}

		static void Test17()
		{
			var t = typeof(Program);
			var e = t.GetEvent("Handled");
			var e2 = t.GetEvent("Handle2");
			var p = new Program();
			var ht = e.EventHandlerType; // EventHandler<EventArgs>
			string text = "hi";
			Action<object, EventArgs> d = (ss, ee) => Console.WriteLine("handled:" + text);

			// 将Action<object, EventArgs>转换为EventHandler<EventArgs>并注册到事件
			var handler = (EventHandler<EventArgs>)Delegate.CreateDelegate(ht, d.Target, d.Method);
			e.AddEventHandler(p, handler);

			p.OnHandled(EventArgs.Empty); // 触发事件，输出 "handled"
		}

		static void Test16()
		{
			var t = typeof(Program);
			var e = t.GetEvent("Handled");
			var p = new Program();
			var ht = e.EventHandlerType; // EventHandler<EventArgs>

			// 获取事件类型的 Invoke 方法
			var handlerType = ht; // EventHandler<EventArgs>

			// 创建动态方法
			var dynamicMethod = new System.Reflection.Emit.DynamicMethod(
				"DynamicHandler",
				typeof(void),
				new[] { typeof(object), typeof(EventArgs) },
				typeof(Program).Module);

			var il = dynamicMethod.GetILGenerator();
			// 获取 Console.WriteLine 方法
			var writeLineMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });
			// 加载字符串常量 "handled"
			il.Emit(System.Reflection.Emit.OpCodes.Ldstr, "handled");
			// 调用 Console.WriteLine
			il.Emit(System.Reflection.Emit.OpCodes.Call, writeLineMethod);
			// 返回
			il.Emit(System.Reflection.Emit.OpCodes.Ret);

			// 创建委托实例并注册到事件
			var handler = dynamicMethod.CreateDelegate(handlerType);
			e.AddEventHandler(p, handler);

			p.OnHandled(EventArgs.Empty); // 触发事件，输出 "handled"
		}

		static void Test15()
		{
			var method = typeof(Program).GetMethod("Test15_Method");
			var type0 = method.GetParameters()[0].ParameterType;
			Console.WriteLine(type0.IsGenericType);
			Console.WriteLine(type0.GetGenericTypeDefinition() == typeof(Expression<>));
			var generics = type0.GetGenericArguments();
			Console.WriteLine(generics[0].GetGenericTypeDefinition() == typeof(Func<,>));
			Console.WriteLine(type0.IsSubclassOf(typeof(LambdaExpression)));
		}

		public static void Test15_Method<T>(Expression<Func<T, bool>> expr)
		{

		}

		static void Test14_Eval()
		{
			Console.WriteLine(new Script().Eval("int n=10;eval(\"n+20\")")); // 30
			Console.WriteLine(new Script().Eval("int n=10;eval(\"n+20\")", ECompileMode.All)); // 30
			Console.WriteLine(new Script().Eval("int n=10;var s=\"n+20\";eval(s)")); // 30
			Console.WriteLine(new Script().Eval("int n=10;var s=\"n+20\";eval(s)", ECompileMode.All)); // 20
			Console.WriteLine(new Script().Eval("int n=10;eval(\"n+=20\");n")); // 30
			Console.WriteLine(new Script().Eval("int n=10;eval(\"n+=20\");n", ECompileMode.All)); // 30
			Console.WriteLine(new Script().Eval("int n=10;var s=\"n+=20\";eval(s);n")); // 30
			Console.WriteLine(new Script().Eval("int n=10;var s=\"n+=20\";eval(s);n", ECompileMode.All)); // 10
		}

		static void Test13_Convert()
		{
			object n = 5L;
			Console.WriteLine(ScriptUtils.Convert(n, typeof(double)));

			var p = Expression.Parameter(typeof(object));
			var v = Expression.Variable(typeof(object));
			var block = Expression.Block(new[] { v },
				Expression.Assign(v, Expression.Convert(p, typeof(object))),
				Expression.Call(ExpressionUtils.Method_ScriptUtils_Convert, v, ExpressionUtils.Constant_typeof_double));
			var expr = Expression.Lambda<Func<object, object>>(block, new ParameterExpression[] { p });
			var func = expr.Compile();
			Console.WriteLine(func(5L).GetType());
		}

		static void Test12_IronPython()
		{
			string s = @"
def exec2(a) :
    m=0
    s=''
    if a>0 and a<10 : 
        m=1
        s='大于0且小于10'
    elif a>=10 and a<20 :
        m=2
        s='大于等于10且小于20'
    elif a>=20 and a<30 :
        m=3
        s='大于等于20且小于30'
    else :
        m=4
        s='大于等于30'
    return (f'{m},{s}')

exec2(26)
";
			var engine = Python.CreateEngine();
			ScriptScope scope = engine.CreateScope();
			var result = engine.Execute(s, scope);
			Console.WriteLine(result);
			Console.WriteLine(result.GetType());
			Console.WriteLine(engine.Execute("exec2(16)", scope));
			Console.WriteLine(engine.Execute("5", scope).GetType());
		}

		static void Test11_Convert()
		{
			var type = typeof(Convert);
			var methodInfo = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				.FirstOrDefault(a => a.Name == "ToInt32");
			//var d = methodInfo.CreateDelegate()

			var s1 = ExpressionUtils.Method_Enumerable_Select1;
			var s2 = ExpressionUtils.Method_Enumerable_Select2;
		}

		static void Test10_Lambda()
		{
			var list = new List<int>();
			var list2 = list.Where(a => a % 2 == 0).ToList();

			var type = typeof(Enumerable);
			var methods = type.GetMethods().Where(a => a.Name == "Where").ToList();
			var method = methods[0];
			var para = method.GetParameters()[0];
			var isExt = para.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
			Console.WriteLine(isExt);
		}

		static void Test07()
		{
			var sb = new StringBuilder();
			sb.Append('a').Append('b');
			Console.WriteLine(sb.Equals("ab"));
			Console.WriteLine(sb.Equals("abc"));
		}

		static void Test06()
		{
			// 1. 定义参数
			var param = Expression.Parameter(typeof(int), "x");

			// 2. 创建返回标签（LabelTarget）
			var returnLabel = Expression.Label(typeof(bool), "returnLabel");

			// 3. 构建条件判断
			var condition = Expression.GreaterThan(param, Expression.Constant(10));

			// 4. 构建 return 语句：如果条件满足，返回 true
			var returnTrue = Expression.Return(returnLabel, Expression.Constant(true), typeof(bool));

			// 5. 构建条件判断的 Block
			var ifThenElse = Expression.IfThenElse(
				condition,
				returnTrue, // 条件为 true 时执行 return
				Expression.Return(returnLabel, Expression.Constant(false), typeof(bool)) // 否则返回 false
			);

			// 6. 构建完整的表达式块
			var block = Expression.Block(
				ifThenElse,
				Expression.Label(returnLabel, Expression.Default(typeof(bool))) // 必须包含 Label
			);

			// 7. 编译表达式树并生成委托
			var lambda = Expression.Lambda<Func<int, bool>>(block, param);
			var func = lambda.Compile();

			// 8. 测试
			Console.WriteLine(func(15)); // 输出 True
			Console.WriteLine(func(5));  // 输出 False
		}

		static void Test05()
		{
			// 1. 构建test函数表达式
			ParameterExpression a = Expression.Parameter(typeof(int), "a");
			ParameterExpression b = Expression.Parameter(typeof(int), "b");
			BinaryExpression add = Expression.Add(a, b);
			LambdaExpression testLambda = Expression.Lambda(add, a, b); // (a, b) => a + b
			var f = Expression.Lambda(testLambda).Compile().DynamicInvoke();
			Console.WriteLine(f.GetType());
		}

		static void Test04()
		{
			// 1. 构建test函数表达式
			ParameterExpression a = Expression.Parameter(typeof(int), "a");
			ParameterExpression b = Expression.Parameter(typeof(int), "b");
			BinaryExpression add = Expression.Add(a, b);
			LambdaExpression testLambda = Expression.Lambda(add, a, b); // (a, b) => a + b

			// 2. 构建主表达式树
			// 常量表达式
			ConstantExpression const100 = Expression.Constant(100);
			ConstantExpression arg1 = Expression.Constant(5);
			ConstantExpression arg2 = Expression.Constant(5);
			ConstantExpression const6 = Expression.Constant(6);
			ConstantExpression const2 = Expression.Constant(2);

			// 调用test函数表达式
			InvocationExpression testCall = Expression.Invoke(
				testLambda,
				arg1,
				arg2
			);

			// 构建运算表达式
			BinaryExpression multiply1 = Expression.Multiply(const100, testCall);   // 100 * test(5,5)
			BinaryExpression subtract = Expression.Subtract(const6, const2);        // (6-2)
			BinaryExpression final = Expression.Multiply(multiply1, subtract);      // * (6-2)

			// 3. 编译表达式树
			Expression<Func<int>> lambda = Expression.Lambda<Func<int>>(final);
			Func<int> compiled = lambda.Compile();

			// 4. 执行并输出结果
			Console.WriteLine(compiled());  // 输出：4000
		}

		static void Test03()
		{
			string s = "int test(int a,int b)=a+b;100 * test(5,5) * (6-2)";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
		}

		static void Test02()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval("100 * (5 + 5) * (6-2)");
		}

		static void Test01_Benchmark()
		{
			var config = ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator);
			//BenchmarkRunner.Run<Benchmarks.DynamicTest>(config);
			//BenchmarkRunner.Run<Benchmarks.DynamicTest2>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest02>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest03_Func>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest04_Var>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest05_Var>(config);
			BenchmarkRunner.Run<Benchmarks.ExpressionTest06_Func>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest06_Func2>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest07_Type>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest08_For>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest09_if>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest10>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest11>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest12>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest13>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest14_For>(config);
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest15_rec>(config);
			//BenchmarkRunner.Run<Benchmarks.PythonTest01>(config);
			//new Benchmarks.PythonTest01().AScript1();
			//new Benchmarks.ExpressionTest05_Var().AScript2_NoCache();
			//new Benchmarks.ExpressionTest06_Func().AScript1_3();
			//new Benchmarks.ExpressionTest06_Func().AScript2_NoCache();
			//new Benchmarks.ExpressionTest08_For().AScript2_NoCache();
			//new Benchmarks.ExpressionTest09().AScript();
			//new Benchmarks.ExpressionTest10().AScript();
			//new Benchmarks.ExpressionTest12().AScript();
		}

	}
}