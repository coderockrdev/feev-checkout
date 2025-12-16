using FeevCheckout.Enums;

namespace FeevCheckout.Models;

public class Credential
{
    public required Guid Id { get; set; }

    public required Guid EstablishmentId { get; set; }

    public Establishment? Establishment { get; set; }

    public required PaymentMethod Method { get; set; }

    public required Dictionary<string, string> Data { get; set; }

    public required string? BraspagProvider { get; set; }
}
