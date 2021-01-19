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

            var listSaleOrders = lineProducts.Where(y => string.IsNullOrEmpty(y.ItemCode) && y.StatusAlmacen != ServiceConstants.Empaquetado).Select(x => x.DeliveryId).ToList();
            listSaleOrders.AddRange(userOrders.Where(y => string.IsNullOrEmpty(y.Productionorderid) && y.StatusAlmacen != ServiceConstants.Empaquetado).Select(x => x.DeliveryId).ToList());
            listSaleOrders = listSaleOrders.OrderBy(x => x).Distinct().ToList();

            var dataToReturn = await this.GetOrdersToReturn(deliveryDetails, deliveryHeaders, userOrders, lineProducts);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, null);
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

        private async Task<List<DeliveryDetailModel>, List<DeliverModel>> GetOrdersByType(List<string> types, List<UserModel> userOrders, List<LineProductsModel> lineModels)
        {
            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(listInvoiceId)).ToList();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => x.Count() == orderHeaders.Where(y => y.PedidoId == x.Key).Count());
                var keys = listMagistral.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keys.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => x.Count() != orderHeaders.Where(y => y.PedidoId == x.Key).Count() && orderHeaders.Where(y => y.PedidoId == x.Key).Count() > 0);
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => orderHeaders.Where(y => y.PedidoId == x.Key).Count() == 0);
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(listSaleOrders)).OrderBy(x => x.DocDate).ThenBy(y => y.DeliveryId).ToList();
            deliveryDetails = this.GetOrdersToLook(deliveryDetails, parameters);
            ////var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(deliveryDetails.Select(x => x.DeliveryId).Distinct().ToList())).ToList();
        }

        /// <summary>
        /// Gets the deliveries to look.
        /// </summary>
        /// <param name="deliveries">All deliveries.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<DeliveryDetailModel> GetOrdersToLook(List<DeliveryDetailModel> deliveries, Dictionary<string, string> parameters)
        {
            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var pedidosId = deliveries.Select(x => x.DeliveryId).Distinct().ToList();
            pedidosId = pedidosId.Skip(offsetNumber).Take(limitNumber).ToList();

            return deliveries.Where(x => pedidosId.Contains(x.DeliveryId)).ToList();
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

                var productList = await this.GetProductListModel(deliveryDetail, userOrdersBySale, lineProductsBySale);

                var productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = d,
                    Doctor = doctor,
                    InitDate = header == null ? DateTime.Now : header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    HasInvoice = deliveryDetail.Any(d => d.InvoiceId.HasValue && d.InvoiceId.Value != 0),
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
        private async Task<List<ProductListModel>> GetProductListModel(List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var listToReturn = new List<ProductListModel>();
            var saleId = deliveryDetails.FirstOrDefault().BaseEntry;
            var saleDetails = (await this.sapDao.GetAllDetails(saleId)).ToList();

            foreach (var order in deliveryDetails)
            {
                var item = (await this.sapDao.GetProductById(order.ProductoId)).FirstOrDefault();
                item = item == null ? new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty } : item;

                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == order.ProductoId);
                var orderId = saleDetail == null ? string.Empty : saleDetail.OrdenFabricacionId.ToString();
                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;

                var listBatches = new List<string>();

                if (item.IsMagistral.Equals("N"))
                {
                    listBatches = await this.GetBatchesByDelivery(order.DeliveryId, order.ProductoId);
                }

                var orderNum = string.IsNullOrEmpty(orderId) ? 0 : int.Parse(orderId);

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
        private async Task<List<string>> GetBatchesByDelivery(int delivery, string itemCode)
        {
            var listToReturn = new List<string>();
            var batchTransacion = await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, delivery);
            var lastBatch = batchTransacion == null || !batchTransacion.Any() ? 0 : batchTransacion.Last().LogEntry;
            var batchTrans = (await this.sapDao.GetBatchTransationsQtyByLogEntry(lastBatch)).ToList();

            var validBatches = (await this.sapDao.GetValidBatches(itemCode, ServiceConstants.PT)).ToList();

            validBatches.Where(x => batchTrans.Any(y => y.SysNumber == x.SysNumber)).ToList().ForEach(z =>
            {
                listToReturn.Add($"{z.DistNumber} | Cad: {z.FechaExp}");
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
