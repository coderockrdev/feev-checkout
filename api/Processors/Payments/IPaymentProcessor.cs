using System.Text.Json;

using FeevCheckout.DTOs;
using FeevCheckout.Enums;
using FeevCheckout.Models;

namespace FeevCheckout.Processors.Payments;

public class PaymentResult
{
    public required bool Success { get; set; }

    public required PaymentMethod Method { get; set; }

    public required PaymentAttemptStatus Status { get; set; }

    public required string ExternalId { get; set; }

    public required JsonDocument ExtraData { get; set; }

    public required JsonDocument Response { get; set; }
}

public interface IPaymentProcessor
{
    PaymentMethod Method { get; }

    Task<PaymentResult> ProcessAsync(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment,
        PaymentRequestDto request
    );
}
