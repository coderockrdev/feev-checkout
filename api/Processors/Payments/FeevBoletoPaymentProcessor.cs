using System.Text.Json;

using FeevCheckout.Dtos;
using FeevCheckout.Enums;
using FeevCheckout.Models;
using FeevCheckout.Services.Payments;

namespace FeevCheckout.Processors.Payments;

public class FeevBoletoInvoicesPaymentData
{
    public required int Installment { get; set; }

    public required DateTime DueAt { get; set; }

    public required string DigitableLine { get; set; }

    public required string Link { get; set; }
}

public class FeevBoletoPaymentData
{
    public required int InvoiceNumber { get; set; }

    public required string BookletLink { get; set; }

    public required List<FeevBoletoInvoicesPaymentData> Invoices { get; set; }
}

public class FeevBoletoPaymentResult : PaymentResult
{
    public new required FeevBoletoPaymentData ExtraData { get; set; }
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
            ExtraData = new FeevBoletoPaymentData
            {
                InvoiceNumber = response.CodigoFatura,
                BookletLink = response.LinkCarne,
                Invoices =
                [
                    .. response.Boletos.Select(invoice =>
                    {
                        return new FeevBoletoInvoicesPaymentData
                        {
                            Installment = invoice.NumeroParcela,
                            DueAt = invoice.Vencimento,
                            DigitableLine = invoice.LinhaDigitavel,
                            Link = invoice.LinkBoleto
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
