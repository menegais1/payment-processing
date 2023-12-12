using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentProcessing;

public interface IAsyncPaymentTransactionPublisherQueue : IDisposable
{
    public bool PublishMessage(PaymentTransactionMessagePayload message);
}