using System.Security.Cryptography;
using System.Text;

namespace FeevCheckout.Utils;

public static class HmacValidator
{
    public static string Compute(string input, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

        return Convert.ToBase64String(hashBytes);
    }

    public static bool Validate(string actualHash, string expectedHash)
    {
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(actualHash),
            Convert.FromBase64String(expectedHash)
        );
    }
}
