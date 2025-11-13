using FeevCheckout.Data;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Services;

public interface IPaymentService
{
    Task<PaymentResult> Process(Transaction transaction, string method, int installments);
}

public class PaymentService(
    AppDbContext context,
    PaymentProcessorFactory paymentProcessorFactory,
    ICredentialService credentialService) : IPaymentService
{
    private readonly AppDbContext _context = context;

    private readonly ICredentialService _credentialService = credentialService;

    private readonly PaymentProcessorFactory _paymentProcessorFactory = paymentProcessorFactory;

    public async Task<PaymentResult> Process(Transaction transaction, string method, int installments)
    {
        var processor = ResolveProcessor(transaction, method) ??
                        throw new InvalidOperationException($"No processor registered for '{method}'.");

        var credentials = await ResolveCredentials(transaction, method) ??
                          throw new InvalidOperationException($"No credentials registered for '{method}'.");

        var attempt = await RegisterAttemp(transaction, method);

        try
        {
            var result = await processor.ProcessAsync(credentials, transaction);

            await UpdateAttempt(attempt, result.ReferenceId, "pending");

            return result;
        }
        catch (Exception)
        {
            await UpdateAttempt(attempt, null, "failed");
            throw;
        }
    }

    private IPaymentProcessor? ResolveProcessor(Transaction transaction, string method)
    {
        var rule = transaction.PaymentRules
                       .FirstOrDefault(paymentRule =>
                           string.Equals(paymentRule.Type, method, StringComparison.OrdinalIgnoreCase)) ??
                   throw new InvalidOperationException(
                       $"Payment method '{method}' not supported for this transaction.");

        return _paymentProcessorFactory.GetProcessor(method);
    }

    private Task<Credential?> ResolveCredentials(Transaction transaction, string method)
    {
        return _credentialService.GetCredentials(transaction.EstablishmentId, method);
    }

    private async Task<PaymentAttempt> RegisterAttemp(Transaction transaction, string method)
    {
        var paymentAttempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Method = method,
            ReferenceId = null,
            Status = "created",
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentAttempts.Add(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<PaymentAttempt> UpdateAttempt(PaymentAttempt paymentAttempt, string? referenceId, string status)
    {
        paymentAttempt.ReferenceId = referenceId;
        paymentAttempt.Status = status;

        _context.PaymentAttempts.Update(paymentAttempt);
        await _context.SaveChangesAsync();

        return paymentAttempt;
    }
}
