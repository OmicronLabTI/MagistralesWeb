// <summary>
// <copyright file="IPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Services.PedidosService
{
    // <summary>
    /// IPedidosService.
    /// </summary>
    public interface IPedidosService
    {
        /// <summary>
        /// GetAsync.
        /// </summary>
        /// <param name="route">Route End Point.</param>
        /// <param name="logBase">Log Base.</param>
        /// <returns>Data.</returns>
        Task<ResultDto> GetAsync(string route, string logBase);

        /// <summary>
        /// PostAsync.
        /// </summary>
        /// <param name="route">Route End Point.</param>
        /// <param name="dataToSend">Data To Send.</param>
        /// <param name="logBase">Log Base.</param>
        /// <returns>Data.</returns>
        Task<ResultDto> PostAsync(string route, object dataToSend, string logBase);
    }
}
