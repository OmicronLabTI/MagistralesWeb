// <summary>
// <copyright file="SapAlmacenService.cs" company="Axity">
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
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Resources.Exceptions;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Doctors;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;
    using StackExchange.Redis;

    /// <summary>
    /// Class for sap almacen service.
    /// </summary>
    public class SapAlmacenService : ISapAlmacenService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        private readonly IProccessPayments proccessPayments;

        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        /// <param name="proccessPayments">the proccesPayments.</param>
        /// <param name="doctorService">the doctor servce.</param>
        public SapAlmacenService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments, IDoctorService doctorService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.proccessPayments = proccessPayments.ThrowIfNull(nameof(proccessPayments));
            this.doctorService = doctorService.ThrowIfNull(nameof(doctorService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var (sapClasification, types) = await ServiceUtils.GetTypesForFilters(parameters, this.sapDao);
            var listStatus = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Status, ServiceConstants.AllStatus);
            var status = listStatus.Split(",").ToList();

            var startDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                            .ToUniversalDateTime().Date;

            var endDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                                      .ToUniversalDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var userOrders = await this.GetUserOrdersToLook(parameters, startDate, endDate);

            var lineProducts = await this.GetLineProductsToLook(
                userOrders.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList(),
                startDate,
                endDate,
                parameters);

            var sapOrders = await this.GetSapLinesToLook(types, userOrders, startDate, endDate, lineProducts, parameters);

            if (parameters.ContainsKey(ServiceConstants.Chips) &&
                int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId) &&
                !this.ValidateOrdersById(userOrders, sapOrders, lineProducts.Item1))
            {
                var emptyData = new AlmacenOrdersModel { SalesOrders = new List<SalesModel>() };
                return ServiceUtils.CreateResult(true, 200, null, emptyData, null, "0-0");
            }

            var orders = this.GetSapLinesToLookByStatus(sapOrders, userOrders, lineProducts.Item1, status);
            orders = await this.GetSapLinesToLookByChips(orders, parameters);
            var totalFilter = orders.Select(x => x.DocNum).Distinct().ToList().Count;
            var listToReturn = this.GetOrdersToReturn(userOrders, orders, lineProducts.Item1, parameters, sapClasification);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersDetails(int orderId)
        {
            var pedidos = await this.GetUserOrderByids(new List<int> { orderId }, ServiceConstants.GetUserSalesOrder);
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLinesBySaleOrder, new List<int> { orderId });
            var lineOrders = JsonConvert.DeserializeObject<List<LineProductsModel>>(almacenResponse.Response.ToString());

            var comments = ServiceShared.CalculateTernary(almacenResponse.Comments == null, string.Empty, almacenResponse.Comments?.ToString());
            var incidences = ServiceShared.DeserializeObject(comments, new List<IncidentsModel>());

            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);

            var sapOrders = await this.sapDao.GetSapOrderDetailForAlmacenRecepcionById(new List<int> { orderId });
            var adressesToFind = sapOrders.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.DeliveryAddressId }).UtilsDistinctBy(y => y.CardCode).ToList();
            var doctorPrescriptionData = await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, adressesToFind);

            var batches = (await this.sapDao.GetBatchesByProdcuts(lineOrders.Select(x => x.ItemCode).ToList())).ToList();
            var transactionsIds = sapOrders.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
            var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
            var listToReturn = await this.GetDetailRecpcionToReturn(orderId, pedidos, lineOrders, incidences, localNeigbors, sapOrders, batches, payments, doctorPrescriptionData);

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetMagistralScannedData(string code)
        {
            var codeArray = code.Split("-");
            int.TryParse(codeArray[0], out var pedidoId);
            int.TryParse(codeArray[1], out var orderId);

            var details = (await this.sapDao.GetAllDetails(new List<int?> { pedidoId })).ToList();
            var order = details.FirstOrDefault(x => x.OrdenFabricacionId == orderId);
            order ??= new CompleteDetailOrderModel();

            var itemCode = (await this.sapDao.GetProductById(order.CodigoProducto)).FirstOrDefault();
            var productType = ServiceShared.CalculateTernary(itemCode.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

            var magistralData = new MagistralScannerModel
            {
                Container = order.Container,
                Description = itemCode.LargeDescription,
                ItemCode = itemCode.ProductoId,
                NeedsCooling = itemCode.NeedsCooling,
                Pieces = order.QtyPlanned.Value,
                ProductType = $"Producto {productType}",
            };

            return ServiceUtils.CreateResult(true, 200, null, magistralData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetLineScannedData(string code, int orderId)
        {
            var product = (await this.sapDao.GetProductByCodeBar(code)).FirstOrDefault();
            if (product == null)
            {
                return ServiceUtils.CreateResult(false, 404, null, new LineScannerModel(), null, null);
            }

            var warehouses = await this.GetActivesWareHouseConfig(product.ProductoId, product.Groupname, product.ProductFirmName);
            if (warehouses.Count == 0)
            {
                return ServiceUtils.CreateResult(false, 404, ServiceConstants.NoActiveWarehouseError, null, ServiceConstants.NoActiveWarehouseError, null);
            }

            var listComponents = warehouses.Select(x => new CompleteDetalleFormulaModel { ProductId = product.ProductoId, Warehouse = x }).ToList();
            var validBatches = (await this.sapDao.GetValidBatches(listComponents)).ToList();
            var productType = ServiceShared.CalculateTernary(product.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

            var listBatchesModel = validBatches
                .Where(vb => this.GetDateFromString(vb.FechaExp) >= DateTime.Today)
                .Select(b =>
                    new LineProductBatchesModel
                    {
                        Batch = b.DistNumber,
                        ExpDate = b.FechaExp,
                        AvailableQuantity = Math.Round(b.Quantity - b.CommitQty, 6),
                        WarehouseCode = b.WarehouseCode,
                    })
                .ToList();

            if (listBatchesModel.Count == 0)
            {
                return ServiceUtils.CreateResult(false, 404, ServiceConstants.NoAvaiableBoxesError, null, ServiceConstants.NoAvaiableBoxesError, null);
            }

            var remissionPieces = await this.CalculateRemissionPieces(orderId, product.ProductoId);

            var lineData = new LineScannerModel
            {
                Batches = listBatchesModel,
                Description = product.LargeDescription,
                ItemCode = product.ProductoId,
                ProductType = $"Producto {productType}",
                NeedsCooling = product.NeedsCooling,
                Pieces = remissionPieces,
            };

            return ServiceUtils.CreateResult(true, 200, null, lineData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrderDetail(int orderId)
        {
            var orders = (await this.sapDao.GetDetailByDocNum(new List<int> { orderId })).ToList();
            return ServiceUtils.CreateResult(true, 200, null, orders, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetProductsWithCodeBars()
        {
            var lineproducts = (await this.sapDao.GetAllLineProducts()).ToList();

            var codebars = lineproducts.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.BarCode), x.ProductoId != x.BarCode)).Select(x => new LineProductWithCodeBarsModel { CodeBar = x.BarCode, ItemCode = x.ProductoId }).ToList();
            return ServiceUtils.CreateResult(true, 200, null, codebars, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetCompleteDetail(int orderId)
        {
            var data = (await this.sapDao.GetAllOrdersForAlmacenById(orderId)).ToList();
            var countDxpOrders = (await this.sapDao.GetCountDxpOrdersByIds(data.Where(d => !string.IsNullOrEmpty(d.DocNumDxp)).Select(x => x.DocNumDxp).Distinct().ToList())).ToList();
            var dxpOrdersInfo = this.GetDxpOrdersInfoForDelivery(countDxpOrders);
            return ServiceUtils.CreateResult(true, 200, null, data, null, dxpOrdersInfo);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersByIds(List<int> ordersId)
        {
            var data = (await this.sapDao.GetOrdersByIdJoinDoctor(ordersId)).ToList();
            var countDxpOrders = (await this.sapDao.GetCountDxpOrdersByIds(data.Where(d => !string.IsNullOrEmpty(d.DocNumDxp)).Select(x => x.DocNumDxp).Distinct().ToList())).ToList();
            var dxpOrdersInfo = this.GetDxpOrdersInfoForDelivery(countDxpOrders);
            return ServiceUtils.CreateResult(true, 200, null, data, null, dxpOrdersInfo);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryBySaleOrderId(List<int> ordersId)
        {
            var data = (await this.sapDao.GetCompleteDeliveryWithDetailBySaleOrder(ordersId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, data, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> AlmacenGraphCount(Dictionary<string, string> parameters)
        {
            var dates = ServiceUtils.GetDateFilter(parameters);

            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);
            var details = (await this.sapDao.GetDetailsbyDocDate(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).GroupBy(x => x.PedidoId).ToList();
            var idsToReturnLine = new List<int>();
            var idsToReturnMix = new List<int>();
            details.ForEach(x =>
            {
                if (x.All(y => lineProducts.Contains(y.ProductoId)))
                {
                    idsToReturnLine.Add(x.Key.Value);
                }

                if (ServiceShared.CalculateAnd(x.Any(y => lineProducts.Contains(y.ProductoId)), !x.All(y => lineProducts.Contains(y.ProductoId))))
                {
                    idsToReturnMix.Add(x.Key.Value);
                }
            });

            return ServiceUtils.CreateResult(true, 200, null, idsToReturnLine, JsonConvert.SerializeObject(idsToReturnMix), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryParties()
        {
            var parties = await this.sapDao.GetDeliveryCompanies();
            return ServiceUtils.CreateResult(true, 200, null, parties, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveries(List<int> deliveryIds)
        {
            var deliveries = await this.sapDao.GetDeliveryDetailByDocEntry(deliveryIds);
            var allDeliveries = await this.sapDao.GetDeliveryDetailBySaleOrderJoinProduct(deliveries.Where(y => y.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).ToList());
            var detailsSale = (await this.sapDao.GetDetailByDocNum(deliveries.Where(y => y.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).ToList())).ToList();
            var products = (await this.sapDao.GetProductByIds(detailsSale.Select(x => x.ProductoId).ToList())).ToList();

            detailsSale.ForEach(x =>
            {
                var product = products.FirstOrDefault(y => y.ProductoId == x.ProductoId);
                var type = ServiceShared.CalculateTernary(this.CalculateTypeProduct(product, "Y", "N"), "mg", "extra");
                type = ServiceShared.CalculateTernary(this.CalculateTypeProduct(product, "N", "Y"), "ln", type);
                x.Label = type;
                x.IsPackage = product.IsPackage;
            });

            var objectToReturn = new { DeliveryDetail = allDeliveries, DetallePedido = detailsSale.Where(d => d.IsPackage != ServiceConstants.IsPackage) };
            return ServiceUtils.CreateResult(true, 200, null, objectToReturn, null, null);
        }

        private async Task<RemittedPiecesModel> CalculateRemissionPieces(int orderId, string itemCode)
        {
            var response = await this.almacenService.GetAlmacenOrders(string.Format(ServiceConstants.GetRemittedPieces, itemCode, orderId));
            var remissionPieces = JsonConvert.DeserializeObject<RemittedPiecesModel>(response.Response.ToString());

            var orderPieces = await this.sapDao.GetDetailByDocNumAndItemCode(orderId, itemCode);

            var totalPieces = orderPieces.Sum(x => x.Quantity);

            remissionPieces.TotalPieces = (int)totalPieces;
            remissionPieces.AvailablePieces = (int)totalPieces - remissionPieces.RemissionPieces;

            return remissionPieces;
        }

        private async Task<List<string>> GetActivesWareHouseConfig(string itemCode, string catalog, string firmName)
        {
            var request = new List<ActiveWarehouseDto> { new ActiveWarehouseDto { ItemCode = itemCode, CatalogName = catalog, FirmName = firmName }, };
            var response = await this.catalogsService.PostCatalogs(request, ServiceConstants.GetWareHouseConfigUrl);
            var warehouseProduct = JsonConvert.DeserializeObject<List<WarehouseDto>>(response.Response.ToString());
            return warehouseProduct.Where(x => x.ItemCode == itemCode).First().WarehouseCodes;
        }

        private bool CalculateTypeProduct(ProductoModel product, string isMagistral, string isLine)
        {
            return ServiceShared.CalculateAnd(product.IsWorkableProduct == "Y", product.IsMagistral == isMagistral, product.IsLine == isLine);
        }

        private async Task<List<UserOrderModel>> GetUserOrderByids(List<int> ordersId, string route)
        {
            var pedidosResponse = await this.pedidosService.PostPedidos(ordersId, route);
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        private async Task<List<LineProductsModel>> GetLineOrdersByIds(List<int> ordersId)
        {
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLinesBySaleOrder, ordersId);
            return JsonConvert.DeserializeObject<List<LineProductsModel>>(almacenResponse.Response.ToString());
        }

        private bool ValidateOrdersById(List<UserOrderModel> userOrderModels, List<CompleteAlmacenOrderModel> sapOrders, List<LineProductsModel> lineProducts)
        {
            if (lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Any(x => x.StatusAlmacen == ServiceConstants.Cancelado))
            {
                return false;
            }

            if (!sapOrders.Any(x => x.IsMagistral == "Y"))
            {
                return true;
            }

            var magistralProducts = sapOrders.Count(x => x.IsMagistral == "Y");
            if (!userOrderModels.Any() || magistralProducts != userOrderModels.Count(x => !string.IsNullOrEmpty(x.Productionorderid)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the orders that are finalized and all the productin orders.
        /// </summary>
        /// <returns>thhe data.</returns>
        private async Task<List<UserOrderModel>> GetUserOrdersToLook(Dictionary<string, string> parameters, DateTime startDate, DateTime endDate)
        {
            if (parameters.ContainsKey(ServiceConstants.Chips) && int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return await this.GetUserOrderByids([pedidoId], ServiceConstants.GetUserOrdersAlmancenId);
            }

            var userOrderModel = await this.pedidosService.GetUserPedidos(
                string.Format(ServiceConstants.EndpointGetUserOrdersAlmancenByRangeDate, startDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy), endDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy)));
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
        }

        private async Task<ReceipcionPedidosDetailModel> GetDetailRecpcionToReturn(
            int orderId,
            List<UserOrderModel> pedidos,
            List<LineProductsModel> lineOrders,
            List<IncidentsModel> incidences,
            List<string> localNeigbors,
            List<CompleteRecepcionPedidoDetailModel> sapOrders,
            List<Batches> batches,
            List<PaymentsDto> payments,
            List<DoctorDeliveryAddressModel> doctorData)
        {
            var userOrder = pedidos.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid));
            var order = sapOrders.FirstOrDefault();
            var payment = payments.FirstOrDefault(p => ServiceShared.ValidateShopTransaction(p.TransactionId, order.DocNumDxp));
            payment ??= new PaymentsDto { ShippingCostAccepted = 1 };
            var invoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeigbors, order.Address, payment);
            var productList = this.GetProductListModel(pedidos, sapOrders, lineOrders, incidences, batches);
            var salesStatusMagistral = this.GetStatusSaleOrder(userOrder);
            salesStatusMagistral = ServiceShared.CalculateTernary(salesStatusMagistral == ServiceConstants.PorRecibir && productList.Any(y => y.Status == ServiceConstants.Pendiente), ServiceConstants.Pendiente, salesStatusMagistral);
            var salesStatusLinea = ServiceShared.CalculateTernary(lineOrders.Any(x => x.DeliveryId != 0), ServiceConstants.BackOrder, ServiceConstants.PorRecibir);
            var salesStatus = ServiceShared.CalculateTernary(userOrder != null, salesStatusMagistral, salesStatusLinea);
            var userProdOrders = pedidos.Count(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Productionorderid), x.Status.Equals(ServiceConstants.Almacenado)));
            var lineProductsCount = productList.Where(x => x.Pieces <= lineOrders.Where(y => y.ItemCode == x.ItemCode && y.StatusAlmacen == ServiceConstants.Almacenado).SelectMany(lp => ServiceShared.DeserializeObject(lp.BatchName, new List<AlmacenBatchModel>())).Sum(x => x.BatchQty)).Count();
            var totalAlmacenados = userProdOrders + lineProductsCount;
            var doctor = doctorData.FirstOrDefault(x => x.AddressId == order.DeliveryAddressId);
            doctor ??= new DoctorDeliveryAddressModel { Contact = order.Cliente };

            var sapClasification = await this.sapDao.GetClassificationsByValue([order.TypeOrder]);
            var productType = ServiceUtils.GetOrderTypeDescription([order.TypeOrder], sapClasification);
            var saleHeader = new AlmacenSalesHeaderModel
            {
                Client = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctor.Contact), order.Medico, doctor.Contact),
                DocNum = orderId,
                Comments = userOrder?.Comments ?? string.Empty,
                Doctor = order.Medico,
                InitDate = order?.FechaInicio ?? DateTime.Now,
                Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, salesStatus),
                TotalItems = sapOrders.DistinctBy(x => x.Producto.ProductoId).Count(),
                TotalPieces = sapOrders.DistinctBy(x => x.Producto.ProductoId).Sum(y => y.Detalles.Quantity),
                TypeSaleOrder = $"Pedido {productType}",
                OrderCounter = $"{totalAlmacenados}/{productList.Count}",
                InvoiceType = invoiceType,
                TypeOrder = order.TypeOrder,
                OrderMuestra = ServiceShared.CalculateTernary(string.IsNullOrEmpty(order.PedidoMuestra), ServiceConstants.IsNotSampleOrder, order.PedidoMuestra),
                SapComments = order.Comments,
                IsPackage = order.IsPackage == ServiceConstants.IsPackage,
                IsOmigenomics = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(order.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(order.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(order.IsSecondary)),
                RemittedPieces = productList.Sum(p => p.RemittedPieces),
            };

            var listToReturn = new ReceipcionPedidosDetailModel
            {
                AlmacenHeader = saleHeader,
                Items = productList,
            };
            return listToReturn;
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProductsToLook(List<int> magistralIds, DateTime startDate, DateTime endDate, Dictionary<string, string> parameters)
        {
            List<LineProductsModel> lineProducts;
            if (parameters.ContainsKey(ServiceConstants.Chips) && int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                lineProducts = await this.GetLineOrdersByIds(new List<int> { pedidoId });
                return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, new List<int>());
            }

            var lineProductsResponse = await this.almacenService.PostAlmacenOrders(
                ServiceConstants.EndPointGetLineProductPedidosByRangeDate,
                new OrdersFilterDto
                {
                    MagistralIds = magistralIds,
                    StartDate = startDate,
                    EndDate = endDate,
                });

            lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());
            var listIdToIgnore = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && ServiceConstants.StatusToIgnoreLineProducts.Contains(x.StatusAlmacen)).Select(y => y.SaleOrderId).ToList();
            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetSapLinesToLook(
            List<string> types,
            List<UserOrderModel> userOrders,
            DateTime startDate,
            DateTime endDate,
            Tuple<List<LineProductsModel>, List<int>> lineProductTuple,
            Dictionary<string, string> parameters)
        {
            var parametersWhs = (await ServiceUtils.GetParams([ServiceConstants.WareHouseToExclude], this.catalogsService)).Select(x => x.Value).ToList();
            if (parameters.ContainsKey(ServiceConstants.Chips) && int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                var sapOrdersById = (await this.sapDao.GetAllOrdersForAlmacenByListIds(new List<int> { pedidoId })).ToList();
                sapOrdersById = ServiceUtils.GetOrdersWithValidWareHouse(sapOrdersById, parametersWhs);
                return ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrdersById);
            }

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(
                this.sapDao,
                userOrders,
                startDate,
                endDate,
                lineProductTuple,
                false);

            var sapCancelled = sapOrders.Where(x => x.Canceled == "Y");
            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();
            sapOrders = ServiceUtils.GetOrdersWithValidWareHouse(sapOrders, parametersWhs);
            var possibleIdsToIgnore = sapOrders.Where(x => !userOrders.Any(y => y.Salesorderid == x.DocNum.ToString()));
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key);
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();
            sapOrders.AddRange(sapCancelled);
            return ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders);
        }

        /// <summary>
        /// Gets the sap orders by status.
        /// </summary>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="userModels">the models.</param>
        /// <param name="lineProducts">the user orders.</param>
        /// <param name="parameters">the params.</param>
        /// <returns>the produtcs.</returns>
        private List<CompleteAlmacenOrderModel> GetSapLinesToLookByStatus(List<CompleteAlmacenOrderModel> sapOrders, List<UserOrderModel> userModels, List<LineProductsModel> lineProducts, List<string> parameters)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();

            if (parameters.Contains(ServiceConstants.Cancelado))
            {
                listToReturn.AddRange(sapOrders.Where(x => x.Canceled == "Y"));
            }

            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();
            if (parameters.Contains(ServiceConstants.Recibir))
            {
                var allIds = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
                allIds.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Select(y => y.SaleOrderId));

                var idsToLook = userModels.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.Productionorderid), x.Status == ServiceConstants.Finalizado, !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen))).Select(y => int.Parse(y.Salesorderid)).ToList();
                idsToLook.AddRange(lineProducts.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen))).Select(y => y.SaleOrderId));
                idsToLook.AddRange(sapOrders.Where(x => !allIds.Contains(x.DocNum)).Select(y => y.DocNum));
                listToReturn.AddRange(sapOrders.Where(x => idsToLook.Contains(x.DocNum)));
            }

            if (parameters.Contains(ServiceConstants.Pendiente))
            {
                var idsPendiente = userModels.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.Productionorderid), x.Status != ServiceConstants.Finalizado, !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen))).Select(y => int.Parse(y.Salesorderid)).ToList();
                listToReturn.AddRange(sapOrders.Where(x => idsPendiente.Contains(x.DocNum)));
            }

            if (parameters.Contains(ServiceConstants.BackOrder))
            {
                var idsBackOrder = userModels.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.Productionorderid), x.StatusAlmacen == ServiceConstants.BackOrder)).Select(y => int.Parse(y.Salesorderid)).ToList();
                idsBackOrder.AddRange(lineProducts.Where(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), x.StatusAlmacen == ServiceConstants.BackOrder)).Select(y => y.SaleOrderId));
                listToReturn.AddRange(sapOrders.Where(x => idsBackOrder.Contains(x.DocNum)));
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the order by the chips criteria.
        /// </summary>
        /// <param name="sapOrders">the orders.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<CompleteAlmacenOrderModel>> GetSapLinesToLookByChips(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            sapOrders = await ServiceUtilsAlmacen.FilterSapOrdersByTypeShipping(sapOrders, parameters, this.proccessPayments, this.redisService, this.catalogsService);

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return sapOrders.Where(x => x.DocNum == pedidoId).ToList();
            }

            if (parameters[ServiceConstants.Chips].StartsWith(ServiceConstants.WildcardDocNumDxp))
            {
                return sapOrders.Where(x => x.DocNumDxp == parameters[ServiceConstants.Chips].ToLower().Remove(0, 1)).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => listNames.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets the data structure.
        /// </summary>
        /// <param name="userOrders">The user orders.</param>
        /// <param name="sapOrders">the Sap orders.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private AlmacenOrdersModel GetOrdersToReturn(
            List<UserOrderModel> userOrders,
            List<CompleteAlmacenOrderModel> sapOrders,
            List<LineProductsModel> lineProducts,
            Dictionary<string, string> parameters,
            IEnumerable<ClassificationsModel> sapClassifications)
        {
            var salesIds = this.GetOrdersToProcess(sapOrders.Select(x => x.DocNum).Distinct().ToList(), parameters);
            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = salesIds.Count,
            };

            foreach (var so in salesIds)
            {
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var userOrder = userOrders.GetSaleOrderHeader(so.ToString());
                var lineOrders = lineProducts.Where(x => x.SaleOrderId == so).ToList();

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctor = order.Medico.ValidateNull();

                var localUserOrders = userOrders.Where(x => ServiceShared.CalculateAnd(x.Salesorderid == so.ToString(), !string.IsNullOrEmpty(x.Productionorderid))).ToList();

                var salesStatusMagistral = this.GetStatusSaleOrder(userOrder);
                salesStatusMagistral = ServiceShared.CalculateTernary(ServiceShared.CalculateAnd(salesStatusMagistral == ServiceConstants.PorRecibir, localUserOrders.Any(y => ServiceShared.CalculateAnd(y.Status == ServiceConstants.Finalizado, y.FinishedLabel == 0))), ServiceConstants.Pendiente, salesStatusMagistral);

                var salesStatusLinea = ServiceShared.CalculateTernary(lineOrders.Any(x => x.DeliveryId != 0), ServiceConstants.BackOrder, ServiceConstants.PorRecibir);
                var salesStatus = ServiceShared.CalculateTernary(userOrder != null, salesStatusMagistral, salesStatusLinea);

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = so,
                    Doctor = doctor,
                    InitDate = order.FechaInicio,
                    Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, salesStatus),
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
                    TypeOrder = order.TypeOrder,
                    OrderMuestra = ServiceShared.CalculateTernary(string.IsNullOrEmpty(order.PedidoMuestra), ServiceConstants.IsNotSampleOrder, order.PedidoMuestra),
                    SaleOrderType = ServiceUtils.GetOrderTypeDescription([order.TypeOrder], sapClassifications),
                    IsPackage = orders.Any(x => x.IsPackage == ServiceConstants.IsPackage),
                    IsOmigenomics = orders.Exists(x => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsSecondary))),
                };

                var saleModel = new SalesModel
                {
                    AlmacenHeader = null,
                    AlmacenSales = salesOrderModel,
                    Items = null,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the orders to process.
        /// </summary>
        /// <param name="docEntries">the docEntries.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<int> GetOrdersToProcess(List<int> docEntries, Dictionary<string, string> parameters)
        {
            var offset = int.TryParse(ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Offset, "0"), out int offsetNumber) ? offsetNumber : 0;
            var limit = int.TryParse(ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Limit, "1"), out int limitNumber) ? limitNumber : 1;

            return docEntries.OrderBy(x => x)
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="lineProductsModel">The lines products.</param>
        /// <param name="incidents">The incidents.</param>
        /// <param name="batchesDataBase">The batches.</param>
        /// <returns>the products.</returns>
        private List<ProductListRecepcionModel> GetProductListModel(List<UserOrderModel> userOrders, List<CompleteRecepcionPedidoDetailModel> sapOrders, List<LineProductsModel> lineProductsModel, List<IncidentsModel> incidents, List<Batches> batchesDataBase)
        {
            var listToReturn = sapOrders.Select(order =>
            {
                var itemcode = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(order.FabricationOrder), $"{order.Producto.ProductoId} - {order.FabricationOrder}", order.Producto.ProductoId);
                var productType = ServiceShared.CalculateTernary(order.Producto.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

                var orderStatus = ServiceConstants.PorRecibir;
                var hasDelivery = false;
                var deliveryIds = new List<int>();
                var batches = new List<string>();
                var remittedPieces = 0;
                var pendingToStore = false;

                if (order.Producto.IsMagistral.Equals("Y"))
                {
                    var userFabOrder = userOrders.FirstOrDefault(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid.Equals(order.FabricationOrder));
                    userFabOrder ??= new UserOrderModel { Status = ServiceConstants.Finalizado };
                    orderStatus = ServiceShared.CalculateTernary(userFabOrder.Status == ServiceConstants.Finalizado, ServiceConstants.PorRecibir, userFabOrder.Status);
                    orderStatus = ServiceShared.CalculateTernary(ServiceShared.CalculateAnd(orderStatus == ServiceConstants.PorRecibir, userFabOrder.FinishedLabel == 0), ServiceConstants.Pendiente, orderStatus);
                    orderStatus = ServiceShared.CalculateTernary(userFabOrder.Status == ServiceConstants.Cancelado, ServiceConstants.Cancelado, orderStatus);
                    hasDelivery = userFabOrder.DeliveryId != 0;
                    deliveryIds = new List<int> { userFabOrder.DeliveryId };
                }
                else
                {
                    remittedPieces = lineProductsModel.Where(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode == order.Producto.ProductoId && x.DeliveryId != 0)
                            .SelectMany(x => ServiceShared.DeserializeObject(x.BatchName, new List<AlmacenBatchModel>())).Sum(b => (int)b.BatchQty);

                    orderStatus = this.CalculateStatus(order.Canceled, (int)order.Detalles.Quantity, remittedPieces);

                    var matchedLines = lineProductsModel.Where(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode == order.Producto.ProductoId).ToList();
                    deliveryIds = matchedLines.Where(x => x.DeliveryId != 0).Select(x => x.DeliveryId).ToList();
                    hasDelivery = deliveryIds.Any();

                    var allBatchModels = matchedLines.SelectMany(x => ServiceShared.DeserializeObject(x.BatchName, new List<AlmacenBatchModel>())).ToList();
                    batches = allBatchModels
                        .Select(y => $"{y.BatchNumber} | {(int)y.BatchQty} pz | Cad: {this.GetExpirationDate(batchesDataBase, y.BatchNumber, order.Producto.ProductoId)}")
                        .ToList();
                }

                pendingToStore = lineProductsModel.Any(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode == order.Producto.ProductoId && x.DeliveryId == 0);

                var incidentdb = incidents.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == order.DocNum, x.ItemCode == order.Producto.ProductoId));
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = ServiceShared.DeserializeObject(incidentdb.Batches, new List<AlmacenBatchModel>()),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                return new ProductListRecepcionModel
                {
                    Container = order.Detalles.Container,
                    Description = order.Producto.LargeDescription.ToUpper(),
                    ItemCode = itemcode,
                    NeedsCooling = order.Producto.NeedsCooling,
                    ProductType = $"Producto {productType}",
                    Pieces = order.Detalles.Quantity,
                    Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, orderStatus),
                    IsMagistral = order.Producto.IsMagistral.Equals("Y"),
                    Batches = batches,
                    Incident = ServiceShared.CalculateTernary(string.IsNullOrEmpty(localIncident.Status), null, localIncident),
                    HasDelivery = hasDelivery,
                    DeliveryId = deliveryIds,
                    RemittedPieces = remittedPieces,
                    HasPendingToStore = pendingToStore,
                };
            }).ToList();

            return listToReturn;
        }

        private string CalculateStatus(string isCanceled, int totalPieces, int remittedPieces) =>
            isCanceled == "Y" ? ServiceConstants.Cancelado :
            remittedPieces == 0 ? ServiceConstants.PorRecibir :
            remittedPieces < totalPieces ? ServiceConstants.BackOrder :
            ServiceConstants.Almacenado;

        private string GetStatusSaleOrder(UserOrderModel userOrder)
        {
            var salesStatusMagistral = ServiceShared.CalculateTernary(userOrder != null && userOrder.Status.Equals(ServiceConstants.Finalizado), ServiceConstants.PorRecibir, ServiceConstants.Pendiente);
            salesStatusMagistral = ServiceShared.CalculateTernary(userOrder != null && !string.IsNullOrEmpty(userOrder.StatusAlmacen) && userOrder.StatusAlmacen != ServiceConstants.Recibir, userOrder?.StatusAlmacen, salesStatusMagistral);
            salesStatusMagistral = ServiceShared.CalculateTernary(salesStatusMagistral == ServiceConstants.Recibir, ServiceConstants.PorRecibir, salesStatusMagistral);
            return salesStatusMagistral;
        }

        private DateTime GetDateFromString(string date)
        {
            var dateText = date ?? DateTime.Now.ToString("dd/MM/yyyy");
            var dateSplit = dateText.Split("/");
            return new DateTime(int.Parse(dateSplit[2]), int.Parse(dateSplit[1]), int.Parse(dateSplit[0]));
        }

        private string GetExpirationDate(List<Batches> batchesDataBase, string batchNumber, string productoId)
        {
            var batch = batchesDataBase.FirstOrDefault(z => ServiceShared.CalculateAnd(z.DistNumber == batchNumber, z.ItemCode == productoId));
            batch ??= new Batches();
            return ServiceShared.GetDateValueOrDefault(batch.ExpDate, string.Empty);
        }

        private IEnumerable<CountDxpOrders> GetDxpOrdersInfoForDelivery(List<CompleteAlmacenOrderModel> countDxpOrders)
        {
            return countDxpOrders
                            .GroupBy(o => o.DocNumDxp)
                            .Select(ord =>
                            new CountDxpOrders
                            {
                                DocNumDxp = ord.Key,
                                NumOrders = ord.Where(o => o.IsWorkableProduct == "Y").Select(o => o.DocNum).Distinct().ToList(),
                                ProductsDetails = ord.Select(o => new CountDxpOrdersDetail { ItemCode = o.Detalles.ProductoId, DocNum = o.DocNum, CatalogGroup = o.Detalles.CatalogGroup, ProductFirmName = o.Detalles.ProductFirmName, }).ToList(),
                            });
        }
    }
}
