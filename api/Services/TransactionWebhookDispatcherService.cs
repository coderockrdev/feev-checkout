using System.Text.Json;

using FeevCheckout.Events;
using FeevCheckout.Models;

using Flurl.Http;
using Flurl.Http.Configuration;

namespace FeevCheckout.Services;

public interface ITransactionWebhookDispatcherService
{
    Task<bool> DispatchAsync(TransactionWebhookEvent @event, Transaction transaction);
}

public class TransactionWebhookDispatcherService() : ITransactionWebhookDispatcherService
{
    public async Task<bool> DispatchAsync(TransactionWebhookEvent @event, Transaction transaction)
    {

        var request = new FlurlRequest(transaction.CallbackUrl)
            .WithTimeout(30)
            .WithSettings(settings => settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
            .WithHeaders(new
            {
                Accept = "application/json"
            });

        try
        {
            await request.PostJsonAsync(new
            {
                Event = @event.Name,
                OccurredAt = DateTime.UtcNow,
                Data = new
                {
                    TransactionId = transaction.Id,
                    Identifier = transaction.Identifier,
                    PaymentAttemptId = transaction.SuccessfulPaymentAttempt?.Id,
                    ExternalId = transaction.SuccessfulPaymentAttempt?.ExternalId
                }
            });

            return true;
        }
        catch
        {
            return false;
        }
    }
}
