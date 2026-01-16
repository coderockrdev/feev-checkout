namespace FeevCheckout.Events;

public sealed class TransactionWebhookEvent
{
    public string Name { get; }

    private TransactionWebhookEvent(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;

    public static readonly TransactionWebhookEvent Created =
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

    public static readonly TransactionWebhookEvent PaymentAttemptFailed =
        new("transaction.payment_attempt.failed");
}
