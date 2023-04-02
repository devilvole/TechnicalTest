using TechnicalTest.API;
using TechnicalTest.API.Services;
using TechnicalTest.Data;

namespace ServiceTests
{
	public class CustomerServiceTests
	{
		ICustomerService service = new CustomerService(new ApplicationContext());

		[Fact]
		public async void TestAdd()
		{
			var rc = await service.AddCustomer(new TechnicalTest.API.Models.AddCustomerModel("", DateTime.Now, default(decimal)));
			Assert.NotNull(rc);
		}
	}
}