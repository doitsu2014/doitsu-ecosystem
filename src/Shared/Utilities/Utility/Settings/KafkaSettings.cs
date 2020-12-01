using Confluent.Kafka;

namespace Utility.Settings
{
    public class KafkaSettings
    {
        public ProducerConfig ProducerConfig { get; set; }
        public ConsumerConfig ConsumerConfig { get; set; }
        public AdminClientConfig AdminClientConfig { get; set; }
    }
}