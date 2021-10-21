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
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for the sap almacen delivery service.
    /// </summary>
    public class SapAlmacenDeliveryService : ISapAlmacenDeliveryService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenDeliveryService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        public SapAlmacenDeliveryService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDelivery(Dictionary<string, string> parameters)
        {
            var typesString = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.AllTypes;
            var types = typesString.Split(",").ToList();

            var userOrders = await this.GetUserOrders();
            var lineProducts = await this.GetLineProducts();

            var deliveryIds = userOrders.Select(x => x.DeliveryId).Distinct().ToList();
            deliveryIds.AddRange(lineProducts.Select(x => x.DeliveryId).Distinct().ToList());

            var sapResponse = await this.GetOrdersByType(types, userOrders, lineProducts, parameters);
            var dataToReturn = await this.GetOrdersToReturn(sapResponse.Item1, sapResponse.Item2, userOrders, sapResponse.Item5);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{sapResponse.Item4}-{sapResponse.Item3}");
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

            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(new List<int> { orderSaleId })).ToList();
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
        private async Task<Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int, int, List<InvoiceHeaderModel>>> GetOrdersByType(List<string> types, List<UserOrderModel> userOrders, List<LineProductsModel> lineModels, Dictionary<string, string> parameters)
        {
            var listDeliveryIds = lineModels.Select(x => x.DeliveryId).ToList();
            listDeliveryIds.AddRange(userOrders.Select(x => x.DeliveryId).ToList());
            listDeliveryIds = listDeliveryIds.OrderBy(x => x).Distinct().ToList();

            var deliveryDetailDb = (await this.sapDao.GetDeliveryByDocEntry(listDeliveryIds)).ToList();
            var invoices = (await this.sapDao.GetInvoiceHeaderByInvoiceId(deliveryDetailDb.Where(x => x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList())).ToList();
            var invoiceRefactura = invoices.Where(x => !string.IsNullOrEmpty(x.Refactura) && x.Refactura == ServiceConstants.IsRefactura).Select(y => y.InvoiceId).ToList();
            invoices = invoices.Where(x => string.IsNullOrEmpty(x.Refactura) || x.Refactura != ServiceConstants.IsRefactura).ToList();
            deliveryDetailDb = deliveryDetailDb.Where(x => !x.InvoiceId.HasValue || !invoiceRefactura.Contains(x.InvoiceId.Value)).ToList();
            var sapOrdersGroup = deliveryDetailDb.GroupBy(x => x.DeliveryId).ToList();
            var granTotal = sapOrdersGroup.Count;

            var lineProducts = (await this.sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();

            var deliveryToReturn = new List<DeliveryDetailModel>();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => !x.Any(y => lineProducts.Contains(y.ProductoId))).ToList();
                var keys = listMagistral.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keys.Contains(x.DeliveryId)));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => !x.All(y => lineProducts.Contains(y.ProductoId)) && x.Any(y => lineProducts.Contains(y.ProductoId))).ToList();
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keysMixta.Contains(x.DeliveryId)));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => x.All(y => lineProducts.Contains(y.ProductoId)));
                var keysLine = listMixta.Select(x => x.Key).ToList();

                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => keysLine.Contains(x.DeliveryId)));
            }

            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNumJoinDoctor(deliveryToReturn.Select(x => x.DeliveryId).Distinct().ToList())).ToList();

            if (types.Contains(ServiceConstants.Maquila.ToLower()))
            {
                var deliveryHeadersMaquila = (await this.sapDao.GetDeliveryModelByDocNumJoinDoctor(deliveryDetailDb.Select(x => x.DeliveryId).ToList())).ToList();
                var listMaquila = deliveryHeadersMaquila.Where(x => x.TypeOrder == ServiceConstants.OrderTypeMQ).ToList();
                deliveryHeaders.AddRange(listMaquila);
                deliveryHeaders = deliveryHeaders.DistinctBy(x => x.DocNum).ToList();
                deliveryToReturn.AddRange(deliveryDetailDb.Where(x => listMaquila.Select(l => l.DocNum).ToList().Contains(x.DeliveryId)));
            }
            else
            {
                deliveryHeaders = deliveryHeaders.Where(x => x.TypeOrder != ServiceConstants.OrderTypeMQ).ToList();
            }

            deliveryHeaders = this.GetSapDeliveriesToLookByPedidoDoctor(deliveryHeaders, parameters);
            deliveryHeaders = deliveryHeaders.OrderByDescending(x => x.DocNum).ToList();
            var filterCount = deliveryHeaders.DistinctBy(x => x.DocNum).Count();

            deliveryHeaders = this.GetOrdersToLook(deliveryHeaders, parameters);
            deliveryToReturn = deliveryToReturn.Where(x => deliveryHeaders.Any(y => y.DocNum == x.DeliveryId)).ToList();
            deliveryToReturn = deliveryToReturn.OrderByDescending(x => x.DeliveryId).ToList();

            return new Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int, int, List<InvoiceHeaderModel>>(deliveryToReturn, deliveryHeaders, filterCount, granTotal, invoices);
        }

        /// <summary>
        /// Gets the order by the chips criteria.
        /// </summary>
        /// <param name="sapOrders">the orders.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<DeliverModel> GetSapDeliveriesToLookByPedidoDoctor(List<DeliverModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return sapOrders.Where(x => x.DocNum == pedidoId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => listNames.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets the deliveries to look.
        /// </summary>
        /// <param name="deliveries">All deliveries.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<DeliverModel> GetOrdersToLook(List<DeliverModel> deliveries, Dictionary<string, string> parameters)
        {
            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

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
        /// <param name="userOrders">the user orders.</param>
        /// <param name="invoices">The invoices.</param>
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersModel> GetOrdersToReturn(List<DeliveryDetailModel> details, List<DeliverModel> headers, List<UserOrderModel> userOrders, List<InvoiceHeaderModel> invoices)
        {
            var listIds = details.Select(x => x.DeliveryId).Distinct().ToList();

            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = listIds.Count,
            };

            var productsIds = details.Where(x => listIds.Contains(x.DeliveryId)).Select(y => y.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();
            var saleOrdersByDeliveries = (await this.sapDao.GetOrdersById(details.Select(x => x.BaseEntry).ToList())).ToList();

            foreach (var d in listIds)
            {
                var header = headers.FirstOrDefault(x => x.DocNum == d);
                header ??= new DeliverModel { Medico = string.Empty, FechaInicio = DateTime.Now, Cliente = string.Empty, Address = string.Empty };
                header.Address = string.IsNullOrEmpty(header.Address) ? string.Empty : header.Address;
                var deliveryDetail = details.Where(x => x.DeliveryId == d).ToList();
                var saleOrders = deliveryDetail.FirstOrDefault() != null ? deliveryDetail.Select(x => x.BaseEntry).ToList() : new List<int> { 0 };
                var saleOrdersString = saleOrders.Select(y => y.ToString()).ToList();
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);

                var ordersByDelivery = saleOrdersByDeliveries.Where(x => deliveryDetail.Any(y => y.BaseEntry == x.DocNum)).ToList();
                var userOrdersBySale = userOrders.Where(x => saleOrdersString.Contains(x.Salesorderid)).ToList();
                var salesCards = this.CreateSaleCard(ordersByDelivery, deliveryDetail, userOrdersBySale, productItems);

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
                };

                var saleHeader = new AlmacenSalesHeaderModel
                {
                    Client = header.Cliente,
                    DocNum = saleOrders.Distinct().Count(),
                    Doctor = header.Medico,
                    InitDate = header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    Remision = d,
                    InvoiceType = header.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                    TypeOrder = header.TypeOrder,
                };

                var saleModel = new SalesModel
                {
                    AlmacenHeader = saleHeader,
                    AlmacenSales = salesOrderModel,
                    SalesOrders = salesCards,
                };

                listToReturn.TotalItems += salesCards.Sum(y => y.Products);
                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the sale cads.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <param name="details">the delivery details.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="products">the products.</param>
        /// <returns>the list to retur.</returns>
        private List<SaleOrderByDeliveryModel> CreateSaleCard(List<OrderModel> orders, List<DeliveryDetailModel> details, List<UserOrderModel> userOrders, List<ProductoModel> products)
        {
            var listToReturn = new List<SaleOrderByDeliveryModel>();

            orders.ForEach(x =>
            {
                var userOrder = userOrders.FirstOrDefault(y => y.Salesorderid == x.DocNum.ToString() && string.IsNullOrEmpty(y.Productionorderid));
                var localDetails = details.Where(y => y.BaseEntry == x.DocNum).ToList();
                var productsBySale = products.Where(y => localDetails.Any(z => z.ProductoId == y.ProductoId)).ToList();
                var productType = productsBySale.All(y => y.IsMagistral == "Y") ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productsBySale.All(y => y.IsMagistral != "Y") ? ServiceConstants.Linea : productType;
                listToReturn.Add(new SaleOrderByDeliveryModel
                {
                    DocNum = x.DocNum,
                    Comments = userOrder == null ? string.Empty : userOrder.Comments,
                    FechaInicio = x.FechaInicio,
                    Pieces = localDetails.Sum(y => (int)y.Quantity),
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
            var saleId = deliveryDetails.FirstOrDefault().BaseEntry;
            var baseDelivery = deliveryDetails.FirstOrDefault().DeliveryId;
            var prodOrders = (await this.sapDao.GetFabOrderBySalesOrderId(new List<int> { saleId })).ToList();
            var batchesQty = await this.GetBatchesBySale(baseDelivery, saleId, deliveryDetails.Select(x => x.ProductoId).ToList());
            var batches = await this.GetValidBatches(deliveryDetails.Select(x => x.ProductoId).ToList(), ServiceConstants.PT);

            foreach (var order in deliveryDetails)
            {
                var item = products.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

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
                    Status = this.CalculateStatus(userOrders, lineProducts, item.IsMagistral, order.BaseEntry, orderNum, item.ProductoId),
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    Batches = listBatches,
                    Incident = string.IsNullOrEmpty(localIncident.Status) ? null : localIncident,
                    DeliveryId = order.DeliveryId,
                    SaleOrderId = order.BaseEntry,
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
