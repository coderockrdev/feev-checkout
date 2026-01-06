namespace FeevCheckout.Models;

public class CardBrandPattern
{
    public required Guid Id { get; set; }

    public required string Brand { get; set; }

    public required string Prefix { get; set; }

    public required int Order { get; set; }
}
