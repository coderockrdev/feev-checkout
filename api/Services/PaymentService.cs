using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Services;

public interface IPaymentService
{
    Task<PaymentResult> Process(Transaction transaction, PaymentMethod method, int installments);
}

public class PaymentService(
    AppDbContext context,
    PaymentProcessorFactory paymentProcessorFactory,
    ICredentialService credentialService) : IPaymentService
{
    private readonly AppDbContext _context = context;

    private readonly ICredentialService _credentialService = credentialService;

    private readonly PaymentProcessorFactory _paymentProcessorFactory = paymentProcessorFactory;

    public async Task<PaymentResult> Process(Transaction transaction, PaymentMethod method, int installments)
    {
        var processor = ResolveProcessor(transaction, method) ??
                        throw new InvalidOperationException($"No processor registered for '{method}'.");

        var credentials = await _credentialService.GetCredentials(transaction.EstablishmentId, method) ??
                          throw new InvalidOperationException($"No credentials registered for '{method}'.");

        var paymentRules = transaction.PaymentRules.FirstOrDefault(paymentRule => paymentRule.Method == method) ??
                           throw new InvalidOperationException(
                               $"Payment method '{method}' not available for this transaction.");

        var installment = paymentRules.Installments.FirstOrDefault(installment => installment.Number == installments) ??
                          throw new InvalidOperationException(
                              $"Installments number '{installments}' not available for this payment method.");

        var attempt = await RegisterAttemp(transaction, method);

        try
        {
            var result = await processor.ProcessAsync(credentials, transaction, paymentRules, installment);

            await UpdateAttempt(attempt, result.ReferenceId, PaymentAttemptStatus.Pending);

            return result;
        }
        catch (Exception)
        {
            await UpdateAttempt(attempt, null, PaymentAttemptStatus.Failed);
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
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentAttempts.Add(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<PaymentAttempt> UpdateAttempt(
        PaymentAttempt paymentAttempt,
        string? referenceId,
        PaymentAttemptStatus status
    )
    {
        paymentAttempt.ReferenceId = referenceId;
        paymentAttempt.Status = status;

        _context.PaymentAttempts.Update(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }
}
