using FeevCheckout.Models;

namespace FeevCheckout.Services.Payments;

public class PaymentResult
{
    public required bool Success { get; set; }

    public required string Method { get; set; }

    public required string ReferenceId { get; set; }

    public object? ExtraData { get; set; }
}

public interface IPaymentProcessor
{
    string Method { get; }

    Task<PaymentResult> ProcessAsync(Credential credentials, Transaction transaction);
}
