using System;
using Confluent.Kafka;

namespace consumer
{
    class Program
    {
        static readonly string TOPIC = "test-topic";
        static readonly string GROUP_ID = "my-consumer-group";
        static void Main(string[] args)
        {
            var config = new ConsumerConfig { BootstrapServers = "kafka:29092", GroupId = GROUP_ID };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(TOPIC);

            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"Received message: {consumeResult.Message.Value}");
            }
        }
    }
}