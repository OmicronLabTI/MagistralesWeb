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
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

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

            /*
             Generar tarjetas
             */

            var response = await this.GetStatusToSearch(userOrders, almacenData.LineProducts);
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
            var response = await this.GetStatusToSearch(userOrders, almacenData.LineProducts);
            return ServiceUtils.CreateResult(true, 200, null, response, null, null);
        }

        /// <summary>
        /// Gets the cards for look up by id.
        /// </summary>
        /// <param name="userOrders">the user orders list.</param>
        /// <returns>the data.</returns>
        private async Task<CardsAdvancedLook> GetStatusToSearch(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var userOrdersId = userOrders.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            userOrdersId.AddRange(lineProducts.Select(x => x.SaleOrderId).Distinct());
            userOrdersId = userOrdersId.Distinct().ToList();
            var cardToReturns = new CardsAdvancedLook();
            cardToReturns.CardOrder = new List<AlmacenSalesHeaderModel>();
            cardToReturns.CardDelivery = new List<AlmacenSalesHeaderModel>();
            cardToReturns.CardInvoice = new List<InvoiceSaleHeaderModel>();

            foreach (var id in userOrdersId)
            {
                var orders = userOrders.Where(x => int.Parse(x.Salesorderid) == id).ToList();
                var linesProducts = lineProducts.Where(x => x.SaleOrderId == id).ToList();

                var receptionOrders = this.GetIsReceptionOrders(orders, linesProducts);
                if (receptionOrders.Item1 != null || receptionOrders.Item2 != null)
                {
                    cardToReturns.CardOrder.Add(await this.GenerateCardForReceptionOrders(linesProducts, receptionOrders.Item1, receptionOrders.Item2));
                }

                var deliveryOrders = this.GetIsReceptionDelivery(orders, linesProducts);
                if (deliveryOrders.Item1 || deliveryOrders.Item2)
                {
                    cardToReturns.CardDelivery.AddRange(await this.GenerateCardForReceptionDelivery(userOrders, lineProducts, deliveryOrders.Item1, deliveryOrders.Item2));
                }

                var invoiceOrder = this.GetIsPackageInvoice(orders, lineProducts);
                if (invoiceOrder.Item1 || invoiceOrder.Item2)
                {
                    cardToReturns.CardInvoice.AddRange(await this.GenerateCardForPackageInvoice(userOrders, lineProducts));
                }
            }

            return cardToReturns;
        }

        private Tuple<UserOrderModel, LineProductsModel> GetIsReceptionOrders(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
           var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)) && string.IsNullOrEmpty(x.StatusInvoice) && x.DeliveryId == 0);
           var lineProductOrder = lineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Recibir && x.DeliveryId == 0);
           return new Tuple<UserOrderModel, LineProductsModel>(userOrder, lineProductOrder);
        }

        private async Task<AlmacenSalesHeaderModel> GenerateCardForReceptionOrders(List<LineProductsModel> lineProducts, UserOrderModel userOrderHeader, LineProductsModel lineProductHeader)
        {
            var order = new CompleteAlmacenOrderModel();
            var status = string.Empty;
            var totalItems = 0;
            var totalPieces = 0;
            var productType = string.Empty;
            var invoiceType = string.Empty;
            var saporders = new List<CompleteAlmacenOrderModel>();

            if (userOrderHeader != null)
            {
                saporders = (await this.sapDao.GetAllOrdersForAlmacenById(int.Parse(userOrderHeader.Salesorderid))).ToList();
                order = saporders.FirstOrDefault();
                status = userOrderHeader.Status == ServiceConstants.Finalizado && userOrderHeader.StatusAlmacen == ServiceConstants.BackOrder ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;
                productType = lineProducts.Any(x => x.SaleOrderId == int.Parse(userOrderHeader.Salesorderid)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
            }
            else
            {
                saporders = (await this.sapDao.GetAllOrdersForAlmacenById(lineProductHeader.SaleOrderId)).ToList();
                order = saporders.FirstOrDefault();
                /*Pending*/
                status = lineProductHeader.StatusAlmacen == ServiceConstants.Recibir ? ServiceConstants.PorRecibir : ServiceConstants.BackOrder;
                productType = ServiceConstants.Line;
            }

            invoiceType = order.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
            totalItems = saporders.Count;
            totalPieces = (int)saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);

            var saleHeader = new AlmacenSalesHeaderModel
            {
                Client = order.Cliente,
                DocNum = order.DocNum,
                Doctor = order.Medico,
                InitDate = order.FechaInicio,
                Status = status,
                TotalItems = totalItems,
                TotalPieces = totalPieces,
                TypeSaleOrder = $"Pedido {productType}",
                InvoiceType = invoiceType,
            };

            return saleHeader;
        }

        private Tuple<bool, bool> GetIsReceptionDelivery(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var userOrder = userOrders.Any() && userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Any(x => x.StatusAlmacen == ServiceConstants.Almacenado && string.IsNullOrEmpty(x.StatusInvoice) && x.DeliveryId != 0);
            var lineProductOrder = lineProducts.Any() && lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).Any(x => x.StatusAlmacen == ServiceConstants.Almacenado && string.IsNullOrEmpty(x.StatusInvoice) && x.DeliveryId != 0);

            return new Tuple<bool, bool>(userOrder, lineProductOrder);
        }

        private async Task<List<AlmacenSalesHeaderModel>> GenerateCardForReceptionDelivery(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, bool userOrder, bool lineProduct)
        {
            var saleHeader = new List<AlmacenSalesHeaderModel>();
            var saleHeaderItem = new AlmacenSalesHeaderModel();

            var deliverysId = userOrders.Select(x => x.DeliveryId).Distinct().ToList();
            deliverysId.AddRange(lineProducts.Select(x => x.DeliveryId).Distinct());
            deliverysId = deliverysId.Where(x => x != 0).Distinct().ToList();

            var deliveryDetailDb = (await this.sapDao.GetDeliveryByDocEntry(deliverysId)).ToList();
            var deliveryHeaders = (await this.sapDao.GetDeliveryModelByDocNum(deliveryDetailDb.Select(x => x.DeliveryId).Distinct().ToList())).ToList();
            var listIdsDelivery = deliveryDetailDb.Select(x => x.DeliveryId).Distinct().ToList();

            var productsIds = deliveryDetailDb.Where(x => listIdsDelivery.Contains(x.DeliveryId)).Select(y => y.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();

            foreach (var d in listIdsDelivery)
            {
                var header = deliveryHeaders.FirstOrDefault(x => x.DocNum == d);
                var deliveryDetail = deliveryDetailDb.Where(x => x.DeliveryId == d).DistinctBy(x => x.ProductoId).ToList();
                var saleOrder = deliveryDetail.FirstOrDefault() != null ? deliveryDetail.FirstOrDefault().BaseEntry : 0;
                var doctor = header == null ? string.Empty : header.Medico;
                var totalItems = deliveryDetail.Count;
                var totalPieces = deliveryDetail.Sum(x => x.Quantity);
                var invoiceType = header.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
                var productType = string.Empty;

                var hasInvoice = deliveryDetailDb.Any(d => d.InvoiceId.HasValue && d.InvoiceId.Value != 0);

                var userOrdersBySale = userOrders.Where(x => x.Salesorderid == saleOrder.ToString()).ToList();
                var lineProductsBySale = lineProducts.Where(x => x.SaleOrderId == saleOrder).ToList();

                var productList = this.GenerateListProductType(deliveryDetail, productItems);
                productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;

                saleHeaderItem = new AlmacenSalesHeaderModel
                {
                    Client = header.Cliente,
                    DocNum = saleOrder,
                    Doctor = header.Medico,
                    InitDate = header.FechaInicio,
                    Status = ServiceConstants.Almacenado,
                    Remision = d,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
                };
                saleHeader.Add(saleHeaderItem);
            }

            return saleHeader;
        }

        private List<ProductListModel> GenerateListProductType(List<DeliveryDetailModel> deliveryDetails, List<ProductoModel> products)
        {
            var listToReturn = new List<ProductListModel>();
            foreach (var order in deliveryDetails)
            {
                var item = products.FirstOrDefault(x => order.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var productModel = new ProductListModel
                {
                    IsMagistral = item.IsMagistral.Equals("Y"),
                };
                listToReturn.Add(productModel);
            }

            return listToReturn;
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

        private Tuple<bool, bool> GetIsPackageInvoice(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var userOrder = userOrders.Any() && userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Any(x => x.StatusInvoice == ServiceConstants.Almacenado || x.StatusInvoice == ServiceConstants.Empaquetado);
            var lineProductOrder = lineProducts.Any() && lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).Any(x => x.StatusInvoice == ServiceConstants.Almacenado || x.StatusInvoice == ServiceConstants.Empaquetado);

            return new Tuple<bool, bool>(userOrder, lineProductOrder);
        }

        private async Task<List<InvoiceSaleHeaderModel>> GenerateCardForPackageInvoice(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var saleHeader = new List<InvoiceSaleHeaderModel>();

            var deliverysId = userOrders.Select(x => x.DeliveryId).Distinct().ToList();
            deliverysId.AddRange(lineProducts.Select(x => x.DeliveryId).Distinct());
            deliverysId = deliverysId.Where(x => x != 0).Distinct().ToList();

            var deliveryDetails = (await this.sapDao.GetDeliveryByDocEntry(deliverysId)).ToList();
            var invoicesId = deliveryDetails.Where(x => x.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList();

            var invoiceHeaders = (await this.sapDao.GetInvoiceHeaderByInvoiceId(invoicesId)).ToList();
            var invoiceDetails = (await this.sapDao.GetInvoiceDetailByDocEntry(invoicesId)).ToList();

            // consulta de remiisones a almacen
            var deliverysListStatus = null;

            foreach (var invoice in invoiceHeaders)
            {
                var invoiceDetailsItem = invoiceDetails.Where(x => x.InvoiceId == invoice.InvoiceId).ToList();
                var deliveryDetailsItem = deliveryDetails.Where(x => x.InvoiceId.HasValue && x.InvoiceId.Value == invoice.InvoiceId).ToList();

                var salesId = deliveryDetails.Select(x => x.BaseEntry).ToList();
                var userOrdersItems = userOrders.Where(x => salesId.Contains(int.Parse(x.Salesorderid))).ToList();
                var lineProductsItems = lineProducts.Where(x => salesId.Contains(x.SaleOrderId)).ToList();

                deliveryDetailsItem.DistinctBy(x => x.DeliveryId).ToList()
                   .ForEach(y =>
                   {
                   });
            }

            return saleHeader;
        }
    }
}
