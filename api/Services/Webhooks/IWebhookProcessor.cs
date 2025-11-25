using System.Text.Json;

using FeevCheckout.Enums;

namespace FeevCheckout.Services.Webhooks;

public interface IWebhookProcessor
{
    PaymentMethod Method { get; }

    Task<object> ProcessAsync(JsonElement Payload);
}
