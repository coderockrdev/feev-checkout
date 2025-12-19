using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services.Webhooks;

public interface IFeevBoletoCancellationService
{
    Task Handle(string eventName, JsonElement payload);
}

public class FeevBoletoCancellationService(AppDbContext context, ITransactionService transactionService)
    : IFeevBoletoCancellationService
{
    private readonly AppDbContext context = context;

    private readonly ITransactionService transactionService = transactionService;

    public async Task Handle(string eventName, JsonElement payload)
    {
        var establishment = await GetEstablishment(payload) ??
                            throw new BadHttpRequestException("Unable to find the related establishment.");

        var transaction = await GetTransaction(establishment, payload, eventName) ??
                          throw new BadHttpRequestException("Unable to find the related transaction.");

        var result = await transactionService.CancelTransaction(establishment.Id, transaction.Id);

        if (!result)
            throw new BadHttpRequestException("Transaction not available for cancellation.");
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

    private async Task<PaymentAttempt?> GetPaymentAttemptFromInvoiceNumber(
        Establishment establishment,
        int invoiceNumber
    )
    {
        return await context.PaymentAttempts
            .Include(paymentAttemp => paymentAttemp.Transaction)
            .Where(paymentAttempt => paymentAttempt.EstablishmentId == establishment.Id)
            .Where(paymentAttempt => paymentAttempt.Method == PaymentMethod.FeevBoleto)
            .Where(paymentAttempt => paymentAttempt.Status == PaymentAttemptStatus.Created)
            .Where(paymentAttempt => paymentAttempt.ExternalId == invoiceNumber.ToString())
            .FirstOrDefaultAsync();
    }

    private async Task<Transaction> GetCancelEventTransaction(Establishment establishment, JsonElement payload)
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

    private async Task<Transaction> GetExpiredEventTransaction(Establishment establishment, JsonElement payload)
    {
        if (!payload.TryGetProperty("numeroboleto", out var invoiceNumber))
            throw new BadHttpRequestException("'numeroboleto' is required.");

        var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(establishment, invoiceNumber.GetInt32()) ??
                             throw new BadHttpRequestException(
                                 $"No payment attempt found for invoice number '{invoiceNumber}'.");

        return paymentAttempt.Transaction!;
    }

    private async Task<Transaction?> GetTransaction(
        Establishment establishment,
        JsonElement payload,
        string eventName
    )
    {
        if (eventName == "fatura.cancelada")
            return await GetCancelEventTransaction(establishment, payload);

        if (eventName == "boleto.expirado")
            return await GetExpiredEventTransaction(establishment, payload);

        return null;
    }
}
