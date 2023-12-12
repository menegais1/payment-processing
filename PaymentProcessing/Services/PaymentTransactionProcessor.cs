using NuGet.Protocol;

namespace PaymentProcessing;

public class PaymentTransactionProcessor
{
    public PaymentTransactionProcessor(IAsyncPaymentTransactionConsumerQueue consumerQueue)
    {
        consumerQueue.RegisterConsumer(this.ProcessMessage);
    }

    public async Task<bool> ProcessMessage(PaymentTransactionMessagePayload transactionMessagePayload)
    {
        await Console.Out.WriteLineAsync(transactionMessagePayload.ToString());
        return true;
    }
}