// <summary>
// <copyright file="KafkaConnector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Broker
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// Class for kafka connectors.
    /// </summary>
    public class KafkaConnector : IKafkaConnector
    {
        private readonly IConfiguration configuration;

        private readonly ProducerConfig producer;

        private readonly ILogger logger;

        private readonly string topic;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConnector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public KafkaConnector(IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.topic = this.configuration["kafka:EH_NAME"];
            this.producer = new ProducerConfig
            {
                BootstrapServers = this.configuration["kafka:EH_FQDN"],
            };

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "Uat")
            {
                this.producer.SecurityProtocol = SecurityProtocol.SaslSsl;
                this.producer.SaslMechanism = SaslMechanism.Plain;
                this.producer.SaslUsername = "$ConnectionString";
                this.producer.SaslPassword = this.configuration["kafka:EH_CONNECTION_STRING"];
                this.producer.SslCaLocation = string.Empty;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> PushMessage(object messaje)
        {
            try
            {
                using (var localProducer = new ProducerBuilder<long, string>(this.producer).SetKeySerializer(Serializers.Int64).SetValueSerializer(Serializers.Utf8).Build())
                {
                    await localProducer.ProduceAsync(this.topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = JsonConvert.SerializeObject(messaje) });
                    this.logger.Information($"Object sent to {this.topic}: {JsonConvert.SerializeObject(messaje)}");
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error sending to kafka, {this.topic}: {JsonConvert.SerializeObject(messaje)} - {ex.Message}-{ex.StackTrace}");
                return false;
            }
        }
    }
}
