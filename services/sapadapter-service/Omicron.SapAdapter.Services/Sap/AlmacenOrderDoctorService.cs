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

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        private readonly IProccessPayments proccessPayments;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDoctorService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        /// <param name="proccessPayments">the proccess payments.</param>
        public AlmacenOrderDoctorService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.proccessPayments = proccessPayments.ThrowIfNull(nameof(proccessPayments));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters)
        {
            var typesString = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.AllTypesByDoctor);
            var types = typesString.Split(",").ToList();

            var userOrdersTuple = await ServiceUtilsAlmacen.GetUserOrdersAlmacenLeftList(this.pedidosService);
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await ServiceUtilsAlmacen.GetLineProductsAlmacenLeftList(this.almacenService, ids, userOrdersTuple.Item3);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple, types);
            var ordersByFilter = await this.GetSapOrdersToLookByDoctor(sapOrders.Item1, parameters);
            var totalFilter = ordersByFilter.Select(x => x.Medico).Distinct().ToList().Count;

            var listToReturn = this.GetCardOrdersToReturn(ordersByFilter, parameters, sapOrders.Item2);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersDetailsByDoctor(DoctorOrdersSearchDeatilDto details)
        {
            var sapOrders = await this.sapDao.GetSapOrderDetailForAlmacenRecepcionById(details.SaleOrders);
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var userOrdersResponse = await this.pedidosService.PostPedidos(details.SaleOrders, ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            var transactionsIds = sapOrders.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
            var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);
            var saleModel = new SalesByDoctorModel();

            saleModel.AlmacenHeaderByDoctor = new AlmacenSalesByDoctorHeaderModel
            {
                Address = details.Address,
                Doctor = details.Name,
                TotalItems = sapOrders.Count(y => y.Detalles != null),
                TotalPieces = sapOrders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                IsPackage = details.IsPackage,
            };

            saleModel.Items = ServiceUtilsAlmacen.GetTotalOrdersForDoctorAndDxp(sapOrders.ToList(), localNeigbors, userOrders, payments);

            return ServiceUtils.CreateResult(true, 200, null, saleModel, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrderdetail(int saleorderid)
        {
            var details = await this.sapDao.GetPedidoByIdJoinProduct(saleorderid);
            var productItems = await this.sapDao.GetProductByIds(details.Select(x => x.ProductoId).ToList());
            var saleDetails = (await this.sapDao.GetAllDetails(new List<int?> { saleorderid })).ToList();
            var listDetails = new List<AlmacenDetailsOrder>();
            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, new List<int> { saleorderid });
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());
            foreach (var detail in details)
            {
                var item = productItems.FirstOrDefault(x => x.ProductoId == detail.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var productType = ServiceShared.CalculateTernary(item.IsMagistral.Equals("Y"), ServiceConstants.Magistral, ServiceConstants.Linea);
                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == detail.ProductoId);
                var orderId = saleDetail?.OrdenFabricacionId.ToString() ?? string.Empty;
                var itemcode = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(orderId), $"{item.ProductoId} - {orderId}", item.ProductoId);

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
                    Pieces = detail.Quantity,
                    Container = detail.Container,
                    Status = ServiceShared.CalculateTernary(detail.CanceledOrder == "Y", ServiceConstants.Cancelado, ServiceConstants.PorRecibir),
                    Incident = ServiceShared.CalculateTernary(string.IsNullOrEmpty(localIncident.Status), null, localIncident),
                };
                listDetails.Add(detailItem);
            }

            return ServiceUtils.CreateResult(true, 200, null, listDetails, null, null);
        }

        /// <summary>
        /// Gets the sap orders.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, List<string>>> GetSapOrders(Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple, List<string> types)
        {
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(this.sapDao, userOrdersTuple, lineProductTuple, false);
            var doctorWithPackages = sapOrders.Where(x => x.IsPackage == ServiceConstants.IsPackage).Select(p => p.Medico).Distinct().ToList();
            sapOrders = sapOrders.Where(x => x.IsPackage != ServiceConstants.IsPackage).ToList();
            var sapCancelled = sapOrders.Where(x => x.Canceled == "Y").ToList();
            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !userOrdersTuple.Item1.Any(y => y.Salesorderid == x.DocNum.ToString())).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();

            sapOrders = ServiceUtilsAlmacen.GetOrdersValidsToReceiveByProducts(userOrdersTuple.Item1, lineProductTuple.Item1, sapOrders);
            sapOrders = sapOrders.Where(x => x.PedidoMuestra != ServiceConstants.OrderTypeMU).ToList();
            sapOrders.AddRange(sapCancelled);
            return new Tuple<List<CompleteAlmacenOrderModel>, List<string>>(ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, lineProducts).Item1, doctorWithPackages);
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            sapOrders = await ServiceUtilsAlmacen.FilterSapOrdersByTypeShipping(sapOrders, parameters, this.proccessPayments, this.redisService, this.catalogsService);

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            var doctorName = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => doctorName.All(y => x.Medico.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets card for doctor.
        /// </summary>
        /// <returns>the data.</returns>
        private AlmacenOrdersByDoctorModel GetCardOrdersToReturn(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters, List<string> doctorWithPackages)
        {
            var doctors = sapOrders.Select(x => x.Medico).Distinct().OrderBy(x => x).ToList();
            doctors = ServiceShared.GetOffsetLimit(doctors, parameters);

            var listToReturn = new AlmacenOrdersByDoctorModel
            {
                SalesOrders = new List<SalesByDoctorModel>(),
                TotalSalesOrders = 0,
            };

            foreach (var doctor in doctors)
            {
                var orders = sapOrders.Where(x => x.Medico == doctor).ToList();
                var totalOrders = orders.DistinctBy(x => x.DocNum).Count();
                var totalItems = orders.DistinctBy(y => new { y.DocNum, y.Detalles.ProductoId }).Count();
                var doctorAddress = sapOrders.FirstOrDefault(x => x.Medico == doctor);
                var address = doctorAddress?.Address?.Replace("\r", " ").Replace("  ", " ").ToUpper() ?? string.Empty;

                var sale = new AlmacenSalesByDoctorModel
                {
                    Doctor = doctor,
                    Address = address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                    SaleOrderId = orders.Select(x => x.DocNum).Distinct().ToList(),
                    IsPackage = doctorWithPackages.Any(dwp => dwp == doctor),
                };

                var saleModel = new SalesByDoctorModel
                {
                    AlmacenSalesByDoctor = sale,
                    AlmacenHeaderByDoctor = null,
                    Items = null,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }
    }
}
