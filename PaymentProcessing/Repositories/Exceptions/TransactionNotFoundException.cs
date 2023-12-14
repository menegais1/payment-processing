namespace PaymentProcessing.Schemas;

public class TransactionNotFoundException : Exception
{
    public override string Message { get; }

    public TransactionNotFoundException(Guid transactionId)
    {
        Message = $"Transaction with TransactionID {transactionId} not Found";
    }
}