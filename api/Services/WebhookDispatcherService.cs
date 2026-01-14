using System.Text.Json;
using System.Text.Json.Serialization;

using FeevCheckout.Libraries.Http;
using FeevCheckout.DTOs;

using Flurl.Http;
using System.Text;

namespace FeevCheckout.Services;

public interface IWebhookDispatcherService
{
    Task<bool> DispatchAsync(TransactionWebhookDto payload);
}

public class WebhookDispatcherService(IFeevWebhookClient feevWebhookClient) : IWebhookDispatcherService
{
    private readonly IFeevWebhookClient feevWebhookClient = feevWebhookClient;

    private static readonly JsonSerializerOptions WebhookJsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
            }
        };

    public async Task<bool> DispatchAsync(TransactionWebhookDto payload)
    {
        var request = feevWebhookClient.CreateRequest("/consumer");

        try
        {
            var json = JsonSerializer.Serialize(payload, WebhookJsonOptions);

            await request.PostAsync(
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            return true;
        }
        catch
        {
            return false;
        }
    }
}
