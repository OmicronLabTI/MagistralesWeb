// <summary>
// <copyright file="SapInvoiceService.cs" company="Axity">
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
    /// Class for the invoces.
    /// </summary>
    public class SapInvoiceService : ISapInvoiceService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapInvoiceService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        public SapInvoiceService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
        }

        /// <summary>
        /// Gets the invoices.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetInvoice(Dictionary<string, string> parameters)
        {
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserOrderInvoice);
            var lineProducts = await this.GetLineProducts(ServiceConstants.GetLinesForInvoice);

            var listIds = userOrders.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
            listIds.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Select(y => y.SaleOrderId));

            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(listIds)).ToList();
            var invoicesId = deliveryDetails.Where(y => y.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();

            var invoiceHeaders = (await this.sapDao.GetInvoiceHeaderByDocNum(invoicesId)).ToList();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(invoicesId)).ToList();
            var fabOrders = (await this.sapDao.GetFabOrderBySalesOrderId(listIds)).ToList();

            var idsToLook = this.GetInvoicesToLook(parameters, invoiceHeaders);

            invoiceHeaders = invoiceHeaders.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();
            invoiceDetails = invoiceDetails.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();

            var retrieveMode = new RetrieveInvoiceModel
            {
                DeliveryDetailModel = deliveryDetails,
                InvoiceDetails = invoiceDetails,
                InvoiceHeader = invoiceHeaders,
                LineProducts = lineProducts,
                OrdersModel = fabOrders,
                UserOrders = userOrders,
            };

            var dataToReturn = this.GetInvoiceToReturn(retrieveMode);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, null);
        }

        /// <summary>
        /// Gets the orders from user Orders.
        /// </summary>
        /// <returns>the user orders.</returns>
        private async Task<List<UserOrderModel>> GetUserOrders(string route)
        {
            var pedidosResponse = await this.pedidosService.GetUserPedidos(route);
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the line products.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetLineProducts(string route)
        {
            var lineProductsResponse = await this.almacenService.GetAlmacenOrders(route);
            return JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the ids to look.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <param name="invoiceHeaders">the invoice headers.</param>
        /// <returns>the ids to look.</returns>
        private List<int> GetInvoicesToLook(Dictionary<string, string> parameters, List<InvoiceHeaderModel> invoiceHeaders)
        {
            var invoiceHeadersIds = invoiceHeaders.OrderBy(x => x.InvoiceId).Select(y => y.InvoiceId).ToList();
            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            return invoiceHeadersIds.Skip(offsetNumber).Take(limitNumber).ToList();
        }

        private InvoiceOrderModel GetInvoiceToReturn(RetrieveInvoiceModel retrieveModel)
        {
            var listToReturn = new InvoiceOrderModel
            {
                Invoices = new List<InvoicesModel>(),
                TotalInvioces = retrieveModel.InvoiceHeader.Count,
                TotalDeliveries = 0,
            };

            foreach (var invoice in retrieveModel.InvoiceHeader)
            {
                var invoiceDetails = retrieveModel.InvoiceDetails.Where(x => x.InvoiceId == invoice.InvoiceId).ToList();
                var deliveryDetails = retrieveModel.DeliveryDetailModel.Where(x => x.InvoiceId.HasValue && x.InvoiceId.Value == invoice.InvoiceId).ToList();

                var salesId = deliveryDetails.Select(x => x.BaseEntry).ToList();
                var orders = retrieveModel.OrdersModel.Where(x => salesId.Contains(x.PedidoId.Value)).ToList();
                var userOrders = retrieveModel.UserOrders.Where(x => salesId.Contains(int.Parse(x.Salesorderid))).ToList();
                var lineProducts = retrieveModel.LineProducts.Where(x => salesId.Contains(x.SaleOrderId)).ToList();

                var doctor = invoice.Medico ?? string.Empty;
                var totalDeliveries = deliveryDetails.Select(x => x.BaseEntry).Distinct().Count();
                var totalProducts = invoiceDetails.Count;

                var invoiceModel = new InvoiceSaleModel
                {
                    Doctor = doctor,
                    Invoice = invoice.DocNum,
                    Deliveries = totalDeliveries,
                    Products = totalProducts,
                    InvoiceDocDate = invoice.FechaInicio,
                };

                var destiny = invoice.Address.Split(",");

                var invoiceHeader = new InvoiceSaleHeaderModel
                {
                    Address = invoice.Address.Replace("\r", string.Empty),
                    Client = invoice.Cliente,
                    Doctor = doctor,
                    Invoice = invoice.DocNum,
                    InvoiceDocDate = invoice.FechaInicio,
                    ProductType = destiny.Count() < 3 || destiny[destiny.Count() - 3].Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                    TotalDeliveries = totalDeliveries,
                    TotalProducts = totalProducts,
                };

                var invoiceModelToAdd = new InvoicesModel
                {
                    Deliveries = this.GetDeliveryModel(deliveryDetails, invoiceDetails, userOrders, lineProducts),
                    InvoiceHeader = invoiceHeader,
                    InvoiceSale = invoiceModel,
                };

                listToReturn.TotalDeliveries += totalDeliveries;
                listToReturn.Invoices.Add(invoiceModelToAdd);
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the deliveries.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="invoiceDetails">the invoice details.</param>
        /// <param name="userOrderModels">the user order modesl.</param>
        /// <param name="lineProducts">the lines prodcuts.</param>
        /// <returns>the data.</returns>
        private List<InvoiceDeliveryModel> GetDeliveryModel(List<DeliveryDetailModel> delivery, List<InvoiceDetailModel> invoiceDetails, List<UserOrderModel> userOrderModels, List<LineProductsModel> lineProducts)
        {
            var listToReturn = new List<InvoiceDeliveryModel>();
            var userOrderStatus = userOrderModels.Where(z => !string.IsNullOrEmpty(z.Productionorderid)).Select(y => y.StatusAlmacen).ToList();
            userOrderStatus.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).Select(y => y.StatusAlmacen));
            delivery.DistinctBy(x => x.DeliveryId).ToList()
                .ForEach(y =>
                {
                    var deliveryModel = new InvoiceDeliveryModel
                    {
                        DeliveryId = y.DeliveryId,
                        DeliveryDocDate = y.DocDate,
                        SaleOrder = y.BaseEntry,
                        Status = userOrderStatus.Any() && userOrderStatus.All(z => z == ServiceConstants.Empaquetado) ? ServiceConstants.Empaquetado : ServiceConstants.Almacenado,
                        TotalItems = invoiceDetails.Where(a => a.BaseEntry.HasValue).Count(z => z.BaseEntry == y.DeliveryId),
                    };

                    listToReturn.Add(deliveryModel);
                });

            return listToReturn;
        }

        /// <summary>
        /// Gets the product list.
        /// </summary>
        /// <param name="invoices">the invoices.</param>
        /// <param name="deliveryDetails">the sale id.</param>
        /// <param name="userOrders">the userOrders.</param>
        /// <param name="lineProducts">the line products.</param>
        /// <param name="orders">the owor orders.</param>
        /// <returns>the products.</returns>
        private async Task<List<InvoiceProductModel>> GetProductModels(List<InvoiceDetailModel> invoices, List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders)
        {
            var listToReturn = new List<InvoiceProductModel>();
            invoices = invoices.Where(x => x.BaseEntry.HasValue).ToList();

            foreach (var invoice in invoices)
            {
                var listBatches = new List<string>();
                var item = (await this.sapDao.GetProductById(invoice.ProductoId)).FirstOrDefault();
                item = item == null ? new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty } : item;

                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                if (item.IsMagistral.Equals("N"))
                {
                    listBatches = await this.GetBatchesByDelivery(invoice.BaseEntry.Value, invoice.ProductoId, ServiceConstants.PT);
                }

                var productModel = new InvoiceProductModel
                {
                    Batches = listBatches,
                    Container = string.Empty,
                    Description = item.LargeDescription,
                    ItemCode = item.ProductoId,
                    NeedsCooling = item.NeedsCooling.Equals("Y"),
                    ProductType = $"Producto {productType}",
                    Quantity = invoice.Quantity,
                    Status = this.GetProductStatus(deliveryDetails, userOrders, lineProducts, orders, invoice),
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    DeliveryId = invoice.BaseEntry.Value,
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the status of a product.
        /// </summary>
        /// <param name="deliveries">the sale order.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="lineProducts">the line products.</param>
        /// <param name="orders">the orders OWOR.</param>
        /// <param name="invoice">the invoices.</param>
        /// <returns>the data.</returns>
        private string GetProductStatus(List<DeliveryDetailModel> deliveries, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders, InvoiceDetailModel invoice)
        {
            var status = ServiceConstants.Almacenado;

            var deliveriesDetail = deliveries.FirstOrDefault(x => x.DeliveryId == invoice.BaseEntry.Value);
            var saleId = deliveriesDetail == null ? 0 : deliveriesDetail.BaseEntry;
            var order = orders.FirstOrDefault(x => x.PedidoId == saleId && x.ProductoId == invoice.ProductoId);

            if (order != null)
            {
                var userOrder = userOrders.FirstOrDefault(x => x.Productionorderid == order.OrdenId.ToString());
                status = userOrder == null ? status : userOrder.StatusAlmacen;
            }
            else
            {
                var lineProduct = lineProducts.FirstOrDefault(x => x.SaleOrderId == saleId && x.ItemCode == invoice.ProductoId);
                status = lineProduct == null ? status : lineProduct.StatusAlmacen;
            }

            return status;
        }

        /// <summary>
        /// Get the batches by delivery.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="itemCode">the item code.</param>
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the data.</returns>
        private async Task<List<string>> GetBatchesByDelivery(int delivery, string itemCode, string warehouse)
        {
            var batchTransacion = await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, delivery);
            var lastBatch = batchTransacion == null || !batchTransacion.Any() ? 0 : batchTransacion.Last().LogEntry;
            var batchTrans = (await this.sapDao.GetBatchTransationsQtyByLogEntry(lastBatch)).ToList();

            var validBatches = (await this.sapDao.GetValidBatches(itemCode, warehouse)).ToList();

            var listToReturn = new List<string>();
            validBatches.Where(x => batchTrans.Any(y => y.SysNumber == x.SysNumber)).ToList().ForEach(z =>
            {
                listToReturn.Add($"{z.DistNumber} | Cad: {z.FechaExp}");
            });

            return listToReturn;
        }
    }
}
