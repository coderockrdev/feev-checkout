using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Services;

public interface IPaymentService
{
    Task<PaymentResult> Process(Transaction transaction, PaymentMethod method, int? installments);
}

public class PaymentService(
    AppDbContext context,
    PaymentProcessorFactory paymentProcessorFactory,
    ICredentialService credentialService) : IPaymentService
{
    private readonly AppDbContext _context = context;

    private readonly ICredentialService _credentialService = credentialService;

    private readonly PaymentProcessorFactory _paymentProcessorFactory = paymentProcessorFactory;

    public async Task<PaymentResult> Process(Transaction transaction, PaymentMethod method, int? installments)
    {
        if (transaction.Status == TransactionStatus.Canceled)
            throw new InvalidOperationException("Canceled transactions cannot be paid.");

        if (transaction.Status == TransactionStatus.Expired)
            throw new InvalidOperationException("Expired transactions cannot be paid.");

        var processor = ResolveProcessor(transaction, method) ??
                        throw new InvalidOperationException($"No processor registered for '{method}'.");

        var credentials = await _credentialService.GetCredentials(transaction.EstablishmentId, method) ??
                          throw new InvalidOperationException($"No credentials registered for '{method}'.");

        var paymentRules = transaction.PaymentRules.FirstOrDefault(paymentRule => paymentRule.Method == method) ??
                           throw new InvalidOperationException(
                               $"Payment method '{method}' not available for this transaction.");

        var installment = (method == PaymentMethod.FeevBoleto || method == PaymentMethod.FeevPix
                              ? paymentRules.Installments.FirstOrDefault()
                              : paymentRules.Installments.FirstOrDefault(installment =>
                                  installment.Number == installments)) ??
                          throw new InvalidOperationException(
                              $"Installments number '{installments}' not available for this payment method.");

        var attempt = await RegisterAttemp(transaction, method);

        try
        {
            var result = await processor.ProcessAsync(credentials, transaction, paymentRules, installment);

            await UpdateAttempt(attempt, result.ReferenceId, PaymentAttemptStatus.Pending, result.Response);
            await UpdateTransaction(transaction, attempt);

            return result;
        }
        catch (Exception)
        {
            await UpdateAttempt(attempt, null, PaymentAttemptStatus.Failed, null);
            throw;
        }
    }

    private IPaymentProcessor? ResolveProcessor(Transaction transaction, PaymentMethod method)
    {
        var rule = transaction.PaymentRules
                       .FirstOrDefault(paymentRule => paymentRule.Method == method) ??
                   throw new InvalidOperationException(
                       $"Payment method '{method}' not supported for this transaction.");

        return _paymentProcessorFactory.GetProcessor(method);
    }

    private async Task<PaymentAttempt> RegisterAttemp(Transaction transaction, PaymentMethod method)
    {
        var paymentAttempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Method = method,
            ReferenceId = null,
            Status = PaymentAttemptStatus.Created,
            Response = null,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentAttempts.Add(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<PaymentAttempt> UpdateAttempt(
        PaymentAttempt paymentAttempt,
        string? referenceId,
        PaymentAttemptStatus status,
        object? response
    )
    {
        paymentAttempt.ReferenceId = referenceId;
        paymentAttempt.Status = status;
        paymentAttempt.Response = response;

        _context.PaymentAttempts.Update(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<Transaction> UpdateTransaction(Transaction transaction, PaymentAttempt paymentAttempt)
    {
        transaction.SuccessfulPaymentAttemptId = paymentAttempt.Id;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}
