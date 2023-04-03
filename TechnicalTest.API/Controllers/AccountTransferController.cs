using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.API.Services;
using TechnicalTest.Data;


namespace TechnicalTest.API.Controllers
{
	public class AccountTransferController : Controller
	{
		ICustomerService customerService;

		public AccountTransferController(ICustomerService customerService)
		{
			this.customerService = customerService;
		}


		[HttpPost]
		[Route("[controller]/Transfer")]
		public async Task<IResult> AccountTransfer([FromBody] AccountTransferModel transfer)
		{
			return await customerService.AccountTransfer(transfer);
		}

		[HttpPost]
		[Route("[controller]/GetTransfers")]
		public async Task<IResult> Get([FromQuery] string customerName)
		{
			return await customerService.GetTransfers(customerName);
		}
	}
}
