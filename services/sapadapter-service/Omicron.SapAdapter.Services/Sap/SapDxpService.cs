// <summary>
// <copyright file="SapDxpService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Resources.Extensions;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapDxpService : ISapDxpService
    {
        private readonly ISapDao sapDao;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDxpService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="logger">the logger.</param>
        public SapDxpService(ISapDao sapDao, ILogger logger)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersActive(List<int> ordersid)
        {
            var ordersSap = (await this.sapDao.GetAllOrdersWIthDetailByIds(ordersid)).ToList();
            var lineProducts = (await this.sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();
            var listIds = ordersSap.Select(x => x.DocNum).Distinct().ToList();
            var ordersToReturn = new List<OrdersActivesDto>();

            foreach (var id in listIds)
            {
                var items = ordersSap.Where(x => x.DocNum == id).ToList();
                var order = items.FirstOrDefault();
                var isLine = items.All(x => lineProducts.Contains(x.Detalles.ProductoId));
                ordersToReturn.Add(new OrdersActivesDto
                {
                    CardCode = order.Codigo,
                    DocNum = order.DocNum,
                    InitDate = order.FechaInicio,
                    AsesorId = order.AsesorId,
                    PedidoStatus = order.PedidoStatus,
                    IsLine = isLine ? "Y" : "N",
                });
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, ordersToReturn, null, null);
        }
    }
}
