using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public interface IFeevPixClient
{
    Task<IFlurlRequest> CreateRequest(Credential credentials, string path);
}

public class FeevPixClient(IConfiguration configuration, IFeevClient feevClient) : IFeevPixClient
{
    private readonly string baseUrl = configuration["AppSettings:FeevPix:BaseUrl"]
                                      ?? throw new InvalidOperationException(
                                          "Feev Pix base URL not found or not specified.");

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
