using FeevCheckout.Data;
using FeevCheckout.DTOs;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services;

public interface ITransactionService
{
    Task<PagedResult<Transaction>> ListTransactions(Guid establishmentId, int page, int pageSize);

    Task<Transaction?> GetTransaction(Guid establishmentId, Guid id);

    Task<Transaction?> GetPublicTransaction(Guid id);

    Task<Transaction> CreateTransaction(Guid establishmentId, CreateTransactionRequest request);

    Task<bool> CancelTransaction(Guid establishmentId, Guid id);
}

public class TransactionService(AppDbContext context) : ITransactionService
{
    private readonly AppDbContext context = context;

    public async Task<PagedResult<Transaction>> ListTransactions(Guid establishmentId, int page, int pageSize)
    {
        var query = context.Transactions
            .Include(transaction => transaction.Establishment)
            .Include(transaction => transaction.Products)
            .Where(transaction => transaction.EstablishmentId == establishmentId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(transaction => transaction.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Transaction>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = transactions
        };
    }

    public async Task<Transaction?> GetTransaction(Guid establishmentId, Guid id)
    {
        return await context.Transactions
            .Include(transaction => transaction.Establishment)
            .Include(transaction => transaction.Products)
            .FirstOrDefaultAsync(transaction =>
                transaction.Id == id &&
                transaction.EstablishmentId == establishmentId
            );
    }

    public async Task<Transaction?> GetPublicTransaction(Guid id)
    {
        return await context.Transactions
            .Include(transaction => transaction.Establishment)
            .Include(transaction => transaction.Products)
            .FirstOrDefaultAsync(transaction => transaction.Id == id);
    }

    public async Task<Transaction> CreateTransaction(Guid establishmentId, CreateTransactionRequest request)
    {
        var totalAmount = request.Products.Sum(product => product.Price);

        var paymentRules = request.PaymentRules.Select(paymentRule => new PaymentRule
        {
            Method = paymentRule.Method,
            Installments =
            [
                .. paymentRule.Installments.Select(installment =>
                {
                    return new Installment
                    {
                        Number = installment.Number,
                        DueAt = installment.DueAt,
                        ExpireAt = installment.ExpireAt,
                        Fee = installment.Fee,
                        FeeType = installment.FeeType,
                        FinalAmount = CalculateInstallmentFinalAmount(installment, totalAmount)
                    };
                })
            ],
            FirstInstallment = paymentRule.FirstInstallment,
            Interest = paymentRule.Interest,
            LateFee = paymentRule.LateFee
        });

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            EstablishmentId = establishmentId,
            Identifier = request.Identifier,
            Description = request.Description,
            Customer = new Customer
            {
                Name = request.Customer.Name,
                Document = request.Customer.Document,
                Email = request.Customer.Email,
                Address = new Address
                {
                    ZipCode = request.Customer.Address.ZipCode,
                    State = request.Customer.Address.State,
                    City = request.Customer.Address.City,
                    District = request.Customer.Address.District,
                    Street = request.Customer.Address.Street,
                    Number = request.Customer.Address.Number,
                    Complement = request.Customer.Address.Complement
                }
            },
            TotalAmount = totalAmount,
            PaymentRules = [.. paymentRules],
            ExpireAt = request.ExpireAt ?? DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var products = request.Products.Select(product => new Product
        {
            Id = Guid.NewGuid(),
            Name = product.Name,
            Price = product.Price,
            TransactionId = transaction.Id
        }).ToList();

        context.Products.AddRange(products);

        await context.SaveChangesAsync();

        transaction = await GetTransaction(establishmentId, transaction.Id);

        return transaction!;
    }

    public async Task<bool> CancelTransaction(Guid establishmentId, Guid id)
    {
        var transaction = await context.Transactions
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.EstablishmentId == establishmentId);

        if (transaction == null || transaction.CanceledAt != null || transaction.Status != TransactionStatus.Available)
            return false;

        transaction.CanceledAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return true;
    }

    private static int CalculateInstallmentFinalAmount(InstallmentDto installment, int totalAmount)
    {
        var finalAmount = totalAmount;

        if (installment.Fee.HasValue && installment.FeeType == "amount")
            finalAmount += installment.Fee.Value;

        if (installment.Fee.HasValue && installment.FeeType == "percentage")
            finalAmount += (int)Math.Round(totalAmount * (installment.Fee.Value / 100m));

        return finalAmount;
    }
}
