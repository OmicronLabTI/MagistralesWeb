// <summary>
// <copyright file="ResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Dtos.Models;

    public interface ISapFacade
    {
        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto);

        /// <summary>
        /// updates the fabriction orders.
        /// </summary>
        /// <param name="updateFabOrderDtos">the orders to update.</param>
        /// <returns>the reult.</returns>
        Task<ResultDto> UpdateFabOrder(List<UpdateFabOrderDto> updateFabOrderDtos);

        /// <summary>
        /// connecto to sap.
        /// </summary>
        /// <returns>connects.</returns>
        Task<ResultDto> Connect();
    }
}
