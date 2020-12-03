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

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenService"/> class.
        /// </summary>
        /// <param name="sapDao">the sap dao.</param>
        /// <param name="pedidosService">the pedidos service.</param>
        public SapAlmacenService(ISapDao sapDao, IPedidosService pedidosService)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
            this.pedidosService = pedidosService ?? throw new ArgumentNullException(nameof(pedidosService));
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetOrders(Dictionary<string, string> parameters)
        {
            var userOrderModel = await this.pedidosService.GetUserPedidos(ServiceConstants.GetUserOrdersAlmancen);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(userOrderModel.Response.ToString());
            int.TryParse(userOrderModel.Comments.ToString(), out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));

            var sapOrders = (await this.sapDao.GetAllOrdersForAlmacen(dateToLook)).ToList();
            var listToReturn = await this.GetOrdersToReturn(userOrders, sapOrders, parameters);

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <summary>
        /// Gets the data of the magistral scanned data.
        /// </summary>
        /// <param name="code">the code.</param>
        /// <returns>the data.</returns>
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

        /// <summary>
        /// Gets the data of the line scanned bar.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetLineScannedData(string code)
        {
            var listBatchesModel = new List<LineProductBatchesModel>();

            // ToDo Get the code from items.
            var itemCode = (await this.sapDao.GetProductById("Linea1")).FirstOrDefault();
            var validBatches = (await this.sapDao.GetValidBatches(itemCode.ProductoId, ServiceConstants.PT)).ToList();
            var productType = itemCode.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

            validBatches.ForEach(b =>
            {
                var batch = new LineProductBatchesModel
                {
                    Batch = b.DistNumber,
                    ExpDate = b.FechaExp,
                    AvailableQuantity = Math.Round(b.Quantity - b.CommitQty, 6),
                };

                listBatchesModel.Add(batch);
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

        /// <summary>
        /// Gets the data structure.
        /// </summary>
        /// <param name="userOrders">The user orders.</param>
        /// <param name="sapOrders">the Sap orders.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        private async Task<AlmacenOrdersModel> GetOrdersToReturn(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, Dictionary<string, string> parameters)
        {
            var sapOrdersToProcess = this.GetOrdersToProcess(sapOrders, parameters);
            var salesIds = sapOrdersToProcess.Select(x => x.DocNum).Distinct().ToList();
            var listToReturn = new AlmacenOrdersModel
            {
                SalesOrders = new List<SalesModel>(),
                TotalItems = 0,
                TotalSalesOrders = 0,
            };

            foreach (var so in salesIds)
            {
                var saleDetail = (await this.sapDao.GetAllDetails(so)).ToList();
                var orders = sapOrdersToProcess.Where(x => x.DocNum == so).ToList();
                var order = orders.FirstOrDefault();

                var userOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(so.ToString()) && string.IsNullOrEmpty(x.Productionorderid));
                var userProdOrders = userOrders.Count(x => x.Salesorderid.Equals(so.ToString()) && !string.IsNullOrEmpty(x.Productionorderid) && x.Status.Equals(ServiceConstants.Almacenado));

                var totalItems = orders.Count;
                var totalpieces = orders.Where(y => y.Detalles != null).Sum(x => x.Detalles.Quantity);
                var doctor = order == null ? string.Empty : order.Medico;
                var salesStatus = userOrder == null || userOrder.Status.Equals(ServiceConstants.Finalizado) ? ServiceConstants.PorRecibir : userOrder.Status;
                var client = order == null ? string.Empty : order.Cliente;
                var comments = userOrder == null ? string.Empty : userOrder.Comments;

                var productList = await this.GetProductListModel(userOrders, orders, saleDetail);

                var productType = productList.All(x => x.IsMagistral) ? ServiceConstants.Magistral : ServiceConstants.Mixto;
                productType = productList.All(x => !x.IsMagistral) ? ServiceConstants.Linea : productType;

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
                    OrderCounter = $"{userProdOrders}/{orders.Count}",
                };

                listToReturn.TotalSalesOrders++;
                listToReturn.TotalItems += productList.Count;

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
            sapOrders = sapOrders.OrderBy(x => x.FechaInicio).ToList();
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
        /// <returns>the products.</returns>
        private async Task<List<ProductListModel>> GetProductListModel(List<UserOrderModel> userOrders, List<CompleteAlmacenOrderModel> sapOrders, List<CompleteDetailOrderModel> detailsList)
        {
            var listToReturn = new List<ProductListModel>();
            foreach (var order in sapOrders)
            {
                var item = (await this.sapDao.GetProductById(order.Detalles.ProductoId)).FirstOrDefault();
                item = item == null ? new ProductoModel() : item;

                var fabOrder = detailsList.FirstOrDefault(x => x.CodigoProducto.Equals(order.Detalles.ProductoId));
                var orderId = fabOrder == null ? string.Empty : fabOrder.OrdenFabricacionId.ToString();

                var itemcode = !string.IsNullOrEmpty(orderId) ? $"{item.ProductoId} - {orderId}" : item.ProductoId;
                var productType = item.IsMagistral.Equals("Y") ? ServiceConstants.Magistral : ServiceConstants.Linea;

                var orderStatus = ServiceConstants.PorRecibir;

                if (item.IsMagistral.Equals("Y"))
                {
                    var userFabOrder = userOrders.FirstOrDefault(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid.Equals(orderId));
                    orderStatus = userFabOrder == null || userFabOrder.Status.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabOrder.Status;
                }
                else
                {
                    var userFabLineOrder = userOrders.FirstOrDefault(x => x.Salesorderid.Equals(order.DocNum.ToString()) && x.ItemCode.Equals(item.ProductoId));
                    orderStatus = userFabLineOrder == null || userFabLineOrder.Status.Equals(ServiceConstants.Almacenado) ? orderStatus : userFabLineOrder.Status;
                }

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
                };

                listToReturn.Add(productModel);
            }

            return listToReturn;
        }
    }
}
