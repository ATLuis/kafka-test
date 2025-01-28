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

            // var config = new ProducerConfig
            // {
            //     // BootstrapServers = "localhost:9092,localhost:9093",
            //     // BootstrapServers = "kafka1:29092,kafka2:29093",
            //     BootstrapServers = "kafka2:29093",
            //     // BootstrapServers = "kafka1:29092",
            //     Acks = Acks.All,
            //     MessageSendMaxRetries = 3,
            //     RetryBackoffMs = 1000,
            //     SocketTimeoutMs = 60000,
            //     MessageTimeoutMs = 60000
            // };

            var config = new ProducerConfig { 
                BootstrapServers = "localhost:9092,localhost:9093", 
                    // BootstrapServers = "kafka1:29092", 
                // BootstrapServers = "kafka2:29093", 
                // BootstrapServers = "kafka1:29092", 
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
