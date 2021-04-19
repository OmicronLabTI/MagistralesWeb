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
            var typesString = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.AllTypes;
            var types = typesString.Split(",").ToList();

            var userOrdersTuple = await this.GetUserOrders();
            var ids = userOrdersTuple.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProductsTuple = await this.GetLineProducts(ids);
            var sapOrders = await this.GetSapOrders(userOrdersTuple, lineProductsTuple, types);
            var ordersByFilter = this.FilterByStatusToReceive(sapOrders.Item1, userOrdersTuple, lineProductsTuple);
            ordersByFilter = this.GetSapOrdersToLookByDoctor(ordersByFilter, parameters);
            var totalFilter = ordersByFilter.Select(x => x.Medico).Distinct().ToList().Count;
            var listToReturn = await this.GetCardOrdersToReturn(ordersByFilter, userOrdersTuple.Item1, lineProductsTuple.Item1);
            listToReturn = this.GetDoctorsToProcess(listToReturn, parameters);
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{sapOrders.Item2}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrderdetail(int saleorderid)
        {
            var details = await this.sapDao.GetPedidoById(saleorderid);
            var productItems = await this.sapDao.GetProductByIds(details.Select(x => x.ProductoId).ToList());
            var saleDetails = (await this.sapDao.GetAllDetails(saleorderid)).ToList();
            var listDetails = new List<AlmacenDetailsOrder>();

            foreach (var detail in details)
            {
                var item = productItems.FirstOrDefault(x => x.ProductoId == detail.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };
                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;
                var saleDetail = saleDetails.FirstOrDefault(x => x.CodigoProducto == detail.ProductoId);
                var orderId = saleDetail == null ? string.Empty : saleDetail.OrdenFabricacionId.ToString();
                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;

                var detailItem = new AlmacenDetailsOrder
                {
                    ItemCode = itemcode,
                    Description = item.LargeDescription.ToUpper(),
                    ProductType = $"Producto {productType}",
                    Pieces = detail.Quantity,
                    Container = detail.Container,
                    Status = ServiceConstants.PorRecibir,
                };
                listDetails.Add(detailItem);
            }

            return ServiceUtils.CreateResult(true, 200, null, listDetails, null, null);
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
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, int>> GetSapOrders(Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple, List<string> types)
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
            var granTotal = sapOrders.Select(x => x.Medico).Distinct().ToList().Count;

            sapOrders = ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, orderHeaders, lineProducts);

            return new Tuple<List<CompleteAlmacenOrderModel>, int>(sapOrders, granTotal);
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private List<CompleteAlmacenOrderModel> FilterByStatusToReceive(List<CompleteAlmacenOrderModel> sapOrders, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductsTuple)
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

            return listToReturn;
        }

        private List<CompleteAlmacenOrderModel> GetSapOrdersToLookByDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Doctor))
            {
                return sapOrders;
            }

            var doctorName = parameters[ServiceConstants.Doctor].Split(",").ToList();
            return sapOrders.Where(x => doctorName.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets card for doctor.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersByDoctorModel> GetCardOrdersToReturn(List<CompleteAlmacenOrderModel> sapOrders, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            sapOrders = sapOrders.OrderByDescending(x => x.DocNum).ToList();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct().ToList();
            var doctors = sapOrders.Select(x => x.Medico).Distinct().ToList();

            var listToReturn = new AlmacenOrdersByDoctorModel
            {
                SalesOrders = new List<SalesByDoctorModel>(),
                TotalSalesOrders = 0,
            };

            var productsIds = sapOrders.Where(x => salesIds.Contains(x.DocNum)).Select(y => y.Detalles.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();
            var productsModel = (await this.sapDao.GetAllLineProducts()).ToList();
            foreach (var doctor in doctors)
            {
                var orders = sapOrders.Where(x => x.Medico == doctor).ToList();
                var totalOrders = orders.DistinctBy(x => x.DocNum).Count();
                var totalItems = orders.DistinctBy(y => new { y.DocNum, y.Detalles.ProductoId }).Count();
                var totalPieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctorAddress = sapOrders.Where(x => x.Medico == doctor).FirstOrDefault();

                var sale = new AlmacenSalesByDoctorModel
                {
                    Doctor = doctor,
                    Address = doctorAddress == null ? string.Empty : doctorAddress.Address,
                    TotalOrders = totalOrders,
                    TotalItems = totalItems,
                };

                var saleHeader = new AlmacenSalesByDoctorHeaderModel
                {
                    Doctor = doctor,
                    Address = doctorAddress == null ? string.Empty : doctorAddress.Address,
                    TotalItems = totalItems,
                    TotalPieces = totalPieces,
                };

                listOrders = this.GetTotalOrdersByDoctor(orders, userOrders, lineProducts, productsModel);

                var saleModel = new SalesByDoctorModel
                {
                    AlmacenSalesByDoctor = sale,
                    AlmacenHeaderByDoctor = saleHeader,
                    Items = listOrders,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        private List<OrderListByDoctorModel> GetTotalOrdersByDoctor(List<CompleteAlmacenOrderModel> sapOrders, List<UserOrderModel> userOrders, List<LineProductsModel> lineProducts, List<ProductoModel> productsModel)
        {
            var listOrders = new List<OrderListByDoctorModel>();
            var salesIds = sapOrders.Select(x => x.DocNum).Distinct();

            foreach (var so in salesIds)
            {
                // var saleDetail = (await this.sapDao.GetAllDetails(so)).ToList();
                var orders = sapOrders.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));
                var lineOrders = lineProducts.Where(x => x.SaleOrderId == so).ToList();

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctor = order == null ? string.Empty : order.Medico;

                var productType = orders.Any(x => x.Detalles != null && productsModel.Any(p => p.ProductoId == x.Detalles.ProductoId)) ? ServiceConstants.Mixto : ServiceConstants.Magistral;
                productType = orders.All(x => x.Detalles != null && productsModel.Select(p => p.ProductoId).Contains(x.Detalles.ProductoId)) ? ServiceConstants.Linea : productType;

                order.Address = string.IsNullOrEmpty(order.Address) ? string.Empty : order.Address;
                var invoiceType = order.Address.Contains(ServiceConstants.NuevoLeon) ? ServiceConstants.Local : ServiceConstants.Foraneo;
                var client = order == null ? string.Empty : order.Cliente;

                var saleItem = new OrderListByDoctorModel
                {
                    DocNum = so,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = ServiceConstants.PorRecibir,
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    InvoiceType = invoiceType,
                    Comments = string.Empty,
                };
                listOrders.Add(saleItem);
            }

            return listOrders;
        }

        private AlmacenOrdersByDoctorModel GetDoctorsToProcess(AlmacenOrdersByDoctorModel ordersByDoctorModel, Dictionary<string, string> parameters)
        {
            var salesByDoctorModel = ordersByDoctorModel.SalesOrders;
            salesByDoctorModel = salesByDoctorModel.OrderByDescending(x => x.AlmacenSalesByDoctor.Doctor).ToList();

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);
            ordersByDoctorModel.SalesOrders = salesByDoctorModel.Skip(offsetNumber).Take(limitNumber).ToList();
            ordersByDoctorModel.TotalSalesOrders = ordersByDoctorModel.SalesOrders.Count();
            return ordersByDoctorModel;
        }
    }
}
