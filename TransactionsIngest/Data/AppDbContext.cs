using Microsoft.EntityFrameworkCore;
using TransactionsIngest.Models;

namespace TransactionsIngest.Data;

public class AppDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<TransactionAudit> TransactionAudits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=transactions.db");
    }
}