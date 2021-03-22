// <summary>
// <copyright file="AssignPedidosService.cs" company="Axity">
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
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// the assign pedidos class.
    /// </summary>
    public class AssignPedidosService : IAssignPedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly ISapDiApi sapDiApi;

        private readonly IUsersService userService;

        private readonly IKafkaConnector kafkaConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignPedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="kafkaConnector">The kafka conector.</param>
        public AssignPedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi, IUsersService userService, IKafkaConnector kafkaConnector)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.kafkaConnector = kafkaConnector ?? throw new ArgumentNullException(nameof(kafkaConnector));
        }

        /// <summary>
        /// Assign the orders.
        /// </summary>
        /// <param name="manualAssign">the manual assign.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> AssignOrder(ManualAssignModel manualAssign)
        {
            if (manualAssign.OrderType.Equals(ServiceConstants.TypePedido))
            {
                return await AsignarLogic.AssignPedido(manualAssign, this.pedidosDao, this.sapAdapter, this.sapDiApi, this.kafkaConnector);
            }
            else
            {
                return await AsignarLogic.AssignOrder(manualAssign, this.pedidosDao, this.sapDiApi, this.sapAdapter, this.kafkaConnector);
            }
        }

        /// <summary>
        /// Makes the automatic assign.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> AutomaticAssign(AutomaticAssingModel assignModel)
        {
            var invalidStatus = new List<string> { ServiceConstants.Finalizado, ServiceConstants.Pendiente, ServiceConstants.Cancelled, ServiceConstants.Entregado, ServiceConstants.Almacenado };
            var orders = await this.sapAdapter.PostSapAdapter(assignModel.DocEntry, ServiceConstants.GetOrderWithDetail);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
            var sapOrderTypes = ordersSap.Select(x => x.Order.OrderType).Distinct().ToList();

            var users = await ServiceUtils.GetUsersByRole(this.userService, ServiceConstants.QfbRoleId.ToString(), true);
            users = sapOrderTypes.Contains(ServiceConstants.Mix) ? users : users.Where(x => sapOrderTypes.Contains(x.Classification)).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByUserId(users.Select(x => x.Id).ToList())).ToList();
            userOrders = userOrders.Where(x => !invalidStatus.Contains(x.Status)).ToList();
            var validUsers = await AsignarLogic.GetValidUsersByLoad(users, userOrders, this.sapAdapter);

            if (!validUsers.Any())
            {
                throw new CustomServiceException(ServiceConstants.ErrorQfbAutomatico, System.Net.HttpStatusCode.BadRequest);
            }

            var userSaleOrder = AsignarLogic.GetValidUsersByFormula(validUsers, ordersSap, userOrders);

            var ordersToUpdate = ordersSap.Where(x => !userSaleOrder.Item2.Contains(x.Order.DocNum)).ToList();
            var pedidosString = ordersToUpdate.Select(x => x.Order.DocNum.ToString()).ToList();

            var listToUpdate = ServiceUtils.GetOrdersToAssign(ordersToUpdate);
            var resultSap = await this.sapDiApi.PostToSapDiApi(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var userOrdersToUpdate = (await this.pedidosDao.GetUserOrderBySaleOrder(pedidosString)).ToList();

            var listOrderToInsert = new List<OrderLogModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            userOrdersToUpdate.ForEach(x =>
            {
                int.TryParse(x.Salesorderid, out int saleOrderInt);
                int.TryParse(x.Productionorderid, out int productionId);

                if (userSaleOrder.Item1.ContainsKey(saleOrderInt))
                {
                    var previousStatus = x.Status;
                    var asignable = !string.IsNullOrEmpty(x.Productionorderid) && listToUpdate.Any(y => y.OrderFabId.ToString() == x.Productionorderid);
                    x.Status = asignable ? ServiceConstants.Asignado : x.Status;
                    x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : x.Status;
                    x.Userid = userSaleOrder.Item1[saleOrderInt];

                    var orderId = string.IsNullOrEmpty(x.Productionorderid) ? saleOrderInt : productionId;
                    var ordenType = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.OrdenVenta : ServiceConstants.OrdenFab;
                    var textAction = string.IsNullOrEmpty(x.Productionorderid) ? string.Format(ServiceConstants.AsignarVenta, userSaleOrder.Item1[saleOrderInt]) : string.Format(ServiceConstants.AsignarOrden, userSaleOrder.Item1[saleOrderInt]);
                    listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, new List<int> { orderId }, textAction, ordenType));
                    if (previousStatus != x.Status)
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assignModel.UserLogistic, new List<UserOrderModel> { x }));
                    }
                }
            });

            await this.pedidosDao.UpdateUserOrders(userOrdersToUpdate);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);
            await this.kafkaConnector.PushMessage(listOrderLogToInsert);

            if (userSaleOrder.Item2.Any())
            {
                var errorParcial = new StringBuilder();
                userSaleOrder.Item2.ForEach(x => errorParcial.Append($"{x}, "));
                throw new CustomServiceException(string.Format(ServiceConstants.ErrirQfbAutomaticoParcial, errorParcial.ToString()), System.Net.HttpStatusCode.BadRequest);
            }

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// Reassign the ordr to a user.
        /// </summary>
        /// <param name="manualAssign">the objecto to assign.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> ReassignOrder(ManualAssignModel manualAssign)
        {
            if (manualAssign.OrderType.Equals(ServiceConstants.TypePedido))
            {
                return await this.ReassingarPedido(manualAssign);
            }
            else
            {
                return await this.ReassignOrders(manualAssign);
            }
        }

        /// <summary>
        /// Reassigns the Pedidos.
        /// </summary>
        /// <param name="assign">the assign object.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> ReassingarPedido(ManualAssignModel assign)
        {
            var listSaleOrders = assign.DocEntry.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderBySaleOrder(listSaleOrders)).Where(x => !ServiceConstants.StatusAvoidReasignar.Contains(x.Status)).ToList();
            var listOrderLogToInsert = new List<SalesLogs>();
            orders.ForEach(x =>
            {
                var previousStatus = x.Status;
                x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : ServiceConstants.Reasignado;
                x.Userid = assign.UserId;
                /** add logs**/
                if (previousStatus != x.Status)
                {
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assign.UserLogistic, new List<UserOrderModel> { x }));
                }
            });

            var listOrderFabId = orders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assign.UserLogistic, assign.DocEntry, string.Format(ServiceConstants.ReasignarPedido, assign.UserId), ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assign.UserLogistic, listOrderFabId, string.Format(ServiceConstants.ReasignarOrden, assign.UserId), ServiceConstants.OrdenFab));

            await this.pedidosDao.UpdateUserOrders(orders);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }

        /// <summary>
        /// method to reasign the orders.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> ReassignOrders(ManualAssignModel assignModel)
        {
            var listOrdersId = assignModel.DocEntry.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(listOrdersId)).ToList();

            var listSales = orders.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrdersBySale = (await this.pedidosDao.GetUserOrderBySaleOrder(listSales)).ToList();

            var listSalesNumber = listSales.Where(y => !string.IsNullOrEmpty(y)).Select(x => int.Parse(x)).ToList();
            var sapOrders = listSalesNumber.Any() ? await ServiceUtils.GetOrdersWithFabOrders(this.sapAdapter, listSalesNumber) : new List<OrderWithDetailModel>();

            var getUpdateUserOrderModel = AsignarLogic.GetUpdateUserOrderModel(orders, userOrdersBySale, sapOrders, assignModel.UserId, ServiceConstants.Reasignado, assignModel.UserLogistic);
            var ordersToUpdate = getUpdateUserOrderModel.Item1;
            var listOrderLogToInsert = new List<SalesLogs>();
            listOrderLogToInsert = getUpdateUserOrderModel.Item2;

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, assignModel.DocEntry, string.Format(ServiceConstants.ReasignarOrden, assignModel.UserId), ServiceConstants.OrdenFab));

            await this.pedidosDao.UpdateUserOrders(ordersToUpdate);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }
    }
}
