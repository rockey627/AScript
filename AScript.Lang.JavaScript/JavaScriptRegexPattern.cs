using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AScript.Lang.JavaScript
{
	public class JavaScriptRegexPattern
	{
		private string _Pattern;
		private bool _SearchAll;
		private RegexOptions _Options;

		public string Value { get; private set; }

		public string Pattern
		{
			get
			{
				TryParse();
				return _Pattern;
			}
		}
		public bool SearchAll
		{
			get
			{
				TryParse();
				return _SearchAll;
			}
		}
		public RegexOptions Options
		{
			get
			{
				TryParse();
				return _Options;
			}
		}

		public JavaScriptRegexPattern(string value)
		{
			this.Value = value;
		}

		private void TryParse()
		{
			if (_Pattern != null) return;
			int lastIndex = this.Value.LastIndexOf('/');
			_Pattern = this.Value.Substring(1, lastIndex - 1);
			var p1 = this.Value.Substring(lastIndex + 1);
			_Options = p1.IndexOf('i') >= 0 ? RegexOptions.IgnoreCase : RegexOptions.None;
			_SearchAll = p1.IndexOf('g') >= 0;
		}

		public override string ToString()
		{
			return this.Value;
		}
	}
}
