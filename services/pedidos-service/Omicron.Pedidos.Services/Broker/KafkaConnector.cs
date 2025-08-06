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
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

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
        public async Task<bool> PushMessage(object messaje, string queueType, string logbase = null)
        {
            logbase = string.IsNullOrEmpty(logbase) ? "Pedidos - Push Message" : logbase;
            try
            {
                this.GetKafkaConfiguration(queueType);
                using var localProducer = new ProducerBuilder<long, string>(this.producer).SetKeySerializer(Serializers.Int64).SetValueSerializer(Serializers.Utf8).Build();
                await localProducer.ProduceAsync(this.topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = JsonConvert.SerializeObject(messaje) });
                this.logger.Information("{LogBase} - Object sent to {Topic}: {Message}", logbase, this.topic, JsonConvert.SerializeObject(messaje));
                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error(
                    ex,
                    "{LogBase} - Error sending to kafka, {Topic}: {Message} - {StackTrace}",
                    logbase,
                    this.topic,
                    JsonConvert.SerializeObject(messaje),
                    ex.StackTrace);
                return false;
            }
        }

        private void GetKafkaConfiguration(string queueType)
        {
            this.topic = this.configuration[$"kafka:{queueType}:EH_NAME"];
            this.producer = new ProducerConfig
            {
                BootstrapServers = this.configuration[$"kafka:{queueType}:EH_FQDN"],
            };

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (ServiceUtils.CalculateOr(environment == "Uat", environment == "Prod"))
            {
                this.producer.SecurityProtocol = SecurityProtocol.SaslSsl;
                this.producer.SaslMechanism = SaslMechanism.Plain;
                this.producer.SaslUsername = "$ConnectionString";
                this.producer.SaslPassword = this.configuration[$"kafka:{queueType}:EH_CONNECTION_STRING"];
                this.producer.SslCaLocation = string.Empty;
                this.producer.SocketKeepaliveEnable = true;
                this.producer.MetadataMaxAgeMs = 180000;
            }
            else
            {
                this.producer.SecurityProtocol = SecurityProtocol.Plaintext;
                this.producer.SocketKeepaliveEnable = true;
                this.producer.MetadataMaxAgeMs = 180000;
            }
        }
    }
}
