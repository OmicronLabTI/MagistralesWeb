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

            var granTotal = deliveryIds.Where(x => x != 0).Distinct().ToList().Count;

            var sapResponse = await this.GetOrdersByType(types, userOrders, lineProducts, parameters);
            var dataToReturn = await this.GetOrdersToReturn(sapResponse.Item1, sapResponse.Item2, userOrders, lineProducts);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{granTotal}-{sapResponse.Item3}");
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
        private async Task<Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int>> GetOrdersByType(List<string> types, List<UserOrderModel> userOrders, List<LineProductsModel> lineModels, Dictionary<string, string> parameters)
        {
            var listDeliveryIds = lineModels.Select(x => x.DeliveryId).ToList();
            listDeliveryIds.AddRange(userOrders.Select(x => x.DeliveryId).ToList());
            listDeliveryIds = listDeliveryIds.OrderBy(x => x).Distinct().ToList();

            var deliveryDetailDb = (await this.sapDao.GetDeliveryByDocEntry(listDeliveryIds)).ToList();
            var sapOrdersGroup = deliveryDetailDb.GroupBy(x => x.DeliveryId).ToList();

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

            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(deliveryToReturn.Select(x => x.DeliveryId).Distinct().ToList())).ToList();
            deliveryHeaders = this.GetSapDeliveriesToLookByPedidoDoctor(deliveryHeaders, parameters);
            deliveryHeaders = deliveryHeaders.OrderBy(x => x.DocNum).ToList();
            var filterCount = deliveryHeaders.DistinctBy(x => x.DocNum).Count();

            deliveryHeaders = this.GetOrdersToLook(deliveryHeaders, parameters);
            deliveryToReturn = deliveryToReturn.Where(x => deliveryHeaders.Any(y => y.DocNum == x.DeliveryId)).ToList();

            return new Tuple<List<DeliveryDetailModel>, List<DeliverModel>, int>(deliveryToReturn, deliveryHeaders, filterCount);
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
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersModel> GetOrdersToReturn(List<DeliveryDetailModel> details, List<DeliverModel> headers, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var listIds = details.Select(x => x.DeliveryId).Distinct().ToList();

            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = listIds.Count,
            };

            var listSale = details.Where(x => listIds.Contains(x.DeliveryId)).Select(x => x.BaseEntry).ToList();
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, listSale);
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var productsIds = details.Where(x => listIds.Contains(x.DeliveryId)).Select(y => y.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();
            foreach (var d in listIds)
            {
                var header = headers.FirstOrDefault(x => x.DocNum == d);
                var deliveryDetail = details.Where(x => x.DeliveryId == d).ToList();
                var saleOrder = deliveryDetail.FirstOrDefault() != null ? deliveryDetail.FirstOrDefault().BaseEntry : 0;
                var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == saleOrder.ToString());
                var userOrdersBySale = userOrders.Where(x => x.Salesorderid == saleOrder.ToString()).ToList();
                var lineProductsBySale = lineProducts.Where(x => x.SaleOrderId == saleOrder).ToList();
                var doctor = header == null ? string.Empty : header.Medico;
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);
                var productList = await this.GetProductListModel(deliveryDetail, userOrdersBySale, lineProductsBySale, incidents, productItems);
                var productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;

                header.Address = string.IsNullOrEmpty(header.Address) ? string.Empty : header.Address;
                var invoiceType = header.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = d,
                    Doctor = doctor,
                    InitDate = header == null ? DateTime.Now : header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    HasInvoice = deliveryDetail.Any(d => d.InvoiceId.HasValue && d.InvoiceId.Value != 0),
                    TypeOrder = header.TypeOrder,
                };

                var saleHeader = new AlmacenSalesHeaderModel
                {
                    Client = header == null ? string.Empty : header.Cliente,
                    DocNum = saleOrder,
                    Comments = userOrder == null ? string.Empty : userOrder.Comments,
                    Doctor = doctor,
                    InitDate = header == null ? DateTime.Now : header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    Remision = d,
                    InvoiceType = invoiceType,
                    TypeOrder = header.TypeOrder,
                };

                var saleModel = new SalesModel
                {
                    AlmacenHeader = saleHeader,
                    AlmacenSales = salesOrderModel,
                    Items = productList,
                };

                listToReturn.TotalItems += productList.Count;
                listToReturn.SalesOrders.Add(saleModel);
            }

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
            var saleDetails = (await this.sapDao.GetAllDetails(saleId)).ToList();

            foreach (var order in deliveryDetails)
            {
                var item = products.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == order.ProductoId);
                var orderId = saleDetail == null ? string.Empty : saleDetail.OrdenFabricacionId.ToString();
                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;

                var listBatches = new List<string>();

                if (item.IsMagistral.Equals("N"))
                {
                    var lineProduct = lineProducts.FirstOrDefault(x => x.SaleOrderId == order.BaseEntry && x.ItemCode == item.ProductoId);
                    lineProduct ??= new LineProductsModel();

                    var batchName = string.IsNullOrEmpty(lineProduct.BatchName) ? new List<AlmacenBatchModel>() : JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineProduct.BatchName);

                    listBatches = await this.GetBatchesByDelivery(order.DeliveryId, order.ProductoId, batchName);
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
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }

        /// <summary>
        /// Get the batches by delivery.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        private async Task<List<string>> GetBatchesByDelivery(int delivery, string itemCode, List<AlmacenBatchModel> batchName)
        {
            var listToReturn = new List<string>();
            var batchTransacion = await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, delivery);
            var lastBatch = batchTransacion == null || !batchTransacion.Any() ? 0 : batchTransacion.Last().LogEntry;
            var batchTrans = (await this.sapDao.GetBatchTransationsQtyByLogEntry(new List<int> { lastBatch })).ToList();

            var listComponents = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { ProductId = itemCode, Warehouse = ServiceConstants.PT },
            };

            var validBatches = (await this.sapDao.GetValidBatches(listComponents)).ToList();

            validBatches.Where(x => batchTrans.Any(y => y.SysNumber == x.SysNumber)).ToList().ForEach(z =>
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
