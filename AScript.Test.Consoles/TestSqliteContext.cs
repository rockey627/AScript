using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles
{
	public class TestSqliteContext : DbContext
	{
		public DbSet<Person> Persons { get; set; }
		public DbSet<AddressInfo> AddressInfos { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.LogTo(Console.WriteLine);

			optionsBuilder.UseSqlite("Data Source=./test.db");
		}
	}
}
