using AScript.Lang.Python3;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class Lang_PythonTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["python3"] = Python3Lang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("python3");
		}

		[TestMethod]
		public void Test24()
		{
			string s = @"
def exec(a) :
	m=5
	s='hello'
	m+','+a+','+s

exec(26)
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("5,26,hello", script.Eval(s));
			Assert.AreEqual("5,16,hello", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test23()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("8hello", script.Eval("3+5+'hello'"));
		}

		[TestMethod]
		public void Test22_2()
		{
			int m = 10;
			Python3Lang.Instance.AddFunc<int, int, int>("sum", (a, b) => a + b + m);
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(18, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test22()
		{
			int m = 10;
			Python3Lang.Instance.AddFunc<int, int, int>("sum", (a, b) => a + b + m);
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(18, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test21_2()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			int m = 10;
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b + m);
			Assert.AreEqual(18, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test21()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			int m = 10;
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b + m);
			Assert.AreEqual(18, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test20_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(8, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test20()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Context.AddFunc<int, int, int>("sum", (a, b) => a + b);
			Assert.AreEqual(8, script.Eval("sum(3,5)"));
		}

		[TestMethod]
		public void Test19_list_mixed_types_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// 多种元素类型
			var code1 = @"
lst = [1, 'hello', True, 3.14, [1, 2], {'key': 'value'}]
len(lst)
";
			Assert.AreEqual(6L, script.Eval(code1));

			// 嵌套列表
			var code2 = @"
lst = [[1, 2], [3, 4], [5, 6]]
lst[1][0]
";
			Assert.AreEqual(3L, script.Eval(code2));

			// 混合类型遍历
			var code3 = @"
result = ''
for x in [1, 'a', True, None]:
    result += str(type(x).__name__) + ','
result
";
			Assert.AreEqual("int,str,bool,NoneType,", script.Eval(code3));

			// 空列表
			Assert.AreEqual(0L, script.Eval("len([])"));
			Assert.AreEqual(true, script.Eval("[] == []"));
		}

		[TestMethod]
		public void Test19_list_mixed_types()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// 多种元素类型
			var code1 = @"
lst = [1, 'hello', True, 3.14, [1, 2], {'key': 'value'}]
len(lst)
";
			Assert.AreEqual(6L, script.Eval(code1));

			// 嵌套列表
			var code2 = @"
lst = [[1, 2], [3, 4], [5, 6]]
lst[1][0]
";
			Assert.AreEqual(3L, script.Eval(code2));

			// 混合类型遍历
			var code3 = @"
result = ''
for x in [1, 'a', True, None]:
    result += str(type(x).__name__) + ','
result
";
			Assert.AreEqual("int,str,bool,NoneType,", script.Eval(code3));

			// 空列表
			Assert.AreEqual(0L, script.Eval("len([])"));
			Assert.AreEqual(true, script.Eval("[] == []"));
		}

		[TestMethod]
		public void Test18_list_iterate_4()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// for in 遍历
			var code1 = @"
total = 0
print(total,';')
for x in [1, 2, 3]:
    #total += x
	print(x,',')
#total
";
			script.Eval(code1);
			//Assert.AreEqual(6L, script.Eval(code1));
		}

		[TestMethod]
		public void Test18_list_iterate_3()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// for in 遍历
			var code1 = @"
total = 0
for x in [1, 2, 3]:
    total += x
total
";
			Assert.AreEqual(6L, script.Eval(code1));
		}

		[TestMethod]
		public void Test18_list_iterate_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// for in 遍历
			var code1 = @"
total = 0
for x in [1, 2, 3]:
    total += x
total
";
			Assert.AreEqual(6L, script.Eval(code1));

			// 遍历索引
			var code2 = @"
result = ''
for i, x in enumerate([1, 2, 3]):
    result += f'{i}:{x},'
result
";
			Assert.AreEqual("0:1,1:2,2:3,", script.Eval(code2));

			// 列表推导式
			var code3 = @"
[x * 2 for x in [1, 2, 3]]
";
			var list = (List<object>)script.Eval(code3);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2L, list[0]);
			Assert.AreEqual(4L, list[1]);
			Assert.AreEqual(6L, list[2]);
		}

		[TestMethod]
		public void Test18_list_iterate()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// for in 遍历
			var code1 = @"
total = 0
for x in [1, 2, 3]:
    total += x
total
";
			Assert.AreEqual(6L, script.Eval(code1));

			// 遍历索引
			var code2 = @"
result = ''
for i, x in enumerate([1, 2, 3]):
    result += f'{i}:{x},'
