using FeevCheckout.Data;
using FeevCheckout.Events;
using FeevCheckout.Enums;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;
using FeevCheckout.Services;
using FeevCheckout.Services.Payments;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Queue;

public class FeevBoletoResponseFileWokerPayload
{
    public required Establishment Establishment { get; set; }

    public required Credential Credentials { get; set; }

    public required string Batch { get; set; }
}

public class FeevBoletoResponseFileWoker(IServiceProvider serviceProvider) : BackgroundService
{
    // private readonly string authBaseUrl = configuration["AppSettings:Feev:AuthBaseUrl"]
    //     ?? throw new InvalidOperationException(
    //         "Feev auth base URL not found or not specified.");

    // private readonly string boletoBaseUrl = configuration["AppSettings:Feev:BoletoBaseUrl"]
    //                                         ?? throw new InvalidOperationException(
    //                                             "Feev Boleto base URL not found or not specified.");

    private readonly IServiceProvider serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = FeevBoletoResponseFileQueue.Channel.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        while (reader.TryRead(out var payload))
            await Handle(payload);
    }

    private async Task Handle(FeevBoletoResponseFileWokerPayload payload)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ITransactionWebhookDispatcherService>();

        var occurrences = await GetOcurrences(payload.Establishment, payload.Credentials, payload.Batch);

        foreach (var occurrence in occurrences)
        {
            var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(
                context,
                payload.Establishment,
                occurrence.NumeroBoleto
            );

            // TODO: should we log it or something?
            if (paymentAttempt == null)
                continue;

            var transaction = paymentAttempt.Transaction ??
                              throw new BadHttpRequestException("Unable to find the related transaction.");

            paymentAttempt.Status = PaymentAttemptStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;

            await dispatcher.DispatchAsync(
                TransactionWebhookEvent.Completed,
                transaction
            );
        }

        await context.SaveChangesAsync();
    }

    private async Task<Ocorrencia[]> GetOcurrences(Establishment establishment, Credential credentials, string batch)
    {
        using var scope = serviceProvider.CreateScope();
        var feevBoletoService = scope.ServiceProvider.GetRequiredService<IFeevBoletoService>();

        var responseFile = await feevBoletoService.GetResponseFile(establishment, credentials, batch);

        return responseFile.Ocorrencias;
    }

    private static async Task<PaymentAttempt?> GetPaymentAttemptFromInvoiceNumber(
        AppDbContext context,
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
}
