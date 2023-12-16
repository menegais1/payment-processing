namespace PaymentProcessing.Schemas;

public class TransactionUpdate
{
    public DateTime? ApprovedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public TransactionStatus? Status { get; set; } = TransactionStatus.Created;
}