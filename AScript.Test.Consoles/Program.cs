using AScript.Lang.JavaScript;
using AScript.Lang.JavaScript.axios;
using AScript.Lang.Sql;
using AScript.Test.Consoles.Benchmarks;
using AScript.Test.Consoles.Benchmarks.FleeTest;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using DynamicExpresso;
using IronPython.Hosting;
using Lua;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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
			Test01_Benchmark();
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
			//Test18_Flee();
			//Test18_DynamicExpresso();
			//Test18_Jurassic();
			//Test18_ClearScript();
			//Test18_Jint();
			//Test18_Lua();
			//Test19();
			//Test20();
			//Test21_ExpandoObject();
			//Test22();
			//Test23();
			//Test24_Sqlite();
			//Test25_js();
			//var p = Expression.Constant(new Person());
			//Console.WriteLine(Expression.PropertyOrField(p, "name"));
			Console.WriteLine("end");
			Console.ReadLine();
		}

		static void Test25_js()
		{
			Script.Langs.Set("js", JavaScriptLang.Instance);
			JavaScriptLang.Instance.AddModule("axios", new JavaScriptAxiosModule());

			{
				// Test import syntax
				var s = @"
import axios from 'axios';
axios.createMock({a:1});
axios.createMock({a:1});
axios.createMock({a:1});
";
				var script = new Script();
				script.Context.Langs = new[] { "js" };
				script.Eval(s);
				Console.WriteLine("Import syntax test passed!");
			}

			{
				// Original test with require
				var s = @"
var axios = require('axios');
axios.get('https://www.runoob.com/try/ajax/json_demo.json')
	.then(res=>{
		console.log('result', res);
	})
	.catch(error=> {
		console.log(error);
    });
";
				var script = new Script();
				script.Context.Langs = new[] { "js" };
				script.Eval(s);
			}
		}

		static void Test24_Sqlite()
		{
			Script.Langs.Set("sql", SqlLang.Instance);

			using (var context = new TestSqliteContext())
			{
				context.Database.Migrate();

				context.Persons.ExecuteDelete();
				context.AddressInfos.ExecuteDelete();
				context.SaveChanges();
			}

			using (var context = new TestSqliteContext())
			{
				//context.Persons.AddRange(new[]
				//{
				//	new Person{ Id = "1001", Name = "tom", Age = 20 },
				//	new Person{ Id = "1002", Name = "san", Age = 25 },
				//	new Person{ Id = "1003", Name = "tony", Age = 18 },
				//	new Person{ Id = "1004", Name = "tim", Age = 25 }
				//});
				context.AddressInfos.AddRange(new[]
				{
					new AddressInfo{UserId = "1002", Address = "a" },
					new AddressInfo{UserId = "1004", Address = "b" },
					new AddressInfo{UserId = "1005", Address = "c" },
				});

				string s = @"
insert into context.Persons (Id,Name,Age) 
values ('1001','tom',20),('1002','san',25),('1003','tony',18),('1004','tim',25)";
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				script.Context.Langs = new[] { "sql" };
				script.Context.SetVar("context", context);
				script.Eval(s);

				context.SaveChanges();
			}

			using (var context = new TestSqliteContext())
			{
				//context.Persons.ExecuteUpdate(a => a.SetProperty(p => p.Name, p => "hello"));
				//context.Persons.RemoveRange

				string s = @"update context.Persons set name='sany',age=age+10 where id='1002'";
				var script = new Script();
				script.Options.CompileMode = ECompileMode.All;
				script.Context.Langs = new[] { "sql" };
				script.Context.SetVar("context", context);
				script.Eval(s);
				context.SaveChanges();
			}

			using (var context = new TestSqliteContext())
			{
				string s = @"
				var persons = context.Persons;
				var q = from a in persons where a.Name.isnotempty() && a.Name.contains2('o')
						orderby a.Age
						select new Person { a.Name, a.Age };
				q.ToList()//.Where(a=>a.Name.contains2('m')).ToList()
				";
				var script = new Script();
				script.Context.AddType<Person>();
				script.Context.AddLambda<Func<string, bool>>("isnotempty", s => !string.IsNullOrEmpty(s));
				script.Context.AddLambda<Func<string, string, bool>>("contains2", (s, a) => s.Contains(a));
				//script.Context.AddFunc<string, string, bool>("contains2", (s, a) => s.Contains(a));
				script.Context.SetVar("context", context);
				var list = script.Eval<IList>(s);
				Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

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

				//string s = @"
				//var q = from a in context.AddressInfos
				//		right join p in context.Persons on a.UserId equals p.Id
				//		select new { p.Id, p.Name, p.Age, MyAddress = a.Address };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IList>(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//string s = @"
				//var q = from p in context.Persons
				//		left join a in context.AddressInfos on p.Id equals a.UserId
				//		select new { p.Id, p.Name, p.Age, MyAddress = a.Address };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IList>(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				// case p.Age when 20 then 1 when 22 then 2 else 3 end as Level
				// case when p.Age=20 then 1 when p.Age=22 then 2 else 3 end as Level
				//string s = @"
				//select p.Id, p.Name, p.Age, a.Address as MyAddress, case when p.Age=20 then 1 when p.Age=22 then 2 else 3 end as Level
				//from context.Persons as p
				//left join context.AddressInfos as a on p.Id = a.UserId
				//order by p.age desc
				//";
				//var script = new Script();
				//script.Context.Langs = new[] { "sql" };
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IEnumerable<dynamic>>(s).ToList();
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

				//string s = @"
				//var q = from p in context.Persons
				//		join a in context.AddressInfos on p.Id equals a.UserId into aa
				//		from a in aa.DefaultIfEmpty()
				//		select new { p.Id, p.Name, p.Age, MyAddress = a.Address };
				//q.ToList();
				//";
				//var script = new Script();
				//script.Context.SetVar("context", context);
				//var list = script.Eval<IList>(s);
				//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));

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

		static void Test22_BuildDynamicType()
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

		static void Test20_Tuple_Destruct()
		{
			var (a, b, c) = ("1", "2", "3");
			Console.WriteLine(a + c);
		}

		//		// 无法序列化委托
		//		static void Test19()
		//		{
		//			// 创建一个委托实例
		//			Func<int, int> add = x => x + 5;

		//			// 序列化
		//			BinaryFormatter formatter = new BinaryFormatter();
		//			using (FileStream stream = new FileStream("delegate.dat", FileMode.Create))
		//			{
		//#pragma warning disable SYSLIB0011 // 类型或成员已过时
		//				formatter.Serialize(stream, add);
		//#pragma warning restore SYSLIB0011 // 类型或成员已过时
		//			}

		//			// 反序列化
		//			using (FileStream stream = new FileStream("delegate.dat", FileMode.Open))
		//			{
		//#pragma warning disable SYSLIB0011 // 类型或成员已过时
		//				Func<int, int> loadedDelegate = (Func<int, int>)formatter.Deserialize(stream);
		//#pragma warning restore SYSLIB0011 // 类型或成员已过时
		//				Console.WriteLine(loadedDelegate(10));  // 输出 15
		//			}
		//		}

		static void Test18_Lua()
		{
			// MoonSharp
			{
				var script = new MoonSharp.Interpreter.Script();
				// 必须有return
				var result = script.DoString("return 10 + 5").ToObject<int>();
				Console.WriteLine(result);
			}

			// NLua
			{
				var lua = new NLua.Lua();
				lua["sum"] = new Func<long, long, long>((a, b) => a + b);
				var result = lua.DoString("return sum(10, 5)")[0];
				Console.WriteLine(result);
				Console.WriteLine(result.GetType());
			}

			// LuaCSharp
			{
				var lua = Lua.LuaState.Create();
				var result = lua.DoStringAsync("return 10 + 5").Result[0].Read<long>();
				Console.WriteLine(result);

				var result2 = lua.DoStringAsync("local a = 10 a='hello' return a").Result[0].Read<string>();
				Console.WriteLine(result2);

				// 需要手动添加setmetatable函数
				lua.Environment["setmetatable"] = new LuaFunction(async (context, cancellationToken) =>
				{
					var arg0 = context.GetArgument<Lua.LuaTable>(0);
					var arg1 = context.GetArgument<Lua.LuaTable>(1);
					arg0.Metatable = arg1;
					context.Return(arg0);
					return 1;
				});

				string s3 = @"
local obj = { age = 100 }
local me = {}
setmetatable(me, obj)
return obj.age
";
				var result3 = lua.DoStringAsync(s3).Result[0].Read<long>();
				Console.WriteLine(result3);
			}
		}

		static void Test18_Jint()
		{
			{
				var engine = new Jint.Engine();
				engine.SetValue("c", 10);
				var result = engine.Evaluate("var a=5; var b = 8; a+b+c");
				Console.WriteLine(result);
				Console.WriteLine("a=" + engine.GetValue("a"));
			}

			{
				var engine = new Jint.Engine();
				var result = engine.Evaluate("var {name, age} = {name: 'Alice', age: 25}; name + age");
				Console.WriteLine(result);
			}

			{
				var engine = new Jint.Engine();
				var result = engine.Evaluate("var arr = [1, 2, 3, 4, 5].filter(x => x % 2 == 0); arr.length");
				Console.WriteLine(result);
			}

			{
				var engine = new Jint.Engine();
				var result = engine.Evaluate("var [name, age] = ['Alice', 25]; name + age");
				Console.WriteLine(result);
			}

			{
				var engine = new Jint.Engine();
				var result = engine.EvaluateAsync("new Promise((resolve, reject) => resolve(42)).then(x => 2 * x)").Result;
				Console.WriteLine(result);
			}

			{
				var engine = new Jint.Engine();
				engine.SetValue("name", "world");
				var result = engine.Evaluate("`hello ${name}`");
				Console.WriteLine(result);
				Console.WriteLine(engine.Evaluate("5**2"));
				Console.WriteLine(engine.Evaluate("'5'.padStart(4,'0')"));
				engine.SetValue("mymethod", new Func<string, int, string>((s, n) => s + n));
				Console.WriteLine(engine.Evaluate("mymethod('hello', 60)"));
				//Console.WriteLine(engine.Evaluate("'hello'.mymethod(60)")); // 不支持
			}

			//// 没成功
			//{
			//	var engine = new Jint.Engine(options =>
			//	{
			//		//options.EnableModules(@"./Benchmarks/JavaScriptTest");
			//	});
			//	//engine.
			//	//engine.Modules.Add()
			//	engine.Modules.Add("mymodule", m => m.AddSource(System.IO.File.ReadAllText("./Benchmarks/JavaScriptTest/utils.js")));
			//	engine.Evaluate("import m from 'mymodule'");
			//	//engine.Evaluate("var m = require('mymodule')");
			//}

		}

		static void Test18_ClearScript()
		{
			var engine = new Microsoft.ClearScript.Windows.JScriptEngine();
			var result = engine.Evaluate("var a = 10; var b = 20; var c = a + b;");
			Console.WriteLine(result); // 输出 30

		}

		static void Test18_Jurassic()
		{
			{
				var engine = new Jurassic.ScriptEngine();
				var result = engine.Evaluate("var a = 15; var b = 6; var c=a+b; a='hello'; a+c");
				Console.WriteLine(result);
				Console.WriteLine(engine.GetGlobalValue("a"));
			}
			{
				var engine = new Jurassic.ScriptEngine();
				var result = engine.Evaluate("var arr = [1, 2, 3, 4, 5]; arr[0]+arr[1]+arr[2]+arr[3]+arr[4]");
				Console.WriteLine(result);
			}
			{
				var engine = new Jurassic.ScriptEngine();
				engine.SetGlobalValue("name", "world");
				var result = engine.Evaluate("`hello ${name}`");
				Console.WriteLine(result);
				Console.WriteLine(engine.Evaluate("5**2"));
				//Console.WriteLine(engine.Evaluate("'5'.padStart(4,'0')")); // 不支持
			}
			//// 不支持
			//{
			//	var engine = new Jurassic.ScriptEngine();
			//	engine.Evaluate("var m = require('mymodule')");
			//}
			//// 不支持import
			//{
			//	var engine = new Jurassic.ScriptEngine();
			//	engine.Evaluate("import m from 'mymodule'");
			//}
			//// 不支持await，不支持 ()=>{}
			//{
			//	var engine = new Jurassic.ScriptEngine();
			//	//var result = engine.Evaluate("new Promise((resolve, reject) => resolve(42)).then(x => 2 * x)");
			//	var result = engine.Evaluate("function a(resolve, reject) { resolve(42); } function b(x) { return 2 * x; } var t = new Promise(a).then(b); await t;");
			//	Console.WriteLine(result);
			//}
			//// 不支持
			//{
			//	var engine = new Jurassic.ScriptEngine();
			//	var result = engine.Evaluate("var [name, age] = ['Alice', 25]; name");
			//	Console.WriteLine(result);
			//}
			//// 不支持
			//{
			//	var engine = new Jurassic.ScriptEngine();
			//	var result = engine.Evaluate("var {name, age} = {name: 'Alice', age: 25}; name");
			//	Console.WriteLine(result);
			//}
		}

		static void Test18_DynamicExpresso()
		{
			var interpreter = new DynamicExpresso.Interpreter(InterpreterOptions.Default | InterpreterOptions.LambdaExpressions);
			interpreter.SetVariable("a", 100);
			interpreter.SetVariable("b", 5);
			interpreter.SetVariable("c", 6);
			var func = interpreter.ParseAsDelegate<Func<int>>("a+b+c");
			Console.WriteLine("dele:" + func()); // 111
			Console.WriteLine("eval:" + interpreter.Eval("a+b+c")); // 111
			interpreter.SetVariable("c", 8);
			Console.WriteLine("dele:" + func()); // 111
			Console.WriteLine("eval:" + interpreter.Eval("a+b+c")); // 113

			interpreter.SetVariable("list", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			var list = interpreter.Eval<List<int>>("list.Where(x=>x%2==0).ToList()");
			Console.WriteLine(string.Join(",", list));
			Console.WriteLine(interpreter.Eval<int>("list.Where(x=>x%2==0).ToList().Count"));

			//interpreter.Eval("int z = 50");
		}

		static void Test18_Flee()
		{
			string code = "a+20";
			var context = new Flee.PublicTypes.ExpressionContext();
			context.Variables["a"] = 10;
			var d = context.CompileDynamic(code);
			Console.WriteLine(d.Evaluate()); // 30
			context.Variables["a"] = 30;
			Console.WriteLine(d.Evaluate()); // 50
			Console.WriteLine(context.CompileDynamic("a>10 and a<20 or a<>40").Evaluate());
		}

		static void Test18_CSharpScript()
		{
			var r = CSharpScript.EvaluateAsync("1+2+3+4+5").Result;
			Console.WriteLine(r);
		}

		static void Test17_AddEvent()
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

		static void Test16_IL_AddEvent()
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

		//static void Test16_DataTable()
		//{
		//	DataTable table;
		//}

		static void Test15_GenericMethod()
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
				Expression.Call(ScriptUtils.Method_ScriptUtils_Convert, v, ScriptUtils.Constant_typeof_double));
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

			var s1 = ScriptUtils.Method_Enumerable_Select1;
			var s2 = ScriptUtils.Method_Enumerable_Select2;
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
			//BenchmarkRunner.Run<Benchmarks.ExpressionTest06_Func>(config);
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
			BenchmarkRunner.Run<AValueTest01>(config);

			//BenchmarkRunner.Run<Benchmarks.FleeTest01_const>(config);
			//BenchmarkRunner.Run<Benchmarks.FleeTest.FleeTest02_var>(config);
			//BenchmarkRunner.Run<Benchmarks.FleeTest03_call>(config);
			//BenchmarkRunner.Run<Benchmarks.FleeTest04_string>(config);
			//BenchmarkRunner.Run<FleeTest05_multi>(config);
			//BenchmarkRunner.Run<FleeTest07_bool>(config);
			//BenchmarkRunner.Run<Benchmarks.FleeTest06_call>(config);

			//BenchmarkRunner.Run<Benchmarks.DynamicExpressoTest.DynamicExpressoTest01_const>(config);
			//BenchmarkRunner.Run<Benchmarks.DynamicExpressoTest.DynamicExpressoTest02_var>(config);
			//BenchmarkRunner.Run<Benchmarks.DynamicExpressoTest.DynamicExpressoTest03_linq>(config);
			//BenchmarkRunner.Run<Benchmarks.DynamicExpressoTest.DynamicExpressoTest04_delegate>(config);
			//BenchmarkRunner.Run<Benchmarks.DynamicExpressoTest.DynamicExpressoTest05_lambda>(config);

			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest01_const>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest02_local>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest03_var>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest04_call>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest05_file>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest06_object>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest07_for>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest08_array>(config);
			//BenchmarkRunner.Run<Benchmarks.JavaScriptTest.JavaScriptTest09_array_filter>(config);

			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest01_const>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest02_local>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest03_var>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest04_call>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest05_function>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest06_table>(config);
			//BenchmarkRunner.Run<Benchmarks.LuaTest.LuaTest07_for>(config);

			//new Benchmarks.PythonTest01().AScript1();
			//new Benchmarks.ExpressionTest05_Var().AScript2_NoCache();
			//new Benchmarks.ExpressionTest06_Func().AScript1_3();
			//new Benchmarks.ExpressionTest06_Func().AScript2_NoCache();
			//new Benchmarks.ExpressionTest08_For().AScript2_NoCache();
			//new Benchmarks.ExpressionTest09().AScript();
			//new Benchmarks.ExpressionTest10().AScript();
			//new Benchmarks.ExpressionTest12().AScript();
			//new Benchmarks.DynamicTest2().Expr();

			//new Benchmarks.FleeTest.FleeTest02_var().AScript4_Cache();
			//new Benchmarks.FleeTest01_const().AScript3_UseCache();
			//new Benchmarks.FleeTest01_const().AScript3_UseCache();
			//new Benchmarks.FleeTest05_multi().AScript2_Compile2();
			//new Benchmarks.FleeTest05_multi().AScript2_Compile2();

			//new Benchmarks.JavaScriptTest.JavaScriptTest01_const().Jint1();
			//new Benchmarks.JavaScriptTest.JavaScriptTest01_const().AScript2_Compile();
			//new Benchmarks.JavaScriptTest.JavaScriptTest02_local().AScript2_Compile2();
			//new Benchmarks.JavaScriptTest.JavaScriptTest05_file().AScript1();
			//new Benchmarks.JavaScriptTest.JavaScriptTest05_file().Jint1();
			//new Benchmarks.JavaScriptTest.JavaScriptTest05_file().Jurassic2();
			//new Benchmarks.JavaScriptTest.JavaScriptTest05_file().AScript2_Compile();
			//new Benchmarks.JavaScriptTest.JavaScriptTest06_object().Jint1();
			//new Benchmarks.JavaScriptTest.JavaScriptTest07_for().Jint1();
			//new Benchmarks.JavaScriptTest.JavaScriptTest07_for().AScript2_Compile();

			//new Benchmarks.LuaTest.LuaTest01_const().AScript1();
			//new Benchmarks.LuaTest.LuaTest06_table().AScript1();
			//new Benchmarks.LuaTest.LuaTest06_table().LuaCSharp1();
			//new Benchmarks.LuaTest.LuaTest07_for().AScript2_Compile();
		}

	}
}