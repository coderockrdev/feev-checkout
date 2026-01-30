using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class InstallmentDto
{
    public required int Number { get; set; }

    public DateOnly? DueAt { get; set; }

    public DateOnly? ExpireAt { get; set; }

    public int? Fee { get; set; }

    public string? FeeType { get; set; }

    public required int InstallmentValue { get; set; }

    public required int FinalAmount { get; set; }

    public static InstallmentDto FromModel(Installment model)
    {
        return new InstallmentDto
        {
            Number = model.Number,
            DueAt = model.DueAt,
            ExpireAt = model.ExpireAt,
            Fee = model.Fee,
            FeeType = model.FeeType,
            InstallmentValue = model.InstallmentValue,
            FinalAmount = model.FinalAmount
        };
    }
}
