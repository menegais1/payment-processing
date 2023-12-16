namespace PaymentProcessing;

public enum PaymentTransactionTaskType
{
    Process,
}

public record PaymentTransactionMessagePayload(PaymentTransactionTaskType TaskType, string MessageBody)
{
    public static string Serialize(object obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    public static PaymentTransactionMessagePayload? Deserialize(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<PaymentTransactionMessagePayload>(json);
    }
}