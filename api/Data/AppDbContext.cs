using System.Text.Json;

using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Credential> Credentials => Set<Credential>();

    public DbSet<Establishment> Establishments => Set<Establishment>();

    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Credential>()
            .Property(transaction => transaction.Data)
            .HasConversion(
                data => JsonSerializer.Serialize(data, (JsonSerializerOptions?)null),
                data => JsonSerializer.Deserialize<Dictionary<string, string>>(data, (JsonSerializerOptions?)null)!
            )
            .HasColumnType("jsonb");

        builder.Entity<Transaction>()
            .OwnsOne(transaction => transaction.Customer, customer =>
            {
                customer.Property(customer => customer.Name).HasColumnName("CustomerName");
                customer.Property(customer => customer.Document).HasColumnName("CustomerDocument");
                customer.Property(customer => customer.Email).HasColumnName("CustomerEmail");

                customer.OwnsOne(customer => customer.Address, address =>
                {
                    address.Property(address => address.Street).HasColumnName("CustomerAddressStreet");
                    address.Property(address => address.City).HasColumnName("CustomerAddressCity");
                    address.Property(address => address.UF).HasColumnName("CustomerAddressUF");
                    address.Property(address => address.PostalCode).HasColumnName("CustomerAddressPostalCode");
                });
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
