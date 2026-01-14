using FeevCheckout.Enums;

namespace FeevCheckout.DTOs;

public sealed record EstablishmentWebhookDto(
    Guid Id,
    string Name
);

public sealed record CustomerWebhookDto(
    string Name,
    string? Email
);

public sealed record PaymentAttemptWebhookDto(
    Guid Id,
    PaymentMethod Method,
    PaymentAttemptStatus Status
);

public sealed record ProductWebhookDto(
    Guid Id,
    string Name,
    int Price
);

public sealed record TransactionWebhookDto(
    Guid TransactionId,
    string Identifier,
    TransactionStatus Status,
    EstablishmentWebhookDto Establishment,
    List<ProductWebhookDto> Products,
    CustomerWebhookDto Customer,
    PaymentAttemptWebhookDto? PaymentAttempt,
    int TotalAmount
);
