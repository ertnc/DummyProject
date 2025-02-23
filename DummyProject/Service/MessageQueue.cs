using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;

namespace DummyProject.Service
{
    public class MessageQueueService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageQueueService(IConfiguration config)
        {
            var factory = new ConnectionFactory { HostName = config["RabbitMQ:HostName"] };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "SendMail", durable: true, exclusive: false, autoDelete: false);
        }

        public void PublishToQueue(string queueName, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: queueName, body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
