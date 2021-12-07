using Confluent.Kafka;
using LanguageExt;
using Shared.LanguageExt.Models.Types;

namespace Shared.Kafka.Interfaces
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

        Task<Either<Seq<ErrorString>, Unit>> DeleteTopicsAsync(params string[] topics);
        Task<Either<Seq<ErrorString>, Unit>> CreateTopicsAsync(string[] topics, int numberOfPartition = 1, short replicationFactor = 1);
    }
}