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

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                producer.Produce(TOPIC, new Message<Null, string> { Value = input });
                producer.Flush();
                Console.Write("Message sent! ");
            }
        }
    }
}
