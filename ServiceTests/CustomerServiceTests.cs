using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.API;
using TechnicalTest.API.Services;
using TechnicalTest.Data;

namespace ServiceTests
{
	public class CustomerServiceTests
	{
		ICustomerService service;

		public CustomerServiceTests()
		{

			var options = new DbContextOptionsBuilder<ApplicationContext>()
				.UseSqlite("Data Source=database.db;")
				.Options;
			ApplicationContext context = new(options);
			service = new CustomerService(context);
			context.Database.EnsureCreated();
		}

		[Fact]
		public async void TestAdd()
		{
			var rc = await service.AddCustomer(new TechnicalTest.API.Models.AddCustomerModel("", DateTime.Now, default(decimal)));
			Assert.NotNull(rc);
		}
	}
}