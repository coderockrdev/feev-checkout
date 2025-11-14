using System.ComponentModel.DataAnnotations;

using FeevCheckout.Enums;

namespace FeevCheckout.Dtos;

public record PaymentRequest(
    [Required(ErrorMessage = "Method is required.")]
    [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Method not supported.")]
    PaymentMethod Method,
    [Required(ErrorMessage = "Installments is required.")]
    int Installments
);
