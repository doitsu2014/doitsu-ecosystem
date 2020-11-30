using System;
using System.Reactive.Linq;
using Confluent.Kafka;

namespace Utility.Extensions
{
    public static class KafkaExtension
    {
        public static IObservable<DeliveryResult<TKey, TValue>> ProduceMessage<TKey, TValue>(this ProducerConfig config,
                                                                                             string topic,
                                                                                             Message<TKey, TValue> kafkaMessage,
                                                                                             ISerializer<TKey> keySerializer = default,
                                                                                             ISerializer<TValue> valueSerializer = default)
        {
            return Observable.Using(
                () =>
                {
                    var result = new ProducerBuilder<TKey, TValue>(config);
                    result.SetValueSerializer(valueSerializer);
                    result.SetKeySerializer(keySerializer);
                    return result.Build();
                },
                (producer) => Observable.FromAsync(async () => await producer.ProduceAsync(topic, kafkaMessage))
            );
        }

    }
}