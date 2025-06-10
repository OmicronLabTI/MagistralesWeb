// <summary>
// <copyright file="AlmacenOrderDoctorService.cs" company="Axity">
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
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for almacen order doctor service.
    /// </summary>
    public class AlmacenOrderDoctorService : IAlmacenOrderDoctorService
    {
        private readonly ISapDao sapDao;

        private readonly IAlmacenService almacenService;

        private readonly IPedidosService pedidosService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDoctorService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="pedidosService">The pedidos service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        public AlmacenOrderDoctorService(ISapDao sapDao, IAlmacenService almacenService, IPedidosService pedidosService, ICatalogsService catalogsService, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrderdetail(int saleorderid)
        {
            var details = await this.sapDao.GetSapOrderDetailForAlmacenRecepcionById(new List<int> { saleorderid });
            var productItems = await this.sapDao.GetProductByIds(details.Select(x => x.Detalles.ProductoId).ToList());
            var saleDetails = (await this.sapDao.GetAllDetails(new List<int?> { saleorderid })).ToList();
            var listDetails = new List<AlmacenDetailsOrder>();
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { saleorderid });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());
            var filterDetails = await this.GetFilterDetails(details, saleorderid);
            foreach (var detail in filterDetails)
            {
                var item = productItems.FirstOrDefault(x => x.ProductoId == detail.Detalles.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var productType = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);
                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == detail.Detalles.ProductoId);
                var orderId = saleDetail?.OrdenFabricacionId ?? 0;
                var itemcode = ServiceShared.CalculateTernary(orderId != 0, $"{item.ProductoId} - {orderId}", item.ProductoId);

                var incidentdb = incidents.FirstOrDefault(x => x.SaleOrderId == saleorderid && x.ItemCode == item.ProductoId);
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = ServiceShared.DeserializeObject(incidentdb.Batches, new List<AlmacenBatchModel>()),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };
                var detailItem = new AlmacenDetailsOrder
                {
                    ItemCode = itemcode,
                    Description = item.LargeDescription.ToUpper(),
                    NeedsCooling = item.NeedsCooling,
                    ProductType = $"Producto {productType}",
                    Pieces = detail.Detalles.Quantity,
                    Container = detail.Detalles.Container,
                    Status = ServiceShared.CalculateTernary(detail.Detalles.CanceledOrder == "Y", ServiceConstants.Cancelado, ServiceConstants.PorRecibir),
                    Incident = ServiceShared.CalculateTernary(string.IsNullOrEmpty(localIncident.Status), null, localIncident),
                };
                listDetails.Add(detailItem);
            }

            return ServiceUtils.CreateResult(true, 200, null, listDetails, null, null);
        }

        private async Task<List<CompleteRecepcionPedidoDetailModel>> GetFilterDetails(List<CompleteRecepcionPedidoDetailModel> sapOrders, int saleorderid)
        {
            var userOrdersResponse = await this.pedidosService.PostPedidos(new List<int> { saleorderid }, ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            var lineResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLinesBySaleOrder, new List<int> { saleorderid });
            var lineOrders = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineResponse.Response.ToString());
            var sapOrdersConfiguration = await ServiceUtils.GetRouteConfigurationsForProducts(this.catalogsService, this.redisService, ServiceConstants.AlmacenDbValue);
            var usersOrdersIds = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(x => int.Parse(x.Productionorderid));
            return await ServiceUtilsAlmacen.GetFilterSapOrdersByConfig(sapOrders, userOrders, lineOrders, this.catalogsService, this.redisService);
        }
    }
}
