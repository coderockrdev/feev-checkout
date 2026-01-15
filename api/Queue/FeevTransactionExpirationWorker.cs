using FeevCheckout.Data;
using FeevCheckout.DTOs.Factories;
using FeevCheckout.Enums;
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
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessExpiredTransactions(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessExpiredTransactions(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcherService>();

        var now = DateTime.UtcNow;

        var expired = await context.Transactions
            .Where(t =>
                t.ExpireAt.HasValue &&
                t.ExpireAt <= now &&
                t.CanceledAt == null &&
                t.CompletedAt == null)
            .ToListAsync(ct);

        foreach (var transaction in expired)
        {
            transaction.CanceledAt = now;

            await dispatcher.DispatchAsync(
                TransactionWebhookDtoFactory.Create(
                    TransactionEvent.Expired,
                    transaction
                )
            );
        }

        await context.SaveChangesAsync(ct);
    }
}
