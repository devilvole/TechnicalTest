using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechnicalTest.API.Models;
using TechnicalTest.Data;

namespace TechnicalTest.API.Controllers
{
	public class CustomerController : Controller
	{
		ApplicationContext db;

		public CustomerController(ApplicationContext db)
		{
			this.db = db;
		}

		[HttpPost]
		[Route("[controller]/Customer")]
		public async Task<IResult> Add([FromBody] AddCustomerModel customer)
		{
			db.Customers.Add(new Customer
			{
				Name = customer.Name,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		}
		 
		[HttpGet]
		[Route("[controller]/Customer")]
		public async Task<IResult> GetAll()
		{
			var customers = db.Customers
				.Select(x => new
				{
					x.Id,
					x.Name
				}).ToList();

			return Results.Ok(customers);
		}
	}
}
