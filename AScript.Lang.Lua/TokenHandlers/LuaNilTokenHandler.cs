using AScript.Syntaxs;

namespace AScript.Lang.Lua.TokenHandlers
{
	/// <summary>
	/// nil 表示空值
	/// </summary>
	public class LuaNilTokenHandler : ITokenHandler
	{
		public static readonly LuaNilTokenHandler Instance = new LuaNilTokenHandler();

		public void Build(DefaultSyntaxAnalyzer analyzer, TokenAnalyzingArgs e)
		{
			e.IsHandled = true;
			if (!e.Ignore)
			{
				e.TreeBuilder.AddData(e.BuildContext, e.ScriptContext, e.Options, e.Control, null);
			}
		}
	}
}
