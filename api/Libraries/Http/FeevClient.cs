using FeevCheckout.Models;

using Flurl;
using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public interface IFeevClient
{
    Task<string> Authenticate(Credential credentials);
}

public class FeevClient(IConfiguration configuration) : IFeevClient
{
    private readonly string baseUrl = configuration["AppSettings:Feev:BaseUrl"]
                                      ?? throw new InvalidOperationException(
                                          "Feev base URL not found or not specified.");

    public async Task<string> Authenticate(Credential credentials)
    {
        var token = await $"{baseUrl}/api/autenticacao/obtertoken"
            .SetQueryParams(credentials.Data)
            .GetStringAsync();

        return token.Trim();
    }
}
