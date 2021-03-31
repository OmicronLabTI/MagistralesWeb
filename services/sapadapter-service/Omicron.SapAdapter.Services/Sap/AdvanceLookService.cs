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
            var type = string.Empty;
            var listStatus = new List<string> { "Finalizado", "Liberado" };
            var listStatusAlmacen = new List<string> { "Back Order", "Recibir" };
            var userOrder = userOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid) && listStatus.Contains(x.Status) && (listStatusAlmacen.Contains(x.StatusAlmacen) || string.IsNullOrEmpty(x.StatusAlmacen)) && string.IsNullOrEmpty(x.StatusInvoice));
            var cardToReturns = new CardsAdvancedLook();

            if (userOrder != null)
            {
                var saporders = (await this.sapDao.GetAllOrdersForAlmacenById(int.Parse(userOrder.Salesorderid))).ToList();
                var order = saporders.FirstOrDefault();

                var status = userOrder.Status == ServiceConstants.Finalizado && userOrder.StatusAlmacen == ServiceConstants.BackOrder ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;
                var productType = lineProducts.Any(x => x.SaleOrderId == int.Parse(userOrder.Salesorderid)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                var invoiceType = order.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;

                var saleHeader = new AlmacenSalesHeaderModel
                {
                    Client = order.Cliente,
                    DocNum = order.DocNum,
                    Doctor = order.Medico,
                    InitDate = order.FechaInicio,
                    Status = status,
                    TotalItems = saporders.Count,
                    TotalPieces = saporders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
                };

                cardToReturns.CardOrder = saleHeader;
            }

            return cardToReturns;
        }

        private UserOrderModel GetCardsInOrders()
        {
            return new UserOrderModel();
        }
    }
}
