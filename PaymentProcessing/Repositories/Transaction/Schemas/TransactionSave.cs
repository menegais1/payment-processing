namespace PaymentProcessing.Schemas;

public class TransactionSave(string payerAccount, string payeeAccount, Int64 amount, string organizationId)
{
    public string PayerAccount { get; set; } = payerAccount;
    public string PayeeAccount { get; set; } = payeeAccount;
    public Int64 Amount { get; set; } = amount;
    public Int64 Fee { get; set; } = 0;
    public string? Description { get; set; }
    public string? CustomerKey { get; set; }
    public string OrganizationId { get; set; } = organizationId;
}