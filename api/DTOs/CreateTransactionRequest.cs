using System.ComponentModel.DataAnnotations;

using FeevCheckout.Enums;

namespace FeevCheckout.DTOs;

public record ProductDto(
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 255 characters.")]
    string Name,
    [Required(ErrorMessage = "Product price is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Product price must be greater than 0.")]
    int Price
);

public record AddressDto(
    [Required(ErrorMessage = "Customer zip code is required.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Customer zip code must have exactly 8 characters.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Customer zip code must contain only digits.")]
    string ZipCode,
    [Required(ErrorMessage = "Customer state is required.")]
    [RegularExpression(@"^[A-Z]{2}$",
        ErrorMessage = "Customer state must be exactly two uppercase letters (e.g. RS, SP, RJ).")]
    string State,
    [Required(ErrorMessage = "Customer city is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Customer city must be between 3 and 50 characters.")]
    string City,
    [Required(ErrorMessage = "Customer district is required.")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Customer district must be between 1 and 255 characters.")]
    string District,
    [Required(ErrorMessage = "Customer street is required.")]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "Customer street must be between 5 and 255 characters.")]
    string Street,
    [Required(ErrorMessage = "Customer number is required.")]
    [StringLength(15, MinimumLength = 1, ErrorMessage = "Customer number must be between 1 and 15 characters.")]
    string Number,
    [StringLength(50, ErrorMessage = "Customer complement must be less then 50 characters.")]
    string? Complement
);

public record CustomerDto(
    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "Customer name must be between 10 and 255 characters.")]
    string Name,
    [Required(ErrorMessage = "Customer document is required.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Customer document must contain only digits.")]
    [StringLength(14, MinimumLength = 11, ErrorMessage = "Customer document must be between 11 and 14 characters.")]
    string Document,
    [Required(ErrorMessage = "Customer e-mail is required.")]
    [EmailAddress(ErrorMessage = "Customer e-mail must be a e-mail address.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "Customer e-mail must be between 10 and 255 characters.")]
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
    [StringLength(50, MinimumLength = 10, ErrorMessage = "Identifier must be between 10 and 50 characters.")]
    string Identifier,
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 255 characters.")]
    string Description,
    [Required(ErrorMessage = "Products is required.")]
    [MinLength(1)]
    List<ProductDto> Products,
    [Required(ErrorMessage = "Customer is required.")]
    CustomerDto Customer,
    [Required(ErrorMessage = "Payment rules is required.")]
    [MinLength(1)]
    List<PaymentRuleDto> PaymentRules,
    DateTime? ExpireAt,
    [Required(ErrorMessage = "Callback URL is required.")]
    [Url(ErrorMessage = "Callback URL must be a valid URL")]
    string CallbackUrl
);
