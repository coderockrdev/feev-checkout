namespace FeevCheckout.Models;

public class Establishment
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Username { get; set; }

    public required string Secret { get; set; }
}
