namespace PaymentProcessing;

public class Config
{
    public static readonly string JWT_SECRET_KEY = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "12345678901233456789091234567831";
}