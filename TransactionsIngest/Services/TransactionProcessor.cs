using TransactionsIngest.Data;
using TransactionsIngest.Models;

namespace TransactionsIngest.Services;

public class TransactionProcessor
{
    public void Process(List<Transaction> snapshot)
    {
        using var db = new AppDbContext();
        using var dbTransaction = db.Database.BeginTransaction();

        int inserted = 0;
        int updated = 0;
        int revoked = 0;
        int finalized = 0;

        var snapshotIds = snapshot
            .Select(t => t.TransactionId)
            .ToHashSet();

        foreach (var incoming in snapshot)
        {
            var existing = db.Transactions
                .FirstOrDefault(t => t.TransactionId == incoming.TransactionId);

            if (existing == null)
            {
                incoming.Status = "Active";
                incoming.LastUpdated = DateTime.UtcNow;

                db.Transactions.Add(incoming);
                inserted++;
            }
            else
            {
                if (existing.Status == "Finalized")
                    continue;

                bool changed = false;

                if (existing.Amount != incoming.Amount)
                {
                    db.TransactionAudits.Add(new TransactionAudit
                    {
                        TransactionId = incoming.TransactionId,
                        FieldName = "Amount",
                        OldValue = existing.Amount.ToString(),
                        NewValue = incoming.Amount.ToString()
                    });

                    existing.Amount = incoming.Amount;
                    changed = true;
                }

                if (changed)
                {
                    existing.LastUpdated = DateTime.UtcNow;
                    updated++;
                }
            }
        }

        var recentTransactions = db.Transactions
            .Where(t => t.TransactionTime > DateTime.UtcNow.AddHours(-24))
            .ToList();

        foreach (var existing in recentTransactions)
        {
            if (!snapshotIds.Contains(existing.TransactionId) &&
                existing.Status == "Active")
            {
                existing.Status = "Revoked";

                db.TransactionAudits.Add(new TransactionAudit
                {
                    TransactionId = existing.TransactionId,
                    FieldName = "Status",
                    OldValue = "Active",
                    NewValue = "Revoked"
                });

                revoked++;
            }
        }

        var oldTransactions = db.Transactions
            .Where(t => t.TransactionTime <= DateTime.UtcNow.AddHours(-24))
            .Where(t => t.Status != "Finalized")
            .ToList();

        foreach (var transaction in oldTransactions)
        {
            transaction.Status = "Finalized";
            finalized++;
        }

        db.SaveChanges();

        dbTransaction.Commit();

        Console.WriteLine("--------------------------------");
        Console.WriteLine("Snapshot Processing Complete");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Inserted:   {inserted}");
        Console.WriteLine($"Updated:    {updated}");
        Console.WriteLine($"Revoked:    {revoked}");
        Console.WriteLine($"Finalized:  {finalized}");
        Console.WriteLine("--------------------------------");
    }
}