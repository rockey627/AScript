using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptQueryableTest
	{
		#region AsQueryable / Where
		[TestMethod]
		public void Test01_AsQueryable()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			var q = (IQueryable<int>)r;
			Assert.AreEqual("1,2,3,4,5", string.Join(',', q));
			Assert.AreEqual("2,4", string.Join(',', q.Where(a => a % 2 == 0)));
		}

		[TestMethod]
		public void Test01_AsQueryable_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			var q = (IQueryable<int>)r;
			Assert.AreEqual("1,2,3,4,5", string.Join(',', q));
			Assert.AreEqual("2,4", string.Join(',', q.Where(a => a % 2 == 0)));
		}

		[TestMethod]
		public void Test02_Where_ToList()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0).ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4", string.Join(',', (List<int>)r));
		}

		[TestMethod]
		public void Test02_Where_ToList_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0).ToList();
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4", string.Join(',', (List<int>)r));
		}

		[TestMethod]
		public void Test03_Where_IQueryable()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("2,4", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void Test03_Where_IQueryable_CompileModeAll()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0);
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("2,4", string.Join(',', (IQueryable<int>)r));
		}
		#endregion

		#region Select
		[TestMethod]
		public void TestSelect()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Select(a=>a*2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("2,4,6", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestSelect_ToList()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Select(a=>a*2).ToList();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(List<int>));
			Assert.AreEqual("2,4,6", string.Join(',', (List<int>)r));
		}

		[TestMethod]
		public void TestSelect_String()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Select(a=>""item""+a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<string>));
			Assert.AreEqual("item1,item2,item3", string.Join(',', (IQueryable<string>)r));
		}
		#endregion

		#region OrderBy / ThenBy
		[TestMethod]
		public void TestOrderBy()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6].AsQueryable();
var r = q.OrderBy(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("1,1,2,3,4,5,6,9", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestOrderByDescending()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6].AsQueryable();
var r = q.OrderByDescending(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("9,6,5,4,3,2,1,1", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestThenBy()
		{
			string s = @"
var q = [[1,'b'],[2,'a'],[1,'a'],[2,'b']].AsQueryable();
var r = q.OrderBy(a=>a[0]).ThenBy(a=>a[1]);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<object>));
			var list = ((IQueryable<object>)r).ToList();
			Assert.AreEqual(4, list.Count);
		}
		#endregion

		#region Take / Skip
		[TestMethod]
		public void TestTake()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("1,2,3", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestSkip()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Skip(2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("3,4,5", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestSkipTake()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Skip(1).Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("2,3,4", string.Join(',', (IQueryable<int>)r));
		}
		#endregion

		#region Distinct
		[TestMethod]
		public void TestDistinct()
		{
			string s = @"
var q = [1,2,2,3,3,3].AsQueryable();
var r = q.Distinct();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("1,2,3", string.Join(',', (IQueryable<int>)r));
		}
		#endregion

		#region ElementAt / First / Last / Single
		[TestMethod]
		public void TestElementAt()
		{
			string s = @"
var q = [10,20,30].AsQueryable();
var r = q.ElementAt(1);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(20, r);
		}

		[TestMethod]
		public void TestFirst()
		{
			string s = @"
var q = [10,20,30].AsQueryable();
var r = q.First();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(10, r);
		}

		[TestMethod]
		public void TestFirst_Predicate()
		{
			string s = @"
var q = [10,20,30].AsQueryable();
var r = q.First(a=>a>20);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestLast()
		{
			string s = @"
var q = [10,20,30].AsQueryable();
var r = q.Last();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(30, r);
		}

		[TestMethod]
		public void TestSingle()
		{
			string s = @"
var q = [42].AsQueryable();
var r = q.Single();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(42, r);
		}

		[TestMethod]
		public void TestSingle_Predicate()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Single(a=>a==2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}
		#endregion

		#region Quantifiers (Any / All / Contains)
		[TestMethod]
		public void TestAny()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Any(a=>a>2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAny_False()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Any(a=>a>10);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestAll()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.All(a=>a>0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}

		[TestMethod]
		public void TestAll_False()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.All(a=>a>1);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestContains()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Contains(2);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(true, r);
		}
		#endregion

		#region Aggregation (Count / Sum / Average / Min / Max)
		[TestMethod]
		public void TestCount()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Count();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(5, r);
		}

		[TestMethod]
		public void TestCount_Predicate()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Count(a=>a%2==0);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestSum()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Sum();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(15, r);
		}

		[TestMethod]
		public void TestAverage()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Average();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(3.0, r);
		}

		[TestMethod]
		public void TestMin()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6].AsQueryable();
var r = q.Min();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(1, r);
		}

		[TestMethod]
		public void TestMax()
		{
			string s = @"
var q = [3,1,4,1,5,9,2,6].AsQueryable();
var r = q.Max();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(9, r);
		}
		#endregion

		#region Chaining (complex queries)
		[TestMethod]
		public void TestComplex_WhereSelectOrderBy()
		{
			string s = @"
var q = [1,2,3,4,5,6,7,8,9,10].AsQueryable();
var r = q.Where(a=>a>3).Select(a=>a*2).OrderBy(a=>a);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("8,10,12,14,16,18,20", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestComplex_OrderBySkipTake()
		{
			string s = @"
var q = [5,3,8,1,9,2,7,4,6].AsQueryable();
var r = q.OrderByDescending(a=>a).Skip(2).Take(3);
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("7,6,5", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestComplex_WhereCount()
		{
			string s = @"
var q = [1,2,3,4,5].AsQueryable();
var r = q.Where(a=>a%2==0).Count();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(2, r);
		}

		[TestMethod]
		public void TestComplex_WhereAny()
		{
			string s = @"
var q = [1,2,3].AsQueryable();
var r = q.Where(a=>a>10).Any();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(false, r);
		}

		[TestMethod]
		public void TestComplex_WhereSelectDistinct()
		{
			string s = @"
var q = [1,1,2,2,3,3].AsQueryable();
var r = q.Where(a=>a>1).Distinct();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.IsInstanceOfType(r, typeof(IQueryable<int>));
			Assert.AreEqual("2,3", string.Join(',', (IQueryable<int>)r));
		}

		[TestMethod]
		public void TestComplex_SelectSum()
		{
			string s = @"
var q = [1,2,3,4].AsQueryable();
var r = q.Select(a=>a*10).Sum();
";
			var script = new Script();
			var r = script.Eval(s);
			Assert.AreEqual(100, r);
		}
		#endregion
	}
}
