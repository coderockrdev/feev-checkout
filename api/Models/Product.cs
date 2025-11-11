namespace FeevCheckout.Models;

public class Product
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required int Price { get; set; }

    public required Guid TransactionId { get; set; }

    public Transaction Transaction { get; set; } = default!;
}
