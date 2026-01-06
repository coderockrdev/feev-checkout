using System.Text.Json;

using FeevCheckout.Data;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services;

public interface ICredentialService
{
    public Task<Credential> CreateCredential(Guid establishmentId, PaymentMethod method, string data, string? braspagProvider = null);

    public Task<Credential?> GetCredentials(Guid establishmentId, PaymentMethod method);
}

public class CredentialService(AppDbContext context) : ICredentialService
{
    private readonly AppDbContext context = context;

    public async Task<Credential> CreateCredential(Guid establishmentId, PaymentMethod method, string data, string? braspagProvider = null)
    {
        var credential = new Credential
        {
            Id = Guid.NewGuid(),
            EstablishmentId = establishmentId,
            Method = method,
            Data = JsonDocument.Parse(data),
            BraspagProvider = braspagProvider,
        };

        context.Credentials.Add(credential);
        await context.SaveChangesAsync();

        return credential;
    }

    public async Task<Credential?> GetCredentials(Guid establishmentId, PaymentMethod method)
    {
        return await context.Credentials
            .FirstOrDefaultAsync(transaction =>
                transaction.EstablishmentId == establishmentId &&
                transaction.Method == method
            );
    }
}
