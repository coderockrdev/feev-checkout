using System.ComponentModel.DataAnnotations;

using FeevCheckout.Enums;

namespace FeevCheckout.Dtos;

public record ProductDto(
    [Required(ErrorMessage = "Product name is required.")]
    string Name,
    [Range(1, int.MaxValue, ErrorMessage = "Product price must be greater than 0.")]
    int Price
);

public record AddressDto(
    [Required(ErrorMessage = "Customer street is required.")]
    string Street,
    [Required(ErrorMessage = "Customer address number is required.")]
    string Number,
    string? Complement,
    [Required(ErrorMessage = "Customer neighborhood is required.")]
    string Neighborhood,
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
    [Required(ErrorMessage = "Installment number is required.")]
    int Number,
    DateOnly? DueAt,
    DateOnly? ExpireAt,
    int? Fee,
    string? FeeType
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Number <= 0)
            yield return new ValidationResult("Installment number must be greater than 0.", [nameof(Number)]);

        if (Fee.HasValue)
        {
            // Ensure fee > 0 if is sent
            if (Fee <= 0)
                yield return new ValidationResult("Installment Fee must be greater than 0.", [nameof(Fee)]);

            // Ensure feeType is sent if fee is sent
            // Ensure feeType is "percentage" or "amount" if sent
            if (string.IsNullOrEmpty(FeeType) || FeeType is not ("percentage" or "amount"))
                yield return new ValidationResult("Installment fee type must be 'percentage' or 'amount'.",
                    [nameof(FeeType)]);
        }
    }
}

public record PaymentRuleDto(
    [Required(ErrorMessage = "Payment rule method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Payment rule method not supported.")]
    PaymentMethod Method,
    [Required(ErrorMessage = "Payment rule installments is required.")]
    List<InstallmentDto> Installments,
    DateOnly? FirstInstallment,
    int? Interest,
    int? LateFee
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Method == PaymentMethod.FeevBoleto)
        {
            if (Installments.Count > 1)
                yield return new ValidationResult("Payment installments cannot be greater than 1.",
                    [nameof(Installments)]);

            foreach (var installment in Installments.Select((value, index) => new { value, index }))
            {
                if (installment.value.DueAt == null)
                    yield return new ValidationResult(
                        $"Payment rule installment[{installment.index}] due at is required.",
                        [nameof(installment.value.DueAt)]);

                if (installment.value.DueAt.HasValue &&
                    installment.value.DueAt.Value <= DateOnly.FromDateTime(DateTime.Now))
                    yield return new ValidationResult(
                        $"Payment rule installment[{installment.index}] due at must be a future date.",
                        [nameof(installment.value.DueAt)]);

                if (installment.value.ExpireAt == null)
                    yield return new ValidationResult(
                        $"Payment rule installment[{installment.index}] expire at is required for boleto.",
                        [nameof(installment.value.ExpireAt)]);

                if (installment.value.ExpireAt.HasValue &&
                    installment.value.ExpireAt.Value <= DateOnly.FromDateTime(DateTime.Now))
                    yield return new ValidationResult(
                        $"Payment rule installment[{installment.index}] expire at must be a future date.",
                        [nameof(installment.value.ExpireAt)]);
            }

            if (FirstInstallment == null)
                yield return new ValidationResult("Payment rule first installemnt is required.",
                    [nameof(FirstInstallment)]);

            if (FirstInstallment.HasValue && FirstInstallment.Value <= DateOnly.FromDateTime(DateTime.Now))
                yield return new ValidationResult("Payment rule first installemnt must be a future date.",
                    [nameof(FirstInstallment)]);

            // Ensure interest > 0 if sent
            if (Interest.HasValue && Interest <= 0)
                yield return new ValidationResult("Payment rule interest must be greater than 0.", [nameof(Interest)]);

            // Ensure lateFee > 0 if sent
            if (LateFee.HasValue && LateFee <= 0)
                yield return new ValidationResult("Payment rule late fee must be greater than 0.", [nameof(LateFee)]);
        }

        if (Method == PaymentMethod.FeevPix)
            if (Installments.Count > 1)
                yield return new ValidationResult("Payment installments cannot be greater than 1.",
                    [nameof(Installments)]);
    }
}

public record CreateTransactionRequest(
    [Required(ErrorMessage = "Identifier is required.")]
    string Identifier,
    [Required(ErrorMessage = "Description is required.")]
    string Description,
    [Required(ErrorMessage = "Products is required.")]
    [MinLength(1)]
    List<ProductDto> Products,
    [Required(ErrorMessage = "Customer is required.")]
    CustomerDto Customer,
    [Required(ErrorMessage = "Payment rules is required.")]
    [MinLength(1)]
    List<PaymentRuleDto> PaymentRules,
    DateTime? ExpireAt
);
