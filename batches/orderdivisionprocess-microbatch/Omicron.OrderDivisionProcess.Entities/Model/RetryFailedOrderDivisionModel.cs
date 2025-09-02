// <summary>
// <copyright file="RetryFailedOrderDivisionModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Entities.Model
{
    /// <summary>
    /// RetryFailedOrderDivisionModel.
    /// </summary>
    public class RetryFailedOrderDivisionModel
    {
        /// <summary>
        /// Identificador del proceso por ejecución del cron (para trazabilidad).
        /// </summary>
        public string BatchProcessId { get; set; }

        /// <summary>
        /// Lote/página de órdenes a reintentar (provenientes de Redis).
        /// </summary>
        public List<ProductionOrderSeparationDetailLogModel> DivisionOrdersPayload { get; set; } = new();
    }
}
