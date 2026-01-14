namespace FeevCheckout.Events;

public sealed class TransactionWebhookEvent
{
    public static readonly TransactionWebhookEvent TransactionCreated =
        new("transaction.created");

    public static readonly TransactionWebhookEvent Canceled =
        new("transaction.canceled");

    public static readonly TransactionWebhookEvent Expired =
        new("transaction.expired");

    public static readonly TransactionWebhookEvent Completed =
        new("transaction.completed");

    public static readonly TransactionWebhookEvent PaymentAttemptCreated =
        new("transaction.payment_attempt.created");

    public static readonly TransactionWebhookEvent PaymentAttemptPending =
        new("transaction.payment_attempt.pending");

    public static readonly TransactionWebhookEvent TransactionPaymentFailed =
        new("transaction.payment_failed");

    private TransactionWebhookEvent(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString()
    {
        return Name;
    }
}
