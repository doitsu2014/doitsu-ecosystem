using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Settings;
using Shared.Validations;
using static LanguageExt.Prelude;
using Some = LanguageExt.Some;

namespace Shared.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<KafkaService> _logger;
        private readonly KafkaSettings _settings;

        public KafkaService(ILogger<KafkaService> logger, IOptions<KafkaSettings> kafkaSettingsOption)
        {
            this._logger = logger;
            this._settings = kafkaSettingsOption.Value;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceMessagesAsync<TKey, TValue>(string topic,
            Message<TKey, TValue> kafkaMessage,
            ISerializer<TKey> keySerializer = default,
            ISerializer<TValue> valueSerializer = default)
        {
            using var producer = new ProducerBuilder<TKey, TValue>(_settings.ProducerConfig).SetKeySerializer(keySerializer)
                .SetValueSerializer(valueSerializer)
                .Build();

            return await producer.ProduceAsync(topic, kafkaMessage);
        }

        public async Task<List<DeliveryResult<TKey, TValue>>> ProduceMessagesAsync<TKey, TValue>(string topic,
            IEnumerable<Message<TKey, TValue>> kafkaMessages,
            ISerializer<TKey> keySerializer = default,
            ISerializer<TValue> valueSerializer = default)
        {
            using var producer = new ProducerBuilder<TKey, TValue>(_settings.ProducerConfig).SetKeySerializer(keySerializer)
                .SetValueSerializer(valueSerializer)
                .Build();

            var result = new List<DeliveryResult<TKey, TValue>>();
            foreach (var kafkaMessage in kafkaMessages)
            {
                result.Add(await producer.ProduceAsync(topic, kafkaMessage));
            }

            return result;
        }

        public async Task<Either<Seq<string>, Unit>> DeleteTopicsAsync(params string[] topics)
        {
            using var adminClient = new AdminClientBuilder(_settings.AdminClientConfig).Build();
            return await StringValidator.ShouldNotNullOrEmpty(topics, $"{nameof(topics)} should not null or empty")
                .Map(req => (tp: req, ac: adminClient))
                .Map((req) =>
                {
                    // Prepare data
                    var listExistedTopics = req
                        .ac
                        .GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic)
                        .ToImmutableList();
                    // Warning existed Topics
                    var listNotExistedTopics = req.tp.Where(t => !listExistedTopics.Contains(t));
                    foreach (var notExistedTopic in listNotExistedTopics)
                    {
                        _logger.LogWarning("{ServiceName} could not delete {topicName}, because it does not exist.",
                            nameof(DeleteTopicsAsync),
                            notExistedTopic);
                    }

                    return (availableTopics: req.tp.Where(t => listExistedTopics.Contains(t)).ToList(), req.ac);
                })
                .ToEither()
                .MapAsync(async req =>
                {
                    // return TryAsync(async () => );
                    await req.ac.DeleteTopicsAsync(req.availableTopics);
                    _logger.LogInformation("{ServiceName} deleted {topics}", nameof(DeleteTopicsAsync), req.availableTopics);
                    return unit;
                });
        }

        public async Task<Either<Seq<string>, Unit>> CreateTopicsAsync(string[] topics, int numberOfPartition = 1, short replicationFactor = 1)
        {
            using var adminClient = new AdminClientBuilder(_settings.AdminClientConfig).Build();
            return await (from listTopics in StringValidator.ShouldNotNullOrEmpty(topics, $"{nameof(topics)} should not null or empty")
                    from nop in Success<string, int>(numberOfPartition)
                    from rf in Success<string, short>(replicationFactor)
                    select (listTopics, nop, rf, ac: adminClient))
                .ToEither()
                .Map((req) =>
                {
                    // Prepare data
                    var listTopicSpecifications = req.listTopics.Select(t => new TopicSpecification()
                    {
                        Name = t,
                        NumPartitions = req.nop,
                        ReplicationFactor = req.rf
                    }).ToImmutableList();
                    var listExistedTopicOnServer = req.ac.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic);

                    // Warning existed Topics
                    var listExistedTopicInParams = listTopicSpecifications.Where(t => listExistedTopicOnServer.Contains(t.Name)).Select(t => t.Name);
                    foreach (var existedTopic in listExistedTopicInParams)
                    {
                        _logger.LogWarning("{ServiceName} could not create {topicName}, because it already existed.",
                            nameof(CreateTopicsAsync),
                            existedTopic);
                    }

                    var listAvailableTopics = listTopicSpecifications.Where(t => !listExistedTopicOnServer.Contains(t.Name)).ToList();
                    return (listAvailableTopics, req.ac);
                })
                .MapAsync(async req =>
                {
                    // Create available Topics
                    await req.ac.CreateTopicsAsync(req.listAvailableTopics);
                    foreach (var topic in req.listAvailableTopics)
                    {
                        _logger.LogInformation("{ServiceName} created {topicName} with {numberOfPartition} partitions and {replicationFactor} replicationFactor.",
                            nameof(CreateTopicsAsync),
                            topic.Name,
                            topic.NumPartitions,
                            topic.ReplicationFactor);
                    }

                    return unit;
                });
        }
    }
}