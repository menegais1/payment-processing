namespace PaymentProcessing;

public class Config
{
    public static readonly string JWT_SECRET_KEY =
        Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "12345678901233456789091234567831";

    public static readonly string RABBIT_MQ_HOST = Environment.GetEnvironmentVariable("RABBIT_MQ_HOST") ?? "rabbitmq";

    public static readonly int RABBIT_MQ_PORT =
        int.Parse(Environment.GetEnvironmentVariable("RABBIT_MQ_PORT") ?? "5672");

    public static readonly string RABBIT_MQ_USERNAME =
        Environment.GetEnvironmentVariable("RABBIT_MQ_USERNAME") ?? "guest";

    public static readonly string RABBIT_MQ_PASSWORD =
        Environment.GetEnvironmentVariable("RABBIT_MQ_PASSWORD") ?? "guest";

    public static readonly string MYSQL_HOST = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "mysql";
    public static readonly string MYSQL_USER = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
    public static readonly string MYSQL_PASSWORD = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "root";
    public static readonly string MYSQL_PORT = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";

    public static readonly string MYSQL_DATABASE =
        Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "payment_processing";

    public static readonly string CONNECTION_STRING =
        $"server={MYSQL_HOST};port={MYSQL_PORT};user={MYSQL_USER};password={MYSQL_PASSWORD};database={MYSQL_DATABASE}";
}