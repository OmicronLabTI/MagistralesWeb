// <summary>
// <copyright file="ProcessOrdersService.cs" company="Axity">
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
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// Process orders.
    /// </summary>
    public class ProcessOrdersService : IProcessOrdersService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly ISapDiApi sapDiApi;

        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOrdersService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        public ProcessOrdersService(ISapAdapter sapAdapter, ISapDiApi sapDiApi, IPedidosDao pedidosDao)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        public async Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId)
        {
            var listToSend = await this.GetListToCreateFromOrders(pedidosId);
            var dictResult = await this.CreateFabOrders(listToSend);
            var listOrders = await this.GetFabOrdersByIdCode(dictResult[ServiceConstants.Ok]);

            var listPedidos = pedidosId.ListIds.Select(x => x.ToString()).ToList();
            var dataBaseSaleOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(listPedidos)).ToList();

            var listToInsert = this.CreateUserModelOrders(listOrders);
            var dataToInsertUpdate = this.GetListToUpdateInsert(pedidosId.ListIds, dataBaseSaleOrders);
            listToInsert.AddRange(dataToInsertUpdate.Item1);
            var listToUpdate = new List<UserOrderModel>(dataToInsertUpdate.Item2);

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, pedidosId.ListIds, ServiceConstants.OrdenVentaPlan, ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, listOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.OrdenFabricacionPlan, ServiceConstants.OrdenFab));

            await this.pedidosDao.InsertUserOrder(listToInsert);
            await this.pedidosDao.UpdateUserOrders(listToUpdate);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            var userError = dictResult[ServiceConstants.ErrorCreateFabOrd].Any() ? ServiceConstants.ErrorAlInsertar : null;
            return ServiceUtils.CreateResult(true, 200, userError, dictResult[ServiceConstants.ErrorCreateFabOrd], null);
        }

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> ProcessByOrder(ProcessByOrderModel processByOrder)
        {
            var ordersSap = await this.GetOrdersWithDetail(new List<int> { processByOrder.PedidoId });

            var orders = ordersSap.FirstOrDefault(x => x.Order.PedidoId == processByOrder.PedidoId);
            var completeListOrders = orders.Detalle.Count;
            var ordersToCreate = orders.Detalle.Where(x => processByOrder.ProductId.Contains(x.CodigoProducto)).ToList();

            var objectToCreate = this.CreateOrderWithDetail(orders, ordersToCreate);
            var dictResult = await this.CreateFabOrders(new List<OrderWithDetailModel> { objectToCreate });

            var listOrders = await this.GetFabOrdersByIdCode(dictResult[ServiceConstants.Ok]);
            var dataBaseOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { processByOrder.PedidoId.ToString() })).ToList();
            var dataToInsert = this.CreateUserModelOrders(listOrders);

            var saleOrder = dataBaseOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid));
            bool insertUserOrdersale = false;

            if (saleOrder == null)
            {
                saleOrder = new UserOrderModel
                {
                    Salesorderid = processByOrder.PedidoId.ToString(),
                };

                insertUserOrdersale = true;
            }

            saleOrder.Status = dataBaseOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList().Count + dataToInsert.Count == completeListOrders ? ServiceConstants.Planificado : ServiceConstants.Abierto;

            if (insertUserOrdersale)
            {
                dataToInsert.Add(saleOrder);
            }
            else
            {
                await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });
            }

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(processByOrder.UserId, new List<int> { processByOrder.PedidoId }, ServiceConstants.OrdenVentaPlan, ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(processByOrder.UserId, listOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.OrdenFabricacionPlan, ServiceConstants.OrdenFab));

            await this.pedidosDao.InsertUserOrder(dataToInsert);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            var userError = dictResult[ServiceConstants.ErrorCreateFabOrd].Any() ? ServiceConstants.ErrorAlInsertar : null;
            return ServiceUtils.CreateResult(true, 200, userError, dictResult[ServiceConstants.ErrorCreateFabOrd], null);
        }

        /// <summary>
        /// gets the details with the order.
        /// </summary>
        /// <param name="listSalesOrder">the list ids.</param>
        /// <returns>the data.</returns>
        private async Task<List<OrderWithDetailModel>> GetOrdersWithDetail(List<int> listSalesOrder)
        {
            var sapResponse = await this.sapAdapter.PostSapAdapter(listSalesOrder, ServiceConstants.GetOrderWithDetail);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));
            return ordersSap;
        }

        /// <summary>
        /// Gets the orders by dict with ok value id-itemCode.
        /// </summary>
        /// <param name="listToLook">the list of values.</param>
        /// <returns>the data.</returns>
        private async Task<List<FabricacionOrderModel>> GetFabOrdersByIdCode(List<string> listToLook)
        {
            var prodOrders = await this.sapAdapter.PostSapAdapter(listToLook, ServiceConstants.GetProdOrderByOrderItem);
            var listOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(prodOrders.Response.ToString());
            return listOrders;
        }

        /// <summary>
        /// Gets the list to create by pedidos.
        /// </summary>
        /// <param name="pedidosId">the pedidos id.</param>
        /// <returns>the data.</returns>
        private async Task<List<OrderWithDetailModel>> GetListToCreateFromOrders(ProcessOrderModel pedidosId)
        {
            var ordersSap = await this.GetOrdersWithDetail(pedidosId.ListIds);
            var listToSend = new List<OrderWithDetailModel>();

            ordersSap.ForEach(o =>
            {
                var listDetalle = new List<CompleteDetailOrderModel>();

                o.Detalle
                .Where(x => string.IsNullOrEmpty(x.Status))
                .ToList()
                .ForEach(y =>
                {
                    y.DescripcionProducto = y.DescripcionCorta;
                    listDetalle.Add(y);
                });

                var objectToCreate = new OrderWithDetailModel { Order = o.Order, Detalle = listDetalle };
                listToSend.Add(objectToCreate);
            });

            return listToSend;
        }

        /// <summary>
        /// Returns the values of the creation in Sap.
        /// </summary>
        /// <param name="listToSend">the data to send.</param>
        /// <returns>the values.</returns>
        private async Task<Dictionary<string, List<string>>> CreateFabOrders(List<OrderWithDetailModel> listToSend)
        {
            var resultSap = await this.sapDiApi.PostToSapDiApi(listToSend, ServiceConstants.CreateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());

            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            return new Dictionary<string, List<string>>
            {
                { ServiceConstants.Ok, listToLook },
                { ServiceConstants.ErrorCreateFabOrd, listErrorId },
            };
        }

        /// <summary>
        /// creates the user model from fabrication.
        /// </summary>
        /// <param name="dataToCreate">the data to create.</param>
        /// <returns>the data.</returns>
        private List<UserOrderModel> CreateUserModelOrders(List<FabricacionOrderModel> dataToCreate)
        {
            var listToReturn = new List<UserOrderModel>();
            dataToCreate.ForEach(x =>
            {
                listToReturn.Add(new UserOrderModel
                {
                    Productionorderid = x.OrdenId.ToString(),
                    Salesorderid = x.PedidoId.ToString(),
                    Status = ServiceConstants.Planificado,
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the list To update or insert.
        /// </summary>
        /// <param name="pedidosId">the pedidos id.</param>
        /// <param name="dataBaseSaleOrders">the database sale orders.</param>
        /// <returns>the first is the list to insert the second the list to update.</returns>
        private Tuple<List<UserOrderModel>, List<UserOrderModel>> GetListToUpdateInsert(List<int> pedidosId, List<UserOrderModel> dataBaseSaleOrders)
        {
            var listToInsert = new List<UserOrderModel>();
            var listToUpdate = new List<UserOrderModel>();

            pedidosId.ForEach(p =>
            {
                var insertUserOrdersale = false;
                var saleOrder = dataBaseSaleOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid.Equals(p.ToString()));

                if (saleOrder == null)
                {
                    saleOrder = new UserOrderModel
                    {
                        Salesorderid = p.ToString(),
                    };

                    insertUserOrdersale = true;
                }

                saleOrder.Status = ServiceConstants.Planificado;

                if (insertUserOrdersale)
                {
                    listToInsert.Add(saleOrder);
                }
                else
                {
                    listToUpdate.Add(saleOrder);
                }
            });

            return new Tuple<List<UserOrderModel>, List<UserOrderModel>>(listToInsert, listToUpdate);
        }

        /// <summary>
        /// creates the order detail.
        /// </summary>
        /// <param name="order">the order.</param>
        /// <param name="listToSend">list to send.</param>
        /// <returns>the data.</returns>
        private OrderWithDetailModel CreateOrderWithDetail(OrderWithDetailModel order, List<CompleteDetailOrderModel> listToSend)
        {
            var listUpdated = new List<CompleteDetailOrderModel>();

            listToSend.ForEach(x =>
            {
                x.DescripcionProducto = x.DescripcionCorta;
                listUpdated.Add(x);
            });

            return new OrderWithDetailModel
            {
                Order = new OrderModel
                {
                    PedidoId = order.Order.PedidoId,
                    FechaInicio = order.Order.FechaInicio,
                    FechaFin = order.Order.FechaFin,
                },
                Detalle = new List<CompleteDetailOrderModel>(listUpdated),
            };
        }
    }
}
