namespace PaymentProcessing.Schemas;

public class ClientFacingTransaction
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
    public string OrganizationId { get; set; }

    public static ClientFacingTransaction ConvertToClientFacingTransaction(Transaction transaction)
    {
        return new ClientFacingTransaction
        {
            Id = transaction.Id,
            PayerAccount = transaction.PayerAccount,
            PayeeAccount = transaction.PayeeAccount,
            Amount = transaction.Amount,
            Fee = transaction.Fee,
            CreatedAt = transaction.CreatedAt,
            ApprovedAt = transaction.ApprovedAt,
            FailedAt = transaction.FailedAt,
            CancelledAt = transaction.CancelledAt,
            Description = transaction.Description,
            CustomerKey = transaction.CustomerKey,
            Status = transaction.Status,
            OrganizationId = transaction.OrganizationId
        };
    }
}