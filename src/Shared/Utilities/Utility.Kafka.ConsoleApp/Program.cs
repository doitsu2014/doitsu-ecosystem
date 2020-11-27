using System;
using System.Net;
using Confluent.Kafka;
using Utility.Extensions;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utility.Kafka.ConsoleApp
{

    public class StudentSerializer : ISerializer<Student>, IDeserializer<Student>
    {
        public Student Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<Student>(data);
        }

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
        public static async Task Main(string[] args)
        {
            var student = new Student()
            {
                Id = Guid.NewGuid(),
                Name = "Doitsu2014",
                Gpa = (float)10.0
            };

            var boostrapServers = "localhost:9093";
            var config = new ProducerConfig()
            {
                BootstrapServers = boostrapServers,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "user",
                SaslPassword = "zaQ@1234",
                SslKeystoreLocation = "Configurations/kafka.truststore.jks",
                SslKeystorePassword = "zaQ@1234",
                SecurityProtocol = SecurityProtocol.SaslSsl 
            };

            var builder = new ProducerBuilder<string, Student>(config);
            builder.SetValueSerializer(new StudentSerializer());
            using (var producer = builder.Build())
            {
                var result = await producer.ProduceAsync("StudentAdded", new Message<string, Student> { Key = student.Id.ToString(), Value = student });
            }

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9093",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            var consumerBuilder = new ConsumerBuilder<string, Student>(consumerConfig);
            consumerBuilder.SetValueDeserializer(new StudentSerializer());
            using (var consumer = consumerBuilder.Build())
            {
                consumer.Subscribe(new string[] { "StudentAdded" });
                var consumeResult = consumer.Consume();
                // System.Console.WriteLine(JsonSerializer.Serialize(consumeResult));
                consumer.Close();
            }


            // config.ProduceMessage<string, Student>("StudentAdded", new Message<string, Student>{Key = student.Id.ToString() ,Value = student}, valueSerializer: new StudentSerializer())
            //     .Subscribe(
            //         res => 
            //         {
            //             var config = new ConsumerConfig
            //             {
            //                 BootstrapServers = "kafka-broker.doitsu.tech:9092",
            //                 GroupId = "foo",
            //                 AutoOffsetReset = AutoOffsetReset.Earliest
            //             };
            //             using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            //             {
            //                 consumer.Subscribe(new string[] { "weblog2" });
            //                 var consumeResult = consumer.Consume();
            //                 // System.Console.WriteLine(JsonSerializer.Serialize(consumeResult));
            //                 consumer.Close();
            //             }
            //         },
            //         error => System.Console.WriteLine(error.Message));
        }
    }
}
