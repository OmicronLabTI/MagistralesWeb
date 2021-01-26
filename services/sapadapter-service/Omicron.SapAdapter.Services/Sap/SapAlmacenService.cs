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

            var dateToLook = await this.GetMaxDaysToLook();
            var userResponse = await this.GetUserOrdersToLook();
            var lineProducts = await this.GetLineProductsToLook();
            var sapOrders = await this.GetSapLinesToLook(dateToLook, types, userResponse, lineProducts);
            var totalFilter = sapOrders.Item1.Select(x => x.DocNum).Distinct().ToList().Count;
            var listToReturn = await this.GetOrdersToReturn(userResponse.Item1, sapOrders.Item1, lineProducts.Item1, parameters);

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, $"{sapOrders.Item2}-{totalFilter}");
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetMagistralScannedData(string code)
        {
            var codeArray = code.Split("-");
            int.TryParse(codeArray[0], out var pedidoId);
            int.TryParse(codeArray[1], out var orderId);

            var details = (await this.sapDao.GetAllDetails(pedidoId)).ToList();
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

            var validBatches = (await this.sapDao.GetValidBatches(itemCode.ProductoId, ServiceConstants.PT)).ToList();
            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

            validBatches.ForEach(b =>
            {
                var batchDate = b.FechaExp == null ? DateTime.Now.ToString("dd/MM/yyyy") : b.FechaExp;
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
        public async Task<ResultModel> GetDeliveryBySaleOrderId(List<int> ordersId)
        {
            var data = (await this.sapDao.GetDeliveryBySaleOrder(ordersId)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, data, null, null);
        }

        /// <summary>
        /// Gets the max days to look.
        /// </summary>
        /// <returns>the datetime.</returns>
        private async Task<DateTime> GetMaxDaysToLook()
        {
            var routeParamas = $"{ServiceConstants.GetParams}?{ServiceConstants.SentMaxDays}={ServiceConstants.SentMaxDays}";
            var catalogResponse = await this.catalogsService.GetParams(routeParamas);
            var catalog = JsonConvert.DeserializeObject<List<ParametersModel>>(catalogResponse.Response.ToString());
            var catalogValue = catalog.FirstOrDefault();
            catalogValue ??= new ParametersModel { Value = "10" };

            int.TryParse(catalogValue.Value, out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            return new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));
        }

        /// <summary>
        /// Gets the orders that are finalized and all the productin orders.
        /// </summary>
        /// <returns>thhe data.</returns>
        private async Task<Tuple<List<UserOrderModel>, List<int>>> GetUserOrdersToLook()
        {
            var userOrderModel = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());

            var listIds = JsonConvert.DeserializeObject<List<int>>(userOrderModel.ExceptionMessage);
            return new Tuple<List<UserOrderModel>, List<int>>(userOrders, listIds);
        }

        /// <summary>
        /// Gets the product lines to look and ignore.
        /// </summary>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<LineProductsModel>, List<int>>> GetLineProductsToLook()
        {
            var lineProductsResponse = await this.almacenService.GetAlmacenOrders(ServiceConstants.GetLineProduct);
            var lineProducts = JsonConvert.DeserializeObject<List<LineProductsModel>>(lineProductsResponse.Response.ToString());

            var listProductToReturn = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen != ServiceConstants.Almacenado).ToList();
            listProductToReturn.AddRange(lineProducts.Where(x => !string.IsNullOrEmpty(x.ItemCode)).ToList());
            var listIdToIgnore = lineProducts.Where(x => string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Almacenado).Select(y => y.SaleOrderId).ToList();

            return new Tuple<List<LineProductsModel>, List<int>>(lineProducts, listIdToIgnore);
        }

        /// <summary>
        /// Gets the sap orders to look.
        /// </summary>
        /// <param name="dateToLook">the date max to look.</param>
        /// <param name="types">the types.</param>
        /// <param name="userOrdersTuple">the user order tuple.</param>
        /// <param name="lineProductTuple">the line product tuple.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<List<CompleteAlmacenOrderModel>, int>> GetSapLinesToLook(DateTime dateToLook, List<string> types, Tuple<List<UserOrderModel>, List<int>> userOrdersTuple, Tuple<List<LineProductsModel>, List<int>> lineProductTuple)
        {
            var listHeaderToReturn = new List<CompleteAlmacenOrderModel>();
            var idsToIgnore = userOrdersTuple.Item2;
            idsToIgnore.AddRange(lineProductTuple.Item2);

            var sapOrders = (await this.sapDao.GetAllOrdersForAlmacen(dateToLook)).ToList();
            sapOrders = sapOrders.Where(x => x.Detalles != null).ToList();
            sapOrders = sapOrders.Where(x => !idsToIgnore.Contains(x.DocNum)).ToList();

            var granTotal = sapOrders.Select(x => x.DocNum).Distinct().ToList().Count;
            var orderHeaders = (await this.sapDao.GetFabOrderBySalesOrderId(sapOrders.Select(x => x.DocNum).ToList())).ToList();
            var sapOrdersGroup = sapOrders.GroupBy(x => x.DocNum).ToList();

            if (types.Contains(ServiceConstants.Magistral.ToLower()))
            {
                var listMagistral = sapOrdersGroup.Where(x => x.Count() == orderHeaders.Where(y => y.PedidoId == x.Key).Count());
                var keys = listMagistral.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keys.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Mixto.ToLower()))
            {
                var listMixta = sapOrdersGroup.Where(x => x.Count() != orderHeaders.Where(y => y.PedidoId == x.Key).Count() && orderHeaders.Where(y => y.PedidoId == x.Key).Count() > 0);
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            if (types.Contains(ServiceConstants.Line))
            {
                var listMixta = sapOrdersGroup.Where(x => orderHeaders.Where(y => y.PedidoId == x.Key).Count() == 0);
                var keysMixta = listMixta.Select(x => x.Key).ToList();

                listHeaderToReturn.AddRange(sapOrders.Where(x => keysMixta.Contains(x.DocNum)));
            }

            return new Tuple<List<CompleteAlmacenOrderModel>, int>(listHeaderToReturn, granTotal);
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

            foreach (var so in salesIds)
            {
                var saleDetail = (await this.sapDao.GetAllDetails(so)).ToList();
                var orders = sapOrdersToProcess.Where(x => x.DocNum == so).ToList();
                var order = orders.FirstOrDefault();

                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));
                var lineOrder = lineProducts.FirstOrDefault(x => x.SaleOrderId == so && string.IsNullOrEmpty(x.ItemCode));

                var userProdOrders = userOrders.Count(x => x.Salesorderid.Equals(so.ToString()) && !string.IsNullOrEmpty(x.Productionorderid) && x.Status.Equals(ServiceConstants.Almacenado));
                var lineProductsCount = lineProducts.Count(x => x.SaleOrderId == so && !string.IsNullOrEmpty(x.ItemCode) && x.StatusAlmacen == ServiceConstants.Almacenado);

                var totalAlmacenados = userProdOrders + lineProductsCount;

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctor = order == null ? string.Empty : order.Medico;
                var salesStatus = userOrder == null || userOrder.Status.Equals(ServiceConstants.Finalizado) ? ServiceConstants.PorRecibir : userOrder.Status;
                salesStatus = lineOrder == null ? salesStatus : lineOrder.StatusAlmacen;
                var client = order == null ? string.Empty : order.Cliente;
                var comments = userOrder == null ? string.Empty : userOrder.Comments;

                var productList = await this.GetProductListModel(userOrders, orders, saleDetail, lineProducts, incidents);

                var productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;
                listToReturn.TotalItems += productList.Count;

                var salesOrderModel = new AlmacenSalesModel
                {
                    DocNum = so,
                    Doctor = doctor,
                    InitDate = order == null ? DateTime.Now : order.FechaInicio,
                    Status = salesStatus,
                    TotalItems = totalItems,
                    TotalPieces = totalpieces,
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
        /// <returns>the products.</returns>
        private async Task<List<ProductListModel>> GetProductListModel(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, List<CompleteDetailOrderModel> detailsList, List<LineProductsModel> lineProductsModel, List<IncidentsModel> incidents)
        {
            var listToReturn = new List<ProductListModel>();
            foreach (var order in sapOrders)
            {
                var item = (await this.sapDao.GetProductById(order.Detalles.ProductoId)).FirstOrDefault();
                item = item == null ? new ProductoModel { IsMagistral = "N", LargeDescription = string.Empty, ProductoId = string.Empty } : item;

                var fabOrder = detailsList.FirstOrDefault(x => x.CodigoProducto.Equals(order.Detalles.ProductoId));
                var orderId = fabOrder == null ? string.Empty : fabOrder.OrdenFabricacionId.ToString();

                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;
                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                var orderStatus = ServiceConstants.PorRecibir;
                var batches = new List<string>();

                if (item.IsMagistral.Equals("Y"))
                {
                    var userFabOrder = userOrders.FirstOrDefault(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid.Equals(orderId));
                    orderStatus = userFabOrder == null || !userFabOrder.Status.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabOrder.Status;
                }
                else
                {
                    var userFabLineOrder = lineProductsModel.FirstOrDefault(x => x.SaleOrderId == order.DocNum && !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.Equals(item.ProductoId));
                    orderStatus = userFabLineOrder == null || !userFabLineOrder.StatusAlmacen.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabLineOrder.StatusAlmacen;

                    var lineOrder = userFabLineOrder ??= new LineProductsModel();
                    var batchObject = !string.IsNullOrEmpty(lineOrder.BatchName) ? JsonConvert.DeserializeObject<List<AlmacenBatchModel>>(lineOrder.BatchName) : new List<AlmacenBatchModel>();
                    batchObject.ForEach(y => batches.Add(y.BatchNumber));
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
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }
    }
}
