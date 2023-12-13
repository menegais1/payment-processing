namespace PaymentProcessing;

public enum TransactionStatus
{
    Created,
    Approved,
    Cancelled,
    Failed
}

public class Transaction
{
    public Guid Id { get; set; }

    public string PayerAccount { get; set; }

    public string PayeeAccount { get; set; }
    public Int64 Amount { get; set; }
    public Int64 Fee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Description { get; set; }
    public string? CustomerKey { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Created;
}