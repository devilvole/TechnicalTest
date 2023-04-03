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
				//				.UseSqlite("Data Source=database.db;")
				.UseSqlite("Data Source=:memory:")
				//				.UseSqlite("Data Source=")
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

		//	customer tests
		//	

		//	Transfer tests
		//	valid account - not the same
		//	valid account - both exist
		//	valid transfer amount - non-zero, non-negative
		//	source has sufficient funds
		//	neither account frozen
		//	transfer would not go over daily limit
		//	transfer limit tests to reset daily limit

	}
}