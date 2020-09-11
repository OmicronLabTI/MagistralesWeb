// <summary>
// <copyright file="BatchesConfigurationModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using System;

namespace Omicron.SapDiApi.Entities.Models
{
    /// <summary>
    /// Batches configuration model
    /// </summary>
    public class BatchesConfigurationModel
    { 
        /// <summary>
        /// Gets or sets batch
        /// </summary>
        /// <value>Batch code.</value>
        public string BatchCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets quantity in the batch
        /// </summary>
        /// <value>The batch quantity.</value>
        public double Quantity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the expiration date 
        /// </summary>
        /// <value>The expiration date.</value>
        public DateTime ExpirationDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the manufacturing date
        /// </summary>
        /// <value>The manufacturing date.</value>
        public DateTime ManufacturingDate
        {
            get; set;
        }
    }
}
