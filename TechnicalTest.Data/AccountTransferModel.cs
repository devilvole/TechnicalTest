namespace TechnicalTest.API.Models;

public record AccountTransferModel(string CustomerName, string SourceAccount, string DestinationAccount, decimal Amount, string Reference);