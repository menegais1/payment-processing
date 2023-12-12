namespace PaymentProcessing.Schemas;

public class CreatePaymentTransactionRequest
{
    public string PayerAccount { get; set; }
    public string PayeeAccount { get; set; }
    public double Amount { get; set; }
}