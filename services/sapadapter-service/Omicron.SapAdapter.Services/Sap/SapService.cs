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
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapService : ISapService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="pedidosService">the pedidosservice.</param>
        /// <param name="userService">user service.</param>
        public SapService(ISapDao sapDao, IPedidosService pedidosService, IUsersService userService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.usersService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <param name="parameters">The params.</param>
        /// <returns>get the orders.</returns>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var dateFilter = ServiceUtils.GetDateFilter(parameters);
            var orders = await this.GetSapDbOrders(parameters, dateFilter);

            var userOrderModel = await this.pedidosService.GetUserPedidos(orders.Select(x => x.DocNum).Distinct().ToList());
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
            var listUsers = await this.GetUsers(userOrders);

            orders = orders.DistinctBy(x => x.DocNum).ToList();
            orders = this.FilterList(orders, parameters, userOrders, listUsers);

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var ordersOrdered = orders.OrderBy(o => o.DocNum).ToList();
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

            var usersOrderModel = await this.pedidosService.GetUserPedidos(new List<int> { docId });
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(usersOrderModel.Response.ToString());

            var listUsers = await this.GetUsers(userOrders);

            details.ToList().ForEach(x =>
            {
                var userOrder = userOrders.FirstOrDefault(y => y.Productionorderid == x.OrdenFabricacionId.ToString());
                var userId = userOrder == null ? string.Empty : userOrder.Userid;
                var user = listUsers.FirstOrDefault(y => y.Id.Equals(userId));
                x.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";

                x.Status = userOrder == null ? string.Empty : userOrder.Status;
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
        /// Gets the production orders bu produc and id.
        /// </summary>
        /// <param name="pedidosIds">list ids each elemente is orderId-producId.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetProdOrderByOrderItem(List<string> pedidosIds)
        {
            var result = new List<OrdenFabricacionModel>();

            foreach (var o in pedidosIds)
            {
                var data = o.Split("-");
                int.TryParse(data[0], out int pedidoId);

                var order = await this.sapDao.GetProdOrderByOrderProduct(pedidoId, data[1]);
                result.Add(order);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, JsonConvert.SerializeObject(result), null, null);
        }

        /// <summary>
        /// Gets the formula of the orden de fabricaion.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <param name="returnFirst">if it returns only the first.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetOrderFormula(List<int> listIds, bool returnFirst)
        {
            var ordenFab = (await this.sapDao.GetFabOrderById(listIds)).ToList();
            var listToReturn = new List<CompleteFormulaWithDetalle>();
            var dictUser = new Dictionary<int, string>();

            foreach (var o in ordenFab)
            {
                if (!dictUser.ContainsKey(o.User))
                {
                    var user = await this.sapDao.GetSapUserById(o.User);
                    dictUser.Add(user.UserId, user.UserName);
                }

                var pedido = (await this.sapDao.GetPedidoById(o.PedidoId)).FirstOrDefault();

                var formulaDetalle = new CompleteFormulaWithDetalle
                {
                    IsChecked = false,
                    ProductionOrderId = o.OrdenId,
                    Code = o.ProductoId,
                    ProductDescription = o.ProdName,
                    Type = ServiceConstants.DictStatusType.ContainsKey(o.Type) ? ServiceConstants.DictStatusType[o.Type] : o.Type,
                    Status = ServiceConstants.DictStatus.ContainsKey(o.Status) ? ServiceConstants.DictStatus[o.Status] : o.Status,
                    PlannedQuantity = (int)o.Quantity,
                    Unit = o.Unit,
                    Warehouse = o.Wharehouse,
                    Number = o.PedidoId,
                    FabDate = o.CreatedDate.ToString("dd/MM/yyyy"),
                    DueDate = o.DueDate.ToString("dd/MM/yyyy"),
                    StartDate = o.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = o.PostDate.ToString("dd/MM/yyyy"),
                    User = dictUser[o.User],
                    Origin = ServiceConstants.DictStatusOrigin.ContainsKey(o.OriginType) ? ServiceConstants.DictStatusOrigin[o.OriginType] : o.OriginType,
                    BaseDocument = o.PedidoId,
                    Client = o.CardCode,
                    CompleteQuantity = (int)o.CompleteQuantity,
                    RealEndDate = o.PostDate.ToString("dd/MM/yyyy"),
                    ProductLabel = pedido == null ? string.Empty : pedido.Label,
                    Container = pedido == null ? string.Empty : pedido.Container,
                    Comments = o.Comments,
                    Details = (await this.sapDao.GetDetalleFormula(o.OrdenId)).ToList(),
                };

                listToReturn.Add(formulaDetalle);
            }

            if (returnFirst)
            {
                return ServiceUtils.CreateResult(true, 200, null, listToReturn.FirstOrDefault(), null, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, listToReturn.Count);
        }

        /// <summary>
        /// gets the items from the dict.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetComponents(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                throw new CustomServiceException(ServiceConstants.NoChipsError);
            }

            var listValues = new List<CompleteDetalleFormulaModel>();
            var chipValues = parameters[ServiceConstants.Chips].Split(ServiceConstants.ChipSeparator).ToList();

            var firstChip = chipValues.FirstOrDefault();
            listValues.AddRange((await this.sapDao.GetItemsByContainsItemCode(firstChip)).ToList());
            listValues.AddRange((await this.sapDao.GetItemsByContainsDescription(firstChip)).ToList());
            listValues = listValues.DistinctBy(p => p.ProductId).ToList();

            foreach (var v in chipValues)
            {
                listValues = listValues.Where(x => $"{x.ProductId} {x.Description}".Contains(v)).ToList();
            }

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var produtOrdered = listValues.OrderBy(o => o.ProductId).ToList();
            var productToReturn = produtOrdered.Skip(offsetNumber).Take(limitNumber).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, productToReturn, null, produtOrdered.Count());
        }

        /// <summary>
        /// gets the orders from sap.
        /// </summary>
        /// <param name="parameters">the filter from front.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <returns>teh orders.</returns>
        private async Task<List<CompleteOrderModel>> GetSapDbOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                return (await this.sapDao.GetAllOrdersById(docNum)).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.FechaInicio))
            {
                return (await this.sapDao.GetAllOrdersByFechaIni(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
            else
            {
                return (await this.sapDao.GetAllOrdersByFechaFin(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
        }

        /// <summary>
        /// filters the list by the params.
        /// </summary>
        /// <param name="orderModels">the list of data.</param>
        /// <param name="parameters">the params.</param>
        /// <param name="userOrder">the usr orders.</param>
        /// <param name="users">the users.</param>
        /// <returns>the data.</returns>
        private List<CompleteOrderModel> FilterList(List<CompleteOrderModel> orderModels, Dictionary<string, string> parameters, List<UserOrderModel> userOrder, List<UserModel> users)
        {
            orderModels.ForEach(x =>
            {
                var order = userOrder.FirstOrDefault(u => u.Salesorderid == x.DocNum.ToString() && string.IsNullOrEmpty(u.Productionorderid));
                x.Qfb = order == null ? string.Empty : order.Userid;

                if (x.PedidoStatus == "O")
                {
                    x.PedidoStatus = ServiceConstants.Abierto;
                }

                x.PedidoStatus = order == null ? x.PedidoStatus : order.Status;
            });

            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docId);
                var ordersById = orderModels.FirstOrDefault(x => x.DocNum == docId);
                return new List<CompleteOrderModel> { ordersById };
            }

            if (parameters.ContainsKey(ServiceConstants.Status))
            {
                orderModels = orderModels.Where(x => x.PedidoStatus == parameters[ServiceConstants.Status]).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.Qfb))
            {
                orderModels = orderModels.Where(x => !string.IsNullOrEmpty(x.Qfb) && x.Qfb.Equals(parameters[ServiceConstants.Qfb])).ToList();
            }

            orderModels.ForEach(x =>
            {
                var user = users.FirstOrDefault(y => y.Id.Equals(x.Qfb));
                x.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
            });

            return orderModels;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="userOrders">the user order model.</param>
        /// <returns>the data.</returns>
        private async Task<List<UserModel>> GetUsers(List<UserOrderModel> userOrders)
        {
            var userIDs = userOrders.Where(x => !string.IsNullOrEmpty(x.Userid)).Select(x => x.Userid).Distinct().ToList();
            var users = await this.usersService.GetUsersById(userIDs);
            return JsonConvert.DeserializeObject<List<UserModel>>(users.Response.ToString());
        }
    }
}
