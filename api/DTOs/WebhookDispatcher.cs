namespace FeevCheckout.DTOs;

public sealed record TransactionWebhookDto(
    string Event,
    DateTime OccurredAt,
    Guid TransactionId,
    string Identifier,
    Guid? PaymentAttemptId,
    string? ExternalId
);
