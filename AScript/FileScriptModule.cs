using System;

namespace AScript
{
	public class FileScriptModule : IScriptModule
	{
		public string[] Langs { get; private set; }
		public string FilePath { get; private set; }
		public BuildOptions Options { get; set; }

		public FileScriptModule(string filePath)
		{
			FilePath = filePath;
		}
		public FileScriptModule(string filePath, string[] langs) : this(filePath)
		{
			Langs = langs;
		}

		public object Install(BaseContext context)
		{
			if (!(context is ScriptContext scriptContext))
			{
				throw new Exceptions.ScriptCustomException("FileScriptModule need ScriptContext");
			}
			if (Langs == null)
			{
				return Script.Eval(scriptContext, this.Options, System.IO.File.OpenRead(FilePath), out _);
			}
			var oldLangs = scriptContext.Langs;
			try
			{
				scriptContext.Langs = Langs;
				return Script.Eval(scriptContext, this.Options, System.IO.File.OpenRead(FilePath), out _);
			}
			finally
			{
				scriptContext.Langs = oldLangs;
			}
		}

		public void Uninstall(BaseContext context)
		{
		}
	}
}
