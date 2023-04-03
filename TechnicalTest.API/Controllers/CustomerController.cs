using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.API.Services;
using TechnicalTest.Data;

namespace TechnicalTest.API.Controllers
{
	public class CustomerController : Controller
	{
		ICustomerService customerService;

		public CustomerController(ICustomerService customerService)
		{
			this.customerService = customerService;
		}

		[HttpPost]
		[Route("[controller]/Add")]
		public async Task<IResult> Add([FromBody] AddCustomerModel customer)
		{
			return await customerService.AddCustomer(customer);
		}

		[HttpGet]
		[Route("[controller]/Get")]
		public async Task<IResult> Get([FromQuery] string name)
		{
			return await customerService.GetCustomer(name);
		}

		[HttpGet]
		[Route("[controller]/GetAll")]
		public async Task<IResult> GetAll()
		{
			return await customerService.GetAllCustomers();
		}

		[HttpPost]
		[Route("[controller]/Update")]
		public async Task<IResult> Update([FromBody] UpdateCustomerModel customer)
		{
			return await customerService.UpdateCustomer(customer);
		}

		[HttpPost]
		[Route("[controller]/Delete")]
		public async Task<IResult> Delete([FromQuery] string name)
		{
			return await customerService.DeleteCustomer(name);
		}
	}
}
