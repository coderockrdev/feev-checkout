using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class AddressDto
{
    public required string Street { get; set; }

    public required string Number { get; set; }

    public string? Complement { get; set; }

    public required string District { get; set; }

    public required string City { get; set; }

    public required string State { get; set; }

    public required string ZipCode { get; set; }

    public static AddressDto FromModel(Address model)
    {
        return new AddressDto
        {
            Street = model.Street,
            Number = model.Number,
            Complement = model.Complement,
            District = model.District,
            City = model.City,
            State = model.State,
            ZipCode = model.ZipCode
        };
    }
}
