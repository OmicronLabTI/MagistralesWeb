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

            return new ResultModel();
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
        /// <param name="userOrders">the user orders list.</param>
        /// <returns>the data.</returns>
        private async Task<CardsAdvancedLook> GetStatusToSearch(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var cardToReturns = new CardsAdvancedLook();

            var receptionOrders = this.GetIsReceptionOrders(userOrders, lineProducts);
            if (receptionOrders.Item1 != null || receptionOrders.Item2 != null)
            {
                cardToReturns.CardOrder.Add(await this.GenerateCardForReceptionOrders(userOrders, lineProducts, receptionOrders.Item1, receptionOrders.Item2));
            }

            var deliveryOrders = this.GetIsReceptionDelivery(userOrders, lineProducts);
            if (deliveryOrders.Item1 || deliveryOrders.Item2)
            {
                cardToReturns.CardOrder = await this.GenerateCardForReceptionDelivery(userOrders, lineProducts, deliveryOrders.Item1, deliveryOrders.Item2);
            }

            return cardToReturns;
        }

        private Tuple<UserOrderModel, LineProductsModel> GetIsReceptionOrders(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && ServiceConstants.StatusReceptionOrders.Contains(x.Status) && (ServiceConstants.StatusAlmacenReceptionOrders.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)) && string.IsNullOrEmpty(x.StatusInvoice));
            var lineProductOrder = lineProducts.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Recibir);

            return new Tuple<UserOrderModel, LineProductsModel>(userOrder, lineProductOrder);
        }

        private async Task<AlmacenSalesHeaderModel> GenerateCardForReceptionOrders(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, UserOrderModel userOrderHeader, LineProductsModel lineProductHeader)
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
            var userOrder = userOrders.Count() > 0 ? userOrders.All(x => !string.IsNullOrEmpty(x.Productionorderid) && (x.StatusAlmacen == ServiceConstants.Almacenado)) : false;
            var lineProductOrder = lineProducts.Count > 0 ? lineProducts.All(x => !string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Almacenado) : false;

            return new Tuple<bool, bool>(userOrder, lineProductOrder);
        }

        private async Task<AlmacenSalesHeaderModel> GenerateCardForReceptionDelivery(List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, bool userOrder, bool lineProduct)
        {
            var saleHeader = new AlmacenSalesHeaderModel();

            return saleHeader;
        }
    }
}
