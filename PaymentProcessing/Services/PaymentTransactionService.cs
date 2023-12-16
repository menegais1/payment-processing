using NuGet.Protocol;
using PaymentProcessing.Database;
using PaymentProcessing.Schemas;

namespace PaymentProcessing;

public class PaymentTransactionService
{
    private Random _random = new Random();

    public PaymentTransactionService(IAsyncPaymentTransactionConsumerQueue consumerQueue)
    {
        consumerQueue.RegisterConsumer(this.ProcessMessage);
    }

    private async Task<bool> ProcessMessage(PaymentTransactionMessagePayload transactionMessagePayload)
    {
        // Add a random timeout to simulate real processing times
        await Task.Delay(_random.Next(1000, 10000));
        Console.Out.WriteLine(transactionMessagePayload);
        switch (transactionMessagePayload.TaskType)
        {
            case PaymentTransactionTaskType.Process:
                await ProcessTransaction(transactionMessagePayload.MessageBody.FromJson<Guid>());
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    $"Task type {transactionMessagePayload.TaskType} is not supported.");
        }

        return true;
    }

    private async Task ProcessTransaction(Guid transactionId)
    {
        await using var transactionRepository = new TransactionRepository(new PaymentProcessingContext());
        var transaction = await transactionRepository.GetTransaction(transactionId);
        if (transaction is null)
        {
            throw new Exception($"Couldn't find transaction {transactionId} in the Database. This is not expected.");
        }

        if (transaction.Status != TransactionStatus.Created)
        {
            return;
        }

        var prob = _random.Next(0, 10);

        if (prob > 2)
        {
            await transactionRepository.UpdateTransaction(transactionId, new TransactionUpdate()
            {
                ApprovedAt = DateTime.UtcNow,
                Status = TransactionStatus.Approved
            });
        }
        else
        {
            await transactionRepository.UpdateTransaction(transactionId, new TransactionUpdate()
            {
                FailedAt = DateTime.UtcNow,
                Status = TransactionStatus.Failed
            });
        }
    }
}