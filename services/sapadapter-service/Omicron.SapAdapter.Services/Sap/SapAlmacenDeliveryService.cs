// <summary>
// <copyright file="SapAlmacenDeliveryService.cs" company="Axity">
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
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Doctors;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for the sap almacen delivery service.
    /// </summary>
    public class SapAlmacenDeliveryService : ISapAlmacenDeliveryService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        private readonly IProccessPayments proccessPayments;

        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenDeliveryService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="doctorService">The doctor service.</param>
        public SapAlmacenDeliveryService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments, IDoctorService doctorService)
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
        public async Task<ResultModel> GetDelivery(Dictionary<string, string> parameters)
        {
            var (sapClasification, types) = await ServiceUtils.GetTypesForFilters(parameters, this.sapDao);
            var startDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                            .ToUniversalDateTime().Date;

            var endDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                                      .ToUniversalDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var userOrders = await this.GetUserOrdersRemision(parameters, startDate, endDate);
            var lineProducts = await this.GetLineProductsRemision(parameters, startDate, endDate);
            var (deliveryToReturn, deliveryHeaders, filterCount, invoices) = await this.GetOrdersByType(types, userOrders, lineProducts, parameters);

            var dataToReturn = this.GetOrdersToReturn(deliveryToReturn, deliveryHeaders, invoices);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{filterCount}-{filterCount}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersDeliveryDetail(int deliveryId)
        {
            var deliveryDetails = await this.sapDao.GetDeliveryDetailForDeliveryById(new List<int> { deliveryId });

            var saleOrders = deliveryDetails.Where(y => y.Detalles.BaseEntry.HasValue).Select(x => x.Detalles.BaseEntry.Value).Distinct().ToList();

            var invoicesId = deliveryDetails.Where(x => x.Detalles.InvoiceId.HasValue).Select(y => y.Detalles.InvoiceId.Value).ToList();
            var invoices = ServiceShared.CalculateTernary(invoicesId.Any(), (await this.sapDao.GetInvoiceHeaderByInvoiceId(invoicesId)).ToList(), new List<InvoiceHeaderModel>());

            var sapSaleOrders = await this.sapDao.GetOrdersById(saleOrders);
            var hasAnyChildFabOrder = await this.sapDao.GetHasAnyChildProductionOrder(saleOrders);
            var details = await this.sapDao.GetDetails(saleOrders.Cast<int?>().ToList());
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var pedidosResponse = await this.pedidosService.PostPedidos(saleOrders, ServiceConstants.GetUserSalesOrder);
            var pedidos = JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
            var transactionsIds = deliveryDetails.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
            var payment = (await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds)).FirstOrDefault(p => ServiceShared.ValidateShopTransaction(p.TransactionId, deliveryDetails.First().DocNumDxp));
            payment ??= new PaymentsDto { ShippingCostAccepted = 1 };

            var addressesToFind = deliveryDetails.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.DeliveryAddressId }).UtilsDistinctBy(x => x.CardCode).ToList();
            var doctorsData = (await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, addressesToFind)).FirstOrDefault(x => x.AddressId == deliveryDetails.FirstOrDefault().DeliveryAddressId);
            doctorsData ??= new DoctorDeliveryAddressModel { Contact = deliveryDetails.FirstOrDefault().Medico };

            var dataToReturn = new SalesModel();
            dataToReturn.SalesOrders = await this.CreateSaleCard(deliveryDetails, pedidos, sapSaleOrders, details.ToList());
            dataToReturn.AlmacenHeader = new AlmacenSalesHeaderModel
            {
                Client = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctorsData.Contact), deliveryDetails.FirstOrDefault().Medico, doctorsData.Contact),
                DocNum = saleOrders.Count,
                Doctor = deliveryDetails.FirstOrDefault().Medico,
                InitDate = deliveryDetails.FirstOrDefault().FechaInicio,
                Status = ServiceConstants.Almacenado,
                TotalItems = dataToReturn.SalesOrders.Sum(x => x.Products),
                TotalPieces = dataToReturn.SalesOrders.Sum(x => x.Pieces),
                Remision = deliveryId,
                InvoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeigbors, deliveryDetails.FirstOrDefault().Address, payment),
                TypeOrder = deliveryDetails.FirstOrDefault().TypeOrder,
                HasInvoice = invoices.Any() && invoices.FirstOrDefault().Canceled == "N",
                IsPackage = deliveryDetails.FirstOrDefault().IsPackage == ServiceConstants.IsPackage,
                IsOmigenomics = deliveryDetails.Exists(del => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(del.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(del.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(del.IsSecondary))),
                DxpId = ServiceShared.ValidateNull(deliveryDetails.FirstOrDefault().DocNumDxp.GetShortShopTransaction()).ToUpper(),
                HasChildOrders = hasAnyChildFabOrder,
            };

            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryIdsByInvoice(int invoiceId)
        {
            var result = await this.sapDao.GetDeliveryIdsByInvoice(invoiceId);
            return ServiceUtils.CreateResult(true, 200, null, result, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetProductsDelivery(string saleId)
        {
            var saleArray = saleId.Split("-").ToList();
            int.TryParse(saleArray[0], out var orderSaleId);
            int.TryParse(saleArray[1], out var deliveryId);

            var dictionaryChip = new Dictionary<string, string> { { ServiceConstants.Chips, deliveryId.ToString() } };
            var userOrders = await this.GetUserOrdersRemision(dictionaryChip, DateTime.Now, DateTime.Now);
            var lineProducts = await this.GetLineProductsRemision(dictionaryChip, DateTime.Now, DateTime.Now);
            userOrders = userOrders.Where(x => x.Salesorderid == orderSaleId.ToString()).ToList();
            lineProducts = lineProducts.Where(x => x.SaleOrderId == orderSaleId).ToList();

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailBySaleOrderJoinProduct(new List<int> { orderSaleId })).ToList();
            deliveryDetails = deliveryDetails.Where(x => x.DeliveryId == deliveryId).ToList();

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { orderSaleId });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var productsIds = deliveryDetails.Select(y => y.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();

            var details = await this.sapDao.GetDetails(new List<int?> { orderSaleId });

            var items = await this.GetProductListModel(deliveryDetails, userOrders, lineProducts, incidents, productItems, details.ToList());
            return ServiceUtils.CreateResult(true, 200, null, items, null, null);
        }

        /// <summary>
        /// Calculate Remitted Pieces.
        /// </summary>
        /// <param name="itemCode">item code.</param>
        /// <param name="lineProducts">line products.</param>
        /// <returns>Remitted Pieces for product.</returns>
        public int CalculateRemittedPieces(string itemCode, List<LineProductsModel> lineProducts)
        {
            if (lineProducts == null || lineProducts.Count == 0)
            {
                return 0;
            }

            return (int)lineProducts
                .Where(lp => itemCode.Contains(lp.ItemCode, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(lp.BatchName) && lp.DeliveryId != 0)
                .SelectMany(lp => ServiceShared.DeserializeObject(lp.BatchName, new List<AlmacenBatchModel>()))
                .Sum(b => b.BatchQty);
        }

        private async Task<List<UserOrderModel>> GetUserOrdersRemision(Dictionary<string, string> parameters, DateTime startDate, DateTime endDate)
        {
            if (parameters.ContainsKey(ServiceConstants.Chips) && int.TryParse(parameters[ServiceConstants.Chips], out int remisionId))
            {
                var userOrdersResponse = await this.pedidosService.PostPedidos(new List<int> { remisionId }, ServiceConstants.GetUserOrderDeliveryId);
                return JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            }

            var pedidosResponse = await this.pedidosService.GetUserPedidos(string.Format(
                ServiceConstants.GetUserOrderDeliveryByDatesRange,
                startDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy),
                endDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy)));

            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the line products.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetLineProductsRemision(Dictionary<string, string> parameters, DateTime startDate, DateTime endDate)
        {
            if (parameters.ContainsKey(ServiceConstants.Chips) && int.TryParse(parameters[ServiceConstants.Chips], out int remisionId))
            {
                var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLinesForDeliveryId, new List<int> { remisionId });
                return JsonConvert.DeserializeObject<List<LineProductsModel>>(almacenResponse.Response.ToString());
            }

            var almacenResponseDate = await this.almacenService.GetAlmacenOrders(
                            string.Format(
                                ServiceConstants.GetLinesForDeliveryByDatesRange,
                                startDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy),
                                endDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy)));

            return JsonConvert.DeserializeObject<List<LineProductsModel>>(almacenResponseDate.Response.ToString());
        }

        /// <summary>
        /// Gets the lines to loop.
        /// </summary>
        /// <param name="types">the types.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="lineModels">the line produtcs.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int, List<InvoiceHeaderModel>>> GetOrdersByType(List<string> types, List<UserOrderModel> userOrders, List<LineProductsModel> lineModels, Dictionary<string, string> parameters)
        {
            var listDeliveryIds = lineModels.Select(x => x.DeliveryId).ToList();
            listDeliveryIds.AddRange(userOrders.Select(x => x.DeliveryId).ToList());
            listDeliveryIds = listDeliveryIds.OrderBy(x => x).Distinct().ToList();

            var deliveryDetailDb = await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(listDeliveryIds);

            var invoices = await this.sapDao.GetInvoiceHeaderByInvoiceId(deliveryDetailDb.Where(x => x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList());
            var invoiceRefactura = invoices.Where(x => x.Refactura == ServiceConstants.IsRefactura).Select(y => y.InvoiceId);
            invoices = invoices.Where(x => x.Refactura != ServiceConstants.IsRefactura);
            deliveryDetailDb = deliveryDetailDb.Where(x => !x.InvoiceId.HasValue || !invoiceRefactura.Contains(x.InvoiceId.Value));

            var deliveryToReturn = deliveryDetailDb.ToList();
            var deliveryHeaders = await this.sapDao.GetDeliveriesByDocNums(listDeliveryIds);

            var maquilaDeliverys = deliveryHeaders.Where(x => x.TypeOrder == ServiceConstants.OrderTypeMQ);
            var packageDeliveries = deliveryHeaders.Where(x => x.IsPackage == ServiceConstants.IsPackage);
            var omigenomicsDeliveries = deliveryHeaders.Where(del => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(del.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(del.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(del.IsSecondary)));

            deliveryHeaders = this.AddSpecialTypes(types, deliveryDetailDb.ToList(), deliveryToReturn, deliveryHeaders, maquilaDeliverys.ToList(), ServiceConstants.Maquila);
            deliveryHeaders = this.AddSpecialTypes(types, deliveryDetailDb.ToList(), deliveryToReturn, deliveryHeaders, packageDeliveries.ToList(), ServiceConstants.Paquetes);
            deliveryHeaders = this.AddSpecialTypes(types, deliveryDetailDb.ToList(), deliveryToReturn, deliveryHeaders, omigenomicsDeliveries.ToList(), ServiceConstants.OmigenomicsGroup);

            deliveryHeaders = await this.GetSapDeliveriesToLookByPedidoDoctor(deliveryHeaders, parameters);
            deliveryHeaders = deliveryHeaders.OrderByDescending(x => x.DocNum).ToList();

            var filter = ServiceConstants.Filter
                .Where(x => types.Exists(type => x.Key.Contains(type.ToUpper())))
                .Select(x => x.Value);

            deliveryHeaders = deliveryHeaders
                .Where(x => types.Contains(x.TypeOrder) ||
                filter.Contains(x.TypeOrder))
                .ToList();

            var pedidos = deliveryHeaders.Select(x => x.PedidoId);

            deliveryToReturn = deliveryDetailDb.Where(x => pedidos.Contains(x.DeliveryId)).OrderByDescending(x => x.DeliveryId).ToList();
            var filterCount = deliveryHeaders.DistinctBy(x => x.DocNum).Count();
            deliveryHeaders = this.GetOrdersToLook(deliveryHeaders, parameters);

            return new Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int, List<InvoiceHeaderModel>>(deliveryToReturn, deliveryHeaders, filterCount, invoices.ToList());
        }

        private List<DeliverModel> AddSpecialTypes(List<string> types, List<DeliveryDetailModel> deliveryDetailDb, List<DeliveryDetailModel> deliveryToReturn, List<DeliverModel> deliveryHeaders, List<DeliverModel> specialDeliveries, string typeToLook)
        {
            var specialId = specialDeliveries.Select(md => md.DocNum).ToList();
            deliveryHeaders = deliveryHeaders.Where(d => deliveryToReturn.Select(x => x.DeliveryId).Distinct().Contains(d.DocNum)).ToList();

            if (types.Select(t => t.ToLower()).Contains(typeToLook.ToLower()))
            {
                deliveryHeaders.AddRange(specialDeliveries);
                deliveryToReturn.AddRange(deliveryDetailDb.Where(d => specialId.Contains(d.DeliveryId)));
            }
            else
            {
                deliveryHeaders = deliveryHeaders.Where(d => !specialId.Contains(d.DocNum)).ToList();
            }

            return deliveryHeaders;
        }

        /// <summary>
        /// Gets the order by the chips criteria.
        /// </summary>
        /// <param name="sapDeliveries">the orders.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<DeliverModel>> GetSapDeliveriesToLookByPedidoDoctor(List<DeliverModel> sapDeliveries, Dictionary<string, string> parameters)
        {
            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var transactionsIds = sapDeliveries.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
                var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
                var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                sapDeliveries = sapDeliveries.Where(x => ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, x.Address.ValidateNull(), payments.GetPaymentBydocNumDxp(x.DocNumDxp)) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapDeliveries;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return sapDeliveries.Where(x => x.DocNum == pedidoId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapDeliveries.Where(x => listNames.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets the deliveries to look.
        /// </summary>
        /// <param name="deliveries">All deliveries.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<DeliverModel> GetOrdersToLook(List<DeliverModel> deliveries, Dictionary<string, string> parameters)
        {
            var offset = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Offset, "0");
            var limit = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Limit, "1");

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var pedidosId = deliveries.Select(x => x.DocNum).Distinct().ToList();
            pedidosId = pedidosId.Skip(offsetNumber).Take(limitNumber).ToList();

            return deliveries.Where(x => pedidosId.Contains(x.DocNum)).ToList();
        }

        /// <summary>
        /// Gets the data to return.
        /// </summary>
        /// <param name="details">the delivery details.</param>
        /// <param name="headers">the delivery header.</param>
        /// <param name="invoices">The invoices.</param>
        /// <returns>the data.</returns>
        private AlmacenOrdersModel GetOrdersToReturn(List<DeliveryDetailModel> details, List<DeliverModel> headers, List<InvoiceHeaderModel> invoices)
        {
            var listIds = headers.Select(x => x.DocNum).Distinct().ToList();

            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = listIds.Count,
            };

            foreach (var d in listIds)
            {
                var header = headers.FirstOrDefault(x => x.DocNum == d);
                header ??= new DeliverModel { Medico = string.Empty, FechaInicio = DateTime.Now, Cliente = string.Empty, Address = string.Empty };
                header.Address = header.Address.ValidateNull();
                var deliveryDetail = details.Where(x => x.DeliveryId == d).DistinctBy(x => new { x.BaseEntry, x.ProductoId }).ToList();
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);

                var deliveryType = ServiceShared.CalculateTernary(deliveryDetail.All(x => x.Producto.IsLine == "Y"), ServiceConstants.LineaAlone, ServiceConstants.Mixto);
                deliveryType = ServiceShared.CalculateTernary(deliveryDetail.All(x => x.Producto.IsMagistral == "Y"), ServiceConstants.Magistral, deliveryType);

                var deliveryWithInvoice = deliveryDetail.FirstOrDefault(x => x.InvoiceId.HasValue && x.InvoiceId.Value != 0);
                deliveryWithInvoice ??= new DeliveryDetailModel { InvoiceId = 0 };
                var invoice = invoices.FirstOrDefault(x => x.InvoiceId == deliveryWithInvoice.InvoiceId.Value);
                var hasInvoice = ServiceShared.CalculateAnd(invoice != null, invoice?.Canceled == "N");

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = d,
                    Doctor = header.Medico,
                    InitDate = header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    HasInvoice = hasInvoice,
                    TypeOrder = header.TypeOrder,
                    DeliveryTypeModel = deliveryType,
                    IsPackage = header.IsPackage == ServiceConstants.IsPackage,
                    IsOmigenomics = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(header.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(header.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(header.IsSecondary)),
                };

                var saleModel = new SalesModel
                {
                    AlmacenHeader = null,
                    AlmacenSales = salesOrderModel,
                    SalesOrders = null,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        private async Task<List<SaleOrderByDeliveryModel>> CreateSaleCard(List<CompleteDeliveryDetailModel> details, List<UserOrderModel> userOrders, List<OrderModel> saleOrders, List<DetallePedidoModel> ordersDetail)
        {
            var listToReturn = new List<SaleOrderByDeliveryModel>();

            var classification = await this.sapDao.GetClassifications(details.Select(x => x.TypeOrder).Distinct().ToList());

            classification.ToList().ForEach(x =>
            {
                if (ServiceConstants.Descriptions.ContainsKey(x.Description))
                {
                    x.Description = ServiceConstants.Descriptions[x.Description];
                }
            });

            saleOrders.ForEach(s =>
            {
                var userOrder = userOrders.GetSaleOrderHeader(s.DocNum.ToString());
                var localDetails = details.Where(y => y.Detalles.BaseEntry == s.DocNum).ToList();

                string mixt = classification.Count() > 1 ? ServiceConstants.Mixto : string.Empty;
                var detailByOrder = ordersDetail.Where(x => x.PedidoId == s.DocNum).ToList();

                listToReturn.Add(new SaleOrderByDeliveryModel
                {
                    DocNum = s.DocNum,
                    Comments = userOrder?.Comments ?? string.Empty,
                    FechaInicio = s.FechaInicio,
                    Pieces = localDetails.Sum(y => (int)y.Detalles.Quantity),
                    Products = localDetails.Count,
                    Status = ServiceConstants.Almacenado,
                    SaleOrderType = !string.IsNullOrEmpty(mixt) ? string.Format(ServiceConstants.Description, mixt) : string.Format(ServiceConstants.Description, classification.Where(x => x.Value == s.OrderType).Select(x => x.Description).FirstOrDefault()),
                    IsPackage = s.IsPackage == ServiceConstants.IsPackage,
                    IsOmigenomics = ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(s.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(s.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(s.IsSecondary)),
                    OrderTotalPieces = (int)detailByOrder.Sum(x => x.Quantity),
                    OrderTotalProducts = detailByOrder.Count,
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the product data.
        /// </summary>
        /// <param name="deliveryDetails">The delivery details.</param>
        /// <returns>the data.</returns>
        private async Task<List<ProductListRemisionModel>> GetProductListModel(List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<IncidentsModel> incidents, List<ProductoModel> products, List<DetallePedidoModel> orderDetail)
        {
            var listToReturn = new List<ProductListRemisionModel>();
            var saleId = deliveryDetails.FirstOrDefault().BaseEntry ?? 0;
            var baseDelivery = deliveryDetails.FirstOrDefault().DeliveryId;
            var prodOrders = (await this.sapDao.GetFabOrderBySalesOrderId(new List<int> { saleId })).ToList();
            var batchesQty = await this.GetBatchesBySale(baseDelivery, saleId, deliveryDetails.Select(x => x.ProductoId).ToList());
            var tuple = batchesQty.Select(x => (x.SysNumber, x.ItemCode)).ToList();
            var batches = (await this.sapDao.GetSelectedBatches(tuple)).ToList();
            var themeIds = deliveryDetails.Select(x => x.Producto.ThemeId).Distinct().ToList();
            var themesResponse = await this.catalogsService.PostCatalogs(themeIds, ServiceConstants.GetThemes);
            var themes = JsonConvert.DeserializeObject<List<ProductColorsDto>>(themesResponse.Response.ToString());
            foreach (var order in deliveryDetails)
            {
                order.BaseEntry ??= 0;
                var item = products.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var saleDetail = prodOrders.FirstOrDefault(x => x.ProductoId == order.ProductoId);
                var orderId = saleDetail?.OrdenId.ToString() ?? string.Empty;

                var productType = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);
                var itemcode = item.ProductoId;
                var isChild = false;

                if (!string.IsNullOrEmpty(orderId))
                {
                    var selectedProduct = userOrders.Where(x => x.DeliveryId == order.DeliveryId && x.Salesorderid == order.BaseEntry.ToString() && !string.IsNullOrEmpty(x.Productionorderid)).FirstOrDefault() ?? new UserOrderModel() { Productionorderid = "0" };
                    var productionOrder = prodOrders.Where(x => x.OrdenId == int.Parse(selectedProduct.Productionorderid)).FirstOrDefault();
                    itemcode = $"{item.ProductoId} - {selectedProduct.Productionorderid}";
                    isChild = productionOrder.OrderRelationType == "N";
                }

                var listBatches = new List<string>();

                if (item.IsMagistral.Equals("N"))
                {
                    var lineProduct = lineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == order.BaseEntry, x.ItemCode == item.ProductoId));
                    lineProduct ??= new LineProductsModel();

                    var batchName = ServiceShared.DeserializeObject(lineProduct.BatchName, new List<AlmacenBatchModel>());
                    listBatches = this.GetBatchesByDelivery(order.ProductoId, batchesQty, batches, batchName);
                }

                var orderNum = string.IsNullOrEmpty(orderId) ? 0 : int.Parse(orderId);

                var incidentdb = incidents.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == order.BaseEntry, x.ItemCode == item.ProductoId));
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = ServiceShared.DeserializeObject(incidentdb.Batches, new List<AlmacenBatchModel>()),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                var selectedTheme = ServiceShared.GetSelectedTheme(order.Producto.ThemeId, themes);
                listToReturn.Add(new ProductListRemisionModel
                {
                    Container = order.Container,
                    Description = item.LargeDescription.ToUpper(),
                    ItemCode = itemcode,
                    NeedsCooling = item.NeedsCooling,
                    ProductType = $"Producto {productType}",
                    Pieces = order.Quantity,
                    Status = this.CalculateStatus(userOrders, lineProducts, item.IsMagistral, order.BaseEntry.Value, orderNum, item.ProductoId),
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    Batches = listBatches,
                    Incident = ServiceShared.CalculateTernary(string.IsNullOrEmpty(localIncident.Status), null, localIncident),
                    DeliveryId = order.DeliveryId,
                    SaleOrderId = order.BaseEntry.Value,
                    TotalOrderPieces = (int)orderDetail.Where(x => x.ProductoId == itemcode).Sum(x => x.Quantity),
                    BackgroundColor = selectedTheme.BackgroundColor,
                    LabelText = selectedTheme.LabelText,
                    LabelColor = selectedTheme.TextColor,
                    IsChild = isChild,
                });
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the batches by the sale.
        /// </summary>
        /// <param name="deliveryId">the delivery.</param>
        /// <param name="saleId">The sale id.</param>
        /// <param name="itemCode">The codes.</param>
        /// <returns>the data.</returns>
        private async Task<List<BatchesTransactionQtyModel>> GetBatchesBySale(int deliveryId, int saleId, List<string> itemCode)
        {
            var batchesBySale = (await this.sapDao.GetBatchesTransactionByOrderAndItemCodes(new List<int> { deliveryId }, itemCode, saleId)).ToList();
            batchesBySale = batchesBySale.Where(x => ServiceShared.CalculateAnd(itemCode.Contains(x.ItemCode), x.BaseEntry == saleId)).ToList();
            var batchesQty = (await this.sapDao.GetBatchTransationsQtyByLogEntry(batchesBySale.Select(x => x.LogEntry).ToList())).ToList();
            return batchesQty;
        }

        /// <summary>
        /// Gets the batches.
        /// </summary>
        /// <param name="itemCode">the code.</param>
        /// <param name="batchTrans">the trans.</param>
        /// <param name="validBatches">the valid batches.</param>
        /// <param name="batchName">the batches from sales.</param>
        /// <returns>the data.</returns>
        private List<string> GetBatchesByDelivery(string itemCode, List<BatchesTransactionQtyModel> batchTrans, List<CompleteBatchesJoinModel> validBatches, List<AlmacenBatchModel> batchName)
        {
            var batchTransLocal = batchTrans.Where(x => x.ItemCode == itemCode).ToList();
            var validBatchesByItemcode = validBatches.Where(x => x.ItemCode == itemCode).ToList();
            var batchesToLoop = validBatchesByItemcode.Where(x => ServiceShared.CalculateAnd(batchTransLocal.Any(y => y.SysNumber == x.SysNumber))).ToList();

            var listToReturn = batchesToLoop
                .Select(z => $"{batchName.GetBatch(z.DistNumber).WarehouseCode ?? "PT"} | {z.DistNumber} | {(int)batchName.GetBatch(z.DistNumber).BatchQty} pz | Cad: {z.FechaExp}")
                .ToList();
            return listToReturn;
        }

        private string CalculateStatus(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, string isMagistral, int saleOrder, int orderId, string itemCode)
        {
            if (isMagistral.Equals("Y"))
            {
                var userOrder = userOrders.FirstOrDefault(x => ServiceShared.CalculateAnd(x.Salesorderid == saleOrder.ToString(), x.Productionorderid == orderId.ToString()));
                return ServiceShared.CalculateTernary(userOrder == null, ServiceConstants.Almacenado, userOrder?.StatusAlmacen);
            }

            var lineProduct = lineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == saleOrder, x.ItemCode == itemCode));
            return ServiceShared.CalculateTernary(lineProduct == null, ServiceConstants.Almacenado, lineProduct?.StatusAlmacen);
        }
    }
}
