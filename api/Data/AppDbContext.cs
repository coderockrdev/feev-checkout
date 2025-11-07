using System.Text.Json;
using FeevCheckout.Models;
using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transaction>()
            .OwnsOne(transaction => transaction.Customer, customer =>
            {
                customer.Property(customer => customer.Name).HasColumnName("CustomerName");
                customer.Property(customer => customer.Document).HasColumnName("CustomerDocument");
            });

        builder.Entity<Transaction>()
            .Property(transaction => transaction.PaymentRules)
            .HasConversion(
                paymentRule => JsonSerializer.Serialize(paymentRule, (JsonSerializerOptions?)null),
                paymentRule => JsonSerializer.Deserialize<List<PaymentRule>>(paymentRule, (JsonSerializerOptions?)null)!
            )
            .HasColumnType("jsonb");
    }
}
