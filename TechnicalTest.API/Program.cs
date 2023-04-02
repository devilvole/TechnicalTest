using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.API.Models;
using TechnicalTest.Data;
using TechnicalTest.API.Services;

namespace BSpoke.Test;

public class Program
{
	/// <summary>
	/// Main Program
	/// </summary>
	/// <param name="args"></param>
	public static void Main(string[] args)
	{

		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddDbContext<ApplicationContext>();

		builder.Services.AddScoped<ICustomerService, CustomerService>();

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		using (var scope = app.Services.CreateScope())
		{
			var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
			db.Database.Migrate();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.MapGet("/", () => "Hello world");

		app.MapPost("/customers/", async ([FromBody] AddCustomerModel customer, ApplicationContext db) =>
		{
			db.Customers.Add(new Customer
			{
				Name = customer.Name,
			});

			await db.SaveChangesAsync();

			return Results.Ok();
		});

		app.MapGet("/customers/", (ApplicationContext db) =>
		{
			var customers = db.Customers
				.Select(x => new
				{
					x.Id,
					x.Name
				}).ToList();

			return Results.Ok(customers);
		});

		app.Run();
	}
}