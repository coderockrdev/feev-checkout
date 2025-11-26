using System.Text.Json;

using FeevCheckout.Enums;

namespace FeevCheckout.Models;

public class PaymentAttempt
{
    public required Guid Id { get; set; }

    public required Guid TransactionId { get; set; }

    public Transaction? Transaction { get; set; }

    public required PaymentMethod Method { get; set; }

    public required string? ExternalId { get; set; }

    public required PaymentAttemptStatus Status { get; set; }

    public required JsonDocument? Response { get; set; }

    public required DateTime CreatedAt { get; set; }
}
