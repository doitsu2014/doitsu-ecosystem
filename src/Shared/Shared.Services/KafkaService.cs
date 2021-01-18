using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Settings;

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

        public async Task DeleteTopicsAsync(params string[] topics)
        {
            using var adminClient = new AdminClientBuilder(_settings.AdminClientConfig).Build();
            // Prepare data
            var existedTopicNames = adminClient.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic);
            // Warning existed Topics
            var listDoesNotExistTopicNames = topics.Where(t => !existedTopicNames.Contains(t));
            foreach (var notExistTopic in listDoesNotExistTopicNames)
            {
                _logger.LogWarning("{ServiceName} could not delete {topicName}, because it does not exist.",
                    nameof(DeleteTopicsAsync),
                    notExistTopic);
            }
            // Delete available
            var listAvailable = topics.Where(t => existedTopicNames.Contains(t)).ToList();
            await adminClient.DeleteTopicsAsync(listAvailable);
            _logger.LogInformation("{ServiceName} deleted {topics}", nameof(DeleteTopicsAsync), listAvailable);
        }

        public async Task CreateTopicsAsync(string[] topics, int numberOfPartition = 1, short replicationFactor = 1)
        {
            using var adminClient = new AdminClientBuilder(_settings.AdminClientConfig).Build();
            
            // Prepare data
            var listTopics = topics.Select(t => new TopicSpecification()
            {
                Name = t,
                NumPartitions = numberOfPartition,
                ReplicationFactor = replicationFactor
            }).ToImmutableList();
            
            var existedTopicNames = adminClient.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic);
            // Warning existed Topics
            var listExistedTopics = listTopics.Where(t => existedTopicNames.Contains(t.Name)).Select(t => t.Name);
            foreach (var existedTopic in listExistedTopics)
            {
                _logger.LogWarning("{ServiceName} could not create {topicName}, because it already existed.",
                    nameof(CreateTopicsAsync),
                    existedTopic);
            }

            // Create available Topics
            var listAvailableTopics = listTopics.Where(t => !existedTopicNames.Contains(t.Name)).ToList();
            await adminClient.CreateTopicsAsync(listAvailableTopics);
            foreach (var topic in listAvailableTopics)
            {
                _logger.LogInformation("{ServiceName} created {topicName} with {numberOfPartition} partitions and {replicationFactor} replicationFactor.",
                    nameof(CreateTopicsAsync),
                    topic.Name,
                    topic.NumPartitions,
                    topic.ReplicationFactor);
            }
        }
    }
}