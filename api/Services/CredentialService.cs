using FeevCheckout.Data;
using FeevCheckout.Models;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Services;

public interface ICredentialService
{
    public Task<Credential?> GetCredentials(Guid establishmentId, string method);
}

public class CredentialService(AppDbContext context) : ICredentialService
{
    private readonly AppDbContext _context = context;

    public async Task<Credential?> GetCredentials(Guid establishmentId, string method)
    {
        return await _context.Credentials
            .FirstOrDefaultAsync(transaction =>
                transaction.EstablishmentId == establishmentId &&
                transaction.Method == method
            );
    }
}
