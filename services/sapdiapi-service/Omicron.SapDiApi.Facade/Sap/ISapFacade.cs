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
    }
}
