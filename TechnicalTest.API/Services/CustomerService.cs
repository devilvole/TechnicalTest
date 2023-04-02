using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.Xml;
using TechnicalTest.API.Models;
using TechnicalTest.Data;

namespace TechnicalTest.API.Services
{
	public class CustomerService : ICustomerService
	{
		ApplicationContext db;

		public CustomerService(ApplicationContext db)
		{
			this.db = db;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<IResult> AddCustomer(AddCustomerModel customer)
		{
			if (!await db.Customers.AnyAsync(x => x.Name == customer.Name))
			{
				db.Customers.Add(new Customer
				{
					Name = customer.Name,
					Birthdate = customer.Birthdate,
					TransferLimit = customer.TransferLimit,
				});

				await db.SaveChangesAsync();
				return Results.Ok();
			}
			else
			{
				return Results.UnprocessableEntity("Customer already exists");
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<IResult> DeleteCustomer(int customerId)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).Include(r => r.AccountTransfers).FirstOrDefaultAsync(x => x.Id == customerId);

			if (customer is Customer)
			{
				// delete accounts etc
				foreach (var account in customer.BankAccounts)
				{
					db.BankAccounts.Remove(account);
				}

				foreach (var transaction in customer.AccountTransfers)
				{
					db.AccountTransfer.Remove(transaction);
				}

				await db.SaveChangesAsync();
				return Results.Ok();
			}
			else
			{
				return Results.UnprocessableEntity("Customer not found");
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<IResult> UpdateCustomer(UpdateCustomerModel updateModel)
		{
			var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == updateModel.Id);

			if (customer is Customer)
			{
				customer.Birthdate = updateModel.Birthdate;
				customer.TransferLimit = updateModel.TransferLimit;

				await db.SaveChangesAsync();
				return Results.Ok();
			}
			else
			{
				return Results.UnprocessableEntity("Customer not found");
			}
		}


		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<IResult> GetAll()
		{
			var customers = await db.Customers
				.Select(x => new
				{
					x.Id,
					x.Name,
					x.Birthdate,
					x.TransferLimit,
				}).ToListAsync();

			return Results.Ok(customers);
		}

		public async Task<IResult> AddAccount(AddAccountModel account)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Id == account.CustomerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			customer.BankAccounts.Add(new BankAccount
			{
				AccountNumber = account.AccountNumber,
				Frozen = account.Frozen,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>

		public async Task<IResult> GetAllAccounts(int customerId)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Id == customerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var accounts = customer.BankAccounts.Select(x => new { x.AccountNumber, x.CustomerId, x.Frozen, x.Id }).ToList();

			return Results.Ok(accounts);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>

		public async Task<IResult> AccountTransfer(AccountTransferModel transfer)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).Include(r => r.AccountTransfers).FirstOrDefaultAsync(x => x.Id == transfer.CustomerId);

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

			customer.AccountTransfers.Add(new AccountTransfer()
			{
				Amount = transfer.amount,
				SourceAccount = source,
				DestinationAccount = destination,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		[HttpPost]
		[Route("[controller]/GetTransfers")]
		public async Task<IResult> GetTransfers(int customerId)
		{
			var customer = await db.Customers.Include(r => r.AccountTransfers).ThenInclude(x => x.SourceAccount).Include(r => r.AccountTransfers).ThenInclude(x => x.DestinationAccount).FirstOrDefaultAsync(x => x.Id == customerId);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var transfers = customer.AccountTransfers.Select(x => new { Source = x.SourceAccount.AccountNumber, Destination = x.DestinationAccount.AccountNumber, Amount = x.Amount}).ToList();

			return Results.Ok(transfers);
		}


	}
}
