@* 使用@lang指定脚本语言，注意前面要再加一个@符号表示脚本行，有2个@符号：@@lang *@
@@lang js
// created at ${new Date().toString('yyyy-MM-dd')} by ${author}
using System;

namespace MyTest.Entity
{
	/// <summary>
	/// ${model.Memo}
	/// </summary>
	[Table("${model.TableName}")]
	public class ${model.Name} {
	@* js遍历集合，元素为列表下标 *@
	@for(var i in model.Columns) {
		@* 取元素 *@
		@var column = model.Columns[i]
		/// <summary>
		/// ${column.Memo}
		/// </summary>
		@if (column.IsPrimaryKey) {
		[PrimaryKey]
		@}
		public ${column.TypeName} ${column.Name} { get; set; }
	@}
	}
}