using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public interface IFeevBoletoClient
{
    Task<IFlurlRequest> CreateRequest(Credential credentials, string path);
}

public class FeevBoletoClient(IConfiguration configuration, IFeevClient feevClient) : IFeevBoletoClient
{
    private readonly string baseUrl = configuration["AppSettings:FeevBoleto:BaseUrl"]
                                      ?? throw new InvalidOperationException(
                                          "Feev Boleto base URL not found or not specified.");

    private readonly IFeevClient feevClient = feevClient;

    public async Task<IFlurlRequest> CreateRequest(Credential credentials, string path)
    {
        var token = await feevClient.Authenticate(credentials);

        return new FlurlRequest(baseUrl)
            .AppendPathSegment(path)
            .WithTimeout(30)
            .WithHeaders(new
            {
                Accept = "application/json"
            })
            .WithOAuthBearerToken(token);
    }
}
