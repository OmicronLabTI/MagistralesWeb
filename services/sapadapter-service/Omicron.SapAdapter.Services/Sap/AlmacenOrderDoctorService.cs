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
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentNullException(nameof(almacenService));
            this.catalogsService = catalogsService ?? throw new ArgumentNullException(nameof(catalogsService));
            this.redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters)
        {
            var typesString = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.AllTypesByDoctor;
            var types = typesString.Split(",").ToList();

            var userOrdersTuple = await this.GetUserOrders();
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await this.GetLineProducts(ids, userOrdersTuple.Item3);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple, types);
            var ordersByFilter = this.GetSapOrdersToLookByDoctor(sapOrders, parameters);
            var totalFilter = ordersByFilter.Select(x => x.Medico).Distinct().ToList().Count;

            var listToReturn = await this.GetCardOrdersToReturn(ordersByFilter, userOrdersTuple.Item1);
            listToReturn = this.GetDoctorsToProcess(listToReturn, parameters);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
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
                    Status = ServiceConstants.PorRecibir,
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

            var orderHeaders = (await this.sapDao.GetFabOrderBySalesOrderId(sapOrders.Select(x => x.DocNum).ToList())).ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !orderHeaders.Any(y => y.PedidoId.Value == x.DocNum)).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();

            sapOrders = this.FilterByStatusToReceive(sapOrders, userOrdersTuple, lineProductTuple);
            sapOrders = await this.GetOrdersValidsToReceiveByProducts(userOrdersTuple.Item1, lineProductTuple.Item1, sapOrders);
            sapOrders = sapOrders.Where(x => x.PedidoMuestra != ServiceConstants.OrderTypeMU).ToList();

            return ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, lineProducts).Item1;
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private List<CompleteAlmacenOrderModel> FilterByStatusToReceive(List<CompleteAlmacenOrderModel> sapOrders, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductsTuple)
        {
            var userModels = userOrdersTuple.Item1;
            var lineProducts = lineProductsTuple.Item1;
            var listToReturn = new List<CompleteAlmacenOrderModel>();

            var allIds = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
            allIds.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Select(y => y.SaleOrderId));

            var idsToLook = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid) && x.Status == ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen)).Select(y => int.Parse(y.Salesorderid)).ToList();
            idsToLook.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).Select(y => y.SaleOrderId));
            idsToLook.AddRange(sapOrders.Where(x => !allIds.Contains(x.DocNum)).Select(y => y.DocNum));
            listToReturn.AddRange(sapOrders.Where(x => idsToLook.Contains(x.DocNum)));

            return listToReturn;
        }

        private List<CompleteAlmacenOrderModel> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            var doctorName = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => doctorName.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets card for doctor.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersByDoctorModel> GetCardOrdersToReturn(List<CompleteAlmacenOrderModel> sapOrders, List<UserOrderModel> userOrders)
        {
            var doctors = sapOrders.Select(x => x.Medico).Distinct().ToList();

            var listToReturn = new AlmacenOrdersByDoctorModel
            {
                SalesOrders = new List<SalesByDoctorModel>(),
                TotalSalesOrders = 0,
            };

            var productsModel = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);

            foreach (var doctor in doctors)
            {
                var orders = sapOrders.Where(x => x.Medico == doctor).ToList();
                var totalOrders = orders.DistinctBy(x => x.DocNum).Count();
                var totalItems = orders.DistinctBy(y => new { y.DocNum, y.Detalles.ProductoId }).Count();
                var totalPieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctorAddress = sapOrders.FirstOrDefault(x => x.Medico == doctor);
                doctorAddress ??= new CompleteAlmacenOrderModel();
                var address = string.IsNullOrEmpty(doctorAddress.Address) ? string.Empty : doctorAddress.Address.Replace("\r", " ").Replace("  ", " ").ToUpper();

                var sale = new AlmacenSalesByDoctorModel
                {
                    Doctor = doctor,
                    Address = address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                };

                var saleHeader = new AlmacenSalesByDoctorHeaderModel
                {
                    Doctor = doctor,
                    Address = address,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                };

                var listOrders = this.GetTotalOrdersByDoctor(orders, productsModel, userOrders, localNeigbors);

                var saleModel = new SalesByDoctorModel
                {
                    AlmacenSalesByDoctor = sale,
                    AlmacenHeaderByDoctor = saleHeader,
                    Items = listOrders,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        private List<OrderListByDoctorModel> GetTotalOrdersByDoctor(List<CompleteAlmacenOrderModel> sapOrders, List<string> productsModel, List<UserOrderModel> userOrders, List<string> localNeighbors)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct().OrderByDescending(x => x);

            foreach (var so in salesIds)
            {
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);

                var productType = orders.Any(x => x.Detalles != null && productsModel.Any(p => p == x.Detalles.ProductoId)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                productType = orders.All(x => x.Detalles != null && productsModel.Contains(x.Detalles.ProductoId)) ? ServiceConstants.Linea : productType;

                order.Address = string.IsNullOrEmpty(order.Address) ? string.Empty : order.Address;
                var orderType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, order.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo;

                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));
                var comments = userOrder == null ? string.Empty : userOrder.Comments;

                var saleItem = new OrderListByDoctorModel
                {
                    DocNum = so,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = ServiceConstants.PorRecibir,
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = orderType,
                    Comments = comments,
                    OrderType = order.TypeOrder,
                };
                listOrders.Add(saleItem);
            }

            return listOrders;
        }

        private AlmacenOrdersByDoctorModel GetDoctorsToProcess(AlmacenOrdersByDoctorModel ordersByDoctorModel, Dictionary<string, string> parameters)
        {
            var salesByDoctorModel = ordersByDoctorModel.SalesOrders;
            salesByDoctorModel = salesByDoctorModel.OrderBy(x => x.AlmacenSalesByDoctor.Doctor).ToList();

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);
            ordersByDoctorModel.SalesOrders = salesByDoctorModel.Skip(offsetNumber).Take(limitNumber).ToList();
            ordersByDoctorModel.TotalSalesOrders = ordersByDoctorModel.SalesOrders.Count;
            return ordersByDoctorModel;
        }

        private List<ProductListModel> GetProductListModel(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, List<CompleteDetailOrderModel> detailsList, List<LineProductsModel> lineProductsModel, List<ProductoModel> products)
        {
            var listToReturn = new List<ProductListModel>();
            foreach (var order in sapOrders)
            {
                var item = products.FirstOrDefault(x => order.Detalles.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var fabOrder = detailsList.FirstOrDefault(x => x.CodigoProducto.Equals(order.Detalles.ProductoId));
                var orderId = fabOrder == null ? string.Empty : fabOrder.OrdenFabricacionId.ToString();

                var orderStatus = ServiceConstants.PorRecibir;

                if (item.IsMagistral.Equals("Y"))
                {
                    var userFabOrder = userOrders.FirstOrDefault(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid.Equals(orderId));
                    userFabOrder ??= new UserOrderModel { Status = ServiceConstants.Finalizado };
                    orderStatus = userFabOrder.Status == ServiceConstants.Finalizado ? ServiceConstants.PorRecibir : userFabOrder.Status;
                    orderStatus = orderStatus == ServiceConstants.PorRecibir && userFabOrder.FinishedLabel == 0 ? ServiceConstants.Pendiente : orderStatus;
                    orderStatus = userFabOrder.Status == ServiceConstants.Cancelado ? ServiceConstants.Cancelado : orderStatus;
                }
                else
                {
                    var userFabLineOrder = lineProductsModel.FirstOrDefault(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.Equals(item.ProductoId));
                    userFabLineOrder ??= new LineProductsModel { StatusAlmacen = ServiceConstants.PorRecibir };
                    orderStatus = !userFabLineOrder.StatusAlmacen.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabLineOrder.StatusAlmacen;
                    orderStatus = userFabLineOrder.StatusAlmacen == ServiceConstants.Cancelado ? ServiceConstants.Cancelado : orderStatus;
                }

                var productModel = new ProductListModel
                {
                    ItemCode = item.ProductoId,
                    Status = orderStatus,
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetOrdersValidsToReceiveByProducts(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<CompleteAlmacenOrderModel> sapOrders)
        {
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct();
            var productsIds = sapOrders.Where(x => salesIds.Contains(x.DocNum)).Select(y => y.Detalles.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();
            var validSalesIds = new List<int>();

            foreach (var so in salesIds)
            {
                var saleDetail = (await this.sapDao.GetAllDetails(new List<int?> { so })).ToList();
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();

                var productsList = this.GetProductListModel(userOrders, orders, saleDetail, lineProducts, productItems);
                if (productsList.All(x => x.Status == ServiceConstants.PorRecibir))
                {
                    validSalesIds.Add(so);
                }
            }

            var listToReturn = sapOrders.Where(x => validSalesIds.Contains(x.DocNum)).ToList();

            return listToReturn;
        }
    }
}
