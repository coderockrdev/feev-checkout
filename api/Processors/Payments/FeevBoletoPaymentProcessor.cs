using System.Text.Json;

using FeevCheckout.DTOs;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Processors.Payments;

public class FeevBoletoPaymentProcessor(IFeevBoletoService feevBoletoService) : IPaymentProcessor
{
    private readonly IFeevBoletoService feevBoletoService = feevBoletoService;

    public PaymentMethod Method => PaymentMethod.FeevBoleto;

    public async Task<PaymentResult> ProcessAsync(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment,
        PaymentRequestDto request
    )
    {
        var response = await feevBoletoService.CreatePayment(
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
            Status = PaymentAttemptStatus.Pending,
            ExternalId = response.Boletos[0].NumeroBoleto.ToString(),
            ExtraData = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                Code = response.CodigoFatura,
                Link = response.LinkCarne,
                Invoices = response.Boletos.Select(boleto =>
                {
                    return new
                    {
                        Number = boleto.NumeroParcela,
                        DueAt = boleto.Vencimento,
                        DigitableLine = boleto.LinhaDigitavel,
                        Link = boleto.LinkBoleto
                    };
                })
            })),
            Response = JsonDocument.Parse(
                JsonSerializer.Serialize(response)
            )
        };
    }
}
