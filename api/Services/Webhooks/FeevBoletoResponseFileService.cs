using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Queue;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services.Webhooks;

public interface IFeevBoletoResponseFileService
{
    Task Handle(string _, JsonElement payload);
}

public class FeevBoletoResponseFileService(AppDbContext context, ICredentialService credentialService)
    : IFeevBoletoResponseFileService
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

        await FeevBoletoResponseFileQueue.Channel.Writer.WriteAsync(new FeevBoletoResponseFileWorkerPayload
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
