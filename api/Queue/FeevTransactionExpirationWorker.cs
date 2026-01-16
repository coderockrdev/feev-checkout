using FeevCheckout.Services;

namespace FeevCheckout.Queue;

public class FeevTransactionExpirationWorker(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    private readonly IServiceProvider serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

            await transactionService.ProcessExpiredTransactions(cancellationToken);

            await Task.Delay(_interval, cancellationToken);
        }
    }
}
