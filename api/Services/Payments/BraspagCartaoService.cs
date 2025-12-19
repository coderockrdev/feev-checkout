using FeevCheckout.Dtos;
using FeevCheckout.Libraries.Http;
using FeevCheckout.Libraries.Interfaces;
using FeevCheckout.Models;

using Flurl.Http;

namespace FeevCheckout.Services.Payments;

public interface IBraspagCartaoService
{
    Task<SalesResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        Installment installment,
        CardDto card
    );
}

public class BraspagCartaoService(IBraspagClient braspagClient) : IBraspagCartaoService
{
    private readonly IBraspagClient braspagClient = braspagClient;

    public async Task<SalesResponse> CreatePayment(
        Establishment establishment,
        Credential credentials,
        Transaction transaction,
        Installment installment,
        CardDto card
    )
    {
        var request = braspagClient.CreateRequest(credentials, "/sales");

        return await request.PostJsonAsync(new
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
