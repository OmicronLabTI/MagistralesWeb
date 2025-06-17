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
    using System.Reflection.PortableExecutable;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Redis;
    using StackExchange.Redis;

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
        /// <param name="groupByDxpOrder">Group by DXP order.</param>
        /// <returns>the datetime.</returns>
        public static List<CompleteAlmacenOrderModel> GetSapOrderByType(List<string> types, List<CompleteAlmacenOrderModel> sapOrders, bool groupByDxpOrder = false)
        {
            var clasificationFilters = types.Where(x => !ServiceConstants.DefaultFilters.Contains(x)).ToList();

            var listToReturn = groupByDxpOrder ?
                    FilterOrdersByClassificationGroupedByDxpOrder(sapOrders, clasificationFilters, types) :
                    FilterOrdersByClassification(sapOrders, clasificationFilters, types);

            var ordersMaquila = sapOrders.Where(x => x.TypeOrder.ValidateNull().ToLower() == ServiceConstants.OrderTypeMQ.ToLower()).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Maquila.ToLower()), ordersMaquila, listToReturn);

            var ordersSample = sapOrders.Where(x => x.PedidoMuestra.ValidateNull() == ServiceConstants.IsSampleOrder).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Muestra.ToLower()), ordersSample, listToReturn);

            var orderPackages = sapOrders.Where(x => x.IsPackage.ValidateNull().ToLower() == ServiceConstants.IsPackage.ToLower()).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.Paquetes.ToLower()), orderPackages, listToReturn);

            var ordersOmigenomics = sapOrders.Where(x => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsSecondary))).ToList();
            listToReturn = FilterByContainsType(types.Contains(ServiceConstants.OmigenomicsGroup.ToLower()), ordersOmigenomics, listToReturn);

            return listToReturn.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList();
        }

        /// <summary>
        /// Get the orders for recepcion pedidos.
        /// </summary>
        /// <param name="sapDao">dao.</param>
        /// <param name="sapOrdersByTransactionId">sapOrdersByTransactionId.</param>
        /// <param name="hasChips">HasChips.</param>
        /// <param name="userOrders">userOrders.</param>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">endDate.</param>
        /// <param name="lineProductTuple">line produc tuple.</param>
        /// <param name="needOnlyDxp">if the query only needs the dxp order.</param>
        /// <param name="catalogsService">the catalogs service.</param>
        /// <param name="redisService">the redis service.</param>
        /// <returns>the orders.</returns>
        public static async Task<List<CompleteAlmacenOrderModel>> GetSapOrderForRecepcionPedidos(
            ISapDao sapDao,
            List<CompleteAlmacenOrderModel> sapOrdersByTransactionId,
            bool hasChips,
            List<UserOrderModel> userOrders,
            DateTime startDate,
            DateTime endDate,
            Tuple<List<LineProductsModel>, List<int>> lineProductTuple,
            bool needOnlyDxp,
            ICatalogsService catalogsService,
            IRedisService redisService)
        {
            var idsMagistrales = userOrders.Select(x => int.Parse(x.Salesorderid)).Distinct();
            var sapOrders = await GetSapInformation(sapDao, sapOrdersByTransactionId, hasChips, startDate, endDate, needOnlyDxp);
            sapOrders = sapOrders.Where(x => x.Detalles != null).ToList();
            var arrayOfSaleToProcess = new List<CompleteAlmacenOrderModel>();

            var sapOrdersConfiguration = await ServiceUtils.GetRouteConfigurationsForProducts(catalogsService, redisService, ServiceConstants.AlmacenDbValue);
            sapOrdersConfiguration.ClassificationCodes.AddRange(new List<string> { ServiceConstants.OrderTypeMQ, ServiceConstants.OrderTypeMU, ServiceConstants.OrderTypePackage });

            sapOrders.Where(o => o.Canceled == "N").GroupBy(x => x.DocNum).ToList().ForEach(orders =>
            {
                var hasProductsWithValidConfig = orders
                    .Where(x => ((sapOrdersConfiguration.ClassificationCodes.Contains(x.TypeOrder) &&
                                !sapOrdersConfiguration.ItemCodesExcludedByException.Contains(x.Detalles.ProductoId)) ||
                                sapOrdersConfiguration.ItemCodesIncludedByConfigRules.Contains(x.Detalles.ProductoId)) && x.ProductionOrderId == 0).Count() > 0;
                if (lineProductTuple.Item2.Contains(orders.Key) || hasProductsWithValidConfig)
                {
                    arrayOfSaleToProcess.AddRange(orders.ToList());
                }

                if (idsMagistrales.Contains(orders.Key))
                {
                    arrayOfSaleToProcess.AddRange(orders.ToList());
                }
            });
            arrayOfSaleToProcess.AddRange(sapOrders.Where(o => o.Canceled == "Y"));
            var orderToAppear = userOrders.Select(x => int.Parse(x.Salesorderid));

            var ordersSapMaquila = await sapDao.GetAllOrdersForAlmacenByTypeOrder(ServiceConstants.OrderTypeMQ, orderToAppear.ToList(), needOnlyDxp);
            arrayOfSaleToProcess.AddRange(ordersSapMaquila.Where(x => x.Detalles != null));
            return arrayOfSaleToProcess.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList();
        }

        /// <summary>
        /// Gets the user orders for almacen.
        /// </summary>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="hasChips">hasChips.</param>
        /// <param name="ordersIds">Orders Ids.</param>
        /// <param name="startDate">StartDate.</param>
        /// <param name="endDate">EndDate.</param>
        /// <returns>the data.</returns>
        public static async Task<List<UserOrderModel>> GetUserOrdersAlmacenLeftList(
            IPedidosService pedidosService,
            bool hasChips,
            List<int> ordersIds,
            DateTime startDate,
            DateTime endDate)
        {
            var userOrderModel = new ResultDto();
            if (hasChips)
            {
                userOrderModel = await pedidosService.PostPedidos(
                ordersIds,
                ServiceConstants.EndpointGetUserOrdersAlmancenByOrdersId);
                return JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
            }

            userOrderModel = await pedidosService.GetUserPedidos(
                string.Format(ServiceConstants.EndpointGetUserOrdersAlmancenByRangeDate, startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy")));
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
        }

        /// <summary>
        /// Gets the list of line order based on datetime and user orders.
        /// </summary>
        /// <param name="almacenService">the service.</param>
        /// <param name="ordersIds">ordersIds.</param>
        /// <param name="hasChips">hasChips.</param>
        /// <param name="magistralIds">the magistralIds.</param>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">endDate.</param>
        /// <returns>the data.</returns>
        public static async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProductsAlmacenLeftList(
            IAlmacenService almacenService,
            List<int> ordersIds,
            bool hasChips,
            List<int> magistralIds,
            DateTime startDate,
            DateTime endDate)
        {
            var lineProductsResponse = await almacenService.PostAlmacenOrders(
                ServiceConstants.EndPointGetLineProductPedidosByRangeDate,
                new OrdersFilterDto
                {
                    MagistralIds = magistralIds,
                    StartDate = startDate,
                    EndDate = endDate,
                    HasChips = hasChips,
                    OrdersIds = ordersIds,
                });

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
                if (p.All(g => g.ProductionOrderId != 0))
                {
                    var saleOrder = userOrders.GetSaleOrderHeader(p.Key.ToString());
                    var familyByOrder = GetFamilyUserOrders(userOrders, p.Key.ToString());
                    var isValid = IsValidUserOrdersToReceive(saleOrder, familyByOrder);
                    ordersToReturn.AddRange(GetOrdersToAdd(isValid, p.ToList()));
                    continue;
                }

                if (p.All(g => g.ProductionOrderId == 0))
                {
                    var saleOrderLn = lineProductsModel.GetLineProductOrderHeader(p.Key);
                    var userFabLineOrder = GetFamilyLineProducts(lineProductsModel, p.Key);
                    var isValidLineOrder = saleOrderLn == null || saleOrderLn.StatusAlmacen != ServiceConstants.Almacenado;
                    var isValid = ServiceShared.CalculateAnd(isValidLineOrder, userFabLineOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && x.StatusAlmacen != ServiceConstants.Empaquetado));
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
        /// <param name="classifications">the classifications.</param>
        /// <returns>the data.</returns>
        public static List<OrderListByDoctorModel> GetTotalOrdersForDoctorAndDxp(List<CompleteRecepcionPedidoDetailModel> sapOrders, List<string> localNeighbors, List<UserOrderModel> usersOrders, List<PaymentsDto> payments, List<ClassificationsModel> classifications)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct().OrderByDescending(x => x);

            foreach (var so in salesIds)
            {
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var payment = payments.GetPaymentBydocNumDxp(order.DocNumDxp);
                var userOrder = usersOrders.GetSaleOrderHeader(so.ToString());
                var typeOrder = ServiceUtils.GetOrderTypeDescription(orders.Select(x => x.TypeOrder).Distinct().ToList(), classifications);
                listOrders.Add(new OrderListByDoctorModel
                {
                    DocNum = so,
                    InitDate = order?.FechaInicio ?? DateTime.Now,
                    Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, ServiceConstants.PorRecibir),
                    TotalItems = orders.Count,
                    TotalPieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                    TypeSaleOrder = $"Pedido {typeOrder}",
                    InvoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeighbors, order.Address, payment),
                    Comments = userOrder?.Comments ?? string.Empty,
                    OrderType = order.TypeOrder,
                    Address = order.Address.ValidateNull().Replace("\r", " ").Replace("  ", " ").ToUpper(),
                    DxpId = order.DocNumDxp.GetShortShopTransaction(),
                    IsPackage = order.IsPackage == ServiceConstants.IsPackage,
                    IsOmigenomics = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(order.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(order.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(order.IsSecondary)),
                });
            }

            return listOrders;
        }

        /// <summary>
        /// filter orders by config.
        /// </summary>
        /// <param name="sapOrders">the saporders.</param>
        /// <param name="userOrders">user order.</param>
        /// <param name="lineOrders">the lineOrders.</param>
        /// <param name="catalogsService">the catalogsService.</param>
        /// <param name="redisService">the redisService.</param>
        /// <returns>the data.</returns>
        public static async Task<List<CompleteRecepcionPedidoDetailModel>> GetFilterSapOrdersByConfig(List<CompleteRecepcionPedidoDetailModel> sapOrders, List<UserOrderModel> userOrders, List<LineProductsModel> lineOrders, ICatalogsService catalogsService, IRedisService redisService)
        {
            var sapOrdersConfiguration = await ServiceUtils.GetRouteConfigurationsForProducts(catalogsService, redisService, ServiceConstants.AlmacenDbValue);
            sapOrdersConfiguration.ClassificationCodes.AddRange(new List<string> { ServiceConstants.OrderTypeMQ, ServiceConstants.OrderTypeMU, ServiceConstants.OrderTypePackage });
            var usersOrdersIds = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(x => int.Parse(x.Productionorderid));
            var sapOrdersFiltered = new List<CompleteRecepcionPedidoDetailModel>();
            sapOrders.ForEach(order =>
            {
                var hasProductsWithValidConfig = ServiceShared.CalculateAnd(sapOrdersConfiguration.ClassificationCodes.Contains(order.TypeOrder), !sapOrdersConfiguration.ItemCodesExcludedByException.Contains(order.Detalles.ProductoId));
                var second = ServiceShared.CalculateAnd(sapOrdersConfiguration.ItemCodesIncludedByConfigRules.Contains(order.Detalles.ProductoId), string.IsNullOrEmpty(order.FabricationOrder));
                var isInLineOrders = lineOrders.Any(x => ServiceShared.CalculateAnd(x.SaleOrderId == order.DocNum, x.ItemCode == order.Detalles.ProductoId));

                if (ServiceShared.CalculateOr(isInLineOrders || hasProductsWithValidConfig || second))
                {
                    sapOrdersFiltered.Add(order);
                }

                var validFabOrder = int.TryParse(order.FabricationOrder, out int fabOrderId);
                if (validFabOrder && usersOrdersIds.Contains(fabOrderId))
                {
                    sapOrdersFiltered.Add(order);
                }
            });

            return sapOrdersFiltered.DistinctBy(x => new { x.DocNum, x.Detalles.ProductoId }).ToList();
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

        private static List<CompleteAlmacenOrderModel> FilterOrdersByClassificationGroupedByDxpOrder(List<CompleteAlmacenOrderModel> sapOrders, List<string> clasificationFilters, List<string> types)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();
            var sapOrdersGroup = sapOrders.GroupBy(x => x.DocNumDxp).ToList();
            clasificationFilters.ForEach(c =>
            {
                var keysOrderPerClassification = sapOrdersGroup.Where(so => so.All(det => det.TypeOrder == c)).Select(x => x.Key).ToList();
                listToReturn.AddRange(sapOrders.Where(x => keysOrderPerClassification.Contains(x.DocNumDxp)));
                sapOrdersGroup.RemoveAll(x => keysOrderPerClassification.Contains(x.Key));
            });

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(so => so.Select(x => x.TypeOrder).Distinct().Count() > 1);
                var keysMixta = listMixta.Select(x => x.Key).ToList();
                listToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNumDxp)));
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            return listToReturn;
        }

        private static List<CompleteAlmacenOrderModel> FilterOrdersByClassification(List<CompleteAlmacenOrderModel> sapOrders, List<string> clasificationFilters, List<string> types)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();
            var clasificationOrders = new List<CompleteAlmacenOrderModel>();

            clasificationFilters.ForEach(c =>
            {
                clasificationOrders = sapOrders.Where(so => so.TypeOrder.ValidateNull().Equals(c, StringComparison.CurrentCultureIgnoreCase)).ToList();
                listToReturn = FilterByContainsType(types.Contains(c), clasificationOrders, listToReturn);
            });

            return listToReturn;
        }

        private static async Task<List<CompleteAlmacenOrderModel>> GetSapInformation(
           ISapDao sapDao,
           List<CompleteAlmacenOrderModel> sapOrdersByTransactionId,
           bool hasChips,
           DateTime startDate,
           DateTime endDate,
           bool needOnlyDxp)
        {
            {
                if (hasChips)
                {
                    return sapOrdersByTransactionId;
                }

                return needOnlyDxp ?
                    (await sapDao.GetAllOrdersForAlmacenDxp(startDate, endDate)).ToList() :
                    (await sapDao.GetAllOrdersForAlmacen(startDate, endDate)).ToList();
            }
        }
    }
}
