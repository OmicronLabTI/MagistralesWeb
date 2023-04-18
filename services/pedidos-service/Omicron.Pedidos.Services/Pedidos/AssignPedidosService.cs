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
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Builders;
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
            this.sapAdapter = sapAdapter.ThrowIfNull(nameof(sapAdapter));
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.sapDiApi = sapDiApi.ThrowIfNull(nameof(sapDiApi));
            this.userService = userService.ThrowIfNull(nameof(userService));
            this.kafkaConnector = kafkaConnector.ThrowIfNull(nameof(kafkaConnector));
        }

        /// <summary>
        /// Assign the orders.
        /// </summary>
        /// <param name="manualAssign">the manual assign.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> AssignOrder(ManualAssignModel manualAssign)
        {
            var hasPedidosType = await this.CheckIfNeedTecnicConfigValidation(manualAssign.DocEntry.Select(x => x.ToString()).ToList(), manualAssign.OrderType);
            var qfbInfoValidated = (await ServiceUtils.GetQfbInfoById(new List<string> { manualAssign.UserId }, this.userService)).FirstOrDefault();
            qfbInfoValidated ??= new QfbTecnicInfoDto();

            if (ServiceShared.CalculateAnd(!qfbInfoValidated.IsValidTecnic, hasPedidosType))
            {
                return ServiceUtils.CreateResult(false, 400, string.Format(ServiceConstants.QfbWithoutTecnic, $"{qfbInfoValidated.QfbFirstName} {qfbInfoValidated.QfbLastName}"), null, null);
            }

            if (manualAssign.OrderType.Equals(ServiceConstants.TypePedido))
            {
                return await AsignarLogic.AssignPedido(manualAssign, qfbInfoValidated, this.pedidosDao, this.sapAdapter, this.sapDiApi, this.kafkaConnector);
            }
            else
            {
                return await AsignarLogic.AssignOrder(manualAssign, qfbInfoValidated, this.pedidosDao, this.sapDiApi, this.sapAdapter, this.kafkaConnector);
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

            var orders = await this.sapAdapter.PostSapAdapter(assignModel.DocEntry, ServiceConstants.GetOrderWithDetailDxp);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
            var relationships = JsonConvert.DeserializeObject<List<RelationDxpDocEntryModel>>(JsonConvert.SerializeObject(orders.Comments));
            relationships ??= new List<RelationDxpDocEntryModel>();

            var sapOrderTypes = ordersSap.Select(x => x.Order.OrderType).Distinct().ToList();
            var allUsers = await ServiceUtils.GetUsersByRole(this.userService, ServiceConstants.QfbRoleId.ToString(), true);
            var users = allUsers.FindAll(x => !x.Classification.ToUpper().Equals(ServiceConstants.UserClassificationDZ)).ToList();

            var builder = new DzIsNotOmigenomicsBuilder(orders, allUsers, ordersSap);
            var relationOrdersWithUsersDZIsNotOmi = builder.AssignOrdersToUsersDZIsNotOmi();

            users = ServiceShared.CalculateTernary(sapOrderTypes.Contains(ServiceConstants.Mix), users, users.Where(x => sapOrderTypes.Contains(x.Classification)).ToList());
            var userOrders = (await this.pedidosDao.GetUserOrderByUserIdAndNotInStatus(users.Select(x => x.Id).ToList(), invalidStatus)).ToList();
            var validUsers = AsignarLogic.GetValidUsersByLoad(users, userOrders, this.sapAdapter);
            bool isOnlyClasificationDZ = ServiceShared.CalculateAnd(relationOrdersWithUsersDZIsNotOmi.Any(), !ordersSap.Any());

            if (ServiceShared.CalculateAnd(!validUsers.Any(), !isOnlyClasificationDZ))
            {
                throw new CustomServiceException(ServiceConstants.ErrorQfbAutomatico, HttpStatusCode.BadRequest);
            }

            var userSaleOrder = AsignarLogic.GetValidUsersByFormula(validUsers, ordersSap, userOrders, relationships);
            var ordersToUpdate = ordersSap.Where(x => !userSaleOrder.Item2.Contains(x.Order.DocNum)).ToList();
            var pedidosString = ordersToUpdate.Select(x => x.Order.DocNum.ToString()).ToList();
            var listToUpdate = ServiceUtils.GetOrdersToAssign(ordersToUpdate);

            var listToUpdateSAP = listToUpdate.Concat(relationOrdersWithUsersDZIsNotOmi
                .SelectMany(relation =>
                    relation.Order.Detalle.Where(d => d.Status.Equals("P"))
                    .Select(detail =>
                        new UpdateFabOrderModel
                        {
                            OrderFabId = detail.OrdenFabricacionId,
                            Status = ServiceConstants.StatusSapLiberado,
                        }))).ToList();

            var pedidosStringUpdate = pedidosString.Concat(relationOrdersWithUsersDZIsNotOmi.Select(x => x.Order.Order.DocNum.ToString())).Distinct().ToList();
            var userOrdersToUpdate = (await this.pedidosDao.GetUserOrderBySaleOrder(pedidosStringUpdate)).ToList();
            var listOrderLogToInsert = new List<SalesLogs>();
            var invalidQfbs = new List<UserModel>();
            userOrdersToUpdate.ForEach(x =>
            {
                bool isHeader = string.IsNullOrEmpty(x.Productionorderid);
                int.TryParse(x.Salesorderid, out int saleOrderInt);
                int.TryParse(x.Productionorderid, out int productionId);
                bool isClasificationDZ = relationOrdersWithUsersDZIsNotOmi.Any(rel => rel.Order.Order.PedidoId.Equals(saleOrderInt));
                bool isTraditional = userSaleOrder.Item1.ContainsKey(saleOrderInt);
                if (ServiceShared.CalculateOr(isTraditional, isClasificationDZ, isOnlyClasificationDZ))
                {
                    var previousStatus = x.Status;
                    var asignable = !isHeader && listToUpdateSAP.Any(y => y.OrderFabId.ToString() == x.Productionorderid);
                    x.Status = ServiceShared.CalculateTernary(asignable, ServiceConstants.Asignado, x.Status);
                    x.Status = ServiceShared.CalculateTernary(isHeader, ServiceConstants.Liberado, x.Status);
                    x.Userid = this.GetUserId(isHeader, relationOrdersWithUsersDZIsNotOmi, userSaleOrder, saleOrderInt, isClasificationDZ, isOnlyClasificationDZ, productionId);
                    this.CalculateInvalidUsers(invalidQfbs, allUsers, x.Userid);
                    x.TecnicId = allUsers.FirstOrDefault(user => user.Id == x.Userid)?.TecnicId;
                    x.StatusForTecnic = x.Status;
                    x.AssignmentDate = DateTime.Now;
                    if (ServiceShared.CalculateAnd(previousStatus != x.Status, x.IsSalesOrder))
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assignModel.UserLogistic, new List<UserOrderModel> { x }));
                    }

                    if (!x.IsSalesOrder)
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assignModel.UserLogistic, new List<UserOrderModel> { x }));
                    }
                }
            });

            if (invalidQfbs.Any())
            {
                throw new CustomServiceException(string.Format(ServiceConstants.QfbWithoutTecnic, string.Join(",", invalidQfbs.Distinct().Select(x => $"{x.FirstName} {x.LastName}"))), HttpStatusCode.BadRequest);
            }

            var resultSap = await this.sapDiApi.PostToSapDiApi(listToUpdateSAP, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            await this.pedidosDao.UpdateUserOrders(userOrdersToUpdate);
            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert);

            if (userSaleOrder.Item2.Any())
            {
                var errorParcial = new StringBuilder();
                userSaleOrder.Item2.ForEach(x => errorParcial.Append($"{x}, "));
                throw new CustomServiceException(string.Format(ServiceConstants.ErrirQfbAutomaticoParcial, errorParcial.ToString()), HttpStatusCode.BadRequest);
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
            var hasPedidosType = await this.CheckIfNeedTecnicConfigValidation(manualAssign.DocEntry.Select(x => x.ToString()).ToList(), manualAssign.OrderType);
            var qfbInfoValidated = (await ServiceUtils.GetQfbInfoById(new List<string> { manualAssign.UserId }, this.userService)).FirstOrDefault();
            qfbInfoValidated ??= new QfbTecnicInfoDto();
            if (ServiceShared.CalculateAnd(!qfbInfoValidated.IsValidTecnic, hasPedidosType))
            {
                return ServiceUtils.CreateResult(false, 400, string.Format(ServiceConstants.QfbWithoutTecnic, $"{qfbInfoValidated.QfbFirstName} {qfbInfoValidated.QfbLastName}"), null, null);
            }

            if (manualAssign.OrderType.Equals(ServiceConstants.TypePedido))
            {
                return await this.ReassingarPedido(manualAssign, qfbInfoValidated);
            }
            else
            {
                return await this.ReassignOrders(manualAssign, qfbInfoValidated);
            }
        }

        /// <summary>
        /// Reassigns the Pedidos.
        /// </summary>
        /// <param name="assign">the assign object.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> ReassingarPedido(ManualAssignModel assign, QfbTecnicInfoDto qfbInfoValidated)
        {
            var listSaleOrders = assign.DocEntry.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderBySaleOrder(listSaleOrders)).Where(x => !ServiceConstants.StatusAvoidReasignar.Contains(x.Status)).ToList();
            var listOrderLogToInsert = new List<SalesLogs>();
            orders.ForEach(x =>
            {
                var previousStatus = x.Status;
                x.Status = ServiceShared.CalculateTernary(string.IsNullOrEmpty(x.Productionorderid), ServiceConstants.Liberado, ServiceConstants.Reasignado);
                x.Userid = assign.UserId;
                x.TecnicId = qfbInfoValidated.TecnicId;
                x.StatusForTecnic = ServiceShared.CalculateTernary(string.IsNullOrEmpty(x.Productionorderid), ServiceConstants.Liberado, ServiceConstants.Reasignado);
                x.ReassignmentDate = DateTime.Now;
                x.PackingDate = null;
                if (ServiceShared.CalculateAnd(previousStatus != x.Status, x.IsSalesOrder))
                {
                    /** add logs**/
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assign.UserLogistic, new List<UserOrderModel> { x }));
                }

                if (!x.IsSalesOrder)
                {
                    /** add logs**/
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assign.UserLogistic, new List<UserOrderModel> { x }));
                }
            });

            await this.UpdateOrderSignedByReassignment(orders.Select(x => x.Id).Distinct().ToList());
            if (orders.Any())
            {
                await this.pedidosDao.UpdateUserOrders(orders);
                _ = this.kafkaConnector.PushMessage(listOrderLogToInsert);
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }

        /// <summary>
        /// method to reasign the orders.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> ReassignOrders(ManualAssignModel assignModel, QfbTecnicInfoDto qfbInfoValidated)
        {
            var listOrdersId = assignModel.DocEntry.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(listOrdersId)).ToList();

            var listSales = orders.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrdersBySale = (await this.pedidosDao.GetUserOrderBySaleOrder(listSales)).ToList();

            var listSalesNumber = listSales.Where(y => !string.IsNullOrEmpty(y)).Select(x => int.Parse(x)).ToList();
            var sapOrders = ServiceShared.CalculateTernary(listSalesNumber.Any(), await ServiceUtils.GetOrdersWithFabOrders(this.sapAdapter, listSalesNumber), new List<OrderWithDetailModel>());

            var getUpdateUserOrderModel = AsignarLogic.GetUpdateUserOrderModel(orders, userOrdersBySale, sapOrders, assignModel.UserId, ServiceConstants.Reasignado, assignModel.UserLogistic, false, qfbInfoValidated);
            var ordersToUpdate = getUpdateUserOrderModel.Item1;
            var listOrderLogToInsert = getUpdateUserOrderModel.Item2;

            await this.pedidosDao.UpdateUserOrders(ordersToUpdate);
            await this.UpdateOrderSignedByReassignment(orders.Select(x => x.Id).Distinct().ToList());
            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert);
            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }

        private string GetUserId(
            bool isHeader,
            List<RelationUserDZAndOrdersDZModel> relationOrdersWithUsersDZIsNotOmi,
            Tuple<Dictionary<int, string>, List<int>> userSaleOrder,
            int saleOrderInt,
            bool isClasificationDZ,
            bool isOnlyClasificationDZ,
            int productionId)
        {
            if (isOnlyClasificationDZ)
            {
                return relationOrdersWithUsersDZIsNotOmi.Where(rel => rel.Order.Order.PedidoId.Equals(saleOrderInt)).First().UserId;
            }

            if (!isHeader)
            {
                isClasificationDZ = relationOrdersWithUsersDZIsNotOmi.Any(rel =>
                        ServiceShared.CalculateAnd(
                            rel.Order.Order.PedidoId.Equals(saleOrderInt),
                            rel.Order.Detalle.Any(product => product.OrdenFabricacionId.Equals(productionId))));
            }

            return isClasificationDZ ?
                relationOrdersWithUsersDZIsNotOmi.Where(rel => rel.Order.Order.PedidoId.Equals(saleOrderInt)).First().UserId :
                userSaleOrder.Item1[saleOrderInt];
        }

        /// <summary>
        /// Update Order Signed By Reassignment.
        /// </summary>
        /// <param name="userModelIds">User model ids.</param>
        private async Task UpdateOrderSignedByReassignment(List<int> userModelIds)
        {
            var orderSignatures = (await this.pedidosDao.GetSignaturesByUserOrderId(userModelIds)).ToList();
            orderSignatures.ForEach(x =>
            {
                x.TechnicalSignature = null;
                x.QfbSignature = null;
            });

            await this.pedidosDao.SaveOrderSignatures(orderSignatures);
        }

        private void CalculateInvalidUsers(List<UserModel> invalidQfbs, List<UserModel> allUsers, string userid)
        {
            var invalidUser = allUsers.Where(user =>
                                                ServiceShared.CalculateAnd(
                                                    user.Id.Equals(userid),
                                                    user.TechnicalRequire,
                                                    string.IsNullOrEmpty(user.TecnicId)))
                              .ToList();

            if (!invalidUser.Any())
            {
                return;
            }

            if (!invalidQfbs.Any(invuser => invalidUser.Select(x => x.Id).Contains(invuser.Id)))
            {
                invalidQfbs.AddRange(invalidUser);
            }
        }

        private async Task<bool> CheckIfNeedTecnicConfigValidation(List<string> listSalesOrders, string orderType)
        {
            if (orderType.Equals(ServiceConstants.TypePedido))
            {
                return true;
            }

            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(listSalesOrders)).ToList();
            return userOrders.Any(order => !string.IsNullOrEmpty(order.Salesorderid));
        }
    }
}
