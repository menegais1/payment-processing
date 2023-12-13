namespace PaymentProcessing;

public interface IAsyncPaymentTransactionConsumerQueue : IDisposable
{
    public bool RegisterConsumer(Func<PaymentTransactionMessagePayload, Task<bool>> callback);
}