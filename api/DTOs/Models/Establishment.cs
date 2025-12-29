using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class EstablishmentDto
{
    public required Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string ShortName { get; set; }

    public required string CNPJ { get; set; }

    public static EstablishmentDto FromModel(Establishment model)
    {
        return new EstablishmentDto
        {
            Id = model.Id,
            FullName = model.FullName,
            ShortName = model.ShortName,
            CNPJ = model.CNPJ
        };
    }
}
