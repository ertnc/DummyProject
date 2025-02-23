using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace DummyProject.BackgroundJob
{
    public class MailSenderBackgroundService : BackgroundService
    {
        private readonly IConfiguration _config;

        public MailSenderBackgroundService(IConfiguration config)
        {
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = _config["RabbitMQ:HostName"] };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "SendMail", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Log.Information("Mail gönderim kuyruğundan alındı: {Message}", message);
                // Burada gerçek mail gönderimi yapılabilir (SMTP vb.)
                Console.WriteLine($"Mail gönderildi: {message}");
            };

            channel.BasicConsume(queue: "SendMail", autoAck: true, consumer: consumer);

            Log.Information("MailSenderBackgroundService başlatıldı");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
