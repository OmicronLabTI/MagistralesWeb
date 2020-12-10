// <summary>
// <copyright file="ISapAlmacenFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// Interface for sapAlmacen.
    /// </summary>
    public interface ISapAlmacenFacade
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="type">the type of the scan.</param>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedData(string type, string code);

        /// <summary>
        /// Gets all the details.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetCompleteDetail(int orderId);

        /// <summary>
        /// Gets the orders from delivery.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDeliveryBySaleOrderId(List<int> ordersId);

        /// <summary>
        /// Gets the delivery orders.
        /// </summary>
        /// <param name="parameters">the parameters to look.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDelivery(Dictionary<string, string> parameters);
    }
}
