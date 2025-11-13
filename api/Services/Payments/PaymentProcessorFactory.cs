namespace FeevCheckout.Services.Payments;

public class PaymentProcessorFactory(IEnumerable<IPaymentProcessor> processors)
{
    private readonly IEnumerable<IPaymentProcessor> _processors = processors;

    public IPaymentProcessor? GetProcessor(string method)
    {
        return _processors.FirstOrDefault(processor =>
            processor.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
    }
}
