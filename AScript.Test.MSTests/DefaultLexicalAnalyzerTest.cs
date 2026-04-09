using AScript.Readers;
using System;

namespace AScript.Test.MSTests
{
	[TestClass]
	public class DefaultLexicalAnalyzerTest
	{
		[TestMethod]
		public void Test17()
		{
			string s = @"string s;
s??'hello'";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(6, tokens.Count);

			Assert.AreEqual("string", tokens[0].Value);
			Assert.AreEqual(ETokenType.Word, tokens[0].Type);
			Assert.AreEqual(1, tokens[0].Line);
			Assert.AreEqual(1, tokens[0].Column);

			Assert.AreEqual("s", tokens[1].Value);
			Assert.AreEqual(ETokenType.Word, tokens[1].Type);
			Assert.AreEqual(1, tokens[1].Line);
			Assert.AreEqual(8, tokens[1].Column);

			Assert.AreEqual(";", tokens[2].Value);
			Assert.AreEqual(ETokenType.None, tokens[2].Type);
			Assert.AreEqual(1, tokens[2].Line);
			Assert.AreEqual(9, tokens[2].Column);

			Assert.AreEqual("s", tokens[3].Value);
			Assert.AreEqual(ETokenType.Word, tokens[3].Type);
			Assert.AreEqual(2, tokens[3].Line);
			Assert.AreEqual(1, tokens[3].Column);

			Assert.AreEqual("??", tokens[4].Value);
			Assert.AreEqual(ETokenType.Operator, tokens[4].Type);
			Assert.AreEqual(2, tokens[4].Line);
			Assert.AreEqual(2, tokens[4].Column);

			Assert.AreEqual("hello", tokens[5].Value);
			Assert.AreEqual(ETokenType.String, tokens[5].Type);
			Assert.AreEqual(2, tokens[5].Line);
			Assert.AreEqual(4, tokens[5].Column);
		}

		[TestMethod]
		public void Test16()
		{
			var tokens = new DefaultTokenStream("string s;s??'hello'").ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(6, tokens.Count);

			Assert.AreEqual("string", tokens[0].Value);
			Assert.AreEqual(ETokenType.Word, tokens[0].Type);
			Assert.AreEqual(1, tokens[0].Line);
			Assert.AreEqual(1, tokens[0].Column);

			Assert.AreEqual("s", tokens[1].Value);
			Assert.AreEqual(ETokenType.Word, tokens[1].Type);
			Assert.AreEqual(1, tokens[1].Line);
			Assert.AreEqual(8, tokens[1].Column);

			Assert.AreEqual(";", tokens[2].Value);
			Assert.AreEqual(ETokenType.None, tokens[2].Type);
			Assert.AreEqual(1, tokens[2].Line);
			Assert.AreEqual(9, tokens[2].Column);

			Assert.AreEqual("s", tokens[3].Value);
			Assert.AreEqual(ETokenType.Word, tokens[3].Type);
			Assert.AreEqual(1, tokens[3].Line);
			Assert.AreEqual(10, tokens[3].Column);

			Assert.AreEqual("??", tokens[4].Value);
			Assert.AreEqual(ETokenType.Operator, tokens[4].Type);
			Assert.AreEqual(1, tokens[4].Line);
			Assert.AreEqual(11, tokens[4].Column);

			Assert.AreEqual("hello", tokens[5].Value);
			Assert.AreEqual(ETokenType.String, tokens[5].Type);
			Assert.AreEqual(1, tokens[5].Line);
			Assert.AreEqual(13, tokens[5].Column);
		}

		[TestMethod]
		public void Test15()
		{
			var tokens = new DefaultTokenStream("n=1>0; n==true").ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(9, tokens.Count);
			Assert.AreEqual("n", tokens[0].Value);
			Assert.AreEqual(ETokenType.Word, tokens[0].Type);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual(ETokenType.Operator, tokens[1].Type);
			Assert.AreEqual("1", tokens[2].Value);
			Assert.AreEqual(ETokenType.Number, tokens[2].Type);
			Assert.AreEqual(">", tokens[3].Value);
			Assert.AreEqual(ETokenType.Operator, tokens[3].Type);
			Assert.AreEqual("0", tokens[4].Value);
			Assert.AreEqual(ETokenType.Number, tokens[4].Type);
			Assert.AreEqual(";", tokens[5].Value);
			Assert.AreEqual(ETokenType.None, tokens[5].Type);
			Assert.AreEqual("n", tokens[6].Value);
			Assert.AreEqual(ETokenType.Word, tokens[6].Type);
			Assert.AreEqual("==", tokens[7].Value);
			Assert.AreEqual(ETokenType.Operator, tokens[7].Type);
			Assert.AreEqual("true", tokens[8].Value);
			Assert.AreEqual(ETokenType.Word, tokens[8].Type);
		}

		[TestMethod]
		public void Test14()
		{
			var tokens = new DefaultTokenStream("5&12").ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("5", tokens[0].Value);
			Assert.AreEqual("&", tokens[1].Value);
			Assert.AreEqual("12", tokens[2].Value);
		}

		[TestMethod]
		public void Test13()
		{
			var tokens = new DefaultTokenStream("5/12").ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("5", tokens[0].Value);
			Assert.AreEqual("/", tokens[1].Value);
			Assert.AreEqual("12", tokens[2].Value);
		}

