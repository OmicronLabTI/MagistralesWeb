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
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// Class for almacen order doctor service.
    /// </summary>
    public class AlmacenOrderDoctorService : IAlmacenOrderDoctorService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenOrderDoctorService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        public AlmacenOrderDoctorService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters)
        {
            var doctorValue = parameters.ContainsKey(ServiceConstants.Doctor) ? parameters[ServiceConstants.Doctor].Split(",").ToList() : new List<string>();

            // get user orders
            var userOrdersTuple = await this.GetUserOrders();
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await this.GetLineProducts(ids);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple);
            var ordersByFilter = this.FilterByStatusAndDoctor(doctorValue, sapOrders.Item1, userOrdersTuple, lineProductsTuple);
            return ServiceUtils.CreateResult(true, 200, null, ordersByFilter.Item1, null, ordersByFilter.Item2);
        }

        private async Task<Tuple<List<UserOrderModel>, List<int>, DateTime>> GetUserOrders()
        {
            var userOrderModel = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());

            var listIds = JsonConvert.DeserializeObject<List<int>>(userOrderModel.ExceptionMessage);

            int.TryParse(userOrderModel.Comments.ToString(), out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));

            return new Tuple<List<UserOrderModel>, List<int>, DateTime>(userOrders, listIds, dateToLook);
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProducts(List<int> magistralIds)
        {
            var lineProductsResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLineProductPedidos, magistralIds);
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var listProductToReturn = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).ToList();
            listProductToReturn.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).ToList());
            var listIdToIgnore = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && ServiceConstants.StatusToIgnoreLineProducts.Contains(x.StatusAlmacen)).Select(y => y.SaleOrderId).ToList();

            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        /// <summary>
        /// Gets the sap orders.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, int>> GetSapOrders(Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple)
        {
            var idsToIgnore = userOrdersTuple.Item2;
            idsToIgnore.AddRange(lineProductTuple.Item2);

            var lineProducts = (await this.sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();

            var sapOrders = (await this.sapDao.GetAllOrdersForAlmacen(userOrdersTuple.Item3)).ToList();
            sapOrders = sapOrders.Where(x => x.Detalles != null).ToList();
            sapOrders = sapOrders.Where(x => !idsToIgnore.Contains(x.DocNum)).ToList();

            var orderToAppear = userOrdersTuple.Item1.Select(x => x.Salesorderid).ToList();
            var ordersSapMaquila = (await this.sapDao.GetAllOrdersForAlmacenByTypeOrder(ServiceConstants.OrderTypeMQ)).ToList();
            ordersSapMaquila = ordersSapMaquila.Where(x => x.Detalles != null).ToList();
            ordersSapMaquila = ordersSapMaquila.Where(x => orderToAppear.Contains(x.DocNum.ToString())).ToList();

            ordersSapMaquila.ForEach(order =>
            {
                var orderSapExists = sapOrders.FirstOrDefault(x => x.DocNum == order.DocNum && x.Detalles.ProductoId == order.Detalles.ProductoId);
                if (orderSapExists == null)
                {
                    sapOrders.Add(order);
                }
            });

            var orderHeaders = (await this.sapDao.GetFabOrderBySalesOrderId(sapOrders.Select(x => x.DocNum).ToList())).ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !orderHeaders.Any(y => y.PedidoId.Value == x.DocNum)).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).ToList().Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();
            var granTotal = sapOrders.Select(x => x.DocNum).Distinct().ToList().Count;

            return new Tuple<List<CompleteAlmacenOrderModel>, int>(sapOrders, granTotal);
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private Tuple<List<CompleteAlmacenOrderModel>, List<int>> FilterByStatusAndDoctor(List<string> doctor, List<CompleteAlmacenOrderModel> sapOrders, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductsTuple)
        {
            var userModels = userOrdersTuple.Item1;
            var lineProducts = lineProductsTuple.Item1;
            var listToReturn = new List<CompleteAlmacenOrderModel>();

            var allIds = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
            allIds.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Select(y => y.SaleOrderId));

            var idsToLook = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid) && x.Status == ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen)).Select(y => int.Parse(y.Salesorderid)).ToList();
            idsToLook.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).Select(y => y.SaleOrderId));
            idsToLook.AddRange(sapOrders.Where(x => !allIds.Contains(x.DocNum)).Select(y => y.DocNum));
            listToReturn.AddRange(sapOrders.Where(x => idsToLook.Contains(x.DocNum)));

            listToReturn = listToReturn.Where(x => doctor.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
            return new Tuple<List<CompleteAlmacenOrderModel>, List<int>>(listToReturn, new List<int>());
        }
    }
}
