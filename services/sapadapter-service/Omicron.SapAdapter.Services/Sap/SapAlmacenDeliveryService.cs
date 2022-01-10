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
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenDeliveryService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        public SapAlmacenDeliveryService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDelivery(Dictionary<string, string> parameters)
        {
            var typesString = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.AllTypes);
            var types = typesString.Split(",").ToList();

            var userOrders = await this.GetUserOrders();
            var lineProducts = await this.GetLineProducts();

            var deliveryIds = userOrders.Select(x => x.DeliveryId).Distinct().ToList();
            deliveryIds.AddRange(lineProducts.Select(x => x.DeliveryId).Distinct().ToList());

            var sapResponse = await this.GetOrdersByType(types, userOrders, lineProducts, parameters);
            var dataToReturn = this.GetOrdersToReturn(sapResponse.Item1, sapResponse.Item2, sapResponse.Item4);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{sapResponse.Item3}-{sapResponse.Item3}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersDeliveryDetail(int deliveryId)
        {
            var deliveryDetails = await this.sapDao.GetDeliveryDetailForDeliveryById(new List<int> { deliveryId });

            var saleOrders = deliveryDetails.Where(y => y.Detalles.BaseEntry.HasValue).Select(x => x.Detalles.BaseEntry.Value).Distinct().ToList();

            var sapSaleOrders = await this.sapDao.GetOrdersById(saleOrders);
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var pedidosResponse = await this.pedidosService.PostPedidos(saleOrders, ServiceConstants.GetUserSalesOrder);
            var pedidos = JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());

            var dataToReturn = new SalesModel();

            dataToReturn.SalesOrders = this.CreateSaleCard(deliveryDetails, pedidos, sapSaleOrders);
            dataToReturn.AlmacenHeader = new AlmacenSalesHeaderModel
            {
                Client = deliveryDetails.FirstOrDefault().Cliente,
                DocNum = saleOrders.Count,
                Doctor = deliveryDetails.FirstOrDefault().Medico,
                InitDate = deliveryDetails.FirstOrDefault().FechaInicio,
                Status = ServiceConstants.Almacenado,
                TotalItems = dataToReturn.SalesOrders.Sum(x => x.Products),
                TotalPieces = dataToReturn.SalesOrders.Sum(x => x.Pieces),
                Remision = deliveryId,
                InvoiceType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, deliveryDetails.FirstOrDefault().Address) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                TypeOrder = deliveryDetails.FirstOrDefault().TypeOrder,
                HasInvoice = deliveryDetails.FirstOrDefault().Detalles.InvoiceId.HasValue,
            };

            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetProductsDelivery(string saleId)
        {
            var saleArray = saleId.Split("-").ToList();
            int.TryParse(saleArray[0], out var orderSaleId);
            int.TryParse(saleArray[1], out var deliveryId);

            var userOrders = await this.GetUserOrders();
            var lineProducts = await this.GetLineProducts();
            userOrders = userOrders.Where(x => x.Salesorderid == orderSaleId.ToString()).ToList();
            lineProducts = lineProducts.Where(x => x.SaleOrderId == orderSaleId).ToList();

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailBySaleOrderJoinProduct(new List<int> { orderSaleId })).ToList();
            deliveryDetails = deliveryDetails.Where(x => x.DeliveryId == deliveryId).ToList();

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { orderSaleId });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var productsIds = deliveryDetails.Select(y => y.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();

            var items = await this.GetProductListModel(deliveryDetails, userOrders, lineProducts, incidents, productItems);
            return ServiceUtils.CreateResult(true, 200, null, items, null, null);
        }

        /// <summary>
        /// Gets the orders from user Orders.
        /// </summary>
        /// <returns>the user orders.</returns>
        private async Task<List<UserOrderModel>> GetUserOrders()
        {
            var pedidosResponse = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrderDelivery);
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the line products.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetLineProducts()
        {
            var lineProductsResponse = await this.almacenService.GetAlmacenOrders(ServiceConstants.GetLinesForDelivery);
            return JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());
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

            var deliveryDetailDb = (await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(listDeliveryIds)).ToList();
            var invoices = (await this.sapDao.GetInvoiceHeaderByInvoiceId(deliveryDetailDb.Where(x => x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList())).ToList();
            var invoiceRefactura = invoices.Where(x => !string.IsNullOrEmpty(x.Refactura) && x.Refactura == ServiceConstants.IsRefactura).Select(y => y.InvoiceId).ToList();
            invoices = invoices.Where(x => string.IsNullOrEmpty(x.Refactura) || x.Refactura != ServiceConstants.IsRefactura).ToList();
            deliveryDetailDb = deliveryDetailDb.Where(x => !x.InvoiceId.HasValue || !invoiceRefactura.Contains(x.InvoiceId.Value)).ToList();
            var sapOrdersGroup = deliveryDetailDb.GroupBy(x => x.DeliveryId).ToList();

            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);

            var deliveryToReturn = new List<DeliveryDetailModel>();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => !x.Any(y => lineProducts.Contains(y.ProductoId))).ToList();
                var keys = listMagistral.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keys.Contains(x.DeliveryId)));
                sapOrdersGroup.RemoveAll(x => keys.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => !x.All(y => lineProducts.Contains(y.ProductoId)) && x.Any(y => lineProducts.Contains(y.ProductoId))).ToList();
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keysMixta.Contains(x.DeliveryId)));
                sapOrdersGroup.RemoveAll(x => keysMixta.Contains(x.Key));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => x.All(y => lineProducts.Contains(y.ProductoId)));
                var keysLine = listMixta.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keysLine.Contains(x.DeliveryId)));
                sapOrdersGroup.RemoveAll(x => keysLine.Contains(x.Key));
            }

            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNumJoinDoctor(deliveryToReturn.Select(x => x.DeliveryId).Distinct().ToList())).ToList();

            if (!types.Contains(ServiceConstants.Maquila.ToLower()))
            {
                var listMaquila = deliveryHeaders.Where(x => x.TypeOrder == ServiceConstants.OrderTypeMQ).Select(x => x.DocNum).ToList();
                deliveryHeaders = deliveryHeaders.Where(x => !listMaquila.Contains(x.DocNum)).ToList();
                deliveryToReturn = deliveryToReturn.Where(x => !listMaquila.Contains(x.DeliveryId)).ToList();
            }

            deliveryHeaders = await this.GetSapDeliveriesToLookByPedidoDoctor(deliveryHeaders, parameters);
            deliveryHeaders = deliveryHeaders.OrderByDescending(x => x.DocNum).ToList();
            var filterCount = deliveryHeaders.DistinctBy(x => x.DocNum).Count();

            deliveryHeaders = this.GetOrdersToLook(deliveryHeaders, parameters);
            deliveryToReturn = deliveryToReturn.Where(x => deliveryHeaders.Any(y => y.DocNum == x.DeliveryId)).ToList();
            deliveryToReturn = deliveryToReturn.OrderByDescending(x => x.DeliveryId).ToList();

            return new Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int, List<InvoiceHeaderModel>>(deliveryToReturn, deliveryHeaders, filterCount, invoices);
        }

        /// <summary>
        /// Gets the order by the chips criteria.
        /// </summary>
        /// <param name="sapOrders">the orders.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<DeliverModel>> GetSapDeliveriesToLookByPedidoDoctor(List<DeliverModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                sapOrders = sapOrders.Where(x => ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, x.Address.ValidateNull()) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return sapOrders.Where(x => x.DocNum == pedidoId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => listNames.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
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
            var listIds = details.Select(x => x.DeliveryId).Distinct().ToList();

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
                header.Address = string.IsNullOrEmpty(header.Address) ? string.Empty : header.Address;
                var deliveryDetail = details.Where(x => x.DeliveryId == d).DistinctBy(x => new { x.BaseEntry, x.ProductoId }).ToList();
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);

                var deliveryType = ServiceShared.CalculateTernary(deliveryDetail.All(x => x.Producto.IsLine == "Y"), ServiceConstants.LineaAlone, ServiceConstants.Mixto);
                deliveryType = ServiceShared.CalculateTernary(deliveryDetail.All(x => x.Producto.IsMagistral == "Y"), ServiceConstants.Magistral, deliveryType);

                var deliveryWithInvoice = deliveryDetail.FirstOrDefault(x => x.InvoiceId.HasValue && x.InvoiceId.Value != 0);
                deliveryWithInvoice ??= new DeliveryDetailModel { InvoiceId = 0 };
                var invoice = invoices.FirstOrDefault(x => x.InvoiceId == deliveryWithInvoice.InvoiceId.Value);
                var hasInvoice = invoice != null && invoice.InvoiceStatus != "C";

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

        private List<SaleOrderByDeliveryModel> CreateSaleCard(List<CompleteDeliveryDetailModel> details, List<UserOrderModel> userOrders, List<OrderModel> saleOrders)
        {
            var listToReturn = new List<SaleOrderByDeliveryModel>();

            saleOrders.ForEach(s =>
            {
                var userOrder = userOrders.FirstOrDefault(y => y.Salesorderid == s.DocNum.ToString() && string.IsNullOrEmpty(y.Productionorderid));
                var localDetails = details.Where(y => y.Detalles.BaseEntry == s.DocNum).ToList();

                var productType = ServiceShared.CalculateTernary(localDetails.All(y => y.Producto.IsMagistral == "Y"), ServiceConstants.Magistral, ServiceConstants.Mixto);
                productType = ServiceShared.CalculateTernary(localDetails.All(y => y.Producto.IsLine == "Y"), ServiceConstants.Linea, productType);

                listToReturn.Add(new SaleOrderByDeliveryModel
                {
                    DocNum = s.DocNum,
                    Comments = userOrder?.Comments ?? string.Empty,
                    FechaInicio = s.FechaInicio,
                    Pieces = localDetails.Sum(y => (int)y.Detalles.Quantity),
                    Products = localDetails.Count,
                    Status = ServiceConstants.Almacenado,
                    SaleOrderType = $"Pedido {productType}",
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the product data.
        /// </summary>
        /// <param name="deliveryDetails">The delivery details.</param>
        /// <returns>the data.</returns>
        private async Task<List<ProductListModel>> GetProductListModel(List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<IncidentsModel> incidents, List<ProductoModel> products)
        {
            var listToReturn = new List<ProductListModel>();
            var saleId = deliveryDetails.FirstOrDefault().BaseEntry ?? 0;
            var baseDelivery = deliveryDetails.FirstOrDefault().DeliveryId;
            var prodOrders = (await this.sapDao.GetFabOrderBySalesOrderId(new List<int> { saleId })).ToList();
            var batchesQty = await this.GetBatchesBySale(baseDelivery, saleId, deliveryDetails.Select(x => x.ProductoId).ToList());
            var batches = await this.GetValidBatches(deliveryDetails.Select(x => x.ProductoId).ToList(), ServiceConstants.PT);

            foreach (var order in deliveryDetails)
            {
                order.BaseEntry ??= 0;
                var item = products.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var productType = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

                var saleDetail = prodOrders.FirstOrDefault(x => x.ProductoId == order.ProductoId);
                var orderId = saleDetail == null ? string.Empty : saleDetail.OrdenId.ToString();
                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;

                var listBatches = new List<string>();

                if (item.IsMagistral.Equals("N"))
                {
                    var lineProduct = lineProducts.FirstOrDefault(x => x.SaleOrderId == order.BaseEntry && x.ItemCode == item.ProductoId);
                    lineProduct ??= new LineProductsModel();

                    var batchName = string.IsNullOrEmpty(lineProduct.BatchName) ? new List<AlmacenBatchModel>() : JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineProduct.BatchName);
                    listBatches = this.GetBatchesByDelivery(order.ProductoId, batchesQty, batches, batchName);
                }

                var orderNum = string.IsNullOrEmpty(orderId) ? 0 : int.Parse(orderId);

                var incidentdb = incidents.FirstOrDefault(x => x.SaleOrderId == order.BaseEntry && x.ItemCode == item.ProductoId);
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = !string.IsNullOrEmpty(incidentdb.Batches) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(incidentdb.Batches) : new List<AlmacenBatchModel>(),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                var productModel = new ProductListModel
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
                    Incident = string.IsNullOrEmpty(localIncident.Status) ? null : localIncident,
                    DeliveryId = order.DeliveryId,
                    SaleOrderId = order.BaseEntry.Value,
                };

                listToReturn.Add(productModel);
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
            var listLastTransactions = new List<BatchTransacitions>();
            var batchesBySale = (await this.sapDao.GetBatchesTransactionByOrderItem(new List<int> { deliveryId })).ToList();
            batchesBySale = batchesBySale.Where(x => itemCode.Contains(x.ItemCode) && x.BaseEntry == saleId).ToList();
            batchesBySale.GroupBy(x => x.ItemCode).Where(a => a.Any()).ToList().ForEach(y =>
            {
                var logs = y.OrderBy(z => z.LogEntry).ToList();
                listLastTransactions.Add(logs.Any() ? logs.Last() : null);
            });

            listLastTransactions = listLastTransactions.Where(x => x != null).ToList();
            var batchesQty = (await this.sapDao.GetBatchTransationsQtyByLogEntry(listLastTransactions.Select(x => x.LogEntry).ToList())).ToList();
            return batchesQty;
        }

        /// <summary>
        /// Get the valid batchs.
        /// </summary>
        /// <param name="products">the products.</param>
        /// <param name="whsCode">the code.</param>
        /// <returns>the batches.</returns>
        private async Task<List<CompleteBatchesJoinModel>> GetValidBatches(List<string> products, string whsCode)
        {
            var listComponents = new List<CompleteDetalleFormulaModel>();

            foreach (var item in products)
            {
                listComponents.Add(new CompleteDetalleFormulaModel { ProductId = item, Warehouse = whsCode });
            }

            return (await this.sapDao.GetValidBatches(listComponents)).ToList();
        }

        /// <summary>
        /// Gets the batches.
        /// </summary>
        /// <param name="itemCode">the code.</param>
        /// <param name="batchTrans">the trans.</param>
        /// <param name="validBatches">the valid batches.</param>
        /// <param name="batchName">the batches from sales.</param>
        /// <returns>the data.</returns>
        private List<string> GetBatchesByDelivery(string itemCode, List<BatchesTransactionQtyModel> batchTrans, List<CompleteBatchesJoinModel> validBatches,  List<AlmacenBatchModel> batchName)
        {
            var listToReturn = new List<string>();
            var batchTransLocal = batchTrans.Where(x => x.ItemCode == itemCode).ToList();
            var batchesToLoop = validBatches.Where(x => batchTransLocal.Any(y => y.SysNumber == x.SysNumber) && x.ItemCode == itemCode).ToList();
            batchesToLoop.ForEach(z =>
            {
                var batch = batchName.FirstOrDefault(a => a.BatchNumber == z.DistNumber);
                batch ??= new AlmacenBatchModel() { BatchQty = 0 };
                listToReturn.Add($"{z.DistNumber} | {(int)batch.BatchQty} pz | Cad: {z.FechaExp}");
            });

            return listToReturn;
        }

        private string CalculateStatus(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, string isMagistral, int saleOrder, int orderId, string itemCode)
        {
            if (isMagistral.Equals("Y"))
            {
                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid == saleOrder.ToString() && x.Productionorderid == orderId.ToString());
                return userOrder == null ? ServiceConstants.Almacenado : userOrder.StatusAlmacen;
            }

            var lineProduct = lineProducts.FirstOrDefault(x => x.SaleOrderId == saleOrder && x.ItemCode == itemCode);
            return lineProduct == null ? ServiceConstants.Almacenado : lineProduct.StatusAlmacen;
        }
    }
}
