using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.API.Services;
using TechnicalTest.Data;

namespace TechnicalTest.API.Controllers
{
	public class AccountController : Controller
	{
		ICustomerService customerService;

		public AccountController(ICustomerService customerService)
		{
			this.customerService = customerService;
		}

		[HttpPost]
		[Route("[controller]/Account")]
		public async Task<IResult> Add([FromBody] AddAccountModel account)
		{
			return await customerService.AddAccount(account);
		}

		[HttpPost]
		[Route("[controller]/GetAccounts")]
		public async Task<IResult> Get([FromQuery] int customerId)
		{
			return await customerService.GetAllAccounts(customerId);
		}
	}
}
