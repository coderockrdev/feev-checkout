using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services.Webhooks;

public class FeevBoletoWebhookProcessor(AppDbContext context, ITransactionService transactionService)
    : IWebhookProcessor
{
    private readonly AppDbContext _context = context;

    private readonly ITransactionService _transactionService = transactionService;

    public PaymentMethod Method => PaymentMethod.FeevBoleto;

    public async Task<object> ProcessAsync(JsonElement payload)
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

        var transaction = await GetTransactionFromPayload(payload, eventName) ??
                          throw new BadHttpRequestException("Unable to find the related transaction.");

        var result = await _transactionService.CancelTransaction(transaction.EstablishmentId, transaction.Id);

        if (!result)
            throw new BadHttpRequestException("Transaction not available for cancellation.");

        return new
        {
            Status = "processed"
        };
    }

    private async Task<Transaction?> GetTransactionFromPayload(JsonElement payload, string eventName)
    {
        if (eventName == "fatura.cancelada")
            return await GetCancellationEventTransaction(payload);

        if (eventName == "boleto.expirado")
            return await GetExpirationEventTransaction(payload);

        return null;
    }

    private async Task<PaymentAttempt?> GetPaymentAttemptFromInvoiceNumber(int invoiceNumber)
    {
        return await _context.PaymentAttempts
            .Include(paymentAttemp => paymentAttemp.Transaction)
            .Where(paymentAttempt => paymentAttempt.Method == PaymentMethod.FeevBoleto)
            .Where(paymentAttempt => paymentAttempt.Status == PaymentAttemptStatus.Completed)
            .Where(paymentAttempt =>
                paymentAttempt.Response != null && EF.Functions.JsonContains(
                    paymentAttempt.Response,
                    $$""" { "Boletos": [ { "NumeroBoleto": {{invoiceNumber}} } ] } """
                )
            )
            .FirstOrDefaultAsync();
    }

    private async Task<Transaction> GetCancellationEventTransaction(JsonElement payload)
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

        var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(invoiceNumber.GetInt32()) ??
                             throw new BadHttpRequestException(
                                 $"No payment attempt found for invoice number '{invoiceNumber}'.");

        return paymentAttempt.Transaction!;
    }

    private async Task<Transaction> GetExpirationEventTransaction(JsonElement payload)
    {
        if (!payload.TryGetProperty("numeroboleto", out var invoiceNumber))
            throw new BadHttpRequestException("'numeroboleto' is required.");

        var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(invoiceNumber.GetInt32()) ??
                             throw new BadHttpRequestException(
                                 $"No payment attempt found for invoice number '{invoiceNumber}'.");

        return paymentAttempt.Transaction!;
    }
}
