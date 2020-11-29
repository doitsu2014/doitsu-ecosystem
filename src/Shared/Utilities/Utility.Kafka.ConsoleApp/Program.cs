using System.Text.RegularExpressions;
using System;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka.Admin;
using Utility.Settings;
using System.Collections.Generic;

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
            var bootstrapServers = "localhost:19093,localhost:29093,localhost:39093";
            var settings = new KafkaSettings()
            {
                AdminClientConfig = new AdminClientConfig()
                {
                    BootstrapServers = bootstrapServers,
                    SslCaLocation = "Configurations/snakeoil-ca-1.crt",
                    SslCertificateLocation = "Configurations/kafkacat-ca1-signed.pem",
                    SslKeyLocation = "Configurations/kafkacat.client.key",
                    SslKeyPassword = "zaQ@1234",
                    SecurityProtocol = SecurityProtocol.Ssl
                },
                ConsumerConfig = new ConsumerConfig()
                {
                    BootstrapServers = bootstrapServers,
                    SslCaLocation = "Configurations/snakeoil-ca-1.crt",
                    SslCertificateLocation = "Configurations/kafkacat-ca1-signed.pem",
                    SslKeyLocation = "Configurations/kafkacat.client.key",
                    SslKeyPassword = "zaQ@1234",
                    SecurityProtocol = SecurityProtocol.Ssl,
                    GroupId = "group-default-01",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                },
                ProducerConfig = new ProducerConfig()
                {
                    BootstrapServers = bootstrapServers,
                    SslCaLocation = "Configurations/snakeoil-ca-1.crt",
                    SslCertificateLocation = "Configurations/kafkacat-ca1-signed.pem",
                    SslKeyLocation = "Configurations/kafkacat.client.key",
                    SslKeyPassword = "zaQ@1234",
                    SecurityProtocol = SecurityProtocol.Ssl,
                    Partitioner = Partitioner.Random
                }
            };
            var topicName = "studentAdded";
            var adminBuilder = new AdminClientBuilder(settings.AdminClientConfig);
            using (var adminClient = adminBuilder.Build())
            {
                try
                {
                    (adminClient.DeleteTopicsAsync(new string[] { topicName })).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

                try
                {
                    adminClient.CreateTopicsAsync(new TopicSpecification[] {
                        new TopicSpecification()
                        {
                            ReplicationFactor = 3,
                            NumPartitions = 3,
                            Name = topicName
                        }
                    }).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }


            var listStudents = new List<Student>();
            for (var i = 0; i < 10; ++i)
            {
                listStudents.Add(new Student()
                {
                    Id = Guid.NewGuid(),
                    Gpa = Faker.RandomNumber.Next(0, 10),
                    Name = Faker.Name.FullName()
                });
            }

            var builder = new ProducerBuilder<string, Student>(settings.ProducerConfig);
            builder.SetValueSerializer(new StudentSerializer());
            using (var producer = builder.Build())
            {
                foreach (var student in listStudents)
                {
                    try
                    {
                        var message = new Message<string, Student> { Key = student.Id.ToString(), Value = student };
                        var result = await producer.ProduceAsync($"{topicName}", message);
                        System.Console.WriteLine($"produce successfully message {message.Value.Id} to partition {result.TopicPartition}");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);

                    }
                }
            }

            var consumerBuilder = new ConsumerBuilder<string, Student>(settings.ConsumerConfig);
            consumerBuilder.SetValueDeserializer(new StudentSerializer());
            using (var consumer = consumerBuilder.Build())
            {
                consumer.Subscribe(new string[] { topicName });
                ConsumeResult<string, Student> consumeResult;
                do
                {
                    consumeResult = consumer.Consume();
                    System.Console.WriteLine($"consume successfully message {consumeResult.Message.Key} from partition {consumeResult.TopicPartition}");
                }
                while (consumeResult != null);
                consumer.Close();
            }
        }
    }
}
