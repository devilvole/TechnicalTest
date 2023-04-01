using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.Data;

namespace TechnicalTest.API.Controllers
{
	public class AccountController : Controller
	{
		ApplicationContext db;

		public AccountController(ApplicationContext db)
		{
			this.db = db;
		}

		[HttpPost]
		[Route("[controller]/Account")]
		public async Task<IResult> Add([FromBody] AddAccountModel account)
		{
			var customer = db.Customers.Include(r => r.BankAccounts).FirstOrDefault(x => x.Id == account.CustomerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			customer.BankAccounts.Add(new BankAccount
			{
				AccountNumber = account.AccountNumber,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		[HttpPost]
		[Route("[controller]/GetAccounts")]
		public async Task<IResult> Get([FromQuery] int customerId)
		{
			var customer = db.Customers.Include(r => r.BankAccounts).FirstOrDefault(x => x.Id == customerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var accounts = customer.BankAccounts.Select(x => new { x.AccountNumber, x.CustomerId }).ToList();

			return Results.Ok(accounts);
		}
	}
}