result
";
			Assert.AreEqual("0:1,1:2,2:3,", script.Eval(code2));

			// 列表推导式
			var code3 = @"
[x * 2 for x in [1, 2, 3]]
";
			var list = (List<object>)script.Eval(code3);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2L, list[0]);
			Assert.AreEqual(4L, list[1]);
			Assert.AreEqual(6L, list[2]);
		}

		[TestMethod]
		public void Test17_list_modify_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// 修改指定索引元素
			var code1 = @"
lst = [1, 2, 3]
lst[0] = 99
lst[1] = 100
lst
";
			var list = script.Eval(code1);
			var arr = (IReadOnlyList<object>)list;
			Assert.AreEqual(99L, arr[0]);
			Assert.AreEqual(100L, arr[1]);
			Assert.AreEqual(3L, arr[2]);

			// 切片修改
			var code2 = @"
lst = [1, 2, 3, 4, 5]
lst[1:3] = [20, 30]
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(20L, arr[1]);
			Assert.AreEqual(30L, arr[2]);
			Assert.AreEqual(4L, arr[3]);
			Assert.AreEqual(5L, arr[4]);
		}

		[TestMethod]
		public void Test17_list_modify()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// 修改指定索引元素
			var code1 = @"
lst = [1, 2, 3]
lst[0] = 99
lst[1] = 100
lst
";
			var list = script.Eval(code1);
			var arr = (List<object>)list;
			Assert.AreEqual(99L, arr[0]);
			Assert.AreEqual(100L, arr[1]);
			Assert.AreEqual(3L, arr[2]);

			// 切片修改
			var code2 = @"
lst = [1, 2, 3, 4, 5]
lst[1:3] = [20, 30]
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(20L, arr[1]);
			Assert.AreEqual(30L, arr[2]);
			Assert.AreEqual(4L, arr[3]);
			Assert.AreEqual(5L, arr[4]);
		}

		[TestMethod]
		public void Test16_list_remove_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// pop 删除末尾元素
			var code1 = @"
lst = [1, 2, 3]
lst.pop()
lst
";
			var list = script.Eval(code1);
			var arr = (List<object>)list;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);

			// pop(index) 删除指定位置元素
			var code2 = @"
lst = [1, 2, 3]
lst.pop(0)
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(3L, arr[1]);

			// remove 删除第一个匹配元素
			var code3 = @"
lst = [1, 2, 3, 2]
lst.remove(2)
lst
";
			list = script.Eval(code3);
			arr = (List<object>)list;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(3L, arr[1]);
			Assert.AreEqual(2L, arr[2]);
		}

		[TestMethod]
		public void Test16_list_remove()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// pop 删除末尾元素
			var code1 = @"
lst = [1, 2, 3]
lst.pop()
lst
";
			var list = script.Eval(code1);
			var arr = (List<object>)list;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);

			// pop(index) 删除指定位置元素
			var code2 = @"
lst = [1, 2, 3]
lst.pop(0)
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(2, arr.Count);
			Assert.AreEqual(2L, arr[0]);
			Assert.AreEqual(3L, arr[1]);

			// remove 删除第一个匹配元素
			var code3 = @"
lst = [1, 2, 3, 2]
lst.remove(2)
lst
";
			list = script.Eval(code3);
			arr = (List<object>)list;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(3L, arr[1]);
			Assert.AreEqual(2L, arr[2]);
		}

		[TestMethod]
		public void Test15_list_add_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// append 添加元素
			var code = @"
lst = [1, 2, 3]
lst.append(4)
lst.append(5)
lst
";
			var list = script.Eval(code);
			var arr = (IReadOnlyList<object>)list;
			Assert.AreEqual(5, arr.Count);
			Assert.AreEqual(4L, arr[3]);
			Assert.AreEqual(5L, arr[4]);

			// insert 插入元素
			var code2 = @"
lst = [1, 2, 3]
lst.insert(1, 99)
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(99L, arr[1]);
			Assert.AreEqual(2L, arr[2]);

			// + 连接列表
			Assert.AreEqual(5L, script.Eval("len([1, 2] + [3, 4, 5])"));
			var result = (List<object>)script.Eval("[1, 2] + [3, 4]");
			Assert.AreEqual(4, result.Count);
		}

		[TestMethod]
		public void Test15_list_add()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// append 添加元素
			var code = @"
