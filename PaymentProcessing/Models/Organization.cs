namespace PaymentProcessing;

public class Organization(string id, string name, string secretKey)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string SecretKey { get; set; } = secretKey;

    public List<Transaction> Transactions { get; set; }
}