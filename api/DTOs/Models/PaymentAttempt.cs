using System.Text.Json;

using FeevCheckout.Enums;
using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class PaymentAttempDto
{
    public required Guid Id { get; set; }

    public required PaymentMethod Method { get; set; }

    public required PaymentAttemptStatus Status { get; set; }

    public required JsonDocument? ExtraData { get; set; }

    public static PaymentAttempDto FromModel(PaymentAttempt model)
    {
        return new PaymentAttempDto
        {
            Id = model.Id,
            Method = model.Method,
            Status = model.Status,
            ExtraData = model.ExtraData
        };
    }
}
