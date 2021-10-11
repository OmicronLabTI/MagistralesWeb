// <summary>
// <copyright file="IKafkaConnector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Broker
{
    using System.Threading.Tasks;

    /// <summary>
    /// Class to connect to kafka.
    /// </summary>
    public interface IKafkaConnector
    {
        /// <summary>
        /// push message to kafka.
        /// </summary>
        /// <param name="messaje">the message.</param>
        /// <returns>the data.</returns>
        Task<bool> PushMessage(object messaje);
    }
}
