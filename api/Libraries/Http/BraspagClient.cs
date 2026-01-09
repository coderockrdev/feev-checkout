using FeevCheckout.Models;
using FeevCheckout.Utils;

using Flurl.Http;

namespace FeevCheckout.Libraries.Http;

public interface IBraspagClient
{
    IFlurlRequest CreateRequest(Credential credentials, string path);
}

public class BraspagClient(IConfiguration configuration) : IBraspagClient
{
    private readonly string baseUrl = configuration["AppSettings:Braspag:BaseUrl"]
                                      ?? throw new InvalidOperationException(
                                          "Braspag base URL not found or not specified.");

    public IFlurlRequest CreateRequest(Credential credentials, string path)
    {
        return new FlurlRequest(baseUrl)
            .AppendPathSegment(path)
            .WithTimeout(30)
            .WithHeaders(new
            {
                Accept = "application/json",
                ContentType = "application/json"
            })
            .WithHeaders(HttpUtils.JsonToObject(credentials.Data));
    }
}
