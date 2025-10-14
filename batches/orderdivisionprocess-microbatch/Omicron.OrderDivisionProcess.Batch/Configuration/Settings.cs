// <summary>
// <copyright file="Settings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Configuration
{
    /// <summary>
    /// Settings.
    /// </summary>
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
