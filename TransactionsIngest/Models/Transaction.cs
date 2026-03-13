namespace TransactionsIngest.Models;

public class Transaction
{
    public int Id { get; set; }

    public string TransactionId { get; set; } = string.Empty;

    public string CardHash { get; set; } = string.Empty;

    public string LocationCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionTime { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}