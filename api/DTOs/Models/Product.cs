using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class ProductDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required int Price { get; set; }

    public static ProductDto FromModel(Product model)
    {
        return new ProductDto
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price
        };
    }
}
