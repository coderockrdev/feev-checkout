using System.Text.Json;

using FeevCheckout.Dtos;
using FeevCheckout.Enums;
using FeevCheckout.Models;

using Flurl;
using Flurl.Http;

namespace FeevCheckout.Services.Payments;

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

public class FeevBoleto
{
    public required int NumeroBoleto { get; set; }

    public required int NumeroParcela { get; set; }

    public required DateTime Vencimento { get; set; }

    public required decimal Valor { get; set; }

    public required string NossoNumero { get; set; }

    public required string LinhaDigitavel { get; set; }

    public required string SituacaoBoleto { get; set; }

    public required string UltimaOcorrenciaBancaria { get; set; }

    public required int CodigoUltimaOcorrenciaBancaria { get; set; }

    public required DateTime DataLimitePagamento { get; set; }

    public required string LinkBoleto { get; set; }

    public required string Banco { get; set; }

    public required string Agencia { get; set; }

    public required string Conta { get; set; }
}

public class FeevBoletoResponse
{
    public required string Evento { get; set; }

    public required string Banco { get; set; }

    public required string Agencia { get; set; }

    public required string Conta { get; set; }

    public required int CodigoFatura { get; set; }

    public required string? CodigoFaturaParceiro { get; set; }

    public required string LinkCarne { get; set; }

    public required string Base64Carne { get; set; }

    public required List<FeevBoleto> Boletos { get; set; }
}

public class FeevBoletoPaymentProcessor(IConfiguration configuration) : IPaymentProcessor
{
    private readonly string authBaseUrl = configuration["AppSettings:Feev:AuthBaseUrl"]
                                          ?? throw new InvalidOperationException(
                                              "Feev auth base URL not found or not specified.");

    private readonly string boletoBaseUrl = configuration["AppSettings:Feev:BoletoBaseUrl"]
                                            ?? throw new InvalidOperationException(
                                                "Feev Boleto base URL not found or not specified.");

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
        if (string.IsNullOrEmpty(establishment.BankNumber) || string.IsNullOrEmpty(establishment.BankAgency) ||
            string.IsNullOrEmpty(establishment.BankAccount))
            throw new InvalidOperationException("Establishment's bank account information is incompleted.");

        var token = await Authenticate(credentials);

        var payload = new
        {
            codigoFaturaParceiro = transaction.Identifier,
            descricaoFatura = transaction.Description,
            dataFatura = DateTime.Now.ToString("yyyy-MM-dd"),
            meioPagamento = "BOLETO",
            banco = establishment.BankNumber,
            agencia = establishment.BankAgency,
            conta = establishment.BankAccount,
            itens = transaction.Products.Select(product =>
            {
                return new
                {
                    descricaoItem = product.Name,
                    quantidadeItem = 1,
                    valorItem = product.Price / 100.0
                };
            }),
            parcelas = MapInstallments(paymentRule, installment),
            pagador = new
            {
                numeroCpfCnpj = transaction.Customer.Document,
                nome = transaction.Customer.Name,
                email = transaction.Customer.Email,
                endereco = new
                {
                    CEP = transaction.Customer.Address.ZipCode,
                    logradouro = transaction.Customer.Address.Street,
                    numero = transaction.Customer.Address.Number,
                    complemento = transaction.Customer.Address.Complement,
                    bairro = transaction.Customer.Address.District,
                    localidade = transaction.Customer.Address.City,
                    transaction.Customer.Address.State
                },
                telefones = Array.Empty<object>()
            }
        };

        var response = await $"{boletoBaseUrl}/api/Fatura/InserirFatura"
            .WithOAuthBearerToken(token)
            .PostJsonAsync(payload)
            .ReceiveJson<FeevBoletoResponse>();

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

    private async Task<string> Authenticate(Credential credentials)
    {
        var token = await $"{authBaseUrl}/api/autenticacao/obtertoken"
            .SetQueryParams(credentials.Data)
            .GetStringAsync();

        return token.Trim();
    }

    private static List<object> MapInstallments(PaymentRule paymentRule, Installment installment)
    {
        var installments = new List<object>();

        var totalAmount = installment.FinalAmount;
        var installmentCount = installment.Number;

        var baseAmount = totalAmount / installmentCount;
        var remainder = totalAmount % installmentCount;

        for (var index = 0; index < installment.Number; index++)
        {
            var installmentAmount = baseAmount + (index == 0 ? remainder : 0);

            installments.Add(new
            {
                valor = installmentAmount / 100.0,
                vencimento = installment.DueAt,
                percentualMulta = paymentRule.LateFee,
                percentualJurosMes = paymentRule.Interest,
                dataLimitePagamento = installment.ExpireAt,
                mensagem1 = "",
                mensagem2 = ""
            });
        }

        return installments;
    }
}
