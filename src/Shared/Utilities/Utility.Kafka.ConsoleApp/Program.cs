using System.Text.RegularExpressions;
using System;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka.Admin;
using Utility.Settings;
using System.Collections.Generic;
using Utility.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Utility.Interfaces;

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
            var serviceProvider = ServiceProviderFactory.GetServiceProvider(args);
            using (var scope = serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                var settings = scope.ServiceProvider.GetService<IOptions<KafkaSettings>>().Value;
                var kafkaService = scope.ServiceProvider.GetService<IKafkaService>();

                var topicName = "studentAdded";
                await kafkaService.DeleteTopicsAsync(new string[] { topicName });
                await kafkaService.CreateTopicsAsync(new string[] { topicName },
                                                     3,
                                                     3);

                var listMessages = new List<Message<string, Student>>();
                for (var i = 0; i < 10; ++i)
                {
                    listMessages.Add((x: Guid.NewGuid(), y: new Message<string, Student>())
                        .Map(req =>
                        {
                            req.y.Key = req.x.ToString();
                            req.y.Value = new Student()
                            {
                                Id = req.x,
                                Gpa = Faker.RandomNumber.Next(0, 10),
                                Name = Faker.Name.FullName()
                            };
                            return req.y;
                        }));
                }

                var produceResults = await kafkaService.ProduceMessagesAsync(topicName, listMessages, valueSerializer: new StudentSerializer());
                foreach (var produceResult in produceResults)
                {
                    System.Console.WriteLine($"produce successfully message {produceResult.Message.Value.Id} to partition {produceResult.TopicPartition}");
                }
            }


            // var produceResults = await settings.ProducerConfig.ProduceMessagesAsync(topicName,
            //                                                                        listMessages,
            //                                                                        valueSerializer: new StudentSerializer());


            // var builder = new ProducerBuilder<string, Student>(settings.ProducerConfig);
            // builder.SetValueSerializer(new StudentSerializer());
            // using (var producer = builder.Build())
            // {
            //     foreach (var student in listStudents)
            //     {
            //         try
            //         {
            //             var message = new Message<string, Student> { Key = student.Id.ToString(), Value = student };
            //             var result = await producer.ProduceAsync($"{topicName}", message);
            //             System.Console.WriteLine($"produce successfully message {message.Value.Id} to partition {result.TopicPartition}");
            //         }
            //         catch (Exception ex)
            //         {
            //             System.Console.WriteLine(ex.Message);

            //         }
            //     }
            // }

            // var consumerBuilder = new ConsumerBuilder<string, Student>(settings.ConsumerConfig);
            // consumerBuilder.SetValueDeserializer(new StudentSerializer());
            // using (var consumer = consumerBuilder.Build())
            // {
            //     consumer.Subscribe(new string[] { topicName });
            //     ConsumeResult<string, Student> consumeResult;
            //     do
            //     {
            //         consumeResult = consumer.Consume();
            //         System.Console.WriteLine($"consume successfully message {consumeResult.Message.Key} from partition {consumeResult.TopicPartition}");
            //     }
            //     while (consumeResult != null);
            //     consumer.Close();
            // }
        }
    }
}
