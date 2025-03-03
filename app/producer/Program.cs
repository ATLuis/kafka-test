using System;
using System.Threading;
using Confluent.Kafka;

namespace producer
{
    class Program
    {
        static readonly string BootstrapServers 
            = "kafka1:29092,kafka2:29093"; // inside Docker
            // = "localhost:9092,localhost:9093"; //outside Docker
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the topic name you want to produce events for:");
            string TOPIC = Console.ReadLine();

            var config = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
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

                int maxRetries = 5;
                int attempt = 0;
                bool sent = false;

                while (attempt < maxRetries && !sent)
                {
                    attempt++;
                    try
                    {
                        var deliveryReport = producer.ProduceAsync(TOPIC, new Message<Null, string> { Value = input }).Result;
                        Console.WriteLine($"Delivered status: '{deliveryReport.Status}'.");
                        sent = true; // Jika berhasil, keluar dari loop retry
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error producing message: {e.Message}");
                        if (attempt < maxRetries)
                        {
                            Console.WriteLine($"Retrying in 5 seconds... (Attempt {attempt}/{maxRetries})");
                            Thread.Sleep(5000); // Tunggu 5 detik sebelum retry
                        }
                        else
                        {
                            Console.WriteLine("Max retry reached. Skipping message.");
                        }
                    }
                }

                producer.Flush();
            }
        }
    }
}
