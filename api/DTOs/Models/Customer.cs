using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public class CustomerDto
{
    public required string Name { get; set; }

    public required string Document { get; set; }

    public required string Email { get; set; }

    public required AddressDto Address { get; set; }

    public static CustomerDto FromModel(Customer model)
    {
        return new CustomerDto
        {
            Name = model.Name,
            Document = model.Document,
            Email = model.Email,
            Address = AddressDto.FromModel(model.Address)
        };
    }
}
