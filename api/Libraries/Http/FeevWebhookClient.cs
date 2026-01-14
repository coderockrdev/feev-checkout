using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public interface IFeevWebhookClient
{
    IFlurlRequest CreateRequest(string path);
}

public class FeevWebhookClient(IConfiguration configuration, IFeevClient feevClient) : IFeevWebhookClient
{
    private readonly string baseUrl = configuration["AppSettings:FeevWebhook:BaseUrl"]
                                      ?? throw new InvalidOperationException(
                                          "Feev Webhook base URL not found or not specified.");

    private readonly IFeevClient feevClient = feevClient;

    public IFlurlRequest CreateRequest(string path)
    {
        return new FlurlRequest(baseUrl)
            .AppendPathSegment(path)
            .WithTimeout(30)
            .WithHeaders(new
            {
                Accept = "application/json"
            });
    }
}
