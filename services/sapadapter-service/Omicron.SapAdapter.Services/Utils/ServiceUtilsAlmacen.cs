// <summary>
// <copyright file="ServiceUtilsAlmacen.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Redis;

    /// <summary>
    /// The class for the services.
    /// </summary>
    public static class ServiceUtilsAlmacen
    {
        /// <summary>
        /// gets the date filter for sap.
        /// </summary>
        /// <param name="types">the type to filter.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="lineProducts">the lines products.</param>
        /// <returns>the datetime.</returns>
        public static Tuple<List<CompleteAlmacenOrderModel>, SaleOrderTypeModel> GetSapOrderByType(List<string> types, List<CompleteAlmacenOrderModel> sapOrders, List<string> lineProducts)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();
            var salesTypes = new SaleOrderTypeModel
            {
                LineSaleOrders = new List<int>(),
                MagistralSaleOrders = new List<int>(),
                MixedSaleOrders = new List<int>(),
            };

            var sapOrdersGroup = sapOrders.GroupBy(x => x.DocNum).ToList();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => ServiceShared.CalculateAnd(!x.Any(y => lineProducts.Contains(y.Detalles.ProductoId)), !x.All(y => lineProducts.Contains(y.Detalles.ProductoId))));
                var keys = listMagistral.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keys.Contains(x.DocNum)));
                salesTypes.MagistralSaleOrders = keys;
                sapOrdersGroup.RemoveAll(x => keys.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => ServiceShared.CalculateAnd(x.Any(y => lineProducts.Contains(y.Detalles.ProductoId)), !x.All(y => lineProducts.Contains(y.Detalles.ProductoId))));
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
                salesTypes.MixedSaleOrders = keysMixta;
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => x.All(y => lineProducts.Contains(y.Detalles.ProductoId)));
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
                salesTypes.LineSaleOrders = keysMixta;
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            var ordersMaquila = sapOrders.Where(x => x.TypeOrder.ValidateNull().ToLower() == ServiceConstants.OrderTypeMQ.ToLower()).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Maquila.ToLower()), ordersMaquila, listToReturn);
            salesTypes = AddSalesTypeByOrders(ordersMaquila, salesTypes);

            var ordersSample = sapOrders.Where(x => x.PedidoMuestra.ValidateNull().ToLower() == ServiceConstants.IsSampleOrder.ToLower()).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Muestra.ToLower()), ordersSample, listToReturn);
            salesTypes = AddSalesTypeByOrders(ordersSample, salesTypes);

            var orderPackages = sapOrders.Where(x => x.IsPackage.ValidateNull().ToLower() == ServiceConstants.IsPackage.ToLower()).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Paquetes.ToLower()), orderPackages, listToReturn);
            salesTypes = AddSalesTypeByOrders(orderPackages, salesTypes);

            return new Tuple<List<CompleteAlmacenOrderModel>, SaleOrderTypeModel>(listToReturn.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList(), salesTypes);
        }

        /// <summary>
        /// Get the orders for recepcion pedidos.
        /// </summary>
        /// <param name="sapDao">dao.</param>
        /// <param name="userOrdersTuple">user order tuuple.</param>
        /// <param name="lineProductTuple">line produc tuple.</param>
        /// <param name="needOnlyDxp">if the query only needs the dxp order.</param>
        /// <returns>the orders.</returns>
        public static async Task<List<CompleteAlmacenOrderModel>> GetSapOrderForRecepcionPedidos(ISapDao sapDao, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple, bool needOnlyDxp)
        {
            var idsMagistrales = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();

            var sapOrders = ServiceShared.CalculateTernary(needOnlyDxp, (await sapDao.GetAllOrdersForAlmacenDxp(userOrdersTuple.Item3)).ToList(), (await sapDao.GetAllOrdersForAlmacen(userOrdersTuple.Item3)).ToList());
            sapOrders = sapOrders.Where(x => x.Detalles != null).ToList();
            var arrayOfSaleToProcess = new List<CompleteAlmacenOrderModel>();

            sapOrders.Where(o => o.Canceled == "N").GroupBy(x => x.DocNum).ToList().ForEach(x =>
            {
                if (ServiceShared.CalculateAnd(x.All(y => y.IsMagistral == "Y"), idsMagistrales.Contains(x.Key)))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
                else if (ServiceShared.CalculateAnd(x.All(y => y.IsLine == "Y"), !lineProductTuple.Item2.Contains(x.Key)))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
                else if (ServiceShared.CalculateAnd(x.Any(y => y.IsLine == "Y"), x.Any(y => y.IsMagistral == "Y"), idsMagistrales.Contains(x.Key)))
                {
                    arrayOfSaleToProcess.AddRange(x.ToList());
                }
            });

            arrayOfSaleToProcess.AddRange(sapOrders.Where(o => o.Canceled == "Y"));
            var orderToAppear = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).ToList();

            var ordersSapMaquila = (await sapDao.GetAllOrdersForAlmacenByTypeOrder(ServiceConstants.OrderTypeMQ, orderToAppear)).ToList();
            ordersSapMaquila = ServiceShared.CalculateTernary(needOnlyDxp, ordersSapMaquila.Where(x => !string.IsNullOrEmpty(x.DocNumDxp)).ToList(), ordersSapMaquila);
            arrayOfSaleToProcess.AddRange(ordersSapMaquila.Where(x => x.Detalles != null));

            arrayOfSaleToProcess = arrayOfSaleToProcess.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList();
            return arrayOfSaleToProcess;
        }

        /// <summary>
        /// Gets the user orders for almacen.
        /// </summary>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <returns>the data.</returns>
        public static async Task<Tuple<List<UserOrderModel>, List<int>, DateTime>> GetUserOrdersAlmacenLeftList(IPedidosService pedidosService)
        {
            var userOrderModel = await pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());

            int.TryParse(userOrderModel.Comments.ToString(), out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));

            return new Tuple<List<UserOrderModel>, List<int>, DateTime>(userOrders, new List<int>(), dateToLook);
        }

        /// <summary>
        /// Gets the list of line order based on datetime and user orders.
        /// </summary>
        /// <param name="almacenService">the service.</param>
        /// <param name="magistralIds">the magistralIds.</param>
        /// <param name="maxDate">the max date.</param>
        /// <returns>the data.</returns>
        public static async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProductsAlmacenLeftList(IAlmacenService almacenService, List<int> magistralIds, DateTime maxDate)
        {
            var lineProductsResponse = await almacenService.PostAlmacenOrders(ServiceConstants.GetLineProductPedidos, new AlmacenGetRecepcionModel { MagistralIds = magistralIds, MaxDateToLook = maxDate });
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var listProductToReturn = lineProducts.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), x.StatusAlmacen != ServiceConstants.Almacenado)).ToList();
            listProductToReturn.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).ToList());
            var listIdToIgnore = lineProducts.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), ServiceConstants.StatusToIgnoreLineProducts.Contains(x.StatusAlmacen))).Select(y => y.SaleOrderId).ToList();

            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        /// <summary>
        /// Gets the valids to receive.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="lineProductsModel">the line products.</param>
        /// <param name="sapOrders">the orders.</param>
        /// <returns>the data.</returns>
        public static List<CompleteAlmacenOrderModel> GetOrdersValidsToReceiveByProducts(List<UserOrderModel> userOrders, List<LineProductsModel> lineProductsModel, List<CompleteAlmacenOrderModel> sapOrders)
        {
            var ordersToReturn = new List<CompleteAlmacenOrderModel>();
            var groups = sapOrders.GroupBy(x => x.DocNum).ToList();

            foreach (var p in groups)
            {
                if (p.All(g => g.IsMagistral == "Y"))
                {
                    var saleOrder = userOrders.GetSaleOrderHeader(p.Key.ToString());
                    var familyByOrder = GetFamilyUserOrders(userOrders, p.Key.ToString());
                    var isValid = IsValidUserOrdersToReceive(saleOrder, familyByOrder);
                    ordersToReturn.AddRange(GetOrdersToAdd(isValid, p.ToList()));
                    continue;
                }

                if (p.All(g => g.IsLine == "Y"))
                {
                    var saleOrderLn = lineProductsModel.GetLineProductOrderHeader(p.Key);
                    var userFabLineOrder = GetFamilyLineProducts(lineProductsModel, p.Key);
                    var isValidLineOrder = saleOrderLn == null || saleOrderLn.StatusAlmacen != ServiceConstants.Almacenado;
                    var isValid = ServiceShared.CalculateAnd(isValidLineOrder, userFabLineOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado));
                    ordersToReturn.AddRange(GetOrdersToAdd(isValid, p.ToList()));
                    continue;
                }

                var saleOrderMix = userOrders.GetSaleOrderHeader(p.Key.ToString());
                var familyMixByOrder = GetFamilyUserOrders(userOrders, p.Key.ToString());
                var isValidMg = IsValidUserOrdersToReceive(saleOrderMix, familyMixByOrder);

                var userFabMixLineOrder = GetFamilyLineProducts(lineProductsModel, p.Key);
                var isValidLn = userFabMixLineOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado);
                ordersToReturn.AddRange(GetOrdersToAdd(ServiceShared.CalculateAnd(isValidMg, isValidLn), p.ToList()));
            }

            return ordersToReturn;
        }

        /// <summary>
        /// get total order for doctor.
        /// </summary>
        /// <param name="sapOrders">the saporders.</param>
        /// <param name="localNeighbors">the local neighboors.</param>
        /// <param name="usersOrders">user order.</param>
        /// <param name="payments">the payments.</param>
        /// <returns>the data.</returns>
        public static List<OrderListByDoctorModel> GetTotalOrdersForDoctorAndDxp(List<CompleteRecepcionPedidoDetailModel> sapOrders, List<string> localNeighbors, List<UserOrderModel> usersOrders, List<PaymentsDto> payments)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct().OrderByDescending(x => x);

            foreach (var so in salesIds)
            {
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var productType = ServiceShared.CalculateTernary(orders.All(x => ServiceShared.CalculateAnd(x.Detalles != null, x.Producto.IsMagistral == "Y")), ServiceConstants.Magistral, ServiceConstants.Mixto);
                productType = ServiceShared.CalculateTernary(orders.All(x => ServiceShared.CalculateAnd(x.Detalles != null, x.Producto.IsLine == "Y")), ServiceConstants.Linea, productType);

                var payment = payments.GetPaymentBydocNumDxp(order.DocNumDxp);
                var userOrder = usersOrders.GetSaleOrderHeader(so.ToString());

                listOrders.Add(new OrderListByDoctorModel
                {
                    DocNum = so,
                    InitDate = order?.FechaInicio ?? DateTime.Now,
                    Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, ServiceConstants.PorRecibir),
                    TotalItems = orders.Count,
                    TotalPieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeighbors, order.Address, payment),
                    Comments = userOrder?.Comments ?? string.Empty,
                    OrderType = order.TypeOrder,
                    Address = order.Address.ValidateNull().Replace("\r", " ").Replace("  ", " ").ToUpper(),
                    DxpId = order.DocNumDxp,
                });
            }

            return listOrders;
        }

        /// <summary>
        /// get sap orders by type shipping.
        /// </summary>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="parameters">parameters.</param>
        /// <param name="proccessPayments">the procces payments.</param>
        /// <param name="redisService">the redis service.</param>
        /// <param name="catalogsService">the catalog service.</param>
        /// <returns>sap orders.</returns>
        public static async Task<List<CompleteAlmacenOrderModel>> FilterSapOrdersByTypeShipping(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters, IProccessPayments proccessPayments, IRedisService redisService, ICatalogsService catalogsService)
        {
            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var transactionsIds = sapOrders.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
                var payments = await ServiceShared.GetPaymentsByTransactionsIds(proccessPayments, transactionsIds);
                var localNeigbors = await ServiceUtils.GetLocalNeighbors(catalogsService, redisService);
                sapOrders = sapOrders.Where(x => ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, x.Address.ValidateNull(), payments.GetPaymentBydocNumDxp(x.DocNumDxp)) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            return sapOrders;
        }

        private static List<LineProductsModel> GetFamilyLineProducts(List<LineProductsModel> lineProductsModel, int saleorderId)
        {
            return lineProductsModel.Where(x => ServiceShared.CalculateAnd(x.SaleOrderId == saleorderId, !string.IsNullOrEmpty(x.ItemCode), x.StatusAlmacen != ServiceConstants.Cancelado)).ToList();
        }

        private static List<UserOrderModel> GetFamilyUserOrders(List<UserOrderModel> userOrders, string saleOrderId)
        {
            return userOrders.Where(x => ServiceShared.CalculateAnd(x.Salesorderid == saleOrderId, !string.IsNullOrEmpty(x.Productionorderid), x.Status != ServiceConstants.Cancelado)).ToList();
        }

        private static bool IsValidUserOrdersToReceive(UserOrderModel saleOrder, List<UserOrderModel> familyByOrder)
        {
            return ServiceShared.CalculateAnd(saleOrder?.Status == ServiceConstants.Finalizado, !ServiceConstants.StatusToIgnorePorRecibir.Contains(saleOrder?.StatusAlmacen), familyByOrder.All(x => ServiceShared.CalculateAnd(x.Status == ServiceConstants.Finalizado, x.FinishedLabel == 1)));
        }

        private static List<CompleteAlmacenOrderModel> GetOrdersToAdd(bool isValid, List<CompleteAlmacenOrderModel> listToAdd)
        {
            return ServiceShared.CalculateTernary(isValid, listToAdd, new List<CompleteAlmacenOrderModel>());
        }

        private static List<CompleteAlmacenOrderModel> FilterByContainsType(bool containsFilter, List<CompleteAlmacenOrderModel> ordersToFilter, List<CompleteAlmacenOrderModel> listToReturn)
        {
            if (containsFilter)
            {
                listToReturn.AddRange(ordersToFilter);
                return listToReturn;
            }

            listToReturn = listToReturn.Where(o => !ordersToFilter.Select(om => om.DocNum).Contains(o.DocNum)).ToList();
            return listToReturn;
        }

        private static SaleOrderTypeModel AddSalesTypeByOrders(List<CompleteAlmacenOrderModel> sapOrdersToGetType, SaleOrderTypeModel salesTypes)
        {
            var sapOrdersGroup = sapOrdersToGetType.GroupBy(x => x.DocNum).ToList();
            salesTypes.MagistralSaleOrders.AddRange(sapOrdersGroup.Where(sapOrd => sapOrd.All(prod => prod.IsMagistral == "Y")).Select(sapOrd => sapOrd.Key));
            salesTypes.LineSaleOrders.AddRange(sapOrdersGroup.Where(sapOrd => sapOrd.All(prod => prod.IsLine == "Y")).Select(sapOrd => sapOrd.Key));
            salesTypes.MixedSaleOrders.AddRange(sapOrdersGroup.Where(sapOrd => ServiceShared.CalculateAnd(sapOrd.Any(prod => prod.IsLine == "Y"), sapOrd.Any(prod => prod.IsMagistral == "Y"))).Select(sapOrd => sapOrd.Key));
            return salesTypes;
        }
    }
}
