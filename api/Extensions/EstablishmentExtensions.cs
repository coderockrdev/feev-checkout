using FeevCheckout.Models;

namespace FeevCheckout.Extensions;

public static class EstablishmentExtensions
{
    public static void EnsureBankAccountComplete(this Establishment establishment)
    {
        if (string.IsNullOrEmpty(establishment.BankNumber) ||
            string.IsNullOrEmpty(establishment.BankAgency) ||
            string.IsNullOrEmpty(establishment.BankAccount))
            throw new InvalidOperationException("Establishment's bank account information is incomplete.");
    }

    public static void EnsureCheckingAccountNumberSet(this Establishment establishment)
    {
        if (string.IsNullOrEmpty(establishment.CheckingAccountNumber))
            throw new InvalidOperationException("Establishment's checking account number not set.");
    }

    public static void EnsurePaymentInfoComplete(this Establishment establishment)
    {
        establishment.EnsureBankAccountComplete();
        establishment.EnsureCheckingAccountNumberSet();
    }
}
