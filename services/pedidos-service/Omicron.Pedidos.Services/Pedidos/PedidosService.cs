// <summary>
// <copyright file="PedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// the pedidos service.
    /// </summary>
    public class PedidosService : IPedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly ISapDiApi sapDiApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        public PedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        public async Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId)
        {
            var orders = await this.sapAdapter.GetSapOrders(pedidosId.ListIds);
            var resultSap = await this.sapDiApi.CreateFabOrder(orders.Response);

            var listToInsert = ServiceUtils.CreateUserOrder(pedidosId.ListIds);
            var listOrderToInsert = ServiceUtils.CreateOrderLog(pedidosId.User, pedidosId.ListIds, new List<int>());

            await this.pedidosDao.InsertUserOrder(listToInsert);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(orders.Response), null);
        }

        /// <summary>
        /// returns the orders.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderBySaleOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(orders), null);
        }
    }
}
