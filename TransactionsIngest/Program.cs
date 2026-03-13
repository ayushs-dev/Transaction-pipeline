using Microsoft.Extensions.Configuration;
using TransactionsIngest.Data;
using TransactionsIngest.Infrastructure;
using TransactionsIngest.Services;
using TransactionsIngest.Models;

Console.WriteLine("Starting transaction ingest...");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var endpoint = config["Api:Endpoint"];
var snapshotPath = config["Api:SnapshotPath"];

using var db = new AppDbContext();
db.Database.EnsureCreated();

var reader = new SnapshotReader();
List<Transaction> snapshot;

if (!string.IsNullOrWhiteSpace(endpoint))
{
    Console.WriteLine("Fetching transactions from API...");

    using var http = new HttpClient();
    var json = await http.GetStringAsync(endpoint);

    snapshot = reader.ReadSnapshotFromJson(json);
}
else
{
    Console.WriteLine("Reading transactions from mock JSON...");

    if (string.IsNullOrWhiteSpace(snapshotPath))
    {
        throw new Exception("SnapshotPath not configured in appsettings.json");
    }

    snapshot = reader.ReadSnapshot(snapshotPath);
}

var processor = new TransactionProcessor();
processor.Process(snapshot);

Console.WriteLine("Processing complete.");