using System.Text.Json;

using FeevCheckout.Enums;

namespace FeevCheckout.Services.Webhooks;

public class FeevBoletoWebhookProcessor(
    FeevBoletoCancellationService cancellationService,
    FeevResponseFileHandling responseFileHandling
) : IWebhookProcessor
{
    private readonly FeevBoletoCancellationService cancellationService = cancellationService;

    private readonly FeevResponseFileHandling responseFileHandling = responseFileHandling;

    public PaymentMethod Method => PaymentMethod.FeevBoleto;

    public async Task<object> ProcessAsync(JsonElement payload)
    {
        var eventName = GetEventName(payload);

        switch (eventName)
        {
            case "fatura.cancelada":
            case "boleto.expirado":
                await cancellationService.Handle(eventName, payload);

                break;
            case "processamento.arquivo.retorno":
                await responseFileHandling.Handle(eventName, payload);

                break;
            default:
                throw new NotImplementedException($"Handle of {eventName} events is not implemeneted yet.");
        }

        return new
        {
            Status = "processed"
        };
    }

    private static string GetEventName(JsonElement payload)
    {
        var supportedEvents = new[]
        {
            "fatura.cancelada",
            "boleto.expirado",
            "processamento.arquivo.retorno"
        };

        if (!payload.TryGetProperty("Evento", out var eventType) &&
            !payload.TryGetProperty("evento", out eventType))
            throw new BadHttpRequestException("'Evento' (or 'evento') is required.");

        var eventName = eventType.GetString();

        if (string.IsNullOrWhiteSpace(eventName))
            throw new BadHttpRequestException("'Evento' (or 'evento') cannot be null or empty.");

        if (!supportedEvents.Contains(eventName))
            throw new BadHttpRequestException($"'{eventName}' is not a supported event.");

        return eventName;
    }
}
