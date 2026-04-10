using AScript.Test.Consoles.中文;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Linq.Expressions;
using System.Text;

namespace AScript.Test.Consoles
{
	internal class Program
	{
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
			Test12();
			//Console.WriteLine(typeof(int[]).FullName);
			Console.WriteLine("end");
			Console.ReadLine();
		}

		private static void Test12()
		{
			Script.Langs["中文"] = 中文语言.实例;

			string s = @"
整型 n=10;
整型 x=0;
如果 n<5 则 x=1+n;
否则 如果 n<20 则 x=2+n;
否则 x=3+n;
x
";
			var script = new Script();
			Console.WriteLine(script.Eval(s));
			Console.WriteLine(script.Eval(s, ECompileMode.All));
		}

		static void Test11_Convert()
		{
			var type = typeof(Convert);
			var methodInfo = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
				.FirstOrDefault(a => a.Name == "ToInt32");
			//var d = methodInfo.CreateDelegate()
		}

		static void Test10_Lambda()
		{
			var list = new List<int>();
			var list2 = list.Where(a => a % 2 == 0).ToList();

			var type = typeof(Enumerable);
			var methods = type.GetMethods().Where(a=>a.Name == "Where").ToList();
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