// <summary>
// <copyright file="ClosePedidosService.cs" company="Axity">
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
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// Contract for close orders.
    /// </summary>
    public class ClosePedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly ISapDiApi sapDiApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        public ClosePedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
        }

        /// <summary>
        /// Change sales order status to close.
        /// </summary>
        /// <param name="ordersToClose">Orders to close.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseSalesOrder(List<OrderIdModel> ordersToClose)
        {
            var results = new SuccessFailResults<OrderIdModel>();
            var logs = new List<OrderLogModel>();

            foreach (var orderToFinish in ordersToClose)
            {
                var ids = new List<string> { orderToFinish.OrderId.ToString() };
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(ids)).ToList();

                // Identify sales order
                var salesOrder = relatedOrders.First(x => string.IsNullOrEmpty(x.Productionorderid));
                if (!salesOrder.Status.Equals(ServiceConstants.Completed))
                {
                    results.AddFailedResult(orderToFinish, ServiceConstants.ReasonOrderNonCompleted);
                    continue;
                }

                var salesOrderId = int.Parse(salesOrder.Salesorderid);

                // Identify production order
                var productionOrders = relatedOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList();
                productionOrders = productionOrders.Where(x => !x.Status.Equals(ServiceConstants.Finalizado)).ToList();

                // Validate completed production orders
                var nonCompleted = productionOrders.Where(x => !x.Status.Equals(ServiceConstants.Completed)).ToList();
                if (nonCompleted.Any())
                {
                    foreach (var completeOrder in nonCompleted)
                    {
                        var message = string.Format(ServiceConstants.ReasonProductionOrderNonCompleted, completeOrder.Productionorderid);
                        results.AddFailedResult(orderToFinish, message);
                    }

                    continue;
                }

                // Update in SAP
                var payload = productionOrders.Select(x => new { OrderId = x.Productionorderid });
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.FinishFabOrder);

                if (!result.Success)
                {
                    results.AddFailedResult(orderToFinish, ServiceConstants.ReasonSapConnectionError);
                    continue;
                }

                var resultMessages = JsonConvert.DeserializeObject<Dictionary<int, string>>(result.Response.ToString());

                // Map errors
                foreach (var error in resultMessages.Where(x => x.Key > 0))
                {
                    results.AddFailedResult(orderToFinish, error.Value);
                }

                // Update production order status
                foreach (var userOrder in productionOrders)
                {
                    int prodOrderId = int.Parse(userOrder.Productionorderid);
                    if (!resultMessages.Keys.Any(x => x.Equals(prodOrderId)))
                    {
                        userOrder.CloseUserId = orderToFinish.UserId;
                        userOrder.CloseDate = DateTime.Now.FormatedDate();
                        userOrder.Status = ServiceConstants.Finalizado;

                        logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { prodOrderId }, string.Format(ServiceConstants.OrderFinished, prodOrderId), ServiceConstants.OrdenFab));
                    }
                }

                await this.pedidosDao.UpdateUserOrders(productionOrders);

                // Update sales order status
                if (resultMessages.Keys.Any(x => x.Equals(0)))
                {
                    salesOrder.CloseUserId = orderToFinish.UserId;
                    salesOrder.CloseDate = DateTime.Now.FormatedDate();
                    salesOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { salesOrderId }, string.Format(ServiceConstants.OrderFinished, salesOrderId), ServiceConstants.OrdenVenta));

                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { salesOrder });
                    results.AddSuccesResult(orderToFinish);
                }
            }

            await this.pedidosDao.InsertOrderLog(logs);
            return ServiceUtils.CreateResult(true, 200, null, results.DistinctResults(), null);
        }

        /// <summary>
        /// Close fabrication orders.
        /// </summary>
        /// <param name="ordersToClose">Orders to close.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseFabricationOrders(List<CloseProductionOrderModel> ordersToClose)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();
            var affectedSalesOrderIds = new List<KeyValuePair<string, string>>();

            foreach (var orderToFinish in ordersToClose)
            {
                var orderIdModel = new OrderIdModel { UserId = orderToFinish.UserId, OrderId = orderToFinish.OrderId };
                var productionOrderId = orderToFinish.OrderId;
                var ids = new List<string> { productionOrderId.ToString() };
                var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(ids)).FirstOrDefault();

                if (productionOrder == null)
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNotExists, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, message));
                    continue;
                }

                // Validate finished production orders
                if (productionOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    successfuly.Add(orderIdModel);
                    continue;
                }

                // Validate completed production orders
                if (!productionOrder.Status.Equals(ServiceConstants.Completed))
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNonCompleted, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, message));
                    continue;
                }

                // Update in SAP
                var payload = new List<object> { orderToFinish };
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.FinishFabOrder);

                if (!result.Success)
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, ServiceConstants.ReasonSapConnectionError));
                    continue;
                }

                var resultMessages = JsonConvert.DeserializeObject<Dictionary<int, string>>(result.Response.ToString());

                // Map errors
                foreach (var error in resultMessages.Where(x => x.Key > 0))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, error.Value));
                }

                // Update production order status
                if (!resultMessages.Keys.Any(x => x.Equals(productionOrderId)))
                {
                    productionOrder.CloseUserId = orderToFinish.UserId;
                    productionOrder.CloseDate = DateTime.Now.FormatedDate();
                    productionOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { productionOrderId }, string.Format(ServiceConstants.OrderFinished, productionOrderId), ServiceConstants.OrdenFab));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { productionOrder });
                    successfuly.Add(orderIdModel);

                    if (!productionOrder.IsIsolatedProductionOrder)
                    {
                        affectedSalesOrderIds.Add(KeyValuePair.Create(orderToFinish.UserId, productionOrder.Salesorderid));
                    }
                }
            }

            // Validate affected sales order
            foreach (var salesOrderToValidate in affectedSalesOrderIds.Distinct())
            {
                var userId = salesOrderToValidate.Key;
                var salesOrderIdAsInt = int.Parse(salesOrderToValidate.Value);
                var ids = new List<string> { salesOrderToValidate.Value };
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(ids)).ToList();

                // Identify production orders
                var productionOrders = relatedOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList();
                if (productionOrders.All(x => x.Status.Equals(ServiceConstants.Finalizado)))
                {
                    var salesOrder = relatedOrders.First(x => string.IsNullOrEmpty(x.Productionorderid));
                    salesOrder.CloseUserId = userId;
                    salesOrder.CloseDate = DateTime.Now.FormatedDate();
                    salesOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(userId, new List<int> { salesOrderIdAsInt }, string.Format(ServiceConstants.OrderFinished, salesOrderIdAsInt), ServiceConstants.OrdenVenta));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { salesOrder });
                }
            }

            await this.pedidosDao.InsertOrderLog(logs);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }
    }
}