using NuGet.Protocol;

namespace PaymentProcessing;

public class PaymentTransactionService
{
    public PaymentTransactionService(IAsyncPaymentTransactionConsumerQueue consumerQueue)
    {
        consumerQueue.RegisterConsumer(this.ProcessMessage);
    }

    public async Task<bool> ProcessMessage(PaymentTransactionMessagePayload transactionMessagePayload)
    {
        await Console.Out.WriteLineAsync(transactionMessagePayload.ToString());
        return true;
    }
}