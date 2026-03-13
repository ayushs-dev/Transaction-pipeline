// using Microsoft.Extensions.Configuration;
// using TransactionsIngest.Data;
// using TransactionsIngest.Infrastructure;
// using TransactionsIngest.Services;

// Console.WriteLine("Starting transaction ingest...");

// var config = new ConfigurationBuilder()
//     .AddJsonFile("appsettings.json")
//     .Build();

// var snapshotPath = config["Api:SnapshotPath"];
// if (string.IsNullOrWhiteSpace(snapshotPath))
// {
//     throw new Exception("Snapshot path is not configured in appsettings.json");
// }
// using var db = new AppDbContext();
// db.Database.EnsureCreated();

// List<Transaction> snapshot;

// var reader = new SnapshotReader();

// if (!string.IsNullOrWhiteSpace(apiEndpoint))
// {
// Console.WriteLine("Fetching snapshot from API...");

// using var httpClient = new HttpClient();
// var json = await httpClient.GetStringAsync(apiEndpoint);

// var options = new JsonSerializerOptions
// {
// PropertyNameCaseInsensitive = true
// };

// snapshot = JsonSerializer.Deserialize<List<Transaction>>(json, options)
// ?? new List<Transaction>();
// }
// else
// {
// if (string.IsNullOrWhiteSpace(snapshotPath))
// {
// throw new Exception("Neither API endpoint nor snapshot path is configured.");
// }

// Console.WriteLine("Reading snapshot from file...");
// snapshot = reader.ReadSnapshot(snapshotPath);
// }

// var processor = new TransactionProcessor();
// processor.Process(snapshot);
// var reader = new SnapshotReader();
// var snapshot = reader.ReadSnapshot(snapshotPath);

// var processor = new TransactionProcessor();
// processor.Process(snapshot);

// Console.WriteLine("Processing complete.");

using Microsoft.Extensions.Configuration;
using TransactionsIngest.Data;
using TransactionsIngest.Infrastructure;
using TransactionsIngest.Services;
using TransactionsIngest.Models;

Console.WriteLine("Starting transaction ingest...");

// Load configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Read configuration values
var endpoint = config["Api:Endpoint"];
var snapshotPath = config["Api:SnapshotPath"];

// Ensure database exists
using var db = new AppDbContext();
db.Database.EnsureCreated();

var reader = new SnapshotReader();
List<Transaction> snapshot;

// Decide source: API or Mock JSON
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

// Process transactions
var processor = new TransactionProcessor();
processor.Process(snapshot);

Console.WriteLine("Processing complete.");