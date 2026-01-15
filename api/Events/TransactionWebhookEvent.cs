namespace FeevCheckout.Events;

public sealed class TransactionWebhookEvent
{
    public string Name { get; }

    private TransactionWebhookEvent(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;

    public static readonly TransactionWebhookEvent TransactionCreated =
        new("transaction.created");

    public static readonly TransactionWebhookEvent TransactionCanceled =
        new("transaction.canceled");

    public static readonly TransactionWebhookEvent TransactionExpired =
        new("transaction.expired");

    public static readonly TransactionWebhookEvent TransactionCompleted =
        new("transaction.completed");

    public static readonly TransactionWebhookEvent TransactionPaymentAttempt =
        new("transaction.payment_attempt");

    public static readonly TransactionWebhookEvent TransactionPaymentCreated =
        new("transaction.payment_created");

    public static readonly TransactionWebhookEvent TransactionPaymentFailed =
        new("transaction.payment_failed");
}
