using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptEnumerableTest
	{
		#region Where
		[TestMethod]
		public void TestWhere_IEnumerable()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestWhere_IEnumerable_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestWhere_ToList()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0).ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4", string.Join(",", (List<int>)r));
		}

		[TestMethod]
		public void TestWhere_ToList_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0).ToList();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4", string.Join(",", (List<int>)r));
		}
		#endregion

		#region Select
		[TestMethod]
		public void TestSelect()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>a*2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,4,6", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSelect_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>a*2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,4,6", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSelect_ToList()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>a*2).ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4,6", string.Join(",", (List<int>)r));
		}

		[TestMethod]
		public void TestSelect_ToList_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>a*2).ToList();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4,6", string.Join(",", (List<int>)r));
		}

		[TestMethod]
		public void TestSelect_String()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>""item""+a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<string>));
			Assert.AreEqual("item1,item2,item3", string.Join(",", (IEnumerable<string>)r));
		}

		[TestMethod]
		public void TestSelect_String_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Select(a=>""item""+a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<string>));
			Assert.AreEqual("item1,item2,item3", string.Join(",", (IEnumerable<string>)r));
		}

		[TestMethod]
		public void TestSelectMany()
		{
			string s = @"
var q = [[1,2],[3,4],[5]];
var r = q.SelectMany(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSelectMany_CompileModeAll()
		{
			string s = @"
var q = [[1,2],[3,4],[5]];
var r = q.SelectMany(a=>a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5", string.Join(",", (IEnumerable<int>)r));
		}
		#endregion

		#region OrderBy / ThenBy
		[TestMethod]
		public void TestOrderBy()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.OrderBy(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,1,2,3,4,5,6,9", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestOrderBy_CompileModeAll()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.OrderBy(a=>a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,1,2,3,4,5,6,9", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestOrderByDescending()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.OrderByDescending(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("9,6,5,4,3,2,1,1", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestOrderByDescending_CompileModeAll()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.OrderByDescending(a=>a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("9,6,5,4,3,2,1,1", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestThenBy()
		{
			string s = @"
var q = [[1,'b'],[2,'a'],[1,'a'],[2,'b']];
var r = q.OrderBy(a=>a[0]).ThenBy(a=>a[1]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<object>));
			var list = ((IEnumerable<object>)r).ToList();
			Assert.AreEqual(4, list.Count);
		}

		[TestMethod]
		public void TestThenBy_CompileModeAll()
		{
			string s = @"
var q = [[1,'b'],[2,'a'],[1,'a'],[2,'b']];
var r = q.OrderBy(a=>a[0]).ThenBy(a=>a[1]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<object>));
			var list = ((IEnumerable<object>)r).ToList();
			Assert.AreEqual(4, list.Count);
		}

		[TestMethod]
		public void TestThenByDescending()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[1,'b'],[2,'a']];
var r = q.OrderBy(a=>a[0]).ThenByDescending(a=>a[1]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<object>));
			var list = ((IEnumerable<object>)r).ToList();
			Assert.AreEqual(4, list.Count);
		}

		[TestMethod]
		public void TestThenByDescending_CompileModeAll()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[1,'b'],[2,'a']];
var r = q.OrderBy(a=>a[0]).ThenByDescending(a=>a[1]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<object>));
			var list = ((IEnumerable<object>)r).ToList();
			Assert.AreEqual(4, list.Count);
		}
		#endregion

		#region Take / Skip
		[TestMethod]
		public void TestTake()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestTake_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Take(3);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkip()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Skip(2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkip_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Skip(2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkipTake()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Skip(1).Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,3,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkipTake_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Skip(1).Take(3);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,3,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestTakeWhile()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.TakeWhile(a=>a<4);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestTakeWhile_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.TakeWhile(a=>a<4);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkipWhile()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.SkipWhile(a=>a<3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestSkipWhile_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.SkipWhile(a=>a<3);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (IEnumerable<int>)r));
		}
		#endregion

		#region Distinct / Union / Intersect / Except
		[TestMethod]
		public void TestDistinct()
		{
			string s = @"
var q = [1,2,2,3,3,3];
var r = q.Distinct();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestDistinct_CompileModeAll()
		{
			string s = @"
var q = [1,2,2,3,3,3];
var r = q.Distinct();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestUnion()
		{
			string s = @"
var q1 = [1,2,3];
var q2 = [3,4,5];
var r = q1.Union(q2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestUnion_CompileModeAll()
		{
			string s = @"
var q1 = [1,2,3];
var q2 = [3,4,5];
var r = q1.Union(q2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestIntersect()
		{
			string s = @"
var q1 = [1,2,3,4];
var q2 = [3,4,5,6];
var r = q1.Intersect(q2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestIntersect_CompileModeAll()
		{
			string s = @"
var q1 = [1,2,3,4];
var q2 = [3,4,5,6];
var r = q1.Intersect(q2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("3,4", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestExcept()
		{
			string s = @"
var q1 = [1,2,3,4];
var q2 = [3,4,5,6];
var r = q1.Except(q2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestExcept_CompileModeAll()
		{
			string s = @"
var q1 = [1,2,3,4];
var q2 = [3,4,5,6];
var r = q1.Except(q2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2", string.Join(",", (IEnumerable<int>)r));
		}
		#endregion

		#region ElementAt / First / Last / Single
		[TestMethod]
		public void TestElementAt()
		{
			string s = @"
var q = [10,20,30];
var r = q.ElementAt(1);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(20, r);
		}

		[TestMethod]
		public void TestElementAt_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.ElementAt(1);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(20, r);
		}

		[TestMethod]
		public void TestElementAtOrDefault()
		{
			string s = @"
var q = [10,20,30];
var r = q.ElementAtOrDefault(10);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestElementAtOrDefault_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.ElementAtOrDefault(10);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestFirst()
		{
			string s = @"
var q = [10,20,30];
var r = q.First();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(10, r);
		}

		[TestMethod]
		public void TestFirst_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.First();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(10, r);
		}

		[TestMethod]
		public void TestFirst_Predicate()
		{
			string s = @"
var q = [10,20,30];
var r = q.First(a=>a>20);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestFirst_Predicate_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.First(a=>a>20);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestFirstOrDefault()
		{
			string s = @"
var q = [10,20,30];
var r = q.FirstOrDefault(a=>a>100);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestFirstOrDefault_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.FirstOrDefault(a=>a>100);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestLast()
		{
			string s = @"
var q = [10,20,30];
var r = q.Last();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestLast_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.Last();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestLast_Predicate()
		{
			string s = @"
var q = [10,20,30];
var r = q.Last(a=>a<30);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(20, r);
		}

		[TestMethod]
		public void TestLast_Predicate_CompileModeAll()
		{
			string s = @"
var q = [10,20,30];
var r = q.Last(a=>a<30);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(20, r);
		}

		[TestMethod]
		public void TestSingle()
		{
			string s = @"
var q = [42];
var r = q.Single();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(42, r);
		}

		[TestMethod]
		public void TestSingle_CompileModeAll()
		{
			string s = @"
var q = [42];
var r = q.Single();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(42, r);
		}

		[TestMethod]
		public void TestSingle_Predicate()
		{
			string s = @"
var q = [1,2,3];
var r = q.Single(a=>a==2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestSingle_Predicate_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Single(a=>a==2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestSingleOrDefault()
		{
			string s = @"
var q = [];
var r = q.SingleOrDefault();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(new object[0].SingleOrDefault(), r);
		}

		[TestMethod]
		public void TestSingleOrDefault_CompileModeAll()
		{
			string s = @"
var q = [];
var r = q.SingleOrDefault();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(new object[0].SingleOrDefault(), r);
		}
		#endregion

		#region Quantifiers (Any / All / Contains)
		[TestMethod]
		public void TestAny_NoPredicate()
		{
			string s = @"
var q = [1,2,3];
var r = q.Any();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAny_NoPredicate_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Any();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAny_False()
		{
			string s = @"
var q = [];
var r = q.Any();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestAny_False_CompileModeAll()
		{
			string s = @"
var q = [];
var r = q.Any();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestAny_Predicate()
		{
			string s = @"
var q = [1,2,3];
var r = q.Any(a=>a>2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAny_Predicate_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Any(a=>a>2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.All(a=>a>0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAll_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.All(a=>a>0);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAll_False()
		{
			string s = @"
var q = [1,2,3];
var r = q.All(a=>a>1);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestAll_False_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.All(a=>a>1);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestContains()
		{
			string s = @"
var q = [1,2,3];
var r = q.Contains(2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestContains_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.Contains(2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestContains_Comparer()
		{
			string s = @"
var q = ['a','b','c'];
var r = q.Contains('B');
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestContains_Comparer_CompileModeAll()
		{
			string s = @"
var q = ['a','b','c'];
var r = q.Contains('B');
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}
		#endregion

		#region Aggregation (Count / Sum / Average / Min / Max)
		[TestMethod]
		public void TestCount()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Count();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(5, r);
		}

		[TestMethod]
		public void TestCount_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Count();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(5, r);
		}

		[TestMethod]
		public void TestCount_Predicate()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Count(a=>a%2==0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestCount_Predicate_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Count(a=>a%2==0);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestSum()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Sum();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(15, r);
		}

		[TestMethod]
		public void TestSum_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Sum();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(15, r);
		}

		[TestMethod]
		public void TestSum_Selector()
		{
			string s = @"
var q = [[1,2],[3,4],[5,6]];
var r = q.Sum(a=>a[0]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(9, r);
		}

		[TestMethod]
		public void TestSum_Selector_CompileModeAll()
		{
			string s = @"
var q = [[1,2],[3,4],[5,6]];
var r = q.Sum(a=>a[0]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(9, r);
		}

		[TestMethod]
		public void TestAverage()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Average();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(3.0, r);
		}

		[TestMethod]
		public void TestAverage_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Average();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(3.0, r);
		}

		[TestMethod]
		public void TestAverage_Selector()
		{
			string s = @"
var q = [[1,10],[2,20],[3,30]];
var r = q.Average(a=>a[1]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(20.0, r);
		}

		[TestMethod]
		public void TestAverage_Selector_CompileModeAll()
		{
			string s = @"
var q = [[1,10],[2,20],[3,30]];
var r = q.Average(a=>a[1]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(20.0, r);
		}

		[TestMethod]
		public void TestMin()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.Min();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(1, r);
		}

		[TestMethod]
		public void TestMin_CompileModeAll()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.Min();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(1, r);
		}

		[TestMethod]
		public void TestMin_Selector()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[0,'c']];
var r = q.Min(a=>a[0]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestMin_Selector_CompileModeAll()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[0,'c']];
var r = q.Min(a=>a[0]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(0, r);
		}

		[TestMethod]
		public void TestMax()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.Max();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(9, r);
		}

		[TestMethod]
		public void TestMax_CompileModeAll()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6];
var r = q.Max();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(9, r);
		}

		[TestMethod]
		public void TestMax_Selector()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[0,'c']];
var r = q.Max(a=>a[0]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestMax_Selector_CompileModeAll()
		{
			string s = @"
var q = [[1,'a'],[2,'b'],[0,'c']];
var r = q.Max(a=>a[0]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestAggregate()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Aggregate((a,b)=>a+b);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(15, r);
		}

		[TestMethod]
		public void TestAggregate_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Aggregate((a,b)=>a+b);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(15, r);
		}
		#endregion

		#region Conversion (ToList / ToArray / ToHashSet)
		[TestMethod]
		public void TestToList()
		{
			string s = @"
var q = [1,2,3];
var r = q.ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual(3, ((List<int>)r).Count);
		}

		[TestMethod]
		public void TestToList_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.ToList();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual(3, ((List<int>)r).Count);
		}

		[TestMethod]
		public void TestToArray()
		{
			string s = @"
var q = [1,2,3];
var r = q.ToArray();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(int[]));
			Assert.AreEqual(3, ((int[])r).Length);
		}

		[TestMethod]
		public void TestToArray_CompileModeAll()
		{
			string s = @"
var q = [1,2,3];
var r = q.ToArray();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(int[]));
			Assert.AreEqual(3, ((int[])r).Length);
		}

		[TestMethod]
		public void TestToHashSet()
		{
			string s = @"
var q = [1,2,2,3];
var r = q.ToHashSet();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(HashSet<int>));
			Assert.AreEqual(3, ((HashSet<int>)r).Count);
		}

		[TestMethod]
		public void TestToHashSet_CompileModeAll()
		{
			string s = @"
var q = [1,2,2,3];
var r = q.ToHashSet();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(HashSet<int>));
			Assert.AreEqual(3, ((HashSet<int>)r).Count);
		}
		#endregion

		#region GroupBy
		[TestMethod]
		public void TestGroupBy()
		{
			string s = @"
var q = [1,2,3,4,5,6];
var r = q.GroupBy(a=>a%2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<IGrouping<int,int>>));
			var groups = ((IEnumerable<IGrouping<int,int>>)r).ToList();
			Assert.AreEqual(2, groups.Count);
		}

		[TestMethod]
		public void TestGroupBy_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5,6];
var r = q.GroupBy(a=>a%2);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<IGrouping<int,int>>));
			var groups = ((IEnumerable<IGrouping<int,int>>)r).ToList();
			Assert.AreEqual(2, groups.Count);
		}

		[TestMethod]
		public void TestGroupBy_ToDictionary()
		{
			string s = @"
var q = [1,2,3,4,5,6];
var r = q.GroupBy(a=>a%2).ToDictionary(g=>g.Key, g=>g.ToList());
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(Dictionary<int, List<int>>));
			var dict = (Dictionary<int, List<int>>)r;
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(3, dict[0].Count);
			Assert.AreEqual(3, dict[1].Count);
		}

		[TestMethod]
		public void TestGroupBy_ToDictionary_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5,6];
var r = q.GroupBy(a=>a%2).ToDictionary(g=>g.Key, g=>g.ToList());
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(Dictionary<int, List<int>>));
			var dict = (Dictionary<int, List<int>>)r;
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(3, dict[0].Count);
			Assert.AreEqual(3, dict[1].Count);
		}
		#endregion

		#region Join
		[TestMethod]
		public void TestJoin()
		{
			var outer = new[] { new[] { 1, 'a' }, new[]{ 2, 'b' }, new[] { 3, 'c' } };
			var inner = new[] { new[] { 1, 100 }, new[] { 2, 200 }, new[] { 4, 400 } };
			var r = outer.Join(inner, o => o[0], i => i[0], (o, i) => o[1] + " - " + i[1]);
			Assert.IsTrue(r is IEnumerable<string>);

			string s = @"
var outer = [[1,'a'],[2,'b'],[3,'c']];
var inner = [[1,100],[2,200],[4,400]];
var r = outer.Join(inner, o=>o[0], i=>i[0], (o,i)=>o[1]+""-""+i[1]);
";
			var script = new Script();
			var result = script.Eval(s);
			Assert.IsTrue(result is IEnumerable<string>);
			var list = ((IEnumerable<string>)result).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list.Contains("a-100"));
			Assert.IsTrue(list.Contains("b-200"));
		}

		[TestMethod]
		public void TestJoin_CompileModeAll()
		{
			string s = @"
var outer = [[1,'a'],[2,'b'],[3,'c']];
var inner = [[1,100],[2,200],[4,400]];
var r = outer.Join(inner, o=>o[0], i=>i[0], (o,i)=>o[1]+""-""+i[1]);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<string>));
			var list = ((IEnumerable<string>)r).ToList();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list.Contains("a-100"));
			Assert.IsTrue(list.Contains("b-200"));
		}
		#endregion

		#region Chaining (complex queries)
		[TestMethod]
		public void TestComplex_WhereSelectOrderBy()
		{
			string s = @"
var q = [1,2,3,4,5,6,7,8,9,10];
var r = q.Where(a=>a>3).Select(a=>a*2).OrderBy(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("8,10,12,14,16,18,20", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_WhereSelectOrderBy_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5,6,7,8,9,10];
var r = q.Where(a=>a>3).Select(a=>a*2).OrderBy(a=>a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("8,10,12,14,16,18,20", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_OrderBySkipTake()
		{
			string s = @"
var q = [5,3,8,1,9,2,7,4,6];
var r = q.OrderByDescending(a=>a).Skip(2).Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("7,6,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_OrderBySkipTake_CompileModeAll()
		{
			string s = @"
var q = [5,3,8,1,9,2,7,4,6];
var r = q.OrderByDescending(a=>a).Skip(2).Take(3);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("7,6,5", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_WhereCount()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0).Count();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestComplex_WhereCount_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5];
var r = q.Where(a=>a%2==0).Count();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestComplex_WhereSelectDistinct()
		{
			string s = @"
var q = [1,1,2,2,3,3];
var r = q.Where(a=>a>1).Distinct();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_WhereSelectDistinct_CompileModeAll()
		{
			string s = @"
var q = [1,1,2,2,3,3];
var r = q.Where(a=>a>1).Distinct();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("2,3", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_SelectSum()
		{
			string s = @"
var q = [1,2,3,4];
var r = q.Select(a=>a*10).Sum();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(100, r);
		}

		[TestMethod]
		public void TestComplex_SelectSum_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4];
var r = q.Select(a=>a*10).Sum();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.AreEqual(100, r);
		}

		[TestMethod]
		public void TestComplex_SelectManyWhere()
		{
			string s = @"
var q = [[1,2,3],[4,5],[6,7,8]];
var r = q.SelectMany(a=>a).Where(a=>a>3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("4,5,6,7,8", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_SelectManyWhere_CompileModeAll()
		{
			string s = @"
var q = [[1,2,3],[4,5],[6,7,8]];
var r = q.SelectMany(a=>a).Where(a=>a>3);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("4,5,6,7,8", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_DistinctOrderBy()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6,3,5];
var r = q.Distinct().OrderBy(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5,6,9", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_DistinctOrderBy_CompileModeAll()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6,3,5];
var r = q.Distinct().OrderBy(a=>a);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IEnumerable<int>));
			Assert.AreEqual("1,2,3,4,5,6,9", string.Join(",", (IEnumerable<int>)r));
		}

		[TestMethod]
		public void TestComplex_TakeSkipReverse()
		{
			string s = @"
var q = [1,2,3,4,5,6,7,8,9,10];
var r = q.Take(5).Skip(2).ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (List<int>)r));
		}

		[TestMethod]
		public void TestComplex_TakeSkipReverse_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5,6,7,8,9,10];
var r = q.Take(5).Skip(2).ToList();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("3,4,5", string.Join(",", (List<int>)r));
		}
		#endregion
	}
}
