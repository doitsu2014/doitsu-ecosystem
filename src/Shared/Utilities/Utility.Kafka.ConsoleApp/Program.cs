using System;
using System.Net;
using Confluent.Kafka;
using Utility.Extensions;
using System.Reactive.Linq;
using System.Text.Json;

namespace Utility.Kafka.ConsoleApp
{

    public class StudentSerializer : ISerializer<Student>
    {
        public byte[] Serialize(Student data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

    public class Student 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gpa { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var student = new Student() {
                Id = Guid.NewGuid(),
                Name = "Doitsu2014",
                Gpa = (float)10.0
            };

            var config = new ClientConfig
            {
                BootstrapServers = "kafka-broker.doitsu.tech:9092",
                ClientId = Dns.GetHostName()
            };

            // using (var producer = new ProducerBuilder<string, Student>(config).Build())
            // {
            //     var a = producer.Name;
            // }

        

            config.ProduceMessage<string, Student>("StudentAdded", new Message<string, Student>{Key = student.Id.ToString() ,Value = student}, valueSerializer: new StudentSerializer())
                .Subscribe(
                    res => 
                    {
                        var config = new ConsumerConfig
                        {
                            BootstrapServers = "kafka-broker.doitsu.tech:9092",
                            GroupId = "foo",
                            AutoOffsetReset = AutoOffsetReset.Earliest
                        };
                        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                        {
                            consumer.Subscribe(new string[] { "weblog2" });
                            var consumeResult = consumer.Consume();
                            // System.Console.WriteLine(JsonSerializer.Serialize(consumeResult));
                            consumer.Close();
                        }
                    },
                    error => System.Console.WriteLine(error.Message));
        }
    }
}
