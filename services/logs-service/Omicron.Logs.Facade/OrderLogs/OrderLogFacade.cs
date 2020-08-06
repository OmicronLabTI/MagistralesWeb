// <summary>
// <copyright file="OrderLogFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Facade.OrderLogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Logs.Dtos.OrderLog;
    using Omicron.Logs.Services.OrderLog;

    /// <summary>
    /// Class OrderLog Facade.
    /// </summary>
    public class OrderLogFacade : IOrderLogFacade
    {
        private readonly IOrdersLogService ordersLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLogFacade"/> class.
        /// </summary>
        /// <param name="orderLogService">Interface Order Log Service.</param>
        public OrderLogFacade(IOrdersLogService orderLogService)
        {
            this.ordersLogService = orderLogService ?? throw new ArgumentNullException(nameof(orderLogService));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertOrderLog(List<OrderLogDto> orderlog)
        {
            return await this.ordersLogService.InsertOrderLog(orderlog);
        }
    }
}
