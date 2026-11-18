using AScript.Readers;
using System;

namespace AScript.Lang.Go
{
	/// <summary>
	/// Go语言的词法分析器
	/// </summary>
	public class GoTokenStream : DefaultTokenStream
	{
		public GoTokenStream(CharReader charReader) : base(charReader)
		{
		}

		/// <summary>
		/// Go语言支持使用反引号包裹的原始字符串
		/// </summary>
		public override bool IsString(char c0)
		{
			return base.IsString(c0) || c0 == '`';
		}

		/// <summary>
		/// 解析字符串，反引号包裹的原始字符串中的内容不做转义处理
		/// </summary>
		protected override void ParseString(char startChar)
		{
			if (startChar != '`')
			{
				base.ParseString(startChar);
				return;
			}
			var c = CharReader.Read();
			while (c.HasValue)
			{
				if (c.Value == '`') return;
				_buffer.Append(c.Value);
				c = CharReader.Read();
			}
			throw new Exceptions.ScriptAnalyzingException($"invalid string at ({CharReader.CurrentLine},{CharReader.CurrentColumn}), expect `");
		}
	}
}
