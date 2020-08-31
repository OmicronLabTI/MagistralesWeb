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
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignPedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="userService">The user service.</param>
        public AssignPedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi, IUsersService userService)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
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

            orders.ForEach(x =>
            {
                x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : ServiceConstants.Reasignado;
                x.Userid = assign.UserId;
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

            var ordersToUpdate = AsignarLogic.GetUpdateUserOrderModel(orders, userOrdersBySale, assignModel.UserId, ServiceConstants.Reasignado);

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, assignModel.DocEntry, string.Format(ServiceConstants.ReasignarOrden, assignModel.UserId), ServiceConstants.OrdenFab));

            await this.pedidosDao.UpdateUserOrders(orders);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }
    }
}
