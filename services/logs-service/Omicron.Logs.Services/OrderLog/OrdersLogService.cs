// <summary>
// <copyright file="OrdersLogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Services.OrderLog
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Logs.DataAccess.DAO.OrderLog;
    using Omicron.Logs.Dtos.OrderLog;
    using Omicron.Logs.Entities.Model;

    /// <summary>
    /// Class User Service.
    /// </summary>
    public class OrdersLogService : IOrdersLogService
    {
        private readonly IMapper mapper;

        private readonly IOrderLogDao orderLogDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersLogService"/> class.
        /// </summary>
        /// <param name="mapper">Object to mapper.</param>
        /// <param name="orderLogDao">Object to userDao.</param>
        public OrdersLogService(IMapper mapper, IOrderLogDao orderLogDao)
        {
            this.mapper = mapper;
            this.orderLogDao = orderLogDao ?? throw new ArgumentNullException(nameof(orderLogDao));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertOrderLog(List<OrderLogDto> orderlog)
        {
            return await this.orderLogDao.InsertOrderLog(this.mapper.Map<List<OrderLogModel>>(orderlog));
        }
    }
}
