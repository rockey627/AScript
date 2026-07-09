using System;
using System.Collections.Generic;
using System.IO;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptReadStream : JavaScriptStream
	{
		public JavaScriptReadStream(Stream stream) : base(stream)
		{
		}
		public JavaScriptReadStream(Stream stream, IDictionary<string, object> options) : base(stream, options)
		{
		}

		public override void on(string name, Delegate @event)
		{
			base.on(name, @event);

			// 异步读取文件

		}

		public void pipe(Stream stream)
		{

		}

		public void pipe(JavaScriptWriteStream stream)
		{

		}
	}
}
