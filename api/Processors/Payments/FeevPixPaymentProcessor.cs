using System.Text.Json;

using FeevCheckout.DTOs;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Processors.Payments;

public class FeevPixPaymentProcessor(IFeevPixService feevPixService) : IPaymentProcessor
{
    private readonly IFeevPixService feevPixService = feevPixService;

    public PaymentMethod Method => PaymentMethod.FeevPix;

    public async Task<PaymentResult> ProcessAsync(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment,
        PaymentRequestDto request
    )
    {
        var response = await feevPixService.CreatePayment(
            establishment,
            credentials,
            transaction,
            paymentRule,
            installment
        );

        return new PaymentResult
        {
            Success = true,
            Method = Method,
            ExternalId = response.Parcelas[0].TxId,
            ExtraData = JsonDocument.Parse(
                JsonSerializer.Serialize(new
                {
                    Code = response.Parcelas[0].Brcode,
                    response.Parcelas[0].Location,
                    CreatedAt = response.Parcelas[0].DataHoraCriacao,
                    DueAt = response.Parcelas[0].DataVencimento,
                    ExpireAt = response.Parcelas[0].DataHoraExpiracao
                })
            ),
            Response = JsonDocument.Parse(
                JsonSerializer.Serialize(response)
            )
        };
    }
}
