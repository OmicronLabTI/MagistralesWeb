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
    using System.Text;
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
            userOrders = userOrders.Where(x => string.IsNullOrEmpty(x.StatusInvoice)).ToList();
            lineProducts = lineProducts.Where(x => string.IsNullOrEmpty(x.StatusInvoice)).ToList();

            var listIds = userOrders.Select(y => y.DeliveryId).ToList();
            listIds.AddRange(lineProducts.Select(y => y.DeliveryId));
            listIds = listIds.Distinct().ToList();

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(listIds)).ToList();
            var invoicesId = deliveryDetails.Where(y => y.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();

            var invoiceHeaders = (await this.sapDao.GetInvoiceHeaderByInvoiceIdJoinDoctor(invoicesId)).Where(x => x.InvoiceStatus != "C").ToList();
            invoiceHeaders = invoiceHeaders.Where(x => string.IsNullOrEmpty(x.Refactura) || x.Refactura != ServiceConstants.IsRefactura).ToList();
            var granTotal = invoiceHeaders.DistinctBy(x => x.InvoiceId).ToList().Count;
            invoiceHeaders = this.GetInvoiceHeaderByParameters(invoiceHeaders, deliveryDetails, parameters);
            var totalByFilters = invoiceHeaders.DistinctBy(x => x.InvoiceId).ToList().Count;
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(invoiceHeaders.Select(x => x.InvoiceId).ToList())).ToList();

            var remisionTotal = invoiceDetails.Where(y => y.BaseEntry.HasValue && y.BaseEntry.Value != 0).Select(x => x.BaseEntry.Value).Distinct().Count();

            var idsToLook = this.GetInvoicesToLook(parameters, invoiceHeaders);
            invoiceHeaders = invoiceHeaders.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();
            invoiceHeaders = invoiceHeaders.OrderByDescending(x => x.InvoiceId).ToList();
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
            dataToReturn.TotalInvioces = totalByFilters;
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{remisionTotal}-{granTotal}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceProducts(int invoiceId)
        {
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserOrderInvoice);
            var lineProducts = await this.GetLineProducts(ServiceConstants.GetLinesForInvoice);

            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNum(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(new List<int> { invoiceHeader.InvoiceId })).ToList();
            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntry(invoiceDetails.Select(x => x.BaseEntry.Value).ToList())).ToList();
            var fabOrders = (await this.sapDao.GetFabOrderBySalesOrderId(deliveryDetails.Select(x => x.BaseEntry).ToList())).ToList();

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, deliveryDetails.Select(x => x.BaseEntry).ToList());
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var products = await this.GetProductModels(invoiceDetails, deliveryDetails, userOrders, lineProducts, fabOrders, incidents);
            return ServiceUtils.CreateResult(true, 200, null, products, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryScannedData(string code)
        {
            var deliveryId = int.Parse(code);

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntry(new List<int> { deliveryId })).ToList();
            var listSaleOrder = deliveryDetails.Select(x => x.BaseEntry).Distinct().ToList();
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserSalesOrder, listSaleOrder);
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, listSaleOrder);

            var invoiceId = deliveryDetails.FirstOrDefault() == null || !deliveryDetails.FirstOrDefault().InvoiceId.HasValue ? 0 : deliveryDetails.FirstOrDefault().InvoiceId.Value;

            var invoiceHeader = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader = invoiceHeader == null ? new InvoiceHeaderModel() : invoiceHeader;
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(new List<int> { invoiceHeader.InvoiceId })).ToList();
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

            var details = (await this.sapDao.GetAllDetails(new List<int?> { saleOrder })).ToList();
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
                NeedsCooling = itemCode.NeedsCooling,
            };

            return ServiceUtils.CreateResult(true, 200, null, lineData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceHeader(InvoicePackageSapLookModel dataToLook)
        {
            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNumJoinDoctor(dataToLook.InvoiceDocNums)).ToList();
            invoiceHeader = dataToLook.Type.Equals(ServiceConstants.Local.ToLower()) ? invoiceHeader.Where(x => x.Address.Contains(ServiceConstants.NuevoLeon) || dataToLook.ExclusivePartnersIds.Any(y => y == x.CardCode)).ToList() : invoiceHeader.Where(x => !x.Address.Contains(ServiceConstants.NuevoLeon) && !dataToLook.ExclusivePartnersIds.Any(y => y == x.CardCode)).ToList();

            var dictParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(dataToLook.Chip))
            {
                dictParams.Add(ServiceConstants.Chips, dataToLook.Chip);
            }

            invoiceHeader = this.GetInvoiceHeaderByParameters(invoiceHeader, new List<DeliveryDetailModel>(), dictParams);
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

            var invoicesDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(invoiceHeaderOrdered.Select(x => x.InvoiceId).ToList())).ToList();

            var invoicesNull = invoicesDetails.Select(x => x.InvoiceId).Cast<int?>().ToList();
            var deliveries = (await this.sapDao.GetDeliveryByInvoiceId(invoicesNull)).ToList();

            var deliveryCompanies = (await this.sapDao.GetDeliveryCompanyById(invoiceHeaderOrdered.Select(x => x.TransportCode).ToList())).ToList();
            var clients = (await this.sapDao.GetClientsById(invoiceHeaderOrdered.Select(x => x.CardCode).ToList())).ToList();
            var salesPerson = (await this.sapDao.GetAsesorWithEmailByIdsFromTheAsesor(invoiceHeaderOrdered.Select(x => x.SalesPrsonId).ToList())).ToList();
            invoiceHeaderOrdered.ForEach(x =>
            {
                var details = invoicesDetails.Where(y => y.InvoiceId == x.InvoiceId).ToList();
                var client = clients.FirstOrDefault(y => y.ClientId == x.CardCode);
                client ??= new ClientCatalogModel();

                var salePerson = salesPerson.FirstOrDefault(y => y.AsesorId == x.SalesPrsonId);
                salePerson ??= new SalesPersonModel();

                var company = deliveryCompanies.FirstOrDefault(y => y.TrnspCode == x.TransportCode);
                company ??= new Repartidores { TrnspName = string.Empty };

                var saleOrders = deliveries.Where(y => y.InvoiceId.HasValue && y.InvoiceId == x.InvoiceId).ToList();

                x.Comments = $"{details.Where(y => y.BaseEntry.HasValue).DistinctBy(x => x.BaseEntry.Value).Count()}-{details.Count}";
                x.ClientEmail = client.Email;
                x.TransportName = company.TrnspName;

                //// ToDo descomentar linea siguiente si hay deploy magis a prod antes que dxp
                //// x.SaleOrder = JsonConvert.SerializeObject(saleOrders.Select(y => y.PedidoId).Distinct().ToList());
                x.SaleOrder = JsonConvert.SerializeObject(saleOrders.Select(y => y.PedidoDxpId?.ToUpper()).Distinct().ToList());
                x.TotalSaleOrder = saleOrders.Count;
                x.SalesPrsonEmail = string.IsNullOrEmpty(salePerson.Email) ? string.Empty : salePerson.Email;
                x.SalesPrsonName = string.IsNullOrEmpty(salePerson.FirstName) ? string.Empty : salePerson.FirstName + ' ' + salePerson.LastName;
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

            var clientesResponse = await this.almacenService.GetAlmacenOrders(ServiceConstants.SpecialClients);
            var clients = JsonConvert.DeserializeObject<List<ExclusivePartnersModel>>(clientesResponse.Response.ToString());

            var status = !packages.Any() ? ServiceConstants.Empaquetado : packages.OrderByDescending(x => x.AssignedDate.Value).FirstOrDefault().Status;

            var model = new InvoiceDeliverModel
            {
                Address = invoiceHeader.Address,
                Client = invoiceHeader.Cliente,
                Comments = invoiceHeader.CommentsInvoice,
                Doctor = invoiceHeader.Medico,
                PackageNumber = invoiceHeader.DocNum,
                Status = status,
            };

            var comments = model.Address.Contains(ServiceConstants.NuevoLeon) || clients.Any(x => x.CodeSN == invoiceHeader.CardCode) ? string.Empty : ServiceConstants.ForeingPackage;
            comments = !status.Equals(ServiceConstants.Empaquetado) && !status.Equals(ServiceConstants.NoEntregado) ? $"{ServiceConstants.PackageNotAvailable} {status}" : comments;

            return ServiceUtils.CreateResult(true, 200, null, model, null, comments);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetSapIds(List<int> salesIds)
        {
            var listToReturn = new List<SapIdsModel>();

            var deliveries = (await this.sapDao.GetDeliveryDetailBySaleOrder(salesIds)).ToList();
            var invoicesIdToLook = deliveries.Where(x => x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList();
            var invoices = (await this.sapDao.GetInvoiceHeaderByInvoiceId(invoicesIdToLook)).ToList();

            salesIds.ForEach(x =>
            {
                deliveries.Where(y => y.BaseEntry == x).ToList().ForEach(d =>
                {
                    var invoiceId = d.InvoiceId.HasValue ? d.InvoiceId.Value : 0;
                    var localInvoice = invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
                    localInvoice ??= new InvoiceHeaderModel();

                    var model = new SapIdsModel
                    {
                        DeliveryId = d.DeliveryId,
                        InvoiceId = localInvoice.DocNum,
                        Itemcode = d.ProductoId,
                        SaleOrderId = x,
                    };

                    listToReturn.Add(model);
                });
            });

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetCancelledInvoices(int days)
        {
            var dateToLook = DateTime.Today.AddDays(-days);
            var invoices = (await this.sapDao.GetInvoiceByDocDate(dateToLook)).ToList();
            invoices = invoices.Where(x => x.Canceled == "Y").ToList();
            return ServiceUtils.CreateResult(true, 200, null, invoices, null, null);
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
            var invoiceHeadersIds = invoiceHeaders.OrderByDescending(x => x.InvoiceId).Select(y => y.InvoiceId).ToList();
            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            return invoiceHeadersIds.Skip(offsetNumber).Take(limitNumber).ToList();
        }

        /// <summary>
        /// gets the invoices by criteria.
        /// </summary>
        /// <param name="invoices">the invoices.</param>
        /// <param name="deliveryDetails">the deliverys.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<InvoiceHeaderModel> GetInvoiceHeaderByParameters(List<InvoiceHeaderModel> invoices, List<DeliveryDetailModel> deliveryDetails, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return invoices;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int invoice))
            {
                int.TryParse(parameters[ServiceConstants.Chips].Replace(ServiceConstants.RemisionChip, string.Empty), out int remision);
                var details = deliveryDetails.Where(x => x.DeliveryId == remision && x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList();
                var invoiceById = invoices.Where(x => details.Contains(x.InvoiceId)).ToList();
                invoiceById.AddRange(invoices.Where(x => x.DocNum == invoice));
                return invoiceById.DistinctBy(x => x.InvoiceId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return invoices.Where(x => listNames.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
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
                var totalProducts = invoiceDetails.Count;

                var invoiceModel = new InvoiceSaleModel
                {
                    Doctor = doctor,
                    Invoice = invoice.DocNum,
                    Deliveries = 0,
                    Products = totalProducts,
                    InvoiceDocDate = invoice.FechaInicio,
                    TypeOrder = invoice.TypeOrder,
                };

                var destiny = invoice.Address.Split(",");

                var invoiceHeader = new InvoiceSaleHeaderModel
                {
                    Address = invoice.Address.Replace("\r", string.Empty).ToUpper(),
                    Client = invoice.Cliente,
                    Doctor = doctor,
                    Invoice = invoice.DocNum,
                    DocEntry = invoice.InvoiceId,
                    InvoiceDocDate = invoice.FechaInicio,
                    ProductType = destiny.Count() < 3 || destiny[destiny.Count() - 3].Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                    TotalDeliveries = 0,
                    TotalProducts = totalProducts,
                    Comments = invoice.Comments,
                    TypeOrder = invoice.TypeOrder,
                    CodeClient = invoice.CardCode,
                    TotalPieces = invoiceDetails.Where(y => y.Quantity > 0).Sum(x => (int)x.Quantity),
                };

                var invoiceModelToAdd = new InvoicesModel
                {
                    Deliveries = this.GetDeliveryModel(deliveryDetails, invoiceDetails, userOrders, lineProducts),
                    InvoiceHeader = invoiceHeader,
                    InvoiceSale = invoiceModel,
                };

                invoiceModelToAdd.InvoiceHeader.TotalDeliveries = invoiceModelToAdd.Deliveries.Count;
                invoiceModelToAdd.InvoiceSale.Deliveries = invoiceModelToAdd.Deliveries.Count;

                listToReturn.TotalDeliveries += invoiceModelToAdd.Deliveries.Count;
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
                    var userOrderStatus = userOrderModels.Where(z => z.DeliveryId == y.DeliveryId && !string.IsNullOrEmpty(z.Productionorderid)).Select(y => y.StatusAlmacen).ToList();
                    userOrderStatus.AddRange(lineProducts.Where(x => x.DeliveryId == y.DeliveryId && !string.IsNullOrEmpty(x.ItemCode)).Select(y => y.StatusAlmacen));
                    var salesOrders = delivery.Where(z => z.DeliveryId == y.DeliveryId).Select(a => a.BaseEntry).ToList();
                    var deliveryModel = new InvoiceDeliveryModel
                    {
                        DeliveryId = y.DeliveryId,
                        DeliveryDocDate = y.DocDate,
                        SaleOrder = salesOrders.Distinct().Count(),
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
            var usedDeliveries = new List<DeliveryDetailModel>();
            var items = (await this.sapDao.GetProductByIds(invoices.Select(x => x.ProductoId).ToList())).ToList();
            foreach (var invoice in invoices)
            {
                var listBatches = new List<string>();
                var item = items.FirstOrDefault(x => x.ProductoId == invoice.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var localDeliverDetails = deliveryDetails.Where(x => !usedDeliveries.Any(y => y.ProductoId == x.ProductoId && x.BaseEntry == y.BaseEntry)).ToList();
                var deliveriesDetail = localDeliverDetails.FirstOrDefault(x => x.DeliveryId == invoice.BaseEntry.Value && x.ProductoId == invoice.ProductoId);
                var saleId = deliveriesDetail == null ? 0 : deliveriesDetail.BaseEntry;
                var prodId = deliveriesDetail == null ? string.Empty : deliveriesDetail.ProductoId;

                usedDeliveries.Add(new DeliveryDetailModel { BaseEntry = saleId, ProductoId = prodId });

                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                if (item.IsMagistral.Equals("N"))
                {
                    var lineProduct = lineProducts.FirstOrDefault(x => x.SaleOrderId == saleId && x.ItemCode == invoice.ProductoId);
                    lineProduct ??= new LineProductsModel();
                    var batchName = string.IsNullOrEmpty(lineProduct.BatchName) ? new List<AlmacenBatchModel>() : JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineProduct.BatchName);

                    listBatches = await this.GetBatchesByDelivery(invoice.BaseEntry.Value, invoice.ProductoId, ServiceConstants.PT, batchName);
                }

                var product = this.GetProductStatus(deliveryDetails, userOrders, lineProducts, orders, invoice, saleId);

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
                    ItemCode = item.IsMagistral.Equals("Y") ? $"{item.ProductoId} - {product.Item2}" : item.ProductoId,
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
        /// <param name="saleIdLook">The sale id.</param>
        /// <returns>the data.</returns>
        private Tuple<string, int, int> GetProductStatus(List<DeliveryDetailModel> deliveries, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders, InvoiceDetailModel invoice, int saleIdLook)
        {
            var status = ServiceConstants.Almacenado;
            var deliveriesDetail = deliveries.FirstOrDefault(x => x.DeliveryId == invoice.BaseEntry.Value && invoice.ProductoId == x.ProductoId && x.BaseEntry == saleIdLook);
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
        private async Task<List<string>> GetBatchesByDelivery(int delivery, string itemCode, string warehouse, List<AlmacenBatchModel> batchName)
        {
            var batchTransacion = await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, delivery);
            var lastBatch = batchTransacion == null || !batchTransacion.Any() ? 0 : batchTransacion.Last().LogEntry;
            var batchTrans = (await this.sapDao.GetBatchTransationsQtyByLogEntry(new List<int> { lastBatch })).ToList();
            var listComponents = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { ProductId = itemCode, Warehouse = warehouse },
            };

            var validBatches = (await this.sapDao.GetValidBatches(listComponents)).ToList();

            var listToReturn = new List<string>();
            validBatches.Where(x => batchTrans.Any(y => y.SysNumber == x.SysNumber)).ToList().ForEach(z =>
            {
                var batch = batchName.FirstOrDefault(a => a.BatchNumber == z.DistNumber);
                batch ??= new AlmacenBatchModel() { BatchQty = 0 };
                listToReturn.Add($"{z.DistNumber} | {(int)batch.BatchQty} pz | Cad: {z.FechaExp}");
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

            var listSales = products.Select(x => x.SaleOrderId).Distinct().ToList();

            var deliveryData = new DeliveryScannedModel
            {
                Client = invoice.Cliente,
                DeliveryId = delivery.DeliveryId,
                InvoiceId = invoice.DocNum,
                SaleOrder = listSales.Count,
                TotalPieces = invoices.Sum(x => x.Quantity),
                Products = products,
                TotalItems = products.Count,
                Status = products.Any() && products.All(x => x.Status.Equals(ServiceConstants.Empaquetado)) ? ServiceConstants.Empaquetado : ServiceConstants.Almacenado,
                ListSalesOrder = string.Join(", ", listSales),
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
            var deliveryDetailsList = (await this.sapDao.GetDeliveryDetailBySaleOrder(new List<int> { saleOrder })).ToList();
            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(deliveryDetailsList.Select(x => x.DeliveryId).ToList())).Where(x => x.Canceled != "Y").ToList();
            var deliveryDetails = deliveryDetailsList.FirstOrDefault(x => deliveryHeaders.Any(y => y.DocNum == x.DeliveryId) && x.ProductoId == itemCode && x.InvoiceId.HasValue);
            deliveryDetails ??= new DeliveryDetailModel { InvoiceId = 0 };

            var header = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { deliveryDetails.InvoiceId.Value })).FirstOrDefault();
            header ??= new InvoiceHeaderModel();

            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(new List<int> { header.InvoiceId })).FirstOrDefault(x => x.BaseEntry.HasValue && x.ProductoId == itemCode && deliveryDetails.DeliveryId == x.BaseEntry.Value);
            invoiceDetails ??= new InvoiceDetailModel { BaseEntry = 0 };

            return new Tuple<InvoiceDetailModel, InvoiceHeaderModel>(invoiceDetails, header);
        }
    }
}
