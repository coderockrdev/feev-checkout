using System.Text.Json;

using FeevCheckout.Dtos;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Processors.Payments;

public class BraspagCartaoPaymentProcessor(IBraspagCartaoService braspagCartaoService) : IPaymentProcessor
{
    private readonly IBraspagCartaoService braspagCartaoService = braspagCartaoService;

    public PaymentMethod Method => PaymentMethod.BraspagCartao;

    public async Task<PaymentResult> ProcessAsync(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment,
        PaymentRequestDto request
    )
    {
        if (string.IsNullOrEmpty(credentials.BraspagProvider))
            throw new InvalidOperationException("Establishment's Braspag Provider information is incompleted.");

        var response =
            await braspagCartaoService.CreatePayment(
                establishment,
                credentials,
                transaction,
                installment,
                request.Card!
            );

        return new PaymentResult
        {
            Success = true,
            Method = request.Method,
            ExternalId = response.Payment.PaymentId,
            ExtraData = new
            {
                response.Payment.ProofOfSale,
                response.Payment.AcquirerTransactionId,
                response.Payment.AuthorizationCode,
                response.Payment.SoftDescriptor,
                response.Payment.SentOrderId,
                response.Payment.PaymentId,
                response.Payment.CapturedDate,
                response.Payment.Provider,
                response.Payment.ReasonCode,
                response.Payment.ReasonMessage,
                response.Payment.Status,
                response.Payment.ProviderReturnCode,
                response.Payment.ProviderReturnMessage,
                response.Payment.CreditCard.PaymentAccountReference
            },
            Response = JsonDocument.Parse(
                JsonSerializer.Serialize(response)
            )
        };
    }
}
