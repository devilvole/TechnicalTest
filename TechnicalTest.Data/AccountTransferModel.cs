namespace TechnicalTest.API.Models;

public record AccountTransferModel(int CustomerId, int SourceAccountId, int DestinationAccountId, decimal amount);