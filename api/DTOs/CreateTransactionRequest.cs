using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Dtos;

public record ProductDto(
    [Required(ErrorMessage = "Product name is required.")]
    string Name,
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    int Price
);

public record AddressDto(
    [Required(ErrorMessage = "Customer street is required.")]
    string Street,
    [Required(ErrorMessage = "Customer city is required.")]
    string City,
    [Required(ErrorMessage = "Customer UF is required.")]
    string UF,
    [Required(ErrorMessage = "Customer postal code is required.")]
    string PostalCode
);

public record CustomerDto(
    [Required(ErrorMessage = "Customer name is required.")]
    string Name,
    [Required(ErrorMessage = "Customer document is required.")]
    string Document,
    [Required(ErrorMessage = "Customer e-mail is required.")]
    string Email,
    [Required(ErrorMessage = "Customer address is required.")]
    AddressDto Address
);

public record InstallmentDto(
    int Number,
    int? Fee,
    string? FeeType
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Number <= 0)
            yield return new ValidationResult("Number must be greater than 0.", [nameof(Number)]);

        if (Fee.HasValue)
        {
            // Ensure fee > 0 if is sent
            if (Fee <= 0)
                yield return new ValidationResult("Fee must be greater than 0.", [nameof(Fee)]);

            // Ensure feeType is sent if fee is sent
            // Ensure feeType is "percentage" or "amount" if sent
            if (string.IsNullOrEmpty(FeeType) || FeeType is not ("percentage" or "amount"))
                yield return new ValidationResult("FeeType must be 'percentage' or 'amount'.", [nameof(FeeType)]);
        }
    }
}

public record PaymentRuleDto(
    string Type,
    List<InstallmentDto> Installments,
    DateOnly? FirstInstallment,
    int? Interest,
    int? LateFee
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type is not ("braspag_cartao" or "feev_pix" or "feev_boleto"))
            yield return new ValidationResult("Type must be 'braspag_cartao', 'feev_pix' or 'feev_boleto'.",
                [nameof(Type)]);

        if (Type == "feev_boleto")
        {
            if (FirstInstallment == null)
                yield return new ValidationResult("FirstInstallment is required for 'feev_boleto'.",
                    [nameof(FirstInstallment)]);

            if (FirstInstallment.HasValue && FirstInstallment.Value <= DateOnly.FromDateTime(DateTime.Now))
                yield return new ValidationResult("FirstInstallment must be a future date.",
                    [nameof(FirstInstallment)]);

            // Ensure interest > 0 if sent
            if (Interest.HasValue && Interest <= 0)
                yield return new ValidationResult("Interest must be greater than 0.", [nameof(Interest)]);

            // Ensure lateFee > 0 if sent
            if (LateFee.HasValue && LateFee <= 0)
                yield return new ValidationResult("LateFee must be greater than 0.", [nameof(Interest)]);
        }
    }
}

public record CreateTransactionRequest(
    [Required] string Description,
    [Required] [MinLength(1)] List<ProductDto> Products,
    [Required] CustomerDto Customer,
    [Required] [MinLength(1)] List<PaymentRuleDto> PaymentRules
);
