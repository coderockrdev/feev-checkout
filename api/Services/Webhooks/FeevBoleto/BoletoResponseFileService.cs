using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Queue;

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

        await FeevBoletoResponseFileQueue.Channel.Writer.WriteAsync(new FeevBoletoResponseFileWokerPayload
        {
            Establishment = establishment,
            Credentials = credentials,
            Batch = batch.ToString()
        });
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
}
