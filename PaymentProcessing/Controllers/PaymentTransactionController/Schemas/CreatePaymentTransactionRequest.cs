namespace PaymentProcessing.Schemas;

public class CreatePaymentTransactionRequest
{
    public string PayerAccount { get; set; }
    public string PayeeAccount { get; set; }
    public Int64 Amount { get; set; }

    public Int64? Fee { get; set; } = 0;
    public string? Description { get; set; }
    public string? CustomerKey { get; set; }
}