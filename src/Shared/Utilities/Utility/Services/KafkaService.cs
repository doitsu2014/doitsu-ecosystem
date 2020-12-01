using System.Security.Cryptography.X509Certificates;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility.Interfaces;
using Confluent.Kafka;
using System.Linq;
using Confluent.Kafka.Admin;
using Utility.Settings;
using Microsoft.Extensions.Options;

namespace Utility.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<KafkaService> logger;
        private readonly KafkaSettings settings;

        public KafkaService(ILogger<KafkaService> logger, IOptions<KafkaSettings> kafkaSettingsOption)
        {
            this.logger = logger;
            this.settings = kafkaSettingsOption.Value;
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceMessagesAsync<TKey, TValue>(string topic,
                                                                                           Message<TKey, TValue> kafkaMessage,
                                                                                           ISerializer<TKey> keySerializer = default,
                                                                                           ISerializer<TValue> valueSerializer = default)
        {
            using (var producer = new ProducerBuilder<TKey, TValue>(settings.ProducerConfig).SetKeySerializer(keySerializer)
                                                                          .SetValueSerializer(valueSerializer)
                                                                          .Build())
            {
                return await producer.ProduceAsync(topic, kafkaMessage);
            }
        }

        public async Task<List<DeliveryResult<TKey, TValue>>> ProduceMessagesAsync<TKey, TValue>(string topic,
                                                                                                 IEnumerable<Message<TKey, TValue>> kafkaMessages,
                                                                                                 ISerializer<TKey> keySerializer = default,
                                                                                                 ISerializer<TValue> valueSerializer = default)
        {
            using (var producer = new ProducerBuilder<TKey, TValue>(settings.ProducerConfig).SetKeySerializer(keySerializer)
                                                                          .SetValueSerializer(valueSerializer)
                                                                          .Build())
            {
                var result = new List<DeliveryResult<TKey, TValue>>();
                foreach (var kafkaMessage in kafkaMessages)
                {
                    result.Add(await producer.ProduceAsync(topic, kafkaMessage));
                }
                return result;
            }
        }

        public async Task DeleteTopicsAsync(params string[] topics)
        {
            using (var adminClient = new AdminClientBuilder(settings.AdminClientConfig).Build())
            {
                // Prepare data
                var existedTopicNames = adminClient.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic);

                // Warning existed Topics
                var listDoesNotExistTopicNames = topics.Where(t => !existedTopicNames.Contains(t));
                foreach (var notExistTopic in listDoesNotExistTopicNames)
                {
                    logger.LogWarning("{ServiceName} could not delete {topicName}, because it does not exist.",
                                          nameof(DeleteTopicsAsync),
                                          notExistTopic);
                }

                // Delete available
                var listAvailable = topics.Where(t => existedTopicNames.Contains(t)).ToList();
                await adminClient.DeleteTopicsAsync(listAvailable);
                logger.LogInformation("{ServiceName} deleted {topics}", nameof(DeleteTopicsAsync), listAvailable);
            }
        }

        public async Task CreateTopicsAsync(string[] topics, int numberOfPartition = 1, short replicationFactor = 1)
        {
            using (var adminClient = new AdminClientBuilder(settings.AdminClientConfig).Build())
            {
                // Prepare data
                var listTopics = topics.Select(t => new TopicSpecification()
                {
                    Name = t,
                    NumPartitions = numberOfPartition,
                    ReplicationFactor = replicationFactor
                });
                var existedTopicNames = adminClient.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Select(t => t.Topic);

                // Warning existed Topics
                var listExistedTopics = listTopics.Where(t => existedTopicNames.Contains(t.Name)).Select(t => t.Name);
                foreach (var existedTopic in listExistedTopics)
                {
                    logger.LogWarning("{ServiceName} could not create {topicName}, because it already existed.",
                                          nameof(CreateTopicsAsync),
                                          existedTopic);
                }

                // Create available Topics
                var listAvailableTopics = listTopics.Where(t => !existedTopicNames.Contains(t.Name)).ToList();
                await adminClient.CreateTopicsAsync(listAvailableTopics);
                foreach (var topic in listAvailableTopics)
                {
                    logger.LogInformation("{ServiceName} created {topicName} with {numberOfPartition} partitions and {replicationFactor} replicationFactor.",
                                          nameof(CreateTopicsAsync),
                                          topic.Name,
                                          topic.NumPartitions,
                                          topic.ReplicationFactor);
                }
            }
        }
    }
}