using System.Text.Json;

namespace FeevCheckout.Utils;

public class HttpUtils
{
    public static IDictionary<string, string?> JsonToObject(JsonDocument json)
    {
        return json.RootElement
            .EnumerateObject()
            .ToDictionary(
                property => property.Name,
                property => property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString()
                    : property.Value.ToString()
            );
    }
}
