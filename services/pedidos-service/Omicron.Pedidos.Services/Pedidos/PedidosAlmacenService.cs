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
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for the almacen pedidos.
    /// </summary>
    public class PedidosAlmacenService : IPedidosAlmacenService
    {
        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosAlmacenService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        public PedidosAlmacenService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForAlmacen()
        {
            var response = await this.GetParametersDateToLook(ServiceConstants.AlmacenMaxDayToLook);
            var orders = await this.pedidosDao.GetSaleOrderForAlmacen(ServiceConstants.Finalizado, response.Item1);

            var ordersToIgnore = await this.pedidosDao.GetOrderForAlmacenToIgnore(ServiceConstants.Finalizado, response.Item1);
            var ordersId = ordersToIgnore.Where(x => x.IsSalesOrder).Select(y => int.Parse(y.Salesorderid)).ToList();

            var ordersToReturn = orders.Select(x => new
            {
                Salesorderid = x.Salesorderid,
                Productionorderid = x.Productionorderid,
                Status = x.Status,
                Comments = x.Comments,
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

            var orderToReturn = userOrders
                .Where(x => string.IsNullOrEmpty(x.StatusInvoice))
                .Select(x => new
                {
                    Salesorderid = x.Salesorderid,
                    Productionorderid = x.Productionorderid,
                    Status = x.Status,
                    StatusAlmacen = x.StatusAlmacen,
                    Comments = x.Comments,
                }).ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForInvoice()
        {
            var userOrders = (await this.pedidosDao.GetUserOrdersForInvoice(ServiceConstants.Almacenado, ServiceConstants.Empaquetado)).ToList();

            var orderToReturn = userOrders
                .Where(x => string.IsNullOrEmpty(x.StatusInvoice))
                .Select(x => new
                {
                    Salesorderid = x.Salesorderid,
                    Productionorderid = x.Productionorderid,
                    Status = x.Status,
                    StatusAlmacen = x.StatusAlmacen,
                })
                .ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersForPackages(Dictionary<string, string> parameters)
        {
            var dataToLook = await this.GetParametersDateToLook(ServiceConstants.SentMaxDaysToLook);
            var arrayStatus = parameters.ContainsKey(ServiceConstants.Status) ? parameters[ServiceConstants.Status].Split(",").ToList() : ServiceConstants.StatusLocal;
            var userOrders = (await this.pedidosDao.GetUserOrderByStatusInvoice(arrayStatus)).ToList();

            var ordersToLoop = userOrders.Where(x => !ServiceConstants.StatusDelivered.Contains(x.StatusInvoice)).ToList();
            ordersToLoop.AddRange(userOrders.Where(x => ServiceConstants.StatusDelivered.Contains(x.StatusInvoice) && x.InvoiceStoreDate >= dataToLook.Item1));

            var orderToReturn = ordersToLoop
                .Where(x => x.IsSalesOrder)
                .DistinctBy(y => y.InvoiceId)
                .Select(x => new
                {
                    InvoiceId = x.InvoiceId,
                    StatusAlmacen = x.StatusAlmacen,
                    InvoiceStoreDate = x.InvoiceStoreDate,
                    StatusInvoice = x.StatusInvoice,
                })
                .ToList();

            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, dataToLook.Item2);
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
