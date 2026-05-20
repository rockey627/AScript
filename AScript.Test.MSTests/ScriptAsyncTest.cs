using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class ScriptAsyncTest
	{
		#region Basic EvalAsync Tests

		[TestMethod]
		public async Task EvalAsync_BasicExpression()
		{
			var script = new Script();
			var result = await script.EvalAsync("4 6");
			Assert.AreEqual(6, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_WithVariable()
		{
			var script = new Script();
			script.Context.SetVar("n", 10);
			var result = await script.EvalAsync("n + 5");
			Assert.AreEqual(15, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_WithFunction()
		{
			var script = new Script();
			var result = await script.EvalAsync(@"
int sum(int a, int b) {
	return a + b;
}
sum(3, 5);
");
			Assert.AreEqual(8, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_TypedResult()
		{
			var script = new Script();
			var result = await script.EvalAsync<int>("4 6");
			Assert.AreEqual(6, result);
		}

		[TestMethod]
		public async Task EvalAsync_StringResult()
		{
			var script = new Script();
			var result = await script.EvalAsync<string>("'hello' + 'world'");
			Assert.AreEqual("helloworld", result);
		}

		[TestMethod]
		public async Task EvalAsync_WithContext()
		{
			var script = new Script();
			script.Context.SetVar("a", 5);
			script.Context.SetVar("b", 6);
			var result = await script.EvalAsync("a + b");
			Assert.AreEqual(11, result.Value);
		}

		#endregion

		#region EvalAsync with CancellationToken

		[TestMethod]
		public async Task EvalAsync_Cancellation_BeforeStart()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();
			cts.Cancel(); // Cancel before execution

			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await script.EvalAsync("4 6", cancellationToken: cts.Token);
			});
		}

		[TestMethod]
		public async Task EvalAsync_Cancellation_DuringExecution()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			// Create a script that waits long enough that we can cancel it
			var resultTask = script.EvalAsync("await Task.Delay(1000)", cancellationToken: cts.Token);

			// Cancel after a short delay
			await Task.Delay(100);
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await resultTask;
			});
		}

		[TestMethod]
		public async Task EvalAsync_Cancellation_NotCancelled()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			// Quick execution that shouldn't be cancelled
			var result = await script.EvalAsync("4 6", cancellationToken: cts.Token);
			Assert.AreEqual(6, result.Value);
		}

		//		[TestMethod]
		//		public async Task EvalAsync_Cancellation_WithLongRunningScript()
		//		{
		//			var script = new Script();
		//			var cts = new CancellationTokenSource();

		//			// Script with loop that can be cancelled
		//			var resultTask = script.EvalAsync(@"
		//int sum = 0;
		//for (int i = 0; i < 1000000000; i++) {
		//	sum += i;
		//}
		//sum
		//", cancellationToken: cts.Token);

		//			// Wait a bit then cancel
		//			await Task.Delay(50);
		//			cts.Cancel();

		//			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
		//			{
		//				await resultTask;
		//			});
		//		}

		[TestMethod]
		public async Task EvalAsync_Cancellation_FastExecution()
		{
			// Even with a very short timeout, fast scripts should complete
			var script = new Script();
			var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));

			var result = await script.EvalAsync(@"
int sum(int a, int b) {
	return a + b;
}
sum(10, 20);
", cancellationToken: cts.Token);

			Assert.AreEqual(30, result.Value);
		}

		#endregion

		#region EvalAsync with Stream

		[TestMethod]
		public async Task EvalAsync_Stream_Basic()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("4 6"));

			var result = await script.EvalAsync(stream);
			Assert.AreEqual(6, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_Stream_WithCancellation()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			var stream = new MemoryStream(Encoding.UTF8.GetBytes("await Task.Delay(1000)"));

			var resultTask = script.EvalAsync(stream, cancellationToken: cts.Token);

			await Task.Delay(100);
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await resultTask;
			});
		}

		[TestMethod]
		public async Task EvalAsync_FuncStream_Basic()
		{
			var script = new Script();
			var result = await script.EvalAsync(() => new MemoryStream(Encoding.UTF8.GetBytes("4 6")));
			Assert.AreEqual(6, result.Value);
		}

		#endregion

		#region EvalAsync with CompileMode

		[TestMethod]
		public async Task EvalAsync_CompileMode_Expression()
		{
			var script = new Script();
			var result = await script.EvalAsync("int n = 10; n + 5", ECompileMode.All);
			Assert.AreEqual(15, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_CompileMode_Typed()
		{
			var script = new Script();
			var result = await script.EvalAsync<string>("'hello'", ECompileMode.All);
			Assert.AreEqual("hello", result);
		}

		[TestMethod]
		public async Task EvalAsync_CompileMode_WithCancellation()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			var resultTask = script.EvalAsync("await Task.Delay(1000)", ECompileMode.All, cancellationToken: cts.Token);

			await Task.Delay(100);
			cts.Cancel();

			await resultTask;
		}

		#endregion

		#region Multiple Concurrent EvalAsync

		[TestMethod]
		public async Task EvalAsync_ConcurrentMultiple()
		{
			var script = new Script();
			script.Context.SetVar("base", 10);

			var tasks = new List<Task<EvalResult>>
			{
				script.EvalAsync("base + 1"),
				script.EvalAsync("base + 2"),
				script.EvalAsync("base + 3"),
				script.EvalAsync("base + 4"),
			};

			var results = await Task.WhenAll(tasks);

			Assert.AreEqual(11, results[0].Value);
			Assert.AreEqual(12, results[1].Value);
			Assert.AreEqual(13, results[2].Value);
			Assert.AreEqual(14, results[3].Value);
		}

		[TestMethod]
		public async Task EvalAsync_Concurrent_Cancellation()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			// Start multiple long-running tasks
			var tasks = new List<Task>
			{
				script.EvalAsync("await Task.Delay(1000)", cancellationToken: cts.Token),
				script.EvalAsync("await Task.Delay(1000)", cancellationToken: cts.Token),
				script.EvalAsync("await Task.Delay(1000)", cancellationToken: cts.Token),
			};

			await Task.Delay(100);
			cts.Cancel();

			var exception = await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await Task.WhenAll(tasks);
			});
		}

		#endregion

		#region CompileGlobalAsync Tests

		[TestMethod]
		public async Task CompileGlobalAsync_Basic()
		{
			var script = new Script();
			var del = await script.CompileGlobalAsync("int n = 10; n + 5");

			var func = (Func<ScriptContext, int>)del;
			var result = func(script.Context);
			Assert.AreEqual(15, result);
		}

		[TestMethod]
		public async Task CompileGlobalAsync_Typed()
		{
			var script = new Script();
			var func = await script.CompileGlobalAsync<int>("5 + 10");
			var result = func(script.Context);
			Assert.AreEqual(15, result);
		}

		#endregion

		#region BuildNodeAsync Tests

		[TestMethod]
		public async Task BuildNodeAsync_Basic()
		{
			var script = new Script();
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("4 6"));

			var node = await script.BuildNodeAsync(stream);
			Assert.IsNotNull(node);
		}

		#endregion

		#region Script Instance Reuse Tests

		[TestMethod]
		public async Task EvalAsync_ReuseScriptInstance()
		{
			var script = new Script();

			var result1 = await script.EvalAsync("result = 5 + 3");
			Assert.AreEqual(8, result1.Value);

			var result2 = await script.EvalAsync("result + 10");
			Assert.AreEqual(18, result2.Value);
		}

		[TestMethod]
		public async Task EvalAsync_IndependentContexts()
		{
			var script1 = new Script();
			var script2 = new Script();

			var result1 = await script1.EvalAsync("5 + 3");
			var result2 = await script2.EvalAsync("10 + 20");

			Assert.AreEqual(8, result1.Value);
			Assert.AreEqual(30, result2.Value);
		}

		#endregion

		#region Error Handling Tests

		[TestMethod]
		public async Task EvalAsync_SyntaxError()
		{
			var script = new Script();
			await Assert.ThrowsExceptionAsync<Exceptions.ScriptAnalyzingException>(async () =>
			{
				await script.EvalAsync("int n = ");
			});
		}

		#endregion

		#region Cache Tests

		[TestMethod]
		public async Task EvalAsync_WithCache()
		{
			var script = new Script();

			// First evaluation - should compute
			var result1 = await script.EvalAsync("5 + 3", cacheTime: 1000, cacheKey: "test1");
			Assert.AreEqual(8, result1.Value);

			// Second evaluation with same key - should use cache
			var result2 = await script.EvalAsync("5 + 3", cacheTime: 1000, cacheKey: "test1");
			Assert.AreEqual(8, result2.Value);
		}

		[TestMethod]
		public async Task EvalAsync_CacheWithCancellation()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			// First call - cached
			var result1 = await script.EvalAsync("5 + 3", cacheTime: 5000, cacheKey: "cancelTest");
			Assert.AreEqual(8, result1.Value);

			// Second call with same key - should be instant and not respect cancellation
			var result2 = await script.EvalAsync("5 + 3", cacheTime: 5000, cacheKey: "cancelTest", cancellationToken: cts.Token);
			Assert.AreEqual(8, result2.Value);
		}

		#endregion

		#region Complex Scripts Tests

		[TestMethod]
		public async Task EvalAsync_ComplexScript_Fibonacci()
		{
			var script = new Script();
			var result = await script.EvalAsync(@"
int fib(int n) {
    if (n <= 1) return n;
    return fib(n - 1) + fib(n - 2);
}
fib(10);
");
			Assert.AreEqual(55, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_RecursiveFunction()
		{
			var script = new Script();
			var result = await script.EvalAsync(@"
int sum(int a) {
	if (a < 1) return 0;
	return a + sum(a - 1);
}
sum(5);
");
			Assert.AreEqual(15, result.Value);
		}

//		[TestMethod]
//		public async Task EvalAsync_AwaitTask()
//		{
//			var script = new Script();
//			var result = await script.EvalAsync(@"
//var t = Task.FromResult(42);
//await t
//");
//			Assert.AreEqual(42, result.Value);
//		}

		[TestMethod]
		public async Task EvalAsync_AwaitTaskDelay()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			var resultTask = script.EvalAsync(@"
await Task.Delay(1000);
42
", cancellationToken: cts.Token);
			Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
			var result = await resultTask;
			Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
			Assert.AreEqual(42, result.Value);
		}

		[TestMethod]
		public async Task EvalAsync_AwaitTaskDelay_Cancellation_2()
		{
			var cts = new CancellationTokenSource();

			var script = new Script();
			script.Context.SetVar("token", cts.Token);
			var resultTask = script.EvalAsync(@"
await Task.Delay(1000, token);
42
", cancellationToken: cts.Token);

			await Task.Delay(100);
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
			{
				await resultTask;
			});
		}

		[TestMethod]
		public async Task EvalAsync_AwaitTaskDelay_Cancellation()
		{
			var cts = new CancellationTokenSource();

			var script = new Script();
			var resultTask = script.EvalAsync(@"
await Task.Delay(1000);
42
", cancellationToken: cts.Token);

			await Task.Delay(100);
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await resultTask;
			});
		}

		#endregion

		#region Typed EvalAsync<T> Tests

		[TestMethod]
		public async Task EvalAsyncT_Int()
		{
			var script = new Script();
			var result = await script.EvalAsync<int>("42");
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public async Task EvalAsyncT_Bool()
		{
			var script = new Script();
			var result = await script.EvalAsync<bool>("true");
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public async Task EvalAsyncT_Double()
		{
			var script = new Script();
			var result = await script.EvalAsync<double>("3.14");
			Assert.AreEqual(3.14, result);
		}

		[TestMethod]
		public async Task EvalAsyncT_ComplexExpression()
		{
			var script = new Script();
			var result = await script.EvalAsync<int>(@"
int sum(int a, int b) => a + b;
sum(10, 20);
");
			Assert.AreEqual(30, result);
		}

		[TestMethod]
		public async Task EvalAsyncT_WithCancellation()
		{
			var script = new Script();
			var cts = new CancellationTokenSource();

			var resultTask = script.EvalAsync<int>("await Task.Delay(1000)", cancellationToken: cts.Token);

			await Task.Delay(100);
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
			{
				await resultTask;
			});
		}

		#endregion
	}
}