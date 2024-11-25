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
            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:29092",
                GroupId = GROUP_ID,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                // Debug = "all"
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Consumer started for group id {GROUP_ID} assigned to partition {string.Join(", ", partitions)}");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Consumer revoked for group id {GROUP_ID} and topic {string.Join(", ", partitions)}");
                })
                .Build();

            consumer.Subscribe(TOPIC);

            Console.WriteLine($"Consumer starting for group id {GROUP_ID} and topic {TOPIC}");

            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}