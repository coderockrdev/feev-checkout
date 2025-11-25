using System.ComponentModel.DataAnnotations;

using FeevCheckout.Enums;

namespace FeevCheckout.Dtos;

public record PaymentRequest(
    [Required(ErrorMessage = "Method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Method not supported.")]
    PaymentMethod Method,
    int? Installments
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Method == PaymentMethod.FeevBoleto || Method == PaymentMethod.FeevPix)
            if (Installments != null)
                yield return new ValidationResult($"Installments are not allowed for '{Method}'.",
                    [nameof(Installments)]);

        if (Method == PaymentMethod.BraspagCartao)
            if (Installments == null)
                yield return new ValidationResult("Installments are required.",
                    [nameof(Installments)]);
    }
}
