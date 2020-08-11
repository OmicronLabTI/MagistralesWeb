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
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
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
            var orders = await this.sapAdapter.PostSapAdapter(pedidosId.ListIds, ServiceConstants.GetOrderWithDetail);

            var resultSap = await this.sapDiApi.CreateFabOrder(orders.Response);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsWhileInserting(listWithError);

            var prodOrders = await this.sapAdapter.PostSapAdapter(listToLook, ServiceConstants.GetProdOrderByOrderItem);
            var listOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(prodOrders.Response.ToString());

            var listToInsert = ServiceUtils.CreateUserOrder(listOrders);
            var listOrderToInsert = ServiceUtils.CreateOrderLog(pedidosId.User, pedidosId.ListIds, listOrders);

            await this.pedidosDao.InsertUserOrder(listToInsert);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            var userError = listErrorId.Any() ? ServiceConstants.ErrorAlInsertar : null;
            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
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

        /// <summary>
        /// Gets the QFB orders (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetFabOrderByUserID(string userId)
        {
            var userOrders = (await this.pedidosDao.GetUserOrderByUserId(new List<string> { userId })).ToList();
            var resultFormula = await this.GetSapOrders(userOrders);

            var groups = this.GroupUserOrder(resultFormula, userOrders);
            return ServiceUtils.CreateResult(true, 200, null, groups, null);
        }

        /// <summary>
        /// Gets the list of user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUserOrdersByUserId(List<string> listIds)
        {
            var userOrder = await this.pedidosDao.GetUserOrderByUserId(listIds);
            return ServiceUtils.CreateResult(true, 200, null, userOrder, null);
        }

        /// <summary>
        /// gets the order from sap.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <returns>tje data.</returns>
        private async Task<List<CompleteFormulaWithDetalle>> GetSapOrders(List<UserOrderModel> userOrders)
        {
            var resultFormula = new List<CompleteFormulaWithDetalle>();

            await Task.WhenAll(userOrders.Select(async x =>
            {
                var route = $"{ServiceConstants.GetFormula}{x.Productionorderid}";
                var result = await this.sapAdapter.GetSapAdapter(route);

                lock (resultFormula)
                {
                    var formula = JsonConvert.DeserializeObject<CompleteFormulaWithDetalle>(JsonConvert.SerializeObject(result.Response));
                    resultFormula.Add(formula);
                }
            }));

            return resultFormula;
        }

        /// <summary>
        /// Groups the data for the front by status.
        /// </summary>
        /// <param name="sapOrders">the sap ordrs.</param>
        /// <param name="userOrders">the user ordes.</param>
        /// <returns>the data froupted.</returns>
        private QfbOrderModel GroupUserOrder(List<CompleteFormulaWithDetalle> sapOrders, List<UserOrderModel> userOrders)
        {
            var result = new QfbOrderModel
            {
                Status = new List<QfbOrderDetail>(),
            };

            foreach (var status in Enum.GetValues(typeof(ServiceEnums.Status)))
            {
                var statusId = (int)Enum.Parse(typeof(ServiceEnums.Status), status.ToString());
                var orders = new QfbOrderDetail
                {
                    StatusName = statusId == (int)ServiceEnums.Status.Proceso ? ServiceConstants.ProcesoStatus : status.ToString(),
                    StatusId = statusId,
                    Orders = new List<FabOrderDetail>(),
                };

                var ordersDetail = new List<FabOrderDetail>();

                userOrders
                    .Where(x => x.Status.Equals(status.ToString()))
                    .Select(y => y.Productionorderid)
                    .ToList()
                    .ForEach(o =>
                    {
                        int.TryParse(o, out int orderId);
                        var sapOrder = sapOrders.FirstOrDefault(s => s.ProductionOrderId == orderId);

                        if (sapOrder != null)
                        {
                            var order = new FabOrderDetail
                            {
                                BaseDocument = sapOrder.BaseDocument,
                                Container = sapOrder.Container,
                                DescriptionProduct = sapOrder.ProductDescription,
                                FinishDate = sapOrder.EndDate,
                                PlannedQuantity = sapOrder.PlannedQuantity,
                                ProductionOrderId = sapOrder.ProductionOrderId,
                                StartDate = sapOrder.StartDate,
                            };

                            ordersDetail.Add(order);
                        }
                    });

                orders.Orders = ordersDetail;
                result.Status.Add(orders);
            }

            return result;
        }
    }
}
