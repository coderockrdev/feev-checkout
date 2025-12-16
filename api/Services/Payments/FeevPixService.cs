using FeevCheckout.Libraries.Http;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public interface IFeevPixService
{
    public Task<IncluirCobrancaPixResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule paymentRule,
        Installment installment
    );
}

public class FeevPixService(IFeevPixClient feevPixClient) : IFeevPixService
{
    private readonly IFeevPixClient feevPixClient = feevPixClient;

    public async Task<IncluirCobrancaPixResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        PaymentRule _,
        Installment installment
    )
    {
        if (string.IsNullOrEmpty(establishment.CheckingAccountNumber))
            throw new InvalidOperationException("Establishment's checking account number not set.");

        var request = await feevPixClient.CreateRequest(credentials, "/IncluirCobrancaPix");

        return await request.PostJsonAsync(new
        {
            tipoCobrancaPix = "imediato",
            TipoOrigemCobranca = "outros",
            segundosExpiracao = 30 * 60, // 30 minutes
            nomeDevedor = transaction.Customer.Name,
            cpfCnpjDevedor = transaction.Customer.Document,
            emailDevedor = transaction.Customer.Email,
            logradouroDevedor = transaction.Customer.Address.Street,
            cidadeDevedor = transaction.Customer.Address.City,
            ufDevedor = transaction.Customer.Address.State,
            cepDevedor = transaction.Customer.Address.ZipCode,
            descricao = transaction.Description,
            codigoContaCorrente = establishment.CheckingAccountNumber,
            parcelas = new[]
            {
                new
                {
                    numeroParcela = 1,
                    dataVencimento = DateTime.Now.ToString("yyyy-MM-dd"),
                    diasValidadeAposVencimento = 0,
                    valor = installment.FinalAmount / 100.0
                }
            }
        }).ReceiveJson<IncluirCobrancaPixResponse>();
    }
}
