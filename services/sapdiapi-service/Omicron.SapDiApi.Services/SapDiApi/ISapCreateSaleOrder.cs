// <summary>
// <copyright file="ISapCreateSaleOrder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates the sap sale orde.
    /// </summary>
    public interface ISapCreateSaleOrder
    {
        /// <summary>
        /// Creates a sale order.
        /// </summary>
        /// <param name="saleOrderModel">the sale order.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateSaleOrder(CreateSaleOrderModel saleOrderModel);
    }
}
