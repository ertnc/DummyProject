namespace DummyProject.Service
{
    public interface IMessageQueueService
    {
        void PublishToQueue(string queueName, string message);
    }
}
