using System;
using Confluent.Kafka;

namespace producer
{
    class Program
    {
        static readonly string TOPIC = "test-topic";
        static void Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "kafka:29092" };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            for (int i = 0; i < 10; i++)
            {
                var message = $"Hello, Kafka! Message number {i + 1}";
                producer.ProduceAsync(TOPIC, new Message<Null, string> { Value = message });
                producer.Flush();
                Console.WriteLine($"Message {i + 1} sent!");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
