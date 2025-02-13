using System;
using Confluent.Kafka;

namespace producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the topic name you want to produce events for:");
            string TOPIC = Console.ReadLine();

            /*
            Your Kafka brokers are actually running on localhost:9092 and localhost:9093, 
            but the consumer was trying to connect to "kafka1:29092,kafka2:29093". 
            These addresses are Docker-internal hostnames, meaning they can only be resolved inside the Docker network. 
            Since your consumer was running outside of Docker, it couldn’t find these broker addresses, which caused the connection issue.

            So, the correct solution depends on where the consumer is running:
            - Consumer inside Docker: Use BootstrapServers = "kafka1:29092,kafka2:29093".
            - Consumer outside Docker: Use BootstrapServers = "localhost:9092,localhost:9093"
            */

            var config = new ProducerConfig { 
                BootstrapServers = "kafka1:29092,kafka2:29093", // inside Docker
                // BootstrapServers = "localhost:9092,localhost:9093", //outside Docker
                Acks = Acks.All, 
                MessageSendMaxRetries = 3, 
                RetryBackoffMs = 1000, 
                SocketTimeoutMs = 60000, 
                MessageTimeoutMs = 60000, 
                MetadataMaxAgeMs = 5000 
            };

            using var producer = new ProducerBuilder<Null, string>(config)
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

            while (true)
            {
                Console.WriteLine("Enter your message (type 'exit' to quit):");
                var input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                try
                {
                    // producer.Produce(TOPIC, new Message<Null, string> { Value = input });
                    // producer.Flush();
                    var deliveryReport = producer.ProduceAsync(TOPIC, new Message<Null, string> { Value = input }).Result;
                    // Console.WriteLine($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}' with status {deliveryReport.Status} at '{deliveryReport.Timestamp}'.");
                    Console.WriteLine($"Delivered status: '{deliveryReport.Status}'.");
                    // if (deliveryReport.Status == PersistenceStatus.NotPersisted)
                    // {
                    //     Console.WriteLine("Message was not sent.");
                    // }
                    producer.Flush();
                    // Console.Write("Message sent! ");
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"Error producing message: {e.Message}");
                }
            }
        }
    }
}
