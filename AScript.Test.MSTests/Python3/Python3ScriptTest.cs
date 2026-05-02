using AScript.Lang.Python3;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AScript.Test.MSTests.Python3
{
	[TestClass]
	public class Python3ScriptTest
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

		private static Script CreateScript(bool compileMode = false)
		{
			var script = new Script();
			if (compileMode)
			{
				script.Options.CompileMode = ECompileMode.All;
			}
			script.Context.Langs = new[] { "python3" };
			return script;
		}

		#region 数学运算测试

		[TestMethod]
		public void TestMath_BasicOperations()
		{
			var script = CreateScript();
			Assert.AreEqual(15L, script.Eval("3 + 12"));
			Assert.AreEqual(-9L, script.Eval("3 - 12"));
			Assert.AreEqual(36L, script.Eval("3 * 12"));
			Assert.AreEqual(2.5, script.Eval("10 / 4"));
			Assert.AreEqual(2.5, script.Eval("n = 10\nn / 4"));
			Assert.AreEqual(2L, script.Eval("10 // 4"));
			Assert.AreEqual(1L, script.Eval("10 % 3"));
			Assert.AreEqual(8L, script.Eval("2 ** 3"));
		}

		[TestMethod]
		public void TestMath_CompareOperations()
		{
			var script = CreateScript();
			Assert.AreEqual(true, script.Eval("3 < 5"));
			Assert.AreEqual(true, script.Eval("3 <= 5"));
			Assert.AreEqual(false, script.Eval("3 > 5"));
			Assert.AreEqual(true, script.Eval("5 >= 5"));
			Assert.AreEqual(true, script.Eval("3 == 3"));
			Assert.AreEqual(true, script.Eval("3 != 5"));
		}

		[TestMethod]
		public void TestMath_LogicalOperations()
		{
			var script = CreateScript();
			Assert.AreEqual(true, script.Eval("True and True"));
			Assert.AreEqual(false, script.Eval("True and False"));
			Assert.AreEqual(true, script.Eval("True or False"));
			Assert.AreEqual(false, script.Eval("False or False"));
			Assert.AreEqual(false, script.Eval("not True"));
			Assert.AreEqual(true, script.Eval("not False"));
		}

		[TestMethod]
		public void TestMath_CompoundAssignment()
		{
			var script = CreateScript();
			Assert.AreEqual(5L, script.Eval("n = 5\nn"));
			Assert.AreEqual(8L, script.Eval("n = 5\nn += 3\nn"));
			Assert.AreEqual(2L, script.Eval("n = 5\nn -= 3\nn"));
			Assert.AreEqual(15L, script.Eval("n = 5\nn *= 3\nn"));
			Assert.AreEqual(2.5, script.Eval("n = 5\nn /= 2\nn"));
		}

		[TestMethod]
		public void TestMath_ComplexExpression()
		{
			var script = CreateScript();
			Assert.AreEqual(14L, script.Eval("2 + 3 * 4"));
			Assert.AreEqual(20L, script.Eval("(2 + 3) * 4"));
			Assert.AreEqual(5L, script.Eval("10 - 3 - 2"));
			Assert.AreEqual(10L, script.Eval("2 ** 3 + 2"));
		}

		[TestMethod]
		public void TestMath_CompileMode()
		{
			var script = CreateScript(compileMode: true);
			Assert.AreEqual(15L, script.Eval("3 + 12"));
			Assert.AreEqual(36L, script.Eval("3 * 12"));
			Assert.AreEqual(2.5, script.Eval("10 / 4"));
			Assert.AreEqual(2L, script.Eval("10 // 4"));
		}

		#endregion

		#region 条件语句测试

		[TestMethod]
		public void TestIf_Simple()
		{
			var script = CreateScript();
			script.Context.SetVar("a", 5);
			Assert.AreEqual("大于0", script.Eval(@"
s = ''
if a > 0:
    s = '大于0'
s
"));

			script.Context.SetVar("a", -1);
			Assert.AreEqual("", script.Eval(@"
s = ''
if a > 0:
    s = '大于0'
s
"));
		}

		[TestMethod]
		public void TestIf_IfElse()
		{
			var script = CreateScript();
			script.Context.SetVar("a", 5);
			Assert.AreEqual("正数", script.Eval(@"
s = ''
if a > 0:
    s = '正数'
else:
    s = '非正数'
s
"));

			script.Context.SetVar("a", -1);
			Assert.AreEqual("非正数", script.Eval(@"
s = ''
if a > 0:
    s = '正数'
else:
    s = '非正数'
s
"));
		}

		[TestMethod]
		public void TestIf_IfElifElse()
		{
			var script = CreateScript();
			script.Context.SetVar("a", 15);
			Assert.AreEqual("大", script.Eval(@"
s = ''
if a > 10:
    s = '大'
elif a > 5:
    s = '中等'
else:
    s = '小'
s
"));

			script.Context.SetVar("a", 8);
			Assert.AreEqual("中等", script.Eval(@"
s = ''
if a > 10:
    s = '大'
elif a > 5:
    s = '中等'
else:
    s = '小'
s
"));

			script.Context.SetVar("a", 3);
			Assert.AreEqual("小", script.Eval(@"
s = ''
if a > 10:
    s = '大'
elif a > 5:
    s = '中等'
else:
    s = '小'
s
"));
		}

		[TestMethod]
		public void TestIf_Nested()
		{
			var script = CreateScript();
			script.Context.SetVar("a", 5);
			script.Context.SetVar("b", 3);
			Assert.AreEqual("a大于b", script.Eval(@"
s = ''
if a > 10:
    if b > 0:
        s = 'a和b都大于0'
    else:
        s = 'a大于0但b不大于0'
elif b > 0:
    s = 'a大于b'
else:
    s = '其他'
s
"));
		}

		[TestMethod]
		public void TestIf_CompileMode()
		{
			var script = CreateScript(compileMode: true);
			script.Context.SetVar("a", 15);
			Assert.AreEqual("大", script.Eval(@"
s = ''
if a > 10:
    s = '大'
elif a > 5:
    s = '中等'
else:
    s = '小'
s
"));
		}

		#endregion

		#region 循环语句测试

		[TestMethod]
		public void TestFor_Range()
		{
			var script = CreateScript();
			Assert.AreEqual(6L, script.Eval(@"
total = 0
for i in range(4):
    total += i
total
"));

			Assert.AreEqual(15L, script.Eval(@"
total = 0
for i in range(1, 6):
    total += i
total
"));

			Assert.AreEqual(20L, script.Eval(@"
total = 0
for i in range(0, 10, 2):
    total += i
total
"));
		}

		[TestMethod]
		public void TestFor_List()
		{
			var script = CreateScript();
			Assert.AreEqual(6L, script.Eval(@"
total = 0
for x in [1, 2, 3]:
    total += x
total
"));

			Assert.AreEqual("abc", script.Eval(@"
result = ''
for c in ['a', 'b', 'c']:
    result += c
result
"));
		}

		[TestMethod]
		public void TestFor_String()
		{
			var script = CreateScript();
			Assert.AreEqual("abc", script.Eval(@"
result = ''
for c in 'abc':
    result += c
result
"));
		}

		[TestMethod]
		public void TestFor_Nested()
		{
			var script = CreateScript();
			Assert.AreEqual(9L, script.Eval(@"
total = 0
for i in range(3):
    for j in range(3):
        total += 1
total
"));
		}

		[TestMethod]
		public void TestFor_BreakContinue()
		{
			var script = CreateScript();
			Assert.AreEqual(3L, script.Eval(@"
total = 0
for i in range(10):
    if i >= 3:
        break
    total += i
total
"));

			Assert.AreEqual(5L, script.Eval(@"
total = 0
for i in range(10):
    if i == 1:
        continue
    if i >= 4:
        break
    total += i
total
"));
		}

		[TestMethod]
		public void TestFor_CompileMode()
		{
			var script = CreateScript(compileMode: true);
			Assert.AreEqual(6L, script.Eval(@"
total = 0
for i in range(4):
    total += i
total
"));
		}

		[TestMethod]
		public void TestWhile_Basic_2()
		{
			var script = CreateScript(true);
			Assert.AreEqual(5L, script.Eval(@"
n = 0
while n < 5:
    n += 1
n
"));

			Assert.AreEqual(15L, script.Eval(@"
n = 0
total = 0
while n < 5:
    n += 1
    total += n
total
"));
		}

		[TestMethod]
		public void TestWhile_Basic()
		{
			var script = CreateScript();
			Assert.AreEqual(5L, script.Eval(@"
n = 0
while n < 5:
    n += 1
n
"));

			Assert.AreEqual(15L, script.Eval(@"
n = 0
total = 0
while n < 5:
    n += 1
    total += n
total
"));
		}

		[TestMethod]
		public void TestWhile_BreakContinue_2()
		{
			var script = CreateScript(true);
			Assert.AreEqual(5L, script.Eval(@"
n = 0
while True:
    n += 1
    if n >= 5:
        break
n
"));

			Assert.AreEqual(52L, script.Eval(@"
n = 0
total = 0
while n < 10:
    n += 1
    if n == 3:
        continue
    total += n
total
"));
		}

		[TestMethod]
		public void TestWhile_BreakContinue()
		{
			var script = CreateScript();
			Assert.AreEqual(5L, script.Eval(@"
n = 0
while True:
    n += 1
    if n >= 5:
        break
n
"));

			Assert.AreEqual(52L, script.Eval(@"
n = 0
total = 0
while n < 10:
    n += 1
    if n == 3:
        continue
    total += n
total
"));
		}

		#endregion

		#region 列表测试

		[TestMethod]
		public void TestList_Create()
		{
			var script = CreateScript();
			var list = (List<object>)script.Eval("[1, 2, 3]");
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(1L, list[0]);
			Assert.AreEqual(2L, list[1]);
			Assert.AreEqual(3L, list[2]);

			var list2 = (List<object>)script.Eval("[]");
			Assert.AreEqual(0, list2.Count);
		}

		[TestMethod]
		public void TestList_Index_2()
		{
			var script = CreateScript(true);
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));
		}

		[TestMethod]
		public void TestList_Index()
		{
			var script = CreateScript();
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
			Assert.AreEqual(3L, script.Eval("[1, 2, 3][-1]"));
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][-2]"));
		}

		[TestMethod]
		public void TestList_Slice()
		{
			var script = CreateScript();
			var list = (List<object>)script.Eval("[1, 2, 3, 4, 5][1:4]");
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2L, list[0]);
			Assert.AreEqual(3L, list[1]);
			Assert.AreEqual(4L, list[2]);
		}

		[TestMethod]
		public void TestList_Modify()
		{
			var script = CreateScript();
			var code = @"
lst = [1, 2, 3]
lst[0] = 10
lst[1] = 20
lst
";
			var list = (List<object>)script.Eval(code);
			Assert.AreEqual(10L, list[0]);
			Assert.AreEqual(20L, list[1]);
			Assert.AreEqual(3L, list[2]);
		}

		[TestMethod]
		public void TestList_Add_2()
		{
			var script = CreateScript(true);
			var code = @"
lst = [1, 2]
lst.append(3)
lst.append(4)
lst
";
			var list = (List<object>)script.Eval(code);
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(4L, list[3]);

			var list2 = (List<object>)script.Eval("[1, 2] + [3, 4]");
			Assert.AreEqual(4, list2.Count);
		}

		[TestMethod]
		public void TestList_Add()
		{
			var script = CreateScript();
			var code = @"
lst = [1, 2]
lst.append(3)
lst.append(4)
lst
";
			var list = (List<object>)script.Eval(code);
			Assert.AreEqual(4, list.Count);
			Assert.AreEqual(4L, list[3]);

			var list2 = (List<object>)script.Eval("[1, 2] + [3, 4]");
			Assert.AreEqual(4, list2.Count);
		}

		[TestMethod]
		public void TestList_Remove()
		{
			var script = CreateScript();
			var list = (List<object>)script.Eval(@"
lst = [1, 2, 3, 2]
lst.remove(2)
lst
");
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(3L, list[1]);

			var list2 = (List<object>)script.Eval(@"
lst = [1, 2, 3]
lst.pop()
lst
");
			Assert.AreEqual(2, list2.Count);
		}

		[TestMethod]
		public void TestList_Len()
		{
			var script = CreateScript();
			Assert.AreEqual(0L, script.Eval("len([])"));
			Assert.AreEqual(3L, script.Eval("len([1, 2, 3])"));
			Assert.AreEqual(4L, script.Eval("len([1, [2, 3], 4, 5])"));
		}

		[TestMethod]
		public void TestList_Contains()
		{
			var script = CreateScript();
			Assert.AreEqual(true, script.Eval("1 in [1, 2, 3]"));
			Assert.AreEqual(false, script.Eval("4 in [1, 2, 3]"));
			Assert.AreEqual(true, script.Eval("'a' in ['a', 'b', 'c']"));
		}

		[TestMethod]
		public void TestList_CompileMode()
		{
			var script = CreateScript(compileMode: true);
			var list = (List<object>)script.Eval("[1, 2, 3]");
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2L, script.Eval("[1, 2, 3][1]"));
		}

		#endregion

		#region 集合测试

		[TestMethod]
		public void TestSet_Create()
		{
			var script = CreateScript();
			var set = (HashSet<object>)script.Eval("{1, 2, 3}");
			Assert.AreEqual(3, set.Count);
			Assert.IsTrue(set.Contains(1L));
			Assert.IsTrue(set.Contains(2L));
			Assert.IsTrue(set.Contains(3L));
		}

		[TestMethod]
		public void TestSet_Empty()
		{
			var script = CreateScript();
			// 注意：{} 在 Python 中是字典，不是空集合
			// 空集合需要用 set() 创建
			var dict = script.Eval("{}");
			Assert.IsTrue(dict is Dictionary<object, object>);
		}

		[TestMethod]
		public void TestSet_Add_2()
		{
			var script = CreateScript(true);
			var set = (HashSet<object>)script.Eval(@"
s = {1, 2}
s.add(3)
s.add(2)
s
");
			Assert.AreEqual(3, set.Count);
		}

		[TestMethod]
		public void TestSet_Add()
		{
			var script = CreateScript();
			var set = (HashSet<object>)script.Eval(@"
s = {1, 2}
s.add(3)
s.add(2)
s
");
			Assert.AreEqual(3, set.Count);
		}

		[TestMethod]
		public void TestSet_Operations_2()
		{
			var script = CreateScript(true);
			var set1 = (HashSet<object>)script.Eval("{1, 2, 3}");
			var set2 = (HashSet<object>)script.Eval("{2, 3, 4}");

			script.Eval("s1 = {1, 2, 3}");
			script.Eval("s2 = {2, 3, 4}");

			var union2 = (HashSet<object>)script.Eval("{1,2,3} | {2,3,4}");
			Assert.AreEqual(4, union2.Count);

			// 并集
			var union = (HashSet<object>)script.Eval("s1 | s2");
			Assert.AreEqual(4, union.Count);

			// 交集
			var inter = (HashSet<object>)script.Eval("s1 & s2");
			Assert.AreEqual(2, inter.Count);

			// 差集
			var diff = (HashSet<object>)script.Eval("s1 - s2");
			Assert.AreEqual(1, diff.Count);
		}

		[TestMethod]
		public void TestSet_Operations()
		{
			var script = CreateScript();
			var set1 = (HashSet<object>)script.Eval("{1, 2, 3}");
			var set2 = (HashSet<object>)script.Eval("{2, 3, 4}");

			script.Eval("s1 = {1, 2, 3}");
			script.Eval("s2 = {2, 3, 4}");

			// 并集
			var union = (HashSet<object>)script.Eval("s1 | s2");
			Assert.AreEqual(4, union.Count);

			var union2 = (HashSet<object>)script.Eval("{1,2,3} | {2,3,4}");
			Assert.AreEqual(4, union2.Count);

			// 交集
			var inter = (HashSet<object>)script.Eval("s1 & s2");
			Assert.AreEqual(2, inter.Count);

			// 差集
			var diff = (HashSet<object>)script.Eval("s1 - s2");
			Assert.AreEqual(1, diff.Count);
		}

		[TestMethod]
		public void TestSet_Len()
		{
			var script = CreateScript();
			Assert.AreEqual(3L, script.Eval("len({1, 2, 3})"));
			Assert.AreEqual(0L, script.Eval("len({})"));
		}

		#endregion

		#region 字典测试

		[TestMethod]
		public void TestDict_Empty()
		{
			var script = CreateScript();
			var dict = (Dictionary<object, object>)script.Eval("{}");
			Assert.AreEqual(0, dict.Count);
		}

		[TestMethod]
		public void TestDict_StringKey()
		{
			var script = CreateScript();
			var dict = (Dictionary<object, object>)script.Eval("{'name': '张三', 'age': 18}");
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual("张三", dict["name"]);
			Assert.AreEqual(18L, dict["age"]);
		}

		[TestMethod]
		public void TestDict_NumberKey()
		{
			var script = CreateScript();
			var dict = (Dictionary<object, object>)script.Eval("{1: 'one', 2: 'two', 3: 'three'}");
			Assert.AreEqual(3, dict.Count);
			Assert.AreEqual("one", dict[1L]);
			Assert.AreEqual("two", dict[2L]);
		}

		[TestMethod]
		public void TestDict_VariableKey()
		{
			var script = CreateScript();
			script.Context.SetVar("key", "mykey");
			var dict = (Dictionary<object, object>)script.Eval("{key: 'value'}");
			Assert.AreEqual(1, dict.Count);
			Assert.AreEqual("value", dict["mykey"]);
		}

		[TestMethod]
		public void TestDict_MixedValues()
		{
			var script = CreateScript();
			var dict = (Dictionary<object, object>)script.Eval("{'list': [1, 2, 3], 'dict': {'a': 1}, 'num': 42}");
			Assert.AreEqual(3, dict.Count);
			var list = (List<object>)dict["list"];
			Assert.AreEqual(3, list.Count);
			var nested = (Dictionary<object, object>)dict["dict"];
			Assert.AreEqual(1L, nested["a"]);
			Assert.AreEqual(42L, dict["num"]);
		}

		[TestMethod]
		public void TestDict_Access()
		{
			var script = CreateScript();
			script.Eval("d = {'name': '张三', 'age': 18}");
			Assert.AreEqual("张三", script.Eval("d['name']"));
			Assert.AreEqual(18L, script.Eval("d['age']"));
		}

		[TestMethod]
		public void TestDict_Len()
		{
			var script = CreateScript();
			Assert.AreEqual(0L, script.Eval("len({})"));
			Assert.AreEqual(2L, script.Eval("len({'a': 1, 'b': 2})"));
		}

		[TestMethod]
		public void TestDict_Contains()
		{
			var script = CreateScript();
			script.Eval("d = {'name': '张三', 'age': 18}");
			Assert.AreEqual(true, script.Eval("'name' in d"));
			Assert.AreEqual(false, script.Eval("'address' in d"));
		}

		[TestMethod]
		public void TestDict_Keys()
		{
			var script = CreateScript();
			var keys = (ICollection)script.Eval("{'a': 1, 'b': 2}.keys()");
			Assert.AreEqual(2, keys.Count);
		}

		[TestMethod]
		public void TestDict_Values()
		{
			var script = CreateScript();
			var values = (ICollection)script.Eval("{'a': 1, 'b': 2}.values()");
			Assert.AreEqual(2, values.Count);
		}

		[TestMethod]
		public void TestDict_Items()
		{
			var script = CreateScript();
			var items = (ICollection)script.Eval("{'a': 1, 'b': 2}.items()");
			Assert.AreEqual(2, items.Count);
		}

		[TestMethod]
		public void TestDict_Update()
		{
			var script = CreateScript();
			var dict = (Dictionary<object, object>)script.Eval(@"
d = {'name': '张三'}
d['age'] = 18
d['name'] = '李四'
d
");
			Assert.AreEqual("李四", dict["name"]);
			Assert.AreEqual(18L, dict["age"]);
		}

		[TestMethod]
		public void TestDict_CompileMode()
		{
			var script = CreateScript(compileMode: true);
			var dict = (Dictionary<object, object>)script.Eval("{'name': '张三', 'age': 18}");
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual("张三", dict["name"]);
		}

		#endregion

		#region 类型注解测试

		[TestMethod]
		public void TestTypeAnnotation_Variable()
		{
			var script = CreateScript();
			script.Eval("a : int = 10");
			Assert.AreEqual(10L, script.Eval("a"));
		}

		[TestMethod]
		public void TestTypeAnnotation_FuncParam()
		{
			var script = CreateScript();
			script.Eval(@"
def add(a : int, b : int) -> int:
    return a + b
");
			Assert.AreEqual(30L, script.Eval("add(10, 20)"));
		}

		[TestMethod]
		public void TestTypeAnnotation_StringType()
		{
			var script = CreateScript();
			script.Eval("name : str = 'hello'");
			Assert.AreEqual("hello", script.Eval("name"));
		}

		[TestMethod]
		public void TestTypeAnnotation_BoolType()
		{
			var script = CreateScript();
			script.Eval("flag : bool = True");
			Assert.AreEqual(true, script.Eval("flag"));
		}

		[TestMethod]
		public void TestTypeAnnotation_FloatType()
		{
			var script = CreateScript();
			script.Eval("pi : float = 3.14");
			Assert.AreEqual(3.14, script.Eval("pi"));
		}

		[TestMethod]
		public void TestTypeAnnotation_ListType()
		{
			var script = CreateScript();
			script.Eval("nums : list = [1, 2, 3]");
			var list = (List<object>)script.Eval("nums");
			Assert.AreEqual(3, list.Count);
		}

		[TestMethod]
		public void TestTypeAnnotation_DictType()
		{
			var script = CreateScript();
			script.Eval("d : dict = {'a': 1}");
			var dict = (Dictionary<object, object>)script.Eval("d");
			Assert.AreEqual(1, dict.Count);
		}

		#endregion

		#region 函数测试

		[TestMethod]
		public void TestFunc_Define()
		{
			var script = CreateScript();
			script.Eval(@"
def greet():
    return 'hello'
");
			Assert.AreEqual("hello", script.Eval("greet()"));
		}

		[TestMethod]
		public void TestFunc_Params()
		{
			var script = CreateScript();
			script.Eval(@"
def add(a, b):
    return a + b
");
			Assert.AreEqual(30L, script.Eval("add(10, 20)"));
			Assert.AreEqual("abc", script.Eval("add('a', 'bc')"));
		}

//		[TestMethod]
//		public void TestFunc_DefaultParams()
//		{
//			var script = CreateScript();
//			script.Eval(@"
//def greet(name = 'world'):
//    return 'hello ' + name
//");
//			Assert.AreEqual("hello world", script.Eval("greet()"));
//			Assert.AreEqual("hello tom", script.Eval("greet('tom')"));
//		}

		//[TestMethod]
		//public void TestFunc_VarArgs()
		//{
		//    var script = CreateScript();
		//    Assert.AreEqual(6L, script.Eval("sum(1, 2, 3)"));
		//    Assert.AreEqual(10L, script.Eval("sum(1, 2, 3, 4)"));
		//}

		[TestMethod]
		public void TestFunc_Lambda_3()
		{
			string s1 = @"
f = lambda x: x * 2
f(3)
";
			var script = CreateScript(true);
			Assert.AreEqual(6L, script.Eval(s1));

			string s2 = @"
add = lambda a, b: a + b
add(10,20)
";
			Assert.AreEqual(30L, script.Eval(s2));
		}

		[TestMethod]
		public void TestFunc_Lambda_2()
		{
			var script = CreateScript(true);
			script.Eval("f = lambda x: x * 2");
			Assert.AreEqual(6L, script.Eval("f(3)"));

			script.Eval("add = lambda a, b: a + b");
			Assert.AreEqual(30L, script.Eval("add(10, 20)"));
		}

		[TestMethod]
		public void TestFunc_Lambda()
		{
			var script = CreateScript();
			script.Eval("f = lambda x: x * 2");
			Assert.AreEqual(6L, script.Eval("f(3)"));

			script.Eval("add = lambda a, b: a + b");
			Assert.AreEqual(30L, script.Eval("add(10, 20)"));
		}

		[TestMethod]
		public void TestFunc_Nested()
		{
			var script = CreateScript();
			script.Eval(@"
def outer():
    x = 10
    def inner():
        return x + 5
    return inner()
");
			Assert.AreEqual(15L, script.Eval("outer()"));
		}

		//        [TestMethod]
		//        public void TestFunc_Closure()
		//        {
		//            var script = CreateScript();
		//            script.Eval(@"
		//def counter():
		//    count = 0
		//    def add():
		//        nonlocal count
		//        count += 1
		//        return count
		//    return add()
		//");
		//            // 注意：closure 行为可能不同
		//        }

		#endregion

		#region 字符串测试

		[TestMethod]
		public void TestString_Index_2()
		{
			var script = CreateScript(true);
			Assert.AreEqual("a", script.Eval("'abc'[0]"));
			Assert.AreEqual("c", script.Eval("'abc'[-1]"));
		}

		[TestMethod]
		public void TestString_Index()
		{
			var script = CreateScript();
			Assert.AreEqual("a", script.Eval("'abc'[0]"));
			Assert.AreEqual("c", script.Eval("'abc'[-1]"));
		}

		[TestMethod]
		public void TestString_Slice()
		{
			var script = CreateScript();
			Assert.AreEqual("ab", script.Eval("'abc'[:2]"));
			Assert.AreEqual("bc", script.Eval("'abc'[1:]"));
			Assert.AreEqual("b", script.Eval("'abc'[1:2]"));
		}

		[TestMethod]
		public void TestString_FString()
		{
			var script = CreateScript();
			script.Context.SetVar("name", "张三");
			Assert.AreEqual("hello 张三", script.Eval("f'hello {name}'"));
		}

		[TestMethod]
		public void TestString_Methods()
		{
			var script = CreateScript();
			Assert.AreEqual(5L, script.Eval("len('hello')"));
			Assert.AreEqual("HELLO", script.Eval("'hello'.upper()"));
			Assert.AreEqual("hello", script.Eval("'HELLO'.lower()"));
			Assert.AreEqual("world", script.Eval("'hello world'.split(' ')[1]"));
		}

		#endregion

		#region 综合测试

		[TestMethod]
		public void TestComplex_Fibonacci()
		{
			var script = CreateScript();
			var code = @"
def fib(n):
    if n <= 1:
        return n
    return fib(n - 1) + fib(n - 2)
";
			script.Eval(code);
			Assert.AreEqual(0L, script.Eval("fib(0)"));
			Assert.AreEqual(1L, script.Eval("fib(1)"));
			Assert.AreEqual(1L, script.Eval("fib(2)"));
			Assert.AreEqual(2L, script.Eval("fib(3)"));
			Assert.AreEqual(55L, script.Eval("fib(10)"));
		}

		[TestMethod]
		public void TestComplex_SumList()
		{
			var script = CreateScript();
			var code = @"
total = 0
for x in [1, 2, 3, 4, 5]:
    total += x
";
			script.Eval(code);
			Assert.AreEqual(15L, script.Eval("total"));
		}

		#endregion
	}
}
