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

            while (true)
            {
                Console.WriteLine("Enter your message (type 'exit' to quit):");
                var input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }

                producer.Produce(TOPIC, new Message<Null, string> { Value = input });
                producer.Flush();
                Console.WriteLine("Message sent!");
            }
        }
    }
}
