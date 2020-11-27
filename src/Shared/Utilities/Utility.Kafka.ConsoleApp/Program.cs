using System.Net.Http.Headers;
using System;
using System.Net;
using Confluent.Kafka;
using Utility.Extensions;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Confluent.Kafka.Admin;

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
            // var students = new List<Student>();
            // for (var i = 0; i < 2; ++i)
            //     students.Add(new Student()
            //     {
            //         Id = Guid.NewGuid(),
            //         Name = (Faker.Name.FullName()),
            //         Gpa = Faker.RandomNumber.Next(1, 10)
            //     });

            var bootstrapServers = "localhost:9093";

            var adminConfig = new AdminClientConfig()
            {
                BootstrapServers = bootstrapServers
            };
            var adminBuilder = new AdminClientBuilder(adminConfig);
            using (var adminClient = adminBuilder.Build())
            {
                try
                {
                    (adminClient.DeleteTopicsAsync(new string[] { "topicname3" })).GetAwaiter().GetResult();
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
                            ReplicationFactor = 2,
                            NumPartitions = 2,
                            Name = "topicname3"
                        }
                    }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

                try
                {
                    adminClient.CreatePartitionsAsync(new PartitionsSpecification[] {
                        new PartitionsSpecification()
                        {
                            IncreaseTo = 4,
                            Topic = "topicname3"
                        }
                    }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Partitioner = Partitioner.Random
            };



            var builder = new ProducerBuilder<Null, string>(producerConfig);
            // builder.SetValueSerializer(new StudentSerializer());
            using (var producer = builder.Build())
            {
                for (var i = 0; i < 10; ++i)
                {
                    try
                    {
                        var message = new Message<Null, string> { Value = "ducth" };
                        var result = await producer.ProduceAsync($"topicname3", message);
                        System.Console.WriteLine($"produce successfully message {message.Value} to partition {result.TopicPartition}");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);

                    }
                }
            }

            // var consumerConfig = new ConsumerConfig
            // {
            //     BootstrapServers = bootstrapServers,
            //     GroupId = "foo",
            //     AutoOffsetReset = AutoOffsetReset.Earliest
            // };

            // var consumerBuilder = new ConsumerBuilder<string, Student>(consumerConfig);
            // consumerBuilder.SetValueDeserializer(new StudentSerializer());
            // using (var consumer = consumerBuilder.Build())
            // {
            //     consumer.Subscribe(new string[] { "StudentAdded" });
            //     ConsumeResult<string, Student> consumeResult;
            //     do
            //     {
            //         consumeResult = consumer.Consume();
            //         System.Console.WriteLine(consumeResult.Message.Value.Name);
            //     }
            //     while (consumeResult != null);
            //     consumer.Close();
            // }

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
