using Microsoft.EntityFrameworkCore;

namespace PaymentProcessing.Database;

public class PaymentProcessingContext : DbContext
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public PaymentProcessingContext()
    {
    }

    public PaymentProcessingContext(DbContextOptions<PaymentProcessingContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(Config.CONNECTION_STRING));

        optionsBuilder.UseMySql(Config.CONNECTION_STRING, serverVersion, options => options.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null))
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
}