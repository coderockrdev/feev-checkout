using System.Threading.Channels;

namespace FeevCheckout.Queue;

public static class FeevBoletoResponseFileQueue
{
    public static readonly Channel<FeevBoletoResponseFileWorkerPayload> Channel =
        System.Threading.Channels.Channel.CreateUnbounded<FeevBoletoResponseFileWorkerPayload>();
}
