using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Events;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;
using FeevCheckout.Services;
using FeevCheckout.Services.Payments;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Queue;

public class FeevBoletoResponseFileWorkerPayload
{
    public required Establishment Establishment { get; set; }

    public required Credential Credentials { get; set; }

    public required string Batch { get; set; }
}

public class FeevBoletoResponseFileWorker(IServiceProvider serviceProvider) : BackgroundService
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

    private async Task Handle(FeevBoletoResponseFileWorkerPayload payload)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FeevBoletoResponseFileWorker>>();

        var occurrences = await GetOcurrences(payload.Establishment, payload.Credentials, payload.Batch);

        foreach (var occurrence in occurrences)
        {
            var invoiceNumber = occurrence.NumeroBoleto;

            var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(
                context,
                payload.Establishment,
                invoiceNumber
            );

            if (paymentAttempt == null)
            {
                logger.LogWarning($"No payment attempt related to {invoiceNumber} invoice.");
                continue;
            }

            var transaction = paymentAttempt.Transaction ??
                              throw new BadHttpRequestException("Unable to find the related transaction.");

            await transactionService.CompleteTransaction(transaction, paymentAttempt);
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
            .Where(paymentAttempt => paymentAttempt.Status == PaymentAttemptStatus.Pending)
            .Where(paymentAttempt => paymentAttempt.ExternalId == invoiceNumber.ToString())
            .FirstOrDefaultAsync();
    }
}
