// <summary>
// <copyright file="AdvanceLookService.cs" company="Axity">
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
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// class for advance looks.
    /// </summary>
    public class AdvanceLookService : IAdvanceLookService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceLookService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="usersService">The user servie.</param>
        public AdvanceLookService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, IUsersService usersService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> AdvanceLookUp(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                return await this.GetElementsById(parameters[ServiceConstants.DocNum]);
            }

            return await this.GetElementsByDoctor(parameters);
        }

        /// <summary>
        /// Gets the cards for look up by id.
        /// </summary>
        /// <param name="docNum">the docnum.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> GetElementsById(string docNum)
        {
            int.TryParse(docNum, out int intDocNum);

            if (intDocNum == 0)
            {
                var cards = new CardsAdvancedLook
                {
                    CardDelivery = new List<AlmacenSalesHeaderModel>(),
                    CardDistribution = new List<InvoiceHeaderAdvancedLookUp>(),
                    CardInvoice = new List<InvoiceHeaderAdvancedLookUp>(),
                    CardOrder = new List<AlmacenSalesHeaderModel>(),
                };

                return ServiceUtils.CreateResult(true, 200, null, cards, null, null);
            }

            var listDocs = new List<int> { intDocNum };
            var userOrdersResponse = await this.pedidosService.GetUserPedidos(listDocs, ServiceConstants.AdvanceLookId);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listDocs);
            var almacenData = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            var tupleList = this.KindLookUp(userOrders, almacenData, listDocs);
            var response = await this.GetStatusToSearch(userOrders, almacenData, tupleList);
            return ServiceUtils.CreateResult(true, 200, null, response, null, null);
        }

        /// <summary>
        /// Gets the cards for look up by id.
        /// </summary>
        /// <param name="parameters">the docnum.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> GetElementsByDoctor(Dictionary<string, string> parameters)
        {
            var doctorValue = parameters.ContainsKey(ServiceConstants.Doctor) ? parameters[ServiceConstants.Doctor].Split(",").ToList() : new List<string>();
            var listDocs = await this.GetIdsToLookByDoctor(parameters);
            var userOrdersResponse = await this.pedidosService.GetUserPedidos(listDocs, ServiceConstants.AdvanceLookId);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listDocs);
            var almacenData = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            var tupleList = this.KindLookUp(userOrders, almacenData, listDocs);
            var response = await this.GetStatusToSearch(userOrders, almacenData, tupleList);

            response.CardOrder = response.CardOrder.Where(x => doctorValue.All(y => x.Doctor.ToLower().Contains(y.ToLower()))).ToList();
            response.CardDelivery = response.CardDelivery.Where(x => doctorValue.All(y => x.Doctor.ToLower().Contains(y.ToLower()))).ToList();
            response.CardInvoice = response.CardInvoice.Where(x => doctorValue.All(y => x.Doctor.ToLower().Contains(y.ToLower()))).ToList();
            response.CardDistribution = response.CardDistribution.Where(x => doctorValue.All(y => x.Doctor.ToLower().Contains(y.ToLower()))).ToList();

            return ServiceUtils.CreateResult(true, 200, null, response, null, null);
        }

        /// <summary>
        /// Gets what is the identity of the id.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="adnvaceLookUp">the almacen data.</param>
        /// <param name="idsToFind">the id to find.</param>
        /// <returns>the tuple.</returns>
        private List<Tuple<int, string>> KindLookUp(List<UserOrderModel> userOrders, AdnvaceLookUpModel adnvaceLookUp, List<int> idsToFind)
        {
            var tupleIds = new List<Tuple<int, string>>();

            idsToFind.ForEach(id =>
            {
                var match = false;

                if (userOrders.Any(x => x.Salesorderid == id.ToString()) || adnvaceLookUp.LineProducts.Any(x => x.SaleOrderId == id))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.SaleOrder));
                    match = true;
                }

                if (userOrders.Any(x => x.DeliveryId == id) || adnvaceLookUp.LineProducts.Any(x => x.DeliveryId == id))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.Delivery));
                    match = true;
                }

                if (userOrders.Any(x => x.InvoiceId == id) || adnvaceLookUp.LineProducts.Any(x => x.InvoiceId == id))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.Invoice));
                    match = true;
                }

                if (adnvaceLookUp.CancelationModel != null && adnvaceLookUp.CancelationModel.Any(x => x.CancelledId == id))
                {
                    var cancelled = adnvaceLookUp.CancelationModel.FirstOrDefault(x => x.CancelledId == id);
                    var type = cancelled.TypeCancellation.ToLower() == ServiceConstants.Invoice ? ServiceConstants.Invoice : ServiceConstants.Delivery;
                    tupleIds.Add(new Tuple<int, string>(id, type));
                    match = true;
                }

                if (!match)
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.DontExistsTable));
                }
            });

            return tupleIds;
        }

        private async Task<CardsAdvancedLook> GetStatusToSearch(List<UserOrderModel> userOrders, AdnvaceLookUpModel almacenData, List<Tuple<int, string>> tupleIds)
        {
            var sapSaleOrder = (await this.sapDao.GetAllOrdersWIthDetailByIds(tupleIds.Where(x => x.Item2 == ServiceConstants.SaleOrder || x.Item2 == ServiceConstants.DontExistsTable).Select(y => y.Item1).ToList())).ToList();
            var sapDeliveryDetails = (await this.sapDao.GetDeliveryByDocEntry(tupleIds.Where(x => x.Item2 == ServiceConstants.Delivery).Select(y => y.Item1).ToList())).ToList();
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryBySaleOrder(sapSaleOrder.Select(x => x.DocNum).ToList()));
            var sapDelivery = (await this.sapDao.GetDeliveryModelByDocNum(sapDeliveryDetails.Select(y => y.DeliveryId).ToList())).ToList();

            var sapInvoicesHeaders = (await this.sapDao.GetInvoiceHeadersByDocNum(tupleIds.Where(x => x.Item2 == ServiceConstants.Invoice || x.Item2 == ServiceConstants.DontExistsTable).Select(y => y.Item1).ToList())).ToList();
            var sapInvoicesDeatils = (await this.sapDao.GetInvoiceDetailByDocEntry(sapInvoicesHeaders.Select(x => x.InvoiceId).ToList())).ToList();
            sapInvoicesDeatils.AddRange(await this.sapDao.GetInvoiceDetailByBaseEntry(sapDelivery.Select(x => x.DocNum).ToList()));
            sapInvoicesHeaders.AddRange(await this.sapDao.GetInvoiceHeaderByInvoiceId(sapInvoicesDeatils.Select(x => x.InvoiceId).ToList()));

            var lineProducts = (await this.sapDao.GetAllLineProducts()).ToList();
            var cardToReturns = new CardsAdvancedLook
            {
                CardOrder = new List<AlmacenSalesHeaderModel>(),
                CardDelivery = new List<AlmacenSalesHeaderModel>(),
                CardInvoice = new List<InvoiceHeaderAdvancedLookUp>(),
                CardDistribution = new List<InvoiceHeaderAdvancedLookUp>(),
            };

            var temporalsapDeliveryDetails = new List<DeliveryDetailModel>();
            var temporalsapInvoicesDeatils = new List<InvoiceDetailModel>();
            var listInvoicedId = sapDeliveryDetails.Where(x => x.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).ToList();
            sapInvoicesHeaders.AddRange(await this.sapDao.GetInvoiceHeaderByInvoiceId(listInvoicedId));
            sapInvoicesDeatils.AddRange(await this.sapDao.GetInvoiceDetailByDocEntry(sapInvoicesHeaders.Select(x => x.InvoiceId).ToList()));

            sapInvoicesDeatils.GroupBy(x => new { x.BaseEntry, x.InvoiceId }).ToList().ForEach(x =>
            {
                temporalsapInvoicesDeatils.AddRange(x.DistinctBy(d => d.ProductoId));
            });
            sapInvoicesDeatils = temporalsapInvoicesDeatils;
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryByDocEntry(sapInvoicesDeatils.Where(x => x.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).ToList()));
            sapDeliveryDetails.GroupBy(x => x.DeliveryId).ToList().ForEach(x =>
            {
                temporalsapDeliveryDetails.AddRange(x.DistinctBy(d => d.ProductoId));
            });
            sapDeliveryDetails = temporalsapDeliveryDetails;

            var listIds = sapDeliveryDetails.Select(x => x.DeliveryId).ToList();
            listIds.AddRange(sapInvoicesHeaders.Select(x => x.DocNum));
            var userOrdersResponse = await this.pedidosService.GetUserPedidos(listIds, ServiceConstants.AdvanceLookId);
            var userOrdersForDelivery = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listIds);
            var almacenDataForDelivery = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            userOrders.AddRange(userOrdersForDelivery);
            almacenData.LineProducts.AddRange(almacenDataForDelivery.LineProducts);
            almacenData.PackageModels.AddRange(almacenDataForDelivery.PackageModels);
            almacenData.CancelationModel.AddRange(almacenDataForDelivery.CancelationModel);

            var deliveryCompanies = (await this.sapDao.GetDeliveryCompanyById(sapInvoicesHeaders.Select(x => x.TransportCode).ToList())).ToList();

            var userResponse = await this.usersService.GetUsersById(almacenData.PackageModels.Select(x => x.AssignedUser).ToList(), ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());

            tupleIds.ForEach(order =>
            {
                cardToReturns.CardOrder.AddRange(this.GetIsReceptionOrders(order, userOrders, almacenData.LineProducts, sapSaleOrder, sapDeliveryDetails, lineProducts, sapDelivery));
                cardToReturns.CardDelivery.AddRange(this.GetIsReceptionDelivery(order, userOrders, almacenData.LineProducts, sapDeliveryDetails, sapDelivery, lineProducts, almacenData.CancelationModel, sapInvoicesHeaders));
                cardToReturns.CardInvoice.AddRange(this.GetIsPackageInvoice(order, userOrdersForDelivery, almacenDataForDelivery.LineProducts, almacenData.CancelationModel, sapInvoicesHeaders, sapInvoicesDeatils, sapDeliveryDetails));
                cardToReturns.CardDistribution.AddRange(this.GetIsPackageDistribution(order, userOrders, almacenData.LineProducts, sapInvoicesHeaders, sapInvoicesDeatils, sapDeliveryDetails, almacenData.PackageModels, deliveryCompanies, users));
            });

            return cardToReturns;
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionOrders(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<CompleteOrderModel> orderDetail, List<DeliveryDetailModel> deliveryDetails, List<ProductoModel> productModel, List<DeliverModel> deliveryHeader)
        {
            var listItemCode = productModel.Select(x => x.ProductoId).ToList();
            var orderbyDocNum = orderDetail.Where(x => x.DocNum == tuple.Item1 && x.PedidoStatus == "O" && x.Detalles != null).ToList();
            var isLineSale = tuple.Item2 == ServiceConstants.DontExistsTable && orderbyDocNum.Any() && orderbyDocNum.All(x => listItemCode.Contains(x.Detalles.ProductoId));
            if (tuple.Item2 != ServiceConstants.SaleOrder && !isLineSale)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var existanceMag = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString());
            existanceMag ??= new UserOrderModel { Status = string.Empty };
            var existanceLin = lineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1);
            existanceLin ??= new LineProductsModel { StatusAlmacen = string.Empty };

            if (existanceMag.Status == ServiceConstants.Cancelado || existanceLin.StatusAlmacen == ServiceConstants.Cancelado)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString() && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)));
            var lineProductOrder = lineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1 && x.StatusAlmacen == ServiceConstants.Recibir);
            return this.GenerateCardForReceptionOrders(tuple, orderDetail, deliveryDetails, userOrder, lineProductOrder, productModel, deliveryHeader, userOrders);
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionOrders(Tuple<int, string> tuple, List<CompleteOrderModel> orderDetail, List<DeliveryDetailModel> deliveryDetails, UserOrderModel userOrder, LineProductsModel lineProductOrder, List<ProductoModel> productModel, List<DeliverModel> deliveryHeader, List<UserOrderModel> userOrders)
        {
            var order = new CompleteOrderModel();
            var status = string.Empty;
            var totalItems = 0;
            var totalPieces = 0;
            var productType = string.Empty;
            var invoiceType = string.Empty;
            var saporders = new List<CompleteOrderModel>();
            var porRecibirDate = DateTime.Now;
            var hasCandidate = false;
            var initDate = DateTime.Now;

            if (userOrder != null)
            {
                userOrders = userOrders.Where(x => x.Salesorderid == tuple.Item1.ToString()).ToList();
                saporders = orderDetail.Where(x => x.DocNum.ToString() == userOrder.Salesorderid).ToList();
                order = saporders.FirstOrDefault();
                status = userOrder.Status == ServiceConstants.Finalizado && userOrder.StatusAlmacen == ServiceConstants.BackOrder ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;
                status = userOrder.Status != ServiceConstants.Finalizado && ServiceConstants.Status != ServiceConstants.Almacenado ? ServiceConstants.Pendiente : status;
                productType = saporders.Any(x => x.Detalles != null && productModel.Any(p => p.ProductoId == x.Detalles.ProductoId)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                porRecibirDate = userOrder.CloseDate ?? porRecibirDate;
                hasCandidate = true;
                hasCandidate = this.CalulateIfSaleOrderIsCandidate(userOrders, userOrder.Status);
            }

            var hasDeliveries = deliveryDetails.Where(x => x.BaseEntry == tuple.Item1).Select(x => x.DeliveryId).ToList();
            var hasActiveDeliveries = hasDeliveries.Any() && deliveryHeader.Where(x => hasDeliveries.Contains(x.DocNum)).Any(y => y.Canceled != "Y");

            if (userOrder == null && (lineProductOrder != null || (lineProductOrder == null && !hasActiveDeliveries)))
            {
                saporders = orderDetail.Where(x => x.DocNum == tuple.Item1).ToList();
                order = saporders.FirstOrDefault();
                status = lineProductOrder == null ? ServiceConstants.PorRecibir : lineProductOrder.StatusAlmacen;
                status = lineProductOrder != null && lineProductOrder.StatusAlmacen == ServiceConstants.Recibir ? ServiceConstants.PorRecibir : status;
                productType = ServiceConstants.Linea;
                porRecibirDate = order.FechaInicio != null ? DateTime.ParseExact(order.FechaInicio, "dd/MM/yyyy", null) : porRecibirDate;
                hasCandidate = true;
            }

            if (!hasCandidate)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            invoiceType = order.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
            totalItems = saporders.Count;
            totalPieces = (int)saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
            initDate = order.FechaInicio != null ? DateTime.ParseExact(order.FechaInicio, "dd/MM/yyyy", null) : initDate;

            var saleHeader = new AlmacenSalesHeaderModel
            {
                DocNum = order.DocNum,
                Status = status,
                TypeSaleOrder = $"Pedido {productType}",
                Doctor = order.Medico,
                InvoiceType = invoiceType,
                TotalItems = totalItems,
                InitDate = initDate,
                Client = order.Cliente,
                TotalPieces = totalPieces,
                DataCheckin = porRecibirDate,
            };

            return new List<AlmacenSalesHeaderModel> { saleHeader };
        }

        private bool CalulateIfSaleOrderIsCandidate(List<UserOrderModel> userOrders, string statusOrder)
        {
            var hasCandidate = true;
            if (statusOrder == ServiceConstants.Finalizado)
            {
                hasCandidate = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(x => (x.Status == ServiceConstants.Finalizado || x.Status == ServiceConstants.Almacenado) && x.FinishedLabel == 1);
                hasCandidate = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(x => x.Status == ServiceConstants.Almacenado && x.FinishedLabel == 1) ? false : hasCandidate;
                return hasCandidate;
            }

            if (statusOrder == ServiceConstants.Liberado)
            {
                hasCandidate = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(x => (x.Status == ServiceConstants.Almacenado || x.Status == ServiceConstants.Finalizado || x.Status == ServiceConstants.Pendiente) && x.FinishedLabel == 1);
                var allAreToReceive = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(x => x.Status == ServiceConstants.Pendiente && x.FinishedLabel == 1);
                hasCandidate = allAreToReceive ? false : hasCandidate;
                return hasCandidate;
            }

            return hasCandidate;
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionDelivery(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<DeliveryDetailModel> deliveryDetailModels, List<DeliverModel> deliveryHeaders, List<ProductoModel> lineSapProducts, List<CancellationResourceModel> cancellations, List<InvoiceHeaderModel> invoiceHeaders)
        {
            if (tuple.Item2 == ServiceConstants.Invoice)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrdersFromSale = userOrders.Where(x => x.Salesorderid == tuple.Item1.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromSale = lineProducts.Where(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var deliveryIdsBySale = userOrdersFromSale.Select(x => x.DeliveryId).ToList();
                deliveryIdsBySale.AddRange(lineProductsFromSale.Select(y => y.DeliveryId));
                return this.GenerateCardForReceptionDelivery(deliveryIdsBySale, deliveryDetailModels, deliveryHeaders, lineSapProducts, userOrdersFromSale, lineProductsFromSale, invoiceHeaders);
            }

            var isCancelled = cancellations != null && cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (tuple.Item2 == ServiceConstants.Delivery && !isCancelled)
            {
                var userOrdersFromDelivery = userOrders.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromDelivery = lineProducts.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var delivryIds = new List<int> { tuple.Item1 };
                return this.GenerateCardForReceptionDelivery(delivryIds, deliveryDetailModels, deliveryHeaders, lineSapProducts, userOrdersFromDelivery, lineProductsFromDelivery, invoiceHeaders);
            }

            if (tuple.Item2 == ServiceConstants.Delivery && isCancelled)
            {
                var delivryIds = new List<int> { tuple.Item1 };
                var cancelation = cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                var cancelledOrder = new List<UserOrderModel> { new UserOrderModel { DeliveryId = tuple.Item1, DateTimeCheckIn = cancelation.CancelDate, StatusAlmacen = ServiceConstants.Cancelado } };
                return this.GenerateCardForReceptionDelivery(delivryIds, deliveryDetailModels, deliveryHeaders, lineSapProducts, cancelledOrder, new List<LineProductsModel>(), new List<InvoiceHeaderModel>());
            }

            return new List<AlmacenSalesHeaderModel>();
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionDelivery(List<int> possibleDeliveries, List<DeliveryDetailModel> deliveryDetailModels, List<DeliverModel> deliveryHeaders, List<ProductoModel> lineSapProducts, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<InvoiceHeaderModel> invoiceHeaders)
        {
            var saleHeader = new List<AlmacenSalesHeaderModel>();

            foreach (var delivery in possibleDeliveries.Distinct())
            {
                var deliveriesWithInvoice = deliveryDetailModels.Where(x => x.DeliveryId == delivery && x.InvoiceId.HasValue && x.InvoiceId.Value != 0).Select(y => y.InvoiceId.Value).ToList();
                var areDeliveriesWithValidInvoice = invoiceHeaders.Where(x => x.Canceled == "N").Any(y => deliveriesWithInvoice.Contains(y.InvoiceId));
                if (areDeliveriesWithValidInvoice)
                {
                    continue;
                }

                var header = deliveryHeaders.FirstOrDefault(x => x.DocNum == delivery);
                header ??= new DeliverModel();
                var deliveryDetail = deliveryDetailModels.Where(x => x.DeliveryId == delivery).DistinctBy(x => x.ProductoId).ToList();
                var saleOrder = deliveryDetail.FirstOrDefault() != null ? deliveryDetail.FirstOrDefault().BaseEntry : 0;
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);
                var invoiceType = header.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
                var productType = this.GenerateListProductTypeDelivery(deliveryDetail, lineSapProducts.Select(x => x.ProductoId).ToList());

                var userOrderByDelivery = userOrders.FirstOrDefault(x => x.DeliveryId == delivery);
                var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.DeliveryId == delivery);
                var initDate = userOrderByDelivery != null ? userOrderByDelivery.DateTimeCheckIn : lineProductByDelivery.DateCheckIn;
                var status = userOrderByDelivery != null ? userOrderByDelivery.StatusAlmacen : lineProductByDelivery.StatusAlmacen;

                var saleHeaderItem = new AlmacenSalesHeaderModel
                {
                    Client = header.Cliente,
                    DocNum = saleOrder,
                    Doctor = header.Medico,
                    InitDate = header.FechaInicio,
                    Status = status,
                    Remision = delivery,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
                    DataCheckin = initDate.Value,
                };
                saleHeader.Add(saleHeaderItem);
            }

            return saleHeader;
        }

        private string GenerateListProductTypeDelivery(List<DeliveryDetailModel> deliveryDetails, List<string> lineProducts)
        {
            if (deliveryDetails.All(x => lineProducts.Contains(x.ProductoId)))
            {
                return ServiceConstants.Linea;
            }

            if (!deliveryDetails.All(x => lineProducts.Contains(x.ProductoId)) && deliveryDetails.Any(x => lineProducts.Contains(x.ProductoId)))
            {
                return ServiceConstants.Mixto;
            }

            return ServiceConstants.Magistral;
        }

        /// <summary>
        /// Gets the ids to look by doctor.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private async Task<List<int>> GetIdsToLookByDoctor(Dictionary<string, string> parameters)
        {
            var doctorValue = parameters.ContainsKey(ServiceConstants.Doctor) ? parameters[ServiceConstants.Doctor].Split(",").ToList() : new List<string>();
            var dictDates = ServiceUtils.GetDateFilter(parameters);
            var type = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.SaleOrder;

            switch (type)
            {
                case ServiceConstants.SaleOrder:
                    var orders = (await this.sapDao.GetOrderModelByDocDate(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    orders = orders.Where(x => doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return orders.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Delivery:
                    var deliveries = (await this.sapDao.GetDeliveryByDocDate(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    deliveries = deliveries.Where(x => doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return deliveries.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Invoice:
                    var invoices = (await this.sapDao.GetInvoiceHeadersByDocDate(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    invoices = invoices.Where(x => doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return invoices.Select(x => x.DocNum).Distinct().ToList();
            }

            return new List<int>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GetIsPackageInvoice(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<CancellationResourceModel> cancellations, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder)
        {
            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrder = userOrders.Where(x => (x.Salesorderid == tuple.Item1.ToString()) && (!string.IsNullOrEmpty(x.Productionorderid)) && (x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado) && (x.DeliveryId != 0)).ToList();
                var lineProductOrder = lineProducts.Where(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && (x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado) && x.DeliveryId != 0).ToList();
                var deliveryIdsBySale = userOrder.Select(x => x.DeliveryId).ToList();
                deliveryIdsBySale.AddRange(lineProductOrder.Select(y => y.DeliveryId));
                return this.GenerateCardForPackageInvoiceTheSalesOrder(deliveryIdsBySale, userOrders, lineProducts, deliverysToLookSaleOrder, invoiceHeadersToLook, invoiceDetailsToLook, deliverysToLookSaleOrder);
            }

            var isCancelled = cancellations != null && cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (tuple.Item2 == ServiceConstants.Delivery && !isCancelled)
            {
                var delivryIds = new List<int> { tuple.Item1 };
                return this.GenerateCardForPackageInvoiceTheSalesOrder(delivryIds, userOrders, lineProducts, deliverysToLookSaleOrder, invoiceHeadersToLook, invoiceDetailsToLook, deliverysToLookSaleOrder);
            }

            if (tuple.Item2 == ServiceConstants.Invoice && !isCancelled)
            {
                var invoice = invoiceHeadersToLook.FirstOrDefault(x => x.DocNum == tuple.Item1);
                invoice ??= new InvoiceHeaderModel();
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoice, invoiceDetailsToLook, deliverysToLookSaleOrder);
            }

            if (tuple.Item2 == ServiceConstants.Invoice && isCancelled)
            {
                var cancelation = cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                return this.GenerateCardForPackageInvoiceCancelled(invoiceHeadersToLook, invoiceDetailsToLook, deliverysToLookSaleOrder, cancelation);
            }

            if ((tuple.Item2 == ServiceConstants.DontExistsTable) && (userOrders.Any() || lineProducts.Any()) && !isCancelled)
            {
                var invoice = invoiceHeadersToLook.FirstOrDefault(x => x.DocNum == tuple.Item1);
                invoice ??= new InvoiceHeaderModel();
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoice, invoiceDetailsToLook, deliverysToLookSaleOrder);
            }

            return new List<InvoiceHeaderAdvancedLookUp>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoiceCancelled(List<InvoiceHeaderModel> invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder, CancellationResourceModel canceled)
        {
            var invoicesHeaders = new List<InvoiceHeaderAdvancedLookUp>();

            var invoiceHeader = invoiceHeaders.FirstOrDefault(x => x.DocNum == canceled.CancelledId);

            if (invoiceHeader == null)
            {
                return invoicesHeaders;
            }

            invoiceHeader ??= new InvoiceHeaderModel();
            var invoiceDetail = invoiceDetailsToLook.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();
            var totalProducts = invoiceDetail.Count;

            var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
                    {
                        Address = invoiceHeader.Address.Replace("\r", string.Empty),
                        Client = invoiceHeader.Cliente,
                        Doctor = invoiceHeader.Medico ?? string.Empty,
                        Invoice = invoiceHeader.DocNum,
                        DocEntry = invoiceHeader.InvoiceId,
                        InvoiceDocDate = invoiceHeader.FechaInicio,
                        ProductType = invoiceHeader.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                        TotalDeliveries = invoiceDetail.DistinctBy(x => x.BaseEntry.Value).Count(),
                        TotalProducts = totalProducts,
                        StatusDelivery = ServiceConstants.Cancelado,
                        DataCheckin = canceled.CancelDate,
                        SalesOrder = deliverysToLookSaleOrder.DistinctBy(x => x.BaseEntry).Count(),
                        IsLookUpInvoices = true,
                    };
            invoicesHeaders.Add(invoiceHeaderLookUp);
            return invoicesHeaders;
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoiceTheSalesOrder(List<int> listDeliveryId, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<DeliveryDetailModel> deliveryDetailModels, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder)
        {
            var invoicesHeaders = new List<InvoiceHeaderAdvancedLookUp>();

            foreach (var delivery in listDeliveryId.Distinct())
            {
                if (deliveryDetailModels.Where(x => x.DeliveryId == delivery).Any(y => y.InvoiceId.HasValue && y.InvoiceId.Value != 0) && invoiceHeadersToLook.Any(x => x.Canceled != "Y"))
                {
                    var invoiceId = deliveryDetailModels.FirstOrDefault(x => x.DeliveryId == delivery);
                    var invoiceHeader = invoiceHeadersToLook.FirstOrDefault(x => x.Canceled != "Y" && x.InvoiceId == invoiceId.InvoiceId);
                    if (invoiceHeader == null)
                    {
                        continue;
                    }

                    var invoiceDetail = invoiceDetailsToLook.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();
                    var deliveryDetails = deliverysToLookSaleOrder.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();

                    var deliverys = this.GetDeliveryModel(deliveryDetails, invoiceDetail, userOrders, lineProducts);
                    var deliveryHeader = deliverys.FirstOrDefault(x => x.DeliveryId == delivery);
                    var totalProducts = invoiceDetail.Count;

                    var userOrderByDate = userOrders.FirstOrDefault(x => x.DeliveryId == delivery);
                    var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.DeliveryId == delivery);
                    var initDate = userOrderByDate != null ? userOrderByDate.DateTimeCheckIn : lineProductByDelivery.DateCheckIn;

                    if (!deliverys.All(x => x.Status == ServiceConstants.Empaquetado))
                    {
                        var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
                        {
                            Address = invoiceHeader.Address.Replace("\r", string.Empty),
                            Client = invoiceHeader.Cliente,
                            Doctor = invoiceHeader.Medico ?? string.Empty,
                            Invoice = invoiceHeader.DocNum,
                            DocEntry = invoiceHeader.InvoiceId,
                            InvoiceDocDate = invoiceHeader.FechaInicio,
                            ProductType = invoiceHeader.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                            TotalDeliveries = deliverys.Count,
                            TotalProducts = totalProducts,
                            DeliverId = delivery,
                            SalesOrder = deliveryHeader.SaleOrder,
                            StatusDelivery = deliveryHeader.Status,
                            DataCheckin = initDate,
                            IsLookUpInvoices = false,
                        };
                        invoicesHeaders.Add(invoiceHeaderLookUp);
                    }
                }
            }

            return invoicesHeaders;
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoice(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, InvoiceHeaderModel invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder)
        {
            var invoicesHeaders = new List<InvoiceHeaderAdvancedLookUp>();

            if (deliverysToLookSaleOrder.Any(y => y.InvoiceId.HasValue && y.InvoiceId.Value != 0))
            {
                var invoiceDetail = invoiceDetailsToLook.Where(x => x.InvoiceId == invoiceHeaders.InvoiceId).ToList();
                var deliveryDetails = deliverysToLookSaleOrder.Where(x => x.InvoiceId == invoiceHeaders.InvoiceId).ToList();

                var deliverys = this.GetDeliveryModel(deliveryDetails, invoiceDetail, userOrders, lineProducts);
                var totalProducts = invoiceDetail.Count;

                var userOrderByDate = userOrders.FirstOrDefault(x => x.InvoiceId == invoiceHeaders.DocNum);
                userOrderByDate = userOrderByDate != null ? userOrderByDate : userOrders.FirstOrDefault();
                var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.InvoiceId == invoiceHeaders.DocNum);
                lineProductByDelivery = lineProductByDelivery != null ? lineProductByDelivery : lineProducts.FirstOrDefault();
                var initDate = userOrderByDate != null ? userOrderByDate.DateTimeCheckIn.Value : lineProductByDelivery.DateCheckIn.Value;

                if (!deliverys.All(x => x.Status == ServiceConstants.Empaquetado))
                {
                    var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
                    {
                        Address = invoiceHeaders.Address.Replace("\r", string.Empty),
                        Client = invoiceHeaders.Cliente,
                        Doctor = invoiceHeaders.Medico ?? string.Empty,
                        Invoice = invoiceHeaders.DocNum,
                        DocEntry = invoiceHeaders.InvoiceId,
                        InvoiceDocDate = invoiceHeaders.FechaInicio,
                        ProductType = invoiceHeaders.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                        TotalDeliveries = deliverys.Count,
                        TotalProducts = totalProducts,
                        StatusDelivery = ServiceConstants.Almacenado,
                        DataCheckin = initDate,
                        SalesOrder = deliverys.DistinctBy(x => x.SaleOrder).Count(),
                        IsLookUpInvoices = true,
                    };
                    invoicesHeaders.Add(invoiceHeaderLookUp);
                }
            }

            return invoicesHeaders;
        }

        private List<InvoiceDeliveryModel> GetDeliveryModel(List<DeliveryDetailModel> delivery, List<InvoiceDetailModel> invoiceDetails, List<UserOrderModel> userOrderModels, List<LineProductsModel> lineProducts)
        {
            var listToReturn = new List<InvoiceDeliveryModel>();
            delivery.DistinctBy(x => x.DeliveryId).ToList()
                .ForEach(y =>
                {
                    var userOrderStatus = userOrderModels.Where(z => z.DeliveryId == y.DeliveryId && !string.IsNullOrEmpty(z.Productionorderid)).Select(y => y.StatusAlmacen).ToList();
                    userOrderStatus.AddRange(lineProducts.Where(x => x.DeliveryId == y.DeliveryId && !string.IsNullOrEmpty(x.ItemCode)).Select(y => y.StatusAlmacen));

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

        private List<InvoiceHeaderAdvancedLookUp> GetIsPackageDistribution(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliveryDetails, List<PackageModel> packages, List<Repartidores> repatridores, List<UserModel> users)
        {
            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                return this.GetDistributionCardFromSale(tuple, userOrders, lineProducts, invoiceHeadersToLook, invoiceDetailsToLook, packages, repatridores, users);
            }

            if (tuple.Item2 == ServiceConstants.Delivery)
            {
                return this.GetDistributionCardFromDelivery(tuple, userOrders, lineProducts, invoiceHeadersToLook, invoiceDetailsToLook, packages, repatridores, users);
            }

            if (tuple.Item2 == ServiceConstants.Invoice)
            {
                return this.GetDistributionCardFromInvoice(tuple, userOrders, lineProducts, invoiceHeadersToLook, invoiceDetailsToLook, packages, deliveryDetails, repatridores, users);
            }

            return new List<InvoiceHeaderAdvancedLookUp>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromSale(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<PackageModel> packages, List<Repartidores> repatridores, List<UserModel> users)
        {
            var invoiceFromSaleMag = userOrders.FirstOrDefault(x => x.Salesorderid == tuple.Item1.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId != 0);
            var invoicefromSaleLine = lineProducts.FirstOrDefault(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId != 0);

            if (invoiceFromSaleMag == null && invoicefromSaleLine == null)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            var invoice = invoiceFromSaleMag != null ? invoiceFromSaleMag.InvoiceId : invoicefromSaleLine.InvoiceId;
            var userOrder = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == invoice).ToList();
            var lineProductOrder = lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == invoice).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));
            var invoiceByOrder = userOrder.FirstOrDefault(x => x.Salesorderid == tuple.Item1.ToString());
            var card = this.GenerateCardForDistribution(invoice, invoiceByOrder, invoiceHeadersToLook, invoiceDetailsToLook, new List<DeliveryDetailModel>(), packages, false, repatridores, users);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromDelivery(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<PackageModel> packages, List<Repartidores> repatridores, List<UserModel> users)
        {
            var invoiceFromSaleMag = userOrders.FirstOrDefault(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId != 0);
            var invoicefromSaleLine = lineProducts.FirstOrDefault(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId != 0);

            if (invoiceFromSaleMag == null && invoicefromSaleLine == null)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            var invoice = invoiceFromSaleMag != null ? invoiceFromSaleMag.InvoiceId : invoicefromSaleLine.InvoiceId;
            var userOrder = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == invoice).ToList();
            var lineProductOrder = lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == invoice).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));
            var card = this.GenerateCardForDistribution(invoice, userOrder.FirstOrDefault(), invoiceHeadersToLook, invoiceDetailsToLook, new List<DeliveryDetailModel>(), packages, false, repatridores, users);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromInvoice(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<InvoiceHeaderModel> invoiceHeadersToLook, List<InvoiceDetailModel> invoiceDetailsToLook, List<PackageModel> packages, List<DeliveryDetailModel> deliveryDdetails, List<Repartidores> repatridores, List<UserModel> users)
        {
            var userOrder = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == tuple.Item1).ToList();
            var lineProductOrder = lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == tuple.Item1).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));
            var card = this.GenerateCardForDistribution(tuple.Item1, userOrder.FirstOrDefault(), invoiceHeadersToLook, invoiceDetailsToLook, deliveryDdetails, packages, true, repatridores, users);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private InvoiceHeaderAdvancedLookUp GenerateCardForDistribution(int invoiceId, UserOrderModel userOrder, List<InvoiceHeaderModel> invoiceHeader, List<InvoiceDetailModel> invoiceDetails, List<DeliveryDetailModel> deliveryDetails, List<PackageModel> packages, bool isFromInvoice, List<Repartidores> repatridores, List<UserModel> users)
        {
            var invoice = invoiceHeader.FirstOrDefault(x => x.DocNum == invoiceId);
            invoice ??= new InvoiceHeaderModel();

            var localInvoiceDetails = invoiceDetails.Where(x => x.InvoiceId == invoice.InvoiceId).ToList();
            var totalSales = deliveryDetails.Where(y => y.InvoiceId.HasValue && y.InvoiceId == invoice.InvoiceId).Select(x => x.BaseEntry).Distinct().ToList();
            var package = packages.FirstOrDefault(x => x.InvoiceId == invoiceId);
            package ??= new PackageModel { AssignedUser = string.Empty, Status = userOrder.StatusInvoice };

            var company = repatridores.FirstOrDefault(x => x.TrnspCode == invoice.TransportCode);

            var card = new InvoiceHeaderAdvancedLookUp
            {
                Invoice = invoiceId,
                DeliverId = !isFromInvoice ? userOrder.DeliveryId : 0,
                SalesOrder = !isFromInvoice ? int.Parse(userOrder.Salesorderid) : totalSales.Count,
                StatusDelivery = userOrder.StatusInvoice,
                Address = invoice.Address.Replace("\r", string.Empty),
                ProductType = invoice.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                Doctor = invoice.Medico,
                TotalDeliveries = localInvoiceDetails.Select(x => x.BaseEntry).Distinct().Count(),
                InvoiceDocDate = userOrder.InvoiceStoreDate.Value,
                Client = invoice.Cliente,
                TotalProducts = localInvoiceDetails.Count,
                DeliveredBy = this.GetDeliveredBy(userOrder.StatusInvoice, package, users),
                ReasonNotDelivered = package != null && package.Status == ServiceConstants.NoEntregado ? package.ReasonNotDelivered : string.Empty,
                DataCheckin = this.CalculateDistributioDate(userOrder.StatusInvoice, package, userOrder),
                IsLookUpInvoices = isFromInvoice,
                TrakingNumber = invoice.TrackingNumber,
                DeliveryCompany = company == null ? string.Empty : company.TrnspName,
                CodeClient = invoice.CardCode,
            };

            return card;
        }

        private DateTime CalculateDistributioDate(string status, PackageModel package, UserOrderModel order)
        {
            if (package == null)
            {
                return order.InvoiceStoreDate.Value;
            }

            switch (status)
            {
                case ServiceConstants.Empaquetado:
                case ServiceConstants.Enviado:
                    return order.InvoiceStoreDate.Value;

                case ServiceConstants.Asignado:
                    return package.AssignedDate.Value;

                case ServiceConstants.Camino:
                    return package.InWayDate.Value;

                case ServiceConstants.NoEntregado:
                case ServiceConstants.Entregado:
                    return package.DeliveredDate.Value;

                default:
                    return order.InvoiceStoreDate.Value;
            }
        }

        private string GetDeliveredBy(string status, PackageModel package, List<UserModel> users)
        {
            switch (status)
            {
                case ServiceConstants.Asignado:
                    var user = users.FirstOrDefault(x => x.Id == package.AssignedUser);
                    return user == null ? string.Empty : $"{user.FirstName} {user.LastName}";

                case ServiceConstants.Entregado:
                    return package == null ? string.Empty : package.Comments;

                default:
                    return string.Empty;
            }
        }
    }
}
