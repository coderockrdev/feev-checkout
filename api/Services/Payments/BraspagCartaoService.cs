using FeevCheckout.Dtos;
using FeevCheckout.Libraries.Http;
using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public class Link
{
    public required string Method { get; set; }

    public required string Rel { get; set; }

    public required string Href { get; set; }
}

public class CreditCard
{
    public required string CardNumber { get; set; }

    public required string Holder { get; set; }

    public required string ExpirationDate { get; set; }

    public required bool SaveCard { get; set; }

    public required string Brand { get; set; }

    public required string PaymentAccountReference { get; set; }
}

public class Payment
{
    public required int ServiceTaxAmount { get; set; }

    public required int Installments { get; set; }

    public required string Interest { get; set; }

    public required bool Capture { get; set; }

    public required bool Authenticate { get; set; }

    public required bool Recurrent { get; set; }

    public required CreditCard CreditCard { get; set; }

    public required string ProofOfSale { get; set; }

    public required string AcquirerTransactionId { get; set; }

    public required string AuthorizationCode { get; set; }

    public required string SoftDescriptor { get; set; }

    public required string SentOrderId { get; set; }

    public required string PaymentId { get; set; }

    public required string Type { get; set; }

    public required int Amount { get; set; }

    public required string ReceivedDate { get; set; }

    public int CapturedAmount { get; set; }

    public required string CapturedDate { get; set; }

    public required string Currency { get; set; }

    public required string Country { get; set; }

    public required string Provider { get; set; }

    public required int ReasonCode { get; set; }

    public required string ReasonMessage { get; set; }

    public required int Status { get; set; }

    public required string ProviderReturnCode { get; set; }

    public required string ProviderReturnMessage { get; set; }

    public required List<Link> Links { get; set; }
}

public class Customer
{
    public required string Name { get; set; }

    public required string Identity { get; set; }

    public required string IdentityType { get; set; }

    public required string Email { get; set; }
}

public class BraspagCartaoResponse
{
    public required string MerchantOrderId { get; set; }

    public required Customer Customer { get; set; }

    public required Payment Payment { get; set; }
}

public interface IBraspagCartaoService
{
    public Task<BraspagCartaoResponse> CreatePayment(Credential credentials, Transaction transaction,
        Installment installment, CardDto card);
}

public class BraspagCartaoService(BraspagClient braspagClient) : IBraspagCartaoService
{
    private readonly BraspagClient braspagClient = braspagClient;

    public async Task<BraspagCartaoResponse> CreatePayment(
        Credential credentials,
        Transaction transaction,
        Installment installment,
        CardDto card
    )
    {
        var client = braspagClient.Build(credentials);

        return await client.Request("/sales").PostJsonAsync(new
        {
            MerchantOrderId = transaction.Identifier,
            Customer = new
            {
                transaction.Customer.Name,
                Identity = transaction.Customer.Document,
                IdentityType = transaction.Customer.Document.Length > 11 ? "CNPJ" : "CPF",
                transaction.Customer.Email,
                AddressObject = new
                {
                    transaction.Customer.Address.Street,
                    transaction.Customer.Address.Number,
                    transaction.Customer.Address.Complement,
                    transaction.Customer.Address.ZipCode,
                    transaction.Customer.Address.City,
                    transaction.Customer.Address.State,
                    Country = "BRA",
                    transaction.Customer.Address.District
                }
            },
            Payment = new
            {
                Provider = credentials.BraspagProvider,
                Type = "CreditCard",
                Amount = installment.FinalAmount,
                Currency = "BRL",
                Country = "BRA",
                Installments = installment.Number,
                Interest = "ByMerchant",
                Capture = true,
                SoftDescriptor = "ADSSADSASA",
                CreditCard = new
                {
                    CardNumber = card.Number,
                    card.Holder,
                    ExpirationDate = card.DueAt,
                    card.SecurityCode,
                    Brand = "visa"
                }
            }
        }).ReceiveJson<BraspagCartaoResponse>();
    }
}
