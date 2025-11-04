using FeevCheckout.Models;
using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
}
