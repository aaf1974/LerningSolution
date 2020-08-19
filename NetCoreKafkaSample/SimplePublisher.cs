using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreKafkaSample
{
    class SimplePublisher
    {
        public async Task SendMessageAsync()
        {
            ProducerConfig config = GetKafkaConfig();

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("CH.TEST", new Message<Null, string> { Value = "test" });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }


        public void SendMessage()
        {
            ProducerConfig config = GetKafkaConfig();

            Action<DeliveryReport<Null, string>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                for (int i = 0; i < 100; ++i)
                {
                    p.Produce("CH.TEST", new Message<Null, string> { Value = i.ToString() }, handler);
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }


        private static ProducerConfig GetKafkaConfig()
        {
            return new ProducerConfig 
            {
                BootstrapServers = "t-kafka.intellectika.local:9093",
                ClientId = Dns.GetHostName(),


                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = "kfkusr",
                SaslKerberosPrincipal = "kfkusr",
                SaslPassword = "Ovger123",
                EnableSslCertificateVerification = false,

                SaslMechanism = SaslMechanism.ScramSha256
            };
        }
    }
}
