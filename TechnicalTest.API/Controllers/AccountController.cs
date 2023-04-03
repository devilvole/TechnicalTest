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
		public async Task<IResult> Get([FromQuery] string customerName)
		{
			return await customerService.GetAllAccounts(customerName);
		}

		[HttpPost]
		[Route("[controller]/DeleteAccount")]
		public async Task<IResult> Delete([FromQuery] DeleteAccountModel model)
		{
			return await customerService.DeleteAccount(model);
		}

		[HttpPost]
		[Route("[controller]/UpdateAccount")]
		public async Task<IResult> Update([FromBody] UpdateAccountModel account)
		{
			return await customerService.UpdateAccount(account);
		}

	}
}
