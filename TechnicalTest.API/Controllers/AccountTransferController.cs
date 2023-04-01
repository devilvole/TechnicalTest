using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.Data;


namespace TechnicalTest.API.Controllers
{
	public class AccountTransferController : Controller
	{
		ApplicationContext db;

		public AccountTransferController(ApplicationContext db)
		{
			this.db = db;
		}

		[HttpPost]
		[Route("[controller]/AccountTransfer")]
		public async Task<IResult> AccountTransfer([FromBody] AccountTransferModel transfer)
		{
			var customer = db.Customers.Include(r => r.BankAccounts).Include(r => r.AccountTransfers).FirstOrDefault(x => x.Id == transfer.CustomerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			if (transfer.SourceAccountId == transfer.DestinationAccountId)
			{
				return Results.UnprocessableEntity("Accounts may not be the same");
			}

			var source = customer.BankAccounts.Where(x => x.Id == transfer.SourceAccountId).FirstOrDefault();
			var destination = customer.BankAccounts.Where(x => x.Id == transfer.DestinationAccountId).FirstOrDefault();

			if (source == null || destination == null)
			{
				return Results.UnprocessableEntity("Account not found");
			}

			if (source.Frozen || destination.Frozen)
			{
				return Results.UnprocessableEntity("Account frozen");
			}

			if (transfer.amount > customer.TransferLimit)
			{
				return Results.UnprocessableEntity("Transfer amount over limit");
			}

			var newTransfer = new AccountTransfer()
			{
				Amount = transfer.amount,
				SourceAccount = source,
				DestinationAccount = destination,
			};

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		[HttpPost]
		[Route("[controller]/GetTransfers")]
		public async Task<IResult> Get([FromQuery] int customerId)
		{
			var customer = db.Customers.Include(r => r.AccountTransfers).FirstOrDefault(x => x.Id == customerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

//			var accounts = customer.AccountTransfers.Select(x => new { x.SourceAccount.AccountNumber, x.DestinationAccount.AccountNumber }).ToList();

			return Results.Ok();
		}
	}
}
