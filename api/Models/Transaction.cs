namespace FeevCheckout.Models;

public class Transaction
{
    public required int Id { get; set; }

    public required string Establihsment { get; set; }

    public required decimal Amout { get; set; }
}
