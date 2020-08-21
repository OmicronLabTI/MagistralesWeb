// <summary>
// <copyright file="UpdateBatches.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Entities.Models
{
    /// <summary>
    /// the update batches class.
    /// </summary>
    public class UpdateBatches
    {
        public string SysNumber { get; set; }

        public int OrderId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public decimal PlannedQuantity { get; set; }

        public decimal QuantityByBatch { get; set; }
    }
}
