// <summary>
// <copyright file="IKafkaConnector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Broker
{
    /// <summary>
    /// Class to connect to kafka.
    /// </summary>
    public interface IKafkaConnector
    {
        /// <summary>
        /// push message to kafka.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="logBase">Log Base.</param>
        /// <returns>the data.</returns>
        Task<bool> PushMessage(object message, string logBase);
    }
}
