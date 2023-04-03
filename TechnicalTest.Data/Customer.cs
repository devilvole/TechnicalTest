namespace TechnicalTest.Data;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Birthdate { get; set; }
	public decimal TransferLimit { get; set; }
	public decimal AmountTransferredToday { get; set; }
	public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
	public ICollection<AccountTransfer> AccountTransfers { get; set; } = new HashSet<AccountTransfer>();
}