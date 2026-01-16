using FeevCheckout.Data;
using FeevCheckout.Events;
using FeevCheckout.Models;
using FeevCheckout.Services;

using Microsoft.EntityFrameworkCore;

namespace FeevCheckout.Queue;

public class FeevTransactionExpirationWorkerPayload
{
    public required Establishment Establishment { get; set; }

    public required Credential Credentials { get; set; }

    public required string Batch { get; set; }
}

public class FeevTransactionExpirationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    private readonly IServiceProvider serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessExpiredTransactions(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessExpiredTransactions(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ITransactionWebhookDispatcherService>();

        var now = DateTime.UtcNow;

        // TODO: move it to TransactionService
        var expired = await context.Transactions
            .Where(transaction =>
                transaction.ExpireAt.HasValue && // TODO: it should never be empty
                transaction.ExpireAt <= now &&
                transaction.CanceledAt == null &&
                transaction.CompletedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var transaction in expired)
        {
            transaction.CanceledAt = now;

            await dispatcher.DispatchAsync(
                TransactionWebhookEvent.Expired,
                transaction
            );
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
