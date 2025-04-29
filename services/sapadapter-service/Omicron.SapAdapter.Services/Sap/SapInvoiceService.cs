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
    using System.Linq.Expressions;
    using System.Numerics;
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
    using StackExchange.Redis;

    /// <summary>
    /// Class for the invoces.
    /// </summary>
    public class SapInvoiceService : ISapInvoiceService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        private readonly IProccessPayments proccessPayments;

        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapInvoiceService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">The redis service.</param>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="doctorService">The doctor service.</param>
        public SapInvoiceService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments, IDoctorService doctorService)
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
        public async Task<ResultModel> GetInvoice(Dictionary<string, string> parameters)
        {
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserOrderInvoice);
            var lineProducts = await this.GetLineProducts(ServiceConstants.GetLinesForInvoice);

            var listIds = userOrders.Select(y => y.DeliveryId).ToList();
            listIds.AddRange(lineProducts.Select(y => y.DeliveryId));
            listIds = listIds.Distinct().ToList();

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(listIds)).ToList();
            var invoicesId = deliveryDetails.Where(y => y.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();

            var invoiceHeaders = (await this.sapDao.GetInvoiceHeaderByInvoiceIdJoinDoctor(invoicesId)).ToList();
            invoiceHeaders = invoiceHeaders.Where(x => ServiceShared.CalculateAnd(ServiceShared.CalculateOr(string.IsNullOrEmpty(x.Refactura), x.Refactura != ServiceConstants.IsRefactura), x.Canceled == "N")).ToList();
            invoiceHeaders = await this.GetInvoiceHeaderByParameters(invoiceHeaders, deliveryDetails, parameters, null);
            var totalByFilters = invoiceHeaders.UtilsDistinctBy(x => x.InvoiceId).ToList().Count;
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(invoiceHeaders.Select(x => x.InvoiceId).ToList())).ToList();

            var remisionTotal = invoiceDetails.Where(y => y.BaseEntry.HasValue && y.BaseEntry.Value != 0).Select(x => x.BaseEntry.Value).Distinct().Count();

            var idsToLook = this.GetInvoicesToLook(parameters, invoiceHeaders);
            invoiceHeaders = invoiceHeaders.Where(x => idsToLook.Contains(x.InvoiceId)).OrderByDescending(x => x.InvoiceId).ToList();
            invoiceDetails = invoiceDetails.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();

            var retrieveMode = new RetrieveInvoiceModel
            {
                DeliveryDetailModel = deliveryDetails,
                InvoiceDetails = invoiceDetails,
                InvoiceHeader = invoiceHeaders,
                LineProducts = lineProducts,
                UserOrders = userOrders,
                LocalNeigbors = new List<string>(),
            };

            var dataToReturn = this.GetInvoiceToReturn(retrieveMode);
            dataToReturn.TotalInvioces = totalByFilters;
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{remisionTotal}-{totalByFilters}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceByFilters(Dictionary<string, string> parameters)
        {
            var invoiceHeaders = await this.GetInvoicesByInitialFilters(parameters);

            var totalByFilters = invoiceHeaders.UtilsDistinctBy(x => x.InvoiceId).ToList().Count;
            var invoiceDetails = await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(invoiceHeaders.Select(x => x.InvoiceId).ToList());

            var idsToLook = this.GetInvoicesToLook(parameters, invoiceHeaders.ToList());
            invoiceHeaders = invoiceHeaders.Where(x => idsToLook.Contains(x.InvoiceId)).OrderByDescending(x => x.InvoiceId);
            invoiceDetails = invoiceDetails.Where(x => idsToLook.Contains(x.InvoiceId)).ToList();
            var remisionTotal = invoiceDetails.Where(y => y.BaseEntry.HasValue && y.BaseEntry.Value != 0).Select(x => x.BaseEntry.Value).Distinct().Count();
            var dataToReturn = this.GetInvoiceToReturnForSearch(invoiceHeaders.ToList(), invoiceDetails.ToList());
            dataToReturn.TotalInvioces = totalByFilters;
            return ServiceUtils.CreateResult(true, 200, null, dataToReturn, null, $"{remisionTotal}-{totalByFilters}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceDetail(int invoice)
        {
            var invoiceDetails = (await this.sapDao.GetInvoiceHeaderDetailByInvoiceIdJoinDoctor(new List<int> { invoice })).ToList();
            var deliveryDetails = (await this.sapDao.GetDeliveryDetailForDeliveryById(invoiceDetails.Where(y => y.Detail.BaseEntry.HasValue).Select(x => (int)x.Detail.BaseEntry).Distinct().ToList())).ToList();
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);

            var salesOrdersId = deliveryDetails.Where(y => y.Detalles != null && y.Detalles.BaseEntry.HasValue).Select(x => x.Detalles.BaseEntry.Value).ToList();
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserSalesOrder, salesOrdersId.Distinct().ToList());
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, salesOrdersId.Distinct().ToList());

            var transactionsIds = invoiceDetails.Where(i => !string.IsNullOrEmpty(i.InvoiceHeader.DocNumDxp)).Select(o => o.InvoiceHeader.DocNumDxp).Distinct().ToList();
            var payment = (await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds)).GetPaymentBydocNumDxp(invoiceDetails.First().InvoiceHeader.DocNumDxp);

            var invoiceHeader = invoiceDetails.FirstOrDefault();

            var addressesToFind = new List<GetDoctorAddressModel> { new GetDoctorAddressModel { CardCode = invoiceHeader.InvoiceHeader.CardCode, AddressId = invoiceHeader.InvoiceHeader.ShippingAddressName } };
            var doctorData = (await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, addressesToFind)).FirstOrDefault(x => x.AddressId == invoiceHeader.InvoiceHeader.ShippingAddressName);
            doctorData ??= new DoctorDeliveryAddressModel { Contact = invoiceHeader.Medico };

            var invoiceToReturn = new InvoiceSaleHeaderModel
            {
                Address = ServiceShared.CalculateTernary(payment.ShippingCostAccepted == ServiceConstants.ShippingCostAccepted, invoiceHeader.InvoiceHeader.Address.Replace("\r", string.Empty).ToUpper(), ServiceConstants.OnSiteDelivery.ToUpper()),
                Client = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctorData.Contact), invoiceHeader.InvoiceHeader.Medico, doctorData.Contact),
                Doctor = invoiceHeader.Medico ?? string.Empty,
                Invoice = invoiceHeader.InvoiceHeader.DocNum,
                DocEntry = invoiceHeader.InvoiceHeader.InvoiceId,
                InvoiceDocDate = invoiceHeader.InvoiceHeader.FechaInicio,
                ProductType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeigbors, invoiceHeader.InvoiceHeader.Address, payment),
                TotalDeliveries = invoiceDetails.Select(x => x.Detail.BaseEntry).Distinct().Count(),
                TotalProducts = invoiceDetails.Count,
                Comments = invoiceHeader.InvoiceHeader.Comments,
                TypeOrder = invoiceHeader.InvoiceHeader.TypeOrder,
                CodeClient = invoiceHeader.InvoiceHeader.CardCode,
                TotalPieces = invoiceDetails.Where(y => y.Detail.Quantity > 0).Sum(x => (int)x.Detail.Quantity),
                IsPackage = invoiceHeader.InvoiceHeader.IsPackage == ServiceConstants.IsPackage,
                IsDeliveredInOffice = invoiceHeader.InvoiceHeader.IsDeliveredInOffice ?? "N",
            };

            var invoiceModelToAdd = new InvoicesModel
            {
                Deliveries = this.GetDeliveryModel(deliveryDetails, invoiceDetails, userOrders, lineOrders),
                InvoiceHeader = invoiceToReturn,
                InvoiceSale = null,
            };
            return ServiceUtils.CreateResult(true, 200, null, invoiceModelToAdd, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceProducts(int invoiceId, string type, List<int> deliveriesIds)
        {
            Dictionary<string, string> routes = ServiceShared.CalculateTernary(
                type == ServiceConstants.Empaquetado,
                new Dictionary<string, string>
                {
                    { "user-order", ServiceConstants.GetUserOrderByDeliveryOrder },
                    { "line-products", ServiceConstants.GetLinesForDeliveryId },
                    { "data", string.Join('|', deliveriesIds) },
                },
                new Dictionary<string, string>
                {
                    { "user-order", ServiceConstants.GetUserOrdersByInvoicesIds },
                    { "line-products", ServiceConstants.GetLineOrdersByInvoice },
                    { "data", invoiceId.ToString() },
                });

            var data = routes["data"].Split('|').Select(deliveryId => int.Parse(deliveryId));

            var userOrdersResponse = await this.pedidosService.PostPedidos(data, routes["user-order"]);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var lineProductsResponse = await this.almacenService.PostAlmacenOrders(routes["line-products"], data);
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNum(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(new List<int> { invoiceHeader.InvoiceId })).ToList();
            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntry(invoiceDetails.Select(x => x.BaseEntry.Value).ToList())).ToList();
            var ordersIdToLookFor = deliveryDetails.Where(x => x.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).Distinct().ToList();
            var fabOrders = (await this.sapDao.GetFabOrderBySalesOrderId(ordersIdToLookFor)).ToList();

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, ordersIdToLookFor);
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var products = await this.GetProductModels(invoiceDetails, deliveryDetails, userOrders, lineProducts, fabOrders, incidents);
            return ServiceUtils.CreateResult(true, 200, null, products, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryScannedData(string code)
        {
            var deliveryId = int.Parse(code);

            var deliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntry(new List<int> { deliveryId })).ToList();
            var listSaleOrder = deliveryDetails.Where(y => y.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).Distinct().ToList();
            var userOrders = await this.GetUserOrders(ServiceConstants.GetUserSalesOrder, listSaleOrder);
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, listSaleOrder);

            var invoiceId = deliveryDetails.FirstOrDefault()?.InvoiceId ?? 0;

            var invoiceHeader = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { invoiceId })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();
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
            order ??= new CompleteDetailOrderModel();

            var itemCode = (await this.sapDao.GetProductById(order.CodigoProducto)).FirstOrDefault();
            var productType = ServiceShared.CalculateTernary(itemCode.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

            var sapData = await this.GetSaleOrderInvoiceDataByItemCode(saleOrder, itemCode.ProductoId);

            var product = new MagistralScannerModel
            {
                Container = order.Container,
                Description = itemCode.LargeDescription,
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
            var deliveryOrder = int.Parse(dataArray[2]);

            var itemCode = (await this.sapDao.GetProductByCodeBar(codeBar)).FirstOrDefault();

            if (itemCode == null)
            {
                return ServiceUtils.CreateResult(true, 404, null, new LineScannerModel(), null, null);
            }

            var productType = ServiceShared.CalculateTernary(itemCode.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);
            var sapData = await this.GetSaleOrderInvoiceDataByItemCode(saleOrder, itemCode.ProductoId, deliveryOrder);
            var lineOrders = await this.GetLineProducts(ServiceConstants.GetLinesBySaleOrder, new List<int> { saleOrder });
            var lineProduct = lineOrders.FirstOrDefault(x => x.ItemCode == itemCode.ProductoId && x.DeliveryId == deliveryOrder);
            lineProduct ??= new LineProductsModel { BatchName = JsonConvert.SerializeObject(new List<AlmacenBatchModel>()) };
            var batchModel = JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineProduct.BatchName);
            var batches = await this.GetBatchesForInvoice(itemCode, batchModel);

            var lineData = new LineScannerModel
            {
                Batches = batches,
                Description = itemCode.LargeDescription,
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
            var localNeighbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var transactionsIds = invoiceHeader.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
            var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
            invoiceHeader = ServiceShared.CalculateTernary(
                dataToLook.Type.Equals(ServiceConstants.Local.ToLower()),
                invoiceHeader.Where(x => ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, x.Address, payments.GetPaymentBydocNumDxp(x.DocNumDxp)) || dataToLook.ExclusivePartnersIds.Any(y => y == x.CardCode)).ToList(),
                invoiceHeader.Where(x => !ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, x.Address, payments.GetPaymentBydocNumDxp(x.DocNumDxp)) && !dataToLook.ExclusivePartnersIds.Any(y => y == x.CardCode)).ToList());

            var dictParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(dataToLook.Chip))
            {
                dictParams.Add(ServiceConstants.Chips, dataToLook.Chip);
            }

            invoiceHeader = await this.GetInvoiceHeaderByParameters(invoiceHeader, new List<DeliveryDetailModel>(), dictParams, localNeighbors);
            var total = invoiceHeader.Count;
            var invoiceHeaderOrdered = from y in dataToLook.InvoiceDocNums
                                       let invoiceDb = invoiceHeader.FirstOrDefault(a => a.DocNum == y)
                                       where invoiceDb != null
                                       select invoiceDb;

            invoiceHeaderOrdered = invoiceHeaderOrdered.Skip(dataToLook.Offset).Take(dataToLook.Limit);

            var invoicesDetails = await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(invoiceHeaderOrdered.Select(x => x.InvoiceId).ToList());
            var deliveries = await this.sapDao.GetDeliveryByInvoiceId(invoicesDetails.Select(x => x.InvoiceId).Cast<int?>().ToList());

            var deliveryCompanies = await this.sapDao.GetDeliveryCompanyById(invoiceHeaderOrdered.Select(x => x.TransportCode).ToList());
            var salesPerson = await this.sapDao.GetAsesorWithEmailByIdsFromTheAsesor(invoiceHeaderOrdered.Select(x => x.SalesPrsonId).ToList());

            var doctorData = await ServiceUtils.GetDoctorDeliveryAddressData(
                this.doctorService,
                invoiceHeaderOrdered.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.ShippingAddressName }).ToList());

            var doctorPrescription = await ServiceShared.GetDoctors(this.doctorService, invoiceHeaderOrdered.Select(x => x.CardCode).Distinct().ToList());

            invoiceHeaderOrdered.ToList().ForEach(x =>
            {
                var details = invoicesDetails.Where(y => y.InvoiceId == x.InvoiceId);
                var salePerson = salesPerson.FirstOrDefault(y => y.AsesorId == x.SalesPrsonId);
                salePerson ??= new SalesPersonModel();

                var company = deliveryCompanies.FirstOrDefault(y => y.TrnspCode == x.TransportCode);
                company ??= new Repartidores { TrnspName = string.Empty };

                var payment = payments.GetPaymentBydocNumDxp(x.DocNumDxp);
                var saleOrders = deliveries.Where(y => y.InvoiceId.HasValue && y.InvoiceId == x.InvoiceId);

                var doctor = doctorData.FirstOrDefault(y => y.DoctorId == x.CardCode && y.AddressId == x.ShippingAddressName);
                doctor ??= new DoctorDeliveryAddressModel { Contact = x.Medico, BetweenStreets = string.Empty, EtablishmentName = string.Empty, References = string.Empty, AddressType = string.Empty };

                var prescriptionData = doctorPrescription.FirstOrDefault(y => y.CardCode == x.CardCode);
                prescriptionData ??= new DoctorPrescriptionInfoModel { DoctorName = x.Medico };

                x.Comments = $"{details.Where(y => y.BaseEntry.HasValue).DistinctBy(x => x.BaseEntry.Value).Count()}-{details.ToList().Count}";
                x.TransportName = company.TrnspName;

                x.Cliente = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctor.Contact), x.Medico, doctor.Contact);
                x.SaleOrder = saleOrders.Any(y => !string.IsNullOrEmpty(y.PedidoDxpId)) ? JsonConvert.SerializeObject(saleOrders.Where(z => !string.IsNullOrEmpty(z.PedidoDxpId)).Select(y => y.PedidoDxpId.ToUpper()).Distinct().ToList()) : JsonConvert.SerializeObject(saleOrders.Select(y => y.PedidoId).Distinct().ToList());
                x.TotalSaleOrder = saleOrders.Select(y => y.PedidoId).Distinct().Count();
                x.SalesPrsonEmail = salePerson.Email.ValidateNull();
                x.SalesPrsonName = $"{salePerson.FirstName.ValidateNull()} {salePerson.LastName.ValidateNull()}".Trim();
                x.Address = ServiceShared.CalculateTernary(payment.ShippingCostAccepted == ServiceConstants.ShippingCostAccepted, x.Address, ServiceConstants.OnSiteDelivery);
                x.ResponsibleMedic = prescriptionData.DoctorName;
                x.BetweenStreets = doctor.BetweenStreets;
                x.EtablishmentName = doctor.EtablishmentName;
                x.References = doctor.References;
                x.DeliveryComments = payment.DeliveryComments;
                x.DeliverySuggestedTime = payment.DeliverySuggestedTime;
                x.IsDoctorDirection = ServiceUtils.GetAddressType(x.DocNumDxp, payment.IsDoctorDirection == 1, doctor.AddressType);
            });

            return ServiceUtils.CreateResult(true, 200, null, invoiceHeaderOrdered, null, total);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoiceData(string code)
        {
            int.TryParse(code, out var intDocNum);
            var invoiceHeader = (await this.sapDao.GetInvoiceHeadersByDocNumJoinDoctor(new List<int> { intDocNum })).FirstOrDefault();
            invoiceHeader ??= new InvoiceHeaderModel();

            var packagesResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetPackagesByInvoice, new List<int> { intDocNum });
            var packages = JsonConvert.DeserializeObject<List<PackageModel>>(packagesResponse.Response.ToString());

            var clientesResponse = await this.almacenService.GetAlmacenOrders(ServiceConstants.SpecialClients);
            var clients = JsonConvert.DeserializeObject<List<ExclusivePartnersModel>>(clientesResponse.Response.ToString());
            var localNeighbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);

            var status = !packages.Any() ? ServiceConstants.Empaquetado : packages.OrderByDescending(x => x.AssignedDate.Value).FirstOrDefault().Status;

            var pickupOffcieInt = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(invoiceHeader.IsDeliveredInOffice) && invoiceHeader.IsDeliveredInOffice == "Y", 0, 1);
            var dxpTransaction = ServiceShared.CalculateTernary(string.IsNullOrEmpty(invoiceHeader.DocNumDxp), string.Empty, invoiceHeader.DocNumDxp);
            var dxpTransactions = (await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, new List<string> { dxpTransaction })).FirstOrDefault(p => ServiceShared.ValidateShopTransaction(p.TransactionId, invoiceHeader.DocNumDxp));
            dxpTransactions ??= new PaymentsDto { ShippingCostAccepted = pickupOffcieInt, DeliveryComments = string.Empty, DeliverySuggestedTime = string.Empty };

            var addressesResponse = await this.doctorService.PostDoctors(new GetDoctorAddressModel { CardCode = invoiceHeader.CardCode, AddressId = invoiceHeader.ShippingAddressName }, ServiceConstants.GetDoctorAddress);
            var address = JsonConvert.DeserializeObject<List<DoctorAddressModel>>(addressesResponse.Response.ToString()).FirstOrDefault();
            address ??= new DoctorAddressModel();

            var salesPerson = (await this.sapDao.GetAsesorWithEmailByIdsFromTheAsesor(new List<int> { invoiceHeader.SalesPrsonId })).FirstOrDefault();
            salesPerson ??= new SalesPersonModel();

            var doctorPrescription = (await ServiceShared.GetDoctors(this.doctorService, new List<string> { invoiceHeader.CardCode })).FirstOrDefault();
            doctorPrescription ??= new DoctorPrescriptionInfoModel();

            var model = new InvoiceDeliverModel
            {
                Address = invoiceHeader.Address,
                Client = invoiceHeader.Cliente,
                Comments = invoiceHeader.CommentsInvoice,
                Doctor = invoiceHeader.Medico,
                PackageNumber = invoiceHeader.DocNum,
                Status = status,
                NeedsDelivery = dxpTransactions.ShippingCostAccepted == 1,
                BetweenStreets = address.BetweenStreets,
                References = address.References,
                Telephone = invoiceHeader.DoctorPhoneNumber,
                EstablishmentName = address.EtablishmentName,
                ResponsibleDoctor = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctorPrescription.DoctorName), invoiceHeader.Medico, doctorPrescription.DoctorName),
                DestinyEmail = invoiceHeader.ClientEmail,
                SalesPersonEmail = salesPerson.Email.ValidateNull(),
                SalesPrsonName = $"{salesPerson.FirstName.ValidateNull()} {salesPerson.LastName.ValidateNull()}".Trim(),
                SalesOrders = JsonConvert.SerializeObject(new List<string> { invoiceHeader.DocNumDxp }),
                Contact = address.Contact.ValidateNull(),
                DeliveryComments = dxpTransactions.DeliveryComments,
                DeliverySuggestedTime = dxpTransactions.DeliverySuggestedTime,
            };

            var isinvoiceLocal = ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, model.Address, dxpTransactions) || clients.Any(x => x.CodeSN == invoiceHeader.CardCode);

            var comments = ServiceShared.CalculateTernary(isinvoiceLocal, string.Empty, ServiceConstants.ForeingPackage);
            comments = ServiceShared.CalculateTernary(!status.Equals(ServiceConstants.Empaquetado) && !status.Equals(ServiceConstants.NoEntregado), $"{ServiceConstants.PackageNotAvailable} {status}", comments);

            return ServiceUtils.CreateResult(true, 200, null, model, null, comments);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetSapIds(List<int> salesIds)
        {
            var deliveries = (await this.sapDao.GetDeliveryDetailBySaleOrder(salesIds)).ToList();
            var invoicesIdToLook = deliveries.Where(x => x.InvoiceId.HasValue).Select(y => y.InvoiceId.Value).ToList();
            var invoices = (await this.sapDao.GetInvoiceHeaderByInvoiceId(invoicesIdToLook)).ToList();

            var listToReturn = salesIds.SelectMany(x =>
            {
                var models = deliveries.Where(y => y.BaseEntry == x).Select(d =>
                {
                    var invoiceId = d.InvoiceId ?? 0;
                    var localInvoice = invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
                    localInvoice ??= new InvoiceHeaderModel();

                    return new SapIdsModel
                    {
                        DeliveryId = d.DeliveryId,
                        InvoiceId = localInvoice.DocNum,
                        Itemcode = d.ProductoId,
                        SaleOrderId = x,
                    };
                }).ToList();
                return models;
            }).ToList();

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

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvoicesByIds(List<int> invoicesIds)
        {
            var invoices = (await this.sapDao.GetInvoiceHeadersByDocNumJoinDoctor(invoicesIds)).ToList();
            var salesPerson = (await this.sapDao.GetAsesorWithEmailByIdsFromTheAsesor(invoices.Select(x => x.SalesPrsonId).ToList())).ToList();

            var adressesToFind = invoices.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.ShippingAddressName }).ToList();
            var doctorData = await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, adressesToFind);

            invoices.ForEach(x =>
            {
                var adviosr = salesPerson.FirstOrDefault(s => s.AsesorId == x.SalesPrsonId);
                adviosr ??= new SalesPersonModel { Email = string.Empty, FirstName = string.Empty, LastName = string.Empty };
                x.SalesPrsonEmail = adviosr.Email;
                x.SalesPrsonName = $"{adviosr.FirstName} {adviosr.LastName}";

                var doctor = doctorData.FirstOrDefault(y => y.DoctorId == x.CardCode && y.AddressId == x.ShippingAddressName);
                doctor ??= new DoctorDeliveryAddressModel { Contact = x.Medico, BetweenStreets = string.Empty, EtablishmentName = string.Empty, References = string.Empty, AddressType = string.Empty };

                x.IsDoctorDirection = doctor.AddressType.Equals(ServiceConstants.DoctorAddressType);
            });

            return ServiceUtils.CreateResult(true, 200, null, invoices, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetClosedInvoicesByDocNum(List<int> docNums)
        {
            var invoicesHeader = await this.sapDao.GetClosedInvoicesByDocNum(docNums);
            return ServiceUtils.CreateResult(true, 200, null, invoicesHeader, null, null);
        }

        /// <summary>
        /// Gets the batches for the invoiceDocNum.
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

                batches.Add(new LineProductBatchesModel
                {
                    AvailableQuantity = b.BatchQty,
                    Batch = b.BatchNumber,
                    ExpDate = ServiceShared.GetDateValueOrDefault(batchDb.ExpDate, string.Empty),
                    WarehouseCode = b.WarehouseCode,
                });
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
                var response = await this.pedidosService.PostPedidos(listIds, route);
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
        /// <param name="invoiceHeaders">the invoiceDocNum headers.</param>
        /// <returns>the ids to look.</returns>
        private List<int> GetInvoicesToLook(Dictionary<string, string> parameters, List<InvoiceHeaderModel> invoiceHeaders)
        {
            var invoiceHeadersIds = invoiceHeaders.OrderByDescending(x => x.InvoiceId).Select(y => y.InvoiceId).ToList();
            var offset = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Offset, "0");
            var limit = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Limit, "1");

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
        /// <param name="localNeighborhoods">localNeighborhoods.</param>
        /// <returns>the data.</returns>
        private async Task<List<InvoiceHeaderModel>> GetInvoiceHeaderByParameters(
            List<InvoiceHeaderModel> invoices,
            List<DeliveryDetailModel> deliveryDetails,
            Dictionary<string, string> parameters,
            List<string> localNeighborhoods)
        {
            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var transactionsIds = invoices.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
                var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
                localNeighborhoods = localNeighborhoods ?? await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                invoices = invoices.Where(x => ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeighborhoods, x.Address.ValidateNull(), payments.GetPaymentBydocNumDxp(x.DocNumDxp)) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return invoices;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int invoice))
            {
                int.TryParse(parameters[ServiceConstants.Chips].Replace(ServiceConstants.RemisionChip, string.Empty), out int remision);
                var details = deliveryDetails.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == remision, x.InvoiceId.HasValue)).Select(y => y.InvoiceId.Value).ToList();
                var invoiceById = invoices.Where(x => details.Contains(x.InvoiceId)).ToList();
                invoiceById.AddRange(invoices.Where(x => x.DocNum == invoice));
                return invoiceById.DistinctBy(x => x.InvoiceId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return invoices.Where(x => listNames.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        private InvoiceOrderModel GetInvoiceToReturn(RetrieveInvoiceModel retrieveModel)
        {
            var listToReturn = new InvoiceOrderModel
            {
                Invoices = new List<InvoicesModel>(),
                TotalInvioces = retrieveModel.InvoiceHeader.Count,
                TotalDeliveries = 0,
            };

            var invoiceModel = new InvoiceSaleModel();
            var invoiceModelToAdd = new InvoicesModel();

            foreach (var invoice in retrieveModel.InvoiceHeader)
            {
                var invoiceDetails = retrieveModel.InvoiceDetails.Where(x => x.InvoiceId == invoice.InvoiceId);

                invoiceModel = new InvoiceSaleModel
                {
                    Doctor = invoice.Medico ?? string.Empty,
                    Invoice = invoice.DocNum,
                    Deliveries = invoiceDetails.Select(x => x.BaseEntry).Distinct().Count(),
                    Products = invoiceDetails.ToList().Count,
                    InvoiceDocDate = invoice.FechaInicio,
                    TypeOrder = invoice.TypeOrder,
                    IsPackage = invoice.IsPackage == ServiceConstants.IsPackage,
                };

                invoiceModelToAdd = new InvoicesModel
                {
                    Deliveries = null,
                    InvoiceHeader = null,
                    InvoiceSale = invoiceModel,
                };

                listToReturn.TotalDeliveries += invoiceModelToAdd.InvoiceSale.Deliveries;
                listToReturn.Invoices.Add(invoiceModelToAdd);
            }

            return listToReturn;
        }

        private InvoiceOrderModel GetInvoiceToReturnForSearch(List<InvoiceHeaderModel> invoiceHeader, List<InvoiceDetailModel> invoicesDetails)
        {
            var listToReturn = new InvoiceOrderModel
            {
                Invoices = new List<InvoicesModel>(),
                TotalInvioces = invoiceHeader.Count,
                TotalDeliveries = 0,
            };

            var invoiceModel = new InvoiceSaleModel();
            var invoiceModelToAdd = new InvoicesModel();

            foreach (var invoice in invoiceHeader)
            {
                var invoiceDetails = invoicesDetails.Where(x => x.InvoiceId == invoice.InvoiceId);

                invoiceModel = new InvoiceSaleModel
                {
                    Doctor = invoice.Medico ?? string.Empty,
                    Invoice = invoice.DocNum,
                    Deliveries = invoiceDetails.Select(x => x.BaseEntry).Distinct().Count(),
                    Products = invoiceDetails.ToList().Count,
                    InvoiceDocDate = invoice.FechaInicio,
                    TypeOrder = invoice.TypeOrder,
                    IsPackage = invoice.IsPackage == ServiceConstants.IsPackage,
                };

                invoiceModelToAdd = new InvoicesModel
                {
                    Deliveries = null,
                    InvoiceHeader = null,
                    InvoiceSale = invoiceModel,
                };

                listToReturn.TotalDeliveries += invoiceModelToAdd.InvoiceSale.Deliveries;
                listToReturn.Invoices.Add(invoiceModelToAdd);
            }

            return listToReturn;
        }

        /// <summary>
        /// gets the deliveries.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="invoiceDetails">the invoiceDocNum details.</param>
        /// <param name="userOrderModels">the user order modesl.</param>
        /// <param name="lineProducts">the lines prodcuts.</param>
        /// <returns>the data.</returns>
        private List<InvoiceDeliveryModel> GetDeliveryModel(List<CompleteDeliveryDetailModel> delivery, List<CompleteInvoiceDetailModel> invoiceDetails, List<UserOrderModel> userOrderModels, List<LineProductsModel> lineProducts)
        {
            var listToReturn = delivery.DistinctBy(x => x.DocNum).AsEnumerable()
                .Select(y =>
                {
                    var userOrderStatus = userOrderModels.Where(z => ServiceShared.CalculateAnd(z.DeliveryId == y.DocNum, !string.IsNullOrEmpty(z.Productionorderid))).Select(y => y.StatusAlmacen).ToList();
                    userOrderStatus.AddRange(lineProducts.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == y.DocNum, !string.IsNullOrEmpty(x.ItemCode))).Select(y => y.StatusAlmacen));
                    var salesOrders = delivery.Where(z => ServiceShared.CalculateAnd(z.DocNum == y.DocNum, z.Detalles != null, z.Detalles.BaseEntry.HasValue)).Select(a => a.Detalles.BaseEntry.Value).Distinct().ToList();

                    return new InvoiceDeliveryModel
                    {
                        DeliveryId = y.DocNum,
                        DeliveryDocDate = y.Detalles.DocDate,
                        SaleOrder = salesOrders.Distinct().Count(),
                        Status = ServiceShared.CalculateTernary(userOrderStatus.TrueForAll(z => z == ServiceConstants.Empaquetado), ServiceConstants.Empaquetado, userOrderStatus.Exists(z => z == ServiceConstants.Empaquetado) && userOrderStatus.Exists(z => z == ServiceConstants.Almacenado) ? ServiceConstants.BackOrder : ServiceConstants.Almacenado),
                        TotalItems = invoiceDetails.Where(a => a.Detail.BaseEntry.HasValue).Count(z => z.Detail.BaseEntry == y.DocNum),
                        IsPackage = y.IsPackage == "Y",
                    };
                }).ToList();

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
        private async Task<List<InvoiceProductModel>> GetProductModels(
            List<InvoiceDetailModel> invoices, List<DeliveryDetailModel> deliveryDetails, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders, List<IncidentsModel> incidents)
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
                var deliveriesDetail = localDeliverDetails.FirstOrDefault(x => ServiceShared.CalculateAnd(x.DeliveryId == invoice.BaseEntry.Value, x.ProductoId == invoice.ProductoId));
                var saleId = deliveriesDetail?.BaseEntry ?? 0;
                var prodId = deliveriesDetail?.ProductoId ?? string.Empty;

                usedDeliveries.Add(new DeliveryDetailModel { BaseEntry = saleId, ProductoId = prodId });

                var productType = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);

                if (item.IsMagistral.Equals("N"))
                {
                    var lineProduct = lineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == saleId, x.ItemCode == invoice.ProductoId));
                    lineProduct ??= new LineProductsModel();
                    var batchName = ServiceShared.DeserializeObject(lineProduct.BatchName, new List<AlmacenBatchModel>());

                    listBatches = await this.GetBatchesByDelivery(invoice.BaseEntry.Value, invoice.ProductoId, batchName);
                }

                var product = this.GetProductStatus(deliveryDetails, userOrders, lineProducts, orders, invoice, saleId);

                var canCancel = this.DetermineCanCancel(isMagistral: item.IsMagistral.Equals("Y"), invoice.BaseEntry.Value,  saleOrderId: saleId, productId: invoice.ProductoId, userOrders: userOrders, lineProducts: lineProducts);

                var incidentdb = incidents.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == product.Item3, x.ItemCode == item.ProductoId));
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = ServiceShared.DeserializeObject(incidentdb.Batches, new List<AlmacenBatchModel>()),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                listToReturn.Add(new InvoiceProductModel
                {
                    Batches = listBatches,
                    Container = invoice.Container,
                    Description = item.LargeDescription.ToUpper(),
                    ItemCode = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), $"{item.ProductoId} - {product.Item2}", item.ProductoId),
                    NeedsCooling = item.NeedsCooling.Equals("Y"),
                    ProductType = $"Producto {productType}",
                    Quantity = invoice.Quantity,
                    Status = product.Item1,
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    DeliveryId = invoice.BaseEntry.Value,
                    OrderId = product.Item2,
                    SaleOrderId = product.Item3,
                    Incident = ServiceShared.CalculateTernary(string.IsNullOrEmpty(localIncident.Status), null, localIncident),
                    CanCancel = canCancel,
                });
            }

            return listToReturn;
        }

        private bool DetermineCanCancel(bool isMagistral, int deliveryId, int saleOrderId, string productId, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            if (isMagistral)
            {
                var order = userOrders.FirstOrDefault(or => or.DeliveryId == deliveryId && or.Salesorderid == saleOrderId.ToString());
                return order != null && order.StatusInvoice == null;
            }
            else
            {
                var line = lineProducts.FirstOrDefault(lp => lp.DeliveryId == deliveryId && lp.SaleOrderId == saleOrderId && lp.ItemCode == productId);
                return line != null && line.StatusInvoice == null;
            }
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
            var saleId = deliveriesDetail?.BaseEntry ?? 0;
            var order = orders.FirstOrDefault(x => ServiceShared.CalculateAnd(x.PedidoId == saleId, x.ProductoId == invoice.ProductoId));

            if (order != null)
            {
                var userOrder = userOrders.FirstOrDefault(x => x.Productionorderid == order.OrdenId.ToString());
                status = ServiceShared.CalculateTernary(userOrder == null, status, userOrder?.StatusAlmacen);
            }
            else
            {
                var lineProduct = lineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(x.SaleOrderId == saleId, x.ItemCode == invoice.ProductoId, x.DeliveryId == invoice.InvoiceId));
                status = ServiceShared.CalculateTernary(lineProduct == null, status, lineProduct?.StatusAlmacen);
                order = new OrdenFabricacionModel { OrdenId = 0 };
            }

            return new Tuple<string, int, int>(status, order.OrdenId, saleId);
        }

        /// <summary>
        /// Get the batches by delivery.
        /// </summary>
        /// <param name="delivery">the delivery.</param>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        private async Task<List<string>> GetBatchesByDelivery(int delivery, string itemCode, List<AlmacenBatchModel> batchName)
        {
            var batchTransacion = await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, delivery);
            var batchesQty = (await this.sapDao.GetBatchTransationsQtyByLogEntry(batchTransacion.Select(x => x.LogEntry).ToList())).ToList();
            var tuple = batchesQty.Select(x => (x.SysNumber, x.ItemCode)).ToList();
            var validBatches = (await this.sapDao.GetSelectedBatches(tuple)).ToList();
            var listToReturn = validBatches
                .Where(x => batchName.Any(y => y.BatchNumber == x.DistNumber))
                .Select(z =>
                {
                    var selectedBatch = batchName.Where(x => x.BatchNumber == z.DistNumber).First();
                    return $"{selectedBatch.WarehouseCode ?? ServiceConstants.PT} | {z.DistNumber} | {(int)batchName.GetBatch(z.DistNumber).BatchQty} pz | Cad: {z.FechaExp}";
                })
                .ToList();

            return listToReturn;
        }

        private async Task<DeliveryScannedModel> GetDeliveryScannedData(InvoiceHeaderModel invoice, List<DeliveryDetailModel> deliveries, List<InvoiceDetailModel> invoices, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<OrdenFabricacionModel> orders)
        {
            var delivery = deliveries.FirstOrDefault();
            delivery = ServiceShared.CalculateTernary(delivery == null || !delivery.BaseEntry.HasValue, new DeliveryDetailModel { BaseEntry = 0 }, delivery);

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { delivery.BaseEntry.Value });
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
                Status = ServiceShared.CalculateTernary(ServiceShared.CalculateAnd(products.Any(), products.All(x => x.Status.Equals(ServiceConstants.Empaquetado))), ServiceConstants.Empaquetado, ServiceConstants.Almacenado),
                ListSalesOrder = string.Join(", ", listSales),
            };

            return deliveryData;
        }

        /// <summary>
        /// Gets the firs value for the deliveryDetailModel, the invoie detail by item code and the invoiceDocNum header.
        /// </summary>
        /// <param name="saleOrder">the sale order.</param>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<InvoiceDetailModel, InvoiceHeaderModel>> GetSaleOrderInvoiceDataByItemCode(int saleOrder, string itemCode, int deliveryId = 0)
        {
            var deliveryDetailsList = (await this.sapDao.GetDeliveryDetailBySaleOrder(new List<int> { saleOrder })).ToList();
            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(deliveryDetailsList.Select(x => x.DeliveryId).ToList())).Where(x => x.Canceled != "Y").ToList();
            var deliveryDetails = deliveryDetailsList.FirstOrDefault(x => deliveryHeaders.Any(y => y.DocNum == x.DeliveryId) && x.ProductoId == itemCode && x.InvoiceId.HasValue && (deliveryId == 0 ? true : x.DeliveryId == deliveryId));
            deliveryDetails ??= new DeliveryDetailModel { InvoiceId = 0 };

            var header = (await this.sapDao.GetInvoiceHeaderByInvoiceId(new List<int> { deliveryDetails.InvoiceId.Value })).FirstOrDefault();
            header ??= new InvoiceHeaderModel();

            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(new List<int> { header.InvoiceId })).FirstOrDefault(x => x.BaseEntry.HasValue && x.ProductoId == itemCode && deliveryDetails.DeliveryId == x.BaseEntry.Value);
            invoiceDetails ??= new InvoiceDetailModel { BaseEntry = 0 };

            return new Tuple<InvoiceDetailModel, InvoiceHeaderModel>(invoiceDetails, header);
        }

        /// <summary>
        /// Gets the orders from user Orders.
        /// </summary>
        /// <returns>the user orders.</returns>
        private async Task<List<UserOrderModel>> GetUserOrdersByRangeDate(DateTime startDate, DateTime endDate)
        {
            var pedidosResponse = await this.pedidosService.GetUserPedidos(
                string.Format(
                    ServiceConstants.GetUserOrderInvoiceByRangeDate,
                    startDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy),
                    endDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy)));

            return JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
        }

        /// <summary>
        /// Gets the line products.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<List<LineProductsModel>> GetLineProductsByRangeDate(DateTime startDate, DateTime endDate)
        {
            var lineProductsResponse = await this.almacenService.GetAlmacenOrders(
                string.Format(
                    ServiceConstants.GetLinesForInvoiceByRangeDate,
                    startDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy),
                    endDate.ToString(ServiceConstants.DateTimeFormatddMMyyyy)));

            return JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());
        }

        private async Task<IEnumerable<InvoiceHeaderModel>> GetInvoicesByInitialFilters(Dictionary<string, string> parameters)
        {
            var (invoicesHeader, validateChipByName) = await this.GetInvoicesToSearch(parameters);

            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var transactionsIds = invoicesHeader.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
                var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
                var localNeighborhoods = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                invoicesHeader = invoicesHeader.Where(x => ServiceUtils.IsTypeLocal(ServiceConstants.NuevoLeon, localNeighborhoods, x.Address.ValidateNull(), payments.GetPaymentBydocNumDxp(x.DocNumDxp)) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (validateChipByName)
            {
                var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
                invoicesHeader = invoicesHeader
                    .Where(x =>
                        listNames.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower())));
            }

            var deliveries = await this.sapDao.GetDeliveryDetailJoinProductByInvoicesIds(invoicesHeader.Select(x => x.InvoiceId).ToList());
            invoicesHeader = await this.FilteredByLineProductsUserOrdersStatus(invoicesHeader, deliveries);

            return invoicesHeader;
        }

        private async Task<IEnumerable<InvoiceHeaderModel>> FilteredByLineProductsUserOrdersStatus(
            IEnumerable<InvoiceHeaderModel> invoicesHeader,
            IEnumerable<DeliveryDetailModel> deliveryDetails)
        {
            var deliveryIds = deliveryDetails.Select(x => x.DeliveryId);
            var responsePedidos = await this.pedidosService.PostPedidos(deliveryIds, ServiceConstants.GetUserOrderInvoiceByDeliveryIds);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(responsePedidos.Response.ToString());

            var response = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLinesForInvoiceByDeliveryIds, deliveryIds);
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(response.Response.ToString());

            var listIds = userOrders.Select(y => y.DeliveryId).ToList();
            listIds.AddRange(lineProducts.Select(y => y.DeliveryId));
            listIds = listIds.Distinct().ToList();
            var filteredInvoicesIds = deliveryDetails.Where(x => listIds.Contains(x.DeliveryId)).Select(x => x.InvoiceId);
            return invoicesHeader.Where(x => filteredInvoicesIds.Contains(x.InvoiceId));
        }

        private async Task<Tuple<IEnumerable<InvoiceHeaderModel>, bool>> GetInvoicesToSearch(Dictionary<string, string> parameters)
        {
            var validateChipByName = false;
            var isChipByRemisionId = false;
            var isChipByInvoiceDocNum = false;
            int remisionId = 0;
            var invoiceDocNum = 0;

            if (parameters.ContainsKey(ServiceConstants.Chips))
            {
                isChipByRemisionId = parameters[ServiceConstants.Chips].Contains(ServiceConstants.RemisionChip) ?
                                     int.TryParse(parameters[ServiceConstants.Chips].Replace(ServiceConstants.RemisionChip, string.Empty), out remisionId) :
                                     false;
                isChipByInvoiceDocNum = int.TryParse(parameters[ServiceConstants.Chips], out invoiceDocNum);
            }

            if (isChipByRemisionId || isChipByInvoiceDocNum)
            {
                return new Tuple<IEnumerable<InvoiceHeaderModel>, bool>(
                        await this.GetInvoicesByRemsionsIdOrInvoiceDocNum(isChipByInvoiceDocNum, invoiceDocNum, isChipByRemisionId, remisionId),
                        validateChipByName);
            }

            var startDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                        .ToUniversalDateTime().Date;

            var endDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                                      .ToUniversalDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            validateChipByName = parameters.ContainsKey(ServiceConstants.Chips);
            return new Tuple<IEnumerable<InvoiceHeaderModel>, bool>(await this.sapDao.GetInvoiceHeaderJoinDoctorByDatesRangesForSearchs(startDate, endDate), validateChipByName);
        }

        private async Task<IEnumerable<InvoiceHeaderModel>> GetInvoicesByRemsionsIdOrInvoiceDocNum(bool isChipByInvoiceDocNum, int invoiceDocNum, bool isChipByRemisionId, int remisionId)
        {
            var docNums = new List<int>();

            if (isChipByInvoiceDocNum)
            {
                docNums.Add(invoiceDocNum);
            }

            if (isChipByRemisionId)
            {
                docNums.AddRange(await this.GetInvoicesByRemisionsIds([remisionId]));
            }

            return await this.sapDao.GetInvoiceHeaderJoinDoctorByDocNumsForSearchs(docNums);
        }

        private async Task<List<int>> GetInvoicesByRemisionsIds(List<int> docEntrys)
        {
            var deliveryDetails = await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(docEntrys);
            return deliveryDetails.Where(y => y.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();
        }
    }
}
