using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Shared.Abstraction.Interfaces.Services
{
    public interface IKafkaService
    {
        Task<DeliveryResult<TKey, TValue>> ProduceMessagesAsync<TKey, TValue>(string topic,
            Message<TKey, TValue> kafkaMessage,
            ISerializer<TKey> keySerializer = default,
            ISerializer<TValue> valueSerializer = default);

        Task<List<DeliveryResult<TKey, TValue>>> ProduceMessagesAsync<TKey, TValue>(string topic,
            IEnumerable<Message<TKey, TValue>> kafkaMessages,
            ISerializer<TKey> keySerializer = default,
            ISerializer<TValue> valueSerializer = default);

        Task DeleteTopicsAsync(params string[] topics);
        Task CreateTopicsAsync(string[] topics, int numberOfPartition = 1, short replicationFactor = 1);
    }
}