using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Services;

public interface IPaymentService
{
    Task<PaymentResult> Process(Guid establishmentId, Transaction transaction, PaymentMethod method, int? installments);
}

public class PaymentService(
    AppDbContext context,
    PaymentProcessorFactory paymentProcessorFactory,
    ICredentialService credentialService,
    IEstablishmentService establishmentService
) : IPaymentService
{
    private readonly AppDbContext context = context;

    private readonly ICredentialService credentialService = credentialService;

    private readonly IEstablishmentService establishmentService = establishmentService;

    private readonly PaymentProcessorFactory paymentProcessorFactory = paymentProcessorFactory;

    public async Task<PaymentResult> Process(Guid establishmentId, Transaction transaction, PaymentMethod method,
        int? installments)
    {
        var establishment = await establishmentService.GetEstablishment(establishmentId)
                            ?? throw new InvalidOperationException("Establishment not found or not available.");

        if (transaction.Status == TransactionStatus.Canceled)
            throw new InvalidOperationException("Canceled transactions cannot be paid.");

        if (transaction.Status == TransactionStatus.Expired)
            throw new InvalidOperationException("Expired transactions cannot be paid.");

        var processor = ResolveProcessor(transaction, method) ??
                        throw new InvalidOperationException($"No processor registered for '{method}'.");

        var credentials = await credentialService.GetCredentials(establishment.Id, method) ??
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

        var attempt = await RegisterAttempt(transaction, method);

        try
        {
            var result =
                await processor.ProcessAsync(establishment, credentials, transaction, paymentRules, installment);

            await UpdateAttempt(attempt, result.ExternalId, PaymentAttemptStatus.Created, result.Response);
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

        return paymentProcessorFactory.GetProcessor(method);
    }

    private async Task<PaymentAttempt> RegisterAttempt(Transaction transaction, PaymentMethod method)
    {
        var paymentAttempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            EstablishmentId = transaction.EstablishmentId,
            TransactionId = transaction.Id,
            Method = method,
            ExternalId = null,
            Status = PaymentAttemptStatus.Pending,
            Response = null,
            CreatedAt = DateTime.UtcNow
        };

        context.PaymentAttempts.Add(paymentAttempt);
        await context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<PaymentAttempt> UpdateAttempt(
        PaymentAttempt paymentAttempt,
        string? externalId,
        PaymentAttemptStatus status,
        JsonDocument? response
    )
    {
        paymentAttempt.ExternalId = externalId;
        paymentAttempt.Status = status;
        paymentAttempt.Response = response;

        context.PaymentAttempts.Update(paymentAttempt);
        await context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<Transaction> UpdateTransaction(Transaction transaction, PaymentAttempt paymentAttempt)
    {
        transaction.SuccessfulPaymentAttemptId = paymentAttempt.Id;

        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();

        return transaction;
    }
}
