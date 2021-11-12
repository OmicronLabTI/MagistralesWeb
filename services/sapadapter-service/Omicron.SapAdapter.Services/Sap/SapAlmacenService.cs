// <summary>
// <copyright file="SapAlmacenService.cs" company="Axity">
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
    /// Class for sap almacen service.
    /// </summary>
    public class SapAlmacenService : ISapAlmacenService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IAlmacenService almacenService;

        private readonly ICatalogsService catalogsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        /// <param name="almacenService">The almacen service.</param>
        /// <param name="catalogsService">The catalog service.</param>
        public SapAlmacenService(ISapDao sapDao, IPedidosService pedidosService, IAlmacenService almacenService, ICatalogsService catalogsService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.almacenService = almacenService ?? throw new ArgumentException(nameof(almacenService));
            this.catalogsService = catalogsService ?? throw new ArgumentNullException(nameof(catalogsService));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var typesString = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.AllTypes;
            var types = typesString.Split(",").ToList();

            var listStatus = parameters.ContainsKey(ServiceConstants.Status) ? parameters[ServiceConstants.Status] : ServiceConstants.AllStatus;
            var status = listStatus.Split(",").ToList();

            var userResponse = await this.GetUserOrdersToLook();
            var ids = userResponse.Item1.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList();
            var lineProducts = await this.GetLineProductsToLook(ids);
            var sapOrders = await this.GetSapLinesToLook(types, userResponse, lineProducts);
            var orders = this.GetSapLinesToLookByStatus(sapOrders.Item1, userResponse.Item1, lineProducts.Item1, status);
            orders = this.GetSapLinesToLookByPedidoDoctor(orders, parameters);
            var totalFilter = orders.Select(x => x.DocNum).Distinct().ToList().Count;
            var listToReturn = await this.GetOrdersToReturn(userResponse.Item1, orders, lineProducts.Item1, parameters);

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{sapOrders.Item2}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetMagistralScannedData(string code)
        {
            var codeArray = code.Split("-");
            int.TryParse(codeArray[0], out var pedidoId);
            int.TryParse(codeArray[1], out var orderId);

            var details = (await this.sapDao.GetAllDetails(new List<int?> { pedidoId })).ToList();
            var order = details.FirstOrDefault(x => x.OrdenFabricacionId == orderId);
            order = order == null ? new CompleteDetailOrderModel() : order;

            var itemCode = (await this.sapDao.GetProductById(order.CodigoProducto)).FirstOrDefault();
            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

            var magistralData = new MagistralScannerModel
            {
                Container = order.Container,
                Description = itemCode.ProductoName,
                ItemCode = itemCode.ProductoId,
                NeedsCooling = itemCode.NeedsCooling,
                Pieces = order.QtyPlanned.Value,
                ProductType = $"Producto {productType}",
            };

            return ServiceUtils.CreateResult(true, 200, null, magistralData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetLineScannedData(string code)
        {
            var listBatchesModel = new List<LineProductBatchesModel>();

            var itemCode = (await this.sapDao.GetProductByCodeBar(code)).FirstOrDefault();

            if (itemCode == null)
            {
                return ServiceUtils.CreateResult(true, 404, null, new LineScannerModel(), null, null);
            }

            var listComponents = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { ProductId = itemCode.ProductoId, Warehouse = ServiceConstants.PT },
            };

            var validBatches = (await this.sapDao.GetValidBatches(listComponents)).ToList();
            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

            validBatches.ForEach(b =>
            {
                var batchDate = b.FechaExp ?? DateTime.Now.ToString("dd/MM/yyyy");
                var dateSplit = batchDate.Split("/");
                var fechaExp = new DateTime(int.Parse(dateSplit[2]), int.Parse(dateSplit[1]), int.Parse(dateSplit[0]));

                if (fechaExp >= DateTime.Today)
                {
                    var batch = new LineProductBatchesModel
                    {
                        Batch = b.DistNumber,
                        ExpDate = b.FechaExp,
                        AvailableQuantity = Math.Round(b.Quantity - b.CommitQty, 6),
                    };

                    listBatchesModel.Add(batch);
                }
            });

            var lineData = new LineScannerModel
            {
                Batches = listBatchesModel,
                Description = itemCode.ProductoName,
                ItemCode = itemCode.ProductoId,
                ProductType = $"Producto {productType}",
                NeedsCooling = itemCode.NeedsCooling,
            };

            return ServiceUtils.CreateResult(true, 200, null, lineData, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetCompleteDetail(int orderId)
        {
            var data = (await this.sapDao.GetAllOrdersForAlmacenById(orderId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, data, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersByIds(List<int> ordersId)
        {
            var data = (await this.sapDao.GetOrdersByIdJoinDoctor(ordersId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, data, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryBySaleOrderId(List<int> ordersId)
        {
            var data = (await this.sapDao.GetDeliveryDetailBySaleOrder(ordersId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, data, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> AlmacenGraphCount(Dictionary<string, string> parameters)
        {
            var dates = ServiceUtils.GetDateFilter(parameters);

            var lineProducts = (await this.sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();
            var details = (await this.sapDao.GetDetailsbyDocDate(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).GroupBy(x => x.PedidoId).ToList();
            var idsToReturnLine = new List<int>();
            var idsToReturnMix = new List<int>();
            details.ForEach(x =>
            {
                if (x.All(y => lineProducts.Contains(y.ProductoId)))
                {
                    idsToReturnLine.Add(x.Key.Value);
                }

                if (x.Any(y => lineProducts.Contains(y.ProductoId)) && !x.All(y => lineProducts.Contains(y.ProductoId)))
                {
                    idsToReturnMix.Add(x.Key.Value);
                }
            });

            return ServiceUtils.CreateResult(true, 200, null, idsToReturnLine, JsonConvert.SerializeObject(idsToReturnMix), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveryParties()
        {
            var parties = await this.sapDao.GetDeliveryCompanies();
            return ServiceUtils.CreateResult(true, 200, null, parties, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveries(List<int> deliveryIds)
        {
            var deliveries = await this.sapDao.GetDeliveryDetailByDocEntry(deliveryIds);
            var allDeliveries = await this.sapDao.GetDeliveryDetailBySaleOrder(deliveries.Select(x => x.BaseEntry).ToList());
            var detailsSale = (await this.sapDao.GetDetailByDocNum(deliveries.Select(x => x.BaseEntry).ToList())).ToList();
            var lineItems = await this.sapDao.GetAllLineProducts();

            detailsSale.ForEach(x =>
            {
                x.Label = lineItems.Any(y => y.ProductoId == x.ProductoId).ToString();
            });

            var objectToReturn = new { DeliveryDetail = allDeliveries, DetallePedido = detailsSale };
            return ServiceUtils.CreateResult(true, 200, null, objectToReturn, null, null);
        }

        /// <summary>
        /// Gets the orders that are finalized and all the productin orders.
        /// </summary>
        /// <returns>thhe data.</returns>
        private async Task<Tuple<List<UserOrderModel>, List<int>, DateTime>> GetUserOrdersToLook()
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
        private async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProductsToLook(List<int> magistralIds)
        {
            var lineProductsResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetLineProductPedidos, magistralIds);
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var listProductToReturn = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).ToList();
            listProductToReturn.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).ToList());
            var listIdToIgnore = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && ServiceConstants.StatusToIgnoreLineProducts.Contains(x.StatusAlmacen)).Select(y => y.SaleOrderId).ToList();

            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        /// <summary>
        /// Gets the sap orders to look.
        /// </summary>
        /// <param name="types">the types.</param>
        /// <param name="userOrdersTuple">the user order tuple.</param>
        /// <param name="lineProductTuple">the line product tuple.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, int>> GetSapLinesToLook(List<string> types, Tuple<List<UserOrderModel>, List<int>, DateTime> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple)
        {
            var lineProducts = (await this.sapDao.GetAllLineProducts()).Select(x => x.ProductoId).ToList();

            var sapOrders = await ServiceUtilsAlmacen.GetSapOrderForRecepcionPedidos(this.sapDao, userOrdersTuple, lineProductTuple);
            var orderHeaders = (await this.sapDao.GetFabOrderBySalesOrderId(sapOrders.Select(x => x.DocNum).ToList())).ToList();

            var possibleIdsToIgnore = sapOrders.Where(x => !orderHeaders.Any(y => y.PedidoId.Value == x.DocNum)).ToList();
            var idsToTake = possibleIdsToIgnore.GroupBy(x => x.DocNum).Where(y => !y.All(z => lineProducts.Contains(z.Detalles.ProductoId))).Select(a => a.Key).ToList();
            sapOrders = sapOrders.Where(x => !idsToTake.Contains(x.DocNum)).ToList();
            var granTotal = sapOrders.Select(x => x.DocNum).Distinct().ToList().Count;

            var listHeaderToReturn = ServiceUtilsAlmacen.GetSapOrderByType(types, sapOrders, orderHeaders, lineProducts);

            return new Tuple<List<CompleteAlmacenOrderModel>, int>(listHeaderToReturn, granTotal);
        }

        /// <summary>
        /// Gets the sap orders by status.
        /// </summary>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="userModels">the models.</param>
        /// <param name="lineProducts">the user orders.</param>
        /// <param name="parameters">the params.</param>
        /// <returns>the produtcs.</returns>
        private List<CompleteAlmacenOrderModel> GetSapLinesToLookByStatus(List<CompleteAlmacenOrderModel> sapOrders, List<UserOrderModel> userModels, List<LineProductsModel> lineProducts, List<string> parameters)
        {
            var listToReturn = new List<CompleteAlmacenOrderModel>();

            if (parameters.Contains(ServiceConstants.Recibir))
            {
                var allIds = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Salesorderid)).ToList();
                allIds.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode)).Select(y => y.SaleOrderId));

                var idsToLook = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid) && x.Status == ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen)).Select(y => int.Parse(y.Salesorderid)).ToList();
                idsToLook.AddRange(lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).Select(y => y.SaleOrderId));
                idsToLook.AddRange(sapOrders.Where(x => !allIds.Contains(x.DocNum)).Select(y => y.DocNum));
                listToReturn.AddRange(sapOrders.Where(x => idsToLook.Contains(x.DocNum)));
            }

            if (parameters.Contains(ServiceConstants.Pendiente))
            {
                var idsPendiente = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid) && x.Status != ServiceConstants.Finalizado && !ServiceConstants.StatusToIgnorePorRecibir.Contains(x.StatusAlmacen)).Select(y => int.Parse(y.Salesorderid)).ToList();
                listToReturn.AddRange(sapOrders.Where(x => idsPendiente.Contains(x.DocNum)));
            }

            if (parameters.Contains(ServiceConstants.BackOrder))
            {
                var idsBackOrder = userModels.Where(x => string.IsNullOrEmpty(x.Productionorderid) && x.StatusAlmacen == ServiceConstants.BackOrder).Select(y => int.Parse(y.Salesorderid)).ToList();
                listToReturn.AddRange(sapOrders.Where(x => idsBackOrder.Contains(x.DocNum)));
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the order by the chips criteria.
        /// </summary>
        /// <param name="sapOrders">the orders.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<CompleteAlmacenOrderModel> GetSapLinesToLookByPedidoDoctor(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                return sapOrders;
            }

            if (int.TryParse(parameters[ServiceConstants.Chips], out int pedidoId))
            {
                return sapOrders.Where(x => x.DocNum == pedidoId).ToList();
            }

            var listNames = parameters[ServiceConstants.Chips].Split(",").ToList();
            return sapOrders.Where(x => listNames.All(y => x.Medico.ToLower().Contains(y.ToLower()))).ToList();
        }

        /// <summary>
        /// Gets the data structure.
        /// </summary>
        /// <param name="userOrders">The user orders.</param>
        /// <param name="sapOrders">the Sap orders.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersModel> GetOrdersToReturn(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, List<LineProductsModel> lineProducts, Dictionary<string, string> parameters)
        {
            var sapOrdersToProcess = this.GetOrdersToProcess(sapOrders, parameters);
            var salesIds = sapOrdersToProcess.Select(x => x.DocNum).Distinct().ToList();
            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = salesIds.Count,
            };

            var almacenResponse = await this.almacenService.PostAlmacenOrders(ServiceConstants.GetIncidents, salesIds);
            var incidents = JsonConvert.DeserializeObject<List<IncidentsModel>>(almacenResponse.Response.ToString());

            var localNeigbors = await ServiceUtils.GetLocalNeighbors(this.catalogsService);

            var productsIds = sapOrders.Where(x => salesIds.Contains(x.DocNum)).Select(y => y.Detalles.ProductoId).Distinct().ToList();
            var productItems = (await this.sapDao.GetProductByIds(productsIds)).ToList();

            var batches = (await this.sapDao.GetBatchesByProdcuts(lineProducts.Select(x => x.ItemCode).ToList())).ToList();
            var saleordersDetails = (await this.sapDao.GetAllDetails(salesIds.Cast<int?>().ToList())).ToList();

            foreach (var so in salesIds)
            {
                var saleDetail = saleordersDetails.Where(x => x.PedidoId == so).ToList();
                var orders = sapOrdersToProcess.Where(x => x.DocNum == so).DistinctBy(y => y.Detalles.ProductoId).ToList();
                var order = orders.FirstOrDefault();

                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));
                var lineOrders = lineProducts.Where(x => x.SaleOrderId == so).ToList();

                var userProdOrders = userOrders.Count(x => x.Salesorderid.Equals(so.ToString()) && !string.IsNullOrEmpty(x.Productionorderid) && x.Status.Equals(ServiceConstants.Almacenado));
                var lineProductsCount = lineProducts.Count(x => x.SaleOrderId == so && !string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Almacenado);

                var totalAlmacenados = userProdOrders + lineProductsCount;

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctor = order == null ? string.Empty : order.Medico;
                var productList = this.GetProductListModel(userOrders, orders, saleDetail, lineProducts, incidents, productItems, batches);

                var salesStatusMagistral = userOrder != null && userOrder.Status.Equals(ServiceConstants.Finalizado) ? ServiceConstants.PorRecibir : ServiceConstants.Pendiente;
                salesStatusMagistral = userOrder != null && !string.IsNullOrEmpty(userOrder.StatusAlmacen) && userOrder.StatusAlmacen != ServiceConstants.Recibir ? userOrder.StatusAlmacen : salesStatusMagistral;
                salesStatusMagistral = salesStatusMagistral == ServiceConstants.Recibir ? ServiceConstants.PorRecibir : salesStatusMagistral;
                salesStatusMagistral = salesStatusMagistral == ServiceConstants.PorRecibir && productList.Any(y => y.Status == ServiceConstants.Pendiente) ? ServiceConstants.Pendiente : salesStatusMagistral;

                var salesStatusLinea = lineOrders.Any(x => x.DeliveryId != 0) ? ServiceConstants.BackOrder : ServiceConstants.PorRecibir;

                var salesStatus = userOrder != null ? salesStatusMagistral : salesStatusLinea;

                var client = order == null ? string.Empty : order.Cliente;
                var comments = userOrder == null ? string.Empty : userOrder.Comments;

                var productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;
                listToReturn.TotalItems += productList.Count;

                order.Address = string.IsNullOrEmpty(order.Address) ? string.Empty : order.Address;
                var invoiceType = ServiceUtils.CalculateTypeLocal(ServiceConstants.NuevoLeon, localNeigbors, order.Address) ? ServiceConstants.Local : ServiceConstants.Foraneo;

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = so,
                    Doctor = doctor,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = salesStatus,
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
                    TypeOrder = order.TypeOrder,
                    OrderMuestra = string.IsNullOrEmpty(order.PedidoMuestra) ? ServiceConstants.IsNotSampleOrder : order.PedidoMuestra,
                };

                var saleHeader = new AlmacenSalesHeaderModel
                {
                    Client = client,
                    DocNum = so,
                    Comments = comments,
                    Doctor = doctor,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = salesStatus,
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
                    TypeSaleOrder = $"Pedido {productType}",
                    OrderCounter = $"{totalAlmacenados}/{orders.Count}",
                    InvoiceType = invoiceType,
                    TypeOrder = order.TypeOrder,
                    OrderMuestra = string.IsNullOrEmpty(order.PedidoMuestra) ? ServiceConstants.IsNotSampleOrder : order.PedidoMuestra,
                    SapComments = order.Comments,
                };

                var saleModel = new SalesModel
                {
                    AlmacenHeader = saleHeader,
                    AlmacenSales = salesOrderModel,
                    Items = productList,
                };

                listToReturn.SalesOrders.Add(saleModel);
            }

            return listToReturn;
        }

        /// <summary>
        /// Gets the orders to process.
        /// </summary>
        /// <param name="sapOrders">the ordes.</param>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        private List<CompleteAlmacenOrderModel> GetOrdersToProcess(List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            sapOrders = sapOrders.OrderBy(x => x.DocNum).ToList();
            var pedidosId = sapOrders.Select(x => x.DocNum).Distinct().ToList();

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            pedidosId = pedidosId.Skip(offsetNumber).Take(limitNumber).ToList();

            return sapOrders.Where(x => pedidosId.Contains(x.DocNum)).ToList();
        }

        /// <summary>
        /// Gets the list of products.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapOrders">the sap orders.</param>
        /// <param name="detailsList">the detail List.</param>
        /// <param name="lineProductsModel">The lines products.</param>
        /// <param name="incidents">The incidents.</param>
        /// <param name="products">the products id.</param>
        /// <param name="batchesDataBase">The batches.</param>
        /// <returns>the products.</returns>
        private List<ProductListModel> GetProductListModel(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, List<CompleteDetailOrderModel> detailsList, List<LineProductsModel> lineProductsModel, List<IncidentsModel> incidents, List<ProductoModel> products, List<Batches> batchesDataBase)
        {
            var listToReturn = new List<ProductListModel>();
            foreach (var order in sapOrders)
            {
                var item = products.FirstOrDefault(x => order.Detalles.ProductoId == x.ProductoId);
                item ??= new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty };

                var fabOrder = detailsList.FirstOrDefault(x => x.CodigoProducto.Equals(order.Detalles.ProductoId));
                var orderId = fabOrder == null ? string.Empty : fabOrder.OrdenFabricacionId.ToString();

                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;
                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                var orderStatus = ServiceConstants.PorRecibir;
                var hasDelivery = false;
                var deliveryId = 0;
                var batches = new List<string>();

                if (item.IsMagistral.Equals("Y"))
                {
                    var userFabOrder = userOrders.FirstOrDefault(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid.Equals(orderId));
                    userFabOrder ??= new UserOrderModel { Status = ServiceConstants.Finalizado };
                    orderStatus = userFabOrder.Status == ServiceConstants.Finalizado ? ServiceConstants.PorRecibir : userFabOrder.Status;
                    orderStatus = orderStatus == ServiceConstants.PorRecibir && userFabOrder.FinishedLabel == 0 ? ServiceConstants.Pendiente : orderStatus;
                    orderStatus = userFabOrder.Status == ServiceConstants.Cancelado ? ServiceConstants.Cancelado : orderStatus;
                    hasDelivery = userFabOrder.DeliveryId != 0;
                    deliveryId = userFabOrder.DeliveryId;
                }
                else
                {
                    var userFabLineOrder = lineProductsModel.FirstOrDefault(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.Equals(item.ProductoId));
                    userFabLineOrder ??= new LineProductsModel { StatusAlmacen = ServiceConstants.PorRecibir };
                    orderStatus = !userFabLineOrder.StatusAlmacen.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabLineOrder.StatusAlmacen;
                    orderStatus = userFabLineOrder.StatusAlmacen == ServiceConstants.Cancelado ? ServiceConstants.Cancelado : orderStatus;
                    hasDelivery = userFabLineOrder.DeliveryId != 0;
                    deliveryId = userFabLineOrder.DeliveryId;

                    var batchObject = !string.IsNullOrEmpty(userFabLineOrder.BatchName) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(userFabLineOrder.BatchName) : new List<AlmacenBatchModel>();
                    batchObject.ForEach(y =>
                    {
                        var batch = batchesDataBase.FirstOrDefault(z => z.DistNumber == y.BatchNumber && z.ItemCode == item.ProductoId);
                        batch ??= new Batches();
                        var expirationDate = batch.ExpDate.HasValue ? batch.ExpDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        batches.Add($"{y.BatchNumber} | {(int)y.BatchQty} pz | Cad: {expirationDate}");
                    });
                }

                var incidentdb = incidents.FirstOrDefault(x => x.SaleOrderId == order.DocNum && x.ItemCode == item.ProductoId);
                incidentdb ??= new IncidentsModel();

                var localIncident = new IncidentInfoModel
                {
                    Batches = !string.IsNullOrEmpty(incidentdb.Batches) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(incidentdb.Batches) : new List<AlmacenBatchModel>(),
                    Comments = incidentdb.Comments,
                    Incidence = incidentdb.Incidence,
                    Status = incidentdb.Status,
                };

                var productModel = new ProductListModel
                {
                    Container = order.Detalles.Container,
                    Description = item.LargeDescription.ToUpper(),
                    ItemCode = itemcode,
                    NeedsCooling = item.NeedsCooling,
                    ProductType = $"Producto {productType}",
                    Pieces = order.Detalles.Quantity,
                    Status = orderStatus,
                    IsMagistral = item.IsMagistral.Equals("Y"),
                    Batches = batches,
                    Incident = string.IsNullOrEmpty(localIncident.Status) ? null : localIncident,
                    HasDelivery = hasDelivery,
                    DeliveryId = deliveryId,
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }
    }
}
