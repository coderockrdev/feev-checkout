using FeevCheckout.Enums;
using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class PaymentRuleDto
{
    public required PaymentMethod Method { get; set; }

    public required List<InstallmentDto> Installments { get; set; }

    public DateOnly? FirstInstallment { get; set; }

    public int? Interest { get; set; }

    public int? LateFee { get; set; }

    public static PaymentRuleDto FromModel(PaymentRule model)
    {
        return new PaymentRuleDto
        {
            Method = model.Method,
            FirstInstallment = model.FirstInstallment,
            Interest = model.Interest,
            LateFee = model.LateFee,
            Installments = [.. model.Installments.Select(InstallmentDto.FromModel)]
        };
    }
}
