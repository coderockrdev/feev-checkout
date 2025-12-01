namespace FeevCheckout.Models;

public class Establishment
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }
}
