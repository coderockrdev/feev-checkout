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

public class TransactionWebhookDispatcherService(
    ILogger<TransactionWebhookDispatcherService> logger) : ITransactionWebhookDispatcherService
{
    private readonly ILogger logger = logger;

    public async Task<bool> DispatchAsync(TransactionWebhookEvent @event, Transaction transaction)
    {
        var request = new FlurlRequest(transaction.CallbackUrl)
            .WithTimeout(30)
            .WithSettings(settings => settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

        try
        {
            await request.PostJsonAsync(new
            {
                Event = @event.Name,
                OccurredAt = DateTime.UtcNow,
                Data = new
                {
                    TransactionId = transaction.Id,
                    transaction.Identifier,
                    PaymentAttemptId = transaction.SuccessfulPaymentAttempt?.Id,
                    transaction.SuccessfulPaymentAttempt?.ExternalId
                }
            });

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error while dispatching transaction event to {transaction.CallbackUrl} webhook URL");

            return false;
        }
    }
}
