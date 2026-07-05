using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptAxiosTest
	{
		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["js"] = JavaScriptLang.Instance;
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("js");
		}

		// axios.get - simple object response
		[TestMethod]
		public void Test01_getObject()
		{
			string s = @"
var req = axios.createMock({a:1});
var resp = await req.get('http://test.com'); 
resp.data.a
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			script.Context.TryInstallModule("axios");
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject_CompileAll()
		{
			string s = @"
var req = axios.createMock({a:1});
var resp = await req.get('http://test.com'); 
resp.data.a
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			script.Context.TryInstallModule("axios");
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject2()
		{
			string s = @"
var axios = require('axios').createMock({a:1});
var resp = await axios.get('http://test.com').catch(err=>{console.log(err)}); 
resp.data.a
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject2_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({a:1});
var resp = await axios.get('http://test.com').catch(err=>{console.log(err)}); 
resp.data.a
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(1L, result);
		}

		// axios.get - array response
		[TestMethod]
		public void Test02_getArray()
		{
			string s = @"
var axios = require('axios').createMock([1, 2, 3]);
var resp = await axios.get('http://test.com'); 
resp.data[0] + resp.data[1] + resp.data[2]
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(6L, result);
		}

		[TestMethod]
		public void Test02_getArray_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock([1, 2, 3]);
var resp = await axios.get('http://test.com'); 
resp.data[0] + resp.data[1] + resp.data[2]
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(6L, result);
		}

		// axios.get - nested object response
		[TestMethod]
		public void Test03_getNested()
		{
			string s = @"
var axios = require('axios').createMock({arr:[1,2],obj:{x:1}});
var resp = await axios.get('http://test.com'); 
resp.data.arr[0] + resp.data.arr[1] + resp.data.obj.x
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("");
			Assert.AreEqual(4L, result);
		}

		[TestMethod]
		public void Test03_getNested_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({arr:[1,2],obj:{x:1}});
var resp = await axios.get('http://test.com'); 
resp.data.arr[0] + resp.data.arr[1] + resp.data.obj.x
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(4L, result);
		}

		// axios.get - string value
		[TestMethod]
		public void Test04_getString()
		{
			string s = @"
var axios = require('axios').createMock({name:'test',value:'hello'});
var resp = await axios.get('http://test.com'); 
resp.data.name + resp.data.value
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("testhello", result);
		}

		[TestMethod]
		public void Test04_getString_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({name:'test',value:'hello'});
var resp = await axios.get('http://test.com'); 
resp.data.name + resp.data.value
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("testhello", result);
		}

		// axios.get - boolean value
		[TestMethod]
		public void Test05_getBoolean()
		{
			string s = @"
var axios = require('axios').createMock({enabled:true,disabled:false});
var resp = await axios.get('http://test.com'); 
resp.data.enabled && !resp.data.disabled
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void Test05_getBoolean_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({enabled:true,disabled:false});
var resp = await axios.get('http://test.com'); 
resp.data.enabled && !resp.data.disabled
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
		}

		// axios.get - null value
		[TestMethod]
		public void Test06_getNull()
		{
			string s = @"
var axios = require('axios').createMock({value:null});
var resp = await axios.get('http://test.com'); 
resp.data.value
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Test06_getNull_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({value:null});
var resp = await axios.get('http://test.com'); 
resp.data.value
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.IsNull(result);
		}

		// axios.get - data is parsed only once (caching)
		[TestMethod]
		public void Test07_dataParsedOnce()
		{
			string s = @"
var axios = require('axios').createMock({a:1});
var resp = await axios.get('http://test.com'); 
var d1 = resp.data; 
var d2 = resp.data; 
d1.a + d2.a
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test07_dataParsedOnce_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({a:1});
var resp = await axios.get('http://test.com'); 
var d1 = resp.data; 
var d2 = resp.data; 
d1.a + d2.a
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(2L, result);
		}

		// axios.get - double value
		[TestMethod]
		public void Test08_getDouble()
		{
			string s = @"
var axios = require('axios').createMock({pi:3.14,price:19.99});
var resp = await axios.get('http://test.com'); 
resp.data.pi + resp.data.price
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(23.13, (double)result, 0.001);
		}

		[TestMethod]
		public void Test08_getDouble_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({pi:3.14,price:19.99});
