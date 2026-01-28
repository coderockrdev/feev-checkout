using System.ComponentModel.DataAnnotations;

namespace FeevCheckout.Models;

public class Establishment
{
    public required Guid Id { get; set; }

    public required string FullName { get; set; }

    [StringLength(13)] public required string ShortName { get; set; }

    [StringLength(14)] public required string CNPJ { get; set; }

    public required string Domain { get; set; }

    public string? BankNumber { get; set; }

    public string? BankAgency { get; set; }

    public string? BankAccount { get; set; }

    public string? CheckingAccountNumber { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }
}
