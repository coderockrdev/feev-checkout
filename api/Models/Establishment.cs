namespace FeevCheckout.Models;

public class Establishment
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public string? BankNumber { get; set; }

    public string? BankAgency { get; set; }

    public string? BankAccount { get; set; }

    public string? CheckingAccountNumber { get; set; }
}
