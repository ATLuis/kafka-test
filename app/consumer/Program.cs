using System;
using System.Linq;
using Confluent.Kafka;

namespace consumer
{
    class Program
    {
        static readonly string GROUP_ID = "my-consumer-group";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the topic name you want to consume:");
            string TOPIC = Console.ReadLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:29092",
                GroupId = GROUP_ID,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                // Debug = "all"
            };

            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "kafka:29092" }).Build();
            try
            {
                var topicExists = false;
                while (!topicExists)
                {
                    try
                    {
                        var metadata = adminClient.GetMetadata(TOPIC, TimeSpan.FromSeconds(10));
                        if (metadata.Topics.Any(t => t.Topic == TOPIC && t.Error.Code == ErrorCode.NoError))
                        {
                            topicExists = true;
                        }
                    }
                    catch (KafkaException ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            catch (KafkaException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return;
            }

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
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
                    if (consumeResult?.Message?.Value != null)
                    {
                        Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
    }
}