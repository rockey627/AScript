using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.Consoles
{
	public class TestMySqlContext : DbContext
	{
		public DbSet<Person> Persons { get; set; }
		public DbSet<AddressInfo> AddressInfos { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.LogTo(Console.WriteLine);

			optionsBuilder.UseMySql("Data Source=127.0.0.1;port=3306;Initial Catalog=testdb;user id=root;password=xxxxxx;", new MySqlServerVersion("5.7.0"));// MySqlServerVersion.LatestSupportedServerVersion);
		}
	}
}
