using FeevCheckout.Data;
using FeevCheckout.Dtos;
using FeevCheckout.Models;
using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services;

public interface ITransactionService
{
    Task<object> ListTransactions(int page, int pageSize);

    Task<Transaction> CreateTransaction(CreateTransactionRequest request);

    Task<bool> CancelTransaction(Guid id);
}

public class TransactionService(AppDbContext context) : ITransactionService
{
    private readonly AppDbContext _context = context;

    public async Task<object> ListTransactions(int page, int pageSize)
    {
        var query = _context.Transactions
            .Include(transaction => transaction.Products)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(transaction => transaction.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = transactions.Select(transaction => new
            {
                transaction.Id,
                transaction.EstablishmentId,
                Customer = new
                {
                    transaction.Customer.Name,
                    transaction.Customer.Document
                },
                Products = transaction.Products.Select(p => new
                {
                    p.Name,
                    p.Price
                }),
                transaction.TotalAmount,
                transaction.CreatedAt,
                transaction.CanceledAt
            })
        };
    }

    public async Task<Transaction> CreateTransaction(CreateTransactionRequest request)
    {
        var totalAmount = request.Products.Sum(product => product.Price);

        var paymentRules = request.PaymentRules.Select(paymentRule => new PaymentRule
        {
            Type = paymentRule.Type,
            Installments = [.. paymentRule.Installments.Select(installment =>
                {
                    var finalAmount = totalAmount;

                    if (installment.Fee.HasValue && installment.FeeType == "amount")
                        finalAmount += installment.Fee.Value;

                    if (installment.Fee.HasValue && installment.FeeType == "percentage")
                        finalAmount += (int)Math.Round(totalAmount * (installment.Fee.Value / 100m));

                    return new Installment
                    {
                        Number = installment.Number,
                        Fee = installment.Fee,
                        FeeType = installment.FeeType,
                        FinalAmount = finalAmount
                    };
                })],
            FirstInstallment = paymentRule.FirstInstallment,
            Interest = paymentRule.Interest,
            LateFee = paymentRule.LateFee
        });

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            EstablishmentId = request.EstablishmentId,
            Customer = new Customer
            {
                Name = request.Customer.Name,
                Document = request.Customer.Document,
            },
            TotalAmount = totalAmount,
            PaymentRules = [.. paymentRules],
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var products = request.Products.Select(product => new Product
        {
            Id = Guid.NewGuid(),
            Name = product.Name,
            Price = product.Price,
            TransactionId = transaction.Id
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        await Task.CompletedTask;

        return transaction;
    }

    public async Task<bool> CancelTransaction(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null || transaction.CanceledAt != null)
            return false;

        transaction.CanceledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