lst = [1, 2, 3]
lst.append(4)
lst.append(5)
lst
";
			var list = script.Eval(code);
			var arr = (List<object>)list;
			Assert.AreEqual(5, arr.Count);
			Assert.AreEqual(4L, arr[3]);
			Assert.AreEqual(5L, arr[4]);

			// insert 插入元素
			var code2 = @"
lst = [1, 2, 3]
lst.insert(1, 99)
lst
";
			list = script.Eval(code2);
			arr = (List<object>)list;
			Assert.AreEqual(4, arr.Count);
			Assert.AreEqual(99L, arr[1]);
			Assert.AreEqual(2L, arr[2]);

			// + 连接列表
			Assert.AreEqual(5L, script.Eval("len([1, 2] + [3, 4, 5])"));
			var result = (List<object>)script.Eval("[1, 2] + [3, 4]");
			Assert.AreEqual(4, result.Count);
		}

		[TestMethod]
		public void Test14_list_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };

			// 基本列表创建和访问
			var list = script.Eval("[1, 2, 3]");
			Assert.IsTrue(list is List<object>);
			var arr = (List<object>)list;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);

			// 负索引访问
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));

			script.Eval("print([1,2,3,4])");
			script.Eval("print([[1,2,3],2,3,4,'hello'])");
		}

		[TestMethod]
		public void Test14_list()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };

			// 基本列表创建和访问
			var list = script.Eval("[1, 2, 3]");
			Assert.IsTrue(list is List<object>);
			var arr = (List<object>)list;
			Assert.AreEqual(3, arr.Count);
			Assert.AreEqual(1L, arr[0]);
			Assert.AreEqual(2L, arr[1]);
			Assert.AreEqual(3L, arr[2]);

			// 负索引访问
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));

			script.Eval("print([1,2,3,4])");
			script.Eval("print([[1,2,3],2,3,4,'hello'])");
		}

		[TestMethod]
		public void Test13_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(true, script.Eval("True"));
			Assert.AreEqual(false, script.Eval("False"));
		}

		[TestMethod]
		public void Test13()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(true, script.Eval("True"));
			Assert.AreEqual(false, script.Eval("False"));
		}

		[TestMethod]
		public void Test12_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("name", "tom");
			Assert.AreEqual("hello tom", script.Eval("f'hello {name}'"));
		}

		[TestMethod]
		public void Test12()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("name", "tom");
			Assert.AreEqual("hello tom", script.Eval("f'hello {name}'"));
		}

		[TestMethod]
		public void Test11_range_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(-2, 4)");
			Assert.IsTrue(r is IReadOnlyList<long>);
			var list = (IReadOnlyList<long>)r;
			Assert.AreEqual(6, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual((long)(i - 2), list[i]);
			}
		}

		[TestMethod]
		public void Test11_range()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(-2, 4)");
			Assert.IsTrue(r is IReadOnlyList<long>);
			var list = (IReadOnlyList<long>)r;
			Assert.AreEqual(6, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual((long)(i - 2), list[i]);
			}
		}

		[TestMethod]
		public void Test10_range_2()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(4)");
			Assert.IsTrue(r is IReadOnlyList<long>);
			var list = (IReadOnlyList<long>)r;
			Assert.AreEqual(4, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual((long)i, list[i]);
			}
		}

		[TestMethod]
		public void Test10_range()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			var r = script.Eval("range(4)");
			Assert.IsTrue(r is IReadOnlyList<long>);
			var list = (IReadOnlyList<long>)r;
			Assert.AreEqual(4, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual((long)i, list[i]);
			}
		}

		[TestMethod]
		public void Test09_2()
		{
			Console.WriteLine(Math.Floor(-9.0 / 2));
			//Console.WriteLine(2 ** 3);
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(2.5, script.Eval("10/4"));
			Assert.AreEqual(2.5, script.Eval("n=10\nn/=4"));
			Assert.AreEqual(2L, script.Eval("10//4"));
			Assert.AreEqual(2L, script.Eval("n=10\nn//=4"));
			Assert.AreEqual(2.0, script.Eval("10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n:=10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n=10.4\nn//=4"));
			Assert.AreEqual(4L, script.Eval("9//2"));
			Assert.AreEqual(-5L, script.Eval("-9//2"));
			Assert.AreEqual(-5L, script.Eval("n:=-9//2"));
			Assert.AreEqual(-5L, script.Eval("n=-9\nn//=2"));
		}

		[TestMethod]
		public void Test09()
		{
			Console.WriteLine(Math.Floor(-9.0 / 2));
			//Console.WriteLine(2 ** 3);
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual(2.5, script.Eval("10/4"));
			Assert.AreEqual(2.5, script.Eval("n=10\nn/=4"));
			Assert.AreEqual(2L, script.Eval("10//4"));
			Assert.AreEqual(2L, script.Eval("n=10\nn//=4"));
			Assert.AreEqual(2.0, script.Eval("10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n:=10.4//4"));
			Assert.AreEqual(2.0, script.Eval("n=10.4\nn//=4"));
			Assert.AreEqual(4L, script.Eval("9//2"));
			Assert.AreEqual(-5L, script.Eval("-9//2"));
			Assert.AreEqual(-5L, script.Eval("n:=-9//2"));
			Assert.AreEqual(-5L, script.Eval("n=-9\nn//=2"));
		}

		[TestMethod]
		public void Test08()
		{
			string s = @"
'''
多行文本1
多行文本2
多行文本3
'''
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			string r = @"
多行文本1
多行文本2
多行文本3
";
			Assert.AreEqual(r, script.Eval(s));
		}

		[TestMethod]
		public void Test07()
		{
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Eval("print('hello')");
			script.Eval("exec('print(\\'hello everyone\\')')");
		}

		[TestMethod]
		public void Test06_2()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var tasks = new Task[100];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					var script = new Script();
					script.Options.CompileMode = ECompileMode.All;
					Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
					Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
				});
			}
			Task.WaitAll(tasks);
		}

		[TestMethod]
		public void Test06()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var tasks = new Task[100];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					var script = new Script();
					Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
					Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
				});
			}
			Task.WaitAll(tasks);
		}

		[TestMethod]
		public void Test05_2()
		{
			string s = @"
def exec(a) :
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
	m+','+s

exec(26)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test05()
		{
			string s = @"
def exec(a) :
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
	m+','+s

exec(26)
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test04_2()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end : # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    return m+','+s
#end
}
exec(26)
";
			var script = new Script();
			// 编译执行模式
			script.Options.CompileMode = ECompileMode.All;
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test04()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif @lang csharp a>=10 && a<20 @end: # 条件嵌入csharp语言
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    return m+','+s
#end
}
exec(26)
";
			var script = new Script();
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			Assert.AreEqual("2,大于等于10且小于20", script.Eval("exec(16)"));
		}

		[TestMethod]
		public void Test03()
		{
			string s = @"
string exec(int a) {
#lang python3
	#使用python3语言
	m=0
	s=''
	if a>0 and a<10:
	  m=1
	  s='大于0且小于10'
	elif a>=10 and a<20:
	  m=2
	  s='大于等于10且小于20'
	elif a>=20 and a<30:
	  m=3
	  s='大于等于20且小于30'
	else :
	  m=4
	  s='大于等于30'
    m+','+s
#end
}
exec(26)
";
			var script = new Script();
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}

		[TestMethod]
		public void Test02()
		{
			string s = @"
m=0
s=''#行注释
if a>0 and a<10: #行注释 (0~10)
  m=1
  '''
多行注释1
多行注释2
多选注释3
'''
  s='大于0且小于10' #行注释说明
  ''''''#空注释
elif a>=10 and a<20:#行注释[10,20)
  m=2
  s='大于等于10且小于20'
else :
  '''
多行文本1
多行文本2
多选文本3
'''
  m=3#其他
m+','+s
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual("1,大于0且小于10", script.Eval(s));
		}

		[TestMethod]
		public void Test01()
		{
			string s = @"
m=0
s=''
if a>0 and a<10:
  m=1
  s='大于0且小于10'
elif a>=10 and a<20:
  m=2
  s='大于等于10且小于20'
elif a>=20 and a<30:
  m=3
  s='大于等于20且小于30'
else :
  m=4
  s='大于等于30'
m+','+s
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			script.Context.SetVar("a", 6);
			Assert.AreEqual("1,大于0且小于10", script.Eval(s));
			script.Context.SetVar("a", 16);
			Assert.AreEqual("2,大于等于10且小于20", script.Eval(s));
			script.Context.SetVar("a", 26);
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
			script.Context.SetVar("a", 36);
			Assert.AreEqual("4,大于等于30", script.Eval(s));
		}

		[TestMethod]
		public void Test00_2()
		{
			string s = @"
def exec(a) :
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
    return f'{m},{s}'

exec(26)
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}

		[TestMethod]
		public void Test00()
		{
			string s = @"
def exec(a) :
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
    return f'{m},{s}'

exec(26)
";
			var script = new Script();
			script.Context.Langs = new[] { "python3" };
			Assert.AreEqual("3,大于等于20且小于30", script.Eval(s));
		}
	}
}
