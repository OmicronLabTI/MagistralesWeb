// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Status of planed production orders.
        /// </summary>
        public const string ProductionOrderPlannedStatus = "Planificado";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisComponentsInputRequest = "redisComponentsInputRequest";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ListComponentsMostAssigned { get; } = new List<string>
        {
            "EN",
            "EM",
        };
    }
}
