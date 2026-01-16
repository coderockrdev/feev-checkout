using System.ComponentModel.DataAnnotations.Schema;

using FeevCheckout.Enums;

namespace FeevCheckout.Models;

public class Address
{
    public required string Street { get; set; }

    public required string Number { get; set; }

    public string? Complement { get; set; } = null;

    public required string District { get; set; }

    public required string City { get; set; }

    public required string State { get; set; }

    public required string ZipCode { get; set; }
}

public class Customer
{
    public required string Name { get; set; }

    public required string Document { get; set; }

    public required string Email { get; set; }

    public required Address Address { get; set; }
}

public class Installment
{
    public required int Number { get; set; }

    public DateOnly? DueAt { get; set; }

    public DateOnly? ExpireAt { get; set; }

    public int? Fee { get; set; }

    public string? FeeType { get; set; }

    public int FinalAmount { get; set; }
}

public class PaymentRule
{
    public required PaymentMethod Method { get; set; }

    public required List<Installment> Installments { get; set; }

    public DateOnly? FirstInstallment { get; set; }

    public int? Interest { get; set; }

    public int? LateFee { get; set; }
}

public class Transaction
{
    public required Guid Id { get; set; }

    public required Guid EstablishmentId { get; set; }

    public Establishment? Establishment { get; set; }

    public required string Identifier { get; set; }

    public required string Description { get; set; }

    public required Customer Customer { get; set; }

    public required int TotalAmount { get; set; }

    public List<Product> Products { get; set; } = [];

    public required List<PaymentRule> PaymentRules { get; set; }

    public Guid? SuccessfulPaymentAttemptId { get; set; }

    public PaymentAttempt? SuccessfulPaymentAttempt { get; set; }

    public DateTime ExpireAt { get; set; }

    public required string CallbackUrl { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public required DateTime CreatedAt { get; set; }

    [NotMapped]
    public TransactionStatus Status
    {
        get
        {
            if (CanceledAt.HasValue)
                return TransactionStatus.Canceled;

            if (ExpireAt <= DateTime.UtcNow)
                return TransactionStatus.Expired;

            return TransactionStatus.Available;
        }
    }
}
