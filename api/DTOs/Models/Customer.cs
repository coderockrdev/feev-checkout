using FeevCheckout.Models;
using FeevCheckout.Utils;

namespace FeevCheckout.DTOs.Models;

public class CustomerDto
{
    public required string Name { get; set; }

    public required string Document { get; set; }

    public required string Email { get; set; }

    public static CustomerDto FromModel(Customer model)
    {
        return new CustomerDto
        {
            Name = model.Name,
            Document = TextUtils.MaskDocumentNumber(model.Document),
            Email = model.Email,
        };
    }
}
