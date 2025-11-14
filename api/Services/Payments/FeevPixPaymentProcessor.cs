using FeevCheckout.Enums;
using FeevCheckout.Models;

using Flurl;
using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public class FeevPixPaymentData
{
    public required string Code { get; set; }

    public required string Location { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime DueAt { get; set; }

    public required DateTime ExpireAt { get; set; }
}

public class FeevPixPaymentResult : PaymentResult
{
    public new required FeevPixPaymentData ExtraData { get; set; }
}

public class FeevPixParcela
{
    public required string TxId { get; set; }

    public required string Brcode { get; set; }

    public required string Location { get; set; }

    public required DateTime DataHoraCriacao { get; set; }

    public required DateTime DataVencimento { get; set; }

    public required DateTime DataHoraExpiracao { get; set; }
}

public class FeevPixResponse
{
    public required List<FeevPixParcela> Parcelas { get; set; }
}

public class FeevPixPaymentProcessor(IConfiguration configuration) : IPaymentProcessor
{
    private readonly string authBaseUrl = configuration["AppSettings:Feev:AuthBaseUrl"]
                                          ?? throw new InvalidOperationException(
                                              "Feev auth base URL not found or not specified.");

    private readonly string pixBaseUrl = configuration["AppSettings:Feev:PixBaseUrl"]
                                         ?? throw new InvalidOperationException(
                                             "Feev Pix base URL not found or not specified.");

    public PaymentMethod Method => PaymentMethod.FeevPix;

    public async Task<PaymentResult> ProcessAsync(Credential credentials, Transaction transaction)
    {
        var token = await Authenticate(credentials);

        var payload = new
        {
            tipoCobrancaPix = "imediato",
            TipoOrigemCobranca = "outros",
            segundosExpiracao = 3600,
            nomeDevedor = transaction.Customer.Name,
            cpfCnpjDevedor = transaction.Customer.Document,
            emailDevedor = transaction.Customer.Email,
            logradouroDevedor = transaction.Customer.Address.Street,
            cidadeDevedor = transaction.Customer.Address.City,
            ufDevedor = transaction.Customer.Address.UF,
            cepDevedor = transaction.Customer.Address.PostalCode,
            descricao = $"Transação {transaction.Id}",
            codigoContaCorrente = 8,
            parcelas = new[]
            {
                new
                {
                    numeroParcela = 1,
                    dataVencimento = DateTime.Now.ToString("yyyy-MM-dd"),
                    diasValidadeAposVencimento = 0,
                    valor = transaction.TotalAmount / 100.0
                }
            }
        };

        var response = await $"{pixBaseUrl}/api/pix/IncluirCobrancaPix"
            .WithOAuthBearerToken(token)
            .PostJsonAsync(payload)
            .ReceiveJson<FeevPixResponse>();

        return new FeevPixPaymentResult
        {
            Success = true,
            Method = Method,
            ReferenceId = response.Parcelas[0].TxId,
            ExtraData = new FeevPixPaymentData
            {
                Code = response.Parcelas[0].Brcode,
                Location = response.Parcelas[0].Location,
                CreatedAt = response.Parcelas[0].DataHoraCriacao,
                DueAt = response.Parcelas[0].DataVencimento,
                ExpireAt = response.Parcelas[0].DataHoraExpiracao
            }
        };
    }

    private async Task<string> Authenticate(Credential credentials)
    {
        var token = await $"{authBaseUrl}/api/autenticacao/obtertoken"
            .SetQueryParams(credentials.Data)
            .GetStringAsync();

        return token.Trim();
    }
}
