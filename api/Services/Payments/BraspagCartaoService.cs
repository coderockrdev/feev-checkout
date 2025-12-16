using FeevCheckout.Dtos;
using FeevCheckout.Libraries.Http;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public interface IBraspagCartaoService
{
    public Task<SalesResponse> CreatePayment(
        Credential credentials,
        Transaction transaction,
        Installment installment,
        CardDto card
    );
}

public class BraspagCartaoService(IBraspagClient client) : IBraspagCartaoService
{
    private readonly IBraspagClient client = client;

    public async Task<SalesResponse> CreatePayment(
        Credential credentials,
        Transaction transaction,
        Installment installment,
        CardDto card
    )
    {
        return await client.CreateRequest(credentials, "/sales").PostJsonAsync(new
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
        }).ReceiveJson<SalesResponse>();
    }
}
