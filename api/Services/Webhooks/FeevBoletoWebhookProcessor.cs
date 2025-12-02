using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services.Webhooks;

public class FeevBoletoWebhookProcessor(AppDbContext context, ITransactionService transactionService)
    : IWebhookProcessor
{
    private readonly AppDbContext context = context;

    private readonly ITransactionService transactionService = transactionService;

    public PaymentMethod Method => PaymentMethod.FeevBoleto;

    public async Task<object> ProcessAsync(JsonElement payload)
    {
        var eventName = GetEventName(payload);

        switch (eventName)
        {
            case "fatura.cancelada":
            case "boleto.expirado":
                var establishment = await GetEstablishment(payload) ??
                                    throw new BadHttpRequestException("Unable to find the related establishment.");

                var transaction = await GetTransactionFromPayload(establishment, payload, eventName) ??
                                  throw new BadHttpRequestException("Unable to find the related transaction.");

                var result = await transactionService.CancelTransaction(establishment.Id, transaction.Id);

                if (!result)
                    throw new BadHttpRequestException("Transaction not available for cancellation.");

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

    private async Task<Establishment?> GetEstablishment(JsonElement payload)
    {
        if (!payload.TryGetProperty("Banco", out var bankNumber) &&
            !payload.TryGetProperty("banco", out bankNumber))
            throw new BadHttpRequestException("'Banco' (or 'banco') is required.");

        if (!payload.TryGetProperty("Agencia", out var bankAgency) &&
            !payload.TryGetProperty("agencia", out bankAgency))
            throw new BadHttpRequestException("'Agencia' (or 'agencia') is required.");

        if (!payload.TryGetProperty("Conta", out var bankAccount) &&
            !payload.TryGetProperty("conta", out bankAccount))
            throw new BadHttpRequestException("'Conta' (or 'conta') is required.");

        return await context.Establishments.FirstOrDefaultAsync(establishments =>
            establishments.BankNumber == bankNumber.GetString() &&
            establishments.BankAgency == bankAgency.GetString() &&
            establishments.BankAccount == bankAccount.GetString());
    }

    private async Task<Transaction?> GetTransactionFromPayload(
        Establishment establishment,
        JsonElement payload,
        string eventName
    )
    {
        if (eventName == "fatura.cancelada")
            return await GetCancellationEventTransaction(establishment, payload);

        if (eventName == "boleto.expirado")
            return await GetExpirationEventTransaction(establishment, payload);

        return null;
    }

    private async Task<PaymentAttempt?> GetPaymentAttemptFromInvoiceNumber(Establishment establishment,
        int invoiceNumber)
    {
        return await context.PaymentAttempts
            .Include(paymentAttemp => paymentAttemp.Transaction)
            .Where(paymentAttempt => paymentAttempt.EstablishmentId == establishment.Id)
            .Where(paymentAttempt => paymentAttempt.Method == PaymentMethod.FeevBoleto)
            .Where(paymentAttempt => paymentAttempt.Status == PaymentAttemptStatus.Created)
            .Where(paymentAttempt => paymentAttempt.ExternalId == invoiceNumber.ToString())
            .FirstOrDefaultAsync();
    }

    private async Task<Transaction> GetCancellationEventTransaction(Establishment establishment, JsonElement payload)
    {
        if (!payload.TryGetProperty("boletosCancelados", out var cancelledInvoices))
            throw new BadHttpRequestException("'boletosCancelados' is required.");

        if (cancelledInvoices.ValueKind != JsonValueKind.Array)
            throw new BadHttpRequestException("'boletosCancelados' must be an array.");

        if (cancelledInvoices.GetArrayLength() < 1)
            throw new BadHttpRequestException("'boletosCancelados' cannot be empty.");

        var targetInvoice = cancelledInvoices[0];

        if (targetInvoice.ValueKind != JsonValueKind.Object)
            throw new BadHttpRequestException("Each item in 'boletosCancelados' must be an object.");

        if (!targetInvoice.TryGetProperty("NumeroBoleto", out var invoiceNumber))
            throw new BadHttpRequestException("'NumeroBoleto' is required inside 'boletosCancelados[0]'.");

        var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(establishment, invoiceNumber.GetInt32()) ??
                             throw new BadHttpRequestException(
                                 $"No payment attempt found for invoice number '{invoiceNumber}'.");

        return paymentAttempt.Transaction!;
    }

    private async Task<Transaction> GetExpirationEventTransaction(Establishment establishment, JsonElement payload)
    {
        if (!payload.TryGetProperty("numeroboleto", out var invoiceNumber))
            throw new BadHttpRequestException("'numeroboleto' is required.");

        var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(establishment, invoiceNumber.GetInt32()) ??
                             throw new BadHttpRequestException(
                                 $"No payment attempt found for invoice number '{invoiceNumber}'.");

        return paymentAttempt.Transaction!;
    }
}
