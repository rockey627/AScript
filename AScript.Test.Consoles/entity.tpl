// created at ${DateTime.Now.ToString('yyyy-MM-dd')} by ${author}
using System;

@*
这里是注释块，不会输出
可以随便写，${xxx} @{xxx}**\@\
*@
namespace MyTest.Entity
{
	/// <summary>
	/// ${model.Memo}
	/// </summary>
	[Table("${model.TableName}")]
	public class ${model.Name} {
	@foreach(var column in model.Columns) {
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