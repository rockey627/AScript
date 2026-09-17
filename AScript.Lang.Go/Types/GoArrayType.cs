using AScript.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AScript.Lang.Go.Types
{
	public class GoArrayType : GoType
	{
		private readonly string _Name;
		private readonly Type _RealType;

		public override Type RealType => _RealType;

		public override string Name => _Name;
		public bool IsArray { get; private set; }

		public GoType ItemType { get; set; }
		public ITreeNode Length { get; set; }
		public ITreeNode Capacity { get; set; }

		public GoArrayType(bool isArray, GoType itemType, ITreeNode length, ITreeNode capacity)
		{
			IsArray = isArray;
			ItemType = itemType;
			Length = length;
			Capacity = capacity;
			if (isArray)
			{
				_RealType = itemType.RealType.MakeArrayType();
				// 
				if (length == null)
				{
					_Name = $"[...]{itemType.Name}";
				}
				else if (length is ObjectNode objectNode)
				{
					_Name = $"[{objectNode.Data}]{itemType.Name}";
				}
				else if (length is ExpressionNode expressionNode && expressionNode.Expr is ConstantExpression constantExpression)
				{
					_Name = $"[{constantExpression.Value}]{itemType.Name}";
				}
				else
				{
					_Name = $"[xxx]{itemType.Name}";
				}
			}
			else
			{
				_RealType = typeof(List<>).MakeGenericType(itemType.RealType);
				// 
				_Name = $"[]{itemType.Name}";
			}
		}
	}
}
