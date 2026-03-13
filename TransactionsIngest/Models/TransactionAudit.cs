namespace TransactionsIngest.Models;

public class TransactionAudit
{
    public int Id { get; set; }

    public string TransactionId { get; set; } = string.Empty;

    public string FieldName { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty;

    public string NewValue { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}