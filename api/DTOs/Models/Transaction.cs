using FeevCheckout.Enums;
using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Models;

public sealed class TransactionDto
{
    public required Guid Id { get; set; }

    public required string Identifier { get; set; }

    public required string Description { get; set; }

    public required EstablishmentDto Establishment { get; set; }

    public required CustomerDto Customer { get; set; }

    public required int TotalAmount { get; set; }

    public required List<ProductDto> Products { get; set; }

    public required List<PaymentRuleDto> PaymentRules { get; set; }

    public required TransactionStatus Status { get; set; }

    public Guid? SuccessfulPaymentAttemptId { get; set; }

    public DateTime? ExpireAt { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }


    public static TransactionDto FromModel(Transaction model)
    {
        return new TransactionDto
        {
            Id = model.Id,
            Identifier = model.Identifier,
            Description = model.Description,
            Establishment = EstablishmentDto.FromModel(model.Establishment!),
            Customer = CustomerDto.FromModel(model.Customer),
            TotalAmount = model.TotalAmount,
            Products = [.. model.Products.Select(ProductDto.FromModel)],
            PaymentRules = [.. model.PaymentRules.Select(PaymentRuleDto.FromModel)],
            Status = model.Status,
            SuccessfulPaymentAttemptId = model.SuccessfulPaymentAttemptId,
            ExpireAt = model.ExpireAt,
            CanceledAt = model.CanceledAt,
            CompletedAt = model.CompletedAt,
            CreatedAt = model.CreatedAt
        };
    }
}
