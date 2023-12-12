namespace PaymentProcessing;

public class Config
{
    public static readonly string JWT_SECRET_KEY = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "12345678901233456789091234567831";
    public static readonly string RABBIT_MQ_HOST = Environment.GetEnvironmentVariable("RABBIT_MQ_HOST") ?? "rabbitmq";
    public static readonly int RABBIT_MQ_PORT = int.Parse(Environment.GetEnvironmentVariable("RABBIT_MQ_PORT") ?? "5672");
    public static readonly string RABBIT_MQ_USERNAME = Environment.GetEnvironmentVariable("RABBIT_MQ_USERNAME") ?? "guest";
    public static readonly string RABBIT_MQ_PASSWORD = Environment.GetEnvironmentVariable("RABBIT_MQ_PASSWORD") ?? "guest";
}