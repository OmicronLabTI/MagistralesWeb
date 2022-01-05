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
    using System.Text;
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

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceLookService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="usersService">The user servie.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        public AdvanceLookService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, IUsersService usersService, ICatalogsService catalogsService, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.usersService = usersService.ThrowIfNull(nameof(usersService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
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
            var userOrdersResponse = await this.pedidosService.PostPedidos(listDocs, ServiceConstants.AdvanceLookId);
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
            var userOrdersResponse = await this.pedidosService.PostPedidos(listDocs, ServiceConstants.AdvanceLookId);
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
            var sapSaleOrder = (await this.sapDao.GetAllOrdersWIthDetailByIdsJoinProduct(tupleIds.Where(x => x.Item2 == ServiceConstants.SaleOrder || x.Item2 == ServiceConstants.DontExistsTable).Select(y => y.Item1).ToList())).ToList();
            var sapDeliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(tupleIds.Where(x => x.Item2 == ServiceConstants.Delivery).Select(y => y.Item1).ToList())).ToList();
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryDetailBySaleOrderJoinProduct(sapSaleOrder.Select(x => x.DocNum).ToList()));
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(sapDeliveryDetails.Select(x => x.DeliveryId).ToList()));
            var sapDelivery = (await this.sapDao.GetDeliveryModelByDocNumJoinDoctor(sapDeliveryDetails.Select(y => y.DeliveryId).ToList())).ToList();

            var sapInvoicesHeaders = (await this.sapDao.GetInvoiceHeadersByDocNumJoinDoctor(tupleIds.Where(x => x.Item2 == ServiceConstants.Invoice || x.Item2 == ServiceConstants.DontExistsTable).Select(y => y.Item1).ToList())).ToList();
            var sapInvoicesDeatils = (await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(sapInvoicesHeaders.Select(x => x.InvoiceId).ToList())).ToList();
            sapInvoicesDeatils.AddRange(await this.sapDao.GetInvoiceDetailByBaseEntryJoinProduct(sapDelivery.Select(x => x.DocNum).ToList()));
            sapInvoicesHeaders.AddRange(await this.sapDao.GetInvoiceHeaderByInvoiceIdJoinDoctor(sapInvoicesDeatils.Select(x => x.InvoiceId).ToList()));

            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);
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
            sapInvoicesHeaders.AddRange(await this.sapDao.GetInvoiceHeaderByInvoiceIdJoinDoctor(listInvoicedId));
            sapInvoicesDeatils.AddRange(await this.sapDao.GetInvoiceDetailByDocEntryJoinProduct(sapInvoicesHeaders.Select(x => x.InvoiceId).ToList()));

            sapInvoicesDeatils.GroupBy(x => new { x.BaseEntry, x.InvoiceId }).ToList().ForEach(x =>
            {
                temporalsapInvoicesDeatils.AddRange(x.DistinctBy(d => d.ProductoId));
            });
            sapInvoicesDeatils = temporalsapInvoicesDeatils;
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(sapInvoicesDeatils.Where(x => x.BaseEntry.HasValue).Select(x => x.BaseEntry.Value).ToList()));
            sapDeliveryDetails.GroupBy(x => new { x.DeliveryId, x.BaseEntry }).ToList().ForEach(x =>
            {
                temporalsapDeliveryDetails.AddRange(x.DistinctBy(d => d.ProductoId));
            });
            sapDeliveryDetails = temporalsapDeliveryDetails;

            var listIds = sapDeliveryDetails.Select(x => x.DeliveryId).ToList();
            listIds.AddRange(sapInvoicesHeaders.Select(x => x.DocNum));
            var userOrdersResponse = await this.pedidosService.PostPedidos(listIds, ServiceConstants.AdvanceLookId);
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

            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);

            var objectCardOrder = new ParamentsCards
            {
                UserOrders = userOrders,
                LineProducts = almacenData.LineProducts,
                OrderDetail = sapSaleOrder,
                DeliveryDetails = sapDeliveryDetails,
                ProductModel = lineProducts,
                DeliveryHeader = sapDelivery,
                Cancellations = almacenData.CancelationModel,
                InvoiceHeaders = sapInvoicesHeaders,
                InvoiceDetailsToLook = sapInvoicesDeatils,
                Packages = almacenData.PackageModels,
                Repatridores = deliveryCompanies,
                Users = users,
                UserOrdersForDelivery = userOrdersForDelivery,
                LocalNeighbors = localNeigbors,
            };

            tupleIds.ForEach(order =>
            {
                cardToReturns.CardOrder.AddRange(this.GetIsReceptionOrders(order, objectCardOrder));
                cardToReturns.CardOrder.AddRange(this.GetIsReceptionOrdersSample(order, objectCardOrder));
                cardToReturns.CardDelivery.AddRange(this.GetIsReceptionDelivery(order, objectCardOrder));
                cardToReturns.CardInvoice.AddRange(this.GetIsPackageInvoice(order, objectCardOrder));
                cardToReturns.CardDistribution.AddRange(this.GetIsPackageDistribution(order, objectCardOrder));
            });

            return cardToReturns;
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionOrders(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var listItemCode = paramentsCards.ProductModel;
            var orderbyDocNum = paramentsCards.OrderDetail.Where(x => x.DocNum == tuple.Item1 && (x.PedidoStatus == "O" || x.Canceled == "Y") && x.Detalles != null).ToList();
            var isLineSale = tuple.Item2 == ServiceConstants.DontExistsTable && orderbyDocNum.Any() && orderbyDocNum.All(x => listItemCode.Contains(x.Detalles.ProductoId));
            if (tuple.Item2 != ServiceConstants.SaleOrder && !isLineSale)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var existanceMag = paramentsCards.UserOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString());
            existanceMag ??= new UserOrderModel { Status = string.Empty };
            var existanceLin = paramentsCards.LineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1);
            existanceLin ??= new LineProductsModel { StatusAlmacen = string.Empty };

            if (ServiceConstants.StatusToIgnoreUserOrderAdvancedLook.Contains(existanceMag.Status) || ServiceConstants.StatusToIgnoreLineOrderAdvancedLook.Contains(existanceLin.StatusAlmacen))
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var userOrder = paramentsCards.UserOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString() && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)));
            var lineProductOrder = paramentsCards.LineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1 && ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen));
            paramentsCards.UserOrder = userOrder;
            paramentsCards.LineProductOrder = lineProductOrder;

            return this.GenerateCardForReceptionOrders(tuple, paramentsCards);
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionOrdersSample(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            if (tuple.Item2 != ServiceConstants.SaleOrder)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var existanceMag = paramentsCards.UserOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString());
            existanceMag ??= new UserOrderModel { Status = ServiceConstants.Almacenado };
            var existanceLin = paramentsCards.LineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1);
            existanceLin ??= new LineProductsModel { StatusAlmacen = ServiceConstants.Almacenado };

            if (existanceMag.Status != ServiceConstants.Almacenado || existanceLin.StatusAlmacen != ServiceConstants.Almacenado)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var userOrder = paramentsCards.UserOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString() && x.Status == ServiceConstants.Almacenado && x.StatusAlmacen == ServiceConstants.Almacenado);
            var lineProductOrder = paramentsCards.LineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1 && x.StatusAlmacen == ServiceConstants.Almacenado);
            paramentsCards.UserOrder = userOrder;
            paramentsCards.LineProductOrder = lineProductOrder;

            return this.GenerateCardForReceptionOrders(tuple, paramentsCards);
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionOrders(Tuple<int, string> tuple, ParamentsCards paramentsCards)
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
            var userOrder = paramentsCards.UserOrder;
            var lineProductOrder = paramentsCards.LineProductOrder;
            var comments = new StringBuilder();

            if (userOrder != null)
            {
                var userOrders = paramentsCards.UserOrders.Where(x => x.Salesorderid == tuple.Item1.ToString()).ToList();
                saporders = paramentsCards.OrderDetail.Where(x => x.DocNum.ToString() == userOrder.Salesorderid).ToList();
                order = saporders.FirstOrDefault();
                status = ServiceConstants.StatusForBackOrder.Contains(userOrder.Status) && userOrder.StatusAlmacen == ServiceConstants.BackOrder ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;
                status = userOrder.Status != ServiceConstants.Finalizado && userOrder.Status != ServiceConstants.Almacenado && status != ServiceConstants.BackOrder ? ServiceConstants.Pendiente : status;
                status = userOrder.Status == ServiceConstants.Almacenado && order.PedidoMuestra == ServiceConstants.IsSampleOrder ? ServiceConstants.Almacenado : status;
                productType = saporders.Any(x => x.Detalles != null && paramentsCards.ProductModel.Any(p => p == x.Detalles.ProductoId)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                porRecibirDate = userOrder.CloseDate ?? porRecibirDate;
                comments.Append($"{userOrder.Comments}&");
                hasCandidate = this.CalulateIfSaleOrderIsCandidate(userOrders, userOrder.Status, order, userOrder);
            }

            var hasDeliveries = paramentsCards.DeliveryDetails.Where(x => x.BaseEntry == tuple.Item1).Select(x => x.DeliveryId).ToList();
            var hasActiveDeliveries = hasDeliveries.Any() && paramentsCards.DeliveryHeader.Where(x => hasDeliveries.Contains(x.DocNum)).Any(y => y.Canceled != "Y");

            if (userOrder == null && (lineProductOrder != null || (lineProductOrder == null && !hasActiveDeliveries)))
            {
                saporders = paramentsCards.OrderDetail.Where(x => x.DocNum == tuple.Item1).ToList();
                order = saporders.FirstOrDefault();
                status = lineProductOrder == null ? ServiceConstants.PorRecibir : lineProductOrder.StatusAlmacen;
                status = lineProductOrder != null && lineProductOrder.StatusAlmacen == ServiceConstants.Recibir ? ServiceConstants.PorRecibir : status;
                status = lineProductOrder != null && lineProductOrder.StatusAlmacen == ServiceConstants.Almacenado && order.PedidoMuestra == ServiceConstants.IsSampleOrder ? ServiceConstants.Almacenado : status;
                productType = ServiceConstants.Linea;
                porRecibirDate = order.FechaInicio != null ? DateTime.ParseExact(order.FechaInicio, "dd/MM/yyyy", null) : porRecibirDate;
                hasCandidate = this.CalculateIfLineOrderIsCandidate(lineProductOrder, status, order);
            }

            if (!hasCandidate)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            invoiceType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, paramentsCards.LocalNeighbors, order.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo;
            totalItems = saporders.Count;
            totalPieces = (int)saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
            initDate = order.FechaInicio != null ? DateTime.ParseExact(order.FechaInicio, "dd/MM/yyyy", null) : initDate;

            var saleHeader = new AlmacenSalesHeaderModel
            {
                DocNum = order.DocNum,
                Status = order.Canceled == "Y" ? ServiceConstants.Cancelado : status,
                TypeSaleOrder = $"Pedido {productType}",
                Doctor = order.Medico,
                InvoiceType = invoiceType,
                TotalItems = totalItems,
                InitDate = initDate,
                Client = order.Cliente,
                TotalPieces = totalPieces,
                DataCheckin = porRecibirDate,
                OrderMuestra = string.IsNullOrEmpty(order.PedidoMuestra) ? ServiceConstants.IsNotSampleOrder : order.PedidoMuestra,
                Comments = comments.ToString(),
                SapComments = order.Comments,
                TypeOrder = order.OrderType,
            };

            return new List<AlmacenSalesHeaderModel> { saleHeader };
        }

        private bool CalulateIfSaleOrderIsCandidate(List<UserOrderModel> userOrders, string statusOrder, CompleteOrderModel order, UserOrderModel saleOrder)
        {
            var hasCandidate = false;

            if (!string.IsNullOrEmpty(saleOrder.StatusAlmacen) && saleOrder.StatusAlmacen == ServiceConstants.BackOrder)
            {
                return true;
            }

            if (statusOrder == ServiceConstants.Finalizado)
            {
                var localOrders = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Status != ServiceConstants.Cancelado).ToList();
                hasCandidate = localOrders.All(x => (x.Status == ServiceConstants.Finalizado || x.Status == ServiceConstants.Almacenado) && x.FinishedLabel == 1) && !localOrders.All(x => x.Status == ServiceConstants.Almacenado && x.FinishedLabel == 1);
                hasCandidate = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(y => y.Status == ServiceConstants.Cancelado) ? true : hasCandidate;
                return hasCandidate;
            }

            if (statusOrder == ServiceConstants.Liberado)
            {
                var localOrders = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Status != ServiceConstants.Cancelado).ToList();
                var orderWithStatusAndLabel = localOrders.Where(x => x.Status == ServiceConstants.Almacenado || x.Status == ServiceConstants.Finalizado).All(y => y.FinishedLabel == 1);
                hasCandidate = localOrders.All(x => (x.Status == ServiceConstants.Almacenado || x.Status == ServiceConstants.Finalizado || x.Status == ServiceConstants.Pendiente)) && orderWithStatusAndLabel;
                var areAllPending = localOrders.All(x => x.Status == ServiceConstants.Pendiente);
                hasCandidate = areAllPending ? false : hasCandidate;
                return hasCandidate;
            }

            if (statusOrder == ServiceConstants.Almacenado && !string.IsNullOrEmpty(order.PedidoMuestra) && order.PedidoMuestra == ServiceConstants.IsSampleOrder)
            {
                return true;
            }

            return hasCandidate;
        }

        private bool CalculateIfLineOrderIsCandidate(LineProductsModel lineProduct, string statusOrder, CompleteOrderModel order)
        {
            var statusToValidate = lineProduct == null ? statusOrder : lineProduct.StatusAlmacen;
            switch (statusToValidate)
            {
                case ServiceConstants.Almacenado:
                    return !string.IsNullOrEmpty(order.PedidoMuestra) && order.PedidoMuestra == ServiceConstants.IsSampleOrder;

                default:
                    return true;
            }
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionDelivery(Tuple<int, string> tuple, ParamentsCards paramsCard)
        {
            if (tuple.Item2 == ServiceConstants.Invoice)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrdersFromSale = paramsCard.UserOrders.Where(x => x.Salesorderid == tuple.Item1.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromSale = paramsCard.LineProducts.Where(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var deliveryIdsBySale = userOrdersFromSale.Select(x => x.DeliveryId).ToList();
                deliveryIdsBySale.AddRange(lineProductsFromSale.Select(y => y.DeliveryId));

                var paramsCardDelivery = new ParametersCardDelivery()
                {
                    PossibleDeliveries = deliveryIdsBySale,
                    DeliveryDetailModels = paramsCard.DeliveryDetails,
                    DeliveryHeaders = paramsCard.DeliveryHeader,
                    LineSapProducts = paramsCard.ProductModel,
                    UserOrders = userOrdersFromSale,
                    LineProducts = lineProductsFromSale,
                    InvoiceHeaders = paramsCard.InvoiceHeaders,
                    LocalNeighbors = paramsCard.LocalNeighbors,
                };
                return this.GenerateCardForReceptionDelivery(paramsCardDelivery);
            }

            var isCancelled = paramsCard.Cancellations != null && paramsCard.Cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (tuple.Item2 == ServiceConstants.Delivery && !isCancelled)
            {
                var userOrdersFromDelivery = paramsCard.UserOrders.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromDelivery = paramsCard.LineProducts.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var delivryIds = new List<int> { tuple.Item1 };

                var paramsCardDelivery = new ParametersCardDelivery()
                {
                    PossibleDeliveries = delivryIds,
                    DeliveryDetailModels = paramsCard.DeliveryDetails,
                    DeliveryHeaders = paramsCard.DeliveryHeader,
                    LineSapProducts = paramsCard.ProductModel,
                    UserOrders = userOrdersFromDelivery,
                    LineProducts = lineProductsFromDelivery,
                    InvoiceHeaders = paramsCard.InvoiceHeaders,
                    LocalNeighbors = paramsCard.LocalNeighbors,
                };
                return this.GenerateCardForReceptionDelivery(paramsCardDelivery);
            }

            if (tuple.Item2 == ServiceConstants.Delivery && isCancelled)
            {
                var delivryIds = new List<int> { tuple.Item1 };
                var cancelation = paramsCard.Cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                var cancelledOrder = new List<UserOrderModel> { new UserOrderModel { DeliveryId = tuple.Item1, DateTimeCheckIn = cancelation.CancelDate, StatusAlmacen = ServiceConstants.Cancelado } };

                var paramsCardDelivery = new ParametersCardDelivery()
                {
                    PossibleDeliveries = delivryIds,
                    DeliveryDetailModels = paramsCard.DeliveryDetails,
                    DeliveryHeaders = paramsCard.DeliveryHeader,
                    LineSapProducts = paramsCard.ProductModel,
                    UserOrders = cancelledOrder,
                    LineProducts = new List<LineProductsModel>(),
                    InvoiceHeaders = new List<InvoiceHeaderModel>(),
                    LocalNeighbors = paramsCard.LocalNeighbors,
                };
                return this.GenerateCardForReceptionDelivery(paramsCardDelivery);
            }

            return new List<AlmacenSalesHeaderModel>();
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionDelivery(ParametersCardDelivery paramsCardDelivery)
        {
            var saleHeader = new List<AlmacenSalesHeaderModel>();
            var possibleDeliveries = paramsCardDelivery.PossibleDeliveries;
            var deliveryDetailModels = paramsCardDelivery.DeliveryDetailModels;
            var invoiceHeaders = paramsCardDelivery.InvoiceHeaders;
            var deliveryHeaders = paramsCardDelivery.DeliveryHeaders;
            var userOrders = paramsCardDelivery.UserOrders;
            var lineProducts = paramsCardDelivery.LineProducts;
            var lineSapProducts = paramsCardDelivery.LineSapProducts;

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
                var deliveryDetail = deliveryDetailModels.Where(x => x.DeliveryId == delivery).ToList();

                var salesOrders = deliveryDetail.Select(x => x.BaseEntry).Distinct().OrderByDescending(y => y).ToList();

                var listSalesOrders = new StringBuilder();
                salesOrders.ForEach(x => listSalesOrders.Append($" {x},"));
                listSalesOrders.Remove(listSalesOrders.Length - 1, 1);

                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);
                var invoiceType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, paramsCardDelivery.LocalNeighbors, header.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo;
                var productType = this.GenerateListProductTypeDelivery(deliveryDetail, lineSapProducts);

                var userOrderByDelivery = userOrders.FirstOrDefault(x => x.DeliveryId == delivery);
                var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.DeliveryId == delivery);
                var initDate = userOrderByDelivery != null ? userOrderByDelivery.DateTimeCheckIn : lineProductByDelivery.DateCheckIn;
                var status = userOrderByDelivery != null ? userOrderByDelivery.StatusAlmacen : lineProductByDelivery.StatusAlmacen;

                var saleHeaderItem = new AlmacenSalesHeaderModel
                {
                    Client = header.Cliente,
                    DocNum = salesOrders.Count,
                    Doctor = header.Medico,
                    InitDate = header.FechaInicio,
                    Status = status,
                    Remision = delivery,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
                    DataCheckin = initDate.Value,
                    ListSaleOrder = listSalesOrders.ToString(),
                    TypeOrder = header.TypeOrder,
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
                    var orders = (await this.sapDao.GetOrderModelByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    orders = orders.Where(x => !string.IsNullOrEmpty(x.Medico) && doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return orders.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Delivery:
                    var deliveries = (await this.sapDao.GetDeliveryByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    deliveries = deliveries.Where(x => !string.IsNullOrEmpty(x.Medico) && doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return deliveries.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Invoice:
                    var invoices = (await this.sapDao.GetInvoiceHeadersByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    invoices = invoices.Where(x => !string.IsNullOrEmpty(x.Medico) && doctorValue.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
                    return invoices.Select(x => x.DocNum).Distinct().ToList();
            }

            return new List<int>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GetIsPackageInvoice(Tuple<int, string> tuple, ParamentsCards paramsCard)
        {
            var userOrders = paramsCard.UserOrdersForDelivery;
            var lineProducts = paramsCard.LineProducts;

            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrder = userOrders.Where(x => (x.Salesorderid == tuple.Item1.ToString()) && (!string.IsNullOrEmpty(x.Productionorderid)) && (x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado) && (x.DeliveryId != 0)).ToList();
                var lineProductOrder = lineProducts.Where(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && (x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado) && x.DeliveryId != 0).ToList();
                var deliveryIdsBySale = userOrder.Select(x => x.DeliveryId).ToList();
                deliveryIdsBySale.AddRange(lineProductOrder.Select(y => y.DeliveryId));

                var paramsCardInvoice = new ParametersCardInvoice()
                {
                    ListDeliveryId = deliveryIdsBySale,
                    UserOrders = userOrders,
                    LineProducts = lineProducts,
                    DeliveryDetailModels = paramsCard.DeliveryDetails,
                    InvoiceHeadersToLook = paramsCard.InvoiceHeaders,
                    InvoiceDetailsToLook = paramsCard.InvoiceDetailsToLook,
                    LocalNeighbors = paramsCard.LocalNeighbors,
                };
                return this.GenerateCardForPackageInvoiceTheSalesOrder(paramsCardInvoice);
            }

            var isCancelled = paramsCard.Cancellations != null && paramsCard.Cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (tuple.Item2 == ServiceConstants.Delivery && !isCancelled)
            {
                var delivryIds = new List<int> { tuple.Item1 };

                var paramsCardInvoice = new ParametersCardInvoice()
                {
                    ListDeliveryId = delivryIds,
                    UserOrders = userOrders,
                    LineProducts = lineProducts,
                    DeliveryDetailModels = paramsCard.DeliveryDetails,
                    InvoiceHeadersToLook = paramsCard.InvoiceHeaders,
                    InvoiceDetailsToLook = paramsCard.InvoiceDetailsToLook,
                    LocalNeighbors = paramsCard.LocalNeighbors,
                };
                return this.GenerateCardForPackageInvoiceTheSalesOrder(paramsCardInvoice);
            }

            if (tuple.Item2 == ServiceConstants.Invoice && !isCancelled)
            {
                var invoice = paramsCard.InvoiceHeaders.FirstOrDefault(x => x.DocNum == tuple.Item1);
                invoice ??= new InvoiceHeaderModel();
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoice, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, paramsCard.LocalNeighbors);
            }

            if (tuple.Item2 == ServiceConstants.Invoice && isCancelled)
            {
                var cancelation = paramsCard.Cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                return this.GenerateCardForPackageInvoiceCancelled(paramsCard.InvoiceHeaders, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, cancelation, paramsCard.LocalNeighbors);
            }

            if ((tuple.Item2 == ServiceConstants.DontExistsTable) && (userOrders.Any() || lineProducts.Any()) && !isCancelled)
            {
                var invoice = paramsCard.InvoiceHeaders.FirstOrDefault(x => x.DocNum == tuple.Item1);
                invoice ??= new InvoiceHeaderModel();
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoice, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, paramsCard.LocalNeighbors);
            }

            return new List<InvoiceHeaderAdvancedLookUp>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoiceCancelled(List<InvoiceHeaderModel> invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder, CancellationResourceModel canceled, List<string> localNeighbors)
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
                Address = invoiceHeader.Address.Replace("\r", string.Empty).ToUpper(),
                Client = invoiceHeader.Cliente,
                Doctor = invoiceHeader.Medico ?? string.Empty,
                Invoice = invoiceHeader.DocNum,
                DocEntry = invoiceHeader.InvoiceId,
                InvoiceDocDate = invoiceHeader.FechaInicio,
                ProductType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeighbors, invoiceHeader.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                TotalDeliveries = invoiceDetail.DistinctBy(x => x.BaseEntry.Value).Count(),
                TotalProducts = totalProducts,
                StatusDelivery = ServiceConstants.Cancelado,
                DataCheckin = canceled.CancelDate,
                SalesOrder = deliverysToLookSaleOrder.DistinctBy(x => x.BaseEntry).Count(),
                IsLookUpInvoices = true,
                IsRefactura = false,
                TypeOrder = invoiceHeader.TypeOrder,
            };
            invoicesHeaders.Add(invoiceHeaderLookUp);
            return invoicesHeaders;
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoiceTheSalesOrder(ParametersCardInvoice paramsCardInvoice)
        {
            var invoicesHeaders = new List<InvoiceHeaderAdvancedLookUp>();
            var listDeliveryId = paramsCardInvoice.ListDeliveryId;
            var deliveryDetailModels = paramsCardInvoice.DeliveryDetailModels;
            var invoiceHeadersToLook = paramsCardInvoice.InvoiceHeadersToLook;
            var invoiceDetailsToLook = paramsCardInvoice.InvoiceDetailsToLook;
            var userOrders = paramsCardInvoice.UserOrders;
            var lineProducts = paramsCardInvoice.LineProducts;

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
                    var deliveryDetails = deliveryDetailModels.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();

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
                            Address = invoiceHeader.Address.Replace("\r", string.Empty).ToUpper(),
                            Client = invoiceHeader.Cliente,
                            Doctor = invoiceHeader.Medico ?? string.Empty,
                            Invoice = invoiceHeader.DocNum,
                            DocEntry = invoiceHeader.InvoiceId,
                            InvoiceDocDate = invoiceHeader.FechaInicio,
                            ProductType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, paramsCardInvoice.LocalNeighbors, invoiceHeader.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                            TotalDeliveries = deliverys.Count,
                            TotalProducts = totalProducts,
                            DeliverId = delivery,
                            SalesOrder = deliveryHeader.SaleOrder,
                            StatusDelivery = deliveryHeader.Status,
                            DataCheckin = initDate,
                            IsLookUpInvoices = false,
                            IsRefactura = !string.IsNullOrEmpty(invoiceHeader.Refactura) && invoiceHeader.Refactura == ServiceConstants.IsRefactura,
                            TypeOrder = invoiceHeader.TypeOrder,
                        };
                        invoicesHeaders.Add(invoiceHeaderLookUp);
                    }
                }
            }

            return invoicesHeaders;
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoice(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, InvoiceHeaderModel invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder, List<string> localNeigbors)
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
                        Address = invoiceHeaders.Address.Replace("\r", string.Empty).ToUpper(),
                        Client = invoiceHeaders.Cliente,
                        Doctor = invoiceHeaders.Medico ?? string.Empty,
                        Invoice = invoiceHeaders.DocNum,
                        DocEntry = invoiceHeaders.InvoiceId,
                        InvoiceDocDate = invoiceHeaders.FechaInicio,
                        ProductType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, invoiceHeaders.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                        TotalDeliveries = deliverys.Count,
                        TotalProducts = totalProducts,
                        StatusDelivery = ServiceConstants.Almacenado,
                        DataCheckin = initDate,
                        SalesOrder = deliverys.DistinctBy(x => x.SaleOrder).Count(),
                        IsLookUpInvoices = true,
                        IsRefactura = !string.IsNullOrEmpty(invoiceHeaders.Refactura) && invoiceHeaders.Refactura == ServiceConstants.IsRefactura,
                        TypeOrder = invoiceHeaders.TypeOrder,
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
                        SaleOrder = y.BaseEntry ?? 0,
                        Status = userOrderStatus.Any() && userOrderStatus.All(z => z == ServiceConstants.Empaquetado) ? ServiceConstants.Empaquetado : ServiceConstants.Almacenado,
                        TotalItems = invoiceDetails.Where(a => a.BaseEntry.HasValue).Count(z => z.BaseEntry == y.DeliveryId),
                    };

                    listToReturn.Add(deliveryModel);
                });

            return listToReturn;
        }

        private List<InvoiceHeaderAdvancedLookUp> GetIsPackageDistribution(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                return this.GetDistributionCardFromSale(tuple, paramentsCards);
            }

            if (tuple.Item2 == ServiceConstants.Delivery)
            {
                return this.GetDistributionCardFromDelivery(tuple, paramentsCards);
            }

            if (tuple.Item2 == ServiceConstants.Invoice)
            {
                return this.GetDistributionCardFromInvoice(tuple, paramentsCards);
            }

            return new List<InvoiceHeaderAdvancedLookUp>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromSale(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var invoiceFromSaleMag = paramentsCards.UserOrders.FirstOrDefault(x => x.Salesorderid == tuple.Item1.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId != 0);
            var invoicefromSaleLine = paramentsCards.LineProducts.FirstOrDefault(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId != 0);

            if (invoiceFromSaleMag == null && invoicefromSaleLine == null)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            var invoice = invoiceFromSaleMag != null ? invoiceFromSaleMag.InvoiceId : invoicefromSaleLine.InvoiceId;
            var userOrder = paramentsCards.UserOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == invoice).ToList();
            var lineProductOrder = paramentsCards.LineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == invoice).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));
            var invoiceByOrder = userOrder.FirstOrDefault(x => x.Salesorderid == tuple.Item1.ToString());

            var objectForDistribution = new ParametersCardForDistribution
            {
                InvoiceId = invoice,
                UserOrder = invoiceByOrder,
                InvoiceHeader = paramentsCards.InvoiceHeaders,
                InvoiceDetails = paramentsCards.InvoiceDetailsToLook,
                DeliveryDetails = new List<DeliveryDetailModel>(),
                Packages = paramentsCards.Packages,
                IsFromInvoice = false,
                Repatridores = paramentsCards.Repatridores,
                Users = paramentsCards.Users,
                LocalNeighbors = paramentsCards.LocalNeighbors,
            };

            var card = this.GenerateCardForDistribution(objectForDistribution);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromDelivery(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var invoiceFromSaleMag = paramentsCards.UserOrders.FirstOrDefault(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId != 0);
            var invoicefromSaleLine = paramentsCards.LineProducts.FirstOrDefault(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId != 0);

            if (invoiceFromSaleMag == null && invoicefromSaleLine == null)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            var invoice = invoiceFromSaleMag != null ? invoiceFromSaleMag.InvoiceId : invoicefromSaleLine.InvoiceId;
            var userOrder = paramentsCards.UserOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == invoice).ToList();
            var lineProductOrder = paramentsCards.LineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == invoice).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));
            var deliveryByTuple = userOrder.FirstOrDefault(x => x.DeliveryId == tuple.Item1);
            deliveryByTuple ??= userOrder.FirstOrDefault();

            var objectForDistribution = new ParametersCardForDistribution
            {
                InvoiceId = invoice,
                UserOrder = deliveryByTuple,
                InvoiceHeader = paramentsCards.InvoiceHeaders,
                InvoiceDetails = paramentsCards.InvoiceDetailsToLook,
                DeliveryDetails = new List<DeliveryDetailModel>(),
                Packages = paramentsCards.Packages,
                IsFromInvoice = false,
                Repatridores = paramentsCards.Repatridores,
                Users = paramentsCards.Users,
                LocalNeighbors = paramentsCards.LocalNeighbors,
            };

            var card = this.GenerateCardForDistribution(objectForDistribution);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromInvoice(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var userOrder = paramentsCards.UserOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.InvoiceId == tuple.Item1).ToList();
            var lineProductOrder = paramentsCards.LineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode) && x.InvoiceId == tuple.Item1).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => x.StatusAlmacen != ServiceConstants.Almacenado && !string.IsNullOrEmpty(x.StatusInvoice));

            if (!hasPossilbeMag && !hasPossibleLine)
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString() }));

            var objectForDistribution = new ParametersCardForDistribution
            {
                InvoiceId = tuple.Item1,
                UserOrder = userOrder.FirstOrDefault(),
                InvoiceHeader = paramentsCards.InvoiceHeaders,
                InvoiceDetails = paramentsCards.InvoiceDetailsToLook,
                DeliveryDetails = paramentsCards.DeliveryDetails,
                Packages = paramentsCards.Packages,
                IsFromInvoice = true,
                Repatridores = paramentsCards.Repatridores,
                Users = paramentsCards.Users,
                LocalNeighbors = paramentsCards.LocalNeighbors,
            };

            var card = this.GenerateCardForDistribution(objectForDistribution);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private InvoiceHeaderAdvancedLookUp GenerateCardForDistribution(ParametersCardForDistribution parametersDistribution)
        {
            var invoiceId = parametersDistribution.InvoiceId;
            var userOrder = parametersDistribution.UserOrder;
            var isFromInvoice = parametersDistribution.IsFromInvoice;

            var invoice = parametersDistribution.InvoiceHeader.FirstOrDefault(x => x.DocNum == invoiceId);
            invoice ??= new InvoiceHeaderModel();

            var localInvoiceDetails = parametersDistribution.InvoiceDetails.Where(x => x.InvoiceId == invoice.InvoiceId).ToList();
            var totalSales = parametersDistribution.DeliveryDetails.Where(y => y.InvoiceId.HasValue && y.InvoiceId == invoice.InvoiceId).Select(x => x.BaseEntry).Distinct().ToList();
            var package = parametersDistribution.Packages.FirstOrDefault(x => x.InvoiceId == invoiceId);
            package ??= new PackageModel { AssignedUser = string.Empty, Status = userOrder.StatusInvoice };

            var company = parametersDistribution.Repatridores.FirstOrDefault(x => x.TrnspCode == invoice.TransportCode);

            var card = new InvoiceHeaderAdvancedLookUp
            {
                Invoice = invoiceId,
                DeliverId = !isFromInvoice ? userOrder.DeliveryId : 0,
                SalesOrder = !isFromInvoice ? int.Parse(userOrder.Salesorderid) : totalSales.Count,
                StatusDelivery = userOrder.StatusInvoice,
                Address = invoice.Address.Replace("\r", string.Empty).ToUpper(),
                ProductType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, parametersDistribution.LocalNeighbors, invoice.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo,
                Doctor = invoice.Medico,
                TotalDeliveries = localInvoiceDetails.Select(x => x.BaseEntry).Distinct().Count(),
                InvoiceDocDate = userOrder.InvoiceStoreDate.Value,
                Client = invoice.Cliente,
                TotalProducts = localInvoiceDetails.Count,
                DeliveredBy = this.GetDeliveredBy(userOrder.StatusInvoice, package, parametersDistribution.Users),
                DeliveryGuyName = package.Status == ServiceConstants.Entregado ? this.GetDeliveredGuyBy(package, parametersDistribution.Users) : string.Empty,
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

        private string GetDeliveredGuyBy(PackageModel package, List<UserModel> users)
        {
            var user = users.FirstOrDefault(x => x.Id == package.AssignedUser);
            return user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
        }
    }
}
