using System.Text.Json;

using FeevCheckout.Libraries.Http;
using FeevCheckout.DTOs;

using Flurl.Http;
using Flurl.Http.Configuration;

namespace FeevCheckout.Services;

public interface IWebhookDispatcherService
{
    Task<bool> DispatchAsync(TransactionWebhookDto payload);
}

public class WebhookDispatcherService(IFeevWebhookClient feevWebhookClient) : IWebhookDispatcherService
{
    private readonly IFeevWebhookClient feevWebhookClient = feevWebhookClient;

    public async Task<bool> DispatchAsync(TransactionWebhookDto payload)
    {
        var request = feevWebhookClient.CreateRequest("/consumer");

        try
        {
            await request
                .WithSettings(settings => settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }))
                .PostJsonAsync(payload);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
