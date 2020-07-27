// <summary>
// <copyright file="IUserFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Facade.OrderLogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Logs.Dtos.OrderLog;

    /// <summary>
    /// Interface OrderLog Facade.
    /// </summary>
    public interface IOrderLogFacade
    {
        /// <summary>
        /// Method to add orderlog registry to DB.
        /// </summary>
        /// <param name="orderlog">OrderLog Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrderLog(OrderLogDto orderlog);
    }
}