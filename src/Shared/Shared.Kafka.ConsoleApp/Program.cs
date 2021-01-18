﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bogus;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Settings;
using Shared.ConsoleApp.Extension;
using Shared.Services;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Some = LanguageExt.Some;

namespace Shared.Kafka.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args) => await ServiceProviderFactory.GetServiceProvider(
            addCustomServices: (opt, conf) =>
            {
                opt.Configure<KafkaSettings>(conf.GetSection("KafkaSettings"));
                opt.AddScoped<IKafkaService, KafkaService>();
            },
            jsonFilePath: "Configurations/appSettings.json",
            assembly: Assembly.GetAssembly(typeof(Program))
        ).MatchAsync(
            async sp =>
            {
                using var scope = sp.CreateScope();
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                var settings = scope.ServiceProvider.GetService<IOptions<KafkaSettings>>()?.Value;
                var kafkaService = scope.ServiceProvider.GetService<IKafkaService>();
                var studentGenerator = StudentFaker.GetFaker();
                var topicName = "studentAdded";

                await kafkaService.DeleteTopicsAsync(new string[] {topicName});
                await kafkaService.CreateTopicsAsync(new string[] {topicName}, 3, 3);

                var listMessages = List<int>().AddRange(Enumerable.Range(0, 20))
                    .Map((_) => new Message<string, Student>()
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = studentGenerator.Generate()
                    });

                var produceResults = await kafkaService.ProduceMessagesAsync(topicName, listMessages, valueSerializer: new StudentSerializer());
                foreach (var produceResult in produceResults)
                {
                    System.Console.WriteLine($"Produces successfully message {produceResult.Message.Value.Id} to partition {produceResult.TopicPartition}");
                }

                var consumerBuilder = new ConsumerBuilder<string, Student>(settings.ConsumerConfig);
                consumerBuilder.SetValueDeserializer(new StudentSerializer());
                using (var consumer = consumerBuilder.Build())
                {
                    consumer.Subscribe(new string[] {topicName});
                    ConsumeResult<string, Student> consumeResult;

                    do
                    {
                        consumeResult = consumer.Consume();
                        System.Console.WriteLine($"Consumes successfully message {consumeResult.Message.Key} from partition {consumeResult.TopicPartition}");
                    } while (consumeResult != null);

                    consumer.Close();
                }

                return true;
            }, () => false);
    }
}