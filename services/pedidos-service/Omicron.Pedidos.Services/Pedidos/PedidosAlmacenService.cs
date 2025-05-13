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
    using Omicron.Pedidos.Dtos.Models;
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
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.sapFileService = sapFileService.ThrowIfNull(nameof(sapFileService));
            this.configuration = configuration.ThrowIfNull(nameof(configuration));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForAlmacen()
        {
            var response = await this.GetParametersDateToLook(ServiceConstants.AlmacenMaxDayToLook);
            var orders = await this.pedidosDao.GetSaleOrderForAlmacen(ServiceConstants.Finalizado, response.Item1, ServiceConstants.StatuPendingAlmacen, ServiceConstants.Almacenado);
            orders = orders.DistinctBy(x => x.Id).ToList();

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

            return ServiceUtils.CreateResult(true, 200, null, ordersToReturn, null, response.Item2);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForAlmacen(List<int> idsToLook)
        {
            var listToReturn = new List<UserOrderModel>();
            var familyOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(idsToLook.Select(x => x.ToString()).ToList())).GroupBy(x => x.Salesorderid).ToList();

            foreach (var x in familyOrders)
            {
                var orders = x.Where(y => y.IsProductionOrder && y.Status != ServiceConstants.Cancelled).ToList();
                var productionStatus = x.Where(z => z.IsProductionOrder && (z.Status == ServiceConstants.Finalizado || z.Status == ServiceConstants.Almacenado)).ToList();
                var saleOrde = x.FirstOrDefault(y => y.IsSalesOrder);

                if (ServiceShared.CalculateAnd(saleOrde != null, saleOrde.Status == ServiceConstants.Finalizado, saleOrde.FinishedLabel == 1))
                {
                    listToReturn.AddRange(x.ToList());
                    continue;
                }

                if (productionStatus.Any() &&
                productionStatus.All(z => z.FinishedLabel == 1) &&
                orders.All(z => ServiceConstants.StatuPendingAlmacen.Contains(z.Status) &&
                !orders.All(z => z.Status == ServiceConstants.Almacenado)))
                {
                    listToReturn.AddRange(x.ToList());
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, "0");
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
                x.InvoiceLineNum = order.InvoiceLineNum;
                x.CloseSampleOrderId = order.CloseSampleOrderId;
            });

            await this.pedidosDao.UpdateUserOrders(dataBaseOrders);
            return ServiceUtils.CreateResult(true, 200, null, true, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForDelivery(Dictionary<string, string> parameters)
        {
            var (startDate, endDate) = this.GetyStartDateAndEndDateParameterWithFormat(parameters);
            var userOrders = (await this.pedidosDao.GetUserOrderForDelivery(
                    [ServiceConstants.Almacenado],
                    ServiceConstants.Empaquetado,
                    startDate,
                    endDate)).ToList();

            userOrders.AddRange((await this.pedidosDao.GetOnlySaleOrderBySaleId(userOrders.Select(x => x.Salesorderid).Distinct().ToList())).ToList());

            var orderToReturn = userOrders
                .Select(x => new
                {
                    x.Salesorderid,
                    x.Productionorderid,
                    x.Status,
                    x.StatusAlmacen,
                    x.Comments,
                    x.DeliveryId,
                });

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, orderToReturn.Count().ToString());
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForDelivery(List<int> deliveryIds)
        {
            var userOrders = (await this.pedidosDao.GetUserOrderByDeliveryId(deliveryIds)).ToList();

            if (!userOrders.Any(x => x.IsSalesOrder))
            {
                userOrders.AddRange(await this.pedidosDao.GetOnlySaleOrderBySaleId(userOrders.Select(x => x.Salesorderid).Distinct().ToList()));
            }

            var orderToReturn = userOrders
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

            var orderToReturn = userOrders
                .Select(x => new
                {
                    x.Salesorderid,
                    x.Productionorderid,
                    x.Status,
                    x.StatusAlmacen,
                    x.DeliveryId,
                    x.StatusInvoice,
                })
                .ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForPackages(Dictionary<string, string> parameters)
        {
            var (startDate, endDate) = this.GetyStartDateAndEndDateParameterWithFormat(parameters);
            var arrayStatus = parameters.ContainsKey(ServiceConstants.Status) ? parameters[ServiceConstants.Status].Split(",").ToList() : ServiceConstants.StatusLocal;
            var type = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Type, ServiceConstants.Local.ToLower());
            var stringInvoiceId = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.Chips, string.Empty);
            var isValidId = int.TryParse(stringInvoiceId, out int invoiceId);

            IEnumerable<UserOrderModel> userOrderByType;

            if (isValidId)
            {
                userOrderByType = await this.pedidosDao.GetUserOrderByInvoiceTypeAndId([type.ToLower()], invoiceId);
            }
            else
            {
                userOrderByType = await this.pedidosDao.GetUserOrderByInvoiceType(
                    [type.ToLower()],
                    startDate,
                    endDate,
                    arrayStatus);
            }

            var orderToReturn = userOrderByType
                .Select(x => new
                {
                    x.InvoiceId,
                    x.StatusAlmacen,
                    x.InvoiceStoreDate,
                    x.StatusInvoice,
                    x.UserInvoiceStored,
                    x.InvoiceLineNum,
                });

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
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
            var userOrdersComplete = (await this.pedidosDao.GetUserOrderBySaleOrder(orders.Select(x => x.Salesorderid).Distinct().ToList())).ToList();
            var objectToReturn = new
            {
                SaleOrderId = orders.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList(),
                UserOrders = userOrdersComplete,
            };

            return ServiceUtils.CreateResult(true, 200, null, objectToReturn, null, null);
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
                LocalPackageTotal = packagedOrders.GetInvoiceCount(ServiceConstants.Local.ToLower(), ServiceConstants.Empaquetado),
                LocalNotDeliveredTotal = packagedOrders.GetInvoiceCount(ServiceConstants.Local.ToLower(), ServiceConstants.NoEntregado),
                LocalAsignedTotal = packagedOrders.GetInvoiceCount(ServiceConstants.Local.ToLower(), ServiceConstants.Asignado),
                LocalInWayTotal = packagedOrders.GetInvoiceCount(ServiceConstants.Local.ToLower(), ServiceConstants.Camino),
                LocalDeliveredTotal = packagedOrders.GetInvoiceCount(ServiceConstants.Local.ToLower(), ServiceConstants.Entregado),
                ForeignPackageTotal = packagedOrders.GetInvoiceCount(ServiceConstants.ForaneoDb.ToLower(), ServiceConstants.Empaquetado),
                ForeignSentTotal = packagedOrders.GetInvoiceCount(ServiceConstants.ForaneoDb.ToLower(), ServiceConstants.Enviado),
                InvoiceIds = packagedOrders.Where(x => x.StatusInvoice == ServiceConstants.NoEntregado).DistinctBy(y => y.InvoiceId).Select(z => z.InvoiceId).ToList(),
                PackagesId = packagedOrders.Select(x => x.InvoiceId).ToList(),
            };

            return ServiceUtils.CreateResult(true, 200, null, model, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderByDeliveryOrder(List<int> deliveryIds)
        {
            var deliveries = await this.pedidosDao.GetUserOrderByDeliveryId(deliveryIds);
            return ServiceUtils.CreateResult(true, 200, null, deliveries, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderByInvoiceId(List<int> invoices)
        {
            var orders = (await this.pedidosDao.GetUserOrdersByInvoiceId(invoices)).ToList();
            var userOrdersComplete = (await this.pedidosDao.GetUserOrderBySaleOrder(orders.Select(x => x.Salesorderid).Distinct().ToList())).ToList();
            return ServiceUtils.CreateResult(true, 200, null, userOrdersComplete, null, null);
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

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrdersByInvoicesIds(List<int> invoicesIds)
        {
            var orders = (await this.pedidosDao.GetUserOrdersByInvoiceId(invoicesIds)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, orders, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForAlmacenByRangeDates(Dictionary<string, string> parameters)
        {
            var (startDate, endDate) = this.GetyStartDateAndEndDateParameterWithFormat(parameters);
            var orders = await this.pedidosDao.GetSaleOrderForAlmacenByRangeDates(
                startDate,
                endDate,
                ServiceConstants.StatuPendingAlmacen,
                ServiceConstants.Finalizado,
                ServiceConstants.Almacenado);

            var ordersToReturn = orders.DistinctBy(x => x.Id).Select(x => new
            {
                x.Salesorderid,
                x.Productionorderid,
                x.Status,
                x.Comments,
                x.DeliveryId,
                x.StatusAlmacen,
                x.FinishedLabel,
                x.TypeOrder,
            });

            return ServiceUtils.CreateResult(true, 200, null, ordersToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForInvoiceByRangeDates(Dictionary<string, string> parameters)
        {
            var (startDate, endDate) = this.GetyStartDateAndEndDateParameterWithFormat(parameters);
            var userOrders = await this.pedidosDao.GetUserOrdersForInvoiceByRangeDates(startDate, endDate, ServiceConstants.Almacenado, ServiceConstants.Empaquetado);
            return ServiceUtils.CreateResult(true, 200, null, userOrders, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrdersForInvoiceByDeliveryIds(List<int> deliveryIds)
        {
            var userOrders = await this.pedidosDao.GetUserOrdersForInvoiceByDeliveryIds(deliveryIds, ServiceConstants.Almacenado, ServiceConstants.Empaquetado);
            return ServiceUtils.CreateResult(true, 200, null, userOrders, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInfoPiecesByOrderId(InvoiceProductsDto dto)
        {
            var userOrders = await this.pedidosDao.GetUserOrdersByInvoiceIdAndLineNumber(dto.Invoices, dto.LineNumbers);
            var result = userOrders.ToList().Select(static x => new InvoiceLineProducts()
            {
                ItemCode = JsonConvert.DeserializeObject<MagistralQrModel>(x.MagistralQr).ItemCode,
                Quantity = (int)x.Quantity,
                InvoiceLineNum = x.InvoiceLineNum,
                Invoice = x.InvoiceId,
                SaleOrderId = int.Parse(x.Salesorderid),
            });
            return ServiceUtils.CreateResult(true, 200, null, result, null, null);
        }

        /// <summary>
        /// Gets the data by the field, used for datetimes.
        /// </summary>
        /// <param name="fieldToLook">the field.</param>
        /// <returns>the data.</returns>
        private async Task<Tuple<DateTime, string>> GetParametersDateToLook(string fieldToLook)
        {
            var parameters = await this.pedidosDao.GetParamsByFieldContains(fieldToLook);
            var days = parameters.FirstOrDefault() != null ? parameters.FirstOrDefault().Value : "15";

            int.TryParse(days, out var maxDays);
            var minDate = DateTime.Today.AddDays(-maxDays).ToString("dd/MM/yyyy").Split("/");
            var dateToLook = new DateTime(int.Parse(minDate[2]), int.Parse(minDate[1]), int.Parse(minDate[0]));
            return new Tuple<DateTime, string>(dateToLook, days);
        }

        private Tuple<DateTime, DateTime> GetyStartDateAndEndDateParameterWithFormat(Dictionary<string, string> parameters)
        {
            var startDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                             .ToUniversalDateTime().Date;

            var endDate = ServiceShared.GetDictionaryValueString(parameters, ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy))
                                      .ToUniversalDateTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }
    }
}
