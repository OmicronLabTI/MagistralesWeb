// <summary>
// <copyright file="CancelPedidosService.cs" company="Axity">
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
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// Implementations for order cancellations.
    /// </summary>
    public class CancelPedidosService : ICancelPedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly ISapDiApi sapDiApi;

        private readonly ISapFileService sapFileService;

        private readonly IUsersService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelPedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="sapFileService">The sap file.</param>
        /// <param name="usersService">The user service.</param>
        public CancelPedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi, ISapFileService sapFileService, IUsersService usersService)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.sapFileService = sapFileService ?? throw new ArgumentException(nameof(sapFileService));
            this.userService = usersService ?? throw new ArgumentException(nameof(usersService));
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="ordersToCancel">Orders to cancel.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CancelFabricationOrders(List<OrderIdModel> ordersToCancel)
        {
            var results = new SuccessFailResults<OrderIdModel>();

            var orderIds = ordersToCancel.Select(x => x.OrderId.ToString()).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(orderIds)).ToList();

            foreach (var missing in ordersToCancel.Where(x => !userOrders.Any(y => y.Productionorderid.Equals(x.OrderId.ToString()))))
            {
                // Get from sap
                var sapProductionOrder = await this.GetFabricationOrderFromSap(missing.OrderId);
                if (sapProductionOrder != null)
                {
                    results = await this.CancelMissinLocalProductionOrder(missing, sapProductionOrder, results);
                    continue;
                }

                results.AddFailedResult(missing, ServiceConstants.ReasonNotExistsOrder);
            }

            var userId = ordersToCancel.First().UserId;

            var cancellationResults = await this.CancelExistingProductionOrders(ordersToCancel, userOrders, results);

            await this.CancelSalesOrderWithAllProductionOrderCancelled(userId, cancellationResults.Item1, this.sapAdapter);

            return ServiceUtils.CreateResult(true, 200, null, cancellationResults.Item2.DistinctResults(), null);
        }

        /// <summary>
        /// Change sales order status to cancel.
        /// </summary>
        /// <param name="ordersToCancel">Update orders info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CancelSalesOrder(List<OrderIdModel> ordersToCancel)
        {
            var results = new SuccessFailResults<OrderIdModel>();
            foreach (var orderToCancel in ordersToCancel)
            {
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { orderToCancel.OrderId.ToString() })).ToList();
                if (!relatedOrders.Any())
                {
                    results = await this.CancelMissinLocalSalesOrder(orderToCancel, results);
                }
                else
                {
                    results = await this.CancelLocalSalesOrder(orderToCancel, relatedOrders, results);
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, results.DistinctResults(), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CancelDelivery(List<int> deliveryIds)
        {
            var modelByDelivery = (await this.pedidosDao.GetUserOrderByDeliveryId(deliveryIds)).ToList();
            var listSales = modelByDelivery.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrdersGroups = (await this.pedidosDao.GetUserOrderBySaleOrder(listSales)).GroupBy(x => x.Salesorderid).ToList();
            var listToUpdate = new List<UserOrderModel>();
            var listSaleOrder = new List<UserOrderModel>();

            userOrdersGroups.ForEach(x =>
            {
                var deliveries = x.Where(y => y.IsProductionOrder).Select(y => y.DeliveryId).ToList();
                var status = deliveries.Distinct().Count() == 1 ? ServiceConstants.Finalizado : ServiceConstants.BackOrder;
                status = deliveries.Any(y => y == 0) ? ServiceConstants.Liberado : status;

                x.ToList().ForEach(y =>
                {
                    listSaleOrder.Add(new UserOrderModel { Salesorderid = y.Salesorderid, DeliveryId = y.DeliveryId });
                    y.StatusAlmacen = null;
                    y.UserCheckIn = null;
                    y.DateTimeCheckIn = null;
                    y.RemisionQr = null;
                    y.DeliveryId = 0;
                    y.Status = ServiceConstants.Finalizado;

                    if (y.IsSalesOrder)
                    {
                        y.Status = status;
                    }

                    listToUpdate.Add(y);
                });
            });

            await this.pedidosDao.UpdateUserOrders(listToUpdate);
            listSaleOrder = listSaleOrder.DistinctBy(x => x.DeliveryId).ToList();
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(listSaleOrder), null);
        }

        /// <summary>
        /// Cancel existing PO in the local data base.
        /// </summary>
        /// <param name="requestInfo">Request info.</param>
        /// <param name="ordersToCancel">Existing orders to cancel.</param>
        /// <param name="results">Object with results.</param>
        /// <returns>Operation result.</returns>
        private async Task<(List<UserOrderModel>, SuccessFailResults<OrderIdModel>)> CancelExistingProductionOrders(List<OrderIdModel> requestInfo, List<UserOrderModel> ordersToCancel, SuccessFailResults<OrderIdModel> results)
        {
            var logs = new List<OrderLogModel>();

            foreach (var order in ordersToCancel)
            {
                var newOrderInfo = requestInfo.First(y => y.OrderId.ToString().Equals(order.Productionorderid));

                // Dircarp cancelled orders
                if (order.Status.Equals(ServiceConstants.Cancelled))
                {
                    results.AddSuccesResult(newOrderInfo);
                    continue;
                }

                // Discarp finalized orders
                if (order.Status.Equals(ServiceConstants.Finalizado))
                {
                    results.AddFailedResult(newOrderInfo, ServiceConstants.ReasonOrderFinished);
                    continue;
                }

                // Process to cancel on local db
                if (await this.CancelProductionOrderInSap(order.Productionorderid))
                {
                    order.Status = ServiceConstants.Cancelled;
                    results.AddSuccesResult(newOrderInfo);
                    logs.Add(this.BuildCancellationLog(newOrderInfo.UserId, order.Productionorderid, ServiceConstants.OrdenFab));
                    continue;
                }

                results.AddFailedResult(newOrderInfo, ServiceConstants.ReasonSapError);
            }

            // Update in local data base
            await this.pedidosDao.UpdateUserOrders(ordersToCancel);
            await this.pedidosDao.InsertOrderLog(logs);
            return (ordersToCancel, results);
        }

        /// <summary>
        /// Cancel SO with all PO cancelled.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="cancelledProductionOrders">Cancelled PO.</param>
        /// <returns>Nothing.</returns>
        private async Task CancelSalesOrderWithAllProductionOrderCancelled(string userId, List<UserOrderModel> cancelledProductionOrders, ISapAdapter sapAdapter)
        {
            var logs = new List<OrderLogModel>();
            var salesOrdersToUpdate = new List<UserOrderModel>();
            var salesOrderIds = cancelledProductionOrders.Where(x => x.IsProductionOrder && !x.IsIsolatedProductionOrder).Select(x => x.Salesorderid);
            var saleOrdersFinalized = new List<int>();

            foreach (var salesOrderId in salesOrderIds.Distinct())
            {
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { salesOrderId })).ToList();
                var salesOrder = relatedOrders.First(x => x.IsSalesOrder);
                var sapMissingOrders = await ServiceUtils.GetPreProductionOrdersFromSap(salesOrder, sapAdapter);
                var productionOrders = relatedOrders.Where(x => x.IsProductionOrder).ToList();

                salesOrder.Status = this.CalculateStatus(salesOrder, sapMissingOrders, productionOrders);

                if (salesOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    saleOrdersFinalized.Add(int.Parse(salesOrder.Salesorderid));
                }

                salesOrdersToUpdate.Add(salesOrder);

                if (salesOrder.Equals(ServiceConstants.Cancelled))
                {
                    logs.Add(this.BuildCancellationLog(userId, salesOrderId, ServiceConstants.OrdenVenta));
                }
            }

            await this.pedidosDao.UpdateUserOrders(salesOrdersToUpdate);
            await this.pedidosDao.InsertOrderLog(logs);

            if (saleOrdersFinalized.Any())
            {
                await SendToGeneratePdfUtils.CreateModelGeneratePdf(saleOrdersFinalized, new List<int>(), this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, true);
            }
        }

        /// <summary>
        /// Gets the status for cancelling.
        /// </summary>
        /// <param name="saleOrder">the sale order.</param>
        /// <param name="sapOrders">the missingsap orders.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <returns>the status.</returns>
        private string CalculateStatus(UserOrderModel saleOrder, List<CompleteDetailOrderModel> sapOrders, List<UserOrderModel> userOrders)
        {
            if (sapOrders.Any())
            {
                return saleOrder.Status;
            }

            var minValue = userOrders.OrderBy(x => x.StatusOrder).FirstOrDefault();
            var status = ((StatusEnum)minValue.StatusOrder).ToString();
            return ServiceConstants.ValidStatusLiberado.Contains(minValue.Status) ? ServiceConstants.Liberado : status;
        }

        /// <summary>
        /// Cancel SAP sales orders.
        /// </summary>
        /// <param name="missingOrder">Missing orders.</param>
        /// <param name="results">Results.</param>
        /// <returns>Object with results.</returns>
        private async Task<SuccessFailResults<OrderIdModel>> CancelMissinLocalSalesOrder(
            OrderIdModel missingOrder,
            SuccessFailResults<OrderIdModel> results)
        {
            var sapOrder = await this.GetSalesOrdersFromSap(missingOrder.OrderId);
            var logs = new List<OrderLogModel>();

            if (sapOrder != null)
            {
                var validationResults = this.IsValidCancelSapSalesOrder(missingOrder, sapOrder, results);
                if (!validationResults.Item1)
                {
                    return validationResults.Item2;
                }

                sapOrder.Detalle.ForEach(async (x) => await this.CancelProductionOrderInSap(x.OrdenFabricacionId.ToString()));

                var newUserOrders = sapOrder.ToUserOrderModels();
                newUserOrders.ForEach(x => x.Status = ServiceConstants.Cancelled);

                newUserOrders.ForEach(x => logs.Add(this.BuildCancellationLog(missingOrder.UserId, x.Productionorderid, ServiceConstants.OrdenFab)));
                logs.Add(this.BuildCancellationLog(missingOrder.UserId, missingOrder.OrderId, ServiceConstants.OrdenVenta));

                results.AddSuccesResult(missingOrder);
                await this.pedidosDao.InsertUserOrder(newUserOrders);
                await this.pedidosDao.InsertOrderLog(logs);
            }

            return results;
        }

        /// <summary>
        /// Cancel SAP production orders.
        /// </summary>
        /// <param name="missingOrder">Missing order.</param>
        /// <param name="sapProductionOrder">Sap production order.</param>
        /// <param name="results">Results.</param>
        /// <returns>Object with results.</returns>
        private async Task<SuccessFailResults<OrderIdModel>> CancelMissinLocalProductionOrder(
            OrderIdModel missingOrder,
            FabricacionOrderModel sapProductionOrder,
            SuccessFailResults<OrderIdModel> results)
        {
            var newUserOrders = new List<UserOrderModel>();
            var logs = new List<OrderLogModel>();

            var validationResults = await this.IsValidCancelSapProductionOrder(missingOrder, sapProductionOrder, results);
            if (!validationResults.Item1)
            {
                return validationResults.Item2;
            }

            if (await this.CancelProductionOrderInSap(sapProductionOrder.OrdenId.ToString()))
            {
                var newUserOrder = new UserOrderModel();
                newUserOrder.Status = ServiceConstants.Cancelled;
                newUserOrder.Productionorderid = sapProductionOrder.OrdenId.ToString();
                newUserOrder.Salesorderid = sapProductionOrder.PedidoId == null ? string.Empty : sapProductionOrder.PedidoId.ToString();

                newUserOrders.Add(newUserOrder);
                logs.Add(this.BuildCancellationLog(missingOrder.UserId, missingOrder.OrderId, ServiceConstants.OrdenFab));

                results.AddSuccesResult(missingOrder);
                await this.pedidosDao.InsertUserOrder(newUserOrders);
                await this.pedidosDao.InsertOrderLog(logs);
                return results;
            }

            results.AddFailedResult(missingOrder, ServiceConstants.ReasonSapError);
            return results;
        }

        /// <summary>
        /// Cancel local sales orders.
        /// </summary>
        /// <param name="orderToCancel">Order to cancel.</param>
        /// <param name="relatedUserOrders">Related orders.</param>
        /// <param name="results">Results.</param>
        /// <returns>Object with results.</returns>
        private async Task<SuccessFailResults<OrderIdModel>> CancelLocalSalesOrder(
            OrderIdModel orderToCancel,
            List<UserOrderModel> relatedUserOrders,
            SuccessFailResults<OrderIdModel> results)
        {
            var salesOrder = relatedUserOrders.First(x => x.IsSalesOrder);

            // Validate finished sales order
            if (salesOrder.Status.Equals(ServiceConstants.Finalizado))
            {
                results.AddFailedResult(orderToCancel, ServiceConstants.ReasonSalesOrderFinished);
                return results;
            }

            // Validate finished related production orders
            var finishedOrders = relatedUserOrders.Where(x => x.Status.Equals(ServiceConstants.Finalizado)).ToList();
            if (finishedOrders.Any())
            {
                foreach (var finishedOrder in finishedOrders.Where(x => x.IsProductionOrder))
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderFinished, finishedOrder.Productionorderid);
                    results.AddFailedResult(orderToCancel, message);
                }

                return results;
            }

            var cancellationResults = await this.CancelUserOrders(orderToCancel, relatedUserOrders, results);
            return cancellationResults.Item2;
        }

        /// <summary>
        /// Cancel user orders.
        /// </summary>
        /// <param name="orderToCancel">Order to cancel.</param>
        /// <param name="relatedUserOrders">Related user orders.</param>
        /// <param name="results">Results.</param>
        /// <returns>Updated user orders and object with results.</returns>
        private async Task<(List<UserOrderModel>, SuccessFailResults<OrderIdModel>)> CancelUserOrders(
            OrderIdModel orderToCancel,
            List<UserOrderModel> relatedUserOrders,
            SuccessFailResults<OrderIdModel> results)
        {
            var logs = new List<OrderLogModel>();
            var updatedOrders = new List<UserOrderModel>();

            foreach (var order in relatedUserOrders)
            {
                var cancelledOnSap = true;
                var docType = ServiceConstants.OrdenVenta;
                var orderId = int.Parse(order.Salesorderid);

                if (order.IsProductionOrder)
                {
                    cancelledOnSap = await this.CancelProductionOrderInSap(order.Productionorderid);
                    docType = ServiceConstants.OrdenFab;
                    orderId = int.Parse(order.Productionorderid);
                }

                if (cancelledOnSap)
                {
                    order.Status = ServiceConstants.Cancelled;
                    results.AddSuccesResult(orderToCancel);
                    updatedOrders.Add(order);
                    logs.Add(this.BuildCancellationLog(orderToCancel.UserId, orderId, docType));
                    continue;
                }

                results.AddFailedResult(orderToCancel, ServiceConstants.ReasonSapError);
            }

            await this.pedidosDao.UpdateUserOrders(updatedOrders);
            await this.pedidosDao.InsertOrderLog(logs);

            return (updatedOrders, results);
        }

        /// <summary>
        /// Validate status for SAP Order.
        /// </summary>
        /// <param name="orderToCancel">Order to cancel.</param>
        /// <param name="sapOrder">Sap order to cancel.</param>
        /// <param name="results">Results.</param>
        /// <returns>Validation flag and results.</returns>
        private (bool, SuccessFailResults<OrderIdModel>) IsValidCancelSapSalesOrder(
            OrderIdModel orderToCancel,
            OrderWithDetailModel sapOrder,
            SuccessFailResults<OrderIdModel> results)
        {
            if (sapOrder.Order.PedidoStatus.Equals("C"))
            {
                results.AddFailedResult(orderToCancel, ServiceConstants.ReasonSalesOrderFinished);
                return (false, results);
            }

            var finishedOrders = sapOrder.Detalle.Where(x => x.Status.Equals("L")).ToList();
            if (finishedOrders.Any())
            {
                foreach (var finishedOrder in finishedOrders)
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderFinished, finishedOrder.OrdenFabricacionId);
                    results.AddFailedResult(orderToCancel, message);
                }

                return (false, results);
            }

            return (true, results);
        }

        /// <summary>
        /// Validate status for SAP Production Order.
        /// </summary>
        /// <param name="orderToCancel">Order to cancel.</param>
        /// <param name="sapProductionOrder">Sap order to cancel.</param>
        /// <param name="results">Results.</param>
        /// <returns>Validation flag and results.</returns>
        private async Task<(bool, SuccessFailResults<OrderIdModel>)> IsValidCancelSapProductionOrder(
            OrderIdModel orderToCancel,
            FabricacionOrderModel sapProductionOrder,
            SuccessFailResults<OrderIdModel> results)
        {
            if (sapProductionOrder.Status.ToLower().Equals("l"))
            {
                results.AddFailedResult(orderToCancel, ServiceConstants.ReasonOrderFinished);
                return (false, results);
            }

            if (sapProductionOrder.PedidoId != null)
            {
                var completeSalesOrder = await this.GetSalesOrdersFromSap(sapProductionOrder.PedidoId.Value);
                if (completeSalesOrder.Order.PedidoStatus.Equals("C"))
                {
                    results.AddFailedResult(orderToCancel, ServiceConstants.ReasonSalesOrderFinished);
                    return (false, results);
                }
            }

            return (true, results);
        }

        /// <summary>
        /// Cancel production order in sap.
        /// </summary>
        /// <param name="productionOrderId">Order to cancel.</param>
        /// <returns>Cancellation result.</returns>
        private async Task<bool> CancelProductionOrderInSap(string productionOrderId)
        {
            var payload = new { OrderId = productionOrderId };
            var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CancelFabOrder);
            var resultResponse = result.Response.ToString();
            return result.Success && (resultResponse.Equals(ServiceConstants.Ok) || resultResponse.Equals(ServiceConstants.ErrorProductionOrderCancelled));
        }

        /// <summary>
        /// Get sales order from SAP.
        /// </summary>
        /// <param name="salesOrderId">Sales order id.</param>
        /// <returns>Sales order.</returns>
        private async Task<OrderWithDetailModel> GetSalesOrdersFromSap(int salesOrderId)
        {
            var orders = await this.sapAdapter.PostSapAdapter(new List<int> { salesOrderId }, ServiceConstants.GetOrderWithDetail);
            var sapOrders = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
            sapOrders = sapOrders.Where(x => x.Order != null).ToList();
            sapOrders.ForEach(o =>
            {
                o.Detalle = o.Detalle.Where(x => !string.IsNullOrEmpty(x.Status)).ToList();
            });

            return sapOrders.FirstOrDefault();
        }

        /// <summary>
        /// Get production order from SAP.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Sales order.</returns>
        private async Task<FabricacionOrderModel> GetFabricationOrderFromSap(int productionOrderId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "docNum", $"{productionOrderId.ToString()}-{productionOrderId.ToString()}" } };
            var payload = new GetOrderFabModel { Filters = parameters, OrdersId = new List<int>() };

            var orders = await this.sapAdapter.PostSapAdapter(payload, ServiceConstants.GetFabOrdersByFilter);
            var sapOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(JsonConvert.SerializeObject(orders.Response));

            return sapOrders.FirstOrDefault();
        }

        /// <summary>
        /// Build cancellation log.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="orderId">Order id.</param>
        /// <param name="orderType">Order type.</param>
        /// <returns>New order log.</returns>
        private OrderLogModel BuildCancellationLog(string userId, object orderId, string orderType)
        {
            var orderIdAsInt = int.Parse(orderId.ToString() ?? throw new InvalidOperationException());
            var newLogs = ServiceUtils.CreateOrderLog(userId, new List<int> { orderIdAsInt }, string.Format(ServiceConstants.OrderCancelled, orderIdAsInt), orderType);
            return newLogs.FirstOrDefault();
        }
    }
}
