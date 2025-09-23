// <summary>
// <copyright file="KafkaConnector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Broker.Impl
{
    /// <summary>
    /// Class for kafka connectors.
    /// </summary>
    public class KafkaConnector : IKafkaConnector
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private ProducerConfig producer;
        private string topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConnector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public KafkaConnector(IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration.ThrowIfNull(nameof(configuration));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<bool> PushMessage(object message, string logBase)
        {
            try
            {
                this.ConfigureRetryFinalizeOrderKafka();
                using var localProducer = new ProducerBuilder<long, string>(this.producer)
                    .SetKeySerializer(Serializers.Int64)
                    .SetValueSerializer(Serializers.Utf8)
                    .Build();

                var kafkaMessage = new Message<long, string>
                {
                    Key = DateTime.UtcNow.Ticks,
                    Value = JsonConvert.SerializeObject(message),
                };

                await localProducer.ProduceAsync(this.topic, kafkaMessage);

                this.logger.Information(
                    "{LogBase} - Object sent to {Topic}: {Message}",
                    logBase,
                    this.topic,
                    JsonConvert.SerializeObject(message));

                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error(
                    ex,
                    "{LogBase} - Error sending to Kafka {Topic}: {Message} - {StackTrace}",
                    logBase,
                    this.topic,
                    JsonConvert.SerializeObject(message),
                    ex.StackTrace);

                return false;
            }
        }

        /// <summary>
        /// Configures the Kafka producer using the RetryFinalizeProductionOrder settings.
        /// </summary>
        private void ConfigureRetryFinalizeOrderKafka()
        {
            // Sección de settings de RetryFinalizeProductionOrder
            var section = this.configuration.GetSection("RetryFinalizeProductionOrder");

            this.topic = section["EH_NAME"];
            var bootstrapServers = section["EH_FQDN"];
            var connectionString = section["EH_CONNECTION_STRING"];

            this.producer = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                SocketKeepaliveEnable = true,
                MetadataMaxAgeMs = 180000,
            };

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment is "Uat" or "Prod")
            {
                this.producer.SecurityProtocol = SecurityProtocol.SaslSsl;
                this.producer.SaslMechanism = SaslMechanism.Plain;
                this.producer.SaslUsername = "$ConnectionString";
                this.producer.SaslPassword = connectionString;
                this.producer.SslCaLocation = null;
            }
            else
            {
                this.producer.SecurityProtocol = SecurityProtocol.Plaintext;
            }
        }
    }
}
