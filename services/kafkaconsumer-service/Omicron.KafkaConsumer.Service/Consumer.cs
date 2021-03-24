// <summary>
// <copyright file="Consumer.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.KafkaConsumer.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Newtonsoft.Json;
    using Omicron.KafkaConsumer.Entities.DbContext;
    using Omicron.KafkaConsumer.Entities.Model;

    /// <summary>
    /// The consumer class.
    /// </summary>
    public class Consumer
    {
        /// <summary>
        /// The consumer.
        /// </summary>
        /// <param name="brokerList">the broker.</param>
        /// <param name="connStr">the conn.</param>
        /// <param name="consumergroup">the consumer.</param>
        /// <param name="topic">the topic.</param>
        /// <param name="cacertlocation">the cert.</param>
        public static async Task ConsumerData(string brokerList, string connStr, string consumergroup, string topic, string cacertlocation)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SocketTimeoutMs = 60000,                //this corresponds to the Consumer config `request.timeout.ms`
                SessionTimeoutMs = 30000,
                GroupId = consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",        //Event Hubs for Kafka Ecosystems supports Kafka v1.0+, a fallback to an older API will fail

                //Security
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //SaslMechanism = SaslMechanism.Plain,
                //SaslUsername = "$ConnectionString",
                //SaslPassword = connStr,
                //SslCaLocation = cacertlocation,
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };

            using (var consumer = new ConsumerBuilder<long, string>(config).SetKeyDeserializer(Deserializers.Int64).SetValueDeserializer(Deserializers.Utf8).Build())
            {
                consumer.Subscribe(topic);

                Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + brokerList);

                while (true)
                {
                    try
                    {
                        var msg = consumer.Consume();
                        var data = JsonConvert.DeserializeObject<List<InsertLogModel>>(msg.Value);
                        await InsertToDb(data);
                        Console.WriteLine($"Received: '{msg.Value}'");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// INSERTS into the table.
        /// </summary>
        /// <param name="modelToSave">the data.</param>
        /// <returns>the result.</returns>
        private static async Task InsertToDb(List<InsertLogModel> modelToSave)
        {
            using (var db = new DatabaseContext())
            {
                db.AddRange(modelToSave);
                await db.SaveChangesAsync();
            }
        }
    }
}
