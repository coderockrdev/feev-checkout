using FeevCheckout.Enums;

namespace FeevCheckout.Processors.Payments;

public class PaymentProcessorFactory(IEnumerable<IPaymentProcessor> processors)
{
    private readonly IEnumerable<IPaymentProcessor> _processors = processors;

    public IPaymentProcessor? GetProcessor(PaymentMethod method)
    {
        return _processors.FirstOrDefault(processor => processor.Method == method);
    }
}
