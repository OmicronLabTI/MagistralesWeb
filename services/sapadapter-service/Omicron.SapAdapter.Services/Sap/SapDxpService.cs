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
    using System.Threading.Tasks;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapDxpService : ISapDxpService
    {
        private readonly ISapDao sapDao;

        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDxpService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="redisService">thre redis service.</param>
        public SapDxpService(ISapDao sapDao, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersActive(List<int> ordersid)
        {
            var ordersSap = (await this.sapDao.GetAllOrdersWIthDetailByIds(ordersid)).ToList();
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);
            var asesorsIdsToLook = ordersSap.Select(x => x.AsesorId).Distinct().ToList();
            var asesors = (await this.sapDao.GetAsesorWithEmailByIdsFromTheAsesor(asesorsIdsToLook)).ToList();
            var listIds = ordersSap.Select(x => x.DocNum).Distinct().ToList();
            var ordersToReturn = new List<OrdersActivesDto>();

            foreach (var id in listIds)
            {
                var items = ordersSap.Where(x => x.DocNum == id).ToList();
                var order = items.FirstOrDefault();
                var isLine = items.All(x => lineProducts.Contains(x.Detalles.ProductoId));
                var asesor = asesors.FirstOrDefault(x => x.AsesorId == order.AsesorId);
                asesor ??= new SalesPersonModel { Email = string.Empty, PhoneMobile = string.Empty };
                ordersToReturn.Add(new OrdersActivesDto
                {
                    CardCode = order.Codigo,
                    DocNum = order.DocNum,
                    InitDate = order.FechaInicio,
                    AsesorId = order.AsesorId,
                    PedidoStatus = order.PedidoStatus,
                    IsLine = isLine ? "Y" : "N",
                    EmailAsesor = asesor.Email,
                    PhoneAsesor = asesor.PhoneMobile,
                });
            }

            var objectToReturn = new
            {
                ordersSap = ordersToReturn,
            };

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, objectToReturn, null, null);
        }
    }
}
