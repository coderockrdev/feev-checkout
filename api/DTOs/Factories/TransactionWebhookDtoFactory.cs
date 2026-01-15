using FeevCheckout.Enums;
using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Factories;

public static class TransactionWebhookDtoFactory
{
    public static TransactionWebhookDto Create(TransactionEvent @event, Transaction transaction)
    {
        return new TransactionWebhookDto(
            Event: ToWebhookEvent(@event),
            OccurredAt: DateTime.UtcNow,
            TransactionId: transaction.Id,
            Identifier: transaction.Identifier,
            PaymentAttemptId: transaction.SuccessfulPaymentAttempt is null
                ? null
                : transaction.SuccessfulPaymentAttempt.Id,
            ExternalId: transaction.SuccessfulPaymentAttempt is null
                ? null
                : transaction.SuccessfulPaymentAttempt.ExternalId
        );
    }

    private static string ToWebhookEvent(TransactionEvent @event) =>
        @event switch
        {
            TransactionEvent.Created   => "transaction.created",
            TransactionEvent.Canceled  => "transaction.canceled",
            TransactionEvent.Expired  => "transaction.expired",
            TransactionEvent.Completed => "transaction.completed",
            TransactionEvent.PaymentCreated => "transaction.payment_created",
            TransactionEvent.PaymentFailed => "transaction.payment_failed",
            TransactionEvent.PaymentPending => "transaction.payment_pending",
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };
}
