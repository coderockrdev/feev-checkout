using System.ComponentModel.DataAnnotations;

using FeevCheckout.Enums;

namespace FeevCheckout.Dtos;

public record CardDto(
    [Required(ErrorMessage = "Card holder is required.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Card holder must be between 6 and 255 characters.")]
    string Holder,
    [Required(ErrorMessage = "Card number is required.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Card number must contain only digits.")]
    [StringLength(19, MinimumLength = 10, ErrorMessage = "Card number must be between 10 and 19 characters.")]
    string Number,
    [Required(ErrorMessage = "Due at is required.")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{4}$", ErrorMessage = "Due at must be in MM/YYYY format.")]
    string DueAt,
    [Required(ErrorMessage = "Security code is required.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Security code must contain only digits.")]
    string SecurityCode
);

public record PaymentRequestDto(
    [Required(ErrorMessage = "Method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Method not supported.")]
    PaymentMethod Method,
    int? Installments,
    CardDto? Card
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Method == PaymentMethod.FeevBoleto || Method == PaymentMethod.FeevPix)
            if (Installments != null)
                yield return new ValidationResult($"Installments are not allowed for '{Method}'.",
                    [nameof(Installments)]);

        if (Method == PaymentMethod.BraspagCartao)
        {
            if (Installments == null)
                yield return new ValidationResult("Installments is required.",
                    [nameof(Installments)]);

            if (Card == null)
                yield return new ValidationResult("Card is required.",
                    [nameof(Card)]);

            if (Card != null && Card.Number != "0000000000000001")
            {
                var digits = Card.Number.Select(_char => _char - '0').ToArray();
                var result = digits.Select((digit, index) =>
                    index % 2 == digits.Length % 2 ? 2 * digit % 10 + digit / 5 : digit).Sum() % 10;

                if (result != 0)
                    yield return new ValidationResult("Card number is not valid.",
                        [nameof(Card)]);
            }
        }
    }
}
