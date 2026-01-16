using FeevCheckout.Data;
using FeevCheckout.DTOs;
using FeevCheckout.Events;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Processors.Payments;

namespace FeevCheckout.Services;

public interface IPaymentService
{
    Task<PaymentResult> Process(Transaction transaction, PaymentRequestDto request);
}

public class PaymentService(
    AppDbContext context,
    PaymentProcessorFactory paymentProcessorFactory,
    ICredentialService credentialService,
    IEstablishmentService establishmentService,
    ITransactionWebhookDispatcherService transactionWebhookDispatcherService
) : IPaymentService
{
    private readonly AppDbContext context = context;

    private readonly ICredentialService credentialService = credentialService;

    private readonly IEstablishmentService establishmentService = establishmentService;

    private readonly PaymentProcessorFactory paymentProcessorFactory = paymentProcessorFactory;

    private readonly ITransactionWebhookDispatcherService transactionWebhookDispatcherService = transactionWebhookDispatcherService;

    public async Task<PaymentResult> Process(Transaction transaction, PaymentRequestDto request)
    {
        var establishment = await establishmentService.GetEstablishment(transaction.EstablishmentId)
                            ?? throw new InvalidOperationException("Establishment not found or not available.");

        if (transaction.Status == TransactionStatus.Canceled)
            throw new InvalidOperationException("Canceled transactions cannot be paid.");

        if (transaction.Status == TransactionStatus.Expired)
            throw new InvalidOperationException("Expired transactions cannot be paid.");

        var processor = ResolveProcessor(transaction, request.Method) ??
                        throw new InvalidOperationException($"No processor registered for '{request.Method}'.");

        var credentials = await credentialService.GetCredentials(establishment.Id, request.Method) ??
                          throw new InvalidOperationException($"No credentials registered for '{request.Method}'.");

        var paymentRules =
            transaction.PaymentRules.FirstOrDefault(paymentRule => paymentRule.Method == request.Method) ??
            throw new InvalidOperationException(
                $"Payment method '{request.Method}' not available for this transaction.");

        var installment = (request.Method == PaymentMethod.FeevBoleto || request.Method == PaymentMethod.FeevPix
                              ? paymentRules.Installments.FirstOrDefault()
                              : paymentRules.Installments.FirstOrDefault(installment =>
                                  installment.Number == request.Installments)) ??
                          throw new InvalidOperationException(
                              $"Installments number '{request.Installments}' not available for this payment method.");

        var attempt = await RegisterAttempt(transaction, request.Method);

        await transactionWebhookDispatcherService.DispatchAsync(
            TransactionWebhookEvent.PaymentAttemptCreated,
            transaction
        );

        try
        {
            var result =
                await processor.ProcessAsync(establishment, credentials, transaction, paymentRules, installment,
                    request);

            await UpdateAttempt(attempt, PaymentAttemptStatus.Pending, result);
            await UpdateTransaction(transaction, attempt);

            await transactionWebhookDispatcherService.DispatchAsync(
                TransactionWebhookEvent.PaymentAttemptPending,
                transaction
            );

            return result;
        }
        catch (Exception)
        {
            await UpdateAttempt(attempt, PaymentAttemptStatus.Failed, null);

            await transactionWebhookDispatcherService.DispatchAsync(
                TransactionWebhookEvent.PaymentAttemptFailed,
                transaction
            );

            throw;
        }
    }

    private IPaymentProcessor? ResolveProcessor(
        Transaction transaction,
        PaymentMethod method
    )
    {
        var rule = transaction.PaymentRules
                       .FirstOrDefault(paymentRule => paymentRule.Method == method)
                   ?? throw new InvalidOperationException(
                       $"Payment method '{method}' not supported for this transaction."
                   );

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
            Status = PaymentAttemptStatus.Created,
            Response = null,
            ExtraData = null,
            CreatedAt = DateTime.UtcNow
        };

        context.PaymentAttempts.Add(paymentAttempt);
        await context.SaveChangesAsync();

        return paymentAttempt;
    }

    private async Task<PaymentAttempt> UpdateAttempt(
        PaymentAttempt paymentAttempt,
        PaymentAttemptStatus status,
        PaymentResult? paymentResult
    )
    {
        paymentAttempt.Status = status;

        if (paymentResult != null)
        {
            paymentAttempt.ExternalId = paymentResult.ExternalId;
            paymentAttempt.ExtraData = paymentResult.ExtraData;
            paymentAttempt.Response = paymentResult.Response;
        }

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
