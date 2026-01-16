using FeevCheckout.Libraries.Http;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public interface IFeevBoletoService
{
    Task<InserirFaturaResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment
    );

    Task<ConsultaArquivoRetornoResponse> GetResponseFile(
        Establishment establishment,
        Credential credentials,
        string batch
    );
}

public class FeevBoletoService(IFeevBoletoClient feevBoletoClient) : IFeevBoletoService
{
    private readonly IFeevBoletoClient feevBoletoClient = feevBoletoClient;

    public async Task<InserirFaturaResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment
    )
    {
        if (string.IsNullOrEmpty(establishment.BankNumber) || string.IsNullOrEmpty(establishment.BankAgency) ||
            string.IsNullOrEmpty(establishment.BankAccount))
            throw new InvalidOperationException("Establishment's bank account information is incompleted.");

        var request = await feevBoletoClient.CreateRequest(credentials, "/InserirFatura");

        return await request.PostJsonAsync(new
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
                        UF = transaction.Customer.Address.State
                    },
                    telefones = Array.Empty<object>()
                }
            })
            .ReceiveJson<InserirFaturaResponse>();
    }

    public async Task<ConsultaArquivoRetornoResponse> GetResponseFile(Establishment establishment,
        Credential credentials, string batch)
    {
        if (string.IsNullOrEmpty(establishment.BankNumber) || string.IsNullOrEmpty(establishment.BankAgency) ||
            string.IsNullOrEmpty(establishment.BankAccount))
            throw new InvalidOperationException("Establishment's bank account information is incompleted.");

        var request = await feevBoletoClient.CreateRequest(credentials, "/ConsultaArquivoRetorno");

        return await request.SetQueryParams(new
        {
            banco = establishment.BankNumber,
            agencia = establishment.BankAgency,
            conta = establishment.BankAccount,
            lote = batch,
            codigoOcorrenciaBancaria = 6
        }).GetJsonAsync<ConsultaArquivoRetornoResponse>();
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
