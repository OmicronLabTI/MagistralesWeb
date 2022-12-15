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
    using Omicron.SapAdapter.Dtos.DxpModels;
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

        private readonly IProccessPayments proccessPayments;

        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceLookService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="usersService">The user servie.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="doctorService">the doctor service.</param>
        public AdvanceLookService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, IUsersService usersService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments, IDoctorService doctorService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.usersService = usersService.ThrowIfNull(nameof(usersService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.proccessPayments = proccessPayments.ThrowIfNull(nameof(proccessPayments));
            this.doctorService = doctorService.ThrowIfNull(nameof(doctorService));
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
            var isDxpId = docNum.StartsWith(ServiceConstants.WildcardDocNumDxp);
            if (ServiceShared.CalculateAnd(intDocNum == 0, !isDxpId))
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
            if (isDxpId)
            {
                var dxpToLookUp = docNum.ToLower().Remove(0, 1);
                var ordersdxp = (await this.sapDao.GetAllOrdersWIthDetailByDocNumDxpJoinProduct(new List<string> { dxpToLookUp })).ToList();
                var salesId = ordersdxp.Select(o => o.DocNum).Distinct().ToList();
                listDocs.AddRange(salesId);
            }

            listDocs = listDocs.Where(id => id != 0).ToList();
            var userOrdersResponse = await this.pedidosService.PostPedidos(listDocs, ServiceConstants.AdvanceLookId);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listDocs);
            var almacenData = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            var tupleList = this.KindLookUp(userOrders, almacenData, listDocs);
            var response = await this.GetStatusToSearch(userOrders, almacenData, tupleList, docNum);
            return ServiceUtils.CreateResult(true, 200, null, response, null, null);
        }

        /// <summary>
        /// Gets the cards for look up by id.
        /// </summary>
        /// <param name="parameters">the docnum.</param>
        /// <returns>the data.</returns>
        private async Task<ResultModel> GetElementsByDoctor(Dictionary<string, string> parameters)
        {
            var doctorText = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Doctor, string.Empty);
            var doctorValue = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctorText), new List<string>(), doctorText.Split(",").ToList());
            var listDocs = await this.GetIdsToLookByDoctor(parameters);
            var userOrdersResponse = await this.pedidosService.PostPedidos(listDocs, ServiceConstants.AdvanceLookId);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listDocs);
            var almacenData = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            var tupleList = this.KindLookUp(userOrders, almacenData, listDocs);
            var response = await this.GetStatusToSearch(userOrders, almacenData, tupleList, string.Empty);

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

                if (ServiceShared.CalculateOr(userOrders.Any(x => x.Salesorderid == id.ToString()), adnvaceLookUp.LineProducts.Any(x => x.SaleOrderId == id)))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.SaleOrder));
                    match = true;
                }

                if (ServiceShared.CalculateOr(userOrders.Any(x => x.DeliveryId == id), adnvaceLookUp.LineProducts.Any(x => x.DeliveryId == id)))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.Delivery));
                    match = true;
                }

                if (ServiceShared.CalculateOr(userOrders.Any(x => x.InvoiceId == id), adnvaceLookUp.LineProducts.Any(x => x.InvoiceId == id)))
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.Invoice));
                    match = true;
                }

                if (adnvaceLookUp.CancelationModel != null && adnvaceLookUp.CancelationModel.Any(x => x.CancelledId == id))
                {
                    var cancelled = adnvaceLookUp.CancelationModel.FirstOrDefault(x => x.CancelledId == id);
                    var type = ServiceShared.CalculateTernary(cancelled.TypeCancellation.ToLower() == ServiceConstants.Invoice, ServiceConstants.Invoice, ServiceConstants.Delivery);
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

        private async Task<CardsAdvancedLook> GetStatusToSearch(List<UserOrderModel> userOrders, AdnvaceLookUpModel almacenData, List<Tuple<int, string>> tupleIds, string initialDocNum)
        {
            var sapSaleOrder = (await this.sapDao.GetAllOrdersWIthDetailByIdsJoinProduct(tupleIds.Where(x => ServiceShared.CalculateOr(x.Item2 == ServiceConstants.SaleOrder, x.Item2 == ServiceConstants.DontExistsTable)).Select(y => y.Item1).ToList())).ToList();
            sapSaleOrder = await this.ExcludeSpecialsWarehouses(sapSaleOrder);

            var sapDeliveryDetails = (await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(tupleIds.Where(x => x.Item2 == ServiceConstants.Delivery).Select(y => y.Item1).ToList())).ToList();
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryDetailBySaleOrderJoinProduct(sapSaleOrder.Select(x => x.DocNum).ToList()));
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryDetailByDocEntryJoinProduct(sapDeliveryDetails.Select(x => x.DeliveryId).ToList()));
            var sapDelivery = (await this.sapDao.GetDeliveryModelByDocNumJoinDoctor(sapDeliveryDetails.Select(y => y.DeliveryId).ToList())).ToList();

            var sapInvoicesHeaders = (await this.sapDao.GetInvoiceHeadersByDocNumJoinDoctor(tupleIds.Where(x => ServiceShared.CalculateOr(x.Item2 == ServiceConstants.Invoice, x.Item2 == ServiceConstants.DontExistsTable)).Select(y => y.Item1).ToList())).ToList();
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
            almacenData.Boxes.AddRange(almacenDataForDelivery.Boxes);

            var deliveryCompanies = (await this.sapDao.GetDeliveryCompanyById(sapInvoicesHeaders.Select(x => x.TransportCode).ToList())).ToList();
            var users = await this.GetUsers(userOrders, almacenData);

            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var payments = await this.GetPayments(sapSaleOrder, sapDelivery, sapInvoicesHeaders);
            var doctorAddresses = await this.GetDeliveryAddressInfo(sapSaleOrder, sapDelivery, sapInvoicesHeaders);

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
                Payments = payments,
                Boxes = almacenData.Boxes,
                IsFromDxpId = initialDocNum.StartsWith(ServiceConstants.WildcardDocNumDxp),
                DocNum = initialDocNum,
                DeliveryAddress = doctorAddresses,
            };

            tupleIds.DistinctBy(order => new { order.Item1, order.Item2 }).ToList().ForEach(order =>
            {
                cardToReturns.CardOrder.AddRange(this.GetIsReceptionOrders(order, objectCardOrder));
                cardToReturns.CardOrder.AddRange(this.GetIsReceptionOrdersSample(order, objectCardOrder));
                cardToReturns.CardDelivery.AddRange(this.GetIsReceptionDelivery(order, objectCardOrder));
                cardToReturns.CardInvoice.AddRange(this.GetIsPackageInvoice(order, objectCardOrder));
                cardToReturns.CardDistribution.AddRange(this.GetIsPackageDistribution(order, objectCardOrder));
            });

            cardToReturns.CardDelivery = cardToReturns.CardDelivery.DistinctBy(cd => new { cd.Remision, cd.ListSaleOrder }).ToList();
            return cardToReturns;
        }

        private async Task<List<DoctorDeliveryAddressModel>> GetDeliveryAddressInfo(List<CompleteOrderModel> sapSaleOrder, List<DeliverModel> sapDelivery, List<InvoiceHeaderModel> sapInvoicesHeaders)
        {
            var addressesToFind = sapDelivery.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.ShippingAddressName }).ToList();
            addressesToFind.AddRange(sapInvoicesHeaders.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.ShippingAddressName }).ToList());
            addressesToFind.AddRange(sapSaleOrder.Select(x => new GetDoctorAddressModel { CardCode = x.Codigo, AddressId = x.ShippingAddressName }).ToList());
            return await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, addressesToFind.DistinctBy(a => new { a.CardCode, a.AddressId }).ToList());
        }

        private async Task<List<UserModel>> GetUsers(List<UserOrderModel> userOrders, AdnvaceLookUpModel almacenData)
        {
            var userTolookFor = almacenData.PackageModels.Select(x => x.AssignedUser).ToList();
            userTolookFor.AddRange(userOrders.Select(x => x.UserCheckIn));
            userTolookFor.AddRange(almacenData.LineProducts.Select(x => x.UserCheckIn));
            userTolookFor.AddRange(userOrders.Select(x => x.UserInvoiceStored));
            userTolookFor.AddRange(almacenData.LineProducts.Select(x => x.UserInvoiceStored));
            var userResponse = await this.usersService.GetUsersById(userTolookFor.Distinct().ToList(), ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionOrders(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var lineProducts = paramentsCards.ProductModel;
            var orderbyDocNum = paramentsCards.OrderDetail.Where(x => ServiceShared.CalculateAnd(x.DocNum == tuple.Item1, x.PedidoStatus == "O" || x.Canceled == "Y", x.Detalles != null)).ToList();
            var isLineSale = ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.DontExistsTable, orderbyDocNum.Any(), orderbyDocNum.All(x => lineProducts.Contains(x.Detalles.ProductoId)));
            if (ServiceShared.CalculateAnd(tuple.Item2 != ServiceConstants.SaleOrder, !isLineSale))
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var existanceMag = paramentsCards.UserOrders.GetSaleOrderHeader(tuple.Item1.ToString());
            existanceMag ??= new UserOrderModel { Status = string.Empty };
            var existanceLin = paramentsCards.LineProducts.GetLineProductOrderHeader(tuple.Item1);
            existanceLin ??= new LineProductsModel { StatusAlmacen = string.Empty };

            if (ServiceShared.CalculateOr(ServiceConstants.StatusToIgnoreUserOrderAdvancedLook.Contains(existanceMag.Status), ServiceConstants.StatusToIgnoreLineOrderAdvancedLook.Contains(existanceLin.StatusAlmacen)))
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var userOrder = paramentsCards.UserOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString() && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)));
            var lineProductOrder = paramentsCards.LineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), x.SaleOrderId == tuple.Item1, ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen)));
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

            var existanceMag = paramentsCards.UserOrders.GetSaleOrderHeader(tuple.Item1.ToString());
            existanceMag ??= new UserOrderModel { Status = ServiceConstants.Almacenado };
            var existanceLin = paramentsCards.LineProducts.GetLineProductOrderHeader(tuple.Item1);
            existanceLin ??= new LineProductsModel { StatusAlmacen = ServiceConstants.Almacenado };

            if (ServiceShared.CalculateOr(existanceMag.Status != ServiceConstants.Almacenado, existanceLin.StatusAlmacen != ServiceConstants.Almacenado))
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var userOrder = paramentsCards.UserOrders.FirstOrDefault(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.Productionorderid), x.Salesorderid == tuple.Item1.ToString(), x.Status == ServiceConstants.Almacenado, x.StatusAlmacen == ServiceConstants.Almacenado));
            var lineProductOrder = paramentsCards.LineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(string.IsNullOrEmpty(x.ItemCode), x.SaleOrderId == tuple.Item1, x.StatusAlmacen == ServiceConstants.Almacenado));
            paramentsCards.UserOrder = userOrder;
            paramentsCards.LineProductOrder = lineProductOrder;

            return this.GenerateCardForReceptionOrders(tuple, paramentsCards);
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionOrders(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var order = new CompleteOrderModel();
            var status = string.Empty;
            var productType = string.Empty;
            var saporders = new List<CompleteOrderModel>();
            var porRecibirDate = DateTime.Now;
            var hasCandidate = false;
            var userOrder = paramentsCards.UserOrder;
            var lineProductOrder = paramentsCards.LineProductOrder;
            var comments = new StringBuilder();

            if (userOrder != null)
            {
                var userOrders = paramentsCards.UserOrders.Where(x => x.Salesorderid == tuple.Item1.ToString()).ToList();
                saporders = paramentsCards.OrderDetail.Where(x => x.DocNum.ToString() == userOrder.Salesorderid).ToList();
                order = saporders.FirstOrDefault() ?? new CompleteOrderModel();
                status = ServiceShared.CalculateTernary(ServiceConstants.StatusForBackOrder.Contains(userOrder.Status) && userOrder.StatusAlmacen == ServiceConstants.BackOrder, ServiceConstants.BackOrder, ServiceConstants.PorRecibir);
                status = ServiceShared.CalculateTernary(userOrder.Status != ServiceConstants.Finalizado && userOrder.Status != ServiceConstants.Almacenado && status != ServiceConstants.BackOrder, ServiceConstants.Pendiente, status);
                status = ServiceShared.CalculateTernary(userOrder.Status == ServiceConstants.Almacenado && order.PedidoMuestra.ValidateNull().ToLower() == ServiceConstants.IsSampleOrder.ToLower(), ServiceConstants.Almacenado, status);
                productType = ServiceShared.CalculateTernary(saporders.Any(x => x.Detalles != null && paramentsCards.ProductModel.Any(p => p == x.Detalles.ProductoId)), ServiceConstants.Mixto, ServiceConstants.Magistral);
                porRecibirDate = userOrder.CloseDate ?? porRecibirDate;
                comments.Append($"{userOrder.Comments}&");
                hasCandidate = this.CalulateIfSaleOrderIsCandidate(userOrders, userOrder.Status, order, userOrder);
            }

            var hasDeliveries = paramentsCards.DeliveryDetails.Where(x => x.BaseEntry == tuple.Item1).Select(x => x.DeliveryId).ToList();
            var hasActiveDeliveries = hasDeliveries.Any() && paramentsCards.DeliveryHeader.Where(x => hasDeliveries.Contains(x.DocNum)).Any(y => y.Canceled != "Y");

            if (ServiceShared.CalculateAnd(userOrder == null, lineProductOrder != null || (lineProductOrder == null && !hasActiveDeliveries)))
            {
                saporders = paramentsCards.OrderDetail.Where(x => x.DocNum == tuple.Item1).ToList();
                order = saporders.FirstOrDefault() ?? new CompleteOrderModel();
                status = ServiceShared.CalculateTernary(lineProductOrder == null, ServiceConstants.PorRecibir, lineProductOrder?.StatusAlmacen);
                status = ServiceShared.CalculateTernary(lineProductOrder?.StatusAlmacen == ServiceConstants.Recibir, ServiceConstants.PorRecibir, status);
                status = ServiceShared.CalculateTernary(lineProductOrder?.StatusAlmacen == ServiceConstants.Almacenado && order.PedidoMuestra.ValidateNull().ToLower() == ServiceConstants.IsSampleOrder.ToLower(), ServiceConstants.Almacenado, status);
                productType = ServiceConstants.Linea;
                porRecibirDate = ServiceShared.ParseExactDateOrDefault(order.FechaInicio, porRecibirDate);
                hasCandidate = this.CalculateIfLineOrderIsCandidate(lineProductOrder, status, order);
            }

            if (ServiceShared.CalculateOr(!hasCandidate, order.DocNum == 0))
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            var payment = paramentsCards.Payments.GetPaymentBydocNumDxp(order.DocNumDxp);
            var deliveryAddress = paramentsCards.DeliveryAddress.GetSpecificDeliveryAddress(order.Codigo, order.ShippingAddressName);
            var invoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, paramentsCards.LocalNeighbors, order.Address, payment);
            var totalItems = saporders.Count;
            var totalPieces = (int)saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
            var initDate = ServiceShared.ParseExactDateOrDefault(order.FechaInicio, DateTime.Now);
            var lettersToRemoveDxpId = ServiceShared.CalculateTernary(paramentsCards.DocNum.Length > 1, 1, 0);

            var saleHeader = new AlmacenSalesHeaderModel
            {
                DocNum = order.DocNum,
                Status = ServiceShared.CalculateTernary(order.Canceled == "Y", ServiceConstants.Cancelado, status),
                TypeSaleOrder = $"Pedido {productType}",
                Doctor = order.Medico,
                InvoiceType = invoiceType,
                TotalItems = totalItems,
                InitDate = initDate,
                Client = deliveryAddress.Contact.ValidateNull(),
                TotalPieces = totalPieces,
                DataCheckin = porRecibirDate,
                OrderMuestra = ServiceShared.CalculateTernary(string.IsNullOrEmpty(order.PedidoMuestra), ServiceConstants.IsNotSampleOrder, order.PedidoMuestra),
                Comments = comments.ToString(),
                SapComments = order.Comments,
                TypeOrder = order.OrderType,
                StoredBy = this.GetUserWhoStored(paramentsCards.Users, userOrder, lineProductOrder),
                IsPackage = order.IsPackage == ServiceConstants.IsPackage,
                IsFromDxpId = paramentsCards.IsFromDxpId,
                DxpId = ServiceShared.CalculateTernary(paramentsCards.IsFromDxpId, paramentsCards.DocNum.ToLower().Remove(0, lettersToRemoveDxpId), string.Empty),
                IsOmigenomics = order.IsOmigenomics == ServiceConstants.IsOmigenomics,
            };

            return new List<AlmacenSalesHeaderModel> { saleHeader };
        }

        private bool CalulateIfSaleOrderIsCandidate(List<UserOrderModel> userOrders, string statusOrder, CompleteOrderModel order, UserOrderModel saleOrder)
        {
            var hasCandidate = false;

            if (ServiceShared.CalculateAnd(!string.IsNullOrEmpty(saleOrder.StatusAlmacen), saleOrder.StatusAlmacen == ServiceConstants.BackOrder))
            {
                return true;
            }

            if (statusOrder == ServiceConstants.Finalizado)
            {
                var localOrders = this.GetLocalOrders(userOrders);
                hasCandidate = localOrders.All(x => ServiceShared.CalculateOr(x.Status == ServiceConstants.Finalizado, x.Status == ServiceConstants.Almacenado) && x.FinishedLabel == 1) && !localOrders.All(x => ServiceShared.CalculateAnd(x.Status == ServiceConstants.Almacenado, x.FinishedLabel == 1));
                hasCandidate = ServiceShared.CalculateOr(userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).All(y => y.Status == ServiceConstants.Cancelado), hasCandidate);
                return hasCandidate;
            }

            if (statusOrder == ServiceConstants.Liberado)
            {
                var localOrders = this.GetLocalOrders(userOrders);
                var orderWithStatusAndLabel = localOrders.Where(x => ServiceShared.CalculateOr(x.Status == ServiceConstants.Almacenado, x.Status == ServiceConstants.Finalizado)).All(y => y.FinishedLabel == 1);
                hasCandidate = localOrders.All(x => ServiceConstants.StatusForOrderLiberado.Contains(x.Status)) && orderWithStatusAndLabel;
                var areAllPending = localOrders.All(x => x.Status == ServiceConstants.Pendiente);
                hasCandidate = ServiceShared.CalculateAnd(!areAllPending, hasCandidate);
                return hasCandidate;
            }

            if (this.ValidateMagistralOrderSample(statusOrder, order))
            {
                return true;
            }

            return hasCandidate;
        }

        private bool ValidateMagistralOrderSample(string statusOrder, CompleteOrderModel order)
        {
            return ServiceShared.CalculateAnd(statusOrder == ServiceConstants.Almacenado, !string.IsNullOrEmpty(order.PedidoMuestra), order.PedidoMuestra.ValidateNull().ToLower() == ServiceConstants.IsSampleOrder.ToLower());
        }

        private List<UserOrderModel> GetLocalOrders(List<UserOrderModel> userOrders)
        {
            return userOrders.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Productionorderid), x.Status != ServiceConstants.Cancelado)).ToList();
        }

        private bool CalculateIfLineOrderIsCandidate(LineProductsModel lineProduct, string statusOrder, CompleteOrderModel order)
        {
            var statusToValidate = ServiceShared.CalculateTernary(lineProduct == null, statusOrder, lineProduct?.StatusAlmacen);
            return statusToValidate switch
            {
                ServiceConstants.Almacenado => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(order.PedidoMuestra), order.PedidoMuestra.ValidateNull().ToLower() == ServiceConstants.IsSampleOrder.ToLower()),
                _ => true,
            };
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionDelivery(Tuple<int, string> tuple, ParamentsCards paramsCard)
        {
            if (tuple.Item2 == ServiceConstants.Invoice)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrdersFromSale = paramsCard.UserOrders.Where(x => ServiceShared.CalculateAnd(x.Salesorderid == tuple.Item1.ToString(), !string.IsNullOrEmpty(x.Productionorderid), x.DeliveryId != 0)).ToList();
                var lineProductsFromSale = paramsCard.LineProducts.Where(x => ServiceShared.CalculateAnd(x.SaleOrderId == tuple.Item1, !string.IsNullOrEmpty(x.ItemCode), x.DeliveryId != 0)).ToList();
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
                    Payments = paramsCard.Payments,
                    DeliveryAddress = paramsCard.DeliveryAddress,
                };
                return this.GenerateCardForReceptionDelivery(paramsCardDelivery);
            }

            var isCancelled = paramsCard.Cancellations != null && paramsCard.Cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.Delivery, !isCancelled))
            {
                var userOrdersFromDelivery = paramsCard.UserOrders.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == tuple.Item1, !string.IsNullOrEmpty(x.Productionorderid), x.DeliveryId != 0)).ToList();
                var lineProductsFromDelivery = paramsCard.LineProducts.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == tuple.Item1, !string.IsNullOrEmpty(x.ItemCode), x.DeliveryId != 0)).ToList();
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
                    Payments = paramsCard.Payments,
                    DeliveryAddress = paramsCard.DeliveryAddress,
                };
                return this.GenerateCardForReceptionDelivery(paramsCardDelivery);
            }

            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.Delivery, isCancelled))
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
                    Payments = paramsCard.Payments,
                    DeliveryAddress = paramsCard.DeliveryAddress,
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
                var deliveriesWithInvoice = deliveryDetailModels.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == delivery, x.InvoiceId.HasValue && x.InvoiceId.Value != 0)).Select(y => y.InvoiceId.Value).ToList();
                var areDeliveriesWithValidInvoice = invoiceHeaders.Where(x => x.Canceled == "N").Any(y => deliveriesWithInvoice.Contains(y.InvoiceId));
                if (areDeliveriesWithValidInvoice)
                {
                    continue;
                }

                var header = deliveryHeaders.FirstOrDefault(x => x.DocNum == delivery);
                header ??= new DeliverModel();
                var deliveryDetail = deliveryDetailModels.Where(x => x.DeliveryId == delivery).ToList();

                var salesOrders = deliveryDetail.Select(x => x.BaseEntry).Distinct().OrderByDescending(y => y).ToList();

                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);
                var payment = paramsCardDelivery.Payments.GetPaymentBydocNumDxp(header.DocNumDxp);
                var deliveryAddress = paramsCardDelivery.DeliveryAddress.GetSpecificDeliveryAddress(header.CardCode, header.ShippingAddressName);
                var invoiceType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, paramsCardDelivery.LocalNeighbors, header.Address, payment);
                var productType = this.GenerateListProductTypeDelivery(deliveryDetail, lineSapProducts);

                var userOrderByDelivery = userOrders.FirstOrDefault(x => x.DeliveryId == delivery);
                var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.DeliveryId == delivery);
                var initDate = ServiceShared.CalculateTernary(userOrderByDelivery != null, userOrderByDelivery?.DateTimeCheckIn, lineProductByDelivery?.DateCheckIn);
                var status = ServiceShared.CalculateTernary(userOrderByDelivery != null, userOrderByDelivery?.StatusAlmacen, lineProductByDelivery?.StatusAlmacen);

                saleHeader.Add(new AlmacenSalesHeaderModel
                {
                    Client = deliveryAddress.Contact.ValidateNull(),
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
                    ListSaleOrder = string.Join(", ", salesOrders),
                    TypeOrder = header.TypeOrder,
                    IsPackage = header.IsPackage == ServiceConstants.IsPackage,
                    IsOmigenomics = header.IsOmigenomics == ServiceConstants.IsOmigenomics,
                    DxpId = header.DocNumDxp,
                });
            }

            return saleHeader;
        }

        private string GenerateListProductTypeDelivery(List<DeliveryDetailModel> deliveryDetails, List<string> lineProducts)
        {
            if (deliveryDetails.All(x => lineProducts.Contains(x.ProductoId)))
            {
                return ServiceConstants.Linea;
            }

            if (ServiceShared.CalculateAnd(!deliveryDetails.All(x => lineProducts.Contains(x.ProductoId)), deliveryDetails.Any(x => lineProducts.Contains(x.ProductoId))))
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
            var doctorText = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Doctor, string.Empty);
            var doctorValue = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctorText), new List<string>(), doctorText.Split(",").ToList());
            var dictDates = ServiceUtils.GetDateFilter(parameters);
            var type = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.SaleOrder);

            switch (type)
            {
                case ServiceConstants.SaleOrder:
                    var orders = (await this.sapDao.GetOrderModelByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    orders = orders.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Medico), doctorValue.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower())))).ToList();
                    return orders.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Delivery:
                    var deliveries = (await this.sapDao.GetDeliveryByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    deliveries = deliveries.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Medico), doctorValue.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower())))).ToList();
                    return deliveries.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Invoice:
                    var invoices = (await this.sapDao.GetInvoiceHeadersByDocDateJoinDoctor(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    invoices = invoices.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Medico), doctorValue.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower())))).ToList();
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
                var userOrder = userOrders.Where(x => ServiceShared.CalculateAnd(x.Salesorderid == tuple.Item1.ToString(), !string.IsNullOrEmpty(x.Productionorderid), x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado, x.DeliveryId != 0)).ToList();
                var lineProductOrder = lineProducts.Where(x => ServiceShared.CalculateAnd(x.SaleOrderId == tuple.Item1, !string.IsNullOrEmpty(x.ItemCode), x.StatusAlmacen == ServiceConstants.Almacenado || x.StatusAlmacen == ServiceConstants.Empaquetado, x.DeliveryId != 0)).ToList();
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
                    Payments = paramsCard.Payments,
                    LookUpTuple = tuple,
                    DeliveryAddress = paramsCard.DeliveryAddress,
                };
                return this.GenerateCardForPackageInvoiceTheSalesOrder(paramsCardInvoice);
            }

            var isCancelled = paramsCard.Cancellations != null && paramsCard.Cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.Delivery, !isCancelled))
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
                    Payments = paramsCard.Payments,
                    DeliveryAddress = paramsCard.DeliveryAddress,
                    LookUpTuple = tuple,
                };
                return this.GenerateCardForPackageInvoiceTheSalesOrder(paramsCardInvoice);
            }

            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.Invoice, !isCancelled))
            {
                var invoice = paramsCard.InvoiceHeaders.FirstOrDefault(x => x.DocNum == tuple.Item1);
                invoice ??= new InvoiceHeaderModel();
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoice, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, paramsCard.LocalNeighbors, paramsCard.Payments, paramsCard.DeliveryAddress);
            }

            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.Invoice, isCancelled))
            {
                var cancelation = paramsCard.Cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                return this.GenerateCardForPackageInvoiceCancelled(paramsCard.InvoiceHeaders, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, cancelation, paramsCard.LocalNeighbors, paramsCard.Payments, paramsCard.DeliveryAddress);
            }

            var invoiceLocal = paramsCard.InvoiceHeaders.FirstOrDefault(x => x.DocNum == tuple.Item1);
            invoiceLocal ??= new InvoiceHeaderModel();
            var deliverys = paramsCard.DeliveryDetails.Where(d => d.InvoiceId == invoiceLocal.InvoiceId).Select(d => d.DeliveryId).ToList();
            if (ServiceShared.CalculateAnd(tuple.Item2 == ServiceConstants.DontExistsTable, userOrders.Any(uo => deliverys.Contains(uo.DeliveryId)) || lineProducts.Any(lp => deliverys.Contains(lp.DeliveryId)), !isCancelled))
            {
                return this.GenerateCardForPackageInvoice(userOrders, lineProducts, invoiceLocal, paramsCard.InvoiceDetailsToLook, paramsCard.DeliveryDetails, paramsCard.LocalNeighbors, paramsCard.Payments, paramsCard.DeliveryAddress);
            }

            return new List<InvoiceHeaderAdvancedLookUp>();
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoiceCancelled(List<InvoiceHeaderModel> invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder, CancellationResourceModel canceled, List<string> localNeighbors, List<PaymentsDto> payments, List<DoctorDeliveryAddressModel> deliveryAddressList)
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
            var payment = payments.GetPaymentBydocNumDxp(invoiceHeader.DocNumDxp);
            var deliveryAddress = deliveryAddressList.GetSpecificDeliveryAddress(invoiceHeader.CardCode, invoiceHeader.ShippingAddressName);

            var remissionList = invoiceDetail.DistinctBy(x => x.BaseEntry).Select(invoice => invoice.BaseEntry);
            var remissionListStr = remissionList.Any() ? string.Join(", ", remissionList) : string.Empty;

            var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
            {
                Address = invoiceHeader.Address.Replace("\r", string.Empty).ToUpper(),
                Client = deliveryAddress.Contact.ValidateNull(),
                Doctor = invoiceHeader.Medico ?? string.Empty,
                Invoice = invoiceHeader.DocNum,
                DocEntry = invoiceHeader.InvoiceId,
                InvoiceDocDate = invoiceHeader.FechaInicio,
                ProductType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeighbors, invoiceHeader.Address, payment),
                TotalDeliveries = invoiceDetail.DistinctBy(x => x.BaseEntry.Value).Count(),
                TotalProducts = totalProducts,
                StatusDelivery = ServiceConstants.Cancelado,
                DataCheckin = canceled.CancelDate,
                SalesOrder = deliverysToLookSaleOrder.DistinctBy(x => x.BaseEntry).Count(),
                IsLookUpInvoices = true,
                IsRefactura = false,
                TypeOrder = invoiceHeader.TypeOrder,
                IsPackage = invoiceHeader.IsPackage == ServiceConstants.IsPackage,
                IsDeliveredInOffice = invoiceHeader.IsDeliveredInOffice ?? "N",
                RemissionList = remissionListStr,
                OrderList = string.Empty,
                TotalPieces = invoiceDetail.Where(y => y.Quantity > 0).Sum(x => (int)x.Quantity),
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
                    var invoiceHeader = invoiceHeadersToLook.FirstOrDefault(x => ServiceShared.CalculateAnd(x.Canceled != "Y", x.InvoiceId == invoiceId.InvoiceId));
                    if (invoiceHeader == null)
                    {
                        continue;
                    }

                    var invoiceDetail = invoiceDetailsToLook.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();
                    var deliveryDetails = deliveryDetailModels.Where(x => x.InvoiceId == invoiceHeader.InvoiceId).ToList();

                    var deliverys = this.GetDeliveryModel(deliveryDetails, invoiceDetail, userOrders, lineProducts);
                    var deliveryHeader = deliverys.FirstOrDefault(x => x.DeliveryId == delivery);
                    var saleOrders = ServiceShared.CalculateTernary(paramsCardInvoice.LookUpTuple.Item2 == ServiceConstants.SaleOrder, paramsCardInvoice.LookUpTuple.Item1, deliveryHeader.SaleOrder);
                    var totalProducts = invoiceDetail.Count;

                    var userOrderByDate = userOrders.FirstOrDefault(x => x.DeliveryId == delivery);
                    var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.DeliveryId == delivery);
                    var initDate = ServiceShared.CalculateTernary(userOrderByDate != null, userOrderByDate?.DateTimeCheckIn, lineProductByDelivery?.DateCheckIn);
                    var payment = paramsCardInvoice.Payments.GetPaymentBydocNumDxp(invoiceHeader.DocNumDxp);
                    var deliveryAddress = paramsCardInvoice.DeliveryAddress.GetSpecificDeliveryAddress(invoiceHeader.CardCode, invoiceHeader.ShippingAddressName);

                    var remissionList = invoiceDetail.DistinctBy(x => x.BaseEntry).Select(invoice => invoice.BaseEntry);
                    var remissionListStr = remissionList.Any() ? string.Join(", ", remissionList) : string.Empty;

                    var orderList = string.Join(", ", deliveryDetails.DistinctBy(x => x.BaseEntry).Select(delivery => delivery.BaseEntry));
                    var orderListStr = orderList.Any() ? string.Join(", ", orderList) : string.Empty;

                    if (!deliverys.All(x => x.Status == ServiceConstants.Empaquetado))
                    {
                        var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
                        {
                            Address = invoiceHeader.Address.Replace("\r", string.Empty).ToUpper(),
                            Client = deliveryAddress.Contact.ValidateNull(),
                            Doctor = invoiceHeader.Medico ?? string.Empty,
                            Invoice = invoiceHeader.DocNum,
                            DocEntry = invoiceHeader.InvoiceId,
                            InvoiceDocDate = invoiceHeader.FechaInicio,
                            ProductType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, paramsCardInvoice.LocalNeighbors, invoiceHeader.Address, payment),
                            TotalDeliveries = deliverys.Count,
                            TotalProducts = totalProducts,
                            DeliverId = delivery,
                            SalesOrder = saleOrders,
                            StatusDelivery = deliveryHeader.Status,
                            DataCheckin = initDate,
                            IsLookUpInvoices = false,
                            IsRefactura = ServiceShared.CalculateAnd(!string.IsNullOrEmpty(invoiceHeader.Refactura), invoiceHeader.Refactura == ServiceConstants.IsRefactura),
                            TypeOrder = invoiceHeader.TypeOrder,
                            IsPackage = invoiceHeader.IsPackage == ServiceConstants.IsPackage,
                            IsDeliveredInOffice = invoiceHeader.IsDeliveredInOffice ?? "N",
                            OrderList = orderListStr,
                            RemissionList = remissionListStr,
                            TotalPieces = invoiceDetail.Where(y => y.Quantity > 0).Sum(x => (int)x.Quantity),
                        };
                        invoicesHeaders.Add(invoiceHeaderLookUp);
                    }
                }
            }

            return invoicesHeaders;
        }

        private List<InvoiceHeaderAdvancedLookUp> GenerateCardForPackageInvoice(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, InvoiceHeaderModel invoiceHeaders, List<InvoiceDetailModel> invoiceDetailsToLook, List<DeliveryDetailModel> deliverysToLookSaleOrder, List<string> localNeigbors, List<PaymentsDto> payments, List<DoctorDeliveryAddressModel> deliveryAddressList)
        {
            var invoicesHeaders = new List<InvoiceHeaderAdvancedLookUp>();

            if (deliverysToLookSaleOrder.Any(y => y.InvoiceId.HasValue && y.InvoiceId.Value != 0))
            {
                var invoiceDetail = invoiceDetailsToLook.Where(x => x.InvoiceId == invoiceHeaders.InvoiceId).ToList();
                var deliveryDetails = deliverysToLookSaleOrder.Where(x => x.InvoiceId == invoiceHeaders.InvoiceId).ToList();

                var remissionList = invoiceDetail.DistinctBy(x => x.BaseEntry).Select(invoice => invoice.BaseEntry);
                var remissionListStr = remissionList.Any() ? string.Join(", ", remissionList) : string.Empty;

                var orderList = string.Join(", ", deliveryDetails.DistinctBy(x => x.BaseEntry).Select(delivery => delivery.BaseEntry));
                var orderListStr = orderList.Any() ? string.Join(", ", orderList) : string.Empty;

                var deliverys = this.GetDeliveryModel(deliveryDetails, invoiceDetail, userOrders, lineProducts);
                var totalProducts = invoiceDetail.Count;

                var userOrderByDate = userOrders.FirstOrDefault(x => x.InvoiceId == invoiceHeaders.DocNum);
                userOrderByDate ??= userOrders.FirstOrDefault();
                var lineProductByDelivery = lineProducts.FirstOrDefault(x => x.InvoiceId == invoiceHeaders.DocNum);
                lineProductByDelivery ??= lineProducts.FirstOrDefault();
                var initDate = ServiceShared.CalculateTernary(userOrderByDate != null, userOrderByDate?.DateTimeCheckIn, lineProductByDelivery?.DateCheckIn);
                var payment = payments.GetPaymentBydocNumDxp(invoiceHeaders.DocNumDxp);
                var deliveryAddress = deliveryAddressList.GetSpecificDeliveryAddress(invoiceHeaders.CardCode, invoiceHeaders.ShippingAddressName);

                if (!deliverys.All(x => x.Status == ServiceConstants.Empaquetado))
                {
                    var invoiceHeaderLookUp = new InvoiceHeaderAdvancedLookUp
                    {
                        Address = invoiceHeaders.Address.Replace("\r", string.Empty).ToUpper(),
                        Client = deliveryAddress.Contact.ValidateNull(),
                        Doctor = invoiceHeaders.Medico ?? string.Empty,
                        Invoice = invoiceHeaders.DocNum,
                        DocEntry = invoiceHeaders.InvoiceId,
                        InvoiceDocDate = invoiceHeaders.FechaInicio,
                        ProductType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, localNeigbors, invoiceHeaders.Address, payment),
                        TotalDeliveries = deliverys.Count,
                        TotalProducts = totalProducts,
                        StatusDelivery = ServiceConstants.Almacenado,
                        DataCheckin = initDate.Value,
                        RemissionList = remissionListStr,
                        OrderList = orderListStr,
                        SalesOrder = deliveryDetails.DistinctBy(x => x.BaseEntry).Count(),
                        IsLookUpInvoices = true,
                        IsRefactura = ServiceShared.CalculateAnd(!string.IsNullOrEmpty(invoiceHeaders.Refactura), invoiceHeaders.Refactura == ServiceConstants.IsRefactura),
                        TypeOrder = invoiceHeaders.TypeOrder,
                        IsPackage = invoiceHeaders.IsPackage == ServiceConstants.IsPackage,
                        IsDeliveredInOffice = invoiceHeaders.IsDeliveredInOffice ?? "N",
                        TotalPieces = invoiceDetail.Where(y => y.Quantity > 0).Sum(x => (int)x.Quantity),
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
                    var userOrderStatus = userOrderModels.Where(z => ServiceShared.CalculateAnd(z.DeliveryId == y.DeliveryId, !string.IsNullOrEmpty(z.Productionorderid))).Select(y => y.StatusAlmacen).ToList();
                    userOrderStatus.AddRange(lineProducts.Where(x => ServiceShared.CalculateAnd(x.DeliveryId == y.DeliveryId, !string.IsNullOrEmpty(x.ItemCode))).Select(y => y.StatusAlmacen));

                    var deliveryModel = new InvoiceDeliveryModel
                    {
                        DeliveryId = y.DeliveryId,
                        DeliveryDocDate = y.DocDate,
                        SaleOrder = y.BaseEntry ?? 0,
                        Status = ServiceShared.CalculateTernary(userOrderStatus.Any() && userOrderStatus.All(z => z == ServiceConstants.Empaquetado), ServiceConstants.Empaquetado, ServiceConstants.Almacenado),
                        TotalItems = invoiceDetails.Where(a => a.BaseEntry.HasValue).Count(z => z.BaseEntry == y.DeliveryId),
                    };

                    listToReturn.Add(deliveryModel);
                });

            return listToReturn;
        }

        private List<InvoiceHeaderAdvancedLookUp> GetIsPackageDistribution(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            return tuple.Item2 switch
            {
                ServiceConstants.SaleOrder => this.GetDistributionCardFromSale(tuple, paramentsCards),
                ServiceConstants.Delivery => this.GetDistributionCardFromDelivery(tuple, paramentsCards),
                ServiceConstants.Invoice => this.GetDistributionCardFromInvoice(tuple, paramentsCards),
                _ => new List<InvoiceHeaderAdvancedLookUp>(),
            };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromSale(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var listToReturn = new List<InvoiceHeaderAdvancedLookUp>();

            var invoices = paramentsCards.UserOrders.Where(x => ServiceShared.CalculateAnd(x.Salesorderid == tuple.Item1.ToString(), !string.IsNullOrEmpty(x.Productionorderid), x.InvoiceId != 0)).Select(y => y.InvoiceId).ToList();
            invoices.AddRange(paramentsCards.LineProducts.Where(x => ServiceShared.CalculateAnd(x.SaleOrderId == tuple.Item1, !string.IsNullOrEmpty(x.ItemCode), x.InvoiceId != 0)).Select(y => y.InvoiceId));

            foreach (var invoice in invoices.Distinct())
            {
                var userOrder = paramentsCards.UserOrders.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Productionorderid), x.InvoiceId == invoice)).ToList();
                var lineProductOrder = paramentsCards.LineProducts.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.ItemCode), x.InvoiceId == invoice)).ToList();

                var hasPossilbeMag = userOrder.Any() && userOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));
                var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));

                if (ServiceShared.CalculateAnd(!hasPossilbeMag, !hasPossibleLine))
                {
                    continue;
                }

                userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString(), UserInvoiceStored = x.UserInvoiceStored }));
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
                    Payments = paramentsCards.Payments,
                    Boxes = paramentsCards.Boxes,
                    DeliveryAddress = paramentsCards.DeliveryAddress,
                };

                listToReturn.Add(this.GenerateCardForDistribution(objectForDistribution));
            }

            return listToReturn;
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromDelivery(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var invoiceFromSaleMag = paramentsCards.UserOrders.FirstOrDefault(x => ServiceShared.CalculateAnd(x.DeliveryId == tuple.Item1, !string.IsNullOrEmpty(x.Productionorderid), x.InvoiceId != 0));
            var invoicefromSaleLine = paramentsCards.LineProducts.FirstOrDefault(x => ServiceShared.CalculateAnd(x.DeliveryId == tuple.Item1, !string.IsNullOrEmpty(x.ItemCode), x.InvoiceId != 0));

            if (ServiceShared.CalculateAnd(invoiceFromSaleMag == null, invoicefromSaleLine == null))
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            var invoice = ServiceShared.CalculateTernary(invoiceFromSaleMag != null, invoiceFromSaleMag?.InvoiceId, invoicefromSaleLine?.InvoiceId).Value;
            var userOrder = paramentsCards.UserOrders.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Productionorderid), x.InvoiceId == invoice)).ToList();
            var lineProductOrder = paramentsCards.LineProducts.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.ItemCode), x.InvoiceId == invoice)).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));

            if (ServiceShared.CalculateAnd(!hasPossilbeMag, !hasPossibleLine))
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString(), UserInvoiceStored = x.UserInvoiceStored }));
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
                Payments = paramentsCards.Payments,
                Boxes = paramentsCards.Boxes,
                DeliveryAddress = paramentsCards.DeliveryAddress,
            };

            var card = this.GenerateCardForDistribution(objectForDistribution);
            return new List<InvoiceHeaderAdvancedLookUp> { card };
        }

        private List<InvoiceHeaderAdvancedLookUp> GetDistributionCardFromInvoice(Tuple<int, string> tuple, ParamentsCards paramentsCards)
        {
            var userOrder = paramentsCards.UserOrders.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.Productionorderid), x.InvoiceId == tuple.Item1)).ToList();
            var lineProductOrder = paramentsCards.LineProducts.Where(x => ServiceShared.CalculateAnd(!string.IsNullOrEmpty(x.ItemCode), x.InvoiceId == tuple.Item1)).ToList();

            var hasPossilbeMag = userOrder.Any() && userOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));
            var hasPossibleLine = lineProductOrder.Any() && lineProductOrder.All(x => ServiceShared.CalculateAnd(x.StatusAlmacen != ServiceConstants.Almacenado, !string.IsNullOrEmpty(x.StatusInvoice)));

            if (ServiceShared.CalculateAnd(!hasPossilbeMag, !hasPossibleLine))
            {
                return new List<InvoiceHeaderAdvancedLookUp>();
            }

            userOrder.AddRange(lineProductOrder.Select(x => new UserOrderModel { StatusInvoice = x.StatusInvoice, InvoiceStoreDate = x.InvoiceStoreDate, DeliveryId = x.DeliveryId, Salesorderid = x.SaleOrderId.ToString(), UserInvoiceStored = x.UserInvoiceStored }));

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
                Payments = paramentsCards.Payments,
                Boxes = paramentsCards.Boxes,
                DeliveryAddress = paramentsCards.DeliveryAddress,
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
            package ??= new PackageModel { AssignedUser = string.Empty, Status = userOrder.StatusInvoice, AssignedDate = DateTime.Now, InWayDate = DateTime.Now, DeliveredDate = DateTime.Now };

            var company = parametersDistribution.Repatridores.FirstOrDefault(x => x.TrnspCode == invoice.TransportCode);
            var payment = parametersDistribution.Payments.GetPaymentBydocNumDxp(invoice.DocNumDxp);
            var deliveryAddress = parametersDistribution.DeliveryAddress.GetSpecificDeliveryAddress(invoice.CardCode, invoice.ShippingAddressName);

            var packer = parametersDistribution.Users.FirstOrDefault(x => x.Id == userOrder.UserInvoiceStored);
            packer ??= new UserModel { FirstName = string.Empty, LastName = string.Empty };

            var shippingCostAcceptedInvoice = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(invoice.IsDeliveredInOffice) && invoice.IsDeliveredInOffice == "Y", 0, 1);
            payment.ShippingCostAccepted = payment.TransactionId == null ? shippingCostAcceptedInvoice : payment.ShippingCostAccepted;

            return new InvoiceHeaderAdvancedLookUp
            {
                Invoice = invoiceId,
                DeliverId = ServiceShared.CalculateTernary(!isFromInvoice, userOrder.DeliveryId, 0),
                SalesOrder = !isFromInvoice ? int.Parse(userOrder.Salesorderid) : totalSales.Count,
                StatusDelivery = userOrder.StatusInvoice,
                Address = ServiceShared.CalculateTernary(payment.ShippingCostAccepted == ServiceConstants.ShippingCostAccepted, invoice.Address.Replace("\r", string.Empty).ToUpper(), ServiceConstants.OnSiteDelivery.ToUpper()),
                ProductType = ServiceUtils.CalculateTypeShip(ServiceConstants.NuevoLeon, parametersDistribution.LocalNeighbors, invoice.Address, payment),
                Doctor = invoice.Medico,
                TotalDeliveries = localInvoiceDetails.Select(x => x.BaseEntry).Distinct().Count(),
                InvoiceDocDate = userOrder.InvoiceStoreDate.Value,
                Client = deliveryAddress.Contact.ValidateNull(),
                TotalProducts = localInvoiceDetails.Count,
                DeliveredBy = this.GetDeliveredBy(userOrder.StatusInvoice, package, parametersDistribution.Users),
                DeliveryGuyName = package.Status == ServiceConstants.Entregado ? this.GetDeliveredGuyBy(package, parametersDistribution.Users) : string.Empty,
                ReasonNotDelivered = ServiceShared.CalculateTernary(package.Status == ServiceConstants.NoEntregado, package.ReasonNotDelivered, string.Empty),
                DataCheckin = this.CalculateDistributioDate(userOrder.StatusInvoice, package, userOrder),
                IsLookUpInvoices = isFromInvoice,
                TrakingNumber = invoice.TrackingNumber,
                DeliveryCompany = company?.TrnspName ?? string.Empty,
                CodeClient = invoice.CardCode,
                IsPackage = invoice.IsPackage == ServiceConstants.IsPackage,
                Boxes = this.GetBoxesByInvoice(userOrder.StatusInvoice, invoiceId, parametersDistribution.Boxes),
                EtablishmentName = deliveryAddress.EtablishmentName,
                References = deliveryAddress.References,
                BetweenStreets = deliveryAddress.BetweenStreets,
                DoctorPhone = invoice.DoctorPhoneNumber,
                IsDeliveredInOffice = invoice.IsDeliveredInOffice ?? "N",
                Packer = $"{packer.FirstName.ValidateNull()} {packer.LastName.ValidateNull()}".Trim(),
                DeliveryComments = payment.DeliveryComments,
                DeliverySuggestedTime = payment.DeliverySuggestedTime,
            };
        }

        private DateTime CalculateDistributioDate(string status, PackageModel package, UserOrderModel order)
        {
            return status switch
            {
                ServiceConstants.Empaquetado => order.InvoiceStoreDate.Value,
                ServiceConstants.Enviado => order.InvoiceStoreDate.Value,
                ServiceConstants.Asignado => package.AssignedDate.Value,
                ServiceConstants.Camino => package.InWayDate.Value,
                ServiceConstants.NoEntregado => package.DeliveredDate.Value,
                ServiceConstants.Entregado => package.DeliveredDate.Value,
                _ => order.InvoiceStoreDate.Value,
            };
        }

        private string GetDeliveredBy(string status, PackageModel package, List<UserModel> users)
        {
            var user = users?.FirstOrDefault(x => x.Id == package.AssignedUser);
            user ??= new UserModel();
            return status switch
            {
                ServiceConstants.Asignado => $"{user.FirstName.ValidateNull()} {user.LastName.ValidateNull()}".Trim(),
                ServiceConstants.Entregado => package?.Comments ?? string.Empty,
                _ => string.Empty,
            };
        }

        private string GetDeliveredGuyBy(PackageModel package, List<UserModel> users)
        {
            var user = users.FirstOrDefault(x => x.Id == package.AssignedUser);
            user ??= new UserModel();
            return $"{user.FirstName.ValidateNull()} {user.LastName.ValidateNull()}".Trim();
        }

        private string GetUserWhoStored(List<UserModel> users, UserOrderModel userOrder, LineProductsModel lineProductOrder)
        {
            var userIdStored = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(userOrder?.UserCheckIn), userOrder?.UserCheckIn, lineProductOrder?.UserCheckIn);
            var userStored = users.FirstOrDefault(user => user.Id == userIdStored);
            userStored ??= new UserModel();
            return $"{userStored.FirstName.ValidateNull()} {userStored.LastName.ValidateNull()}".Trim();
        }

        private async Task<List<PaymentsDto>> GetPayments(List<CompleteOrderModel> sapSaleOrder, List<DeliverModel> sapDelivery, List<InvoiceHeaderModel> sapInvoicesHeaders)
        {
            var transactionsIds = sapSaleOrder.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).ToList();
            transactionsIds.AddRange(sapDelivery.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).ToList());
            transactionsIds.AddRange(sapInvoicesHeaders.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).ToList());
            return await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds.Distinct().ToList());
        }

        private List<BoxModel> GetBoxesByInvoice(string status, int invoiceid, List<BoxModel> boxes)
        {
            return status switch
            {
                ServiceConstants.Enviado => boxes.Where(b => b.InvoiceId == invoiceid).DistinctBy(b => new { b.Dimensions, b.Weight }).ToList(),
                _ => new List<BoxModel>(),
            };
        }

        private async Task<List<CompleteOrderModel>> ExcludeSpecialsWarehouses(List<CompleteOrderModel> sapSaleOrder)
        {
            var parametersWhs = (await ServiceUtils.GetParams(new List<string> { ServiceConstants.WareHouseToExclude }, this.catalogsService)).Select(x => x.Value).ToList();
            var orderWIthPt = sapSaleOrder.Where(x => parametersWhs.Contains(x.Detalles.WhsCode)).Select(y => y.DocNum).Distinct().ToList();
            return sapSaleOrder.Where(x => !orderWIthPt.Contains(x.DocNum)).ToList();
        }
    }
}
