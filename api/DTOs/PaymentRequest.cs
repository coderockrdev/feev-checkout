using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Dtos;

public record PaymentRequest(
    [Required] string Method,
    [Required] int Installments
);
