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
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Utils;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDxpService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        /// <param name="redisService">thre redis service.</param>
        public AlmacenOrderDxpService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService, IRedisService redisService)
        {
            this.sapDao = sapDao.ThrowIfNull(nameof(sapDao));
            this.pedidosService = pedidosService.ThrowIfNull(nameof(pedidosService));
            this.almacenService = almacenService.ThrowIfNull(nameof(almacenService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDxpId(Dictionary<string, string> parameters)
        {
            var typesString = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.AllTypesByDxp);
            var types = typesString.Split(",").ToList();

            var userOrdersTuple = await ServiceUtilsAlmacen.GetUserOrdersAlmacenLeftList(this.pedidosService);
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await ServiceUtilsAlmacen.GetLineProductsAlmacenLeftList(this.almacenService, ids, userOrdersTuple.Item3);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple, types);
            var ordersByFilter = await this.GetSapOrdersToLookByDoctor(sapOrders.Item1, parameters);
            var totalFilter = ordersByFilter.Select(x => x.DocNumDxp).Distinct().ToList().Count;

            var listToReturn = this.GetCardOrdersToReturn(ordersByFilter, parameters, sapOrders.Item2);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{totalFilter}-{totalFilter}");
        }

        public async Task<ResultModel> SearchAlmacenOrdersDetailsByDxpId(DoctorOrdersSearchDeatilDto details)
        {

        }

        /// <summary>
        /// Gets the sap orders.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, List<string>>> GetSapOrders(Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple, List<string> types)
        {
            var lineProducts = await ServiceUtils.GetLineProducts(this.sapDao, this.redisService);

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(this.sapDao, userOrdersTuple, lineProductTuple, true);
            var orderWithPackages = sapOrders.Where(x => x.IsPackage == ServiceConstants.IsPackage).Select(p => p.DocNumDxp).Distinct().ToList();
            sapOrders = sapOrders.Where(x => x.IsPackage != ServiceConstants.IsPackage).ToList();
            var sapCancelled = sapOrders.Where(x => x.Canceled == "Y").ToList();
            sapOrders = sapOrders.Where(x => x.Canceled == "N").ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !userOrdersTuple.Item1.Any(y => y.Salesorderid == x.DocNum.ToString())).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();

            sapOrders = ServiceUtilsAlmacen.GetOrdersValidsToReceiveByProducts(userOrdersTuple.Item1, lineProductTuple.Item1, sapOrders);
            sapOrders = sapOrders.Where(x => x.PedidoMuestra != ServiceConstants.OrderTypeMU).ToList();
            sapOrders.AddRange(sapCancelled);
            return new Tuple<List<CompleteAlmacenOrderModel>, List<string>>(ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, lineProducts).Item1, orderWithPackages);
        }

        private async Task<List<CompleteAlmacenOrderModel>> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (ServiceShared.IsValidFilterByTypeShipping(parameters))
            {
                var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService, this.redisService);
                sapOrders = sapOrders.Where(x => ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, x.Address.ValidateNull()) == ServiceUtils.IsLocalString(parameters[ServiceConstants.Shipping])).ToList();
            }

            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            var dxpOrder = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => dxpOrder.All(y => x.DocNumDxp.ValidateNull().ToLower().Contains(y.ToLower()))).ToList();
        }

        private AlmacenOrdersByDoctorModel GetCardOrdersToReturn(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters, List<string> orderWithPackages)
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
                    DxpId = dxpId,
                    Address = address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                    SaleOrderId = orders.Select(x => x.DocNum).Distinct().ToList(),
                    IsPackage = orderWithPackages.Any(dwp => dwp == dxpId),
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
