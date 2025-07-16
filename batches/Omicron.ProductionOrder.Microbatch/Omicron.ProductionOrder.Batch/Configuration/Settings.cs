// <summary>
// <copyright file="PedidosSettings.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Configuration
{
    public class Settings
    {
        /// <summary>
        /// Gets or sets the PedidosUrl.
        /// </summary>
        /// <value>
        /// PedidosUrl.
        /// </value>
        public string PedidosUrl { get; set; }

        /// <summary>
        /// Gets or sets the RedisUrl.
        /// </summary>
        /// <value>
        /// RedisUrl.
        /// </value>
        public string RedisUrl { get; set; }

        /// <summary>
        /// Gets or sets the SeqUrl.
        /// </summary>
        /// <value>
        /// SeqUrl.
        /// </value>
        public string SeqUrl { get; set; }

        /// <summary>
        /// Gets or sets the BatchSize.
        /// </summary>
        /// <value>
        /// BatchSize.
        /// </value>
        public int BatchSize { get; set; }
    }
}
