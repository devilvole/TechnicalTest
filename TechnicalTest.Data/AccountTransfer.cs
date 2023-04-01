namespace TechnicalTest.Data;

public class AccountTransfer
{
	public int Id { get; set; }
	public int SourceAccountId { get; set; }
	public int DestinationAccountId { get; set; }
	public BankAccount? SourceAccount { get; set; }
	public BankAccount? DestinationAccount { get; set; }
	public decimal Amount { get; set; }
	public DateTime? TransferedAt { get; set; } = DateTime.UtcNow;
	public int CustomerId { get; set; }
	public Customer? Customer { get; set; }
}