		[TestMethod]
		public void Test12()
		{
			var tokens = new DefaultTokenStream(File.OpenRead("DefaultLexicalAnalyzerTest_Test12.txt"), true).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("5", tokens[0].Value);
			Assert.AreEqual("+", tokens[1].Value);
			Assert.AreEqual("2", tokens[2].Value);
		}

		[TestMethod]
		public void Test11()
		{
			string s = "+.9";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(2, tokens.Count);
			Assert.AreEqual("+", tokens[0].Value);
			Assert.AreEqual(".9", tokens[1].Value);
		}

		[TestMethod]
		public void Test10()
		{
			string s = "'hello'.Length";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("hello", tokens[0].Value);
			Assert.AreEqual(".", tokens[1].Value);
			Assert.AreEqual("Length", tokens[2].Value);
		}

		[TestMethod]
		public void Test09()
		{
			string s = ".9";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual(".9", tokens[0].Value);
		}

		[TestMethod]
		public void Test08()
		{
			string s = "6.0";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual("6.0", tokens[0].Value);
		}

		[TestMethod]
		public void Test07()
		{
			string s = "DateTime.Now.Year";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(5, tokens.Count);
			Assert.AreEqual("DateTime", tokens[0].Value);
			Assert.AreEqual(".", tokens[1].Value);
			Assert.AreEqual("Now", tokens[2].Value);
			Assert.AreEqual(".", tokens[3].Value);
			Assert.AreEqual("Year", tokens[4].Value);
		}

		[TestMethod]
		public void Test06()
		{
			string s = "height =  0x0A0F";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("height", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("0x0A0F", tokens[2].Value);
		}

		[TestMethod]
		public void Test05()
		{
			string s = "m =6.03f.round(  0.6 )";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(8, tokens.Count);
			Assert.AreEqual("m", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("6.03f", tokens[2].Value);
			Assert.AreEqual(".", tokens[3].Value);
			Assert.AreEqual("round", tokens[4].Value);
			Assert.AreEqual("(", tokens[5].Value);
			Assert.AreEqual("0.6", tokens[6].Value);
			Assert.AreEqual(")", tokens[7].Value);
		}

		[TestMethod]
		public void Test04()
		{
			string s = "m =6.03.round(  0.6 )";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(8, tokens.Count);
			Assert.AreEqual("m", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("6.03", tokens[2].Value);
			Assert.AreEqual(".", tokens[3].Value);
			Assert.AreEqual("round", tokens[4].Value);
			Assert.AreEqual("(", tokens[5].Value);
			Assert.AreEqual("0.6", tokens[6].Value);
			Assert.AreEqual(")", tokens[7].Value);
		}

		[TestMethod]
		public void Test03()
		{
			string s = "m =6.03f;  n2=27; m+ n2 * (10-m)";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(17, tokens.Count);
			Assert.AreEqual("m", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("6.03f", tokens[2].Value);
			Assert.AreEqual(";", tokens[3].Value);
			Assert.AreEqual("n2", tokens[4].Value);
			Assert.AreEqual("=", tokens[5].Value);
			Assert.AreEqual("27", tokens[6].Value);
			Assert.AreEqual(";", tokens[7].Value);
			Assert.AreEqual("m", tokens[8].Value);
			Assert.AreEqual("+", tokens[9].Value);
			Assert.AreEqual("n2", tokens[10].Value);
			Assert.AreEqual("*", tokens[11].Value);
			Assert.AreEqual("(", tokens[12].Value);
			Assert.AreEqual("10", tokens[13].Value);
			Assert.AreEqual("-", tokens[14].Value);
			Assert.AreEqual("m", tokens[15].Value);
			Assert.AreEqual(")", tokens[16].Value);
		}

		[TestMethod]
		public void Test02()
		{
			string s = "m =6;  n2=27; m+ n2 * (10-m)";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(17, tokens.Count);
			Assert.AreEqual("m", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("6", tokens[2].Value);
			Assert.AreEqual(";", tokens[3].Value);
			Assert.AreEqual("n2", tokens[4].Value);
			Assert.AreEqual("=", tokens[5].Value);
			Assert.AreEqual("27", tokens[6].Value);
			Assert.AreEqual(";", tokens[7].Value);
			Assert.AreEqual("m", tokens[8].Value);
			Assert.AreEqual("+", tokens[9].Value);
			Assert.AreEqual("n2", tokens[10].Value);
			Assert.AreEqual("*", tokens[11].Value);
			Assert.AreEqual("(", tokens[12].Value);
			Assert.AreEqual("10", tokens[13].Value);
			Assert.AreEqual("-", tokens[14].Value);
			Assert.AreEqual("m", tokens[15].Value);
			Assert.AreEqual(")", tokens[16].Value);
		}

		[TestMethod]
		public void Test01()
		{
			string s = "m =6";
			var tokens = new DefaultTokenStream(s).ParseAll();
			Console.WriteLine(string.Join(' ', tokens.Select(a => a.Value)));
			Assert.AreEqual(3, tokens.Count);
			Assert.AreEqual("m", tokens[0].Value);
			Assert.AreEqual("=", tokens[1].Value);
			Assert.AreEqual("6", tokens[2].Value);
		}
	}
}
