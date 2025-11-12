using System.Text.Json;

using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Establishment> Establishments => Set<Establishment>();

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
