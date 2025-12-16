using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public class BraspagClient(IConfiguration configuration)
{
    private readonly IConfiguration configuration = configuration;

    public IFlurlClient Build(Credential credentials)
    {
        var baseUrl = configuration["AppSettings:Braspag:BaseUrl"]
                      ?? throw new InvalidOperationException("Braspag base URL not found or not specified.");

        return new FlurlClient(baseUrl)
            .WithHeaders(new
            {
                Accept = "application/json",
                ContentType = "application/json"
            })
            .WithHeaders(credentials.Data);
    }
}
