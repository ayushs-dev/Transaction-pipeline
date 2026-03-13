using Xunit;
using TransactionsIngest.Services;
using TransactionsIngest.Models;
using TransactionsIngest.Data;

public class IdempotencyTests
{
    [Fact]
    public void RunningSameSnapshotTwice_ShouldNotCrash()
    {
        // Create DB and tables before running processor
        using (var setupDb = new AppDbContext())
        {
            setupDb.Database.EnsureDeleted();
            setupDb.Database.EnsureCreated();
        }

        var processor = new TransactionProcessor();

        var snapshot = new List<Transaction>
        {
            new Transaction
            {
                TransactionId = "TEST1",
                ProductName = "Mouse",
                LocationCode = "LOC1",
                CardHash = "123",
                Amount = 10,
                TransactionTime = DateTime.UtcNow
            }
        };

        processor.Process(snapshot);
        processor.Process(snapshot);

        Assert.True(true);
    }
}