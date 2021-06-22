// <summary>
// <copyright file="ISapDxpFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Dtos.Models.Experience;
    using Omicron.SapDiApi.Dtos.Models;

    /// <summary>
    /// Class for the sapfacade.
    /// </summary>
    public interface ISapDxpFacade
    {
        /// <summary>
        /// Creates a sale order.
        /// </summary>
        /// <param name="saleOrderDto">the sale order.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreateSaleOrder(CreateSaleOrderDto saleOrderDto);
    }
}
