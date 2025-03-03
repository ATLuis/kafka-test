using System;
using System.Linq;
using System.Threading;
using Confluent.Kafka;

namespace consumer
{
    class Program
    {
        static readonly string GROUP_ID = "my-consumer-group";
        static readonly string BootstrapServers 
            = "kafka1:29092,kafka2:29093"; // inside Docker
            // = "localhost:9092,localhost:9093"; //outside Docker
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the topic name you want to consume:");
            string TOPIC = Console.ReadLine();

            var config = new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = GROUP_ID,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                SessionTimeoutMs = 10000,
                HeartbeatIntervalMs = 3000,
                MaxPollIntervalMs = 300000,
                MetadataMaxAgeMs = 5000,
                SocketKeepaliveEnable = true,
                ReconnectBackoffMs = 1000,
                ReconnectBackoffMaxMs = 10000
            };

            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = BootstrapServers }).Build();

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
                            Console.WriteLine($"Topic {TOPIC} exists.");
                        }
                    }
                    catch (KafkaException ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        Thread.Sleep(30000); // Retry setelah 30 detik
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
                    Console.WriteLine($"Consumer started for group id {GROUP_ID} assigned to partitions: {string.Join(", ", partitions)}");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Consumer revoked for group id {GROUP_ID} and partitions: {string.Join(", ", partitions)}");
                })
                .SetErrorHandler((_, e) =>
                {
                    Console.WriteLine($"Error: {e.Reason}");
                })
                .SetStatisticsHandler((_, json) =>
                {
                    Console.WriteLine($"Statistics: {json}");
                })
                .SetLogHandler((_, logMessage) =>
                {
                    Console.WriteLine($"Log: {logMessage.Message}");
                })
                .Build();

            consumer.Subscribe(TOPIC);

            Console.WriteLine($"Consumer starting for group id {GROUP_ID} and topic {TOPIC}");

            while (true)
            {
                ConsumeResult<Ignore, string> consumeResult = null;
                try
                {
                    consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));

                    if (consumeResult?.Message?.Value != null)
                    {
                        Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                        consumer.Commit(consumeResult);
                    }
                    else
                    {
                        Console.WriteLine("No message received in the last 10 seconds.");
                    }
                }
                catch (ConsumeException e)
                {
                    if (consumeResult != null)
                    {
                        consumer.Seek(consumeResult.TopicPartitionOffset);
                    }
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    Thread.Sleep(30000); // Retry setelah 30 detik
                }
                catch (Exception e)
                {
                    if (consumeResult != null)
                    {
                        consumer.Seek(consumeResult.TopicPartitionOffset);
                    }
                    Console.WriteLine($"Error occurred: {e.Message}");
                    Thread.Sleep(30000); // Retry setelah 30 detik
                }
            }
        }
    }
}