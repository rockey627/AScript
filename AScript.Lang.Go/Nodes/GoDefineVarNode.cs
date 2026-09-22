using AScript.Lang.Go.Types;
using AScript.Nodes;
using System;

namespace AScript.Lang.Go.Nodes
{
	public class GoDefineVarNode : DefineVarNode
	{
		private GoType _GoType;

		public GoType GoType
		{
			get => _GoType;
			set
			{
				_GoType = value;
				this.Type = value?.Name;
				this.SystemType = value?.RealType;
			}
		}

		public GoDefineVarNode(string varName) : base(varName) { }
		public GoDefineVarNode(string varName, GoType goType) : base(varName)
		{
			this.GoType = goType;
		}
	}
}
