using FeevCheckout.Data;
using FeevCheckout.Dtos;
using FeevCheckout.Models;

namespace FeevCheckout.Services;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request);
}

public class TransactionService(AppDbContext context) : ITransactionService
{
    private readonly AppDbContext _context = context;

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request)
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
}
