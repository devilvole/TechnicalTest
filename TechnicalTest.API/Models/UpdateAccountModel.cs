namespace TechnicalTest.API.Models;

public record UpdateAccountModel( string AccountNumber, bool Frozen, string customerName, decimal Amount );