using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Flurl;
using Flurl.Http;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services.Webhooks.FeevBoleto;

public class FeevOcorrencia
{
    public required string Carteira { get; set; }

    public required int NumeroBoleto { get; set; }

    public required int CodigoOcorrenciaBancaria { get; set; }

    public required string DescricaoOcorrencia { get; set; }

    public required string DataOcorrencia { get; set; }

    public required string NossoNumero { get; set; }

    public required double ValorPago { get; set; }

    public required string DataCredito { get; set; }

    public required string DataHoraImportacao { get; set; }

    public required string DataHoraProcessamento { get; set; }

    public required string CodigoInconsistencia { get; set; }

    public required string BancoCobrador { get; set; }

    public required string AgenciaCobradora { get; set; }

    public required string DescricaoInconsistencia { get; set; }
}

public class FeevFaturaResponse
{
    public required string Banco { get; set; }

    public required string Agencia { get; set; }

    public required string Conta { get; set; }

    public required string DataGeracaoArquivo { get; set; }

    public required string Lote { get; set; }

    public required string Carteira { get; set; }

    public required string LinkArquivoRetorno { get; set; }

    public required string NomeArquivoRetorno { get; set; }

    public required string Situacao { get; set; }

    public required FeevOcorrencia[] Ocorrencias { get; set; }
}

public class BoletoResponseFileService(AppDbContext context, ICredentialService credentialService)
{
    // private readonly string authBaseUrl = configuration["AppSettings:Feev:AuthBaseUrl"]
    //     ?? throw new InvalidOperationException(
    //         "Feev auth base URL not found or not specified.");

    // private readonly string boletoBaseUrl = configuration["AppSettings:Feev:BoletoBaseUrl"]
    //                                         ?? throw new InvalidOperationException(
    //                                             "Feev Boleto base URL not found or not specified.");

    private readonly string authBaseUrl = "https://apiseguranca.2safe.com";

    private readonly string boletoBaseUrl = "https://api.fatura.2safe.com";
    private readonly AppDbContext context = context;

    private readonly ICredentialService credentialService = credentialService;

    public async Task Handle(string _, JsonElement payload)
    {
        var establishment = await GetEstablishmentFromPayload(payload) ??
                            throw new BadHttpRequestException("Unable to find the related establishment.");

        var credentials = await credentialService.GetCredentials(establishment.Id, PaymentMethod.FeevBoleto) ??
                          throw new InvalidOperationException(
                              $"No credentials registered for '{PaymentMethod.FeevBoleto}'.");

        if (!payload.TryGetProperty("Lote", out var batch) &&
            !payload.TryGetProperty("lote", out batch))
            throw new BadHttpRequestException("'Lote' (or 'lote') is required.");

        var token = await Authenticate(credentials);

        var occurrences = await GetOcurrences(token, establishment, batch.GetInt32());

        foreach (var occurrence in occurrences)
        {
            var invoiceNumber = occurrence.NumeroBoleto;

            var paymentAttempt = await GetPaymentAttemptFromInvoiceNumber(establishment, invoiceNumber);

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

    private async Task<Establishment?> GetEstablishmentFromPayload(JsonElement payload)
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

    private async Task<string> Authenticate(Credential credentials)
    {
        var token = await $"{authBaseUrl}/api/autenticacao/obtertoken"
            .SetQueryParams(credentials.Data)
            .GetStringAsync();

        return token.Trim();
    }

    private async Task<FeevOcorrencia[]> GetOcurrences(string token, Establishment establishment, int batch)
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
}
