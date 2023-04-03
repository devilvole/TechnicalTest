using Microsoft.AspNetCore.Components;
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
		public async Task<IResult> DeleteCustomer(string name)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).Include(r => r.AccountTransfers).FirstOrDefaultAsync(x => x.Name == name);

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
			var customer = await db.Customers.FirstOrDefaultAsync(x => x.Name == updateModel.Name);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			customer.Birthdate = updateModel.Birthdate;
			customer.TransferLimit = updateModel.TransferLimit;

			await db.SaveChangesAsync();
			return Results.Ok();
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
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Name == account.customerName);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			customer.BankAccounts.Add(new BankAccount
			{
				AccountNumber = account.AccountNumber,
				Frozen = account.Frozen,
				Amount = account.Amount,
				Customer = customer,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>

		public async Task<IResult> GetAllAccounts(string customerName)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Name == customerName);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var accounts = customer.BankAccounts.Select(x => new { x.AccountNumber, x.Amount, x.Frozen, x.Id }).ToList();

			return Results.Ok(accounts);
		}

		public async Task<IResult> DeleteAccount(DeleteAccountModel model)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Name == model.customerName);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var account = customer.BankAccounts.Where(x => x.AccountNumber == model.AccountNumber).FirstOrDefault();

			if (account is not BankAccount)
			{
				return Results.UnprocessableEntity("account not found");
			}

			customer.BankAccounts.Remove(account);

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<IResult> UpdateAccount(UpdateAccountModel model)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).FirstOrDefaultAsync(x => x.Name == model.customerName);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var account = customer.BankAccounts.Where(x => x.AccountNumber == model.AccountNumber).FirstOrDefault();

			if (account is not BankAccount)
			{
				return Results.UnprocessableEntity("account not found");
			}

			account.Frozen = model.Frozen;
			account.Amount = model.Amount;

			await db.SaveChangesAsync();
			return Results.Ok();
		}



		/// <summary>
		/// <inheritdoc/>
		/// </summary>

		public async Task<IResult> AccountTransfer(AccountTransferModel model)
		{
			var customer = await db.Customers.Include(r => r.BankAccounts).Include(r => r.AccountTransfers).FirstOrDefaultAsync(x => x.Name == model.CustomerName);

			if (customer is not Customer)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			if (model.SourceAccount == model.DestinationAccount)
			{
				return Results.UnprocessableEntity("Accounts may not be the same");
			}

			var source = customer.BankAccounts.Where(x => x.AccountNumber == model.SourceAccount).FirstOrDefault();
			var destination = customer.BankAccounts.Where(x => x.AccountNumber == model.DestinationAccount).FirstOrDefault();

			if (source is not BankAccount || destination is not BankAccount)
			{
				return Results.UnprocessableEntity("Account not found");
			}

			if (source.Frozen || destination.Frozen)
			{
				return Results.UnprocessableEntity("Account frozen");
			}

			// reset daily limit
			var lastTransfer = customer.AccountTransfers.LastOrDefault();
			if (lastTransfer is AccountTransfer)
			{
				if( DateTime.Now.DayOfYear != lastTransfer.TransferedAt.DayOfYear) 
				{
					customer.AmountTransferredToday = 0;
				}
			}

			if (model.Amount + customer.AmountTransferredToday > customer.TransferLimit)
			{
				return Results.UnprocessableEntity("Transfer amount over limit");
			}

			if(model.Amount > source.Amount)
			{
				return Results.UnprocessableEntity("Source has unsufficient funds");
			}

			source.Amount -= model.Amount;
			destination.Amount += model.Amount;

			customer.AccountTransfers.Add(new AccountTransfer()
			{
				Amount = model.Amount,
				SourceAccount = source,
				DestinationAccount = destination,
			});

			customer.AmountTransferredToday += model.Amount;

			await db.SaveChangesAsync();

			return Results.Ok();
		}

		public async Task<IResult> GetTransfers(string customerName)
		{
			var customer = await db.Customers.Include(r => r.AccountTransfers).ThenInclude(x => x.SourceAccount).Include(r => r.AccountTransfers).ThenInclude(x => x.DestinationAccount).FirstOrDefaultAsync(x => x.Name == customerName);

			if (customer == null)
			{
				return Results.UnprocessableEntity("Customer not found");
			}

			var transfers = customer.AccountTransfers.Select(x => new { Source = x.SourceAccount.AccountNumber, Destination = x.DestinationAccount.AccountNumber, Amount = x.Amount }).ToList();

			return Results.Ok(transfers);
		}


	}
}
