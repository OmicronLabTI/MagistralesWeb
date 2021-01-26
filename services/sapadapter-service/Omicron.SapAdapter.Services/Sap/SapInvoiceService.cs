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

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoice(Dictionary<string, string> parameters)
        {
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserOrderInvoice);
            var lineProducts = await this.GetLineProducts(ServiceConstants.GetLinesForInvoice);

            var listIds = userOrders.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
            listIds.AddRange(lineProducts.Select(y => y.SaleOrderId).Distinct());

            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(listIds)).ToList();
            var invoicesId = deliveryDetails.Where(y => y.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();

            var invoiceHeaders = (await this.sapDao.GetInvoiceHeaderByInvoiceId(invoicesId)).ToList();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(invoicesId)).ToList();
            var granTotal = invoiceHeaders.DistinctBy(x => x.InvoiceId).ToList().Count;

            var idsToLook = this.GetInvoicesToLook(parameters, invoiceHeaders);
            invoiceHeaders = invoiceHeaders.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();
            invoiceDetails = invoiceDetails.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();

            var retrieveMode = new RetrieveInvoiceModel
            {
                DeliveryDetailModel = deliveryDetails,
                InvoiceDetails = invoiceDetails,
                InvoiceHeader = invoiceHeaders,
                LineProducts = lineProducts,
                UserOrders = userOrders,
            };

            var dataToReturn = this.GetInvoiceToReturn(retrieveMode);
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{granTotal}-{granTotal}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceProducts(int invoiceId)
        {
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserOrderInvoice);
            var lineProducts = await this.GetLineProducts(ServiceConstants.GetLinesForInvoice);

            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNum(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(new List<int> { invoiceHeader.InvoiceId })).ToList();
            var deliveryDetails = (await this.sapDao.GetDeliveryByDocEntry(invoiceDetails.Select(x => x.BaseEntry.Value).ToList())).ToList();
            var fabOrders = (await this.sapDao.GetFabOrderBySalesOrderId(deliveryDetails.Select(x => x.BaseEntry).ToList())).ToList();

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, deliveryDetails.Select(x => x.BaseEntry).ToList());
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var products = await this.GetProductModels(invoiceDetails, deliveryDetails, userOrders, lineProducts, fabOrders, incidents);
            return ServiceUtils.CreateResult(true, 200, null, products, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryScannedData(string code)
        {
            var dataArray = code.Split("-");
            var saleOrder = int.Parse(dataArray[0]);
            var deliveryId = int.Parse(dataArray[1]);

            var listSaleOrder = new List<int> { saleOrder };
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserSalesOrder, listSaleOrder);
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, listSaleOrder);

            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(listSaleOrder)).ToList();
            deliveryDetails = deliveryDetails.Where(x => x.DeliveryId == deliveryId).ToList();
            var invoiceId = deliveryDetails.FirstOrDefault() == null ? 0 : deliveryDetails.FirstOrDefault().InvoiceId.Value;

            var invoiceHeader = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader = invoiceHeader == null ? new InvoiceHeaderModel() : invoiceHeader;
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(new List<int> { invoiceHeader.InvoiceId })).ToList();
            var fabOrders = (await this.sapDao.GetFabOrderBySalesOrderId(listSaleOrder)).ToList();

            var deliveryData = await this.GetDeliveryScannedData(invoiceHeader, deliveryDetails, invoiceDetails, userOrders, lineOrders, fabOrders);
            return ServiceUtils.CreateResult(true, 200, null, deliveryData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetMagistralProductInvoice(string code)
        {
            var dataArray = code.Split("-");
            var saleOrder = int.Parse(dataArray[0]);
            var orderId = int.Parse(dataArray[1]);

            var details = (await this.sapDao.GetAllDetails(saleOrder)).ToList();
            var order = details.FirstOrDefault(x => x.OrdenFabricacionId == orderId);
            order = order == null ? new CompleteDetailOrderModel() : order;

            var itemCode = (await this.sapDao.GetProductById(order.CodigoProducto)).FirstOrDefault();
            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

            var sapData = await this.GetSaleOrderInvoiceDataByItemCode(saleOrder, itemCode.ProductoId);

            var product = new MagistralScannerModel
            {
                Container = order.Container,
                Description = itemCode.ProductoName,
                ItemCode = itemCode.ProductoId,
                NeedsCooling = itemCode.NeedsCooling,
                Pieces = sapData.Item1.Quantity,
                ProductType = $"Producto {productType}",
                DeliveryId = sapData.Item1.BaseEntry.Value,
                InvoiceId = sapData.Item2.DocNum,
            };

            return ServiceUtils.CreateResult(true, 200, null, product, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetLineProductInvoice(string code)
        {
            var dataArray = code.Split("-");
            var codeBar = dataArray[0];
            var saleOrder = int.Parse(dataArray[1]);

            var itemCode = (await this.sapDao.GetProductByCodeBar(codeBar)).FirstOrDefault();

            if (itemCode == null)
            {
                return ServiceUtils.CreateResult(true, 404, null, new LineScannerModel(), null, null);
            }

            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;
            var sapData = await this.GetSaleOrderInvoiceDataByItemCode(saleOrder, itemCode.ProductoId);
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, new List<int> { saleOrder });
            var lineProduct = lineOrders.FirstOrDefault(x => x.ItemCode == itemCode.ProductoId);
            lineProduct = lineProduct == null ? new LineProductsModel { BatchName = JsonConvert.SerializeObject(new List<AlmacenBatchModel>()) } : lineProduct;
            var batchModel = JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineProduct.BatchName);
            var batches = await this.GetBatchesForInvoice(itemCode, batchModel);

            var lineData = new LineScannerModel
            {
                Batches = batches,
                Description = itemCode.ProductoName,
                ItemCode = itemCode.ProductoId,
                ProductType = $"Producto {productType}",
                InvoiceId = sapData.Item2.DocNum,
                DeliveryId = sapData.Item1.BaseEntry.Value,
            };

            return ServiceUtils.CreateResult(true, 200, null, lineData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceHeader(InvoicePackageSapLookModel dataToLook)
        {
            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNum(dataToLook.InvoiceDocNums)).ToList();
            invoiceHeader = dataToLook.Type.Equals(ServiceConstants.Local.ToLower()) ? invoiceHeader.Where(x => x.Address.Contains(ServiceConstants.NuevoLeon)).ToList() : invoiceHeader.Where(x => !x.Address.Contains(ServiceConstants.NuevoLeon)).ToList();

            var total = invoiceHeader.Count;
            var invoiceHeaderOrdered = new List<InvoiceHeaderModel>();
            dataToLook.InvoiceDocNums.ForEach(y =>
            {
                var invoiceDb = invoiceHeader.FirstOrDefault(a => a.DocNum == y);

                if (invoiceDb != null)
                {
                    invoiceHeaderOrdered.Add(invoiceDb);
                }
            });

            invoiceHeaderOrdered = invoiceHeaderOrdered.Skip(dataToLook.Offset).Take(dataToLook.Limit).ToList();

            var invoicesDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(invoiceHeaderOrdered.Select(x => x.InvoiceId).ToList())).ToList();

            var invoicesNull = new List<int?>();
            invoicesDetails.Select(x => x.InvoiceId).ToList().ForEach(y => invoicesNull.Add(y));
            var deliveries = (await this.sapDao.GetDeliveryByInvoiceId(invoicesNull)).ToList();

            var deliveryCompanies = (await this.sapDao.GetDeliveryCompanyById(invoiceHeaderOrdered.Select(x => x.TransportCode).ToList())).ToList();
            var clients = (await this.sapDao.GetClientsById(invoiceHeaderOrdered.Select(x => x.CardCode).ToList())).ToList();

            invoiceHeaderOrdered.ForEach(x =>
            {
                var details = invoicesDetails.Where(y => y.InvoiceId == x.InvoiceId).ToList();
                var client = clients.FirstOrDefault(y => y.ClientId == x.CardCode);
                client ??= new ClientCatalogModel();

                var company = deliveryCompanies.FirstOrDefault(y => y.TrnspCode == x.TransportCode);
                company ??= new Repartidores { TrnspName = string.Empty };

                var saleOrders = deliveries.Where(y => y.InvoiceId.HasValue && y.InvoiceId == x.InvoiceId).ToList();

                x.Comments = $"{details.Where(y => y.BaseEntry.HasValue).DistinctBy(x => x.BaseEntry.Value).Count()}-{details.Count}";
                x.ClientEmail = client.Email;
                x.TransportName = company.TrnspName;
                x.SaleOrder = JsonConvert.SerializeObject(saleOrders.Select(y => y.BaseEntry).Distinct().ToList());
            });

            return ServiceUtils.CreateResult(true, 200, null, invoiceHeaderOrdered, null, total);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceData(string code)
        {
            int.TryParse(code, out var intDocNum);
            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNum(new List<int> { intDocNum })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();

            var packagesResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetPackagesByInvoice, new List<int> { intDocNum });
            var packages = JsonConvert.DeserializeObject<List<PackageModel>>(packagesResponse.Response.ToString());

            var status = !packages.Any() ? ServiceConstants.Empaquetado : packages.OrderBy(x => x.AssignedDate.Value).FirstOrDefault().Status;

            var model = new InvoiceDeliverModel
            {
                Address = invoiceHeader.Address,
                Client = invoiceHeader.Cliente,
                Comments = invoiceHeader.CommentsInvoice,
                Doctor = invoiceHeader.Medico,
                PackageNumber = invoiceHeader.DocNum,
                Status = status,
            };

            var comments = model.Address.Contains(ServiceConstants.NuevoLeon) ? string.Empty : ServiceConstants.ForeingPackage;
            comments = !status.Equals(ServiceConstants.Empaquetado) && !status.Equals(ServiceConstants.NoEntregado) ? $"{ServiceConstants.PackageNotAvailable} {status}" : comments;

            return ServiceUtils.CreateResult(true, 200, null, model, null, comments);
        }

        /// <summary>
        /// Gets the batches for the invoice.
        /// </summary>
        /// <param name="itemCode">the product.</param>
        /// <param name="batchModel">the batch model.</param>
        /// <returns>the batches.</returns>
        private async Task<List<LineProductBatchesModel>> GetBatchesForInvoice(ProductoModel itemCode, List<AlmacenBatchModel> batchModel)
        {
            var batches = new List<LineProductBatchesModel>();
            foreach (var b in batchModel)
            {
                var batchDb = (await this.sapDao.GetBatchByProductDistNumber(new List<string> { itemCode.ProductoId }, new List<string> { b.BatchNumber })).FirstOrDefault();
                batchDb ??= new Batches { ExpDate = DateTime.Now };
                var expDate = batchDb.ExpDate.HasValue ? batchDb.ExpDate.Value.ToString("dd/MM/yyyy") : string.Empty;

                var batch = new LineProductBatchesModel
                {
                    AvailableQuantity = b.BatchQty,
                    Batch = b.BatchNumber,
                    ExpDate = expDate,
                };

                batches.Add(batch);
            }

            return batches;
        }

        /// <summary>
        /// Gets the orders from user Orders.
        /// </summary>
        /// <returns>the user orders.</returns>
        private async Task<List<UserOrderModel>> GetUserOrders(string route, List<int> listIds = null)
        {
            if (listIds != null)
            {
                var response = await this.pedidosService.GetUserPedidos(listIds, route);
                return JsonConvert.DeserializeObject<List<UserOrderModel>>(response.Response.ToString());
            }

            var pedidosResponse = await this.pedidosService.GetUserPedidos(route);
            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the line products.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetLineProducts(string route, object dataToSend = null)
        {
            if (dataToSend != null)
            {
                var response = await this.almacenService.PostAlmacenOrders(route, dataToSend);
                return JsonConvert.DeserializeObject<List<LineProductsModel>>(response.Response.ToString());
            }

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
                    Comments = invoice.Comments,
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
            delivery.DistinctBy(x => x.DeliveryId).ToList()
                .ForEach(y =>
                {
                    var userOrderStatus = userOrderModels.Where(z => z.Salesorderid == y.BaseEntry.ToString() && !string.IsNullOrEmpty(z.Productionorderid)).Select(y => y.StatusAlmacen).ToList();
                    userOrderStatus.AddRange(lineProducts.Where(x => x.SaleOrderId == y.BaseEntry && !string.IsNullOrEmpty(x.ItemCode)).Select(y => y.StatusAlmacen));

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
        private async Task<List<InvoiceProductModel>> GetProductModels(List<InvoiceDetailModel> invoices, List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders, List<IncidentsModel> incidents)
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

                var product = this.GetProductStatus(deliveryDetails, userOrders, lineProducts, orders, invoice);

                var incidentdb = incidents.FirstOrDefault(x => x.SaleOrderId == product.Item3 && x.ItemCode == item.ProductoId);
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = !string.IsNullOrEmpty(incidentdb.Batches) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(incidentdb.Batches) : new List<AlmacenBatchModel>(),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                var productModel = new InvoiceProductModel
                {
                    Batches = listBatches,
                    Container = invoice.Container,
                    Description = item.LargeDescription.ToUpper(),
                    ItemCode = item.IsMagistral.Equals("Y") ? $"{item.ProductoId}- {product.Item2}" : item.ProductoId,
                    NeedsCooling = item.NeedsCooling.Equals("Y"),
                    ProductType = $"Producto {productType}",
                    Quantity = invoice.Quantity,
                    Status = product.Item1,
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    DeliveryId = invoice.BaseEntry.Value,
                    OrderId = product.Item2,
                    SaleOrderId = product.Item3,
                    Incident = string.IsNullOrEmpty(localIncident.Status) ? null : localIncident,
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
        private Tuple<string, int, int> GetProductStatus(List<DeliveryDetailModel> deliveries, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders, InvoiceDetailModel invoice)
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
                order = new OrdenFabricacionModel { OrdenId = 0 };
            }

            return new Tuple<string, int, int>(status, order.OrdenId, saleId);
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

        private async Task<DeliveryScannedModel> GetDeliveryScannedData(InvoiceHeaderModel invoice, List<DeliveryDetailModel> deliveries, List<InvoiceDetailModel> invoices, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders)
        {
            var delivery = deliveries.FirstOrDefault();
            delivery = delivery == null ? new DeliveryDetailModel() : delivery;

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { delivery.BaseEntry });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            invoices = invoices.Where(x => x.BaseEntry.HasValue && x.BaseEntry.Value == delivery.DeliveryId).ToList();
            var products = await this.GetProductModels(invoices, deliveries, userOrders, lineProducts, orders, incidents);

            var deliveryData = new DeliveryScannedModel
            {
                Client = invoice.Cliente,
                DeliveryId = delivery.DeliveryId,
                InvoiceId = invoice.DocNum,
                SaleOrder = delivery.BaseEntry,
                TotalPieces = invoices.Sum(x => x.Quantity),
                Products = products,
                TotalItems = products.Count,
                Status = products.Any() && products.All(x => x.Status.Equals(ServiceConstants.Empaquetado)) ? ServiceConstants.Empaquetado : ServiceConstants.Almacenado,
            };

            return deliveryData;
        }

        /// <summary>
        /// Gets the firs value for the deliveryDetailModel, the invoie detail by item code and the invoice header.
        /// </summary>
        /// <param name="saleOrder">the sale order.</param>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<InvoiceDetailModel, InvoiceHeaderModel>> GetSaleOrderInvoiceDataByItemCode(int saleOrder, string itemCode)
        {
            var deliveryDetails = (await this.sapDao.GetDeliveryBySaleOrder(new List<int> { saleOrder })).FirstOrDefault(x => x.InvoiceId.HasValue);
            deliveryDetails ??= new DeliveryDetailModel { InvoiceId = 0 };

            var header = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { deliveryDetails.InvoiceId.Value })).FirstOrDefault();
            header ??= new InvoiceHeaderModel();

            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(new List<int> { header.InvoiceId })).FirstOrDefault(x => x.BaseEntry.HasValue && x.ProductoId == itemCode && deliveryDetails.DeliveryId == x.BaseEntry.Value);
            invoiceDetails ??= new InvoiceDetailModel { BaseEntry = 0 };

            return new Tuple<InvoiceDetailModel, InvoiceHeaderModel>(invoiceDetails, header);
        }
    }
}
