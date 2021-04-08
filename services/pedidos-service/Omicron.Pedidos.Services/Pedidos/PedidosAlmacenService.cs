// <summary>
// <copyright file="PedidosAlmacenService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for the almacen pedidos.
    /// </summary>
    public class PedidosAlmacenService : IPedidosAlmacenService
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapFileService sapFileService;

        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosAlmacenService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapFileService">The sap file.</param>
        /// <param name="configuration">The configuration.</param>
        public PedidosAlmacenService(IPedidosDao pedidosDao, ISapFileService sapFileService, IConfiguration configuration)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapFileService = sapFileService ?? throw new ArgumentNullException(nameof(sapFileService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForAlmacen()
        {
            var response = await this.GetParametersDateToLook(ServiceConstants.AlmacenMaxDayToLook);
            var orders = await this.pedidosDao.GetSaleOrderForAlmacen(ServiceConstants.Finalizado, response.Item1, ServiceConstants.StatuPendingAlmacen, ServiceConstants.Almacenado);
            orders = orders.DistinctBy(x => x.Id).ToList();
            var ordersToIgnore = await this.pedidosDao.GetOrderForAlmacenToIgnore(ServiceConstants.Finalizado, response.Item1);

            var odersToLook = orders.Select(x => x.Salesorderid).Distinct().ToList();
            var possibleToIgnore = ordersToIgnore.Where(x => x.IsSalesOrder).Select(y => y.Salesorderid).ToList();
            var idsToIgnore = possibleToIgnore.Where(x => !odersToLook.Any(y => y == x)).ToList();
            var ordersId = ordersToIgnore.Where(x => idsToIgnore.Contains(x.Salesorderid)).Select(y => int.Parse(y.Salesorderid)).Distinct().ToList();

            var ordersToReturn = orders.Select(x => new
            {
                x.Salesorderid,
                x.Productionorderid,
                x.Status,
                x.Comments,
                x.DeliveryId,
                x.StatusAlmacen,
                x.FinishedLabel,
                x.TypeOrder,
            }).ToList();

            return ServiceUtils.CreateResult(true, 200, null, ordersToReturn, JsonConvert.SerializeObject(ordersId), response.Item2);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateUserOrders(List<UserOrderModel> listOrders)
        {
            var ids = listOrders.Select(x => x.Id).ToList();
            var dataBaseOrders = await this.pedidosDao.GetUserOrdersById(ids);

            dataBaseOrders.ForEach(x =>
            {
                var order = listOrders.FirstOrDefault(y => y.Id == x.Id);

                x.Status = order.Status;
                x.StatusAlmacen = order.StatusAlmacen;
                x.UserCheckIn = order.UserCheckIn;
                x.DateTimeCheckIn = order.DateTimeCheckIn;
                x.RemisionQr = order.RemisionQr;
                x.DeliveryId = order.DeliveryId;
                x.StatusInvoice = order.StatusInvoice;
                x.UserInvoiceStored = order.UserInvoiceStored;
                x.InvoiceStoreDate = order.InvoiceStoreDate;
                x.InvoiceQr = order.InvoiceQr;
                x.InvoiceId = order.InvoiceId;
                x.InvoiceType = order.InvoiceType;
            });

            await this.pedidosDao.UpdateUserOrders(dataBaseOrders);
            return ServiceUtils.CreateResult(true, 200, null, true, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForDelivery()
        {
            var userOrders = (await this.pedidosDao.GetUserOrderByStatus(new List<string> { ServiceConstants.Almacenado })).ToList();

            var listToAdd = new List<UserOrderModel>();

            userOrders.GroupBy(x => x.DeliveryId).ToList().ForEach(y =>
            {
                var products = y.Where(z => z.IsProductionOrder).ToList();

                if (!products.All(z => z.StatusAlmacen == ServiceConstants.Empaquetado))
                {
                    listToAdd.AddRange(products);
                }
            });

            var saleOrder = (await this.pedidosDao.GetUserOrderBySaleOrder(listToAdd.Select(x => x.Salesorderid).ToList())).ToList();
            listToAdd.AddRange(saleOrder.Where(x => x.IsSalesOrder));

            var orderToReturn = listToAdd
                .Select(x => new
                {
                    x.Salesorderid,
                    x.Productionorderid,
                    x.Status,
                    x.StatusAlmacen,
                    x.Comments,
                    x.DeliveryId,
                }).ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForInvoice()
        {
            var userOrders = (await this.pedidosDao.GetUserOrdersForInvoice(ServiceConstants.Almacenado, ServiceConstants.Empaquetado)).ToList();

            var orderToReturn = userOrders.Where(y => y.IsProductionOrder && string.IsNullOrEmpty(y.StatusInvoice))
                .Select(x => new
                {
                    x.Salesorderid,
                    x.Productionorderid,
                    x.Status,
                    x.StatusAlmacen,
                    x.DeliveryId,
                })
                .ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForPackages(Dictionary<string, string> parameters)
        {
            var dataToLook = await this.GetParametersDateToLook(ServiceConstants.SentMaxDaysToLook);
            var arrayStatus = parameters.ContainsKey(ServiceConstants.Status) ? parameters[ServiceConstants.Status].Split(",").ToList() : ServiceConstants.StatusLocal;
            var type = parameters.ContainsKey(ServiceConstants.Type) ? parameters[ServiceConstants.Type] : ServiceConstants.Local.ToLower();

            var userOrderByType = (await this.pedidosDao.GetUserOrderByInvoiceType(new List<string> { type })).ToList();
            userOrderByType = userOrderByType.Where(x => !string.IsNullOrEmpty(x.StatusInvoice)).ToList();

            var ordersToLoop = userOrderByType.Where(x => x.IsProductionOrder && !ServiceConstants.StatusDelivered.Contains(x.StatusInvoice)).ToList();
            var invoiceIDs = ordersToLoop.Where(x => x.IsProductionOrder).DistinctBy(y => y.InvoiceId).Select(z => z.InvoiceId).ToList();
            ordersToLoop.AddRange(userOrderByType.Where(x => x.IsProductionOrder && ServiceConstants.StatusDelivered.Contains(x.StatusInvoice) && x.InvoiceStoreDate >= dataToLook.Item1));

            var orderToReturn = ordersToLoop
                .Where(x => x.IsProductionOrder && arrayStatus.Contains(x.StatusInvoice))
                .DistinctBy(y => y.InvoiceId)
                .Select(x => new
                {
                    x.InvoiceId,
                    x.StatusAlmacen,
                    x.InvoiceStoreDate,
                    x.StatusInvoice,
                })
                .ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, JsonConvert.SerializeObject(invoiceIDs), dataToLook.Item2);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateSentOrders(List<UserOrderModel> userToUpdate)
        {
            var invoicesId = userToUpdate.Select(x => x.InvoiceId).ToList();
            var orders = (await this.pedidosDao.GetUserOrdersByInvoiceId(invoicesId)).ToList();

            orders.ForEach(x =>
            {
                var orderSent = userToUpdate.FirstOrDefault(y => y.InvoiceId == x.InvoiceId);
                orderSent ??= new UserOrderModel { StatusInvoice = ServiceConstants.Empaquetado };
                x.StatusInvoice = orderSent.StatusInvoice;
            });

            await this.pedidosDao.UpdateUserOrders(orders);

            return ServiceUtils.CreateResult(true, 200, null, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetAlmacenGraphData(Dictionary<string, string> parameters)
        {
            var dates = ServiceUtils.GetDictDates(parameters[ServiceConstants.FechaInicio]);

            var ordersByDates = (await this.pedidosDao.GetUserOrderByFinalizeDate(dates[ServiceConstants.FechaInicio], dates[ServiceConstants.FechaFin])).Where(x => !x.IsIsolatedProductionOrder).ToList();

            var idsSalesFinalized = ordersByDates.Where(x => x.IsSalesOrder && x.Status == ServiceConstants.Finalizado).DistinctBy(x => x.Salesorderid).Select(y => y.Salesorderid).ToList();
            var idsPossiblePending = ordersByDates.Where(x => x.IsProductionOrder && (x.Status == ServiceConstants.Finalizado || x.Status == ServiceConstants.Almacenado)).Select(y => y.Salesorderid).Distinct().ToList();

            var idsPending = idsPossiblePending.Where(x => !idsSalesFinalized.Any(y => y == x)).ToList();
            var ordersPending = (await this.pedidosDao.GetUserOrderBySaleOrder(idsPending)).GroupBy(x => x.Salesorderid).ToList();

            ordersPending = ordersPending.Where(x => !x.Any(y => y.StatusAlmacen == ServiceConstants.BackOrder)).ToList();
            ordersPending = ordersPending.Where(x => !x.All(y => y.StatusAlmacen == ServiceConstants.Almacenado)).ToList();
            ordersPending = ordersPending.Where(x => x.All(y => y.StatusAlmacen != ServiceConstants.Empaquetado)).ToList();
            ordersPending = ordersPending.Where(x => x.All(y => string.IsNullOrEmpty(y.StatusInvoice))).ToList();

            var packagedOrders = ordersByDates.Where(x => x.IsProductionOrder && !string.IsNullOrEmpty(x.InvoiceType)).DistinctBy(y => y.InvoiceId).ToList();
            var model = new AlmacenGraphicsCount
            {
                RecibirTotal = ordersByDates.Count(x => x.IsSalesOrder && x.Status == ServiceConstants.Finalizado && x.FinishedLabel == 1 && (string.IsNullOrEmpty(x.StatusAlmacen) || x.StatusAlmacen == ServiceConstants.Recibir)),
                AlmacenadosTotal = ordersByDates.Count(x => x.IsSalesOrder && x.Status == ServiceConstants.Almacenado),
                BackOrderTotal = ordersByDates.Count(x => x.IsSalesOrder && x.StatusAlmacen == ServiceConstants.BackOrder),
                PendienteTotal = ordersPending.Count(x => x.Any(y => y.IsProductionOrder && (y.Status == ServiceConstants.Finalizado || y.Status == ServiceConstants.Almacenado) && y.FinishedLabel == 1) && x.Where(z => z.IsProductionOrder).All(y => ServiceConstants.StatuPendingAlmacen.Contains(y.Status))),
                LocalPackageTotal = packagedOrders.Count(x => x.InvoiceType == ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Empaquetado),
                LocalNotDeliveredTotal = packagedOrders.Count(x => x.InvoiceType == ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.NoEntregado),
                LocalAsignedTotal = packagedOrders.Count(x => x.InvoiceType == ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Asignado),
                LocalInWayTotal = packagedOrders.Count(x => x.InvoiceType == ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Camino),
                LocalDeliveredTotal = packagedOrders.Count(x => x.InvoiceType == ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Entregado),
                ForeignPackageTotal = packagedOrders.Count(x => x.InvoiceType != ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Empaquetado),
                ForeignSentTotal = packagedOrders.Count(x => x.InvoiceType != ServiceConstants.Local.ToLower() && x.StatusInvoice == ServiceConstants.Enviado),
                InvoiceIds = packagedOrders.Where(x => x.StatusInvoice == ServiceConstants.NoEntregado).DistinctBy(y => y.InvoiceId).Select(z => z.InvoiceId).ToList(),
                PackagesId = packagedOrders.Select(x => x.InvoiceId).ToList(),
            };

            return ServiceUtils.CreateResult(true, 200, null, model, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderByDeliveryOrder(List<int> deliveryIds)
        {
            var deliveries = (await this.pedidosDao.GetUserOrderByDeliveryId(deliveryIds)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, deliveries, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreatePdf(string type, List<int> invoiceIds)
        {
            var listRoutes = new List<string>();
            var route = string.Format(ServiceConstants.CreatePdfByType, type);
            var responseFile = await this.sapFileService.PostSimple(invoiceIds, route);

            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFile.Response.ToString());
            ServiceUtils.GetValuesContains(dictResult, ServiceConstants.Ok)
            .ForEach(x =>
            {
                var targetPath = dictResult[x].Replace("Ok-", string.Empty);
                var baseRoute = this.configuration["OmicronFilesAddress"];

                var pathArray = targetPath.Split(@"\").Where(x => x.ToUpper() != "C:").ToList();
                var completePath = new StringBuilder();
                completePath.Append(baseRoute);
                pathArray.ForEach(x => completePath.Append($"{x}/"));
                var path = completePath.ToString().Remove(completePath.ToString().Length - 1);
                listRoutes.Add(path);
            });

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreatePdf);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            return ServiceUtils.CreateResult(true, 200, JsonConvert.SerializeObject(listErrorId), listRoutes, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> AdvanceLook(List<int> docNum)
        {
            var listString = docNum.Select(x => x.ToString()).ToList();

            var userOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(listString)).ToList();
            userOrders.AddRange(await this.pedidosDao.GetUserOrderByDeliveryId(docNum));
            userOrders.AddRange(await this.pedidosDao.GetUserOrdersByInvoiceId(docNum));

            return ServiceUtils.CreateResult(true, 200, null, userOrders, null);
        }

        /// <summary>
        /// Gets the data by the field, used for datetimes.
        /// </summary>
        /// <param name="fieldToLook">the field.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<DateTime, string>> GetParametersDateToLook(string fieldToLook)
        {
            var parameters = await this.pedidosDao.GetParamsByFieldContains(fieldToLook);
            var days = parameters.FirstOrDefault() != null ? parameters.FirstOrDefault().Value : "10";

            int.TryParse(days, out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));
            return new Tuple<DateTime, string>(dateToLook, days);
        }
    }
}
