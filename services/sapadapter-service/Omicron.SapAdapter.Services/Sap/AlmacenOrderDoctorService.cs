// <summary>
// <copyright file="AlmacenOrderDoctorService.cs" company="Axity">
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
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for almacen order doctor service.
    /// </summary>
    public class AlmacenOrderDoctorService : IAlmacenOrderDoctorService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDoctorService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        public AlmacenOrderDoctorService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters)
        {
            var typesString = ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.AllTypesByDoctor);
            var types = typesString.Split(",").ToList();

            var userOrdersTuple = await this.GetUserOrders();
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await this.GetLineProducts(ids, userOrdersTuple.Item3);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple, types);
            var ordersByFilter = await this.GetSapOrdersToLookByDoctor(sapOrders, parameters);
            var totalFilter = ordersByFilter.Select(x => x.Medico).Distinct().ToList().Count;

            var listToReturn = this.GetCardOrdersToReturn(ordersByFilter, parameters);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersDetailsByDoctor(DoctorOrdersSearchDeatilDto details)
        {
            var sapOrders = await this.sapDao.GetSapOrderDetailForAlmacenRecepcionById(details.SaleOrders);
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var userOrdersResponse = await this.pedidosService.PostPedidos(details.SaleOrders, ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            var saleModel = new SalesByDoctorModel();

            saleModel.AlmacenHeaderByDoctor = new AlmacenSalesByDoctorHeaderModel
            {
                Address = details.Address,
                Doctor = details.Name,
                TotalItems = sapOrders.Count(y => y.Detalles != null),
                TotalPieces = sapOrders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
            };

            saleModel.Items = this.GetTotalOrdersByDoctor(sapOrders.ToList(), localNeigbors, userOrders);

            return ServiceUtils.CreateResult(true, 200, null, saleModel, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrderdetail(int saleorderid)
        {
            var details = await this.sapDao.GetPedidoByIdJoinProduct(saleorderid);
            var productItems = await this.sapDao.GetProductByIds(details.Select(x => x.ProductoId).ToList());
            var saleDetails = (await this.sapDao.GetAllDetails(new List<int?> { saleorderid })).ToList();
            var listDetails = new List<AlmacenDetailsOrder>();
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { saleorderid });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());
            foreach (var detail in details)
            {
                var item = productItems.FirstOrDefault(x => x.ProductoId == detail.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;
                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == detail.ProductoId);
                var orderId = saleDetail == null ? string.Empty : saleDetail.OrdenFabricacionId.ToString();
                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;

                var incidentdb = incidents.FirstOrDefault(x => x.SaleOrderId == saleorderid && x.ItemCode == item.ProductoId);
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = !string.IsNullOrEmpty(incidentdb.Batches) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(incidentdb.Batches) : new List<AlmacenBatchModel>(),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };
                var detailItem = new AlmacenDetailsOrder
                {
                    ItemCode = itemcode,
                    Description = item.LargeDescription.ToUpper(),
                    NeedsCooling = item.NeedsCooling,
                    ProductType = $"Producto {productType}",
                    Pieces = detail.Quantity,
                    Container = detail.Container,
                    Status = detail.CanceledOrder == "Y" ? ServiceConstants.Cancelado : ServiceConstants.PorRecibir,
                    Incident = string.IsNullOrEmpty(localIncident.Status) ? null : localIncident,
                };
                listDetails.Add(detailItem);
            }

            return ServiceUtils.CreateResult(true, 200, null, listDetails, null, null);
        }

        private async Task<Tuple<List<UserOrderModel>, List<int>, DateTime>> GetUserOrders()
        {
            var userOrderModel = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());

            int.TryParse(userOrderModel.Comments.ToString(), out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));

            return new Tuple<List<UserOrderModel>, List<int>, DateTime>(userOrders, new List<int>(), dateToLook);
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProducts(List<int> magistralIds, DateTime maxDate)
        {
            var lineProductsResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLineProductPedidos, new AlmacenGetRecepcionModel { MagistralIds = magistralIds, MaxDateToLook = maxDate });
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var listProductToReturn = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).ToList();
            listProductToReturn.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).ToList());
            var listIdToIgnore = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && ServiceConstants.StatusToIgnoreLineProducts.Contains(x.StatusAlmacen)).Select(y => y.SaleOrderId).ToList();

            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        /// <summary>
        /// Gets the sap orders.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<CompleteAlmacenOrderModel>> GetSapOrders(Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple, List<string> types)
        {
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(this.sapDao, userOrdersTuple, lineProductTuple);
            var sapCancelled = sapOrders.Where(x => x.Canceled == "Y").ToList();
            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !userOrdersTuple.Item1.Any(y => y.Salesorderid == x.DocNum.ToString())).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();

            sapOrders = this.GetOrdersValidsToReceiveByProducts(userOrdersTuple.Item1, lineProductTuple.Item1, sapOrders);
            sapOrders = sapOrders.Where(x => x.PedidoMuestra != ServiceConstants.OrderTypeMU).ToList();
            sapOrders.AddRange(sapCancelled);
            return ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, lineProducts).Item1;
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(ServiceConstants.Shipping) && parameters[ServiceConstants.Shipping].Split(",").Count() == 1)
            {
                var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                sapOrders = sapOrders.Where(x => ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, x.Address.ValidateNull()) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            var doctorName = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => doctorName.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets card for doctor.
        /// </summary>
        /// <returns>the data.</returns>
        private AlmacenOrdersByDoctorModel GetCardOrdersToReturn(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            var doctors = sapOrders.Select(x => x.Medico).Distinct().OrderBy(x => x).ToList();
            doctors = this.GetDoctorsToProcess(doctors, parameters);

            var listToReturn = new AlmacenOrdersByDoctorModel
            {
                SalesOrders = new List<SalesByDoctorModel>(),
                TotalSalesOrders = 0,
            };

            foreach (var doctor in doctors)
            {
                var orders = sapOrders.Where(x => x.Medico == doctor).ToList();
                var totalOrders = orders.DistinctBy(x => x.DocNum).Count();
                var totalItems = orders.DistinctBy(y => new { y.DocNum, y.Detalles.ProductoId }).Count();
                var doctorAddress = sapOrders.FirstOrDefault(x => x.Medico == doctor);
                var address = doctorAddress?.Address?.Replace("\r", " ").Replace("  ", " ").ToUpper() ?? string.Empty;

                var sale = new AlmacenSalesByDoctorModel
                {
                    Doctor = doctor,
                    Address = address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                    SaleOrderId = orders.Select(x => x.DocNum).Distinct().ToList(),
                };

                var saleModel = new SalesByDoctorModel
                {
                    AlmacenSalesByDoctor = sale,
                    AlmacenHeaderByDoctor = null,
                    Items = null,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        private List<OrderListByDoctorModel> GetTotalOrdersByDoctor(List<CompleteRecepcionPedidoDetailModel> sapOrders, List<string> localNeighbors, List<UserOrderModel> usersOrders)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct().OrderByDescending(x => x);

            foreach (var so in salesIds)
            {
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var productType = orders.All(x => x.Detalles != null && x.Producto.IsMagistral == "Y") ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = orders.All(x => x.Detalles != null && x.Producto.IsLine == "Y") ? ServiceConstants.Linea : productType;

                var orderType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, order.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo;

                var userOrder = usersOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));

                var saleItem = new OrderListByDoctorModel
                {
                    DocNum = so,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = order.Canceled == "Y" ? ServiceConstants.Cancelado : ServiceConstants.PorRecibir,
                    TotalItems = orders.Count,
                    TotalPieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = orderType,
                    Comments = userOrder == null ? string.Empty : userOrder.Comments,
                    OrderType = order.TypeOrder,
                    Address = order.Address.ValidateNull().Replace("\r", " ").Replace("  ", " ").ToUpper(),
                };
                listOrders.Add(saleItem);
            }

            return listOrders;
        }

        private List<string> GetDoctorsToProcess(List<string> doctors, Dictionary<string, string> parameters)
        {
            var offset = ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Offset, "0");
            var limit = ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Limit, "1");

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);
            return doctors.Skip(offsetNumber).Take(limitNumber).ToList();
        }

        private List<CompleteAlmacenOrderModel> GetOrdersValidsToReceiveByProducts(List<UserOrderModel> userOrders, List<LineProductsModel> lineProductsModel, List<CompleteAlmacenOrderModel> sapOrders)
        {
            var ordersToReturn = new List<CompleteAlmacenOrderModel>();
            var groups = sapOrders.GroupBy(x => x.DocNum).ToList();

            foreach (var p in groups)
            {
                if (p.All(g => g.IsMagistral == "Y"))
                {
                    var saleOrder = userOrders.FirstOrDefault(x => x.Salesorderid == p.Key.ToString() && string.IsNullOrEmpty(x.Productionorderid));
                    var familyByOrder = userOrders.Where(x => x.Salesorderid == p.Key.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.Status != ServiceConstants.Cancelado).ToList();
                    var isValid = saleOrder != null && saleOrder.Status == ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(saleOrder.StatusAlmacen) && familyByOrder.All(x => x.Status == ServiceConstants.Finalizado && x.FinishedLabel == 1);
                    ordersToReturn.AddRange(isValid ? p.ToList() : new List<CompleteAlmacenOrderModel>());
                    continue;
                }

                if (p.All(g => g.IsLine == "Y"))
                {
                    var saleOrderLn = lineProductsModel.FirstOrDefault(x => x.SaleOrderId == p.Key && string.IsNullOrEmpty(x.ItemCode));
                    var userFabLineOrder = lineProductsModel.Where(x => x.SaleOrderId == p.Key && !string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Cancelado).ToList();
                    var isValid = (saleOrderLn == null || saleOrderLn.StatusAlmacen != ServiceConstants.Almacenado) && userFabLineOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado);
                    ordersToReturn.AddRange(isValid ? p.ToList() : new List<CompleteAlmacenOrderModel>());
                    continue;
                }

                var saleOrderMix = userOrders.FirstOrDefault(x => x.Salesorderid == p.Key.ToString() && string.IsNullOrEmpty(x.Productionorderid));
                var familyMixByOrder = userOrders.Where(x => x.Salesorderid == p.Key.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.Status != ServiceConstants.Cancelado).ToList();
                var isValidMg = saleOrderMix != null && saleOrderMix.Status == ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(saleOrderMix.StatusAlmacen) && familyMixByOrder.All(x => x.Status == ServiceConstants.Finalizado && x.FinishedLabel == 1);

                var userFabMixLineOrder = lineProductsModel.Where(x => x.SaleOrderId == p.Key && !string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Cancelado).ToList();
                var isValidLn = userFabMixLineOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado);
                ordersToReturn.AddRange(isValidMg && isValidLn ? p.ToList() : new List<CompleteAlmacenOrderModel>());
            }

            return ordersToReturn;
        }
    }
}
