using Confluent.Kafka;

namespace Shared.Abstraction.Settings
{
    public class KafkaSettings
    {
        public ProducerConfig ProducerConfig { get; set; }
        public ConsumerConfig ConsumerConfig { get; set; }
        public AdminClientConfig AdminClientConfig { get; set; }
    }
}
