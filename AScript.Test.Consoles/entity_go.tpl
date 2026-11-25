@{
	@lang go
	import 'time'
	var now = time.Now();
}
@*
golang中的时间格式：
2006 - yyyy
01 - MM
02 - dd
*@
// created at ${now.Format('2006-01-02')} by ${author}
using System;

namespace MyTest.Entity
{
	/// <summary>
	/// ${model.Memo}
	/// </summary>
	[Table("${model.TableName}")]
	public class ${model.Name} {
	@* golang使用range遍历列表，第1个参数为下标，第2个参数为元素 *@
	@for _,column := range model.Columns {
		/// <summary>
		/// ${column.Memo}
		/// </summary>
		@if column.IsPrimaryKey {
		[PrimaryKey]
		@}
		public ${column.TypeName} ${column.Name} { get; set; }
	@}
	}
}