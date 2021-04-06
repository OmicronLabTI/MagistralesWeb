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
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// class for advance looks.
    /// </summary>
    public class AdvanceLookService : IAdvanceLookService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvanceLookService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        public AdvanceLookService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
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
            var listDocs = await this.GetIdsToLookByDoctor(parameters);
            var userOrdersResponse = await this.pedidosService.GetUserPedidos(listDocs, ServiceConstants.AdvanceLookId);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.AdvanceLookId, listDocs);
            var almacenData = JsonConvert.DeserializeObject<AdnvaceLookUpModel>(almacenResponse.Response.ToString());

            var tupleList = this.KindLookUp(userOrders, almacenData, listDocs);
            var response = await this.GetStatusToSearch(userOrders, almacenData, tupleList);
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

                if (adnvaceLookUp.CancelationModel.Any(x => x.CancelledId == id))
                {
                    var cancelled = adnvaceLookUp.CancelationModel.FirstOrDefault(x => x.CancelledId == id);
                    var type = cancelled.TypeCancellation.ToLower() == ServiceConstants.Invoice ? ServiceConstants.Invoice : ServiceConstants.Delivery;
                    tupleIds.Add(new Tuple<int, string>(id, type));
                    match = true;
                }

                if (!match)
                {
                    tupleIds.Add(new Tuple<int, string>(id, ServiceConstants.SaleOrder));
                }
            });

            return tupleIds;
        }

        private async Task<CardsAdvancedLook> GetStatusToSearch(List<UserOrderModel> userOrders, AdnvaceLookUpModel almacenData, List<Tuple<int, string>> tupleIds)
        {
            var sapSaleOrder = (await this.sapDao.GetAllOrdersWIthDetailByIds(tupleIds.Where(x => x.Item2 == ServiceConstants.SaleOrder).Select(y => y.Item1).ToList())).ToList();
            var sapDeliveryDetails = (await this.sapDao.GetDeliveryByDocEntry(tupleIds.Where(x => x.Item2 == ServiceConstants.Delivery).Select(y => y.Item1).ToList())).ToList();
            sapDeliveryDetails.AddRange(await this.sapDao.GetDeliveryBySaleOrder(sapSaleOrder.Select(x => x.DocNum).ToList()));
            var sapDelivery = (await this.sapDao.GetDeliveryModelByDocNum(sapDeliveryDetails.Select(y => y.DeliveryId).ToList())).ToList();

            var sapInvoicesHeaders = (await this.sapDao.GetInvoiceHeadersByDocNum(tupleIds.Where(x => x.Item2 == ServiceConstants.Invoice).Select(y => y.Item1).ToList())).ToList();
            var sapInvoicesDeatils = (await this.sapDao.GetInvoiceDetailByDocEntry(sapInvoicesHeaders.Select(x => x.DocNum).ToList())).ToList();
            sapInvoicesDeatils.AddRange(await this.sapDao.GetInvoiceDetailByBaseEntry(sapDelivery.Select(x => x.DocNum).ToList()));
            sapInvoicesHeaders.AddRange(await this.sapDao.GetInvoiceHeaderByInvoiceId(sapInvoicesDeatils.Select(x => x.InvoiceId).ToList()));
            var lineProducts = (await this.sapDao.GetAllLineProducts()).ToList();
            var cardToReturns = new CardsAdvancedLook();
            cardToReturns.CardOrder = new List<AlmacenSalesHeaderModel>();
            cardToReturns.CardDelivery = new List<AlmacenSalesHeaderModel>();

            tupleIds.ForEach(order =>
            {
                var carPedido = this.GetIsReceptionOrders(order, userOrders, almacenData.LineProducts, sapSaleOrder, sapDeliveryDetails);
                var cardRemision = this.GetIsReceptionDelivery(order, userOrders, almacenData.LineProducts, sapDeliveryDetails, sapDelivery, lineProducts, almacenData.CancelationModel);
            });

            return cardToReturns;
        }

        private AlmacenSalesHeaderModel GetIsReceptionOrders(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<CompleteOrderModel> orderDetail, List<DeliveryDetailModel> deliveryDetails)
        {
            if (tuple.Item2 != ServiceConstants.SaleOrder)
            {
                return null;
            }

            var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && x.Salesorderid == tuple.Item1.ToString() && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)));
            var lineProductOrder = lineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.SaleOrderId == tuple.Item1 && x.StatusAlmacen == ServiceConstants.Recibir);
            return this.GenerateCardForReceptionOrders(tuple, lineProducts, orderDetail, deliveryDetails, userOrder, lineProductOrder);
        }

        private AlmacenSalesHeaderModel GenerateCardForReceptionOrders(Tuple<int, string> tuple, List<LineProductsModel> lineProducts, List<CompleteOrderModel> orderDetail, List<DeliveryDetailModel> deliveryDetails, UserOrderModel userOrder, LineProductsModel lineProductOrder)
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
            if (userOrder != null)
            {
                saporders = orderDetail.Where(x => x.DocNum.ToString() == userOrder.Salesorderid).ToList();
                order = saporders.FirstOrDefault();
                status = userOrder.Status == ServiceConstants.Finalizado && userOrder.StatusAlmacen == ServiceConstants.BackOrder ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;
                status = userOrder.Status != ServiceConstants.Finalizado && ServiceConstants.Status != ServiceConstants.Almacenado ? ServiceConstants.Pendiente : status;
                productType = lineProducts.Any(x => x.SaleOrderId == int.Parse(userOrder.Salesorderid)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                porRecibirDate = userOrder.CloseDate ?? porRecibirDate;
                hasCandidate = true;
            }

            if (lineProductOrder != null || (lineProductOrder == null && !deliveryDetails.Any(x => x.DeliveryId == tuple.Item1)))
            {
                saporders = orderDetail.Where(x => x.DocNum == tuple.Item1).ToList();
                order = saporders.FirstOrDefault();
                status = lineProductOrder == null ? ServiceConstants.PorRecibir : lineProductOrder.StatusAlmacen;
                status = lineProductOrder != null && lineProductOrder.StatusAlmacen == ServiceConstants.Recibir ? ServiceConstants.PorRecibir : status;
                productType = ServiceConstants.Line;
                porRecibirDate = DateTime.Parse(order.FechaInicio);
                hasCandidate = true;
            }

            if (!hasCandidate)
            {
                return null;
            }

            invoiceType = order.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
            totalItems = saporders.Count;
            totalPieces = (int)saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);

            var saleHeader = new AlmacenSalesHeaderModel
            {
                DocNum = order.DocNum,
                Status = status,
                TypeSaleOrder = $"Pedido {productType}",
                Doctor = order.Medico,
                InvoiceType = invoiceType,
                TotalItems = totalItems,
                InitDate = porRecibirDate,
                Client = order.Cliente,
                TotalPieces = totalPieces,
            };

            return saleHeader;
        }

        private List<AlmacenSalesHeaderModel> GetIsReceptionDelivery(Tuple<int, string> tuple, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<DeliveryDetailModel> deliveryDetailModels, List<DeliverModel> deliveryHeaders, List<ProductoModel> lineSapProducts, List<CancellationResourceModel> cancellations)
        {
            if (tuple.Item2 != ServiceConstants.Delivery)
            {
                return new List<AlmacenSalesHeaderModel>();
            }

            if (tuple.Item2 == ServiceConstants.SaleOrder)
            {
                var userOrdersFromSale = userOrders.Where(x => x.Salesorderid == tuple.Item1.ToString() && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromSale = lineProducts.Where(x => x.SaleOrderId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var deliveryIdsBySale = userOrdersFromSale.Select(x => x.DeliveryId).ToList();
                deliveryIdsBySale.AddRange(lineProductsFromSale.Select(y => y.DeliveryId));
                return this.GenerateCardForReceptionDelivery(deliveryIdsBySale, deliveryDetailModels, deliveryHeaders, lineSapProducts, userOrdersFromSale, lineProductsFromSale);
            }

            var isCancelled = cancellations.Any(x => x.CancelledId == tuple.Item1);
            if (tuple.Item2 == ServiceConstants.Delivery && !isCancelled)
            {
                var userOrdersFromDelivery = userOrders.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.Productionorderid) && x.DeliveryId != 0).ToList();
                var lineProductsFromDelivery = lineProducts.Where(x => x.DeliveryId == tuple.Item1 && !string.IsNullOrEmpty(x.ItemCode) && x.DeliveryId != 0).ToList();
                var delivryIds = new List<int> { tuple.Item1 };
                return this.GenerateCardForReceptionDelivery(delivryIds, deliveryDetailModels, deliveryHeaders, lineSapProducts, userOrdersFromDelivery, lineProductsFromDelivery);
            }

            if (tuple.Item2 == ServiceConstants.Delivery && isCancelled)
            {
                var delivryIds = new List<int> { tuple.Item1 };
                var cancelation = cancellations.FirstOrDefault(x => x.CancelledId == tuple.Item1);
                cancelation ??= new CancellationResourceModel();
                var cancelledOrder = new List<UserOrderModel> { new UserOrderModel { DeliveryId = tuple.Item1, DateTimeCheckIn = cancelation.CancelDate, StatusAlmacen = ServiceConstants.Cancelado } };
                return this.GenerateCardForReceptionDelivery(delivryIds, deliveryDetailModels, deliveryHeaders, lineSapProducts, cancelledOrder, new List<LineProductsModel>());
            }

            return new List<AlmacenSalesHeaderModel>();
        }

        private List<AlmacenSalesHeaderModel> GenerateCardForReceptionDelivery(List<int> possibleDeliveries, List<DeliveryDetailModel> deliveryDetailModels, List<DeliverModel> deliveryHeaders, List<ProductoModel> lineSapProducts, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var saleHeader = new List<AlmacenSalesHeaderModel>();

            foreach (var delivery in possibleDeliveries)
            {
                if (deliveryDetailModels.Where(x => x.DeliveryId == delivery).Any(y => y.InvoiceId.HasValue && y.InvoiceId.Value != 0))
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
                    InitDate = initDate.Value,
                    Status = status,
                    Remision = delivery,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
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
                    var orders = (await this.sapDao.GetAllOrdersByFechaIni(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    orders = orders.Where(x => doctorValue.All(y => x.Medico.Contains(y))).ToList();
                    return orders.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Delivery:
                    var deliveries = (await this.sapDao.GetDeliveryByDocDate(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    deliveries = deliveries.Where(x => doctorValue.All(y => x.Medico.Contains(y))).ToList();
                    return deliveries.Select(x => x.DocNum).Distinct().ToList();

                case ServiceConstants.Invoice:
                    var invoices = (await this.sapDao.GetInvoiceHeadersByDocDate(dictDates[ServiceConstants.FechaInicio], dictDates[ServiceConstants.FechaFin])).ToList();
                    invoices = invoices.Where(x => doctorValue.All(y => x.Medico.Contains(y))).ToList();
                    return invoices.Select(x => x.DocNum).Distinct().ToList();
            }

            return new List<int>();
        }
    }
}
