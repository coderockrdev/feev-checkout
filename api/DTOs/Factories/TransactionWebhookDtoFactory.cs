using FeevCheckout.Models;

namespace FeevCheckout.DTOs.Factories;

public static class TransactionWebhookDtoFactory
{
    public static TransactionWebhookDto Create(Transaction transaction)
    {
        return new TransactionWebhookDto(
            TransactionId: transaction.Id,
            Identifier: transaction.Identifier,
            Status: transaction.Status,
            Establishment: new EstablishmentWebhookDto(
                transaction.Establishment!.Id,
                transaction.Establishment.FullName
            ),
            Customer: new CustomerWebhookDto(
                transaction.Customer.Name,
                transaction.Customer.Email
            ),
            TotalAmount: transaction.TotalAmount,
            Products: transaction.Products
                .Select(p => new ProductWebhookDto(p.Id, p.Name, p.Price))
                .ToList(),
            PaymentAttempt: transaction.SuccessfulPaymentAttempt is null
                ? null
                : new PaymentAttemptWebhookDto(
                    transaction.SuccessfulPaymentAttempt.Id,
                    transaction.SuccessfulPaymentAttempt.Method,
                    transaction.SuccessfulPaymentAttempt.Status
                )
        );
    }
}
