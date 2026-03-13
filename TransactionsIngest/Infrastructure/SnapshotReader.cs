using System.Text.Json;
using TransactionsIngest.Models;

namespace TransactionsIngest.Infrastructure;

public class SnapshotReader
{
    public List<Transaction> ReadSnapshot(string filePath)
    {
        var json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<Transaction>>(json, options)
               ?? new List<Transaction>();
    }
}