using System.Threading.Channels;

namespace FeevCheckout.Queue;

public static class FeevBoletoResponseFileQueue
{
    public static readonly Channel<FeevBoletoResponseFileWokerPayload> Channel =
        System.Threading.Channels.Channel.CreateUnbounded<FeevBoletoResponseFileWokerPayload>();
}
