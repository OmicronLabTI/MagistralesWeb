// <summary>
// <copyright file="SapService.cs" company="Axity">
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
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;
    using Omicron.SapAdapter.Resources.Extensions;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Mapping;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapService : ISapService
    {
        private readonly ISapDao sapDao;

        private readonly IPedidosService pedidosService;

        private readonly IUsersService usersService;

        private readonly IConfiguration configuration;

        private readonly IGetProductionOrderUtils getProductionOrderUtils;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        /// <param name="pedidosService">the pedidosservice.</param>
        /// <param name="userService">user service.</param>
        /// <param name="configuration">App configuration.</param>
        /// <param name="logger">the logger.</param>
        /// <param name="getProductionOrderUtils">the getproduction order utisl.</param>
        public SapService(ISapDao sapDao, IPedidosService pedidosService, IUsersService userService, IConfiguration configuration, ILogger logger, IGetProductionOrderUtils getProductionOrderUtils)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
            this.usersService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger;
            this.getProductionOrderUtils = getProductionOrderUtils;
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <param name="parameters">The params.</param>
        /// <returns>get the orders.</returns>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var dateFilter = ServiceUtils.GetDateFilter(parameters);
            var orders = await this.GetSapDbOrders(parameters, dateFilter);

            var userOrderModel = await this.pedidosService.GetUserPedidos(orders.Select(x => x.DocNum).Distinct().ToList(), ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
            var listUsers = await this.GetUsers(userOrders);
            orders = ServiceUtils.FilterList(orders, parameters, userOrders, listUsers);

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var ordersOrdered = orders.OrderBy(o => o.DocNum).ToList();
            var orderToReturn = ordersOrdered.Skip(offsetNumber).Take(limitNumber).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, orderToReturn, null, orders.Count);
        }

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="docId">the doc id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetOrderDetails(int docId)
        {
            var details = await this.sapDao.GetAllDetails(docId);

            var usersOrderModel = await this.pedidosService.GetUserPedidos(new List<int> { docId }, ServiceConstants.GetUserSalesOrder);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(usersOrderModel.Response.ToString());

            var listUsers = await this.GetUsers(userOrders);

            var listToProcess = details.Where(y => y.OrdenFabricacionId == 0).ToList();
            listToProcess.AddRange(details.Where(y => y.OrdenFabricacionId != 0).DistinctBy(y => y.OrdenFabricacionId));
            listToProcess = listToProcess.OrderBy(x => x.OrdenFabricacionId).ThenBy(x => x.DescripcionProducto).ToList();

            foreach (var x in listToProcess)
            {
                var pedido = userOrders.FirstOrDefault(y => string.IsNullOrEmpty(y.Productionorderid) && y.Salesorderid == docId.ToString());
                var userOrder = userOrders.FirstOrDefault(y => y.Productionorderid == x.OrdenFabricacionId.ToString());
                userOrder = userOrder == null ? new UserOrderModel { Userid = string.Empty, Status = string.Empty } : userOrder;
                var userId = userOrder.Userid;
                var user = listUsers.FirstOrDefault(y => y.Id.Equals(userId));
                x.Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";

                x.Status = userOrder.Status;
                x.Status = x.Status.Equals(ServiceConstants.Proceso) ? ServiceConstants.EnProceso : x.Status;
                x.FechaOfFin = userOrder.FinishDate;
                x.PedidoStatus = pedido == null ? ServiceConstants.Abierto : pedido.Status;
                x.HasMissingStock = x.OrdenFabricacionId != 0 && (await this.sapDao.GetDetalleFormula(x.OrdenFabricacionId)).Any(y => y.Stock == 0);
                x.Comments = pedido == null ? null : pedido.Comments;
                x.Label = x.Label.ToLower().Equals(ServiceConstants.Personalizado.ToLower()) ? ServiceConstants.Personalizado : ServiceConstants.Generico;
                x.FinishedLabel = userOrder.FinishedLabel;
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listToProcess, null, null);
        }

        /// <summary>
        /// Gets the orders with their detail.
        /// </summary>
        /// <param name="pedidosIds">the detail.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetPedidoWithDetail(List<int> pedidosIds)
        {
            var listData = new List<OrderWithDetailModel>();

            foreach (var x in pedidosIds)
            {
                var data = new OrderWithDetailModel();
                var order = (await this.sapDao.GetOrdersById(x)).FirstOrDefault();
                var detail = await this.sapDao.GetAllDetails(x);

                var listToProcess = detail.Where(y => y.OrdenFabricacionId == 0).ToList();
                listToProcess.AddRange(detail.Where(y => y.OrdenFabricacionId != 0).DistinctBy(y => y.OrdenFabricacionId));

                data.Order = order;
                data.Detalle = listToProcess;
                listData.Add(data);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listData, null, null);
        }

        /// <summary>
        /// Gets the production orders bu produc and id.
        /// </summary>
        /// <param name="pedidosIds">list ids each elemente is orderId-producId.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetProdOrderByOrderItem(List<string> pedidosIds)
        {
            var result = new List<OrdenFabricacionModel>();

            foreach (var o in pedidosIds)
            {
                var data = o.Split("-");
                int.TryParse(data[0], out int pedidoId);

                var orders = await this.sapDao.GetProdOrderByOrderProduct(pedidoId, data[1]);
                result.AddRange(orders);
            }

            result = result.DistinctBy(x => x.OrdenId).ToList();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, JsonConvert.SerializeObject(result), null, null);
        }

        /// <summary>
        /// Gets the formula of the orden de fabricaion.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <param name="returnFirst">if it returns only the first.</param>
        /// <param name="returnDetails">Return the details.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetOrderFormula(List<int> listIds, bool returnFirst, bool returnDetails)
        {
            var ordenFab = (await this.sapDao.GetFabOrderById(listIds)).ToList();
            var listToReturn = new List<CompleteFormulaWithDetalle>();
            var dictUser = new Dictionary<int, string>();

            var result = await this.pedidosService.GetUserPedidos(ordenFab.Select(x => x.OrdenId).ToList(), ServiceConstants.GetUserOrders);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(result.Response.ToString());

            foreach (var o in ordenFab)
            {
                if (!dictUser.ContainsKey(o.User))
                {
                    var user = await this.sapDao.GetSapUserById(o.User);
                    dictUser.Add(user.UserId, user.UserName);
                }

                o.PedidoId = o.PedidoId.HasValue ? o.PedidoId : 0;
                var details = await this.GetDetailsByOrder(o.OrdenId);
                details = details.OrderBy(x => x.Description).ToList();

                var pedido = (await this.sapDao.GetPedidoById(o.PedidoId.Value)).FirstOrDefault(p => p.ProductoId == o.ProductoId);
                var item = (await this.sapDao.GetProductById(o.ProductoId)).FirstOrDefault();
                var userOrder = userOrders.FirstOrDefault(x => x.Productionorderid.Equals(o.OrdenId.ToString()));
                var comments = userOrder != null ? userOrder.Comments : string.Empty;
                var realEndDate = userOrder != null ? userOrder.CloseDate : string.Empty;

                var formulaDetalle = new CompleteFormulaWithDetalle
                {
                    IsChecked = false,
                    ProductionOrderId = o.OrdenId,
                    Code = o.ProductoId,
                    ProductDescription = item == null ? string.Empty : item.LargeDescription,
                    Type = ServiceConstants.DictStatusType.ContainsKey(o.Type) ? ServiceConstants.DictStatusType[o.Type] : o.Type,
                    Status = ServiceConstants.DictStatus.ContainsKey(o.Status) ? ServiceConstants.DictStatus[o.Status] : o.Status,
                    PlannedQuantity = o.Quantity,
                    Unit = o.Unit,
                    Warehouse = o.Wharehouse,
                    Number = o.PedidoId.Value,
                    FabDate = o.CreatedDate.Value.ToString("dd/MM/yyyy"),
                    DueDate = o.DueDate.HasValue ? o.DueDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    StartDate = o.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = o.PostDate.HasValue ? o.PostDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    User = dictUser[o.User],
                    Origin = ServiceConstants.DictStatusOrigin.ContainsKey(o.OriginType) ? ServiceConstants.DictStatusOrigin[o.OriginType] : o.OriginType,
                    BaseDocument = o.PedidoId.Value,
                    Client = o.CardCode,
                    CompleteQuantity = o.CompleteQuantity,
                    RealEndDate = realEndDate,
                    ProductLabel = pedido == null ? string.Empty : pedido.Label,
                    Container = pedido == null ? string.Empty : pedido.Container,
                    DestinyAddress = pedido == null ? string.Empty : pedido.DestinyAddress,
                    Comments = comments,
                    HasBatches = details.Any(x => x.HasBatches),
                    HasMissingStock = details.Any(y => y.Stock == 0),
                    Details = returnDetails ? details : new List<CompleteDetalleFormulaModel>(),
                };

                listToReturn.Add(formulaDetalle);
            }

            if (returnFirst)
            {
                return ServiceUtils.CreateResult(true, 200, null, listToReturn.FirstOrDefault(), null, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, listToReturn.Count);
        }

        /// <summary>
        /// Obtiene los nombres, email de asesores dada una lista de pedidos.
        /// </summary>
        /// <param name="salesOrder">the orderId list.</param>
        /// <returns>the object.</returns>
        public async Task<ResultModel> GetAsesorsByOrderId(List<OrderIdModel> salesOrder)
        {
            var asesors = new List<object>();
            foreach (var order in salesOrder)
            {
                var asesor = (await this.sapDao.GetAsesorWithEmailByIds(order.OrderId)).FirstOrDefault();

                if (!ReferenceEquals(null, asesor))
                {
                    asesors.Add(new
                    {
                        asesorId = asesor.AsesorId,
                        firstName = asesor.FirstName,
                        lastName = asesor.LastName,
                        email = string.IsNullOrEmpty(asesor.Email) ? string.Empty : asesor.Email,
                        orderId = order.OrderId,
                    });
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, asesors, null, asesors.Count);
        }

        /// <summary>
        /// Get fabrication orders by criterial.
        /// </summary>
        /// <param name="salesOrderIds">Sales order ids.</param>
        /// <param name="fabricationOrderIds">Fabrication order ids.</param>
        /// <param name="components">Flag for get components.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetFabricationOrdersByCriterial(List<int> salesOrderIds, List<int> fabricationOrderIds, bool components)
        {
            var results = new List<CompleteFormulaWithDetalle>();
            var fabricationOrders = (await this.sapDao.GetFabOrderById(fabricationOrderIds)).ToList();
            fabricationOrders.AddRange(await this.sapDao.GetFabOrderBySalesOrderId(salesOrderIds));
            fabricationOrders = fabricationOrders.Where(x => !string.IsNullOrEmpty(x.Status)).Distinct().ToList();

            var resultUserOrders = await this.pedidosService.GetUserPedidos(fabricationOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.GetUserOrders);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(resultUserOrders.Response.ToString());

            foreach (var fabricationOrder in fabricationOrders)
            {
                var fabOrderComponents = components ? (await this.GetDetailsByOrder(fabricationOrder.OrdenId)) : new List<CompleteDetalleFormulaModel>();
                var userOrder = userOrders.FirstOrDefault(x => x.Productionorderid.Equals(fabricationOrder.OrdenId.ToString()));

                var fabOrderWithFormula = new CompleteFormulaWithDetalle();
                fabOrderWithFormula.Map(fabricationOrder);
                fabOrderWithFormula.Map(fabOrderComponents);
                fabOrderWithFormula.Map(userOrder);

                results.Add(fabOrderWithFormula);
            }

            return ServiceUtils.CreateResult(true, 200, null, results, null, results.Count);
        }

        /// <summary>
        /// gets the items from the dict.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetComponents(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                throw new CustomServiceException(ServiceConstants.NoChipsError);
            }

            var listValues = new List<CompleteDetalleFormulaModel>();
            var chipValues = parameters[ServiceConstants.Chips].Split(ServiceConstants.ChipSeparator).ToList();
            chipValues = ServiceUtils.UndecodeSpecialCaracters(chipValues);

            var firstChip = chipValues.FirstOrDefault().ToLower();
            listValues.AddRange((await this.sapDao.GetItemsByContainsItemCode(firstChip)).ToList());
            listValues.AddRange((await this.sapDao.GetItemsByContainsDescription(firstChip)).ToList());
            listValues = listValues.DistinctBy(p => p.ProductId).ToList();

            foreach (var v in chipValues)
            {
                listValues = listValues.Where(x => $"{x.ProductId} {x.Description}".ToLower().Contains(v.ToLower())).ToList();
            }

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var produtOrdered = listValues.OrderBy(o => o.ProductId).ToList();
            var productToReturn = produtOrdered.Skip(offsetNumber).Take(limitNumber).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, productToReturn, null, produtOrdered.Count);
        }

        /// <summary>
        /// Get products management by batches with criterials.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetProductsManagmentByBatch(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Chips))
            {
                throw new CustomServiceException(ServiceConstants.NoChipsError);
            }

            var chipValues = parameters[ServiceConstants.Chips].Split(ServiceConstants.ChipSeparator).ToList();
            var items = await this.sapDao.GetProductsManagmentByBatch(chipValues);

            parameters.TryGet<string, string, int>(ServiceConstants.Offset, 0, out int offset);
            parameters.TryGet<string, string, int>(ServiceConstants.Limit, 1, out int limit);

            var itemsToReturn = items.Skip(offset).Take(limit).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, itemsToReturn, null, items.Count);
        }

        /// <summary>
        /// Get the components managed by batches.
        /// </summary>
        /// <param name="ordenId">the ordenid.</param>
        /// <returns>the data to return.</returns>
        public async Task<ResultModel> GetBatchesComponents(int ordenId)
        {
            var componentes = (await this.sapDao.GetComponentByBatches(ordenId)).ToList();
            var listToReturn = new List<BatchesComponentModel>();

            foreach (var x in componentes)
            {
                double.TryParse(x.PendingQuantity.ToString(), out double totalNecesario);

                var lotes = await this.GetValidBatches(x.ProductId, x.Warehouse);
                var batches = await this.GetTransacitionBatches(ordenId, x.ProductId, lotes);

                var totalBatches = batches.Any() ? batches.Sum(y => y.CantidadSeleccionada) : 0;
                double.TryParse(totalBatches.ToString(), out var doubleTotalBathches);

                totalNecesario = Math.Round(totalNecesario, 6);
                doubleTotalBathches = Math.Round(doubleTotalBathches, 6);

                listToReturn.Add(new BatchesComponentModel
                {
                    Almacen = x.Warehouse,
                    CodigoProducto = x.ProductId,
                    DescripcionProducto = x.Description,
                    TotalNecesario = Math.Round(totalNecesario - doubleTotalBathches, 6),
                    TotalSeleccionado = doubleTotalBathches,
                    Lotes = lotes,
                    LotesAsignados = batches,
                });
            }

            listToReturn = listToReturn.OrderBy(x => x.DescripcionProducto).ToList();

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetlLastIsolatedProductionOrderId(string productId, string uniqueId)
        {
            var lastId = await this.sapDao.GetlLastIsolatedProductionOrderId(productId, uniqueId);
            return ServiceUtils.CreateResult(true, 200, null, lastId, null, null);
        }

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetNextBatchCode(string productCode)
        {
            var max = 0;
            var batchCodePrefix = this.configuration["SapOmicron:BatchCodes:prefix"];
            var batchCodeNumberPositions = int.Parse(this.configuration["SapOmicron:BatchCodes:numberPositions"]);
            var batchCodePattern = batchCodePrefix.Concat("[0-9]", batchCodeNumberPositions);
            var maxBatchCode = await this.sapDao.GetMaxBatchCode(batchCodePattern, productCode);

            if (!string.IsNullOrEmpty(maxBatchCode))
            {
                var startIndex = maxBatchCode.IndexOf('-') + 1;
                var endIndex = maxBatchCode.Length;
                var codeNumber = maxBatchCode.Substring(startIndex, endIndex - startIndex);
                max = int.Parse(codeNumber);
            }

            max += 1;

            var nextCode = $"{batchCodePrefix}{max.ToString().PadLeft(batchCodeNumberPositions, '0')}";
            return ServiceUtils.CreateResult(true, 200, null, nextCode, null, null);
        }

        /// <summary>
        /// Validate if exists batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <param name="batchCode">the batch code.</param>
        /// <returns>the validation result.</returns>
        public async Task<ResultModel> ValidateIfExistsBatchCodeByItemCode(string productCode, string batchCode)
        {
            var existingBatchCode = await this.sapDao.GetBatchCode(productCode, batchCode);
            return ServiceUtils.CreateResult(true, 200, null, !string.IsNullOrEmpty(existingBatchCode), null, null);
        }

        /// <summary>
        /// Gets the ordersby the filter.
        /// </summary>
        /// <param name="orderFabModel">the params.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetFabOrders(GetOrderFabModel orderFabModel)
        {
            var dateFilter = ServiceUtils.GetDateFilter(orderFabModel.Filters);

            if (orderFabModel.Filters.ContainsKey(ServiceConstants.Qfb) ||
                orderFabModel.Filters.ContainsKey(ServiceConstants.Status) ||
                orderFabModel.Filters.ContainsKey(ServiceConstants.FechaFin))
            {
                var orders = (await this.sapDao.GetFabOrderById(orderFabModel.OrdersId)).ToList();
                orders = this.getProductionOrderUtils.GetSapLocalProdOrders(orderFabModel.Filters, dateFilter, orders).OrderBy(x => x.OrdenId).ToList();
                var orderCount = orders.Count;
                orders = this.ApplyOffsetLimit(orders, orderFabModel.Filters);
                orders = orderFabModel.Filters.ContainsKey(ServiceConstants.NeedsLargeDsc) ? await this.getProductionOrderUtils.CompleteOrder(orders) : orders;
                return ServiceUtils.CreateResult(true, 200, null, orders, null, orderCount);
            }

            this.logger.Information("Busqueda por filtros");
            var dataBaseOrders = (await this.getProductionOrderUtils.GetSapDbProdOrders(orderFabModel.Filters, dateFilter)).OrderBy(x => x.OrdenId).ToList();
            this.logger.Information("Consulta por filtros exitosa");
            var total = dataBaseOrders.Count;

            var ordersToReturn = this.ApplyOffsetLimit(dataBaseOrders, orderFabModel.Filters);
            ordersToReturn = orderFabModel.Filters.ContainsKey(ServiceConstants.NeedsLargeDsc) ? await this.getProductionOrderUtils.CompleteOrder(ordersToReturn) : ordersToReturn;
            return ServiceUtils.CreateResult(true, 200, null, ordersToReturn, null, total);
        }

        /// <summary>
        /// Gets the orderd by id.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetFabOrdersById(List<int> ordersId)
        {
            var orders = (await this.sapDao.GetFabOrderById(ordersId)).ToList();

            return ServiceUtils.CreateResult(true, 200, null, orders, null, null);
        }

        /// <summary>
        /// Gets the recipes.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetRecipe(int orderId)
        {
            var order = (await this.sapDao.GetOrdersById(orderId)).ToList();
            var atcEntries = order.Where(x => x.AtcEntry.HasValue).Select(x => x.AtcEntry.Value).ToList();
            var modelToReturn = new List<OrderRecipeModel>();

            if (!atcEntries.Any())
            {
                return ServiceUtils.CreateResult(true, 200, ServiceConstants.NoRecipes, modelToReturn, null, null);
            }

            var attachments = await this.sapDao.GetAttachmentsById(atcEntries);
            var baseRoute = this.configuration["OmicronRecipeAddress"];

            foreach (var r in attachments)
            {
                var attachment = new OrderRecipeModel { Order = orderId };
                if (string.IsNullOrEmpty(r.TargetPath) || string.IsNullOrEmpty(r.FileName) || string.IsNullOrEmpty(r.FileExt))
                {
                    continue;
                }

                var fileName = $"{r.FileName}.{r.FileExt}";
                var pathArray = r.TargetPath.Split(@"\").Where(x => x.ToUpper() != "C:").ToList();
                var completePath = new StringBuilder();
                completePath.Append(baseRoute);
                pathArray.ForEach(x => completePath.Append($"{x}/"));
                completePath.Append(fileName);
                attachment.Recipe = completePath.ToString();
                modelToReturn.Add(attachment);
            }

            return ServiceUtils.CreateResult(true, 200, null, modelToReturn, null, null);
        }

        /// <summary>
        /// Gets the recipes.
        /// </summary>
        /// <param name="ordersId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetOriginalRouteRecipes(List<int> ordersId)
        {
            var order = (await this.sapDao.GetOrdersById(ordersId)).ToList();
            var atcEntries = order.Where(x => x.AtcEntry.HasValue).Select(x => x.AtcEntry.Value).ToList();
            var attachments = await this.sapDao.GetAttachmentsById(atcEntries);

            var modelToReturn = new List<OrderRecipeModel>();

            order.ForEach(o =>
            {
                attachments.Where(x => x.AbsEntry == o.AtcEntry).ToList().ForEach(x =>
                {
                    var modelAttach = new OrderRecipeModel
                    {
                        Order = o.PedidoId,
                        Recipe = x == null ? string.Empty : x.CompletePath,
                    };

                    modelToReturn.Add(modelAttach);
                });
            });

            return ServiceUtils.CreateResult(true, 200, null, modelToReturn, null, null);
        }

        /// <summary>
        /// Validates if the order is ready to be finished.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> ValidateOrder(int orderId)
        {
            var listErrors = new List<OrderValidationResponse>();
            var listErrorsBatches = new OrderValidationResponse { Type = ServiceConstants.BatchesAreMissingError, ListItems = new List<string>() };
            var listErrorStock = new OrderValidationResponse { Type = ServiceConstants.MissingWarehouseStock, ListItems = new List<string>() };

            (await this.sapDao.GetDetalleFormula(orderId)).ToList().ForEach(x =>
            {
                if (x.WarehouseQuantity <= 0 || x.RequiredQuantity >= x.WarehouseQuantity)
                {
                    listErrorStock.ListItems.Add(x.ProductId);
                }
            });

            var resultBatches = await this.GetBatchesComponents(orderId);
            ((List<BatchesComponentModel>)resultBatches.Response).ForEach(x =>
            {
                if (!x.LotesAsignados.Any() || x.TotalNecesario > 0)
                {
                    listErrorsBatches.ListItems.Add(x.CodigoProducto);
                }
            });

            listErrorsBatches.ListItems = listErrorsBatches.ListItems.OrderBy(x => x).ToList();
            listErrorStock.ListItems = listErrorStock.ListItems.OrderBy(x => x).ToList();

            if (listErrorsBatches.ListItems.Any() || listErrorStock.ListItems.Any())
            {
                listErrors.Add(listErrorsBatches);
                listErrors.Add(listErrorStock);
                return ServiceUtils.CreateResult(false, 400, null, listErrors, null, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null, null);
        }

        /// <summary>
        /// gets the orders from sap.
        /// </summary>
        /// <param name="parameters">the filter from front.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <returns>teh orders.</returns>
        private async Task<List<CompleteOrderModel>> GetSapDbOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                int.TryParse(parameters[ServiceConstants.DocNum], out int docNum);
                return (await this.sapDao.GetAllOrdersById(docNum)).ToList();
            }

            if (parameters.ContainsKey(ServiceConstants.FechaInicio))
            {
                return (await this.sapDao.GetAllOrdersByFechaIni(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
            else
            {
                return (await this.sapDao.GetAllOrdersByFechaFin(dateFilter[ServiceConstants.FechaInicio], dateFilter[ServiceConstants.FechaFin])).ToList();
            }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="userOrders">the user order model.</param>
        /// <returns>the data.</returns>
        private async Task<List<UserModel>> GetUsers(List<UserOrderModel> userOrders)
        {
            var userIDs = userOrders.Where(x => !string.IsNullOrEmpty(x.Userid)).Select(x => x.Userid).Distinct().ToList();
            var users = await this.usersService.GetUsersById(userIDs, ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(users.Response.ToString());
        }

        /// <summary>
        /// Get the valid batches by component.
        /// </summary>
        /// <param name="item">the item.</param>
        /// <param name="almacen">the almacen.</param>
        /// <returns>the value.</returns>
        private async Task<List<ValidBatches>> GetValidBatches(string item, string almacen)
        {
            var listToReturn = new List<ValidBatches>();
            var product = (await this.sapDao.GetProductById(item)).FirstOrDefault();
            var batches = (await this.sapDao.GetValidBatches(product.ProductoId, almacen)).ToList();

            batches.ForEach(x =>
            {
                listToReturn.Add(new ValidBatches
                {
                    SysNumber = x.SysNumber,
                    NumeroLote = x.DistNumber,
                    CantidadAsignada = x.CommitQty,
                    CantidadDisponible = Math.Round(x.Quantity - x.CommitQty, 6),
                    FechaExp = x.FechaExp,
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Gets the batches assigned.
        /// </summary>
        /// <param name="orderId">the orde id.</param>
        /// <param name="itemCode">the item cide.</param>
        /// <param name="validBatches">The valid batches.</param>
        /// <returns>the batches.</returns>
        private async Task<List<AssignedBatches>> GetTransacitionBatches(int orderId, string itemCode, List<ValidBatches> validBatches)
        {
            var listToReturn = new List<AssignedBatches>();
            var batchTransactions = (await this.sapDao.GetBatchesTransactionByOrderItem(itemCode, orderId)).ToList();
            var lastTransaction = batchTransactions.Any() ? batchTransactions.OrderBy(x => x.LogEntry).Last(y => y.DocQuantity > 0) : null;

            if (lastTransaction == null)
            {
                return new List<AssignedBatches>();
            }

            var batchesQty = (await this.sapDao.GetBatchTransationsQtyByLogEntry(lastTransaction.LogEntry)).ToList();

            batchesQty.ForEach(x =>
            {
                var batch = validBatches.FirstOrDefault(y => y.SysNumber == x.SysNumber);
                listToReturn.Add(new AssignedBatches
                {
                    CantidadSeleccionada = x.AllocQty,
                    NumeroLote = batch == null ? string.Empty : batch.NumeroLote,
                    SysNumber = x.SysNumber,
                });
            });

            return listToReturn;
        }

        /// <summary>
        /// Applies the offset.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <param name="parameters">the dic.</param>
        /// <returns>the data.</returns>
        private List<OrdenFabricacionModel> ApplyOffsetLimit(List<OrdenFabricacionModel> orders, Dictionary<string, string> parameters)
        {
            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            return orders.Skip(offsetNumber).Take(limitNumber).ToList();
        }

        /// <summary>
        /// Gets if the details has batches.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        private async Task<List<CompleteDetalleFormulaModel>> GetDetailsByOrder(int orderId)
        {
            var details = (await this.sapDao.GetDetalleFormula(orderId)).ToList();

            foreach (var detail in details)
            {
                detail.HasBatches = (await this.GetTransacitionBatches(orderId, detail.ProductId, new List<ValidBatches>())).Any();
            }

            return details;
        }
    }
}