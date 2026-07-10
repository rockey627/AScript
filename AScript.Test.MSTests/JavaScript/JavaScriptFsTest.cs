using AScript.Lang.JavaScript;
using AScript.Lang.JavaScript.fs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AScript.Test.MSTests.JavaScript
{
	[TestClass]
	public class JavaScriptFsTest
	{
		private static string _testDir;
		private static string _testFile;
		private static string _testFile2;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			Script.Langs["js"] = JavaScriptLang.Instance;
			JavaScriptLang.Instance.AddModule("fs", new JavaScriptFileSystemModule());

			_testDir = Path.Combine(Path.GetTempPath(), "AScript_FsTest");
			Directory.CreateDirectory(_testDir);
			_testFile = Path.Combine(_testDir, "test.txt");
			_testFile2 = Path.Combine(_testDir, "test2.txt");
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Script.Langs.TryRemove("js");
			if (Directory.Exists(_testDir))
			{
				try { Directory.Delete(_testDir, true); } catch { }
			}
		}

		private Script CreateScript()
		{
			var script = new Script();
			script.Context.Langs = new[] { "js" };
			script.Context.SetVar("testFile", _testFile);
			script.Context.SetVar("testFile2", _testFile2);
			return script;
		}

		private Script CreateScriptCompileAll()
		{
			var script = new Script();
			script.Options.CompileMode = ECompileMode.All;
			script.Context.Langs = new[] { "js" };
			script.Context.SetVar("testFile", _testFile);
			script.Context.SetVar("testFile2", _testFile2);
			return script;
		}

		// ==================== readFileSync ====================

		[TestMethod]
		public void Test01_readFileSync_String()
		{
			File.WriteAllText(_testFile, "hello world", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("hello world", result);
		}

		[TestMethod]
		public void Test01_readFileSync_String_CompileAll()
		{
			File.WriteAllText(_testFile, "hello world", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("hello world", result);
		}

		// ==================== writeFileSync ====================

		[TestMethod]
		public void Test02_writeFileSync()
		{
			string s = @"
var fs = require('fs');
fs.writeFileSync(testFile, 'test content', 'utf-8');
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("test content", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test02_writeFileSync_CompileAll()
		{
			string s = @"
var fs = require('fs');
fs.writeFileSync(testFile, 'test content', 'utf-8');
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("test content", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== readFile async/await ====================

		[TestMethod]
		public async Task Test03_readFile_Async()
		{
			File.WriteAllText(_testFile, "async content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.readFile(testFile, 'utf-8')
";
			var script = CreateScript();
			var result = await script.EvalAsync<string>(s);
			Assert.AreEqual("async content", result);
		}

		[TestMethod]
		public async Task Test03_readFile_Async_CompileAll()
		{
			File.WriteAllText(_testFile, "async content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.readFile(testFile, 'utf-8')
";
			var script = CreateScriptCompileAll();
			var result = await script.EvalAsync<string>(s);
			Assert.AreEqual("async content", result);
		}

		// ==================== writeFile async/await ====================

		[TestMethod]
		public async Task Test04_writeFile_Async()
		{
			string s = @"
var fs = require('fs');
await fs.writeFile(testFile, 'async write', 'utf-8');
true
";
			var script = CreateScript();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("async write", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public async Task Test04_writeFile_Async_CompileAll()
		{
			string s = @"
var fs = require('fs');
await fs.writeFile(testFile, 'async write', 'utf-8');
true
";
			var script = CreateScriptCompileAll();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("async write", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== appendFileSync ====================

		[TestMethod]
		public void Test05_appendFileSync()
		{
			File.WriteAllText(_testFile, "first", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.appendFileSync(testFile, '-second', 'utf-8');
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("first-second", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test05_appendFileSync_CompileAll()
		{
			File.WriteAllText(_testFile, "first", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.appendFileSync(testFile, '-second', 'utf-8');
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("first-second", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== appendFile async ====================

		[TestMethod]
		public async Task Test06_appendFile_Async()
		{
			File.WriteAllText(_testFile, "start", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.appendFile(testFile, '-appended', 'utf-8');
true
";
			var script = CreateScript();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("start-appended", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public async Task Test06_appendFile_Async_CompileAll()
		{
			File.WriteAllText(_testFile, "start", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.appendFile(testFile, '-appended', 'utf-8');
true
";
			var script = CreateScriptCompileAll();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("start-appended", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== copyFileSync ====================

		[TestMethod]
		public void Test07_copyFileSync()
		{
			File.WriteAllText(_testFile, "copy source", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.copyFileSync(testFile, testFile2);
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("copy source", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test07_copyFileSync_CompileAll()
		{
			File.WriteAllText(_testFile, "copy source", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.copyFileSync(testFile, testFile2);
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("copy source", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		// ==================== copyFile async ====================

		[TestMethod]
		public async Task Test08_copyFile_Async()
		{
			File.WriteAllText(_testFile, "async copy", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.copyFile(testFile, testFile2);
true
";
			var script = CreateScript();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("async copy", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public async Task Test08_copyFile_Async_CompileAll()
		{
			File.WriteAllText(_testFile, "async copy", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.copyFile(testFile, testFile2);
true
";
			var script = CreateScriptCompileAll();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("async copy", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		// ==================== unlinkSync ====================

		[TestMethod]
		public void Test09_unlinkSync()
		{
			File.WriteAllText(_testFile, "to delete", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.unlinkSync(testFile);
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		[TestMethod]
		public void Test09_unlinkSync_CompileAll()
		{
			File.WriteAllText(_testFile, "to delete", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.unlinkSync(testFile);
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		// ==================== unlink async ====================

		[TestMethod]
		public async Task Test10_unlink_Async()
		{
			File.WriteAllText(_testFile, "to delete async", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.unlink(testFile);
true
";
			var script = CreateScript();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		[TestMethod]
		public async Task Test10_unlink_Async_CompileAll()
		{
			File.WriteAllText(_testFile, "to delete async", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
await fs.unlink(testFile);
true
";
			var script = CreateScriptCompileAll();
			var result = await script.EvalAsync<bool>(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		// ==================== createReadStream + createWriteStream + pipe ====================

		[TestMethod]
		public void Test11_Pipe_Streams()
		{
			File.WriteAllText(_testFile, "pipe content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var reader = fs.createReadStream(testFile);
var writer = fs.createWriteStream(testFile2);
reader.pipe(writer);
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("pipe content", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test11_Pipe_Streams_CompileAll()
		{
			File.WriteAllText(_testFile, "pipe content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var reader = fs.createReadStream(testFile);
var writer = fs.createWriteStream(testFile2);
reader.pipe(writer);
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("pipe content", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		// ==================== Stream Events (on/bind) ====================

		[TestMethod]
		public void Test12_Stream_Events()
		{
			File.WriteAllText(_testFile, "event content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var result = '';
var reader = fs.createReadStream(testFile);
reader.on('data', function(chunk) {
    result = result + chunk;
});
reader.on('end', function() {
    // nothing
});
result
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("event content", result);
		}

		[TestMethod]
		public void Test12_Stream_Events_CompileAll()
		{
			File.WriteAllText(_testFile, "event content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var result = '';
var reader = fs.createReadStream(testFile);
reader.on('data', function(chunk) {
    result = result + chunk;
});
reader.on('end', function() {
    // nothing
});
result
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("event content", result);
		}

		// ==================== WriteStream write and end ====================

		[TestMethod]
		public void Test13_WriteStream_Write_End()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write('hello');
writer.write(' ');
writer.write('world');
writer.end();
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("hello world", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test13_WriteStream_Write_End_CompileAll()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write('hello');
writer.write(' ');
writer.write('world');
writer.end();
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("hello world", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== createReadStream with options ====================

		[TestMethod]
		public void Test14_CreateReadStream_WithOptions()
		{
			File.WriteAllText(_testFile, "options test", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var reader = fs.createReadStream(testFile, {encoding:'utf-8'});
var result = '';
reader.on('data', function(chunk) {
    result = result + chunk;
});
result
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("options test", result);
		}

		[TestMethod]
		public void Test14_CreateReadStream_WithOptions_CompileAll()
		{
			File.WriteAllText(_testFile, "options test", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var reader = fs.createReadStream(testFile, {encoding:'utf-8'});
var result = '';
reader.on('data', function(chunk) {
    result = result + chunk;
});
result
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("options test", result);
		}

		// ==================== createWriteStream with options ====================

		[TestMethod]
		public void Test15_CreateWriteStream_WithOptions()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile, {encoding:'utf-8'});
writer.write('stream options');
writer.end();
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("stream options", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test15_CreateWriteStream_WithOptions_CompileAll()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile, {encoding:'utf-8'});
writer.write('stream options');
writer.end();
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("stream options", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== readFile with callback ====================

		[TestMethod]
		public void Test16_readFile_Callback()
		{
			File.WriteAllText(_testFile, "callback content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err, data;
fs.readFile(testFile, 'utf-8', function(e, d) {
    err = e;
    data = d;
});
await delay(50);
data
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("callback content", result);
		}

		[TestMethod]
		public void Test16_readFile_Callback_CompileAll()
		{
			File.WriteAllText(_testFile, "callback content", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err, data;
fs.readFile(testFile, 'utf-8', function(e, d) {
    err = e;
    data = d;
});
await delay(50);
data
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("callback content", result);
		}

		// ==================== writeFile with callback ====================

		[TestMethod]
		public void Test17_writeFile_Callback()
		{
			string s = @"
var fs = require('fs');
var err;
fs.writeFile(testFile, 'callback write', 'utf-8', function(e) {
    err = e;
});
err == null
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("callback write", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test17_writeFile_Callback_CompileAll()
		{
			string s = @"
var fs = require('fs');
var err;
fs.writeFile(testFile, 'callback write', 'utf-8', function(e) {
    err = e;
});
err == null
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("callback write", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== copyFile with callback ====================

		[TestMethod]
		public void Test18_copyFile_Callback()
		{
			File.WriteAllText(_testFile, "copy callback", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err;
fs.copyFile(testFile, testFile2, function(e) {
    err = e;
});
sleep(50);
err == null
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("copy callback", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test18_copyFile_Callback_CompileAll()
		{
			File.WriteAllText(_testFile, "copy callback", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err;
fs.copyFile(testFile, testFile2, function(e) {
    err = e;
});
sleep(50);
err == null
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("copy callback", File.ReadAllText(_testFile2, System.Text.Encoding.UTF8));
		}

		// ==================== unlink with callback ====================

		[TestMethod]
		public void Test19_unlink_Callback()
		{
			File.WriteAllText(_testFile, "unlink callback", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err;
fs.unlink(testFile, function(e) {
    err = e;
});
await delay(50);
err == null
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		[TestMethod]
		public void Test19_unlink_Callback_CompileAll()
		{
			File.WriteAllText(_testFile, "unlink callback", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
var err;
fs.unlink(testFile, function(e) {
    err = e;
});
await delay(50);
err == null
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.IsFalse(File.Exists(_testFile));
		}

		// ==================== Chinese content ====================

		[TestMethod]
		public void Test20_Chinese_Content()
		{
			File.WriteAllText(_testFile, "中文内容测试", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("中文内容测试", result);
		}

		[TestMethod]
		public void Test20_Chinese_Content_CompileAll()
		{
			File.WriteAllText(_testFile, "中文内容测试", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("中文内容测试", result);
		}

		// ==================== Empty file ====================

		[TestMethod]
		public void Test21_Empty_File()
		{
			File.WriteAllText(_testFile, "", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("", result);
		}

		[TestMethod]
		public void Test21_Empty_File_CompileAll()
		{
			File.WriteAllText(_testFile, "", System.Text.Encoding.UTF8);
			string s = @"
var fs = require('fs');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("", result);
		}

		// ==================== Multiple operations in sequence ====================

		[TestMethod]
		public void Test22_Multiple_Operations()
		{
			string s = @"
var fs = require('fs');
fs.writeFileSync(testFile, 'a', 'utf-8');
fs.appendFileSync(testFile, 'b', 'utf-8');
fs.appendFileSync(testFile, 'c', 'utf-8');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual("abc", result);
		}

		[TestMethod]
		public void Test22_Multiple_Operations_CompileAll()
		{
			string s = @"
var fs = require('fs');
fs.writeFileSync(testFile, 'a', 'utf-8');
fs.appendFileSync(testFile, 'b', 'utf-8');
fs.appendFileSync(testFile, 'c', 'utf-8');
fs.readFileSync(testFile, 'utf-8')
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual("abc", result);
		}

		// ==================== Write different types ====================

		[TestMethod]
		public void Test23_Write_Number()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write(12345);
writer.write(67.89);
writer.end();
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("1234567.89", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test23_Write_Number_CompileAll()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write(12345);
writer.write(67.89);
writer.end();
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("1234567.89", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== Write boolean ====================

		[TestMethod]
		public void Test24_Write_Boolean()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write(true);
writer.write(false);
writer.end();
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("TrueFalse", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test24_Write_Boolean_CompileAll()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.write(true);
writer.write(false);
writer.end();
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("TrueFalse", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		// ==================== WriteStream end with value ====================

		[TestMethod]
		public void Test25_WriteStream_End_WithValue()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.end('final content');
true
";
			var script = CreateScript();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("final content", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}

		[TestMethod]
		public void Test25_WriteStream_End_WithValue_CompileAll()
		{
			string s = @"
var fs = require('fs');
var writer = fs.createWriteStream(testFile);
writer.end('final content');
true
";
			var script = CreateScriptCompileAll();
			var result = script.Eval(s);
			Assert.AreEqual(true, result);
			Assert.AreEqual("final content", File.ReadAllText(_testFile, System.Text.Encoding.UTF8));
		}
	}
}
