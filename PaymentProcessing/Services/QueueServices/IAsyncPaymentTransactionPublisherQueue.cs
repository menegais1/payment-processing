namespace PaymentProcessing;

public interface IAsyncPaymentTransactionPublisherQueue : IDisposable
{
    public bool PublishMessage(PaymentTransactionMessagePayload message);
}