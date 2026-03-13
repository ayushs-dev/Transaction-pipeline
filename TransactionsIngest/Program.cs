// using TransactionsIngest.Data;
// // using TransactionsIngest.Infrastructure;

// using TransactionsIngest.Services;

// Console.WriteLine("Starting transaction ingest...");

// using var db = new AppDbContext();
// db.Database.EnsureCreated();

// // var reader = new SnapshotReader();
// var reader = new TransactionsIngest.Infrastructure.SnapshotReader();
// //if in appsettings.json have api endpoint, then call that external api and fetch the json and that json should be passed to readsnapshot method 
// var snapshot = reader.ReadSnapshot("MockData/transactions.json");
// Console.WriteLine($"Snapshot loaded: {snapshot.Count} transactions");
// var processor = new TransactionProcessor();
// processor.Process(snapshot);

// Console.WriteLine("Processing complete.");
using Microsoft.Extensions.Configuration;
using TransactionsIngest.Data;
using TransactionsIngest.Infrastructure;
using TransactionsIngest.Services;

Console.WriteLine("Starting transaction ingest...");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var snapshotPath = config["Api:SnapshotPath"];
if (string.IsNullOrWhiteSpace(snapshotPath))
{
    throw new Exception("Snapshot path is not configured in appsettings.json");
}
using var db = new AppDbContext();
db.Database.EnsureCreated();

var reader = new SnapshotReader();
var snapshot = reader.ReadSnapshot(snapshotPath);

var processor = new TransactionProcessor();
processor.Process(snapshot);

Console.WriteLine("Processing complete.");