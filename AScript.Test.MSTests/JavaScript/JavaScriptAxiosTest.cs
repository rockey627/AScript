using AScript.Lang.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		public static void SetMockHandler(HttpMessageHandler handler)
		{
			JavaScriptLang.Instance.SetVar("axios", new HttpClient(handler));
		}

		// axios.get - simple object response
		[TestMethod]
		public void Test01_getObject()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.a");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.a");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject2()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com').catch(err=>{console.log(err)}); resp.data.a");
			Assert.AreEqual(1L, result);
		}

		[TestMethod]
		public void Test01_getObject2_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com').catch(err=>{console.log(err)}); resp.data.a");
			Assert.AreEqual(1L, result);
		}

		// axios.get - array response
		[TestMethod]
		public void Test02_getArray()
		{
			var handler = new MockHttpMessageHandler("[1, 2, 3]", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data[0] + resp.data[1] + resp.data[2]");
			Assert.AreEqual(6L, result);
		}

		[TestMethod]
		public void Test02_getArray_CompileAll()
		{
			var handler = new MockHttpMessageHandler("[1, 2, 3]", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data[0] + resp.data[1] + resp.data[2]");
			Assert.AreEqual(6L, result);
		}

		// axios.get - nested object response
		[TestMethod]
		public void Test03_getNested()
		{
			var handler = new MockHttpMessageHandler("{\"arr\":[1,2],\"obj\":{\"x\":1}}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.arr[0] + resp.data.arr[1] + resp.data.obj.x");
			Assert.AreEqual(4L, result);
		}

		[TestMethod]
		public void Test03_getNested_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"arr\":[1,2],\"obj\":{\"x\":1}}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.arr[0] + resp.data.arr[1] + resp.data.obj.x");
			Assert.AreEqual(4L, result);
		}

		// axios.get - string value
		[TestMethod]
		public void Test04_getString()
		{
			var handler = new MockHttpMessageHandler("{\"name\":\"test\",\"value\":\"hello\"}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data.name + resp.data.value");
			Assert.AreEqual("testhello", result);
		}

		[TestMethod]
		public void Test04_getString_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"name\":\"test\",\"value\":\"hello\"}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data.name + resp.data.value");
			Assert.AreEqual("testhello", result);
		}

		// axios.get - boolean value
		[TestMethod]
		public void Test05_getBoolean()
		{
			var handler = new MockHttpMessageHandler("{\"enabled\":true,\"disabled\":false}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data.enabled && !resp.data.disabled");
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void Test05_getBoolean_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"enabled\":true,\"disabled\":false}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data.enabled && !resp.data.disabled");
			Assert.AreEqual(true, result);
		}

		// axios.get - null value
		[TestMethod]
		public void Test06_getNull()
		{
			var handler = new MockHttpMessageHandler("{\"value\":null}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.value");
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Test06_getNull_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"value\":null}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.value");
			Assert.IsNull(result);
		}

		// axios.get - data is parsed only once (caching)
		[TestMethod]
		public void Test07_dataParsedOnce()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); var d1 = resp.data; var d2 = resp.data; d1.a + d2.a");
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test07_dataParsedOnce_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"a\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); var d1 = resp.data; var d2 = resp.data; d1.a + d2.a");
			Assert.AreEqual(2L, result);
		}

		// axios.get - double value
		[TestMethod]
		public void Test08_getDouble()
		{
			var handler = new MockHttpMessageHandler("{\"pi\":3.14,\"price\":19.99}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.pi + resp.data.price");
			Assert.AreEqual(23.13, (double)result, 0.001);
		}

		[TestMethod]
		public void Test08_getDouble_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"pi\":3.14,\"price\":19.99}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.pi + resp.data.price");
			Assert.AreEqual(23.13, (double)result, 0.001);
		}

		// axios.get - empty object
		[TestMethod]
		public void Test09_getEmptyObject()
		{
			var handler = new MockHttpMessageHandler("{}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data != null");
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void Test09_getEmptyObject_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://test.com'); resp.data != null");
			Assert.AreEqual(true, result);
		}

		// axios.get - empty array
		[TestMethod]
		public void Test10_getEmptyArray()
		{
			var handler = new MockHttpMessageHandler("[]", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.length");
			Assert.AreEqual(0L, result);
		}

		[TestMethod]
		public void Test10_getEmptyArray_CompileAll()
		{
			var handler = new MockHttpMessageHandler("[]", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval<dynamic>("var resp = await axios.get('http://test.com'); resp.data.length");
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
			var handler = new MockHttpMessageHandler("{\"value\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var r1 = await axios.get('http://test.com'); var r2 = await axios.get('http://test.com'); r1.data.value + r2.data.value");
			Assert.AreEqual(2L, result);
		}

		[TestMethod]
		public void Test12_multipleRequests_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"value\":1}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var r1 = await axios.get('http://test.com'); var r2 = await axios.get('http://test.com'); r1.data.value + r2.data.value");
			Assert.AreEqual(2L, result);
		}

		// axios.get - different URLs
		[TestMethod]
		public void Test13_differentUrls()
		{
			var handler = new MockHttpMessageHandler("{\"url\":\"mocked\"}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://example.com/api/data'); resp.data.url");
			Assert.AreEqual("mocked", result);
		}

		[TestMethod]
		public void Test13_differentUrls_CompileAll()
		{
			var handler = new MockHttpMessageHandler("{\"url\":\"mocked\"}", System.Net.HttpStatusCode.OK);
			SetMockHandler(handler);

			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			var result = script.Eval("var resp = await axios.get('http://example.com/api/data'); resp.data.url");
			Assert.AreEqual("mocked", result);
		}

	}

	// Mock HttpMessageHandler for testing
	internal class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly string _responseBody;
		private readonly System.Net.HttpStatusCode _statusCode;

		public MockHttpMessageHandler(string responseBody, System.Net.HttpStatusCode statusCode)
		{
			_responseBody = responseBody;
			_statusCode = statusCode;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage(_statusCode)
			{
				Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
			};
			return Task.FromResult(response);
		}
	}
}
