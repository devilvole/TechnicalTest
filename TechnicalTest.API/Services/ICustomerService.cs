using Microsoft.AspNetCore.Mvc;
using TechnicalTest.API.Models;
using TechnicalTest.Data;

namespace TechnicalTest.API.Services
{
	public interface ICustomerService
	{
		Task<IResult> AddCustomer(AddCustomerModel customer);

		Task<IResult> DeleteCustomer(int customerId);

		Task<IResult> GetAll();

		Task<IResult> AddAccount(AddAccountModel account);

		Task<IResult> GetAllAccounts(int customerId);
		Task<IResult> AccountTransfer(AccountTransferModel transfer);

		Task<IResult> GetTransfers(int customerId);

		Task<IResult> UpdateCustomer(UpdateCustomerModel updateModel);
	}

}
