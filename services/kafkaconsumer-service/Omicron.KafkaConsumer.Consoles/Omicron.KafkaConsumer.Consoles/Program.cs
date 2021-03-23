// <summary>
// <copyright file="Program.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.KafkaConsumer.Consoles
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Omicron.KafkaConsumer.Service;

    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing Consumer");
            string brokerList = ConfigurationManager.AppSettings["EH_FQDN"];
            string connectionString = ConfigurationManager.AppSettings["EH_CONNECTION_STRING"];
            string topic = ConfigurationManager.AppSettings["EH_NAME"];
            string caCertLocation = ConfigurationManager.AppSettings["CA_CERT_LOCATION"];
            string consumerGroup = ConfigurationManager.AppSettings["CONSUMER_GROUP"];

            await Consumer.ConsumerData(brokerList, connectionString, consumerGroup, topic, caCertLocation);
        }
    }
}
