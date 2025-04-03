// <summary>
// <copyright file="AlmacenOrderDxpService.cs" company="Axity">
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
    using System.Numerics;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
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
    /// Class for almacen order doctor service.
    /// </summary>
    public class AlmacenOrderDxpService : IAlmacenOrderDxpService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        private readonly IRedisService redisService;

        private readonly IProccessPayments proccessPayments;

        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDxpService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        /// <param name="proccessPayments">the proccess payments.</param>
        /// <param name="doctorService">the doctor service.</param>
        public AlmacenOrderDxpService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService, IProccessPayments proccessPayments, IDoctorService doctorService)
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
        public async Task<ResultModel> SearchAlmacenOrdersByDxpId(Dictionary<string, string> parameters)
        {
            var typesString = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.AllTypesByDxp);
            var types = typesString.Split(",").ToList();

            var startDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                            .ToUniversalDateTime().Date;

            var endDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                                      .ToUniversalDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var userOrders = await ServiceUtilsAlmacen.GetUserOrdersAlmacenLeftList(this.pedidosService, startDate, endDate);

            var lineProductsTuple = await ServiceUtilsAlmacen.GetLineProductsAlmacenLeftList(
                this.almacenService,
                userOrders.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList(),
                startDate,
                endDate);

            var sapOrders = await this.GetSapOrders(userOrders, startDate, endDate, lineProductsTuple, types);
            var ordersByFilter = await this.GetSapOrdersToLookByDoctor(sapOrders.Item1, parameters);
            var totalFilter = ordersByFilter.Select(x => x.DocNumDxp).Distinct().ToList().Count;

            var sapClasification = await this.sapDao.GetClassifications(types.Where(x => !ServiceConstants.DefaultFilters.Contains(x)).ToList());
            var listToReturn = this.GetCardOrdersToReturn(ordersByFilter, parameters, sapOrders.Item2, sapOrders.Item3, sapClasification);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersDetailsByDxpId(DoctorOrdersSearchDeatilDto details)
        {
            var sapOrders = await this.sapDao.GetSapOrderDetailForAlmacenRecepcionById(details.SaleOrders);
            var ordersByDxp = await this.sapDao.GetCountOrdersWIthDetailByDocNumDxpJoinProduct(details.DxpId);
            var totalOrdersWithDeliverys = ordersByDxp.Where(ord => ServiceShared.CalculateAnd(ord.PedidoStatus == "C", ord.Canceled != "Y"))
                .Select(ord => ord.DocNum)
                .Distinct().Count();
            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
            var userOrdersResponse = await this.pedidosService.PostPedidos(details.SaleOrders, ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrdersResponse.Response.ToString());
            var transactionsIds = sapOrders.Where(o => !string.IsNullOrEmpty(o.DocNumDxp)).Select(o => o.DocNumDxp).Distinct().ToList();
            var payments = await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, transactionsIds);

            var order = sapOrders.FirstOrDefault();
            var adressesToFind = sapOrders.Select(x => new GetDoctorAddressModel { CardCode = x.CardCode, AddressId = x.DeliveryAddressId }).UtilsDistinctBy(y => y.CardCode).ToList();
            var doctorPrescriptionData = await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, adressesToFind);
            var doctor = doctorPrescriptionData.FirstOrDefault(x => x.AddressId == order.DeliveryAddressId);
            doctor ??= new DoctorDeliveryAddressModel { Contact = order.Cliente };

            var saleModel = new SalesByDoctorModel
            {
                AlmacenHeaderByDoctor = new AlmacenSalesByDoctorHeaderModel
                {
                    Client = ServiceShared.CalculateTernary(string.IsNullOrEmpty(doctor.Contact), order.Medico, doctor.Contact),
                    Address = details.Address,
                    Doctor = details.Name,
                    TotalItems = sapOrders.Count(y => y.Detalles != null),
                    TotalPieces = sapOrders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity),
                    IsPackage = details.IsPackage,
                    DxpId = details.DxpId.GetShortShopTransaction(),
                    TotalShopOrders = ordersByDxp.GroupBy(x => x.DocNum).Count(),
                    TotalOrdersWithDelivery = totalOrdersWithDeliverys,
                    IsOmigenomics = sapOrders.Exists(x => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsSecondary))),
                },
                Items = ServiceUtilsAlmacen.GetTotalOrdersForDoctorAndDxp(sapOrders, localNeigbors, userOrders, payments),
            };

            return ServiceUtils.CreateResult(true, 200, null, saleModel, null, null);
        }

        /// <summary>
        /// Gets the sap orders.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, List<string>, List<string>>> GetSapOrders(
            List<UserOrderModel> userOrders,
            DateTime startDate,
            DateTime endDate,
            Tuple<List<LineProductsModel>,
            List<int>> lineProductTuple,
            List<string> types)
        {
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(this.sapDao, userOrders, startDate, endDate, lineProductTuple, true);
            var orderWithPackages = sapOrders.Where(x => x.IsPackage == ServiceConstants.IsPackage).Select(p => p.DocNumDxp).Distinct().ToList();
            var orderWithOmigenomics = sapOrders.Where(x => ServiceUtils.CalculateTernary(!string.IsNullOrEmpty(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsOmigenomics), ServiceConstants.IsOmigenomicsValue.Contains(x.IsSecondary))).Select(p => p.DocNumDxp).Distinct().ToList();
            var sapCancelled = sapOrders.Where(x => x.Canceled == "Y").ToList();
            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !userOrders.Any(y => y.Salesorderid == x.DocNum.ToString())).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();

            sapOrders = ServiceUtilsAlmacen.GetOrdersValidsToReceiveByProducts(userOrders, lineProductTuple.Item1, sapOrders);
            sapOrders = sapOrders.Where(x => x.PedidoMuestra != ServiceConstants.IsSampleOrder).ToList();
            sapOrders.AddRange(sapCancelled);
            return new Tuple<List<CompleteAlmacenOrderModel>, List<string>, List<string>>(ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, true), orderWithPackages, orderWithOmigenomics);
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            sapOrders = await ServiceUtilsAlmacen.FilterSapOrdersByTypeShipping(sapOrders, parameters, this.proccessPayments, this.redisService, this.catalogsService);

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            var chip = parameters[ServiceConstants.Chips].ToLower().Remove(0, 1);
            return sapOrders.Where(x =>
                                        x.DocNumDxp.GetShortShopTransaction().ToLower() == chip ||
                                        x.DocNumDxp.Contains(chip)).ToList();
        }

        private AlmacenOrdersByDoctorModel GetCardOrdersToReturn(
            List<CompleteAlmacenOrderModel> sapOrders,
            Dictionary<string, string> parameters,
            List<string> orderWithPackages,
            List<string> orderWithOmigenomics,
            IEnumerable<LblContainerModel> classifications)
        {
            var dxpIds = sapOrders.Select(x => x.DocNumDxp).Distinct().OrderBy(x => x).ToList();
            dxpIds = ServiceShared.GetOffsetLimit(dxpIds, parameters);

            var listToReturn = new AlmacenOrdersByDoctorModel
            {
                SalesOrders = new List<SalesByDoctorModel>(),
                TotalSalesOrders = 0,
            };

            foreach (var dxpId in dxpIds)
            {
                var orders = sapOrders.Where(x => x.DocNumDxp == dxpId).ToList();
                var totalOrders = orders.DistinctBy(x => x.DocNum).Count();
                var totalItems = orders.DistinctBy(y => new { y.DocNum, y.Detalles.ProductoId }).Count();
                var doctorAddress = sapOrders.FirstOrDefault(x => x.DocNumDxp == dxpId);
                var address = doctorAddress?.Address?.Replace("\r", " ").Replace("  ", " ").ToUpper() ?? string.Empty;

                var sale = new AlmacenSalesByDoctorModel
                {
                    Doctor = orders.FirstOrDefault().Medico,
                    DxpId = dxpId.GetShortShopTransaction(),
                    Address = address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                    SaleOrderId = orders.Select(x => x.DocNum).Distinct().ToList(),
                    IsPackage = orderWithPackages.Any(dwp => dwp == dxpId),
                    IsOmigenomics = orderWithOmigenomics.Any(dwo => dwo == dxpId),
                    SaleOrderType = ServiceUtils.GetOrderTypeDescription(orders.Select(x => x.TypeOrder).Distinct().ToList(), classifications),
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
