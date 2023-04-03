using Microsoft.AspNetCore.Mvc;
using TechnicalTest.API.Models;
using TechnicalTest.Data;

namespace TechnicalTest.API.Services
{
	public interface ICustomerService
	{
		Task<IResult> AddCustomer(AddCustomerModel customer);
		Task<IResult> DeleteCustomer(string name);
		Task<IResult> GetAllCustomers();
		Task<IResult> UpdateCustomer(UpdateCustomerModel updateModel);
		Task<IResult> GetCustomer(string name);


		Task<IResult> AddAccount(AddAccountModel account);
		Task<IResult> DeleteAccount(DeleteAccountModel model);
		Task<IResult> GetAllAccounts(string customerName);
		Task<IResult> UpdateAccount([FromBody] UpdateAccountModel account);

		Task<IResult> AccountTransfer(AccountTransferModel transfer);
		Task<IResult> GetTransfers(string customerName);
	}

}
