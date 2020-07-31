// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// The constants classs.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FilterDate = "date";

        /// <summary>
        /// today value.
        /// </summary>
        public const string Today = "today";

        /// <summary>
        /// two weeks value.
        /// </summary>
        public const string TwoWeeks = "twoweeks";

        /// <summary>
        /// month value.
        /// </summary>
        public const string Month = "month";

        /// <summary>
        /// const for offset.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// Const for the limit.
        /// </summary>
        public const string Limit = "limit";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatus { get; } = new Dictionary<string, string>
        {
            { "P", "Planificado" },
            { "L", "Cerrado" },
            { "C", "Cancelado" },
            { "R", "Liberado" },
        };
    }
}
