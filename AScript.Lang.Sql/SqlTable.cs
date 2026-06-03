using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AScript.Lang.Sql
{
	public class SqlTable : DataTable, IEnumerable<DataRow>
	{
		public SqlTable()
		{
		}
		public SqlTable(string tableName) : base(tableName)
		{
		}

		IEnumerator<DataRow> IEnumerable<DataRow>.GetEnumerator()
		{
			return this.Rows.Cast<DataRow>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Rows.Cast<DataRow>().GetEnumerator();
		}
	}
}
