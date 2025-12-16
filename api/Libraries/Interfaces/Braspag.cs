namespace FeevCheckout.Libraries.Interfaces;

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

public class SalesResponse
{
    public required string MerchantOrderId { get; set; }

    public required Customer Customer { get; set; }

    public required Payment Payment { get; set; }
}