var resp = await axios.get('http://test.com'); 
resp.data.pi + resp.data.price
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(23.13, (double)result, 0.001);
		}

		// axios.get - empty object
		[TestMethod]
		public void Test09_getEmptyObject()
		{
			string s = @"
var axios = require('axios').createMock({});
var resp = await axios.get('http://test.com'); 
resp.data != null
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void Test09_getEmptyObject_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({});
var resp = await axios.get('http://test.com'); 
resp.data != null
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
		}

		// axios.get - empty array
		[TestMethod]
		public void Test10_getEmptyArray()
		{
			string s = @"
var axios = require('axios').createMock([]);
var resp = await axios.get('http://test.com'); 
resp.data.length
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(0L, result);
		}

		[TestMethod]
		public void Test10_getEmptyArray_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock([]);
var resp = await axios.get('http://test.com'); 
resp.data.length
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>(s);
			Assert.AreEqual(0L, result);
		}

		// axios extension object registration
		[TestMethod]
		public void Test11_axiosRegistered()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var axios = script.Eval("axios");
			Assert.IsNotNull(axios);
		}

		[TestMethod]
		public void Test11_axiosRegistered_CompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var axios = script.Eval("axios");
			Assert.IsNotNull(axios);
		}

		// axios.get - multiple sequential requests
		[TestMethod]
		public void Test12_multipleRequests()
		{
			string s = @"
var axios = require('axios').createMock({value:1});
var r1 = await axios.get('http://test.com'); 
var r2 = await axios.get('http://test.com'); 
r1.data.value + r2.data.value
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test12_multipleRequests_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({value:1});
var r1 = await axios.get('http://test.com'); 
var r2 = await axios.get('http://test.com'); 
r1.data.value + r2.data.value
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(2L, result);
		}

		// axios.get - different URLs
		[TestMethod]
		public void Test13_differentUrls()
		{
			string s = @"
var axios = require('axios').createMock({url:'mocked'});
var resp = await axios.get('http://example.com/api/data'); 
resp.data.url
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("mocked", result);
		}

		[TestMethod]
		public void Test13_differentUrls_CompileAll()
		{
			string s = @"
var axios = require('axios').createMock({url:'mocked'});
var resp = await axios.get('http://example.com/api/data'); 
resp.data.url
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual("mocked", result);
		}

		// axios.all - multiple concurrent requests
		[TestMethod]
		public void Test14_axiosAll()
		{
			string s = @"
var axios = require('axios');
var req = axios.createMock({value:1});
var resps = await axios.all([req.get('http://test.com'), req.get('http://test.com')]); 
resps[0].data.value + resps[1].data.value
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("");
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test14_axiosAll_CompileAll()
		{
			string s = @"
var axios = require('axios');
var req = axios.createMock({value:1});
var resps = await axios.all([req.get('http://test.com'), req.get('http://test.com')]); 
resps[0].data.value + resps[1].data.value
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test15_axiosAllThreeRequests()
		{
			string s = @"
var axios = require('axios');
var req = axios.createMock({value:1});
var resps = await axios.all([req.get('http://test.com'), req.get('http://test.com'), req.get('http://test.com')]); 
resps.length
";
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(3L, result);
		}

		[TestMethod]
		public void Test15_axiosAllThreeRequests_CompileAll()
		{
			string s = @"
var axios = require('axios');
var req = axios.createMock({value:1});
var resps = await axios.all([req.get('http://test.com'), req.get('http://test.com'), req.get('http://test.com')]); 
resps.length
";
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval(s);
			Assert.AreEqual(3L, result);
		}
	}
}
