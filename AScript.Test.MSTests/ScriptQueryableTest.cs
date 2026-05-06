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
		[TestMethod]
		public void Test02_Where()
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
	}
}
