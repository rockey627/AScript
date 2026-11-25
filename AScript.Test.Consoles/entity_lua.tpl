@{
	@lang lua
	local now = os.time();
}
// created at ${os.date('%Y-%m-%d', now)} by ${author}
using System;

namespace MyTest.Entity
{
	/// <summary>
	/// ${model.Memo}
	/// </summary>
	[Table("${model.TableName}")]
	public class ${model.Name} {
	@for column in model.Columns do
		/// <summary>
		/// ${column.Memo}
		/// </summary>
		@if column.IsPrimaryKey then
		[PrimaryKey]
		@end
		public ${column.TypeName} ${column.Name} { get; set; }
	@end
	}
}