using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AScript.Lang.Sql
{
	public class SqlTable : IEnumerable<DataRow>
	{
		private readonly DataTable _Table;

		public DataTable Table => _Table;

		public string TableName => _Table.TableName;
		public DataRowCollection Rows => _Table.Rows;
		public DataColumnCollection Columns => _Table.Columns;

		public SqlTable()
		{
			_Table = new DataTable();
		}
		public SqlTable(string tableName)
		{
			_Table = new DataTable(tableName);
		}
		public SqlTable(DataTable table)
		{
			_Table = table;
		}

		public void RemoveRange(IEnumerable<DataRow> rows)
		{
			if (rows != null)
			{
				foreach (var row in rows)
				{
					_Table.Rows.Remove(row);
				}
			}
		}

		IEnumerator<DataRow> IEnumerable<DataRow>.GetEnumerator()
		{
			return _Table.Rows.Cast<DataRow>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Table.Rows.Cast<DataRow>().GetEnumerator();
		}
	}
}
