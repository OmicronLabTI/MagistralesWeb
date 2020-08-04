// <summary>
// <copyright file="IOrdersLogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Services.OrderLog
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Logs.Dtos.OrderLog;

    /// <summary>
    /// Interface OrdersLog Service.
    /// </summary>
    public interface IOrdersLogService
    {
        /// <summary>
        /// Method for add orderlog to DB.
        /// </summary>
        /// <param name="orderlog">OrderLog Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrderLog(List<OrderLogDto> orderlog);
    }
}
