using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace PaymentProcessing;

public class RabbitMqAsyncPaymentTransactionQueue : IAsyncPaymentTransactionPublisherQueue,
    IAsyncPaymentTransactionConsumerQueue
{
    private readonly ConnectionFactory _rabbitmqFactory;
    private IConnection _rabbitmqConnection;
    private IModel _rabbitmqModel;

    public RabbitMqAsyncPaymentTransactionQueue()
    {
        _rabbitmqFactory = new ConnectionFactory
        {
            HostName = Config.RABBIT_MQ_HOST,
            Port = Config.RABBIT_MQ_PORT,
            UserName = Config.RABBIT_MQ_USERNAME,
            Password = Config.RABBIT_MQ_PASSWORD,
        };
    }

    public async Task Connect()
    {
        while (_rabbitmqConnection is null)
        {
            try
            {
                _rabbitmqConnection = _rabbitmqFactory.CreateConnection();
            }
            catch (BrokerUnreachableException e)
            {
                Console.Out.WriteLine("Unable to connect to rabbitMq, retrying");
                await Task.Delay(2000);
            }
        }

        _rabbitmqModel = _rabbitmqConnection.CreateModel();
        _rabbitmqModel.ExchangeDeclare("PaymentTransaction", ExchangeType.Topic);

        _rabbitmqModel.QueueDeclare("PaymentTransactionQueue", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        _rabbitmqModel.QueueBind("PaymentTransactionQueue", "PaymentTransaction", "payment.*");
    }

    private List<AsyncEventingBasicConsumer> _consumers = new List<AsyncEventingBasicConsumer>();

    public bool RegisterConsumer(Func<PaymentTransactionMessagePayload, Task<bool>> callback)
    {
        var consumer = new EventingBasicConsumer(_rabbitmqModel);
        // Create a consumer to listen for messages
        consumer.Received += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            PaymentTransactionMessagePayload? payload = PaymentTransactionMessagePayload.Deserialize(message);
            if (payload is null)
            {
                throw new Exception("Payload from queue is null. This is a critical failure");
            }

            var ret = await callback(payload);
            if (ret)
            {
                _rabbitmqModel.BasicAck(eventArgs.DeliveryTag, false);
            }
            else
            {
                _rabbitmqModel.BasicNack(eventArgs.DeliveryTag, false, true);
            }
        };

        // Start listening for messages on the PaymentTransaction topic
        _rabbitmqModel.BasicConsume(queue: "PaymentTransactionQueue",
            autoAck: false,
            consumer: consumer);
        return true;
    }

    public bool PublishMessage(PaymentTransactionMessagePayload message)
    {
        // Publish a sample message
        var serializedMessage = PaymentTransactionMessagePayload.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);
        _rabbitmqModel.BasicPublish(exchange: "PaymentTransaction",
            routingKey: "payment.message",
            basicProperties: null,
            body: body);
        return true;
    }

    public void Dispose()
    {
        _rabbitmqModel.Dispose();
        _rabbitmqConnection.Dispose();
        GC.SuppressFinalize(this);
    }
}