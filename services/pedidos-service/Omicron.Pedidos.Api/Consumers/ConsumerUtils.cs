// <summary>
// <copyright file="ConsumerUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Consumers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// ConsumerUtils.
    /// </summary>
    public static class ConsumerUtils
    {
        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <param name="configuration">The config.</param>
        /// <param name="queueName">the queue name.</param>
        /// <returns>the data.</returns>
        public static ConsumerConfig GetConsumerConfig(IConfiguration configuration, string queueName)
        {
            var consumerConfigByEnviorment = BuildConfigDictionary(configuration, queueName);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return consumerConfigByEnviorment.GetValueOrDefault(
                environment,
                new ConsumerConfig
                {
                    BootstrapServers = configuration[$"kafka:{queueName}:EH_FQDN"],
                    GroupId = configuration[$"kafka:{queueName}:CONSUMER_GROUP"],
                    SocketTimeoutMs = 60000,
                    SessionTimeoutMs = 30000,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    BrokerVersionFallback = "1.0.0",
                    SecurityProtocol = SecurityProtocol.Plaintext,
                    SocketKeepaliveEnable = true,
                    EnableAutoCommit = false,
                    MetadataMaxAgeMs = 180000,
                });
        }

        private static Dictionary<string, ConsumerConfig> BuildConfigDictionary(IConfiguration configuration, string queueName) =>
                new Dictionary<string, ConsumerConfig>
                {
                    {
                        "Prod", new ConsumerConfig
                        {
                            BootstrapServers = configuration[$"kafka:{queueName}:EH_FQDN"],
                            GroupId = configuration[$"kafka:{queueName}:CONSUMER_GROUP"],
                            SocketTimeoutMs = 60000,
                            SessionTimeoutMs = 30000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            BrokerVersionFallback = "1.0.0",
                            SecurityProtocol = SecurityProtocol.SaslSsl,
                            SaslMechanism = SaslMechanism.Plain,
                            SaslUsername = "$ConnectionString",
                            SaslPassword = configuration[$"kafka:{queueName}:EH_CONNECTION_STRING"],
                            SslCaLocation = null,
                            SocketKeepaliveEnable = true,
                            EnableAutoCommit = false,
                            MetadataMaxAgeMs = 180000,
                            Debug = "all",
                        }
                    },
                    {
                        "Uat", new ConsumerConfig
                        {
                            BootstrapServers = configuration[$"kafka:{queueName}:EH_FQDN"],
                            GroupId = configuration[$"kafka:{queueName}:CONSUMER_GROUP"],
                            SocketTimeoutMs = 60000,
                            SessionTimeoutMs = 30000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            BrokerVersionFallback = "1.0.0",
                            SecurityProtocol = SecurityProtocol.SaslSsl,
                            SaslMechanism = SaslMechanism.Plain,
                            SaslUsername = "$ConnectionString",
                            SaslPassword = configuration[$"kafka:{queueName}:EH_CONNECTION_STRING"],
                            SslCaLocation = null,
                            SocketKeepaliveEnable = true,
                            EnableAutoCommit = false,
                            MetadataMaxAgeMs = 180000,
                            Debug = "all",
                        }
                    },
                    {
                        "Staging", new ConsumerConfig
                        {
                            BootstrapServers = configuration[$"kafka:{queueName}:EH_FQDN"],
                            GroupId = configuration[$"kafka:{queueName}:CONSUMER_GROUP"],
                            SocketTimeoutMs = 60000,
                            SessionTimeoutMs = 30000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            BrokerVersionFallback = "1.0.0",
                            SecurityProtocol = SecurityProtocol.Plaintext,
                            SocketKeepaliveEnable = true,
                            EnableAutoCommit = false,
                            MetadataMaxAgeMs = 180000,
                        }
                    },
                    {
                        "Development", new ConsumerConfig
                        {
                            BootstrapServers = configuration[$"kafka:{queueName}:EH_FQDN"],
                            GroupId = configuration[$"kafka:{queueName}:CONSUMER_GROUP"],
                            SocketTimeoutMs = 60000,
                            SessionTimeoutMs = 30000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            BrokerVersionFallback = "1.0.0",
                            SecurityProtocol = SecurityProtocol.Plaintext,
                            SocketKeepaliveEnable = true,
                            EnableAutoCommit = false,
                            MetadataMaxAgeMs = 180000,
                        }
                    },
                };
    }
}
