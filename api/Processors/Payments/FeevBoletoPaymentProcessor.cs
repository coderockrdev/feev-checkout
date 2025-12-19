using System.Text.Json;

using FeevCheckout.Dtos;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Processors.Payments;

public class Boleto
{
    public required int NumeroParcela { get; set; }

    public required DateTime Vencimento { get; set; }

    public required string LinhaDigitavel { get; set; }

    public required string LinkBoleto { get; set; }
}

public class FeevBoletoExtraData
{
    public required int CodigoFatura { get; set; }

    public required string LinkCarne { get; set; }

    public required List<Boleto> Boletos { get; set; }
}

public class FeevBoletoPaymentResult : PaymentResult
{
    public new required FeevBoletoExtraData ExtraData { get; set; }
}

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

        return new FeevBoletoPaymentResult
        {
            Success = true,
            Method = Method,
            ExternalId = response.Boletos[0].NumeroBoleto.ToString(),
            ExtraData = new FeevBoletoExtraData
            {
                CodigoFatura = response.CodigoFatura,
                LinkCarne = response.LinkCarne,
                Boletos =
                [
                    .. response.Boletos.Select(boleto =>
                    {
                        return new Boleto
                        {
                            NumeroParcela = boleto.NumeroParcela,
                            Vencimento = boleto.Vencimento,
                            LinhaDigitavel = boleto.LinhaDigitavel,
                            LinkBoleto = boleto.LinkBoleto
                        };
                    })
                ]
            },
            Response = JsonDocument.Parse(
                JsonSerializer.Serialize(response)
            )
        };
    }
}
