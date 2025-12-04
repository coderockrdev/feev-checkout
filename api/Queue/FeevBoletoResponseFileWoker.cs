using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Webhooks.FeevBoleto;

using Flurl;
using Flurl.Http;

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

    private readonly string authBaseUrl = "https://apiseguranca.2safe.com";

    private readonly string boletoBaseUrl = "https://api.fatura.2safe.com";

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

        var token = await Authenticate(payload.Credentials);

        var occurrences = await GetOcurrences(token, payload.Establishment, payload.Batch);

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
        }

        await context.SaveChangesAsync();
    }

    private async Task<string> Authenticate(Credential credentials)
    {
        var token = await $"{authBaseUrl}/api/autenticacao/obtertoken"
            .SetQueryParams(credentials.Data)
            .GetStringAsync();

        return token.Trim();
    }

    private async Task<FeevOcorrencia[]> GetOcurrences(string token, Establishment establishment, string batch)
    {
        var response = await $"{boletoBaseUrl}/api/Fatura/ConsultaArquivoRetorno"
            .WithOAuthBearerToken(token)
            .SetQueryParams(new
            {
                banco = establishment.BankNumber,
                agencia = establishment.BankAgency,
                conta = establishment.BankAccount,
                lote = batch,
                codigoOcorrenciaBancaria = 6
            })
            .GetJsonAsync<FeevFaturaResponse>();

        return response.Ocorrencias;
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
