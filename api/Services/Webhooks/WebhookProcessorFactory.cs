using FeevCheckout.Enums;

namespace FeevCheckout.Services.Webhooks;

public class WebhookProcessorFactory(IEnumerable<IWebhookProcessor> processors)
{
    private readonly IEnumerable<IWebhookProcessor> _processors = processors;

    public IWebhookProcessor? GetProcessor(PaymentMethod method)
    {
        return _processors.FirstOrDefault(processor => processor.Method == method);
    }
}
