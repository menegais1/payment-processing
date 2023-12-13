namespace PaymentProcessing.Schemas;

public class CreatePaymentTransactionResponse
{
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; }
}