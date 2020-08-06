// <summary>
// <copyright file="SapService.cs" company="Axity">
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
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapService : ISapService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="pedidosService">the pedidosservice.</param>
        public SapService(ISapDao sapDao, IPedidosService pedidosService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <param name="parameters">The params.</param>
        /// <returns>get the orders.</returns>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var orders = new List<CompleteOrderModel>();
            var dateFilter = ServiceUtils.GetDateFilter(parameters);

            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                orders = (await this.sapDao.GetAllOrdersById(docNum)).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.FechaInicio))
            {
                orders = (await this.sapDao.GetAllOrdersByFechaIni(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
            else
            {
                orders = (await this.sapDao.GetAllOrdersByFechaFin(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }

            var usersQfb = await this.pedidosService.GetUserPedidos(orders.Select(x => x.DocNum).Distinct().ToList());





            orders = this.FilterList(orders, parameters);

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var ordersOrdered = orders.OrderBy(o => o.DocNum);
            var orderToReturn = ordersOrdered.Skip(offsetNumber).Take(limitNumber).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, orderToReturn, null, orders.Count());
        }

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="docId">the doc id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetOrderDetails(int docId)
        {
            var details = await this.sapDao.GetAllDetails(docId);

            details.ToList().ForEach(x =>
            {
                x.Status = !string.IsNullOrEmpty(x.Status) && ServiceConstants.DictStatus.ContainsKey(x.Status) ? ServiceConstants.DictStatus[x.Status] : x.Status;
            });

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, details, null, null);
        }

        /// <summary>
        /// Gets the orders with their detail.
        /// </summary>
        /// <param name="pedidosIds">the detail.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetPedidoWithDetail(List<int> pedidosIds)
        {
            var listData = new List<OrderWithDetailModel>();

            foreach (var x in pedidosIds)
            {
                var data = new OrderWithDetailModel();
                var order = (await this.sapDao.GetOrdersById(x)).FirstOrDefault();
                var detail = (await this.sapDao.GetAllDetails(x)).Where(s => string.IsNullOrEmpty(s.Status));

                data.Order = order;
                data.Detalle = detail.ToList();
                listData.Add(data);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listData, null, null);
        }

        /// <summary>
        /// filters the list by the params.
        /// </summary>
        /// <param name="orderModels">the list of data.</param>
        /// <param name="parameters">the params.</param>
        /// <returns>the data.</returns>
        private List<CompleteOrderModel> FilterList(List<CompleteOrderModel> orderModels, Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(ServiceConstants.Status))
            {
                orderModels = orderModels.Where(x => x.PedidoStatus.Equals(parameters[ServiceConstants.Status])).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Qfb))
            {
                orderModels = orderModels.Where(x => x.Qfb == parameters[ServiceConstants.Qfb]).ToList();
            }

            return orderModels;
        }
    }
}
